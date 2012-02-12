using System.IO;
using System.Reflection;

namespace SimpleService.Helpers {
    internal static class ResourceHelper {
        public static string GetEmbeddedResource(string resourceName) {
            Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            StreamReader streamReader = new StreamReader(resourceStream);
            string resource = streamReader.ReadToEnd();
            return resource;
        }
    }
}