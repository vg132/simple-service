using System;
using System.Web;
using System.Web.Routing;

namespace SimpleService.Routing {
    public class SimpleServiceRouteHandler<TService> : IRouteHandler where TService : SimpleWebService, new() {
        public IHttpHandler GetHttpHandler(RequestContext requestContext) {
            return new TService();
        }
    }

    public class SimpleServiceRouteHandler : IRouteHandler {
        public Type SimpleWebServiceType { get; set; }

        public SimpleServiceRouteHandler(Type simpleWebServiceType) {
            SimpleWebServiceType = simpleWebServiceType;
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext) {
            return Activator.CreateInstance(SimpleWebServiceType) as IHttpHandler;
        }
    }
}