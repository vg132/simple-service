using System;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web.UI;
using SimpleService.Helpers;

namespace SimpleService.Template {
    public class TemplateParser {
        public string Template { get; set; }
        public StringBuilder ParsedTemplate { get; set; }
        private readonly object _model;
        private readonly object _contextModel;
        private SyntaxOption _syntax;

        private bool _inVariable;
        private bool _inRepeaterBlock;
        private string _currentVariable;
        private string _repeaterProperty;
        private StringBuilder _repeaterTemplate = new StringBuilder();
        private int _openRepeaters;
        private bool _preventNextAddCharacter;

        public TemplateParser(string template, object model, SyntaxOption syntax, object contextModel = null,
                              StringBuilder parsedTemplate = null) {
            Template = template;
            ParsedTemplate = parsedTemplate ?? new StringBuilder();
            _model = model;
            _contextModel = contextModel;
            _syntax = syntax;
        }

        public void Parse() {
            if (Template == null)
                return;

            for (int charPosition = 0; charPosition < Template.Length; charPosition++) {
                char currentChar = Template[charPosition];

                if (currentChar == _syntax.VariableCharacterWrapper && !_inRepeaterBlock) {
                    ProcessVariableCharacter();
                }
                else if (currentChar == _syntax.CommentBeginCharacter) {
                    ProcessCommentBeginCharacter(ref charPosition);
                }

                AddCharacterToTemplate(currentChar);
            }
        }

        private void ProcessVariableCharacter() {
            _inVariable = !_inVariable;

            if (!_inVariable) {
                ParsedTemplate.Append(GetModelValue(_currentVariable));
                _currentVariable = null;
            }
            _preventNextAddCharacter = true;
        }

        private void ProcessCommentBeginCharacter(ref int charPosition) {
            if (Peek(charPosition, _syntax.CommentBeginString.Length) == _syntax.CommentBeginString) {
                string operatorString = PeekUntil(charPosition, _syntax.CommentEndString);

                if (operatorString.Contains("start-repeat")) {
                    if (_openRepeaters == 0) {
                        _repeaterProperty = GetRepeaterProperty(operatorString);
                        _inRepeaterBlock = true;

                        // jump forward to after operator line-end
                        charPosition = charPosition + operatorString.Length - 1;
                        _preventNextAddCharacter = true;
                    }

                    _openRepeaters++;
                }
                else if (operatorString.Contains("end-repeat")) {
                    if (_openRepeaters == 1) {
                        var model =
                            ReflectionHelper.GetPropertyValue(_contextModel ?? _model, _repeaterProperty) as IEnumerable;

                        if (model != null) {
                            foreach (var contextModel in model) {
                                var templateBlock = new TemplateParser(_repeaterTemplate.ToString(), _model, _syntax,
                                                                       contextModel, ParsedTemplate);
                                templateBlock.Parse();
                            }
                        }

                        _repeaterProperty = null;
                        _inRepeaterBlock = false;
                        _repeaterTemplate = new StringBuilder();

                        // jump forward to after operator line-end
                        charPosition = charPosition + operatorString.Length - 1;
                        _preventNextAddCharacter = true;
                    }

                    _openRepeaters--;
                }
            }
        }

        public override string ToString() {
            return ParsedTemplate.ToString();
        }

        private void AddCharacterToTemplate(char currentChar) {
            if (_preventNextAddCharacter) {
                _preventNextAddCharacter = false;
                return;
            }

            if (_inVariable && !IsAlphaNumeric(currentChar)) {
                ParsedTemplate.Append(_syntax.VariableCharacterWrapper + _currentVariable + currentChar);
                _inVariable = false;
                _currentVariable = null;
            }
            else if (_inVariable) {
                _currentVariable += currentChar;
            }
            else if (_inRepeaterBlock) {
                _repeaterTemplate.Append(currentChar);
            }
            else {
                ParsedTemplate.Append(currentChar);
            }
        }

        private static string GetRepeaterProperty(string operatorString) {
            Match match = Regex.Match(operatorString, "(start-repeat:(?<methodName>.+) )");
            return match.Groups["methodName"].Value;
        }

        private string Peek(int from, int length) {
            if (from + length > Template.Length)
                return null;
            return Template.Substring(from, length);
        }

        private string PeekUntil(int beginIndex, string targetString) {
            StringBuilder sb = new StringBuilder();
            int targetLength = targetString.Length;

            for (int i = beginIndex; i < Template.Length; i++) {
                string currentChars = Peek(i, targetLength);

                if (currentChars == targetString) {
                    sb.Append(currentChars);
                    return sb.ToString();
                }
                sb.Append(Template[i]);
            }
            return sb.ToString();
        }

        private string GetModelValue(string propertyName) {
            if (propertyName.Equals(":value"))
                return (_contextModel ?? _model).ToString();

            // first try get the property with the contextual model (in a repeater)
            if (_contextModel != null && ReflectionHelper.ObjectHasProperty(_contextModel, propertyName))
                return ReflectionHelper.GetPropertyValueAsString(_contextModel, propertyName);

            // then try with the template model
            if (ReflectionHelper.ObjectHasProperty(_model, propertyName))
                return ReflectionHelper.GetPropertyValueAsString(_model, propertyName);

            return string.Format(_syntax.VariableWrapperFormat, propertyName);
        }

        /// <summary>
        /// Returns true if the character is a letter, digit, underscore,
        /// dollar sign, or non-ASCII character.
        /// </summary>
        private static bool IsAlphaNumeric(int ch) {
            return ((ch >= 'a' && ch <= 'z') ||
                    (ch >= '0' && ch <= '9') ||
                    (ch >= 'A' && ch <= 'Z') ||
                    ch == '_' || ch == '$' || ch > 126);
        }
    }
}