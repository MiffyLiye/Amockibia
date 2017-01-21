using Amockibia.Rule;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace Amockibia.Test.Utilities
{
    internal class StatusCodeOnlyResponder : IRequestRespondable
    {
        private HttpStatusCode StatusCode { get; }
        public StatusCodeOnlyResponder(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }
        public async Task Respond(HttpResponse response)
        {
            response.StatusCode = (int)StatusCode;
            await response.WriteAsync("");
        }
    }
}
