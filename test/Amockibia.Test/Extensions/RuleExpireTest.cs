using System.Net.Http;
using System.Threading.Tasks;
using Amockibia.Extensions;
using Amockibia.Test.Utilities;
using FluentAssertions;
using Xunit;

namespace Amockibia.Test.Extensions
{
    public class RuleExpireTest : TestBase
    {
        public RuleExpireTest()
        {
            Client = SelectHttpClient(true);
        }

        private HttpClient Client { get; }

        [Fact]
        public async Task should_expire_when_matched_times_reaches_available_matches_times()
        {
            Server.Setup(When.Get("stub-uri")
                .SendOK()
                .WithPayloadObject("first")
                .WithPriority(0)
                .WithMatchTimesUntilExpire(1));
            Server.Setup(When.Get("stub-uri")
                .SendOK()
                .WithPayloadObject("last")
                .WithPriority(1)
                .WithMatchTimesUntilExpire(1));
            
            var firstResponse = await Client.GetAsync("stub-uri");
            var firstPayload = await firstResponse.Content.ReadAsAsync<string>();
            firstPayload.Should().Be("first");
            
            var secondResponse = await Client.GetAsync("stub-uri");
            var secondPayload = await secondResponse.Content.ReadAsAsync<string>();
            secondPayload.Should().Be("last");
        }
    
        [Fact]
        public async Task should_not_expire_when_matched_times_not_reaches_available_matches_times()
        {
            Server.Setup(When.Get("stub-uri")
                .SendOK()
                .WithPayloadObject("1-2")
                .WithMatchTimesUntilExpire(2)
                .WithPriority(0));
            Server.Setup(When.Get("stub-uri")
                .SendOK()
                .WithPayloadObject("3")
                .WithMatchTimesUntilExpire(1)
                .WithPriority(1));
            
            var firstResponse = await Client.GetAsync("stub-uri");
            var firstPayload = await firstResponse.Content.ReadAsAsync<string>();
            firstPayload.Should().Be("1-2");
            
            var secondResponse = await Client.GetAsync("stub-uri");
            var secondPayload = await secondResponse.Content.ReadAsAsync<string>();
            secondPayload.Should().Be("1-2");
        }
    
    }
}
