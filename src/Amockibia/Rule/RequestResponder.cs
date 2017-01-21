using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Amockibia.Rule
{
    internal class RequestResponder
    {
        private Func<HttpContext, Task> RespondAction { get; }
        public RequestResponder(Func<HttpContext, Task> respond)
        {
            RespondAction = respond;
        }

        public async Task Respond(HttpContext context)
        {
            await RespondAction(context);
        }
    }
}
