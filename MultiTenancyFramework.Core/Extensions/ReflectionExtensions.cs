using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace MultiTenancyFramework
{
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Determine whether the object is real - non-abstract, non-generic-needed, non-interface class.
        /// </summary>
        /// <param name="testType">Type to be verified.</param>
        /// <returns>True in case the class is real, false otherwise.</returns>
        public static bool IsRealClass(this Type testType)
        {
            return !testType.IsAbstract
                && !testType.IsGenericTypeDefinition
                && !testType.IsInterface
                && !testType.IsPrimitiveType() 
                && !testType.IsArray
                && !testType.IsGenericType;
        }

        public static bool IsPrimitiveType(this Type t)
        {
            return t.IsPrimitive || t.IsValueType || (t == typeof(string));
        }

        //a thread-safe way to hold default instances created at run-time
        private static ConcurrentDictionary<Type, object> typeDefaults = new ConcurrentDictionary<Type, object>();

        public static object GetDefaultValue(this Type type)
        {
            if (!type.IsValueType) return null;
            object obj;
            if (!typeDefaults.TryGetValue(type, out obj))
            {
                obj = Activator.CreateInstance(type);
            }
            return obj;
        }
        
        public static string GetTableName(this Type type)
        {
            var attr = type.GetCustomAttribute<TableAttribute>();
            if (attr == null) return type.Name.ToPlural();

            return attr.Name;
        }

        public static bool IsNullable(this Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// Whether or not the property's type is Nullable<>
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool IsTypeNullable(this PropertyInfo prop)
        {
            return (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>));
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

        /// <summary>
        /// Gets the propertyinfo of a given exrpession. E.g. For expression: x => x.IsDeleted, it will return 
        /// the property info for the 'IsDeleted' property in the type x.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="propertyLambda"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(this Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            MemberExpression member = GetMember(propertyLambda);

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null) throw new ArgumentException("Expression is not a valid property.");

            if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
            {
                throw new ArgumentException(
                    $"Expresion refers to a property that is not from type {type.Name}.");
            }
            return propInfo;
        }

        private static MemberExpression GetMember<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertyLambda)
        {
            if (propertyLambda.Body is MemberExpression)
            {
                return (MemberExpression)propertyLambda.Body;
            }

            if (propertyLambda.Body is UnaryExpression)
            {
                return (MemberExpression)(((UnaryExpression)propertyLambda.Body).Operand);
            }

            throw new ArgumentException($"Expression '{propertyLambda.Name}' refers is not a member expression or unary expression.");
        }

    }
}
