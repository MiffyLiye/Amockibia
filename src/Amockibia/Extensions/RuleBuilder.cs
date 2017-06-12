using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Amockibia.Extensions.Matcher;
using Amockibia.Extensions.Responder;
using Amockibia.Rule;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Amockibia.Extensions
{
    public abstract class RuleBuilder
    {
        protected HttpMethod HttpMethod { get; set; }
        protected string RelativeUri { get; set; }

        protected HttpStatusCode HttpStatusCode { get; set; }
        protected object Payload { get; set; }
        protected List<KeyValuePair<string, string>> ExtraHeaders { get; set; }
        protected int Priority { get; set; }
        protected int MatchTimeUntilExpire { get; set; }

        protected string RuleId { get; set; }

        protected RuleBuilder(RuleBuilder builder)
        {
            HttpMethod = builder.HttpMethod;
            RelativeUri = builder.RelativeUri;

            HttpStatusCode = builder.HttpStatusCode;
            Payload = builder.Payload;
            ExtraHeaders = builder.ExtraHeaders;
            
            Priority = builder.Priority;
            MatchTimeUntilExpire = builder.MatchTimeUntilExpire;
            RuleId = builder.RuleId;
        }

        protected RuleBuilder()
        {
            HttpStatusCode = HttpStatusCode.OK;
            ExtraHeaders = new List<KeyValuePair<string, string>>();
            Priority = 0;
            MatchTimeUntilExpire = -1;
        }

        protected RequestHandler BuildRule(string serverId)
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
    }
}
