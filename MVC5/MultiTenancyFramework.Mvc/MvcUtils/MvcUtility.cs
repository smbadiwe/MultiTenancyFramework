using Microsoft.AspNet.Identity;
using MultiTenancyFramework.Mvc.Logic;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MultiTenancyFramework.Mvc {
    public static class MvcUtility {
        public const string _TRUE_ICON = "<i class='text-info fa fa-check'></i>";
        public const string _FALSE_ICON = "<i class='text-danger fa fa-close'></i>";
        public static ContentResult GetLoginPageResult(string instCode, string returnUrl = null) {
            var baseUrl = ConfigurationHelper.GetSiteUrl();
            var url = string.Format("{0}{1}{2}", baseUrl, instCode, DataCacheMVC.MultiTenancyFrameworkSettings.LoginPath);
            if (!string.IsNullOrWhiteSpace(returnUrl)) {
                url += $"?returnUrl={returnUrl}";
            }
            return new ContentResult {
                Content = "<html><script>window.top.location.href = '" + url + "'; </script></html>",
                ContentType = "text/html"
            };
        }

        /// <summary>
        /// Use this to get a page when outside of a controller-derived class. It's supposed to work like a RedirectToAction(...) method
        /// </summary>
        /// <param name="action"></param>
        /// <param name="controller"></param>
        /// <param name="area"></param>
        /// <param name="instCode"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        public static RedirectToRouteResult GetPageResult(string action, string controller, string area, string institution, Dictionary<string, object> others = null) {
            if (others == null) {
                others = new Dictionary<string, object>(4);
            }
            others["action"] = action;
            others["controller"] = controller;
            others["area"] = area;
            others["institution"] = institution;
            return new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(others));
        }

        public static string GetRouteNameForArea(string areaName)
        {
            return $"{areaName}_MultiTenant";
        }

        public static void RegisterArea(string areaName, AreaRegistrationContext context) {
            context.MapRoute( //LowerCase(
                name: GetRouteNameForArea(areaName),
                url: "{institution}/" + areaName + "/{controller}/{action}/{id}",
                defaults: new { area = areaName, id = UrlParameter.Optional },
                constraints: new { id = @"\d*" }
            );
        }

        public static string HashString(string clearText) {
            return System.Web.Helpers.Crypto.HashPassword(clearText);
        }

        public static bool VerifyHash(string hashedText, string clearText) {
            return System.Web.Helpers.Crypto.VerifyHashedPassword(hashedText, clearText);
        }

        public static void SendMail(string toEmail, string subject, string body) {
            IIdentityMessageService emailService = new EmailService();
            var mail = new EmailMessage {
                Destination = toEmail,
                Body = body,
                Subject = subject,
            };
            emailService.SendAsync(mail);
        }
    }
}
