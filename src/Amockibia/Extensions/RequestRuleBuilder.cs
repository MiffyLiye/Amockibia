using System.Net;
using System.Net.Http;

namespace Amockibia.Extensions
{
    public class RequestRuleBuilder : RuleBuilder
    {
        public RequestRuleBuilder(HttpMethod httpMethod, string relativeUri)
        {
            HttpMethod = httpMethod;
            RelativeUri = relativeUri;
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