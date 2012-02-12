using System.Reflection;
using System.Web.Routing;
using SimpleService.Routing;

[assembly: WebActivator.PreApplicationStartMethod(typeof(SimpleService.SampleWebForms.App_Start.SimpleService), "Start")]

namespace SimpleService.SampleWebForms.App_Start {
    public static class SimpleService {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.MapSimpleServices(assembly: Assembly.GetExecutingAssembly());
        }

        public static void Start() {
            RegisterRoutes(RouteTable.Routes);
        }
    }
}