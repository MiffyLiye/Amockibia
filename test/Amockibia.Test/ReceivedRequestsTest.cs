using Amockibia.Rule;
using Amockibia.Rule.Builder;
using Amockibia.Stub;
using Amockibia.Verify;
using Amockibia.Test.Utilities;
using FluentAssertions;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Amockibia.Test
{
    public class ReceivedRequestsTest : TestBase
    {
        private class NamedRuleBuilder : IRuleBuildable
        {
            private string Id { get; }

            public NamedRuleBuilder(string id)
            {
                Id = id;
            }

            public RequestHandler Build(string serverId)
            {
                return new RequestHandler(new AlwaysMatchMatcher(), new StatusCodeOnlyResponder(HttpStatusCode.OK), id: Id);
            }
        }

        [Theory]
        [InlineData("/first")]
        [InlineData("/second")]
        public async Task should_get_received_requests_for_verification(string relativeUri)
        {
            var handlerId = "Get OK";
            Server.Stub(new NamedRuleBuilder(handlerId));
            var client = SelectHttpClient(true);

            await client.GetAsync(relativeUri);

            var receivedRequests = Server.Handler(handlerId).ReceivedRequests;
            receivedRequests.Single().Path.ToString().Should().Be(relativeUri);
        }

        [Fact]
        public async Task should_get_received_requests_in_order_for_verification()
        {
            var handlerId = "Get OK";
            Server.Stub(new NamedRuleBuilder(handlerId));
            var client = SelectHttpClient(true);

            await client.GetAsync("/first");
            await client.GetAsync("/last");

            var receivedRequests = Server.Handler(handlerId).ReceivedRequests;
            receivedRequests.Count.Should().Be(2);
            receivedRequests.First().Path.ToString().Should().Be("/first");
            receivedRequests.Last().Path.ToString().Should().Be("/last");
        }
    }
}
