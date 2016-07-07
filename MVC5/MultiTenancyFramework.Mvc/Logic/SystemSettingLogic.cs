using MultiTenancyFramework.Data;
using MultiTenancyFramework.Entities;
using MultiTenancyFramework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            MvcUtility.SystemSettings = e;
            base.OnAfterCommittingChanges(e);
        }
    }
}
