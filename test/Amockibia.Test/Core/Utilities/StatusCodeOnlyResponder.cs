using System.Net;
using System.Threading.Tasks;
using Amockibia.Rule;
using Microsoft.AspNetCore.Http;

namespace Amockibia.Test.Core.Utilities
{
    internal class StatusCodeOnlyResponder : IRequestRespondable
    {
        private HttpStatusCode StatusCode { get; }
        public StatusCodeOnlyResponder(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }
        public Task Respond(HttpRequest request, HttpResponse response)
        {
            response.StatusCode = (int)StatusCode;
            return Task.CompletedTask;
        }
    }
}
