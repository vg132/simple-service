namespace SimpleService.Helpers {
    internal static class ReflectionHelper {
        public static string GetPropertyValueAsString(object obj, string propertyName) {
            if (obj == null)
                return null;
            var type = obj.GetType();
            var property = type.GetProperty(propertyName);
            if (property != null)
                return property.GetValue(obj, null).ToString();
            return null;
        }

        public static object GetPropertyValue(object obj, string propertyName) {
            if (obj == null)
                return null;
            var type = obj.GetType();
            var property = type.GetProperty(propertyName);
            if (property != null)
                return property.GetValue(obj, null);
            return null;
        }

        public static bool ObjectHasProperty(object obj, string propertyName) {
            if (obj == null)
                return false;
            var type = obj.GetType();
            var property = type.GetProperty(propertyName);
            return property != null;
        }
    }
}