using System.Web.Mvc;
using System.Web.Routing;

namespace MultiTenancyFramework.Mvc
{
    /// <summary>
    /// My custom Route Config
    /// </summary>
    public class InstitutionRouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*staticfile}", new { staticfile = @".*\.(css|less|sass|js|gif|png|jpg|jpeg|ico|svg|ttf|eot\woff|woff2|xml|csv|txt|map|json)(/.*)?" });

            routes.LowercaseUrls = true;

            routes.MapMvcAttributeRoutes();

            // From most specific to most general

            routes.MapRoute(
                name: "Default",
                url: "",
                defaults: new { institution = Utilities.INST_DEFAULT_CODE, area = "", controller = "Home", action = "Index", id = UrlParameter.Optional } //,
                //constraints: new { id = @"\d*" }
            );

            routes.MapRoute(
                name: "Error",
                url: "Error",
                defaults: new { institution = Utilities.INST_DEFAULT_CODE, area = "", controller = "Error", action = "Index", id = UrlParameter.Optional } //,
                //constraints: new { id = @"\d*" }
            );

            routes.MapRoute(
                name: "TenantError",
                url: "{institution}/Error",
                defaults: new { area = "", controller = "Error", action = "Index", id = UrlParameter.Optional } //,
                //constraints: new { id = @"\d*" }
            );

            routes.MapRoute(
                name: "InstitutionOnly",
                url: "{institution}",
                defaults: new { area = "", controller = "Home", action = "Index", id = UrlParameter.Optional } //,
                //constraints: new { id = @"\d*" }
            );

            routes.MapRoute(
                name: "ControllerAndActionOnly",
                url: "{controller}/{action}/{id}",
                defaults: new { institution = Utilities.INST_DEFAULT_CODE, area = "", id = UrlParameter.Optional } //,
                //constraints: new { id = @"\d*" }
            );

            routes.MapRoute(
                name: "MultiTenant",
                url: "{institution}/{controller}/{action}/{id}",
                defaults: new { area = "", id = UrlParameter.Optional } //,
                //constraints: new { id = @"\d*" }
            );

            routes.MapRoute(
                name: "MultiTenantWithArea",
                url: "{institution}/{area}/{controller}/{action}/{id}",
                defaults: new { id = UrlParameter.Optional } //, 
                //constraints: new { id = @"\d*" }
            );

        }
    }
}
