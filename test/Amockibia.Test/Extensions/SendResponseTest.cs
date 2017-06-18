using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Amockibia.Extensions;
using Amockibia.Test.Extensions.Utilities;
using Microsoft.Net.Http.Headers;
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
        [InlineData(10)]
        [InlineData(20)]
        public async Task should_send_http_general_headers(int maxAge)
        {
            Server.Setup(When.Post("stub-uri")
                .Send(HttpStatusCode.Created)
                .WithHeader(HeaderNames.CacheControl, $"max-age={maxAge}"));

            var response = await Client.PostAsync("stub-uri", null);
            response.Headers.CacheControl.MaxAge.Should().Be(TimeSpan.FromSeconds(maxAge));
        }

        [Theory]
        [InlineData("article/1")]
        [InlineData("article/2")]
        public async Task should_send_http_response_headers(string location)
        {
            Server.Setup(When.Post("stub-uri")
                .Send(HttpStatusCode.Created)
                .WithHeader(HeaderNames.Location, location));

            var response = await Client.PostAsync("stub-uri", null);
            response.Headers.Location.Should().Be(location);
        }

        [Fact]
        public async Task should_send_http_entity_headers_and_message_body()
        {
            var message = "window.start = new Date()";
            var messageBody = new MemoryStream(Encoding.UTF8.GetBytes(message));
            var messageMd5 = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(message));
            Server.Setup(When.Get("script")
                .Send(HttpStatusCode.OK)
                .WithHeader(HeaderNames.Allow, "GET, HEAD")
                .WithHeader(HeaderNames.ContentEncoding, "identity")
                .WithHeader(HeaderNames.ContentLanguage, "en")
                .WithHeader(HeaderNames.ContentLength, messageBody.Length.ToString())
                .WithHeader(HeaderNames.ContentLocation, "/script/1")
                .WithHeader(HeaderNames.ContentMD5, BitConverter.ToString(messageMd5).Replace("-", string.Empty))
                .WithHeader(HeaderNames.ContentRange, $"bytes 0-{messageBody.Length}/{messageBody.Length}")
                .WithHeader(HeaderNames.ContentType, "application/javascript")
                .WithHeader(HeaderNames.Expires, "Mon, 31 Dec 2012 12:00:00 UTC")
                .WithHeader(HeaderNames.LastModified, "Mon, 31 Dec 2012 10:00:00 UTC")
                .WithBody(messageBody));

            var response = await Client.GetAsync("script");
            var entityHeaders = response.Content.Headers;
            entityHeaders.ContentType
                .ToString()
                .Should().Be("application/javascript");
            entityHeaders.Allow
                .Should().BeEquivalentTo(new [] { "GET", "HEAD" });
            entityHeaders.ContentEncoding
                .Should().BeEquivalentTo(new[] { "identity" });
            entityHeaders.ContentLanguage
                .Should().BeEquivalentTo(new[] { "en" });
            entityHeaders.ContentLength
                .Should().Be(messageBody.Length);
            entityHeaders.ContentLocation
                .ToString()
                .Should().Be("/script/1");
            // entityHeaders.ContentMD5
            //    .Should().Equal(messageMd5);
            // entityHeaders.ContentRange
            //    .Should().Be(new ContentRangeHeaderValue(0, messageBody.Length, messageBody.Length));
            // entityHeaders.Expires
            //    .Should().Be(new DateTimeOffset(new DateTime(2012, 12, 31, 12, 0, 0), TimeSpan.FromMinutes(0)));
            // entityHeaders.LastModified
            //    .Should().Be(new DateTimeOffset(new DateTime(2012, 12, 31, 10, 0, 0), TimeSpan.FromMinutes(0)));
            var script = await response.Content.ReadAsStringAsync();
            script.Should().Be(message);
        }

        [Theory]
        [InlineData("max-age=10")]
        [InlineData("max-age=20")]
        public async Task should_send_http_custom_headers(string customValue)
        {
            Server.Setup(When.Post("stub-uri")
                .Send(HttpStatusCode.Created)
                .WithHeader("Strict-Transport-Security", customValue));

            var response = await Client.PostAsync("stub-uri", null);
            response.Headers.GetValues("Strict-Transport-Security").Single().Should().Be(customValue);
        }
    }
}
