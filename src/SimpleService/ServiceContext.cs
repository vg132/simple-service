using System;
using System.Reflection;
using System.Web;

namespace SimpleService {
    public class ServiceContext {
        public ServiceContext(HttpContext httpContext, string rawActionName, Uri callingUrl) {
            HttpContext = httpContext;
            RawActionName = rawActionName;
            CallingUrl = callingUrl;
        }

        public string RawActionName { get; set; }
        public MethodInfo ActionMethod { get; set; }
        public Uri CallingUrl { get; set; }
        public HttpContext HttpContext { get; set; }
        public ServiceResult Result { get; set; }

        internal ServiceContext WithResult(ServiceResult result) {
            Result = result;
            return this;
        }
    }
}