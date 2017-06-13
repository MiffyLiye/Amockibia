using System;
using System.Collections.Concurrent;
using Amockibia.Server;

namespace Amockibia.Utilities
{
    internal static class ServerExtensions
    {
        private static Lazy<ConcurrentDictionary<string, ServerConfig>> Config { get; } = new Lazy<ConcurrentDictionary<string, ServerConfig>>(() => new ConcurrentDictionary<string, ServerConfig>());

        public static ServerConfig GetConfig(this string serverId, AmockibiaServer server = null)
        {
            return Config.Value.GetOrAdd(serverId, s => new ServerConfig(server));
        }

        public static void Stop(this string serverId)
        {
            ServerConfig value;
            Config.Value.TryRemove(serverId, out value);
        }
    }
}
