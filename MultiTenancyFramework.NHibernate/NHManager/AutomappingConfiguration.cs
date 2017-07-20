using FluentNHibernate.Automapping;
using System;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using FluentNHibernate;
using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.NHibernate.NHManager
{
    public class AutomappingConfiguration : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            var map = type.IsClass && !type.IsGenericType && !type.IsAbstract
                && typeof(IEntity).IsAssignableFrom(type) && !type.IsGenericTypeDefinition; // ContainsGenericParameters;

            return map;
        }

        public override bool ShouldMap(Member member)
        {
            if (!member.CanWrite || member.MemberInfo.GetCustomAttribute<NotMappedAttribute>() != null)
                return false;

            return base.ShouldMap(member);
        }

        public override bool IsComponent(Type type)
        {
            // Component types should not be 'Entity's and may or may not have [ComplexType] on them
            if (type != typeof(string) && type.IsClass && !type.IsAbstract && !type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                var isComponent = !typeof(IEntity).IsAssignableFrom(type) &&
                    (type.BaseType == typeof(object) || type.GetCustomAttribute<ComplexTypeAttribute>() != null);
                return isComponent;
            }
            return false;
        }
    }
}
