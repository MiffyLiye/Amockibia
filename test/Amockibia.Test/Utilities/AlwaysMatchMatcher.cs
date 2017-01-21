using Amockibia.Rule;
using Microsoft.AspNetCore.Http;

namespace Amockibia.Test.Utilities
{
    internal class AlwaysMatchMatcher : IRequestMatchable
    {
        public bool Matches(HttpRequest request)
        {
            return true;
        }
    }
}
