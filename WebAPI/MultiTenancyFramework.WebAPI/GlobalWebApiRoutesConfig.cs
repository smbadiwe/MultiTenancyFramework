using System.Web.Http;

namespace MultiTenancyFramework.WebAPI
{
    public class GlobalWebApiRoutesConfig
    {
        /// <summary>
        /// Routes are mapped such that urls generated are lowercase
        /// </summary>
        /// <param name="routes"></param>
        public static void RegisterRoutes(HttpRouteCollection routes)
        {
            //NB: ic === "institution code"; ver === "version"
            string instCode = "Core";

            routes.IgnoreRoute(routeName: "resources", routeTemplate: "{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("static", "{ *staticfile}", new { staticfile = @".*\.(css|less|sass|js|gif|png|jpg|jpeg|ico|svg|ttf|eot|woff|woff2|xml|csv|txt|map|json|pdf|doc|docx|xls|xlsx|dll|exe|pdb)(/.*)?" });
            
            // From most specific to most general
            routes.MapHttpRoute( //LowerCase(
                name: "Api_Default",
                routeTemplate: "api/",
                defaults: new { ver = "1.0", ic = instCode, area = "", controller = "Home", action = "Index", id = RouteParameter.Optional },
                constraints: new { id = @"\d*" }
            );
            
            routes.MapHttpRoute( //LowerCase(
                name: "Api_DefaultApi",
                routeTemplate: "api/{ver}/",
                defaults: new { ic = instCode, area = "", controller = "Home", action = "Index", id = RouteParameter.Optional },
                constraints: new { id = @"\d*" }
            );

            routes.MapHttpRoute( //LowerCase(
                name: "Api_Error",
                routeTemplate: "api/{ver}/Error",
                defaults: new { ic = instCode, area = "", controller = "Error", action = "Index", id = RouteParameter.Optional },
                constraints: new { id = @"\d*" }
            );

            routes.MapHttpRoute( //LowerCase(
                name: "Api_TenantError",
                routeTemplate: "api/{ver}/{ic}/Error",
                defaults: new { area = "", controller = "Error", action = "Index", id = RouteParameter.Optional },
                constraints: new { id = @"\d*" }
            );

            routes.MapHttpRoute( //LowerCase(
                name: "Api_IcOnly",
                routeTemplate: "api/{ver}/{ic}",
                defaults: new { area = "", controller = "Home", action = "Index", id = RouteParameter.Optional },
                constraints: new { id = @"\d*" }
            );

            // Ambigous; this clashes with the below a lot of times
            //routes.MapHttpRoute( //LowerCase(
            //    name: "Api_ControllerAndActionOnly",
            //    routeTemplate: "api/{ver}/{controller}/{action}/{id}",
            //    defaults: new { ic = instCode, area = "", id = RouteParameter.Optional },
            //    constraints: new { id = @"\d*" }
            //);

            routes.MapHttpRoute( //LowerCase(
                name: "Api_MultiTenant",
                routeTemplate: "api/{ver}/{ic}/{controller}/{action}/{id}",
                defaults: new { area = "", id = RouteParameter.Optional },
                constraints: new { id = @"\d*" }
            );

            routes.MapHttpRoute( //LowerCase(
                name: "Api_MultiTenantWithArea",
                routeTemplate: "api/{ver}/{ic}/{area}/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional },
                constraints: new { id = @"\d*" }
            );

        }
    }
}
