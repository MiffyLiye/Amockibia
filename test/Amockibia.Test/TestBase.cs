using System;
using System.Net.Http;

namespace Amockibia.Test
{
    public abstract class TestBase : IDisposable
    {
        private static object Locker { get; } = new object();
        private static int NextPortNumber { get; set; } = 4000;
        private Uri BaseAddress { get; }
        protected AmockibiaServer Server { get; }
        private Lazy<HttpClient> InMemoryClient { get; }
        private Lazy<HttpClient> SelfHostClient { get; }

        protected TestBase(string baseUrl = "")
        {
            if (Uri.IsWellFormedUriString(baseUrl, UriKind.Absolute))
            {
                BaseAddress = new Uri(baseUrl);
            }
            else
            {
                baseUrl = baseUrl.StartsWith("/") ? baseUrl : "/" + baseUrl;
                lock (Locker)
                {
                    BaseAddress = new Uri($"http://localhost:{NextPortNumber}/{baseUrl}");
                    NextPortNumber += 1;
                }
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
