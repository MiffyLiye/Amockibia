using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Amockibia.Rule.Builder
{
    internal class NotImplementedResponder : IRequestRespondable
    {
        public async Task Respond(HttpRequest request, HttpResponse response)
        {
            response.StatusCode = (int)HttpStatusCode.NotImplemented;
            await response.WriteAsync("NotImplemented");
        }
    }
}
