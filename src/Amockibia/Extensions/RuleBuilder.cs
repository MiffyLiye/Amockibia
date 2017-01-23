using System;
using Amockibia.Extensions.Matcher;
using Amockibia.Rule;
using Amockibia.Rule.Builder;

namespace Amockibia.Extensions
{
    public class RuleBuilder : IRuleBuildable
    {
        public string RelativeUri { get; set; }

        internal IRequestRespondable Responder { get; set; }

        public RequestHandler Build(string serverId)
        {
            return new RequestHandler(new UriMatcher(serverId, RelativeUri), Responder);
        }

        RequestHandler IRuleBuildable.Build(string serverId)
        {
            throw new NotImplementedException();
        }
    }
}
