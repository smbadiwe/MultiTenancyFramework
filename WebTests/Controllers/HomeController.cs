using MultiTenancyFramework.Mvc;
using MultiTenancyFramework;
using MultiTenancyFramework.Entities;
using MultiTenancyFramework.SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebTests.Controllers
{
    [AllowAnonymous]
    public class HomeController : CoreController
    {
        // GET: Home
        public ActionResult Index()
        {
            var login = new UserRole
            {
                Name = "UserRole Web 3"
            };
            var dao = new MultiTenancyFramework.Logic.UserRoleLogic("Core");
            dao.Insert(login);

            UserRole thatLogin = null;
            foreach (var item in dao.RetrieveAllActive())
            {
                if (item.Name == "UserRole Web 3")
                {
                    item.Name += " Modified";
                    thatLogin = item;
                }
            }
            dao.Update(thatLogin);

            return View();
        }
    }
}