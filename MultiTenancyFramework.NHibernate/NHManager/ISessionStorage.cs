using NHibernate;

namespace MultiTenancyFramework.NHibernate.NHManager
{
    /// <summary>
    /// Provides a standard interface for managing session storage
    /// </summary>
    public interface ISessionStorage
    {
        ISession Session { get; set; }
    }

}
