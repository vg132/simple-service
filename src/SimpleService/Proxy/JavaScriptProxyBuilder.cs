using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using SimpleWebService.Helpers;
using SimpleWebService.Proxy.Models;
using SimpleWebService.Template;

namespace SimpleWebService.Proxy {
    public class JavaScriptProxyBuilder {
        private const string PROXY_TEMPLATE_RESOURCE_KEY = "SimpleWebService.Proxy.Template.proxy-template.js";
        public SimpleService Service { get; set; }
        
        public JavaScriptProxyBuilder(SimpleService service)
        {
            Service = service;
        }

        public string Generate(bool minify = false) {
            string proxyTemplate = ResourceHelper.GetEmbeddedResource(PROXY_TEMPLATE_RESOURCE_KEY);
            
            var serviceModel = GetServiceModelFromService(Service);
            TemplateParser template = new TemplateParser(proxyTemplate, serviceModel, SyntaxOptions.JavaScriptSyntax);

            template.Parse();

            var source = template.ToString();
            
            if (minify)
                source = JavaScriptCompressor.Compress(template.ToString());

            return source;
        }

        public ServiceModel GetServiceModelFromService(SimpleService service) {
            var serviceType = service.GetType();
            var methods = serviceType.GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
            var serviceMethods = methods.Select(ConvertToServiceMethod).ToList();
            var serviceModel = new ServiceModel(CamelCase(serviceType.Name), GetCleanServicePath(service.Request.Url),
                                                serviceMethods);

            return serviceModel;
        }

        private string GetCleanServicePath(Uri uri) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < uri.Segments.Length-1; i++) {
                sb.Append(uri.Segments[i]);
            }
            return sb.ToString();
        }

        private ServiceMethod ConvertToServiceMethod(MethodInfo methodInfo) {
            var serviceMethod = new ServiceMethod(CamelCase(methodInfo.Name), methodInfo.Name, 
                GetParameterString(methodInfo), GetParameterObject(methodInfo));

            return serviceMethod;
        }


        private static string GetParameterString(MethodInfo methodInfo) {
            StringBuilder sb = new StringBuilder();

            foreach (ParameterInfo parameterInfo in methodInfo.GetParameters()) {
                sb.Append(CamelCase(parameterInfo.Name) + ", ");
            }
            sb.Append("callback");
            
            return sb.ToString();
        }

        private static string GetParameterObject(MethodInfo methodInfo) {
            StringBuilder sb = new StringBuilder();
            
            sb.Append("{");
            
            foreach (ParameterInfo parameterInfo in methodInfo.GetParameters()) {
                string name = CamelCase(parameterInfo.Name);
                sb.Append(name + ": " + name);
                sb.Append(",");
            }
            
            if (sb.Length > 1)
                sb.Remove(sb.Length - 1, 1);

            sb.Append("}");


            return sb.ToString();
        }
        private static string CamelCase(string name) {
            return name.Substring(0, 1).ToLower() + name.Substring(1);
        }
    }
}