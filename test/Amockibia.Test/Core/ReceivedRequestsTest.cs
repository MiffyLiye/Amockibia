using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amockibia.Rule;
using Amockibia.Rule.Builder;
using Amockibia.Test.Core.Utilities;
using FluentAssertions;
using Xunit;

namespace Amockibia.Test.Core
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
            Server.Setup(new NamedRuleBuilder(handlerId));
            var client = SelectHttpClient(true);

            await client.GetAsync(relativeUri);

            var receivedRequests = Server.GetHandler(handlerId).ReceivedRequests;
            receivedRequests.Single().Path.ToString().Should().Be(relativeUri);
        }

        [Fact]
        public async Task should_get_received_requests_in_order_for_verification()
        {
            var handlerId = "Get OK";
            Server.Setup(new NamedRuleBuilder(handlerId));
            var client = SelectHttpClient(true);

            await client.GetAsync("/first");
            await client.GetAsync("/last");

            var receivedRequests = Server.GetHandler(handlerId).ReceivedRequests;
            receivedRequests.Count.Should().Be(2);
            receivedRequests.First().Path.ToString().Should().Be("/first");
            receivedRequests.Last().Path.ToString().Should().Be("/last");
        }
        
        [Fact]
        public async Task should_get_received_request_contents_in_order_for_verification()
        {
            var handlerId = "Get OK";
            Server.Setup(new NamedRuleBuilder(handlerId));
            var client = SelectHttpClient(true);

            await client.PostAsync("/", new StringContent("first"));
            await client.PostAsync("/", new StringContent("last"));

            var receivedRequests = Server.GetHandler(handlerId).ReceivedRequests;
            receivedRequests.Count.Should().Be(2);
            (await receivedRequests.First().Body.ReadAsStringAsync()).Should().Be("first");
            (await receivedRequests.Last().Body.ReadAsStringAsync()).Should().Be("last");
        }
    }
}
