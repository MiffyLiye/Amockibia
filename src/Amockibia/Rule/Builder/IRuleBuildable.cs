using System.ComponentModel;

namespace Amockibia.Rule.Builder
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IRuleBuildable
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        RequestHandler Build(string serverId);
    }
}
