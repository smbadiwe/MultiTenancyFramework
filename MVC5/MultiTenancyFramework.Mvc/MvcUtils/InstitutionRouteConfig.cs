using MultiTenancyFramework;
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
            routes.IgnoreRoute("{resource}.js");
            routes.IgnoreRoute("{resource}.css");
            routes.IgnoreRoute("{resource}.png");
            routes.IgnoreRoute("{resource}.jpeg");
            routes.IgnoreRoute("{resource}.jpg");
            routes.IgnoreRoute("{resource}.ico");
            routes.IgnoreRoute("{resource}.svg");
            routes.IgnoreRoute("{resource}.woff");
            routes.IgnoreRoute("{resource}.woff2");
            routes.IgnoreRoute("{resource}.ttf");
            routes.IgnoreRoute("{resource}.eot");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "MultiTenantWithArea",
                url: "{institution}/{area}/{controller}/{action}/{id}",
                defaults: new { id = UrlParameter.Optional },
                constraints: new { id = @"\d*" }
            );

            routes.MapRoute(
                name: "MultiTenant",
                url: "{institution}/{controller}/{action}/{id}",
                defaults: new { area = "", id = UrlParameter.Optional },
                constraints: new { id = @"\d*" }
            );

            routes.MapRoute(
                name: "ControllerAndActionOnly",
                url: "{controller}/{action}/{id}",
                defaults: new { institution = Utilities.INST_DEFAULT_CODE, area = "", id = UrlParameter.Optional },
                constraints: new { id = @"\d*" }
            );

            routes.MapRoute(
                name: "Error",
                url: "Error",
                defaults: new { institution = Utilities.INST_DEFAULT_CODE, area = "", action = "Index", id = UrlParameter.Optional },
                constraints: new { id = @"\d*" }
            );

            routes.MapRoute(
                name: "InstitutionOnly",
                url: "{institution}",
                defaults: new { area = "", controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { id = @"\d*" }
            );

            routes.MapRoute(
                name: "Default",
                url: "",
                defaults: new { institution = Utilities.INST_DEFAULT_CODE, area = "", controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { id = @"\d*" }
            );

        }
    }
}
