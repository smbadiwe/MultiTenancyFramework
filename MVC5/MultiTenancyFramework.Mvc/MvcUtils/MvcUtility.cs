using Microsoft.AspNet.Identity;
using MultiTenancyFramework;
using MultiTenancyFramework.Data;
using MultiTenancyFramework.Entities;
using System.Web;
using System.Web.Mvc;

namespace MultiTenancyFramework.Mvc
{
    public static class MvcUtility
    {
        public const string _TRUE_ICON= "<i class='text-info fa fa-check'></i>";
        public const string _FALSE_ICON = "<i class='text-danger fa fa-close'></i>";
        public static ContentResult GetLoginPageResult(string instCode)
        {
            var baseUrl = ConfigurationHelper.AppSettingsItem<string>("SiteUrl");
            var url = string.Format("{0}{1}/Account/Login", baseUrl, instCode);
            return new ContentResult
            {
                Content = "<html><script>window.top.location.href = '" + url + "'; </script></html>",
                ContentType = "text/html"
            };
        }

        public static void RegisterArea(string area, AreaRegistrationContext context)
        {
            context.MapRoute(
                name: $"{area}_MultiTenant",
                url: "{institution}/" + area +"/{controller}/{action}/{id}",
                defaults: new { id = UrlParameter.Optional },
                constraints: new { id = @"\d*" }
            );
        }


        public const string SS_SYS_SETTINGS = "::SystemSettings::";

        public static SystemSetting SystemSettings
        {
            get
            {
                if (HttpRuntime.Cache != null)
                {
                    var item = HttpRuntime.Cache[SS_SYS_SETTINGS] as SystemSetting;
                    if (item == null)
                    {
                        var dao = MyServiceLocator.GetInstance<ICoreDAO<SystemSetting>>();
                        try
                        {
                            item = dao.RetrieveOne();
                        }
                        catch (System.Data.Common.DbException)
                        {
                            return new SystemSetting();
                        }
                        HttpRuntime.Cache[SS_SYS_SETTINGS] = item;
                    }
                    return item;
                }
                return null;
            }
            set
            {
                if (HttpRuntime.Cache != null)
                {
                    HttpRuntime.Cache[SS_SYS_SETTINGS] = value;
                }
            }
        }

        public static string HashString(string clearText)
        {
            return System.Web.Helpers.Crypto.HashPassword(clearText); //new Microsoft.AspNet.Identity.PasswordHasher()
        }

        public static bool VerifyHash(string hashedText, string clearText)
        {
            return System.Web.Helpers.Crypto.VerifyHashedPassword(hashedText, clearText); //new Microsoft.AspNet.Identity.PasswordHasher()
        }

        public static void SendMail(string toEmail, string subject, string body)
        {
            IIdentityMessageService emailService = new EmailService();
            var mail = new EmailMessage
            {
                Destination = toEmail,
                Body = body,
                Subject = subject,
            };
            emailService.SendAsync(mail);
        }
    }
}
