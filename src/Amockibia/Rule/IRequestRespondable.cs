using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Amockibia.Rule
{
    public interface IRequestRespondable
    {
        /// <summary>
        /// Set response to response parameter object.
        /// Should not change request parameter object.
        /// </summary>
        Task Respond(HttpRequest request, HttpResponse response);
    }
}
