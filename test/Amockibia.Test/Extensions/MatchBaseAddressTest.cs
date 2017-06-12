using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amockibia.Extensions;
using Amockibia.Setup;
using FluentAssertions;
using Xunit;

namespace Amockibia.Test.Extensions
{
    public class MatchBaseAddressTest : TestBase
    {
        public MatchBaseAddressTest() : base("https://miffyliye.org/test/")
        {
            Client = SelectHttpClient(true);
        }

        private HttpClient Client { get; }

        [Fact]
        public async Task should_match_same_absolute_uri()
        {
            Server.Setup(When.Get("http://miffyliye.org/example").SendOK());

            var response = await Client.GetAsync("http://miffyliye.org/example");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        
        [Fact]
        public async Task should_not_match_different_host()
        {
            Server.Setup(When.Get("https://miffyliye.org/test/").SendOK());

            var response = await Client.GetAsync("https://miffyliye.com/test/");
            response.StatusCode.Should().Be(HttpStatusCode.NotImplemented);
        }
        
        [Fact]
        public async Task should_not_match_different_scheme()
        {
            Server.Setup(When.Get("https://miffyliye.org/test/").SendOK());

            var response = await Client.GetAsync("http://miffyliye.org/test/");
            response.StatusCode.Should().Be(HttpStatusCode.NotImplemented);
        }
        
        [Fact]
        public async Task should_match_multiple_hosts_in_memory_mode()
        {
            Server.Setup(When.Get("http://miffyliye.org/test/").Send(HttpStatusCode.OK));
            Server.Setup(When.Get("http://miffyliye.com/test/").Send(HttpStatusCode.Accepted));

            (await Client.GetAsync("http://miffyliye.org/test/"))
                .StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync("http://miffyliye.com/test/"))
                .StatusCode.Should().Be(HttpStatusCode.Accepted);
        }
    }
}
