﻿using System;
using System.Collections.Concurrent;
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

        /// <summary>
        /// Determines whether the given type is primitive. Here, strings and value types are treated as primitive.
        /// </summary>
        /// <param name="t">The type t.</param>
        /// <returns>
        ///   <c>true</c> if the specified type is primitive; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPrimitiveType(this Type t)
        {
            return t.IsPrimitive || t == typeof(string) || t.IsValueType;
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

        /// <summary>
        /// Determines whether the specified property is overridden from a base class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>
        ///   <c>true</c> if the specified property is overridden from a base class; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsOverridden(this PropertyInfo property)
        {
            // If the property is overridden, its 'getter' MethodInfo will 
            // return a different MethodInfo from GetBaseDefinition.
            return property != null && property.GetGetMethod().GetBaseDefinition().DeclaringType != property.DeclaringType;
        }
    }
}
