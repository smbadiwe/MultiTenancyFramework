using System;

namespace MultiTenancyFramework.Mvc
{
    /// <summary>
    /// When institution code indicates that the parent (central) institution is accessing the action, 
    /// we use this to check whether or not the central institution can access the action.
    /// Use this on actions and/or controllers. This is only triggered if action or controller is not anonymous.
    /// <para>Our default is to assume most of the actions can be accessed by tenants only; but we know there are a few 
    /// that should be availabe to both tenants and Core. Normally we use privileges and roles to control what a user can access; this is just an extra level of enforcement</para>
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AllowAccessToParentAttribute : Attribute
    {
        
    }
}
