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
            var messageMd5 = MD5.Create().ComputeHash(messageBody);
            var expires = new DateTimeOffset(2012, 12, 31, 12, 0, 0, TimeSpan.FromMinutes(0));
            var lastModified = new DateTimeOffset(2012, 12, 31, 10, 0, 0, TimeSpan.FromMinutes(0));
            Server.Setup(When.Get("script")
                .Send(HttpStatusCode.OK)
                .WithHeader(HeaderNames.Allow, "GET, HEAD")
                .WithHeader(HeaderNames.ContentEncoding, "identity")
                .WithHeader(HeaderNames.ContentLanguage, "en")
                .WithHeader(HeaderNames.ContentLength, messageBody.Length.ToString())
                .WithHeader(HeaderNames.ContentLocation, "/script/1")
                .WithHeader(HeaderNames.ContentMD5, Convert.ToBase64String(messageMd5))
                .WithHeader(HeaderNames.ContentRange, $"bytes 0-{messageBody.Length}/{messageBody.Length}")
                .WithHeader(HeaderNames.ContentType, "application/javascript")
                .WithHeader(HeaderNames.Expires, expires.ToString("R"))
                .WithHeader(HeaderNames.LastModified, lastModified.ToString("R"))
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
            entityHeaders.Expires
                .Should().Be(expires);
            entityHeaders.LastModified
                .Should().Be(lastModified);
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

        [Fact]
        public async Task should_send_multiple_headers_with_same_key()
        {
            Server.Setup(When.Post("stub-uri")
                .Send(HttpStatusCode.Created)
                .WithHeader("Set-Cookie", "sessionToken=001")
                .WithHeader("Set-Cookie", "culture=en"));

            var response = await Client.PostAsync("stub-uri", null);
            response.Headers.GetValues("Set-Cookie")
                .Should().Equal(new[] { "sessionToken=001", "culture=en" });
        }
    }
}
