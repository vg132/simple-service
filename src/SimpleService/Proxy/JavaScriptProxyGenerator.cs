using System;
using System.Linq;
using System.Reflection;
using System.Text;
using SimpleService.Helpers;
using SimpleService.Proxy.Models;
using SimpleService.Template;

namespace SimpleService.Proxy {
    public class JavaScriptProxyGenerator {
        private const string PROXY_TEMPLATE_RESOURCE_KEY = "SimpleService.Proxy.Templates.proxy.js";
        public SimpleWebService Service { get; set; }

        public JavaScriptProxyGenerator(SimpleWebService service) {
            Service = service;
        }

        public string Generate(bool minify = false) {
            string proxyTemplate = ResourceHelper.GetEmbeddedResource(PROXY_TEMPLATE_RESOURCE_KEY);

            var serviceModel = GetServiceModelFromService(Service);
            TemplateParser template = new TemplateParser(proxyTemplate, serviceModel, SyntaxOptions.JavaScriptSyntax);

            template.Parse();

            var source = template.ToString();

            if (minify)
                source = JavaScriptCompressor.Compress(source);

            return source;
        }

        public ProxyModel GetServiceModelFromService(SimpleWebService service) {
            var serviceType = service.GetType();
            var methods = serviceType.GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
            var serviceMethods = methods.Select(ConvertToServiceMethod).ToList();
            var serviceModel = new ProxyModel(serviceType.Name, GetCleanServicePath(service.Request.Url),
                                              serviceMethods);

            return serviceModel;
        }

        private string GetCleanServicePath(Uri uri) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < uri.Segments.Length - 1; i++) {
                sb.Append(uri.Segments[i]);
            }
            return sb.ToString();
        }

        private ServiceMethod ConvertToServiceMethod(MethodInfo methodInfo) {
            var serviceMethod = new ServiceMethod(methodInfo.Name.CamelCase(), methodInfo.Name,
                                                  GetParameterString(methodInfo), GetParameterObject(methodInfo));

            return serviceMethod;
        }


        private static string GetParameterString(MethodInfo methodInfo) {
            StringBuilder sb = new StringBuilder();

            foreach (ParameterInfo parameterInfo in methodInfo.GetParameters()) {
                sb.Append(parameterInfo.Name.CamelCase() + ", ");
            }
            sb.Append("callback");

            return sb.ToString();
        }

        private static string GetParameterObject(MethodInfo methodInfo) {
            StringBuilder sb = new StringBuilder();

            sb.Append("{");

            foreach (ParameterInfo parameterInfo in methodInfo.GetParameters()) {
                string name = parameterInfo.Name.CamelCase();
                sb.Append(name + ": " + name);
                sb.Append(",");
            }

            if (sb.Length > 1)
                sb.Remove(sb.Length - 1, 1);

            sb.Append("}");


            return sb.ToString();
        }
    }
}