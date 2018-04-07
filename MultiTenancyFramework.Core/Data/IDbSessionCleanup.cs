namespace MultiTenancyFramework.Data
{
    public interface IDbSessionCleanup
    {
        void CloseDbConnections();
    }
}
