﻿using Amockibia.Rule;
using Amockibia.Rule.Builder;
using Amockibia.Stub;
using Amockibia.Test.Utilities;
using FluentAssertions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Amockibia.Test
{
    public class HybridModeTest : TestBase
    {
        private class PriorityRuleBuilder : IRuleBuildable
        {
            private HttpStatusCode StatusCode { get; }
            private int Priority { get; }
            public PriorityRuleBuilder(HttpStatusCode statusCode, int priority)
            {
                StatusCode = statusCode;
                Priority = priority;
            }
            public RequestHandler Build(string serverId)
            {
                return new RequestHandler(new AlwaysMatchMatcher(), new StatusCodeOnlyResponder(StatusCode), Priority, 1);
            }
        }

        [Fact]
        public async Task should_share_same_server_when_use_both_self_host_client_and_in_memory_client()
        {
            Server.Stub(new PriorityRuleBuilder(HttpStatusCode.OK, 1));
            Server.Stub(new PriorityRuleBuilder(HttpStatusCode.NoContent, 2));

            var inMemoryClient = SelectHttpClient(true);
            var selfHostClient = SelectHttpClient(false);

            (await inMemoryClient.GetAsync("")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await selfHostClient.GetAsync("")).StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
