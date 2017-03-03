using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amockibia.Extensions;
using Amockibia.Stub;
using FluentAssertions;
using Xunit;

namespace Amockibia.Test.Extensions
{
    public class MatchUriTest : TestBase
    {
        public MatchUriTest() : base("/base-uri/")
        {
            Client = SelectHttpClient(true);
        }

        private HttpClient Client { get; }

        [Fact]
        public async Task should_match_same_relative_uri()
        {
            Server.Stub(When.Get("relative-stub-uri").RespondOK());

            var response = await Client.GetAsync("relative-stub-uri");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task should_match_same_absolute_uri()
        {
            Server.Stub(When.Get("relative-stub-uri").RespondOK());

            var response = await Client.GetAsync($"{Server.BaseAddress}relative-stub-uri");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("")]
        [InlineData("stub=1")]
        [InlineData("stub=1&uri=2")]
        public async Task should_match_same_uri_and_ignore_additional_query(string query)
        {
            Server.Stub(When.Get("stub-uri").RespondOK());

            var response = await Client.GetAsync($"stub-uri?{query}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("")]
        [InlineData("hash?stub=1")]
        [InlineData("hash?stub=1&uri=2")]
        public async Task should_match_same_uri_and_ignore_additional_hash(string hash)
        {
            Server.Stub(When.Get("stub-uri").RespondOK());

            var response = await Client.GetAsync($"stub-uri#{hash}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("")]
        [InlineData("stub-uri-after")]
        [InlineData("before-stub-uri")]
        [InlineData("/stub-uri")]
        public async Task should_not_match_different_uri(string uri)
        {
            Server.Stub(When.Get("stub-uri").RespondOK());

            var response = await Client.GetAsync(uri);

            response.StatusCode.Should().Be(HttpStatusCode.NotImplemented);
        }

        [Fact]
        public async Task should_not_match_different_http_method()
        {
            Server.Stub(When.Get("stub-uri").RespondOK());

            var response = await Client.PostAsync("stub-uri", null);

            response.StatusCode.Should().Be(HttpStatusCode.NotImplemented);
        }
    }
}
