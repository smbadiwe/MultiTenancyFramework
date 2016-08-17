using MultiTenancyFramework;
using MultiTenancyFramework.Entities;
using MultiTenancyFramework.SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTests
{
    class Program
    {
        static void Init()
        {
            var baseContainer = new BaseContainer();
            MyServiceLocator.SetIoCContainer(baseContainer.Container);
        }

        static void Main(string[] args)
        {
            Init();

            var login = new UserRole
            {
                Name = "UserRole 1"
            };
            var dao = new MultiTenancyFramework.Logic.UserRoleLogic("Core");
            //dao.Insert(login);

            UserRole thatLogin = null;
            foreach (var item in dao.RetrieveAllActive())
            {
                if (item.Name == "UserRole 1")
                {
                    item.Name += " Modified";
                    thatLogin = item;
                }
            }
            dao.Update(thatLogin);

            Console.ReadKey();
        }

        public static string GetSessionKey(string institutionCode = null, bool isWebSession = true)
        {
            if (string.IsNullOrWhiteSpace(institutionCode)) institutionCode = "Core";
            return institutionCode + "_" + (isWebSession ? "true" : "false");
        }
    }
}
