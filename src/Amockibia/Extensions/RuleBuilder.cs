using System.Net.Http;
using Amockibia.Extensions.Matcher;
using Amockibia.Rule;
using Amockibia.Rule.Builder;

namespace Amockibia.Extensions
{
    public class RuleBuilder : IRuleBuildable
    {
        public HttpMethod HttpMethod { get; set; }
        public string RelativeUri { get; set; }
        public string RuleId { get; private set; }

        internal IRequestRespondable Responder { get; set; }

        public RequestHandler Build(string serverId)
        {
            return new RequestHandler(new UriMatcher(serverId,  HttpMethod, RelativeUri), Responder, id: RuleId);
        }

        public IRuleBuildable WithId(string ruleId)
        {
            RuleId = ruleId;
            return this;
        }
    }
}
