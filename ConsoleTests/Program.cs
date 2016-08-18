using MultiTenancyFramework;
using MultiTenancyFramework.Entities;
using MultiTenancyFramework.Logic;
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

            var engine = new UserRoleLogic("LYYKL");
            UserRole item = engine.Retrieve(1);
            Console.ReadKey();
        }

        public static string GetSessionKey(string institutionCode = null, bool isWebSession = true)
        {
            if (string.IsNullOrWhiteSpace(institutionCode)) institutionCode = "Core";
            return institutionCode + "_" + (isWebSession ? "true" : "false");
        }
    }
}
