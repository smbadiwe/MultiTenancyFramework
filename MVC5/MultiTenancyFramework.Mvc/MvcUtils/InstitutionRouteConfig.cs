using System.Web.Mvc;
using System.Web.Routing;

namespace MultiTenancyFramework.Mvc
{
    /// <summary>
    /// My custom Route Config.
    /// </summary>
    public class InstitutionRouteConfig
    {
        /// <summary>
        /// Routes are mapped such that urls generated are lowercase
        /// </summary>
        /// <param name="routes"></param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*staticfile}", new { staticfile = @".*\.(css|less|sass|js|gif|png|jpg|jpeg|ico|svg|ttf|eot|woff|woff2|xml|csv|txt|map|json|pdf|doc|docx|xls|xlsx|dll|exe|pdb)(/.*)?" });

            //routes.LowercaseUrls = true; //This worked, but caused some View rendering issues. So I'm using the extension method: .MapRouteLowerCase

            routes.MapMvcAttributeRoutes();

            // From most specific to most general

            routes.MapRoute( //LowerCase(
                name: "Default",
                url: "",
                defaults: new { institution = Utilities.INST_DEFAULT_CODE, area = "", controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { id = @"\d*" }
            );

            routes.MapRoute( //LowerCase(
                name: "Error",
                url: "Error",
                defaults: new { institution = Utilities.INST_DEFAULT_CODE, area = "", controller = "Error", action = "Index", id = UrlParameter.Optional },
                constraints: new { id = @"\d*" }
            );

            routes.MapRoute( //LowerCase(
                name: "TenantError",
                url: "{institution}/Error",
                defaults: new { area = "", controller = "Error", action = "Index", id = UrlParameter.Optional },
                constraints: new { id = @"\d*" }
            );

            routes.MapRoute( //LowerCase(
                name: "InstitutionOnly",
                url: "{institution}",
                defaults: new { area = "", controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { id = @"\d*" }
            );

            routes.MapRoute( //LowerCase(
                name: "ControllerAndActionOnly",
                url: "{controller}/{action}/{id}",
                defaults: new { institution = Utilities.INST_DEFAULT_CODE, area = "", id = UrlParameter.Optional },
                constraints: new { id = @"\d*" }
            );

            routes.MapRoute( //LowerCase(
                name: "MultiTenant",
                url: "{institution}/{controller}/{action}/{id}",
                defaults: new { area = "", id = UrlParameter.Optional },
                constraints: new { id = @"\d*" }
            );

            routes.MapRoute( //LowerCase(
                name: "MultiTenantWithArea",
                url: "{institution}/{area}/{controller}/{action}/{id}",
                defaults: new { id = UrlParameter.Optional }, 
                constraints: new { id = @"\d*" }
            );

        }
    }

}
