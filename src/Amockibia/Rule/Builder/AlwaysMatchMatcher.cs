using Microsoft.AspNetCore.Http;

namespace Amockibia.Rule.Builder
{
    internal class AlwaysMatchMatcher : IRequestMatchable
    {
        public bool Matches(HttpRequest request)
        {
            return true;
        }
    }
}
