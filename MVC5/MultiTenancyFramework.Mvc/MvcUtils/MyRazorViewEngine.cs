using System.Web.Mvc;

namespace MultiTenancyFramework.Mvc
{
    public class MyRazorViewEngine : RazorViewEngine
    {
        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            if (base.FileExists(controllerContext, virtualPath))
            {
                return true;
            }
            throw new ViewNotFoundException(virtualPath);
        }
        
    }
}
