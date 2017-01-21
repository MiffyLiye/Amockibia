using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Amockibia.Rule
{
    public class RequestHandler
    {
        private object Locker { get; } = new object();
        private IRequestMatchable Matcher { get; }
        private IRequestRespondable Responder { get; }
        /// <summary>
        /// Priority: smaller value means higher priority.
        /// </summary>
        public int Priority { get; }
        /// <summary>
        /// RemainingRespondTime: times it can respond to matched requests before this rule get invalid. -1 means infinite.
        /// </summary>
        public int RemainingRespondTimes { get; private set; }

        public RequestHandler(IRequestMatchable requestMatcher, IRequestRespondable responder, int priority = 100, int remainingRespondTimes = -1)
        {
            Matcher = requestMatcher;
            Responder = responder;
            Priority = priority;
            RemainingRespondTimes = remainingRespondTimes;
        }

        public bool Matches(HttpRequest request)
        {
            lock (Locker)
            {
                return RemainingRespondTimes != 0 &&
                    Matcher.Matches(request);
            }
        }

        public async Task Respond(HttpResponse response)
        {
            lock (Locker)
            {
                RemainingRespondTimes -= 1;
            }
            await Responder.Respond(response);
        }
    }
}
