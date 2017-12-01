using Microsoft.AspNet.Identity;
using PT.Entitiy.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PT.BLL.AccountRepository;

namespace PT.Web.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            var roles = MembershipTools.NewRoleManager().Roles.ToList();
            var userManager = MembershipTools.NewUserManager();
            var users = userManager.Users.ToList().Select(x => new UsersViewModel
            {
                Email = x.Email,
                Name = x.Name,
                RegisterDate = x.RegisterDate,
                Salary = x.Salary,
                Surname = x.Surname,
                UserId = x.Id,
                UserName = x.UserName,
                RoleId = x.Roles.FirstOrDefault().RoleId,
                RoleName = roles.FirstOrDefault(r => r.Id == userManager.FindById(x.Id).Roles.FirstOrDefault().RoleId).Name
            }).ToList();
            return View(users);
        }

        public ActionResult EditUser(string id)
        {
            if (id == null)
                RedirectToAction("Index");

            var roles = MembershipTools.NewRoleManager().Roles.ToList();
            List<SelectListItem> rolList = new List<SelectListItem>();
            roles.ForEach(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id
            });
            ViewBag.roles = rolList;
            var userManager = MembershipTools.NewUserManager();
            var user = userManager.FindById(id);
            if (user == null)
                return RedirectToAction("Index");

            var model = new UsersViewModel()
            {
                UserName = user.UserName,
                Email = user.Email,
                Surname = user.Surname,
                Name = user.Name,
                RegisterDate = user.RegisterDate,
                RoleId = user.Roles.ToList().FirstOrDefault().RoleId,
                RoleName = roles.FirstOrDefault(r => r.Id == userManager.FindById(user.Id).Roles.FirstOrDefault().RoleId).Name,
                Salary = user.Salary,
                UserId = user.Id
            };
            return View(model);
        }
    }
}