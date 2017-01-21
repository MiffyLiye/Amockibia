using System;
using Amockibia.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Amockibia.Rule.Matcher
{
    internal class UriMatcher : RequestMatcher
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

        public override bool Matches(HttpContext context)
        {
            var actual = context.Request.GetDisplayUrl();
            return actual == ExpectedUri.ToString();
        }
    }
}
