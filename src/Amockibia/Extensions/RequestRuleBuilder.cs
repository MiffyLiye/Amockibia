using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace Amockibia.Extensions
{
    public class RequestRuleBuilder : RuleBuilder
    {
        public RequestRuleBuilder(Func<HttpRequest, bool> predicate)
        {
            Predicates.Add(predicate);
        }

        public RequestRuleBuilder(HttpMethod httpMethod, string uri)
        {
            HttpMethod = httpMethod;
            Uri = uri;
        }

        public RequestRuleBuilder WithRequest(Func<HttpRequest, bool> predicate)
        {
            Predicates.Add(predicate);
            return this;
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