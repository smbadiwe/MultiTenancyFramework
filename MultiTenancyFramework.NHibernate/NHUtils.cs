using MultiTenancyFramework.Entities;
using System;
using System.Web;

namespace MultiTenancyFramework.NHibernate
{
    public class NHUtils
    {
        public static AppUser CurrentUser
        {
            get
            {
                try
                {
                    return HttpContext.Current.Session["::SS_CURRENT_USER::"] as AppUser;
                }
                catch (Exception)
                {
                    return null;
                }

            }
        }

    }
}
