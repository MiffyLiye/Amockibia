using Microsoft.AspNetCore.Http;
using System.Net;

namespace Amockibia.Rule.Responder
{
    internal class DefaultResponder : RequestResponder
    {
        public DefaultResponder() : base(async (context) => {
            context.Response.StatusCode = (int) HttpStatusCode.NotImplemented;
            await context.Response.WriteAsync("NotImplemented");
        }) { }
    }
}
