using System.Web.Mvc;

namespace MultiTenancyFramework.Mvc.MvcUtils
{
    public class TrimModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext,  ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var attemptedValue = value?.AttemptedValue;

            return string.IsNullOrEmpty(attemptedValue) ? attemptedValue : attemptedValue.Trim();
        }
    }
}
