using MultiTenancyFramework.Data;
using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.Logic
{
    public class UserRoleLogic : CoreLogic<UserRole>
    {
        public UserRoleLogic(string institutionCode)
            : base(MyServiceLocator.GetInstance<ICoreDAO<UserRole>>(), institutionCode)
        {
        }

    }
}
