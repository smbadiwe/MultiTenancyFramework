using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using System.Collections.Generic;

namespace MultiTenancyFramework.NHibernate.Queries
{
    public sealed class GetPhotosByOwnerIdAndImageTypeQueryHandler
        : CoreGeneralDAO, IDbQueryHandler<GetPhotosByOwnerIdAndImageTypeQuery, IList<Photo>>
    {
        public IList<Photo> Handle(GetPhotosByOwnerIdAndImageTypeQuery theQuery)
        {
            if (string.IsNullOrWhiteSpace(theQuery.OwnerID)) return new List<Photo>();

            var session = BuildSession();
            var query = session.QueryOver<Photo>()
                .Where(x => x.OwnerID == theQuery.OwnerID)
                .And(x => x.ImageType == theQuery.ImageType);
            return query.List();
        }
    }
}
