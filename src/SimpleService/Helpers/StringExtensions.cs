namespace SimpleService.Helpers {
    public static class StringExtensions {
        public static string CamelCase(this string source) {
            if (source == null) return null;
            if (source.Length == 1) return source.ToLower();
            return source.Substring(0, 1).ToLower() + source.Substring(1);
        }
    }
}