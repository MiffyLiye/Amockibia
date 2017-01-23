using Amockibia.Rule;
using Amockibia.Utilities;
using System.Linq;

namespace Amockibia.Verify
{
    public static class VerifyExtensions
    {
        public static RequestHandler Handler(this AmockibiaServer server, string handlerId)
        {
            return server.ServerId.GetConfig().Rules.Single(r => r.Id == handlerId);
        }
    }
}
