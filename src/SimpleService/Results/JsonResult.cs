using System.Web;
using System.Web.Script.Serialization;

namespace SimpleService {
    public class JsonResult : ServiceResult {
        public object Data { get; set; }

        public override void Execute(ServiceContext context) {
            var output = SerializeToJson(Data);

            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.Write(output);
            context.HttpContext.Response.End();
        }

        protected virtual string SerializeToJson(object data) {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(Data);
        }
    }
}