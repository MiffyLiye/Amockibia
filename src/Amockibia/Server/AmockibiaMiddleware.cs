using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amockibia.Rule;
using Amockibia.Utilities;
using Microsoft.AspNetCore.Http;

namespace Amockibia.Server
{
    public class AmockibiaMiddleware
    {
        public RequestDelegate Next { get; }
        private object Locker { get; } = new object();
        public List<RequestHandler> Rules { get; }

        public AmockibiaMiddleware(RequestDelegate next, string serverId)
        {
            Next = next;
            Rules = serverId.GetConfig().Rules;
        }

        public async Task Invoke(HttpContext context)
        {
            await Next(context);
            var candidateRules = Rules.Where(r => r.Alive() && r.Matches(context.Request)).OrderBy(r => r.Priority)
                .ToList();
            RequestHandler matchedRule;
            lock (Locker)
            {
                matchedRule = candidateRules.First(r => r.Alive());
                matchedRule.Reserve();
            }
            await matchedRule.Respond(context.Request, context.Response);
        }
    }
}