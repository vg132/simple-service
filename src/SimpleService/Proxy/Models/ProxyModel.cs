using System.Collections.Generic;

namespace SimpleService.Proxy.Models {
    public class ProxyModel {
        public ProxyModel(string serviceName, string serviceUrl, List<ServiceMethod> methods) {
            ServiceName = serviceName;
            ServiceUrl = serviceUrl;
            Methods = methods;
        }

        public string ServiceName { get; set; }
        public string ServiceUrl { get; set; }
        public List<ServiceMethod> Methods { get; set; }
    }
}