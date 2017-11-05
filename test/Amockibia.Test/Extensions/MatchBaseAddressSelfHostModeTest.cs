using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amockibia.Extensions;
using FluentAssertions;
using Xunit;

namespace Amockibia.Test.Extensions
{
    public class MatchBaseAddressSelfHostModeTest : TestBase
    {
        public MatchBaseAddressSelfHostModeTest() : base("http://localhost:3999/test/")
        {
            Client = SelectHttpClient(false);
        }

        private HttpClient Client { get; }

        [Theory]
        [InlineData("http://localhost:3999/test/", "http://localhost:3999/test/")]
        public async Task should_match_same_host(string stubUri, string requestUri)
        {
            Server.Setup(When.Get(stubUri).SendOK());

            var response = await Client.GetAsync(requestUri);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("http://localhost:3999/test/", "http://localhost:4000/test/")]
        public async Task should_not_match_different_host(string stubUri, string requestUri)
        {
            Server.Setup(When.Get(stubUri).SendOK());

            async Task Action() => await Client.GetAsync(requestUri);
            
            await Assert.ThrowsAsync<HttpRequestException>(Action);
        }
    }
}
