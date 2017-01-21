namespace Amockibia.Rule.Builder
{
    internal class DefaultRule : RequestHandler
    {
        public DefaultRule() : base (new AlwaysMatchMatcher(), new NotImplementedResponder(), 9999, -1)
        {
        }
    }
}
