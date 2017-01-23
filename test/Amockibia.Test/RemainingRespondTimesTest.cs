using System.Threading.Tasks;
using Amockibia.Rule;
using Amockibia.Rule.Builder;
using Amockibia.Stub;
using Xunit;
using System.Net;
using FluentAssertions;
using System.Linq;
using Amockibia.Test.Utilities;

namespace Amockibia.Test
{
    public class RemainingRespondTimesTest : TestBase
    {
        private class LimitedTimesOKRuleBuilder : IRuleBuildable
        {
            private int Limit { get; }
            public LimitedTimesOKRuleBuilder(int limit)
            {
                Limit = limit;
            }
            public RequestHandler Build(string serverId)
            {
                return new RequestHandler(new AlwaysMatchMatcher(), new StatusCodeOnlyResponder(HttpStatusCode.OK), remainingRespondTimes: Limit);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task should_return_stub_until_remaining_respond_times_becomes_zero(int limit)
        {
            Server.Stub(new LimitedTimesOKRuleBuilder(limit));
            var client = SelectHttpClient(true);

            foreach (var i in Enumerable.Range(1, limit))
            {
                (await client.GetAsync("")).StatusCode.Should().Be(HttpStatusCode.OK);
            }

            (await client.GetAsync("")).StatusCode.Should().Be(HttpStatusCode.NotImplemented);
        }

        [Fact]
        public async Task should_always_return_stub_when_remaining_respond_times_is_negative_one_which_means_infinite()
        {
            Server.Stub(new LimitedTimesOKRuleBuilder(-1));
            var client = SelectHttpClient(true);

            foreach (var i in Enumerable.Range(1, 3))
            {
                (await client.GetAsync("")).StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }
    }
}
