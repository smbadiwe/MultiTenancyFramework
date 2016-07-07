using System.Web.Mvc;
using System.Web.Routing;

namespace MultiTenancyFramework.Mvc.MvcUtils
{
    public class AppStartInitializer
    {
        public static void Initialize()
        {

            AreaRegistration.RegisterAllAreas();
            ModelMetadataProviders.Current = new MyModelMetadataProvider();
            InstitutionFilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            InstitutionRouteConfig.RegisterRoutes(RouteTable.Routes); //
                                                                      //ModelBinders.Binders.DefaultBinder = new MyModelBinder();

            MvcHandler.DisableMvcResponseHeader = true;
        }
    }
}
