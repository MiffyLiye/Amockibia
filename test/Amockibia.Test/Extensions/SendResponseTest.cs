using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amockibia.Extensions;
using Amockibia.Setup;
using Amockibia.Test.Extensions.Utilities;
using FluentAssertions;
using Xunit;

namespace Amockibia.Test.Extensions
{
    public class SendResponseTest : TestBase
    {
        public SendResponseTest()
        {
            Client = SelectHttpClient(true);
        }

        private HttpClient Client { get; }

        [Theory]
        [InlineData(HttpStatusCode.OK)]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.InternalServerError)]
        public async Task should_send_http_status_code(HttpStatusCode httpStatusCode)
        {
            Server.Setup(When.Get("stub-uri").Send(httpStatusCode));
            var response = await Client.GetAsync("stub-uri");

            response.StatusCode.Should().Be(httpStatusCode);
        }
        
        [Theory]
        [InlineData("good")]
        [InlineData("very good")]
        public async Task should_send_body(string value)
        {
            Server.Setup(When.Get("stub-uri").SendOK().WithPayloadObject(new {Value = value}));
            
            var response = await Client.GetAsync("stub-uri");
            var payload = await response.Content.ReadAsAnonymousTypeAsync(new {Value = default(string)});
            payload.Value.Should().Be(value);
        }
        
        [Theory]
        [InlineData("/article/1")]
        [InlineData("/article/2")]
        public async Task should_send_headers(string location)
        {
            Server.Setup(When.Post("stub-uri")
                .Send(HttpStatusCode.Created)
                .WithHeader("Location", location)
                .WithHeader("CustomKey", "custom value"));
            
            var response = await Client.PostAsync("stub-uri", null);
            response.Headers.Location.Should().Be(location);
            response.Headers.GetValues("CustomKey").Single().Should().Be("custom value");
        }
    }
}
