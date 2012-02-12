using System.IO.Compression;
using System.Web;

namespace SimpleService {
    public class JavaScriptResult : ServiceResult {
        public string Script { get; set; }
        public bool UseCompression { get; set; }

        public override void Execute(ServiceContext context) {
            HttpResponse response = context.HttpContext.Response;
            response.ContentType = "application/x-javascript";

            if (Script != null) {
                if (UseCompression)
                    EnableCompression(context.HttpContext);

                response.Write(Script);
            }
        }

        private void EnableCompression(HttpContext context) {
            string acceptEncoding = context.Request.Headers["Accept-Encoding"];

            if (string.IsNullOrEmpty(acceptEncoding)) return;

            acceptEncoding = acceptEncoding.ToUpperInvariant();

            if (acceptEncoding.Contains("GZIP")) {
                context.Response.AppendHeader("Content-encoding", "gzip");
                context.Response.Filter = new GZipStream(context.Response.Filter, CompressionMode.Compress);
            }
            else if (acceptEncoding.Contains("DEFLATE")) {
                context.Response.AppendHeader("Content-encoding", "deflate");
                context.Response.Filter = new DeflateStream(context.Response.Filter, CompressionMode.Compress);
            }
        }
    }
}