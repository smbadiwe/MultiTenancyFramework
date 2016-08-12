using NHibernate;

namespace MultiTenancyFramework.NHibernate.NHManager
{
    /// <summary>
    /// Implements <see cref="ISessionStorage"/> via the session-per-request pattern
    /// Handles storage of Nhibernate Storage using the HttpContext and Closes the session at the end of the HttpRequest.
    /// </summary>
    public class WebSessionStorage : ISessionStorage
    {
        /// <summary>
        /// Constant key for storing the session in the HttpContext
        /// </summary>
        public const string CurrentSessionKey = "::nhibernate_current_session::";

        public string InstitutionCode { get; set; }
        public ISession Session { get; set; }
    }
}
