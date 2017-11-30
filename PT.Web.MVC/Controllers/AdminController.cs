using Microsoft.AspNet.Identity;
using PT.BLL.AccountRepository;
using PT.Entitiy.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PT.Web.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            var userManager = MembershipTools.NewUserManager();
            var users = userManager.Users.Select(x => new UsersViewModel
            {
                Email = x.Email,
                Name = x.Name,
                RegisterDate = x.RegisterDate,
                Salary = x.Salary,
                Surname = x.Surname,
                UserId = x.Id,
                UserName = x.UserName,
                RoleId = x.Roles.FirstOrDefault().RoleId,
                RoleName = MembershipTools.NewRoleManager().FindById(x.Roles.FirstOrDefault().RoleId).Name
            }).ToList();
            var roles = MembershipTools.NewRoleManager().Roles.ToList();
            List<SelectListItem> rolList = new List<SelectListItem>();
            roles.ForEach(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id
            });
            ViewBag.roles = rolList;
            return View(users);
        }
    }
}