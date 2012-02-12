using System.Web;

namespace SimpleService {
    public class ContentResult : ServiceResult {
        public string Text { get; set; }
        public string ContentType { get; set; }

        public override void Execute(ServiceContext context) {
            context.HttpContext.Response.ContentType = ContentType ?? "text/plain";
            context.HttpContext.Response.Write(Text);
        }
    }
}