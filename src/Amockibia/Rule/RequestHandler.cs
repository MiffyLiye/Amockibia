using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amockibia.Utilities;
using Microsoft.AspNetCore.Http;

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
        public int MatchTimesUntilExpire { get; private set; }

        public RequestHandler(IRequestMatchable requestMatcher, IRequestRespondable responder, int priority = 100, int matchTimesUntilExpire = -1, string id = null)
        {
            HandledRequests = new List<HttpRequest>();
            Matcher = requestMatcher;
            Responder = responder;
            Priority = priority;
            MatchTimesUntilExpire = matchTimesUntilExpire;
            Id = id;
        }

        public bool Alive()
        {
            return MatchTimesUntilExpire != 0;
        }

        public void Reserve()
        {
            MatchTimesUntilExpire -= MatchTimesUntilExpire == -1 ? 0 : 1;
        }

        /// <summary>
        /// Get whether this handler can be used to process the request.
        /// Should have no side effects.
        /// </summary>
        public bool Matches(HttpRequest request)
        {
            try
            {
                return Matcher.Matches(request);

            }
            catch (Exception e)
            {
                e.Ignore();
                return false;
            }
        }

        public async Task Respond(HttpRequest request, HttpResponse response)
        {
            if (!string.IsNullOrEmpty(Id))
            {
                var persistentStream = new MemoryStream();
                await response.HttpContext.Request.Body.CopyToAsync(persistentStream);
                response.HttpContext.Request.Body = persistentStream;
                
                HandledRequests.Add(response.HttpContext.Request);
            }
            await Responder.Respond(request, response);
        }
    }
}
