namespace MultiTenancyFramework.Data
{
    public interface ICoreUnitOfWorkDAO
    {
        /// <summary>
        /// Commits the changes.
        /// </summary>
        void CommitChanges();

        /// <summary>
        /// Rollbacks the changes.
        /// </summary>
        void RollbackChanges();

    }
}
