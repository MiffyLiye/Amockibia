using System.Collections.Generic;
using System.ComponentModel;
using Amockibia.Rule;
using Amockibia.Rule.Builder;

namespace Amockibia.Extensions
{
    public class ResponseRuleBuilder : RuleBuilder, IRuleBuildable
    {
        public ResponseRuleBuilder(RequestRuleBuilder builder) : base(builder)
        {
        }
        
        public ResponseRuleBuilder WithPayloadObject(object payload)
        {
            Payload = payload;
            return this;
        }
        
        public ResponseRuleBuilder WithHeader(string key, string value)
        {
            ExtraHeaders.Add(new KeyValuePair<string, string>(key, value));
            return this;
        }

        public AdvancedRuleBuilder WithPriority(int priority)
        {
            Priority = priority;
            return new AdvancedRuleBuilder(this);
        }
        
        public AdvancedRuleBuilder WithMatchTimesUntilExpire(int matchTimesUntilExpire)
        {
            MatchTimeUntilExpire = matchTimesUntilExpire;
            return new AdvancedRuleBuilder(this);
        }
        
        public AdvancedRuleBuilder WithId(string id)
        {
            RuleId = id;
            return new AdvancedRuleBuilder(this);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public RequestHandler Build(string serverId)
        {
            return BuildRule(serverId);
        }
    }
}