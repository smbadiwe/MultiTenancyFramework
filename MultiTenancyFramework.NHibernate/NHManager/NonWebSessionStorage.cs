using NHibernate;

namespace MultiTenancyFramework.NHibernate.NHManager
{
    /// <summary>
    /// Basic implementation of <see cref="ISessionStorage"/> for use when a non-web session is desired
    /// </summary>
    public class NonWebSessionStorage : ISessionStorage
    {
        //public string InstitutionCode { get; set; }

        public ISession Session { get; set; }
    }
}
