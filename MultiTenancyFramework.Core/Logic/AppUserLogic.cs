using MultiTenancyFramework.Data;
using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.Logic
{
    public class AppUserLogic : AppUserLogic<AppUser>
    {
        public AppUserLogic(string institutionCode) : base(institutionCode, MyServiceLocator.GetInstance<IAppUserDAO>())
        {
        }
    }

    public class AppUserLogic<T> : CoreLogic<T> where T : AppUser
    {
        public AppUserLogic(string institutionCode, IAppUserDAO<T> baseUserDAO)
            : base(MyServiceLocator.GetInstance<IAppUserDAO<T>>(), institutionCode)
        {
            _dao.EntityName = EntityName = baseUserDAO.EntityName;
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
        
        public virtual RetrievedData<T> Search(string lastname, string othernames, string username, long userRole, int page, int pageSize)
        {
            var theQuery = new GetAppUsersByGridSearchParamsQuery
            {
                LastName = lastname,
                OtherNames = othernames,
                Username = username,
                UserRole = userRole,
                PageIndex = page,
                PageSize = pageSize,
            };
            return QueryProcessor.Process(theQuery).Cast<AppUser, T>();
        }

        public override void OnBeforeSaving(T e)
        {
            e.ForceChangeOfPassword = true;
            base.OnBeforeSaving(e);
        }

        public virtual T RetrieveByEmail(string email)
        {
            var theQuery = new GetAppUserByEmailQuery
            {
                Email = email,
            };
            return QueryProcessor.Process(theQuery) as T;
        }

        public virtual T RetrieveByUsername(string username)
        {
            var theQuery = new GetAppUserByUsernameQuery
            {
                Username = username,
            };
            return QueryProcessor.Process(theQuery) as T;
        }
    }

}
