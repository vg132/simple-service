using System.Linq;
using System.Text;
using System.Reflection;
using SimpleService.Helpers;
using SimpleService.Proxy.Models;
using SimpleService.Template;

namespace SimpleService.Proxy {
    public class HelpPageGenerator {
        private const string HELP_PAGE_RESOURCE_KEY = "SimpleService.Proxy.Templates.help.html";
        public SimpleWebService Service { get; set; }

        public HelpPageGenerator(SimpleWebService service) {
            Service = service;
        }

        public string Generate() {
            string helpPage = ResourceHelper.GetEmbeddedResource(HELP_PAGE_RESOURCE_KEY);
            var serviceModel = GetServiceModelFromService(Service);

            TemplateParser template = new TemplateParser(helpPage, serviceModel, SyntaxOptions.HtmlSyntax);
            template.Parse();

            return template.ToString();
        }

        public HelpPageModel GetServiceModelFromService(SimpleWebService service) {
            var serviceType = service.GetType();
            var methods = serviceType.GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
            string absolutePath = Service.Request.Url.AbsolutePath;

            var model = new HelpPageModel();
            model.ServiceFullUrl = Service.Request.Url.ToString();
            model.ServiceName = serviceType.Name;
            model.ServiceUrl = absolutePath.EndsWith("/") ? absolutePath : absolutePath + "/";
            model.Methods = methods.Select(ConvertToServiceMethod).ToList();
            return model;
        }

        private ServiceMethodExtended ConvertToServiceMethod(MethodInfo methodInfo) {
            var parameters =
                methodInfo.GetParameters().Select(p => new Parameter(p.Name, p.ParameterType.Name)).ToList();
            var serviceMethod = new ServiceMethodExtended(methodInfo.Name, parameters, GetParameterString(methodInfo));

            return serviceMethod;
        }

        private static string GetParameterString(MethodInfo methodInfo) {
            StringBuilder sb = new StringBuilder();

            foreach (ParameterInfo parameterInfo in methodInfo.GetParameters()) {
                sb.Append(parameterInfo.Name.CamelCase() + ", ");
            }

            var output = sb.ToString();

            //if (output.EndsWith(", "))
            //    output = output.Substring(0, output.Length - 2);

            return output;
        }
    }
}