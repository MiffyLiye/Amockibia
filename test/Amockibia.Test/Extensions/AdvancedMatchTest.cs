using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amockibia.Extensions;
using Microsoft.Extensions.Primitives;
using FluentAssertions;
using Xunit;

namespace Amockibia.Test.Extensions
{
    public class AdvancedMatchTest : TestBase
    {
        public AdvancedMatchTest()
        {
            Client = SelectHttpClient(true);
        }

        private HttpClient Client { get; }

        [Theory]
        [InlineData("secret_key", HttpStatusCode.OK)]
        [InlineData("wrong_key", HttpStatusCode.NotImplemented)]
        public async Task should_match_with_advanced_predicate(string actualAuthKey, HttpStatusCode statusCode)
        {
            Server.Setup(When.Receive(r => {
                var value = default(StringValues);
                var hasHeader = r.Headers.TryGetValue("AuthKey", out value);
                return hasHeader && value.Count == 1 && value.First() == "secret_key";
            }).SendOK());

            var request = new HttpRequestMessage {
                RequestUri = new Uri(Server.BaseAddress, "/advanced"),
                Method = HttpMethod.Get
            };
            request.Headers.Add("AuthKey", actualAuthKey);
            var response = await Client.SendAsync(request);
            response.StatusCode.Should().Be(statusCode);
        }

        [Theory]
        [InlineData("/advanced", HttpStatusCode.OK)]
        [InlineData("/mismatch-advanced", HttpStatusCode.NotImplemented)]
        public async Task should_match_with_primary_predicate_and_advanced_predicate(string relativeUri, HttpStatusCode statusCode)
        {
            Server.Setup(When.Put("/advanced").WithRequest(r => {
                var authKey = default(StringValues);
                var hasHeader = r.Headers.TryGetValue("AuthKey", out authKey);
                return hasHeader && authKey.Count == 1 && authKey.First() == "secret_key";
            }).SendOK());

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(Server.BaseAddress, relativeUri),
                Method = HttpMethod.Put
            };
            request.Headers.Add("AuthKey", "secret_key");
            var response = await Client.SendAsync(request);
            response.StatusCode.Should().Be(statusCode);
        }

        [Theory]
        [InlineData("secret_key", "MiffyLiye", HttpStatusCode.OK)]
        [InlineData("wrong_key", "MiffyLiye", HttpStatusCode.NotImplemented)]
        [InlineData("secret_key", "wrong_id", HttpStatusCode.NotImplemented)]
        public async Task should_match_with_multiple_and_advanced_predicates(string actualAuthKey, string actualId, HttpStatusCode statusCode)
        {
            Server.Setup(When.Get("/advanced")
                .WithRequest(r => {
                    var authKey = default(StringValues);
                    var hasHeader = r.Headers.TryGetValue("AuthKey", out authKey);
                    return hasHeader && authKey.Count == 1 && authKey.First() == "secret_key";
                })
                .WithRequest(r => {
                    var id = default(StringValues);
                    var hasHeader = r.Headers.TryGetValue("Id", out id);
                    return hasHeader && id.Count == 1 && id.First() == "MiffyLiye";
                })
                .SendOK());

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(Server.BaseAddress, "/advanced"),
                Method = HttpMethod.Get
            };
            request.Headers.Add("AuthKey", actualAuthKey);
            request.Headers.Add("Id", actualId);
            var response = await Client.SendAsync(request);
            response.StatusCode.Should().Be(statusCode);
        }
    }
}
