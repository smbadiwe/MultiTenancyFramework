using System.Net;
using System.Net.Sockets;
using System.Web;

namespace MultiTenancyFramework
{
    public class IPResolver
    {
        /// <summary>
        /// Try to get IP address from DNS server
        /// </summary>
        /// <param name="returnIP6IfIP4DoesntExist">if set to <c>true</c> [return i p6 if i p4 doesnt exist].</param>
        /// <returns></returns>
        public static string GetIP4Address(bool returnIP6IfIP4DoesntExist = true)
        {
            string str = string.Empty;
            if (HttpContext.Current == null) return str;
            
            try
            {
                Utilities.Logger.Log("Trying to get IP address from DNS server");
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
