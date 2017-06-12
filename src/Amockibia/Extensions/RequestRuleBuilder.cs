using System.Net;
using System.Net.Http;

namespace Amockibia.Extensions
{
    public class RequestRuleBuilder : RuleBuilder
    {
        public RequestRuleBuilder(HttpMethod httpMethod, string uri)
        {
            HttpMethod = httpMethod;
            Uri = uri;
        }

        public ResponseRuleBuilder Send(HttpStatusCode httpStatusCode)
        {
            HttpStatusCode = httpStatusCode;
            return new ResponseRuleBuilder(this);
        }
        
        // ReSharper disable once InconsistentNaming
        public ResponseRuleBuilder SendOK()
        {
            return Send(HttpStatusCode.OK);
        }
    }
}