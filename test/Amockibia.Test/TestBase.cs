using System;
using System.Net.Http;

namespace Amockibia.Test
{
    public abstract class TestBase
    {
        private static object Locker = new object();
        private static int NextPortNumber = 4000;
        private Uri BaseAddress { get; }
        protected AmockibiaServer Server { get; }
        private Lazy<HttpClient> InMemoryClient { get; }
        private Lazy<HttpClient> SelfHostClient { get; }

        public TestBase(string baseRelativeUrl = "")
        {
            baseRelativeUrl = baseRelativeUrl.StartsWith("/") ? baseRelativeUrl : "/" + baseRelativeUrl;
            lock (Locker)
            {
                BaseAddress = new Uri($"http://localhost:{NextPortNumber}/{baseRelativeUrl}");
                NextPortNumber += 1;
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

        protected HttpClient SelectHttpClient(bool isInMemoryHost)
        {
            return isInMemoryHost ? InMemoryClient.Value : SelfHostClient.Value;
        }
    }
}
