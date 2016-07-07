using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.NHibernate.Queries
{
    public class GetInstitutionByCodeQueryHandler : CoreGeneralDAO, IDbQueryHandler<GetInstitutionByCodeQuery, Institution>
    {
        public GetInstitutionByCodeQueryHandler()
        {
            EntityName = NHManager.NHSessionManager.GetEntityNameToUseInNHSession(typeof(Institution));
        }

        public Institution Handle(GetInstitutionByCodeQuery theQuery)
        {
            var session = BuildSession();
            var query = session.QueryOver<Institution>(EntityName).Where(x => x.Code == theQuery.Code);
            return query.SingleOrDefault();
        }
    }
}
