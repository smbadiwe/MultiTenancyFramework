namespace MultiTenancyFramework.Entities
{

    public abstract class Person : BaseEntity
    {
        /// <summary>
        /// Passport is what I have in mind here
        /// </summary>
        public virtual Photo Photo { get; set; }
        public virtual string LastName { get; set; }
        public virtual string OtherNames { get; set; }
        /// <summary>
        /// Returns the full name of the person.
        /// </summary>
        public virtual string Name { get { return FullNames; } }
        public virtual string FullNames { get { return $"{LastName}, {OtherNames}"; } }
        public virtual string Email { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual bool IsFemale { get { return Gender == Gender.Female; } }
        public virtual bool IsMale { get { return Gender == Gender.Male; } }
    }
}
