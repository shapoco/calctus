using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Texts {
    static class CStyleEscaping {
        public static string EscapeAndQuote(string s) => "\"" + Escape(s) + "\"";
        public static string EscapeAndQuote(char c) => "'" + Escape(c) + "'";

        public static string Escape(string s) {
            var sb = new StringBuilder();
            foreach (var c in s) sb.Append(Escape(c, true));
            return sb.ToString();
        }

        public static string Escape(char c) => Escape(c, false);

        public static string Escape(char c, bool stringMode) {
            switch (c) {
                case '\a': return "\\a";
                case '\b': return "\\b";
                case '\f': return "\\f";
                case '\n': return "\\n";
                case '\r': return "\\r";
                case '\t': return "\\t";
                case '\v': return "\\v";
                case '\\': return "\\\\";
                case '\'':
                    if (stringMode) {
                        return c.ToString();
                    }
                    else {
                        return "\\'";
                    }
                case '"':
                    if (stringMode) {
                        return "\\\"";
                    }
                    else {
                        return c.ToString();
                    }
                case '\0': return "\\0";
                default:
                    if (char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsSeparator(c) || char.IsSymbol(c)) {
                        return c.ToString();
                    }
                    else {
                        var hex = "0000" + Convert.ToString(c, 16);
                        return "\\u" + hex.Substring(hex.Length - 4, 4);
                    }
            }
        }

        public static string Unescape(string s) => UnquoteAndUnescape("\"" + s + "\"");

        public static string UnquoteAndUnescape(string s) {
            var lex = new SimpleLexer(s);
            var ret = lex.PopCStyleString();
            if (!lex.Eos) throw lex.Input.CreateException("Invalid string format.");
            return ret;
        }
    }
}
