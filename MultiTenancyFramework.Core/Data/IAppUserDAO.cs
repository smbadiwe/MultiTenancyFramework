using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.Data
{
    public interface IAppUserDAO : IAppUserDAO<AppUser>
    {

    }

    public interface IAppUserDAO<T> : ICoreDAO<T> where T : AppUser
    {

    }
}
