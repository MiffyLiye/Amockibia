using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amockibia.Rule;
using Amockibia.Rule.Builder;
using Amockibia.Setup;
using Amockibia.Test.Core.Utilities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Amockibia.Test.Core
{
    public class Example : TestBase
    {
        private class HttpMethodMatcher : IRequestMatchable
        {
            private HttpMethod Method { get; }

            public HttpMethodMatcher(string serverId, HttpMethod method)
            {
                Method = method;
            }

            public bool Matches(HttpRequest request)
            {
                return request.Method == Method.ToString();
            }
        }

        private class HttpMethodMatchRuleBuilder : IRuleBuildable
        {
            private HttpMethod Method { get; }
            public HttpMethodMatchRuleBuilder(HttpMethod method)
            {
                Method = method;
            }
            public RequestHandler Build(string serverId)
            {
                return new RequestHandler(new HttpMethodMatcher(serverId, Method), new StatusCodeOnlyResponder(HttpStatusCode.OK));
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task should_return_not_implemented_when_request_not_matched(bool isInMemoryHost)
        {
            Server.Setup(new HttpMethodMatchRuleBuilder(HttpMethod.Delete));
            var client = SelectHttpClient(isInMemoryHost);

            var response = await client.GetAsync("NotImplemented");

            response.StatusCode.Should().Be(HttpStatusCode.NotImplemented);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task should_return_stub_when_request_matched(bool isInMemoryHost)
        {
            Server.Setup(new HttpMethodMatchRuleBuilder(HttpMethod.Delete));
            var client = SelectHttpClient(isInMemoryHost);

            var response = await client.DeleteAsync("");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
