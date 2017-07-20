using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.NHibernate
{
    public abstract class CoreGeneralWithGridPagingDAO<T> : CoreGridPagingDAO<T, long> where T : class, IBaseEntity<long>
    {

    }

}
