namespace MultiTenancyFramework.Entities
{
    public class Privilege : Entity, IAmHostedCentrally
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        private string _description;
        public virtual string Description
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_description)) _description = DisplayName;
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        /// <summary>
        /// The readable form of the name
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// If true, then every user must have this privilege. Example is LogOff, View/Edit User Profile
        /// </summary>
        public virtual bool IsDefault { get; set; }

        public virtual AccessScope Scope { get; set; }

        /// <summary>
        /// Whether or not this is available only for the guy(s) at the back, managing all institutions' issues
        /// </summary>
        public virtual bool IsRootOnly { get { return Scope == AccessScope.CentralOnly; } }

        public virtual string ScopeStr { get { return Scope.ToReadableString(); } }

        public override bool Equals(object obj)
        {
            if (Name == null) return false;
            var other = obj as Privilege;
            if (other == null) return false;
            return Name.Equals(other.Name) && InstitutionCode == other.InstitutionCode;
        }
    }
}
