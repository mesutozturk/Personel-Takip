using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using PT.BLL.AccountRepository;
using PT.BLL.Settings;
using PT.Entitiy.IdentityModel;
using PT.Entitiy.ViewModel;

namespace PT.Web.MVC.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            //kayıt olmadan önce kontrol ediliyor
            if (!ModelState.IsValid)
                return View(model);
            var userManager = MembershipTools.NewUserManager();
            var checkUser = userManager.FindByName(model.Username);
            if (checkUser != null)
            {
                ModelState.AddModelError(string.Empty, "Bu kullanıcı zaten kayıtlı!");
                return View(model);
            }
            // register işlemi yapılır
            var activationCode = Guid.NewGuid().ToString();
            ApplicationUser user = new ApplicationUser()
            {
                Name = model.Name,
                Surname = model.Surname,
                Email = model.Email,
                UserName = model.Username,
                ActivationCode = activationCode
            };
            var response = userManager.Create(user, model.Password);
            if (response.Succeeded)
            {
                string siteUrl = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host +
                                 (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                if (userManager.Users.Count() == 1)
                {
                    userManager.AddToRole(user.Id, "Admin");
                    await SiteSettings.SendMail(new MailModel
                    {
                        To = user.Email,
                        Subject = "Hoşgeldin Sahip",
                        Message = "Sitemizi yöneteceğin için çok mutluyuz ^^"
                    });
                }
                else
                {
                    userManager.AddToRole(user.Id, "Passive");
                    await SiteSettings.SendMail(new MailModel
                    {
                        To = user.Email,
                        Subject = "Personel Yönetimi - Aktivasyon",
                        Message =
                            $"Merhaba {user.Name} {user.Surname} <br/>Hesabınızı aktifleştirmek için <a href='{siteUrl}/Account/Activation?code={activationCode}'>Aktivasyon Kodu</a>"
                    });
                }

                return RedirectToAction("Login", "Account");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Kayıt İşleminde bir hata oluştu");
                return View(model);
            }
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var userManager = MembershipTools.NewUserManager();
            var user = await userManager.FindAsync(model.UserName, model.Password);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Böyle bir kullanıcı bulunamadı");
                return View(model);
            }
            var authManager = HttpContext.GetOwinContext().Authentication;
            var userIdentity = await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            authManager.SignIn(new AuthenticationProperties
            {
                IsPersistent = model.RememberMe
            }, userIdentity);
            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public ActionResult Logout()
        {
            var authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignOut();
            return RedirectToAction("Login", "Account");
        }

        public async Task<ActionResult> Activation(string code)
        {
            var userStore = MembershipTools.NewUserStore();
            var userManager = new UserManager<ApplicationUser>(userStore);
            var sonuc = userStore.Context.Set<ApplicationUser>().FirstOrDefault(x => x.ActivationCode == code);
            if (sonuc == null)
            {
                ViewBag.sonuc = "Aktivasyon işlemi  başarısız";
                return View();
            }
            sonuc.EmailConfirmed = true;
            await userStore.UpdateAsync(sonuc);
            await userStore.Context.SaveChangesAsync();

            userManager.RemoveFromRole(sonuc.Id, "Passive");
            userManager.AddToRole(sonuc.Id, "User");

            ViewBag.sonuc = $"Merhaba {sonuc.Name} {sonuc.Surname} <br/> Aktivasyon işleminiz başarılı";

            await SiteSettings.SendMail(new MailModel()
            {
                To = sonuc.Email,
                Message = ViewBag.sonuc.ToString(),
                Subject = "Aktivasyon",
                Bcc = "poyildirim@gmail.com"
            });

            return View();
        }

        public ActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RecoverPassword(string email)
        {
            var userStore = MembershipTools.NewUserStore();
            var userManager = new UserManager<ApplicationUser>(userStore);
            var sonuc = userStore.Context.Set<ApplicationUser>().FirstOrDefault(x => x.Email == email);
            if (sonuc == null)
            {
                ViewBag.sonuc = "E mail adresiniz sisteme kayıtlı değil";
                return View();
            }
            var randomPass = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 6);
            await userStore.SetPasswordHashAsync(sonuc, userManager.PasswordHasher.HashPassword(randomPass));
            await userStore.UpdateAsync(sonuc);
            await userStore.Context.SaveChangesAsync();

            await SiteSettings.SendMail(new MailModel()
            {
                To = sonuc.Email,
                Subject = "Şifreniz Değişti",
                Message = $"Merhaba {sonuc.Name} {sonuc.Surname} <br/>Yeni Şifreniz : <b>{randomPass}</b>"
            });
            ViewBag.sonuc = "Email adresinize yeni şifreniz gönderilmiştir";
            return View();
        }

        [Authorize]
        public ActionResult Profile()
        {
            var userManager = MembershipTools.NewUserManager();
            var user = userManager.FindById(HttpContext.GetOwinContext().Authentication.User.Identity.GetUserId());
            var model = new ProfileViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                UserName = user.UserName
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                var userStore = MembershipTools.NewUserStore();
                var userManager = new UserManager<ApplicationUser>(userStore);
                var user = userManager.FindById(model.Id);
                user.Name = model.Name;
                user.Surname = model.Surname;
                if (user.Email != model.Email)
                {
                    user.Email = model.Email;
                    if (HttpContext.User.IsInRole("Admin"))
                    {
                        userManager.RemoveFromRole(user.Id, "Admin");
                    }
                    else if (HttpContext.User.IsInRole("User"))
                    {
                        userManager.RemoveFromRole(user.Id, "User");
                    }
                    userManager.AddToRole(user.Id, "Passive");
                    user.ActivationCode = Guid.NewGuid().ToString().Replace("-", "");
                    string siteUrl = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host +
                                    (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                    await SiteSettings.SendMail(new MailModel
                    {
                        To = user.Email,
                        Subject = "Personel Yönetimi - Aktivasyon",
                        Message =
                                $"Merhaba {user.Name} {user.Surname} <br/>Email adresinizi <b>değiştirdiğiniz</b> için hesabınızı tekrar aktif etmelisiniz. <a href='{siteUrl}/Account/Activation?code={user.ActivationCode}'>Aktivasyon Kodu</a>"
                    });
                }
                await userStore.UpdateAsync(user);
                await userStore.Context.SaveChangesAsync();
                var model1 = new ProfileViewModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Surname = user.Surname,
                    UserName = user.UserName
                };
                ViewBag.sonuc = "Bilgileriniz güncelleşmiştir";
                return View(model1);
            }
            catch (Exception ex)
            {
                ViewBag.sonuc = ex.Message;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdatePassword(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                var userStore = MembershipTools.NewUserStore();
                var userManager = new UserManager<ApplicationUser>(userStore);
                var user = userManager.FindById(model.Id);
                user = userManager.Find(user.UserName, model.OldPassword);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Mevcut şifreniz yanlış girilmiştir");
                    return View(model);
                }
                await userStore.SetPasswordHashAsync(user, userManager.PasswordHasher.HashPassword(model.NewPassword));
                await userStore.UpdateAsync(user);
                await userStore.Context.SaveChangesAsync();
                HttpContext.GetOwinContext().Authentication.SignOut();
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.sonuc = "Güncelleşme işleminde bir hata oluştu. " + ex.Message;
                return View(model);
            }
        }
    }
}