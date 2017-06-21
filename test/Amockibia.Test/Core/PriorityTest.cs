using System.Net;
using System.Threading.Tasks;
using Amockibia.Rule;
using Amockibia.Rule.Builder;
using Amockibia.Test.Utilities;
using Amockibia.Utilities;
using FluentAssertions;
using Xunit;

namespace Amockibia.Test.Core
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
                serverId.Ignore();
                return new RequestHandler(new AlwaysMatchMatcher(), new StatusCodeOnlyResponder(StatusCode), Priority, 1);
            }
        }

        [Fact]
        public async Task should_get_response_from_high_priority_handler_until_it_expires()
        {
            Server.Setup(new PriorityRuleBuilder(HttpStatusCode.OK, 1));
            Server.Setup(new PriorityRuleBuilder(HttpStatusCode.NoContent, 2));

            var client = SelectHttpClient(true);

            (await client.GetAsync("")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await client.GetAsync("")).StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
        
        [Fact]
        public async Task should_match_first_rule_when_two_rules_with_same_priority_matches()
        {
            Server.Setup(new PriorityRuleBuilder(HttpStatusCode.OK, 1));
            Server.Setup(new PriorityRuleBuilder(HttpStatusCode.NoContent, 1));

            var client = SelectHttpClient(true);

            (await client.GetAsync("")).StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
