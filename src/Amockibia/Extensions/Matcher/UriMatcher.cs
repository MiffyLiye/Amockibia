using System;
using Amockibia.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Amockibia.Rule;

namespace Amockibia.Extensions.Matcher
{
    internal class UriMatcher : IRequestMatchable
    {
        private Uri ExpectedUri { get; }

        public UriMatcher(string serverId, string relativeUri)
        {
            var baseUri = serverId.GetConfig().Server.BaseAddress;
            ExpectedUri = new Uri(baseUri, relativeUri);
        }

        public bool Matches(HttpRequest request)
        {
            var actual = request.GetDisplayUrl();
            return actual == ExpectedUri.ToString();
        }
    }
}
