using System;

namespace MultiTenancyFramework
{
    /// <summary>
    /// Use this to provide an alternative or more readable name for your enum
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EnumDescriptionAttribute : Attribute
    {
        private string _name;
        public EnumDescriptionAttribute(string name)
        {
            _name = name;
        }

        /// <summary>
        /// The alternative name
        /// </summary>
        public string Name { get { return _name; } }
    }

}
