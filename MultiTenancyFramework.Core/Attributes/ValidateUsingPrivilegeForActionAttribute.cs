using System;

namespace MultiTenancyFramework.MvcUtils //Attributes
{
    /// <summary>
    /// MVC-specific; when two or more actions share the same privilege, point the actions to one using this attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ValidateUsingPrivilegeForActionAttribute : Attribute
    {
        private string _actionNames;
        public string[] ActionNames
        {
            get
            {
                return _actionNames.Split(',');
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionNames">Comma-separated list of the relevant action names</param>
        public ValidateUsingPrivilegeForActionAttribute(string actionNames)
        {
            if (string.IsNullOrWhiteSpace(actionNames)) throw new ArgumentNullException(actionNames);
            _actionNames = actionNames;
        }
    }
}
