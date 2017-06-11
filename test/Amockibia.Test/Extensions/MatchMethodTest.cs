using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Amockibia.Extensions;
using Amockibia.Setup;
using FluentAssertions;
using Xunit;

namespace Amockibia.Test.Extensions
{
    public class MatchMethodTest : TestBase
    {
        public MatchMethodTest()
        {
            Client = SelectHttpClient(true);
        }

        private HttpClient Client { get; }

        [Theory]
        [InlineData("Get")]
        [InlineData("Post")]
        [InlineData("Put")]
        [InlineData("Delete")]
        public async Task should_match_same_method(string httpMethodName)
        {
            var httpMethod = ToHttpMethod(httpMethodName);
            Server.Setup(When.Receive(httpMethod, "stub-uri").SendOK());

            var response = await Client.SendAsync(new HttpRequestMessage(httpMethod, "stub-uri"));
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        
        [Theory]
        [InlineData("Get")]
        [InlineData("Post")]
        [InlineData("Put")]
        [InlineData("Delete")]
        public async Task should_not_match_different_method(string httpMethodName)
        {
            Server.Setup(When.Receive(HttpMethod.Options, "stub-uri").SendOK());

            var httpMethod = ToHttpMethod(httpMethodName);
            var response = await Client.SendAsync(new HttpRequestMessage(httpMethod, "stub-uri"));
            response.StatusCode.Should().Be(HttpStatusCode.NotImplemented);
        }

        private static HttpMethod ToHttpMethod(string httpMethodName)
        {
            return (HttpMethod) typeof(HttpMethod).GetProperty(httpMethodName).GetValue(null, null);
        }
    }
}
