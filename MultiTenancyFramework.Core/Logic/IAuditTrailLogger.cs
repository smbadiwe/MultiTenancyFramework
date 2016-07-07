namespace MultiTenancyFramework.Logic
{
    public interface IAuditTrailLogger
    {
        /// <summary>
        /// Audit Trail at every login or logout attempt. Do not set the second parameter 
        /// if it was possible at all to retrieve the user. In that case, just bundle the user in session.
        /// We'll pick it up from there.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="remarks">Remarks.</param>
        /// <param name="institutionCode">Institution Code.</param>
        void AuditLogin(EventType action, string userName = null, string remarks = null, string institutionCode = null);
    }
}
