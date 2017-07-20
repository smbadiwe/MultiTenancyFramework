namespace MultiTenancyFramework.Entities
{
    /// <summary>
    /// A person's photo record
    /// </summary>
    public class Photo : Entity
    {
        public Photo()
        {
            SkipAudit = true;
        }

        /// <summary>
        /// The ID representing the person that owns this photo record
        /// </summary>
        public virtual string OwnerID { get; set; }

        /// <summary>
        /// The image, as byte array, typically to be saved in the Database
        /// </summary>
        public virtual byte[] Image { get; set; }

        /// <summary>
        /// Url (filepath) of the image, use when you're saving the image to disk, not database
        /// </summary>
        public virtual string ImageUrl { get; set; }
        
        /// <summary>
        /// Image type
        /// </summary>
        public virtual ImageType ImageType { get; set; }
    }
}
