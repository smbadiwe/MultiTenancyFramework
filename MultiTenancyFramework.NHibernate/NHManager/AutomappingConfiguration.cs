using FluentNHibernate.Automapping;
using System;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using FluentNHibernate;
using MultiTenancyFramework.Entities;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Mapping;
using MultiTenancyFramework;

namespace MultiTenancyFramework.NHibernate.NHManager
{
    public class AutomappingConfiguration : DefaultAutomappingConfiguration
    {
        private Type[] searchableTypes;

        /// <summary>
        /// Gets potential types derived from ClassMap
        /// <para>UPDATE: This method will usually not be needed as Fluent NHibernate handles this well for us.</para>
        /// </summary>
        /// <param name="mappingAssemblies">The mapping assemblies.</param>
        public void ScanForPotentialClassMaps(IList<Assembly> mappingAssemblies)
        {
            if (mappingAssemblies != null)
            {
                searchableTypes = mappingAssemblies.SelectMany(x => x.GetTypes())
                    .Where(x => (!x.IsGenericType && !x.IsAbstract && x.IsPublic && !typeof(CoreGeneralDAO).IsAssignableFrom(x)))
                    .ToArray();
            }
        }

        public override bool ShouldMap(Type type)
        {
            var shouldMap = base.ShouldMap(type) && !type.IsGenericType && !type.IsAbstract
                && typeof(IBaseEntity).IsAssignableFrom(type);
            if (shouldMap && searchableTypes != null && searchableTypes.Length > 0)
            {
                // Check if the entity is already explicitly mapped and exclude it here
                var classMapExists = searchableTypes.Any(x => IsDefinedClassMapOfEntity(x, type));

                return !classMapExists;
            }
            return shouldMap;
        }

        public override bool ShouldMap(Member member)
        {
            //base.ShouldMap(member) tests for IsPublic and IsProperty
            if (base.ShouldMap(member) && member.CanWrite
                && member.MemberInfo.GetCustomAttribute<NotMappedAttribute>() == null)
            {
                // Check if the property is overridden in a child class. 
                // If it is, return false, otherwise, return true.
                // The idea is that if this property is overridden, then we'll map it in the parent class.
                return false == (member.MemberInfo as PropertyInfo).IsOverridden();
            }
            return false;
        }

        public override bool IsComponent(Type type)
        {
            // Component types should not be 'Entity's and may or may not have [ComplexType] on them
            if (type != typeof(string) && type.IsClass && !type.IsAbstract && !type.IsGenericType)
            {
                var isComponent = !typeof(IBaseEntity).IsAssignableFrom(type) &&
                    (type.BaseType == typeof(object) || type.GetCustomAttribute<ComplexTypeAttribute>() != null);
                return isComponent;
            }
            return false;
        }

        private bool IsDefinedClassMapOfEntity(Type potentialClassMapType, Type entityType)
        {
            if (potentialClassMapType.IsGenericType || potentialClassMapType.IsAbstract
                || typeof(CoreGeneralDAO).IsAssignableFrom(potentialClassMapType)) return false;

            Type genericType = typeof(ClassMap<>);
            Type type = potentialClassMapType;
            while (type != null)
            {
                if (type.IsGenericType)
                {
                    if (type.GetGenericTypeDefinition() == genericType)
                    {
                        return type.GenericTypeArguments.Contains(entityType);
                    }
                }
                type = type.BaseType;
            }
            return false;
        }

    }
}
