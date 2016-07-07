using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.Data
{
    public interface IInstitutionDAO : IInstitutionDAO<Institution>
    {
    }
    public interface IInstitutionDAO<T> : ICoreDAO<T> where T : Institution
    {
    }
}
