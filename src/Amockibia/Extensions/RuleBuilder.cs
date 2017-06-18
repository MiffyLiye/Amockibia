using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Amockibia.Extensions.Matcher;
using Amockibia.Extensions.Responder;
using Amockibia.Rule;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Amockibia.Extensions
{
    public abstract class RuleBuilder
    {
        protected List<Func<HttpRequest, bool>> Predicates { get; set; }
        protected HttpMethod HttpMethod { get; set; }
        protected string Uri { get; set; }

        protected HttpStatusCode HttpStatusCode { get; set; }
        protected object Payload { get; set; }
        protected List<KeyValuePair<string, string>> ExtraHeaders { get; set; }
        protected Stream Body { get; set; }

        protected int Priority { get; set; }
        protected int MatchTimeUntilExpire { get; set; }

        protected string RuleId { get; set; }

        protected RuleBuilder(RuleBuilder builder)
        {
            Predicates = builder.Predicates;
            HttpMethod = builder.HttpMethod;
            Uri = builder.Uri;

            HttpStatusCode = builder.HttpStatusCode;
            Payload = builder.Payload;
            ExtraHeaders = builder.ExtraHeaders;
            Body = builder.Body;

            Priority = builder.Priority;
            MatchTimeUntilExpire = builder.MatchTimeUntilExpire;
            RuleId = builder.RuleId;
        }

        protected RuleBuilder()
        {
            Predicates = new List<Func<HttpRequest, bool>>();
            HttpStatusCode = HttpStatusCode.OK;
            ExtraHeaders = new List<KeyValuePair<string, string>>();
            Priority = 0;
            MatchTimeUntilExpire = -1;
        }

        protected RequestHandler BuildRule(string serverId)
        {
            var matcher = new RequestMatcher(serverId, Predicates, HttpMethod, Uri);
            var responder = new RequestResponder(async response =>
            {
                response.StatusCode = (int)HttpStatusCode;
                foreach (var extraHeader in ExtraHeaders)
                {
                    response.Headers.Append(extraHeader.Key, extraHeader.Value);
                    //response.Headers[extraHeader.Key] = extraHeader.Value;
                }
                if (!response.Headers.ContainsKey(HeaderNames.ContentType))
                {
                    response.Headers[HeaderNames.ContentType] = "application/json; charset=utf-8";
                }

                if (Body != null)
                {
                    Body.Position = 0;
                    await Body.CopyToAsync(response.Body);
                }
                else
                {
                    await response.WriteAsync(Payload != null ? JsonConvert.SerializeObject(Payload) : "", Encoding.UTF8);
                }
            });
            return new RequestHandler(matcher, responder, Priority, MatchTimeUntilExpire, RuleId);
        }
    }
}
