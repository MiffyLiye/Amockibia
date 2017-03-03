using Amockibia.Extensions.Responder;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http;

namespace Amockibia.Extensions
{
    public class When
    {
        public static RuleBuilder Get(string relativeUri)
        {
            return new RuleBuilder { HttpMethod = HttpMethod.Get , RelativeUri = relativeUri };
        }
    }

    public static class RuleExtensions
    {
        public static RuleBuilder RespondOK(this RuleBuilder builder)
        {
            builder.Responder = new RequestResponder(async response => {
                response.StatusCode = (int)HttpStatusCode.OK;
                await response.WriteAsync("");
            });
            return builder;
        }
    }
}
