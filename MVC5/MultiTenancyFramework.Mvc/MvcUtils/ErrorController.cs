using System.Net;
using System.Web.Mvc;
using MultiTenancyFramework;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;

namespace MultiTenancyFramework.Mvc
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        /// <summary>
        /// The name of the Error View under Shared Folder. It's "Error" by default,
        /// but you may want to set the full path: ~/Views/...
        /// </summary>
        public static string SharedErrorViewName { get; set; } = "Error";
        /// <summary>
        /// The name of the ReLogin View (view used to ask a user to enter password again to reauthenticate)
        /// under Shared Folder. It's "ReLogin" by default, but you may want to set the full path: ~/Views/...
        /// </summary>
        public static string ReLoginViewName { get; set; } = "ReLogin";

        private ILogger Logger = Utilities.Logger;
        // GET: Error
        public virtual ActionResult Index()
        {
            return ErrorView();
        }

        // GET: Error/ReLogin
        public virtual ActionResult ReLogin()
        {
            var currentUser = WebUtilities.GetCurrentlyLoggedInUser();
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account", new { area = "", institution = WebUtilities.InstitutionCode });
            }
            var model = new ReLoginModel
            {
                Username = currentUser.UserName
            };
            return View(ReLoginViewName, model);
        }
        
        public virtual ActionResult DenyInstitutionAccess()
        {
            WebUtilities.LogOut();
            return ErrorView(HttpStatusCode.Forbidden, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionAttempted">The action for which we're denying access</param>
        /// <returns></returns>
        public virtual ActionResult DenyAccess(string actionAttempted = null)
        {
            WebUtilities.LogOut();
            return View(SharedErrorViewName, new ErrorMessageModel(string.Format("You are not authorized to access this page.{0}", !string.IsNullOrWhiteSpace(actionAttempted) ? $" [You need to be given the privilege: {actionAttempted}]" : ""), true)
            {
                ResponseCode = HttpStatusCode.Forbidden,
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionAttempted">The action for which we're denying access</param>
        /// <returns></returns>
        public virtual ActionResult TenantsOnlyAllowed(string actionAttempted = null)
        {
            return View(SharedErrorViewName, new ErrorMessageModel(string.Format("Only tenant institutions are allowed to access this page.{0}", !string.IsNullOrWhiteSpace(actionAttempted) ? $" [You need to be given the privilege: {actionAttempted}]" : "")
               , false)
            {
                ResponseCode = HttpStatusCode.Forbidden,
            });
        }

        private ViewResult ErrorView(HttpStatusCode code = HttpStatusCode.BadRequest, bool showFully = false)
        {
            ErrorMessageModel model;
            if (TempData.ContainsKey(ErrorMessageModel.ErrorMessageKey))
            {
                model = TempData[ErrorMessageModel.ErrorMessageKey] as ErrorMessageModel;
            }
            else
            {
                model = new ErrorMessageModel(showFully);
            }
            if (model.ResponseCode == HttpStatusCode.BadRequest)
            {
                model.ResponseCode = code;
            }
            return View(SharedErrorViewName, model);
        }

        // GET: InvalidUrl
        public virtual ActionResult InvalidUrl()
        {
            var path = Server.MapPath("~/404.html");
            if (System.IO.File.Exists(path))
            {
                return File(path, "text/html");
            }

            return View(SharedErrorViewName, new ErrorMessageModel("Invalid Url. Please cross-check.", true)
            {
                ResponseCode = HttpStatusCode.NotFound
            });
        }

        public virtual ActionResult ViewNotFound()
        {
            return View(SharedErrorViewName, new ErrorMessageModel("Looks like the view for the requested action is not available or under construction")
            {
                ResponseCode = HttpStatusCode.ServiceUnavailable
            });
        }

        // GET: DisabledInstitution
        public virtual ActionResult DisabledInstitution(string instName)
        {
            return View(SharedErrorViewName, new ErrorMessageModel(string.Format("This institution <b>{0}</b> has not been registered on our platform.", instName), true)
            {
                ResponseCode = HttpStatusCode.Forbidden
            });
        }

        // GET: Show
        public virtual ActionResult Show(string err)
        {
            return string.IsNullOrWhiteSpace(err) ? View(SharedErrorViewName, new ErrorMessageModel()) : View(SharedErrorViewName, new ErrorMessageModel(err));
        }
    }

}
