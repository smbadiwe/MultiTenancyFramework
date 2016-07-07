using System.Net;
using System.Net.Sockets;
using System.Web;

namespace MultiTenancyFramework
{
    public class IPResolver
    {
        public static string GetIP4Address(bool returnIP6IfIP4DoesntExist = true)
        {
            string str = string.Empty;
            if (HttpContext.Current == null) return str;

            try
            {
                IPAddress[] allHostAddresses = Dns.GetHostAddresses(HttpContext.Current.Request.UserHostAddress);
                foreach (IPAddress ipAddress in allHostAddresses)
                {
                    if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                    {
                        str = ipAddress.ToString();
                        break;
                    }
                }
                if (str != string.Empty)
                {
                    return str;
                }

                //If code gets here, it only means you're browsing localhost
                allHostAddresses = Dns.GetHostAddresses(Dns.GetHostName());
                foreach (IPAddress ipAddress in allHostAddresses)
                {
                    if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                    {
                        str = ipAddress.ToString();
                        break;
                    }
                }

                if (returnIP6IfIP4DoesntExist && string.IsNullOrWhiteSpace(str))
                    return HttpContext.Current.Request.UserHostAddress;
            }
            catch (HttpException)
            {
                return "Could not get IP Address of User";
            }
            return str;
        }
    }
}
