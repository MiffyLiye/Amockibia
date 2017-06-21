using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Amockibia.Test.Utilities
{
    public static class StreamExtensions
    {
        public static async Task<string> ReadAsStringAsync(this Stream stream)
        {
            var streamReader = new StreamReader(stream);
            streamReader.BaseStream.Position = 0;
            return await streamReader.ReadToEndAsync().ConfigureAwait(false);
        }
        
        public static async Task<T> ReadAsAsync<T>(this Stream stream)
        {
            var streamReader = new StreamReader(stream);
            streamReader.BaseStream.Position = 0;
            var value = await streamReader.ReadToEndAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(value);
        }
        
        public static async Task<T> ReadAsAnonymousTypeAsync<T>(this Stream stream, T schema)
        {
            var streamReader = new StreamReader(stream);
            streamReader.BaseStream.Position = 0;
            var value = await streamReader.ReadToEndAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}