using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amockibia.Extensions;
using FluentAssertions;
using Xunit;

namespace Amockibia.Test.Extensions
{
    public class MatchUriTest : TestBase
    {
        public MatchUriTest() : base("/base-uri/")
        {
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task should_match_same_relative_uri(bool useInMemoryClient)
        {
            Server.Setup(When.Get("relative-stub-uri").SendOK());
            var client = SelectHttpClient(useInMemoryClient);

            var response = await client.GetAsync("relative-stub-uri");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task should_match_same_absolute_uri(bool useInMemoryClient)
        {
            Server.Setup(When.Get("relative-stub-uri").SendOK());
            var client = SelectHttpClient(useInMemoryClient);

            var response = await client.GetAsync($"{Server.BaseAddress}relative-stub-uri");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [InlineData(true, "")]
        [InlineData(false, "")]
        [InlineData(true, "stub=1")]
        [InlineData(false, "stub=1")]
        [InlineData(true, "stub=1&uri=2")]
        [InlineData(false, "stub=1&uri=2")]
        public async Task should_match_same_uri_and_ignore_additional_query(bool useInMemoryClient, string query)
        {
            Server.Setup(When.Get("stub-uri").SendOK());
            var client = SelectHttpClient(useInMemoryClient);

            var response = await client.GetAsync($"stub-uri?{query}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [InlineData(true, "")]
        [InlineData(false, "")]
        [InlineData(true, "?stub=1#hash")]
        [InlineData(false, "?stub=1#hash")]
        [InlineData(true, "?stub=1&uri=2#hash")]
        [InlineData(false, "?stub=1&uri=2#hash")]
        public async Task should_match_same_uri_and_ignore_additional_hash(bool useInMemoryClient, string queryAndHash)
        {
            Server.Setup(When.Get("stub-uri").SendOK());
            var client = SelectHttpClient(useInMemoryClient);

            var response = await client.GetAsync($"stub-uri{queryAndHash}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [InlineData(true, "")]
        [InlineData(false, "")]
        [InlineData(true, "stub-uri-after")]
        [InlineData(false, "stub-uri-after")]
        [InlineData(true, "before-stub-uri")]
        [InlineData(false, "before-stub-uri")]
        [InlineData(true, "/stub-uri")]
        [InlineData(false, "/stub-uri")]
        public async Task should_not_match_different_uri(bool useInMemoryClient, string uri)
        {
            Server.Setup(When.Get("stub-uri").SendOK());
            var client = SelectHttpClient(useInMemoryClient);
            
            var response = await client.GetAsync(uri);
            response.StatusCode.Should().Be(HttpStatusCode.NotImplemented);
        }
    }
}
