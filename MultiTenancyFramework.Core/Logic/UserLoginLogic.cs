using MultiTenancyFramework.Data;
using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.Logic
{
    public class UserLoginLogic : CoreLogic<UserLogin>
    {
        public UserLoginLogic(string institutionCode)
            : base(MyServiceLocator.GetInstance<ICoreDAO<UserLogin>>(), institutionCode)
        {
        }
        
        public override string InstitutionCode
        {
            get
            {
                return base.InstitutionCode;
            }
            set
            {
                _dao.InstitutionCode = base.InstitutionCode = value;
            }
        }

    }
}
