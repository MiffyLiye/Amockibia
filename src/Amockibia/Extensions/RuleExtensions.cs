using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Amockibia.Extensions
{
    public class When
    {
        public static RuleBuilder Receive(HttpMethod httpMethod, string relativeUri)
        {
            return new RuleBuilder { HttpMethod = httpMethod, RelativeUri = relativeUri };
        }

        public static RuleBuilder Get(string relativeUri)
        {
            return Receive(HttpMethod.Get, relativeUri);
        }
        
        public static RuleBuilder Post(string relativeUri)
        {
            return Receive(HttpMethod.Post, relativeUri);
        }
        
        public static RuleBuilder Put(string relativeUri)
        {
            return Receive(HttpMethod.Put, relativeUri);
        }
        
        public static RuleBuilder Delete(string relativeUri)
        {
            return Receive(HttpMethod.Delete, relativeUri);
        }
    }

    public static class RuleExtensions
    {
        // ReSharper disable once InconsistentNaming
        public static RuleBuilder SendOK(this RuleBuilder builder)
        {
            return builder.Send(HttpStatusCode.OK);
        }

        public static RuleBuilder Send(this RuleBuilder builder, HttpStatusCode httpStatusCode)
        {
            builder.HttpStatusCode = httpStatusCode;
            return builder;
        }
        
        public static RuleBuilder WithPayloadObject(this RuleBuilder builder, object payload)
        {
            builder.Payload = payload;
            return builder;
        }
        
        public static RuleBuilder WithHeader(this RuleBuilder builder, string key, string value)
        {
            builder.ExtraHeaders.Add(new KeyValuePair<string, string>(key, value));
            return builder;
        }

        public static RuleBuilder WithPriority(this RuleBuilder builder, int priority)
        {
            builder.Priority = priority;
            return builder;
        }
        
        public static RuleBuilder WithMatchTimesUntilExpire(this RuleBuilder builder, int matchTimesUntilExpire)
        {
            builder.MatchTimeUntilExpire = matchTimesUntilExpire;
            return builder;
        }
    }
}
