using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Amockibia.Test.Extensions.Utilities
{
    public class ObjectContent : HttpContent
    {
        private class JsonContentImpl : StringContent
        {
            public JsonContentImpl(string content, Encoding encoding, string mediaType) : base(content, encoding, mediaType)
            {
            }
            
            public Task SerializeToStreamAsyncPublic(Stream stream, TransportContext context)
            {
                return SerializeToStreamAsync(stream, context);
            }

            public bool TryComputeLengthPublic(out long length)
            {
                return TryComputeLength(out length);
            }
        }
        
        private JsonContentImpl Content { get; }
        
        public ObjectContent(object obj)
        {
            Content = new JsonContentImpl(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return Content.SerializeToStreamAsyncPublic(stream, context);
        }

        protected override bool TryComputeLength(out long length)
        {
            return Content.TryComputeLengthPublic(out length);
        }
    }
}