using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Amockibia.Rule
{
    public interface IRequestRespondable
    {
        Task Respond(HttpResponse response);
    }
}
