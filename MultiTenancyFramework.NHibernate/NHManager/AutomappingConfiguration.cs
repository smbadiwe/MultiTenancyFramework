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
            var shouldMap = base.ShouldMap(type) && !type.IsGenericType && !type.IsAbstract
                && typeof(IBaseEntity).IsAssignableFrom(type);

            return shouldMap;
        }

        public override bool ShouldMap(Member member)
        {
            //base.ShouldMap(member) tests for IsPublic and IsProperty
            if (base.ShouldMap(member) && member.CanWrite
                && member.MemberInfo.GetCustomAttribute<NotMappedAttribute>() == null)
            {
                // Check if the property is overridden in a child class. If it is, return false.
                if (member.MemberInfo.DeclaringType == member.MemberInfo.ReflectedType)
                {
                    var classProps = member.MemberInfo.ReflectedType.BaseType.GetProperty(member.Name, BindingFlags.Instance | BindingFlags.Public);
                    return classProps == null;
                }
                return true;
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
    }
}
