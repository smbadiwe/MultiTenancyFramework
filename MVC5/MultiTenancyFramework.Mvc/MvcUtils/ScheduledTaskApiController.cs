using MultiTenancyFramework.Core.TaskManager;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MultiTenancyFramework.Mvc.MvcUtils
{
    /// <summary>
    /// A thread does an API call to this controller/action and expects nothing in return. 
    /// It's not meant to be used or browsed.
    /// </summary>
    /// <seealso cref="Controller" />
    public class ScheduledTaskApiController : Controller
    {
        [HttpPost]
        public async Task<ActionResult> RunTask(string taskType, string instCode)
        {
            try
            {
                var processor = Utilities.QueryProcessor;
                processor.InstitutionCode = instCode;
                ScheduledTask scheduledTask = await processor.ProcessAsync(new GetTaskByTypeQuery { Type = taskType });
                if (scheduledTask != null)
                {
                    var task = new ScheduledTaskRunner(scheduledTask);
                    await task.Execute();
                }
            }
            catch (System.Exception ex)
            {
                Utilities.Logger.Log("ScheduledTaskApi/RunTask" + ex.GetFullExceptionMessage());
            }
            return new EmptyResult();
        }
    }
}
