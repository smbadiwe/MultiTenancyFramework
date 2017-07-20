using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.NHibernate.Maps
{
    public class PhotoMap : EntityMap<Photo>
    {
        public PhotoMap()
        {
            Map(x => x.Image);
            Map(x => x.ImageUrl);
            Map(x => x.ImageType);
            Map(x => x.OwnerID);
        }
    }
}
