using MultiTenancyFramework.Data;
using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.Logic
{
    public class UserClaimLogic : CoreLogic<UserClaim>
    {
        public UserClaimLogic(string institutionCode)
            : base(MyServiceLocator.GetInstance<ICoreDAO<UserClaim>>(), institutionCode)
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
