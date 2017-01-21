using Amockibia.Rule.Matcher;
using Amockibia.Rule.Responder;

namespace Amockibia.Rule
{
    internal class DefaultRule : HandlingRule
    {
        public DefaultRule() : base (new DefaultMatcher(), new DefaultResponder(), 9999, -1)
        {
        }
    }
}
