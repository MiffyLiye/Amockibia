using Microsoft.AspNetCore.Http;

namespace Amockibia.Rule
{
    internal abstract class RequestMatcher
    {
        public abstract bool Matches(HttpContext context);
    }
}
