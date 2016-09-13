using MultiTenancyFramework.Entities;
using System.Collections.Generic;

namespace MultiTenancyFramework.Data.Queries
{
    public sealed class GetPhotosByOwnerIdAndImageTypeQuery : IDbQuery<IList<Photo>>
    {
        public string OwnerID { get; set; }
        public ImageType ImageType { get; set; }
    }
}
