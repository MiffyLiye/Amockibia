using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Amockibia.Rule;
using Amockibia.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace Amockibia.Extensions.Matcher
{
    internal class RequestMatcher : IRequestMatchable
    {
        private List<Func<HttpRequest, bool>> Predicates { get; set; }
        private HttpMethod HttpMethod { get; }
        private Uri ExpectedUri { get; }
        private bool IsAbsolute { get; }

        public RequestMatcher(string serverId, List<Func<HttpRequest, bool>> predicates, HttpMethod httpMethod, string uri)
        {
            Predicates = predicates;
            HttpMethod = httpMethod;
            var baseUri = serverId.GetConfig().Server.BaseAddress;
            IsAbsolute = Uri.IsWellFormedUriString(uri, UriKind.Absolute);
            ExpectedUri = uri != null ? (IsAbsolute
                ? new Uri(uri)
                : new Uri(baseUri, uri)) : null;
        }

        public bool Matches(HttpRequest request)
        {
            if (Predicates.Any(p => !p(request))) return false;
            if (HttpMethod != null && request.Method != HttpMethod.ToString()) return false;
            if (ExpectedUri != null)
            {
                if (IsAbsolute)
                {
                    if (request.Host.Host != ExpectedUri.Host) return false;
                    if (request.Scheme != ExpectedUri.Scheme) return false;
                    if ((request.Host.Port ?? (request.Scheme == "https" ? 443 : 80)) != ExpectedUri.Port) return false;
                }
                if (request.Path != ExpectedUri.AbsolutePath) return false;

                var queries = QueryHelpers.ParseQuery(ExpectedUri.Query);

                foreach (var query in queries)
                {
                    StringValues value;
                    if (!request.Query.TryGetValue(query.Key, out value)) return false;
                    if (!query.Value.Equals(value)) return false;
                }
            }

            return true;
        }
    }
}
