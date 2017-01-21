using Microsoft.AspNetCore.Http;

namespace Amockibia.Rule
{
    public interface IRequestMatchable
    {
        bool Matches(HttpRequest request);
    }
}
