using Microsoft.AspNetCore.Hosting;
using System;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using Amockibia.Utilities;
using Amockibia.Rule;

namespace Amockibia
{
    public class AmockibiaServer : IDisposable
    {
        private static object Locker { get; } = new object();
        private static int NextID = 1;
        public Uri BaseAddress { get; }
        private string ServerID { get; }
        private Lazy<IWebHost> SelfHost { get; }
        private Lazy<TestServer> InMemoryHost { get; }

        public AmockibiaServer(Uri baseAddress)
        {
            lock (Locker)
            {
                ServerID = NextID.ToString();
                NextID += 1;

            }
            var config = ServerID.GetConfig(this);
            ServerID.GetConfig().Rules.Add(new DefaultRule());

            BaseAddress = baseAddress;

            SelfHost = new Lazy<IWebHost>(() =>
            new WebHostBuilder()
                .UseKestrel()
                .UseStartup(typeof(Startup))
                .UseEnvironment(ServerID)
                .Start(new[] { BaseAddress.ToString() }));

            InMemoryHost = new Lazy<TestServer>(() =>
            new TestServer(new WebHostBuilder()
                .UseStartup(typeof(Startup))
                .UseEnvironment(ServerID)));
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
            ServerID.Stop();
        }

        public void Stub(RuleBuilder builder)
        {
            ServerID.GetConfig().Rules.Add(builder.Build(ServerID));
        }
    }
}
