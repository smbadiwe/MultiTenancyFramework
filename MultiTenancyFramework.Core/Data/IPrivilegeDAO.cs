using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.Data
{
    public interface IPrivilegeDAO : IPrivilegeDAO<Privilege>
    {
    }

    public interface IPrivilegeDAO<T> : ICoreDAO<T> where T : Privilege
    {
    }
}
