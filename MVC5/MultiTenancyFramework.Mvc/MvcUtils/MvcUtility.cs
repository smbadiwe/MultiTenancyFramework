using Microsoft.AspNet.Identity;
using MultiTenancyFramework.Data;
using MultiTenancyFramework.Entities;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;

namespace MultiTenancyFramework.Mvc
{
    public static class MvcUtility
    {
        public const string _TRUE_ICON= "<i class='text-info fa fa-check'></i>";
        public const string _FALSE_ICON = "<i class='text-danger fa fa-close'></i>";
        public static ContentResult GetLoginPageResult(string instCode, HttpContextBase context)
        {
            //var baseUrl = context.Request.Url.Authority; // "/"; // VirtualPathUtility.ToAbsolute("~/"); //
            var baseUrl = ConfigurationHelper.GetSiteUrl();
            var url = string.Format("{0}{1}/Account/Login", baseUrl, instCode);
            return new ContentResult
            {
                Content = "<html><script>window.top.location.href = '" + url + "'; </script></html>",
                ContentType = "text/html"
            };
        }

        public static void RegisterArea(string areaName, AreaRegistrationContext context)
        {
            context.MapRoute( //LowerCase(
                name: $"{areaName}_MultiTenant",
                url: "{institution}/" + areaName +"/{controller}/{action}/{id}",
                defaults: new { area = areaName, id = UrlParameter.Optional },
                constraints: new { id = @"\d*" }
            );
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
