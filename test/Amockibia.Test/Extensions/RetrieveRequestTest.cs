using System.Linq;
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
    public class RetrieveRequestTest : TestBase
    {
        public RetrieveRequestTest()
        {
            Client = SelectHttpClient(true);
        }

        private HttpClient Client { get; }

        [Fact]
        public async Task should_retrieve_has_been_called()
        {
            Server.Setup(When.Delete("stub-uri").RespondOK().WithId("delete action"));
            await Client.DeleteAsync( "stub-uri");

            Server.Retrieve("delete action").HasBeenCalled.Should().BeTrue();
        }
        
        [Fact]
        public async Task should_retrieve_has_not_been_called()
        {
            Server.Setup(When.Delete("stub-uri").RespondOK().WithId("delete action"));
            await Client.GetAsync( "stub-uri");

            Server.Retrieve("delete action").HasBeenCalled.Should().BeFalse();
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task should_retrieve_has_been_called_times(int times)
        {
            Server.Setup(When.Delete("stub-uri").RespondOK().WithId("delete action"));
            for (var time = 0; time < times; time++)
            {
                await Client.DeleteAsync("stub-uri");
            }

            Server.Retrieve("delete action").HasBeenCalledTimes.Should().Be(times);
        }
        
        [Theory]
        [InlineData("create user")]
        [InlineData("send email")]
        public async Task should_retrieve_request_content(string name)
        {
            Server.Setup(When.Post("stub-uri").RespondOK().WithId("post action"));
            await Client.PostAsync("stub-uri", new ObjectContent(new {Name = name}));

            var data = await Server.Retrieve("post action").Requests.Single().Body
                .ReadAsAnonymousTypeAsync(new {Name = default(string)});
            
            data.Name.Should().Be(name);
        }
        
        [Fact]
        public async Task should_retrieve_request_contents_in_order()
        {
            Server.Setup(When.Post("stub-uri").RespondOK().WithId("post action"));
            await Client.PostAsync("stub-uri", new ObjectContent(new {Name = "first"}));
            await Client.PostAsync("stub-uri", new ObjectContent(new {Name = "last"}));

            var firstData = await Server.Retrieve("post action").Requests.First().Body
                .ReadAsAnonymousTypeAsync(new {Name = default(string)});
            firstData.Name.Should().Be("first");
            var lastData = await Server.Retrieve("post action").Requests.Last().Body
                .ReadAsAnonymousTypeAsync(new {Name = default(string)});
            lastData.Name.Should().Be("last");
        }
    }
}
