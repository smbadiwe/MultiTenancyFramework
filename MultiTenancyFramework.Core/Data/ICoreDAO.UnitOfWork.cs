using System.Threading;
using System.Threading.Tasks;

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
        Task CommitChangesAsync(CancellationToken token = default(CancellationToken));
        Task RollbackChangesAsync(CancellationToken token = default(CancellationToken));
    }
}
