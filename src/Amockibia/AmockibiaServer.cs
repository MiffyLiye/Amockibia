using System;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Amockibia.Utilities;
using Amockibia.Rule;
using Amockibia.Rule.Builder;
using Amockibia.Server;

namespace Amockibia
{
    public class AmockibiaServer : IDisposable
    {
        public string ServerId { get; }
        public Uri BaseAddress { get; }
        private static object Locker { get; } = new object();
        private static int NextId { get; set; } = 1;
        private Lazy<IWebHost> SelfHost { get; }
        private Lazy<TestServer> InMemoryHost { get; }

        public AmockibiaServer(Uri baseAddress)
        {
            lock (Locker)
            {
                ServerId = NextId.ToString();
                NextId += 1;

            }
            var config = ServerId.GetConfig(this);
            config.Rules.Add(new RequestHandler(new AlwaysMatchMatcher(), new NotImplementedResponder(), int.MaxValue, -1));

            BaseAddress = baseAddress;

            SelfHost = new Lazy<IWebHost>(() =>
            new WebHostBuilder()
                .UseKestrel()
                .UseStartup(typeof(Startup))
                .UseEnvironment(ServerId)
                .Start(new[] { BaseAddress.ToString() }));

            InMemoryHost = new Lazy<TestServer>(() =>
            new TestServer(new WebHostBuilder()
                .UseStartup(typeof(Startup))
                .UseEnvironment(ServerId)));
        }
        
        public void Setup(IRuleBuildable builder)
        {
            ServerId.GetConfig().Rules.Add(builder.Build(ServerId));
        }
        
        public RequestHandler GetHandler(string handlerId)
        {
            return ServerId.GetConfig().Rules.Single(r => r.Id == handlerId);
        }

        public void StartSelfHost()
        {
            SelfHost.Value.Ignore();
        }

        [Obsolete("Use CreateInMemoryClient() to trigger in memory host.")]
        public void StartInMemoryHost()
        {
            InMemoryHost.Value.Ignore();
        }

        public HttpClient CreateInMemoryClient()
        {
            var client = InMemoryHost.Value.CreateClient();
            // Reset BaseAddress because TestServer ignored port number.
            client.BaseAddress = BaseAddress;
            return client;
        }

        public void Dispose()
        {
            if (SelfHost.IsValueCreated) SelfHost.Value.Dispose();
            if (InMemoryHost.IsValueCreated) InMemoryHost.Value.Dispose();
            ServerId.Stop();
        }
    }
}
