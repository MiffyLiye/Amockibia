using Amockibia.Rule;
using Amockibia.Rule.Builder;
using Amockibia.Stub;
using Amockibia.Test.Utilities;
using FluentAssertions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Amockibia.Test
{
    public class PriorityTest : TestBase
    {
        private class PriorityRuleBuilder : IRuleBuildable
        {
            private HttpStatusCode StatusCode { get; }
            private int Priority { get; }
            public PriorityRuleBuilder(HttpStatusCode statusCode, int priority)
            {
                StatusCode = statusCode;
                Priority = priority;
            }
            public RequestHandler Build(string serverId)
            {
                return new RequestHandler(new AlwaysMatchMatcher(), new StatusCodeOnlyResponder(StatusCode), Priority, 1);
            }
        }

        [Fact]
        public async Task should_get_response_from_high_priority_handler_until_it_expires()
        {
            Server.Stub(new PriorityRuleBuilder(HttpStatusCode.OK, 1));
            Server.Stub(new PriorityRuleBuilder(HttpStatusCode.NoContent, 2));

            var client = SelectHttpClient(true);

            (await client.GetAsync("")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await client.GetAsync("")).StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
