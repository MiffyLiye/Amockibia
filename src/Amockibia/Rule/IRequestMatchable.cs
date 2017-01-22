using Microsoft.AspNetCore.Http;

namespace Amockibia.Rule
{
    public interface IRequestMatchable
    {
        /// <summary>
        /// Get whether the request matches the predicate.
        /// Should have no side effects.
        /// </summary>
        bool Matches(HttpRequest request);
    }
}
