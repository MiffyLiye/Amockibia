using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Amockibia.Extensions.Matcher;
using Amockibia.Extensions.Responder;
using Amockibia.Rule;
using Amockibia.Rule.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Amockibia.Extensions
{
    public class RuleBuilder : IRuleBuildable
    {

        public HttpMethod HttpMethod { get; set; }
        public string RelativeUri { get; set; }
        public string RuleId { get; private set; }
        
        public HttpStatusCode HttpStatusCode { get; set; }
        public object Payload { get; set; }
        public List<KeyValuePair<string, string>> ExtraHeaders { get; set; }
        public int Priority { get; set; }
        public int MatchTimeUntilExpire { get; set; }

        public RuleBuilder()
        {
            HttpStatusCode = HttpStatusCode.OK;
            ExtraHeaders = new List<KeyValuePair<string, string>>();
            Priority = 0;
            MatchTimeUntilExpire = -1;
        }
        
        public RequestHandler Build(string serverId)
        {
            var matcher = new UriMatcher(serverId,  HttpMethod, RelativeUri);
            var responder = new RequestResponder(async response =>
            {
                response.StatusCode = (int) HttpStatusCode;
                response.ContentType = "application/json; charset=utf-8";
                foreach (var extraHeader in ExtraHeaders)
                {
                    response.Headers.Add(extraHeader.Key, extraHeader.Value);
                }
                await response.WriteAsync(Payload != null ? JsonConvert.SerializeObject(Payload) : "", Encoding.UTF8);
            });
            return new RequestHandler(matcher, responder, Priority, MatchTimeUntilExpire, RuleId);
        }

        public IRuleBuildable WithId(string ruleId)
        {
            RuleId = ruleId;
            return this;
        }
    }
}
