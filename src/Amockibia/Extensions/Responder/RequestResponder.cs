﻿using System;
using System.Threading.Tasks;
using Amockibia.Rule;
using Microsoft.AspNetCore.Http;

namespace Amockibia.Extensions.Responder
{
    internal class RequestResponder : IRequestRespondable
    {
        private Func<HttpResponse, Task> RespondAction { get; }
        
        public RequestResponder(Func<HttpResponse, Task> respond)
        {
            RespondAction = respond;
        }

        public async Task Respond(HttpRequest request, HttpResponse response)
        {
            await RespondAction(response);
        }
    }

}
