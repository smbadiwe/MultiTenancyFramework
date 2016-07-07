namespace MultiTenancyFramework.Entities
{
    public class Photo : Entity
    {
        public virtual long OwnerID { get; set; }

        public virtual byte[] Image { get; set; }

        public virtual ImageType ImageType { get; set; }

    }
}
