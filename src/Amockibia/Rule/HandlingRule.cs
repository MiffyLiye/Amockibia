using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Amockibia.Rule
{
    internal class HandlingRule
    {
        private RequestMatcher RequestMatcher { get; }
        private RequestResponder Responder { get; }
        /// <summary>
        /// Priority: smaller value means higher priority.
        /// </summary>
        public int Priority { get; }
        /// <summary>
        /// RemainingRespondTime: times it can respond to matched requests before this rule get invalid. -1 means infinity.
        /// </summary>
        public int RemainingRespondTimes { get; }

        public HandlingRule(RequestMatcher requestMatcher, RequestResponder responder, int priority = 100, int remainingRespondTimes = -1)
        {
            RequestMatcher = requestMatcher;
            Responder = responder;
            Priority = priority;
            RemainingRespondTimes = remainingRespondTimes;
        }

        public bool Matches(HttpContext context)
        {
            return RequestMatcher.Matches(context);
        }

        public async Task Respond(HttpContext context)
        {
            await Responder.Respond(context);
        }
    }
}
