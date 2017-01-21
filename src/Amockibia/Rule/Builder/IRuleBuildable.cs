namespace Amockibia.Rule.Builder
{
    public interface IRuleBuildable
    {
        RequestHandler Build(string serverId);
    }
}
