using FluentAssertions;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Amockibia.Test
{
    public class Example : IDisposable
    {
        private static object Locker = new object();
        private static int PortNumber = 4000;
        private Uri BaseAddress { get; }
        private AmockibiaServer Server { get; }
        private Lazy<HttpClient> InMemoryClient { get; }
        private Lazy<HttpClient> SelfHostClient { get; }

        public Example()
        {
            lock (Locker)
            {
                BaseAddress = new Uri($"http://localhost:{PortNumber}/");
                PortNumber += 1;
            }
            Server = new AmockibiaServer(BaseAddress);
            InMemoryClient = new Lazy<HttpClient>(() => Server.CreateInMemoryClient());
            SelfHostClient = new Lazy<HttpClient>(() => { Server.StartSelfHost(); return new HttpClient { BaseAddress = BaseAddress }; });
        }

        public void Dispose()
        {
            if (InMemoryClient.IsValueCreated) InMemoryClient.Value.Dispose();
            if (SelfHostClient.IsValueCreated) SelfHostClient.Value.Dispose();
            Server.Dispose();
        }

        private HttpClient SelectHttpClient(bool isInMemoryHost)
        {
            return isInMemoryHost ? InMemoryClient.Value : SelfHostClient.Value;
        }

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
