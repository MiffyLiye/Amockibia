using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amockibia.Extensions;
using FluentAssertions;
using Xunit;

namespace Amockibia.Test.Extensions
{
    public class MatchUriQueryTest : TestBase
    {
        public MatchUriQueryTest()
        {
            Client = SelectHttpClient(true);
        }

        private HttpClient Client { get; }

        [Fact]
        public async Task should_match_same_uri_with_given_parameters()
        {
            Server.Setup(When.Get("relative-stub-uri?name=miffyliye").SendOK());

            var response = await Client.GetAsync("relative-stub-uri?name=miffyliye&id=1");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        
        [Fact]
        public async Task should_not_match_uri_without_given_parameters()
        {
            Server.Setup(When.Get("relative-stub-uri?name=miffyliye").SendOK());

            var response = await Client.GetAsync("relative-stub-uri");
            response.StatusCode.Should().Be(HttpStatusCode.NotImplemented);
        }
        
        [Fact]
        public async Task should_match_uri_with_given_different_parameters_not_in_order()
        {
            Server.Setup(When.Get("relative-stub-uri?name=miffyliye&id=1").SendOK());

            var response = await Client.GetAsync("relative-stub-uri?id=1&name=miffyliye");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        
        [Fact]
        public async Task should_match_uri_with_given_same_parameters_in_order()
        {
            Server.Setup(When.Get("relative-stub-uri?id=0&id=1,2").SendOK());

            var response = await Client.GetAsync("relative-stub-uri?id=0&id=1,2");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        
        [Fact]
        public async Task should_not_match_uri_with_given_same_parameters_not_in_order()
        {
            Server.Setup(When.Get("relative-stub-uri?id=0&id=1,2").SendOK());

            var response = await Client.GetAsync("relative-stub-uri?id=1,2&id=0");
            response.StatusCode.Should().Be(HttpStatusCode.NotImplemented);
        }
    }
}
