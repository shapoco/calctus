using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model {
    class ScriptFilter {
        public const string ExeFilter = "*.exe";
        public const string DefaultParameter = "\"%s\" %p";

        public string Filter { get; set; }
        public string Command { get; set; }
        public string Parameter { get; set; }

        public ScriptFilter(string filter, string exe, string param) {
            Filter = filter;
            Command = exe;
            Parameter = param;
        }

        public bool IsExecutedDirectly => Filter == ExeFilter;

        public string CommandLabel {
            get {
                if (IsExecutedDirectly) {
                    return "(executed directly)";
                }
                else if (string.IsNullOrEmpty(Command)) {
                    return "(associated application)";
                }
                else {
                    return Command;
                }
            }
        }

        public string ParameterLabel {
            get {
                if (string.IsNullOrEmpty(Parameter)) {
                    return DefaultParameter;
                }
                else {
                    return Parameter;
                }
            }
        }

        // https://www.hiimray.co.uk/2020/04/18/implementing-simple-wildcard-string-matching-using-regular-expressions/474
        public static bool IsWildcardMatch(string wildcardPattern, string subject) {
            if (string.IsNullOrWhiteSpace(wildcardPattern)) {
                return false;
            }

            string regexPattern = string.Concat("^", Regex.Escape(wildcardPattern).Replace("\\*", ".*"), "$");

            int wildcardCount = wildcardPattern.Count(x => x.Equals('*'));
            if (wildcardCount <= 0) {
                return subject.Equals(wildcardPattern, StringComparison.CurrentCultureIgnoreCase);
            }
            else if (wildcardCount == 1) {
                string newWildcardPattern = wildcardPattern.Replace("*", "");

                if (wildcardPattern.StartsWith("*")) {
                    return subject.EndsWith(newWildcardPattern, StringComparison.CurrentCultureIgnoreCase);
                }
                else if (wildcardPattern.EndsWith("*")) {
                    return subject.StartsWith(newWildcardPattern, StringComparison.CurrentCultureIgnoreCase);
                }
                else {
                    try {
                        return Regex.IsMatch(subject, regexPattern);
                    }
                    catch {
                        return false;
                    }
                }
            }
            else {
                try {
                    return Regex.IsMatch(subject, regexPattern);
                }
                catch {
                    return false;
                }
            }
        }
    }
}
