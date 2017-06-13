using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amockibia.Extensions;
using Amockibia.Setup;
using Amockibia.Test.Core.Utilities;
using Amockibia.Test.Extensions.Utilities;
using FluentAssertions;
using Xunit;

namespace Amockibia.Test.Extensions
{
    public class Example : TestBase
    {
        public Example()
        {
            Client = SelectHttpClient(true);
        }

        private HttpClient Client { get; }

        [Fact]
        public async Task demonstrate_rules()
        {
            Server.Setup(When.Post("/users?id=1")
                .Send(HttpStatusCode.Created)
                .WithHeader("Location", "/users/1")
                .WithPayloadObject(new {Id = 1})
                .WithMatchTimesUntilExpire(1)
                .WithPriority(1)
                .WithId("create user"));
            Server.Setup(When.Post("/users?id=1")
                .Send(HttpStatusCode.Forbidden)
                .WithMatchTimesUntilExpire(1)
                .WithPriority(2)
                .WithId("create user failed"));

            var response = await Client.PostAsync("/users?id=1", new ObjectContent(new {Value = "create request"}));
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            (await Server.Retrieve("create user")
                    .Requests
                    .First()
                    .Body
                    .ReadAsAnonymousTypeAsync(new {Value = default(string)}))
                .Value.Should().Be("create request");
            
            var failedResponse = await Client.PostAsync("/users?id=1", new ObjectContent(new {Value = "create request"}));
            failedResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task demonstrate_multiple_hosts()
        {
            Server.Setup(When.Get("http://miffyliye.org").Send((HttpStatusCode) 200));
            Server.Setup(When.Get("http://miffyliye.org:8080").Send((HttpStatusCode) 201));
            Server.Setup(When.Get("https://miffyliye.org").Send((HttpStatusCode) 202));
            Server.Setup(When.Get("http://miffyliye.com").Send((HttpStatusCode) 203));
            Server.Setup(When.Get("https://miffyliye.com:4433").Send((HttpStatusCode) 204));
            Server.Setup(When.Get("https://miffyliye.com").Send((HttpStatusCode) 205));

            (await Client.GetAsync("http://miffyliye.org"))
                .StatusCode.Should().Be((HttpStatusCode) 200);
            (await Client.GetAsync("http://miffyliye.org:8080"))
                .StatusCode.Should().Be((HttpStatusCode) 201);
            (await Client.GetAsync("https://miffyliye.org"))
                .StatusCode.Should().Be((HttpStatusCode) 202);
            (await Client.GetAsync("http://miffyliye.com"))
                .StatusCode.Should().Be((HttpStatusCode) 203);
            (await Client.GetAsync("https://miffyliye.com:4433"))
                .StatusCode.Should().Be((HttpStatusCode) 204);
            (await Client.GetAsync("https://miffyliye.com"))
                .StatusCode.Should().Be((HttpStatusCode) 205);
        }
    }
}
