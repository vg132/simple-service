using System.Collections.Generic;

namespace SimpleWebService.Proxy.Models {
    public class ServiceModel {
        public ServiceModel(string serviceName, string serviceUrl, List<ServiceMethod> methods) {
            ServiceName = serviceName;
            ServiceUrl = serviceUrl;
            Methods = methods;
        }

        public string ServiceName { get; set; }
        public string ServiceUrl { get; set; }
        public List<ServiceMethod> Methods { get; set; }
    }
}