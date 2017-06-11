using System.Net.Http;
using System.Threading.Tasks;
using Amockibia.Extensions;
using Amockibia.Setup;
using Amockibia.Test.Extensions.Utilities;
using FluentAssertions;
using Xunit;

namespace Amockibia.Test.Extensions
{
    public class RulePriorityTest : TestBase
    {
        public RulePriorityTest()
        {
            Client = SelectHttpClient(true);
        }

        private HttpClient Client { get; }

        [Theory]
        [InlineData(100, 200, "first")]
        [InlineData(200, 100, "last")]
        [InlineData(-1, 0, "first")]
        public async Task should_match_high_priority_rule_first(int firstPriority, int lastPriority, string expected)
        {
            Server.Setup(When.Get("stub-uri").SendOK().WithPayloadObject("first").WithPriority(firstPriority));
            Server.Setup(When.Get("stub-uri").SendOK().WithPayloadObject("last").WithPriority(lastPriority));
            var response = await Client.GetAsync("stub-uri");

            var payload = await response.Content.ReadAsAsync<string>();
            payload.Should().Be(expected);
        }
    }
}
