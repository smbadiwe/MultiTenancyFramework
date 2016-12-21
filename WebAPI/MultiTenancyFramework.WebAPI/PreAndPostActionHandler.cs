using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiTenancyFramework.WebAPI
{
    /// <summary>
    /// Handles what happens before and after request is processed.
    /// Issue with this: the IPrincipal disappears when we go out of WebAPI context.
    /// However, it's host-agnostic.
    /// </summary>
    public class PreAndPostActionHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!Validate(request))
            {
                var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                var tsc = new TaskCompletionSource<HttpResponseMessage>();
                tsc.SetResult(response);
                return tsc.Task;
            }

            return base.SendAsync(request, cancellationToken)
                .ContinueWith(task =>
                {
                    // work on the response
                    var response = task.Result;
                    response.Headers.Add("X-Dummy-Header", Guid.NewGuid().ToString());
                    return response;
                }, cancellationToken);
        }

        //TODO: work on the request validation
        private bool Validate(HttpRequestMessage request)
        {
            //throw new NotImplementedException();
            return true;
        }
    }
}
