using System.Web.Mvc;

namespace MultiTenancyFramework.Mvc
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        /// <summary>
        /// The name of the Error View under Shared Folder. It's "Error" by default
        /// </summary>
        public string SharedErrorViewName { get; set; } = "Error";
        
        // GET: Error
        public virtual ActionResult Index()
        {
            return ErrorView();
        }

        public virtual ActionResult DenyInstitutionAccess()
        {
            return ErrorView(true);
        }

        public virtual ActionResult DenyAccess()
        {
            return View(SharedErrorViewName, new ErrorMessageModel("You are not authorized to access this page."));
        }

        private ViewResult ErrorView(bool showFully = false)
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
            return View(SharedErrorViewName, model);
        }

        // GET: InvalidUrl
        public virtual ActionResult InvalidUrl()
        {
            return View(SharedErrorViewName, new ErrorMessageModel("Invalid Url. Please cross-check.", true));
        }

        // GET: InvalidUrl
        public virtual ActionResult DisabledInstitution(string instName)
        {
            return View(SharedErrorViewName, new ErrorMessageModel(string.Format("This institution <b>{0}</b> has not been registered on our platform.", instName), true));
        }

        // GET: Show
        public virtual ActionResult Show(string err)
        {
            return string.IsNullOrWhiteSpace(err) ? View(SharedErrorViewName, new ErrorMessageModel()) : View(SharedErrorViewName, new ErrorMessageModel(err));
        }
    }

}
