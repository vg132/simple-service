using System.Collections.Generic;
using SimpleService.Helpers;

namespace SimpleService.Proxy.Models {
    public class ServiceMethodExtended {
        public ServiceMethodExtended(string methodName, List<Parameter> parameters, string parameterString) {
            MethodName = methodName;
            MethodNameCamel = methodName.CamelCase();
            Parameters = parameters;
            ParameterString = parameterString;
        }

        public string MethodName { get; set; }
        public string MethodNameCamel { get; set; }
        public string ParameterString { get; set; }
        public List<Parameter> Parameters { get; set; }
    }
}