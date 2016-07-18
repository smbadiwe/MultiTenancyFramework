using System;
using System.ComponentModel;
using System.Web.Mvc;

namespace MultiTenancyFramework.Mvc
{
    public class MyModelMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        // Creates a nice DisplayName from the model’s property name if one hasn't been specified
        protected override ModelMetadata GetMetadataForProperty(
           Func<object> modelAccessor,
           Type containerType,
           PropertyDescriptor propertyDescriptor)
        {
            ModelMetadata metadata = base.GetMetadataForProperty(modelAccessor, containerType, propertyDescriptor);

            if (metadata.DisplayName == null)
            {
                metadata.DisplayName = DisplayNameFromCamelCase(metadata.PropertyName); //.GetDisplayName()
            }
            metadata.NullDisplayText = $"Enter {metadata.DisplayName}";
            return metadata;
        }

        private string DisplayNameFromCamelCase(string name)
        {
            //name.AsSplitPascalCasedString();
            //name = _camelCaseRegex.Replace(name, " $0");
            if (name.EndsWith(" Id", StringComparison.CurrentCultureIgnoreCase))
                name = name.Substring(0, name.Length - 3);
            else if (name.EndsWith(" Str", StringComparison.CurrentCultureIgnoreCase))
                name = name.Substring(0, name.Length - 4);
            return name.AsSplitPascalCasedString() + ":";
        }
    }
}
