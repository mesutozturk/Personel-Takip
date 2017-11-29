﻿using System;
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
    }
}