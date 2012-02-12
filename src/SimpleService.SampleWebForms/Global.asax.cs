using System;
using System.Web;
using System.Web.Routing;

namespace SimpleService.SampleWebForms {
    public class Global : HttpApplication {
        void Application_Start(object sender, EventArgs e) {
            RegisterRoutes(RouteTable.Routes);
        }

        private static void RegisterRoutes(RouteCollection routes) {
            //routes.MapSimpleServices();
            //routes.MapSimpleService<HelloWorldService>("services/helloWorld");
        }

    }
}

