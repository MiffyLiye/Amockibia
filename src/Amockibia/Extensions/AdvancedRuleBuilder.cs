using System.ComponentModel;
using Amockibia.Rule;
using Amockibia.Rule.Builder;

namespace Amockibia.Extensions
{
    public class AdvancedRuleBuilder : RuleBuilder, IRuleBuildable
    {
        public AdvancedRuleBuilder(RuleBuilder builder) : base(builder)
        {
        }
        
        public AdvancedRuleBuilder WithPriority(int priority)
        {
            Priority = priority;
            return this;
        }
        
        public AdvancedRuleBuilder WithMatchTimesUntilExpire(int matchTimesUntilExpire)
        {
            MatchTimeUntilExpire = matchTimesUntilExpire;
            return this;
        }
        
        public AdvancedRuleBuilder WithId(string id)
        {
            RuleId = id;
            return this;
        }
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        public RequestHandler Build(string serverId)
        {
            return base.BuildRule(serverId);
        }
    }
}