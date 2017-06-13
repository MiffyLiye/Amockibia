using System;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Amockibia.Rule;
using Amockibia.Utilities;

namespace Amockibia.Extensions.Matcher
{
    internal class UriMatcher : IRequestMatchable
    {
        private HttpMethod HttpMethod { get; }
        private Uri ExpectedUri { get; }
        private bool IsAbsolute { get; }

        public UriMatcher(string serverId, HttpMethod httpMethod, string uri)
        {
            HttpMethod = httpMethod;
            var baseUri = serverId.GetConfig().Server.BaseAddress;
            IsAbsolute = Uri.IsWellFormedUriString(uri, UriKind.Absolute);
            ExpectedUri = IsAbsolute
                ? new Uri(uri)
                : new Uri(baseUri, uri);
        }

        public bool Matches(HttpRequest request)
        {
            if (request.Method != HttpMethod.ToString()) return false;
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

            return true;
        }
    }
}
