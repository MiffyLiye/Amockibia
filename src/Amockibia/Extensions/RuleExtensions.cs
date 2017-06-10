using Amockibia.Extensions.Responder;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http;

namespace Amockibia.Extensions
{
    public class When
    {
        public static RuleBuilder Receive(HttpMethod httpMethod, string relativeUri)
        {
            return new RuleBuilder { HttpMethod = httpMethod, RelativeUri = relativeUri };
        }

        public static RuleBuilder Get(string relativeUri)
        {
            return Receive(HttpMethod.Get, relativeUri);
        }
        
        public static RuleBuilder Post(string relativeUri)
        {
            return Receive(HttpMethod.Post, relativeUri);
        }
        
        public static RuleBuilder Put(string relativeUri)
        {
            return Receive(HttpMethod.Put, relativeUri);
        }
        
        public static RuleBuilder Delete(string relativeUri)
        {
            return Receive(HttpMethod.Delete, relativeUri);
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
