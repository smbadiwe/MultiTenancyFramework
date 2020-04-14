using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace MultiTenancyFramework
{
    /// <summary>
    /// More in 'ReflectionExtensions' in MultiTenancyFramework.Utils
    /// </summary>
    public static class ReflectionExtensions
    {
        public static string GetTableName(this Type type)
        {
            var attr = type.GetCustomAttribute<TableAttribute>();
            if (attr == null)
            {
                var tableName = type.Name.ToPlural();
                if (ConfigurationHelper.AppSettingsItem<bool>("UseLowercaseTableNames"))
                {
                    tableName = tableName.ToLowerInvariant();
                }
                return tableName;
            }
            return attr.Name;
        }

        /// <summary>
        /// This gets the FieldNameInDB Attribute if set, otherwise, the property name
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="ignoreFieldNameAttribute"></param>
        /// <param name="ignoreFieldNameAttribute">If this is true, then ignoreFieldNameAttribute must be false</param>
        /// <returns></returns>
        public static string GetPropertyName(this PropertyInfo prop, CompositeMappingModifyFieldNamesAttribute modifyFieldNameAttr = null)
        {
            try
            {
                //When this is in operation, prop refers to in inner property of the member whose
                // property has this attribute
                if (modifyFieldNameAttr != null)
                {
                    return modifyFieldNameAttr.FieldAndPropNames[prop.Name];
                }
                var fieldNameAttr = prop.GetCustomAttribute<ColumnAttribute>(true);
                if (fieldNameAttr != null)
                {
                    return fieldNameAttr.Name;
                }
                return prop.Name;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get corresponding name for prop: " + prop.Name, ex);
            }
        }

    }
}
