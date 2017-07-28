using System.Web.Mvc;
using System.Web.Routing;

namespace MultiTenancyFramework.Mvc.MvcUtils
{
    public class AppStartInitializer
    {
        /// <summary>
        /// This registers the Areas, Filters and Routes, as well as sets the View Engines to only use Razor and .cshtml files
        /// </summary>
        public static void Initialize(bool useLowercaseRoutes = true)
        {
            AreaRegistration.RegisterAllAreas();
            ModelBinders.Binders.Add(typeof(string), new TrimModelBinder());
            ModelMetadataProviders.Current = new MyModelMetadataProvider();
            InstitutionFilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            InstitutionRouteConfig.RegisterRoutes(RouteTable.Routes, useLowercaseRoutes);
            MvcHandler.DisableMvcResponseHeader = true;

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine { FileExtensions = new[] { "cshtml" } });
        }
    }
}
