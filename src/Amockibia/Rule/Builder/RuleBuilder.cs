using System.Net;
using Amockibia.Rule;
using Amockibia.Rule.Matcher;
using Microsoft.AspNetCore.Http;

namespace Amockibia
{
    public class RuleBuilder
    {
        public string RelativeUri { get; set; }

        private RequestResponder Responder { get; set; }

        internal HandlingRule Build(string serverId)
        {
            return new HandlingRule(new UriMatcher(serverId, RelativeUri), Responder);
        }

        public RuleBuilder RespondOK()
        {
            Responder = new RequestResponder(async context => {
                context.Response.StatusCode = (int) HttpStatusCode.OK;
                await context.Response.WriteAsync("");
            });
            return this;
        }
    }

    public class When
    {
        public static RuleBuilder Get(string relativeUri)
        {
            return new RuleBuilder{ RelativeUri = relativeUri };
        }
    }
}
