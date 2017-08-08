using System.Web.Mvc;

namespace MultiTenancyFramework.Mvc.MvcUtils
{
    public class TrimModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            // First check if request validation is required
            var shouldPerformRequestValidation = controllerContext.Controller.ValidateRequest 
                && bindingContext.ModelMetadata.RequestValidationEnabled;

            var unvalidatedValueProvider = bindingContext.ValueProvider as IUnvalidatedValueProvider;
            var value = (unvalidatedValueProvider != null)
               ? unvalidatedValueProvider.GetValue(bindingContext.ModelName, !shouldPerformRequestValidation)
               : bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            
            var attemptedValue = value?.AttemptedValue;

            return string.IsNullOrEmpty(attemptedValue) ? attemptedValue : attemptedValue.Trim();
        }
    }
}
