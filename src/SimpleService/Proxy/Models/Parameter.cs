namespace SimpleService.Proxy.Models {
    public class Parameter {
        public Parameter(string name, string typeName) {
            Name = name;
            TypeName = typeName;
        }

        public string Name { get; set; }
        public string TypeName { get; set; }
    }
}