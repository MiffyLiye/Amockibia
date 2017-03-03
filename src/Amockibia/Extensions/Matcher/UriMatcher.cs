using System;
using System.Net.Http;
using Amockibia.Utilities;
using Microsoft.AspNetCore.Http;
using Amockibia.Rule;

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
            return request.Method == HttpMethod.ToString() && request.Path == ExpectedUri.AbsolutePath;
        }
    }
}
