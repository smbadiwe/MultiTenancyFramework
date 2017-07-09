
using System;

namespace MultiTenancyFramework.Mvc
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DisplayHintAttribute : Attribute, IModelAttribute
    {
        public DisplayHintAttribute(string hint)
        {
            Hint = hint;
        }

        ///// <summary>
        ///// Initializes a new instance of the <see cref="DisplayHintAttribute"/> class, supplyinh both hint and a display name for the model.
        ///// </summary>
        ///// <param name="hint">The hint.</param>
        ///// <param name="displayName">The display name.</param>
        //public DisplayHintAttribute(string hint, string displayName)
        //    : base(displayName)
        //{
        //    Hint = hint;
        //}
        
        public string Hint { get; set; }

        public string Name
        {
            get { return "DisplayHintAttribute"; }
        }
    }
}
