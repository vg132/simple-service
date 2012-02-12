using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Routing;

namespace SimpleService.Routing {
    public static class ServiceRouteCollectionExtensions {
        /// <summary>
        /// Maps all services in the specified assembly (defaults to all loaded assemblies) to routes.
        /// </summary>
        /// <param name="routes"></param>
        /// <param name="baseUrl">Url to prepend to routes for services.</param>
        /// <param name="assembly">Assembly to load services from. Defaults to all assemblies in CurrentDomain.</param>
        public static void MapSimpleServices(this RouteCollection routes, string baseUrl = "services",
                                             Assembly assembly = null) {
            var simpleWebServiceType = typeof (SimpleWebService);
            IEnumerable<Type> types;

            if (assembly != null)
                types = assembly.GetTypes();
            else
                types = AppDomain.CurrentDomain.GetAssemblies().ToList().SelectMany(s => s.GetTypes());

            types = types.Where(p => !p.IsAbstract && simpleWebServiceType.IsAssignableFrom(p))
                .ToList();

            foreach (var type in types) {
                var url = baseUrl + "/" + type.Name.Replace("Service", "").ToLower() + "/{method}";
                var routeValues = new RouteValueDictionary {{"method", ""}};

                Route route = new Route(url, routeValues, new SimpleServiceRouteHandler(type));

                routes.Add(route);
            }
        }

        /// <summary>
        /// Map a service to route.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="routes"></param>
        /// <param name="url"></param>
        /// <param name="defaults"></param>
        /// <returns></returns>
        public static Route MapSimpleService<TService>(this RouteCollection routes, string url, object defaults = null)
                where TService : SimpleWebService, new() {
            var routeValues = new RouteValueDictionary(defaults) {{"method", ""}};

            if (!url.Contains("{method}")) {
                if (url.EndsWith("/"))
                    url = url.Substring(0, url.Length - 1);
                url = url + "/{method}";
            }

            Route route = new Route(url,
                                    routeValues,
                                    new SimpleServiceRouteHandler<TService>());

            routes.Add(route);

            return route;
        }
    }
}