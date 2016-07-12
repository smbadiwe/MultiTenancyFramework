

using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiTenancyFramework.Entities
{
    /// <summary>
    /// Access to (MVC) actions and, by extension, app functionalities
    /// </summary>
    [Table("Privileges")]
    public class ActionAccessPrivilege : Privilege, IRole<long>
    {
        /// <summary>
        /// Introducing IRole forced me to map Name RoleName since it's what we used to call it in the old model
        /// <para>It's a combination of <see cref="Controller"/> and <see cref="Action"/>. Therefore, it should NOT be set</para>
        /// </summary>
        /// <value>The name of the role.</value>
        public override string Name { get { return $"{Action}-{Controller}-{Area}"; } set { } }

        /// <summary>
        /// The (MVC) Action name
        /// </summary>
        public virtual string Action { get; set; }
        /// <summary>
        /// The (MVC) Controller name
        /// </summary>
        public virtual string Controller { get; set; }
        /// <summary>
        /// The (MVC) Area name
        /// </summary>
        public virtual string Area { get; set; }

    }
}
