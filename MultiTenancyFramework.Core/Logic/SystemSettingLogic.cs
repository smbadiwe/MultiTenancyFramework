using MultiTenancyFramework.Data;
using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.Logic
{
    public class SystemSettingLogic : CoreBaseLogic<SystemSetting>
    {
        public SystemSettingLogic() : base(MyServiceLocator.GetInstance<ICoreDAO<SystemSetting>>())
        {
        }

        public SystemSetting RetrieveSystemSetting()
        {
            return _dao.RetrieveOne();
        }

        public override void OnAfterCommittingChanges(SystemSetting e)
        {
            base.OnAfterCommittingChanges(e);
            Utilities.SystemSettings = e;
        }
    }
}
