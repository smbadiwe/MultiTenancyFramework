using MultiTenancyFramework.Data;
using MultiTenancyFramework.Logic;

namespace MultiTenancyFramework.Core.TaskManager.Tasks
{
    public class EmailAccountLogic : CoreLogic<EmailAccount>
    {
        public EmailAccountLogic(string institutionCode)
            : base(MyServiceLocator.GetInstance<ICoreDAO<EmailAccount>>(), institutionCode)
        {

        }
    }
}
