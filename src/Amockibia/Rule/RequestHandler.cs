using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amockibia.Rule
{
    public sealed class RequestHandler
    {
        public string Id { get; }
        private List<HttpRequest> HandledRequests { get; }
        public IReadOnlyList<HttpRequest> ReceivedRequests => HandledRequests.AsReadOnly();
        private IRequestMatchable Matcher { get; }
        private IRequestRespondable Responder { get; }
        /// <summary>
        /// Smaller value means higher priority.
        /// </summary>
        public int Priority { get; }
        /// <summary>
        /// Times it can respond to matched requests before this rule get expired. -1 means infinite.
        /// </summary>
        public int RemainingRespondTimes { get; private set; }

        public RequestHandler(IRequestMatchable requestMatcher, IRequestRespondable responder, int priority = 100, int remainingRespondTimes = -1, string id = null)
        {
            HandledRequests = new List<HttpRequest>();
            Matcher = requestMatcher;
            Responder = responder;
            Priority = priority;
            RemainingRespondTimes = remainingRespondTimes;
            Id = id;
        }

        public bool Alive()
        {
            return RemainingRespondTimes != 0;
        }

        public void Reserve()
        {
            RemainingRespondTimes -= RemainingRespondTimes == -1 ? 0 : 1;
        }

        /// <summary>
        /// Get whether this handler can be used to process the request.
        /// Should have no side effects.
        /// </summary>
        public bool Matches(HttpRequest request)
        {
            return Matcher.Matches(request);
        }

        public async Task Respond(HttpRequest request, HttpResponse response)
        {
            await Responder.Respond(request, response);
            HandledRequests.Add(response.HttpContext.Request);
        }
    }
}
