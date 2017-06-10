using Amockibia.Rule.Builder;
using Amockibia.Utilities;

namespace Amockibia.Setup
{
    public static class SetupExtensions
    {
        public static void Setup(this AmockibiaServer server, IRuleBuildable builder)
        {
            server.ServerId.GetConfig().Rules.Add(builder.Build(server.ServerId));
        }
    }
}
