using System;
using System.Net.Http;
using Amockibia.Utilities;
using Microsoft.AspNetCore.Http;
using Amockibia.Rule;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace Amockibia.Extensions.Matcher
{
    internal class UriMatcher : IRequestMatchable
    {
        private HttpMethod HttpMethod { get; }
        private Uri ExpectedUri { get; }

        public UriMatcher(string serverId, HttpMethod httpMethod, string relativeUri)
        {
            HttpMethod = httpMethod;
            var baseUri = serverId.GetConfig().Server.BaseAddress;
            ExpectedUri = new Uri(baseUri, relativeUri);
        }

        public bool Matches(HttpRequest request)
        {
            if (request.Method != HttpMethod.ToString()) return false;
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
