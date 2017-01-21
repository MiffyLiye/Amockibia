using Amockibia.Extensions;
using FluentAssertions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Amockibia.Test
{
    public class Example : TestBase
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task should_return_not_implemented_when_request_not_matched(bool isInMemoryHost)
        {
            Server.Stub(When.Get("OK").RespondOK());
            var client = SelectHttpClient(isInMemoryHost);

            var response = await client.GetAsync("NotImplemented");

            response.StatusCode.Should().Be(HttpStatusCode.NotImplemented);
        }

        [Theory]
        [InlineData(true, "OK")]
        [InlineData(false, "OK")]
        public async Task should_return_stub_when_request_matched(bool isInMemoryHost, string relativeUri)
        {
            Server.Stub(When.Get(relativeUri).RespondOK());
            var client = SelectHttpClient(isInMemoryHost);

            var response = await client.GetAsync(relativeUri);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
