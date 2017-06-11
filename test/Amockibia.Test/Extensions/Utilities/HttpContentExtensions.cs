using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Amockibia.Test.Extensions.Utilities
{
    public static class HttpContentExtensions
    {
        public static async Task<T> ReadAsAsync<T>(this HttpContent content)
        {
            return JsonConvert.DeserializeObject<T>(await content.ReadAsStringAsync());
        }
        
        public static async Task<T> ReadAsAnonymousTypeAsync<T>(this HttpContent content, T schema)
        {
            return JsonConvert.DeserializeObject<T>(await content.ReadAsStringAsync());
        }
    }
}