using System;
using Amockibia.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Amockibia.Rule;

namespace Amockibia.Extensions.Matcher
{
    internal class UriMatcher : IRequestMatchable
    {
        private Uri BaseUri { get; }
        private string RelativeUri { get; }
        private Uri ExpectedUri { get; }

        public UriMatcher(string serverId, string relativeUri)
        {
            BaseUri = serverId.GetConfig().Server.BaseAddress;
            RelativeUri = relativeUri;
            ExpectedUri = new Uri(BaseUri, relativeUri);
        }

        public bool Matches(HttpRequest request)
        {
            var actual = request.GetDisplayUrl();
            return actual == ExpectedUri.ToString();
        }
    }
}
