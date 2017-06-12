using System.Net.Http;

namespace Amockibia.Extensions
{
    public class When
    {
        public static RequestRuleBuilder Receive(HttpMethod httpMethod, string relativeUri)
        {
            return new RequestRuleBuilder(httpMethod, relativeUri);
        }

        public static RequestRuleBuilder Get(string relativeUri)
        {
            return Receive(HttpMethod.Get, relativeUri);
        }
        
        public static RequestRuleBuilder Post(string relativeUri)
        {
            return Receive(HttpMethod.Post, relativeUri);
        }
        
        public static RequestRuleBuilder Put(string relativeUri)
        {
            return Receive(HttpMethod.Put, relativeUri);
        }
        
        public static RequestRuleBuilder Delete(string relativeUri)
        {
            return Receive(HttpMethod.Delete, relativeUri);
        }
    }
}
