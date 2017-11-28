using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PT.BLL.AccountRepository;
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
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var userManager = MembershipTools.NewUserManager();
            var checkUser = userManager.FindByName(model.Username);
            if (checkUser != null)
            {
                ModelState.AddModelError(string.Empty, "Bu kullanıcı zaten kayıtlı!");
                return View(model);
            }
            return View();
        }
    }
}