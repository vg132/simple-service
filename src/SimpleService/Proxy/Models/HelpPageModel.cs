using System.Collections.Generic;

namespace SimpleService.Proxy.Models {
    public class HelpPageModel {
        public string ServiceName { get; set; }
        public string ServiceUrl { get; set; }
        public string ServiceFullUrl { get; set; }

        public List<ServiceMethodExtended> Methods { get; set; }
    }
}