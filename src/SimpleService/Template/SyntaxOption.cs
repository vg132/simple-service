namespace SimpleService.Template {
    public static class SyntaxOptions {
        public static SyntaxOption JavaScriptSyntax = new SyntaxOption('$', "/*", "*/");
        public static SyntaxOption HtmlSyntax = new SyntaxOption('$', "<!--", "-->");
    }

    public struct SyntaxOption {
        /// <summary>
        /// Format string for variable.
        /// </summary>
        public string VariableWrapperFormat;

        /// <summary>
        /// Character to use as prefix and suffix for variables.
        /// </summary>
        public char VariableCharacterWrapper;

        /// <summary>
        /// First character of a code comment.
        /// </summary>
        public char CommentBeginCharacter;

        /// <summary>
        /// Open tag for a comment. E.g. "!--" for HTML.
        /// </summary>
        public string CommentBeginString;

        /// <summary>
        /// End tag for a comment. E.g. "-->" for HTML.
        /// </summary>
        public string CommentEndString;

        public SyntaxOption(char variableCharacterWrapper, string commentBeginString, string commentEndString) {
            VariableWrapperFormat = variableCharacterWrapper + "{0}" + variableCharacterWrapper;
            VariableCharacterWrapper = variableCharacterWrapper;
            CommentBeginCharacter = commentBeginString[0];
            CommentBeginString = commentBeginString;
            CommentEndString = commentEndString;
        }
    }
}