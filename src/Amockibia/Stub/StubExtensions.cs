using Amockibia.Rule.Builder;
using Amockibia.Utilities;

namespace Amockibia.Stub
{
    public static class StubExtensions
    {
        public static void Stub(this AmockibiaServer server, IRuleBuildable builder)
        {
            server.ServerId.GetConfig().Rules.Add(builder.Build(server.ServerId));
        }
    }
}
