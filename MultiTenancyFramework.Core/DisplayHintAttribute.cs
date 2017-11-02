
using System;

namespace MultiTenancyFramework
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayHintAttribute"/> class, supplying both hint and a display name for the model.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DisplayHintAttribute : Attribute, IModelAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayHintAttribute"/> class, supplying both hint and a display name for the model.
        /// </summary>
        /// <param name="hint">The hint.</param>
        public DisplayHintAttribute(string hint)
        {
            Hint = hint;
        }
        
        public string Hint { get; set; }

        public string Name
        {
            get { return "DisplayHintAttribute"; }
        }
    }
}
