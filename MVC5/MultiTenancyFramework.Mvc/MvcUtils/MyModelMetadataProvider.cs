using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using System.Linq;

namespace MultiTenancyFramework.Mvc
{
    /// <summary>
    /// GetMetadataForProperty and CreateMetadata are overridden here from <seealso cref="DataAnnotationsModelMetadataProvider" />
    /// </summary>
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

        protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
        {
            var metadata = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);
            var additionalValues = attributes.OfType<IModelAttribute>().ToList();
            foreach (var additionalValue in additionalValues)
            {
                if (metadata.AdditionalValues.ContainsKey(additionalValue.Name))
                    throw new GeneralException("There is already an attribute with the name of \"" + additionalValue.Name +
                                           "\" on this model.");
                metadata.AdditionalValues.Add(additionalValue.Name, additionalValue);
            }
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
