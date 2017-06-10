using System.Net;
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
    public class RespondBasedOnRequestTest : TestBase
    {
        private class RequestPathResponder : IRequestRespondable
        {
            public async Task Respond(HttpRequest request, HttpResponse response)
            {
                response.StatusCode = (int)HttpStatusCode.OK;
                await response.WriteAsync(request.Path);
            }
        }

        private class RequestPathRuleBuilder : IRuleBuildable
        {
            public RequestHandler Build(string serverId)
            {
                return new RequestHandler(new AlwaysMatchMatcher(), new RequestPathResponder());
            }
        }

        [Theory]
        [InlineData("/first")]
        [InlineData("/second")]
        public async Task should_respond_based_on_request(string relativeUri)
        {
            Server.Setup(new RequestPathRuleBuilder());
            var client = SelectHttpClient(true);

            var response = await client.GetAsync(relativeUri);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be(relativeUri);
        }
    }
}
