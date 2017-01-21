using System;
using Amockibia.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Amockibia.Rule.Matcher
{
    internal class DefaultMatcher : RequestMatcher
    {
        public override bool Matches(HttpContext context)
        {
            return true;
        }
    }
}
