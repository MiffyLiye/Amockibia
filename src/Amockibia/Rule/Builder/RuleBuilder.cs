using Amockibia.Extensions.Matcher;

namespace Amockibia.Rule.Builder
{
    public class RuleBuilder : IRuleBuildable
    {
        public string RelativeUri { get; set; }

        internal IRequestRespondable Responder { get; set; }

        public RequestHandler Build(string serverId)
        {
            return new RequestHandler(new UriMatcher(serverId, RelativeUri), Responder);
        }
    }
}
