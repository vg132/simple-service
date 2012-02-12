using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.UI;
using SimpleService.Proxy;

namespace SimpleService {
    public abstract class SimpleWebService : IHttpHandler {
        public HttpContext Context { get; set; }
        public HttpRequest Request { get; set; }
        public HttpResponse Response { get; set; }
        public ServiceContext ServiceContext { get; set; }
        public Uri CallingUrl { get { return new Uri(Request.Params["_callingUrl"] ?? "/"); } }

        public void ProcessRequest(HttpContext context) {
            Context = context;
            Request = context.Request;
            Response = context.Response;

            var serviceContext = GetServiceContextFromRequest();

            if (serviceContext.Result != null)
                serviceContext.Result.Execute(serviceContext);
        }

        private ServiceContext GetServiceContextFromRequest() {
            var actionName = Request.RequestContext.RouteData.Values["method"] as string;
            var callingUrl = new Uri(Request.Params["_callingUrl"] ?? Context.Request.Url.ToString());
            var context = new ServiceContext(Context, actionName, callingUrl);

            if (!IsRequestAuthenticated()) {
                Response.StatusCode = 403;
                return context;
            }

            if (actionName == "proxy.js" || actionName == "proxy.min.js") {
                var javaScriptProxy = GetJavaScriptProxy(actionName.Contains(".min.js"));
                return context.WithResult(JavaScript(javaScriptProxy));
            }

            if (string.IsNullOrEmpty(actionName)) {
                var debugMode = Context.IsDebuggingEnabled;
                if (debugMode) {
                    HelpPageGenerator helpPageGenerator = new HelpPageGenerator(this);
                    return context.WithResult(Content(helpPageGenerator.Generate(), "text/html"));
                }
                return context.WithResult(Error("Missing method name."));
            }

            var methodInfo = GetType().GetMethod(actionName);
            
            if (methodInfo == null || !methodInfo.IsPublic) {
                return context.WithResult(Error("Service method could not be found. Remember that method names are case sensitive."));
            }

            object[] parameters = ModelBindParameters(methodInfo);

            var result = methodInfo.Invoke(this, parameters);
            return context.WithResult(result as ServiceResult);
        }

        protected virtual bool IsRequestAuthenticated() { return true; }

        protected virtual JavaScriptResult JavaScript(string script, bool useGzip = true) {
            return new JavaScriptResult {Script = script, UseCompression = useGzip};
        }

        protected virtual JsonResult Error(string message) {
            return new JsonResult {Data = new {error = message}};
        }

        protected virtual ContentResult Content(string text, string contentType = "text/plain") {
            return new ContentResult {Text = text, ContentType = contentType };
        }

        protected virtual ControlResult<Control> Control(string path, Action<Control> controlAction = null) {
            return new ControlResult<Control> {Path = path, ControlAction = controlAction};
        }

        /// <summary>
        /// Renders a control as view. The path to control is resolved by using the namespace. In case this isn't same as folder structure, use the path parameter.
        /// </summary>
        /// <typeparam name="TControl">The control type.</typeparam>
        /// <param name="path">Path to control. Start with ~/.</param>
        /// <param name="controlAction"></param>
        protected virtual ControlResult<TControl> Control<TControl>(string path = null, Action<TControl> controlAction = null) where TControl : Control {
            return new ControlResult<TControl> {Path = path, ControlAction = controlAction};
        }

        /// <summary>
        /// Used to repeat a control from a collection. 
        /// If the UserControl implements IModelTemplate the assignment of model is automatic.
        /// </summary>
        public virtual ControlListResult<TControl, TModel> ControlList<TControl, TModel>(string path, IEnumerable<TModel> model, Action<TControl, TModel> controlAction = null) where TControl : Control {
            return new ControlListResult<TControl, TModel> 
            {
                ControlAction = controlAction,
                Model = model,
                Path = path
            };
        }

        /// <summary>
        /// Used to repeat a control from a collection. 
        /// If the UserControl implements IModelTemplate the assignment of model is automatic.
        /// </summary>
        public virtual ControlListResult<TControl, TModel> ControlList<TControl, TModel>(IEnumerable<TModel> model, Action<TControl, TModel> controlAction = null) where TControl : Control {
            return new ControlListResult<TControl, TModel> 
            {
                ControlAction = controlAction,
                Model = model,
                Path = null
            };
        }

        protected virtual JsonResult Json(object data) {
            return new JsonResult {Data = data};
        }

        public bool IsReusable {
            get { return false; }
        }

        private string GetJavaScriptProxy(bool minify) {
            string cacheKey = "proxy-" + minify + "-" + ToString(); // ToString() returns the class name
            var cachedProxy = Context.Cache.Get(cacheKey);

            if (cachedProxy == null) {
                var javaScriptProxyBuilder = new JavaScriptProxyGenerator(this);
                var proxy = javaScriptProxyBuilder.Generate(minify);
                Context.Cache.Insert(cacheKey, proxy);

                return proxy;
            }

            return cachedProxy.ToString();
        }

        private object[] ModelBindParameters(MethodInfo methodInfo) {
            List<object> parameterValues = new List<object>();
            var parameters = methodInfo.GetParameters();
            var routeValues = Request.RequestContext.RouteData.Values;

            foreach (var parameterInfo in parameters) {
                object value = routeValues.ContainsKey(parameterInfo.Name)
                                   ? routeValues[parameterInfo.Name]
                                   : Request.Params[parameterInfo.Name];
                
                if (value == null)
                    value = GetDefault(parameterInfo.ParameterType);

                var valueTyped = Convert.ChangeType(value, parameterInfo.ParameterType);
                parameterValues.Add(valueTyped);
            }

            return parameterValues.ToArray();
        }

        public static object GetDefault(Type type) {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}