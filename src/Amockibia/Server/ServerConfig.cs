using System.Collections.Generic;
using Amockibia.Rule;

namespace Amockibia
{
    internal class ServerConfig
    {
        public AmockibiaServer Server { get; }
        public List<RequestHandler> Rules { get; }

        public ServerConfig(AmockibiaServer server)
        {
            Server = server;
            Rules = new List<RequestHandler>();
        }
    }
}
