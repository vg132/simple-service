namespace SimpleService.Proxy.Models {
    public class ServiceMethod {
        public ServiceMethod(string methodName, string serviceMethodName, string parameterString, string parameterObject) {
            MethodName = methodName;
            ServiceMethodName = serviceMethodName;
            ParameterString = parameterString;
            ParameterObject = parameterObject;
        }

        public string MethodName { get; set; }
        public string ParameterString { get; set; }
        public string ServiceMethodName { get; set; }
        public string ParameterObject { get; set; }
    }
}