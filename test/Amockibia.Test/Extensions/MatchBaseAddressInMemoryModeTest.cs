using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amockibia.Extensions;
using FluentAssertions;
using Xunit;

namespace Amockibia.Test.Extensions
{
    public class MatchBaseAddressInMemoryModeTest : TestBase
    {
        public MatchBaseAddressInMemoryModeTest() : base("https://miffyliye.org/test/")
        {
            Client = SelectHttpClient(true);
        }

        private HttpClient Client { get; }

        [Theory]
        [InlineData("http://miffyliye.org/test/", "http://miffyliye.org/test/")]
        [InlineData("https://miffyliye.org/test/", "https://miffyliye.org/test/")]
        [InlineData("http://miffyliye.org:8080/test/", "http://miffyliye.org:8080/test/")]
        [InlineData("http://miffyliye.org/test/", "http://miffyliye.org:80/test/")]
        [InlineData("http://miffyliye.org:80/test/", "http://miffyliye.org/test/")]
        [InlineData("https://miffyliye.org/test/", "https://miffyliye.org:443/test/")]
        [InlineData("https://miffyliye.org:443/test/", "https://miffyliye.org/test/")]
        public async Task should_match_same_host(string stubUri, string requestUri)
        {
            Server.Setup(When.Get(stubUri).SendOK());

            var response = await Client.GetAsync(requestUri);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("http://miffyliye.org/test/", "http://miffyliye.com/test/")]
        [InlineData("https://miffyliye.org/test/", "https://miffyliye.com/test/")]
        [InlineData("http://miffyliye.org/test/", "https://miffyliye.org/test/")]
        [InlineData("http://miffyliye.org/test/", "http://miffyliye.org:8080/test/")]
        [InlineData("http://miffyliye.org:443/test/", "https://miffyliye.org/test/")]
        [InlineData("https://miffyliye.org/test/", "http://miffyliye.org:443/test/")]
        public async Task should_not_match_different_host(string stubUri, string requestUri)
        {
            Server.Setup(When.Get(stubUri).SendOK());

            var response = await Client.GetAsync(requestUri);
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
