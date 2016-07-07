using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.Data.Queries
{
    public class GetInstitutionByCodeQuery : IDbQuery<Institution>
    {
        public string Code { get; set; }
    }
}
