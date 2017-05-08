using MultiTenancyFramework.Mvc;
using MultiTenancyFramework;
using MultiTenancyFramework.Entities;
using MultiTenancyFramework.SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MultiTenancyFramework.Data;
using System.Threading.Tasks;

namespace WebTests.Controllers
{
    public class HomeController : CoreController
    {
        // GET: Home
        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            var roles = new List<UserRole>
            {
                new UserRole
                {
                    Name = "UserRole Web 3",
                    Description = "Go Away",
                    DateCreated = DateTime.UtcNow,
                    IsDeleted = false,
                },
                new UserRole
                {
                    Name = "UserRole Web 33",
                    Description = "Go Away 4 3",
                    DateCreated = DateTime.UtcNow,
                    IsDeleted = true,
                },
                new UserRole
                {
                    Name = "UserRole Web 13",
                    Description = "Go Away",
                    DateCreated = DateTime.UtcNow,
                    IsDeleted = false,
                },
                new UserRole
                {
                    Name = "UserRole Web 73",
                    Description = "Go gfe Away",
                    DateCreated = DateTime.UtcNow,
                    IsDeleted = true,
                },
            };
            UserRole x;
            return await roles.ToDataTable(new[] {
                new MyDataColumn(nameof(x.DateCreated)),
                new MyDataColumn(nameof(x.IsDeleted)),
                new MyDataColumn(nameof(x.IsDisabled)),
                new MyDataColumn(nameof(x.Name)),
                new MyDataColumn(nameof(x.Description)),
            })
            .ExportFile("xlsx", "UserRoles");


            //var login = new UserRole
            //{
            //    Name = "UserRole Web 3"
            //};
            //var dao = new MultiTenancyFramework.Logic.UserRoleLogic("Core");
            //dao.Insert(login);

            //UserRole thatLogin = null;
            //foreach (var item in dao.RetrieveAllActive())
            //{
            //    if (item.Name == "UserRole Web 3")
            //    {
            //        item.Name += " Modified";
            //        thatLogin = item;
            //    }
            //}
            //dao.Update(thatLogin);
            //Logger.Log("Testing stuffs in Index");
            //return View();
        }

        public ActionResult Welcome()
        {
            //var login = new UserRole
            //{
            //    Name = "UserRole Web 3"
            //};
            //var dao = new MultiTenancyFramework.Logic.UserRoleLogic("Core");
            //dao.Insert(login);

            //UserRole thatLogin = null;
            //foreach (var item in dao.RetrieveAllActive())
            //{
            //    if (item.Name == "UserRole Web 3")
            //    {
            //        item.Name += " Modified";
            //        thatLogin = item;
            //    }
            //}
            //dao.Update(thatLogin);

            Logger.Log("Testing stuffs in Welcome");
            return View("Index");
        }
    }
}