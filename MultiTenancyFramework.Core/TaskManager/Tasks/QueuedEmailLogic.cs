using MultiTenancyFramework.Data;
using MultiTenancyFramework.Logic;

namespace MultiTenancyFramework.Core.TaskManager.Tasks
{
    public class QueuedEmailLogic : CoreLogic<QueuedEmail>
    {
        public QueuedEmailLogic(string institutionCode)
            : base(MyServiceLocator.GetInstance<ICoreDAO<QueuedEmail>>(), institutionCode)
        {

        }
    }
}
