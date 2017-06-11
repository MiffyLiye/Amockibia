using System.Collections.Generic;
using System.Linq;
using Amockibia.Rule;
using Microsoft.AspNetCore.Http;

namespace Amockibia.Extensions
{
    public class RuleExecutionResult
    {
        private readonly RequestHandler _handler;

        internal RuleExecutionResult(RequestHandler handler)
        {
            _handler = handler;
        }

        public int HasBeenCalledTimes => _handler.ReceivedRequests.Count;
        public bool HasBeenCalled => _handler.ReceivedRequests.Any();
        public IReadOnlyList<HttpRequest> Requests => _handler.ReceivedRequests;
    }
}