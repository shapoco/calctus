using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;

namespace Shapoco.Calctus.Model.Formats {
    class CharFormat : ValFormat {
        public static char Unescape(string tok) {
            switch (tok) {
                case "\\a": return '\a';
                case "\\b": return '\b';
                case "\\f": return '\f';
                case "\\n": return '\n';
                case "\\r": return '\r';
                case "\\t": return '\t';
                case "\\v": return '\v';
                case "\\\\": return '\\';
                case "\\'": return '\'';
                case "\\\"": return '"';
                case "\\0": return '\0';
                default:
                    if (tok.StartsWith("\\o")) {
                        var code = Convert.ToUInt64(tok.Substring(2), 8);
                        if (code < char.MinValue || char.MaxValue < code) {
                            throw new CalctusError("Char code out of range.");
                        }
                        return (char)code;
                    }
                    else if (tok.StartsWith("\\x") || tok.StartsWith("\\u")) {
                        var code = Convert.ToUInt64(tok.Substring(2), 16);
                        if (code < char.MinValue || char.MaxValue < code) {
                            throw new CalctusError("Char code out of range.");
                        }
                        return (char)code;
                    }
                    else {
                        return tok[0];
                    }
            }
        }

        public static void Escape(StringBuilder sb, char c, bool stringMode) {
            switch (c) {
                case '\a': sb.Append("\\a"); break;
                case '\b': sb.Append("\\b"); break;
                case '\f': sb.Append("\\f"); break;
                case '\n': sb.Append("\\n"); break;
                case '\r': sb.Append("\\r"); break;
                case '\t': sb.Append("\\t"); break;
                case '\v': sb.Append("\\v"); break;
                case '\\': sb.Append("\\\\"); break;
                case '\'':
                    if (stringMode) {
                        sb.Append(c); break;
                    }
                    else {
                        sb.Append("\\'"); break;
                    }
                case '"':
                    if (stringMode) {
                        sb.Append("\\\""); break;
                    }
                    else {
                        sb.Append(c); break;
                    }
                case '\0': sb.Append("\\0"); break;
                default:
                    if (char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsSeparator(c) || char.IsSymbol(c)) {
                        sb.Append(c);
                    }
                    else {
                        var hex = "0000" + Convert.ToString(c, 16);
                        sb.Append("\\u").Append(hex.Substring(hex.Length - 4, 4));
                    }
                    break;
            }
        }

        private static readonly Regex pattern
            = new Regex("'(?<char>[^'\\\\]|\\\\[abfnrtv\\\\\'0]|\\\\o[0-7]{3}|\\\\x[0-9a-fA-F]{2}|\\\\u[0-9a-fA-F]{4})'");

        private static CharFormat _instance;
        public static CharFormat Instance => (_instance != null) ? _instance : (_instance = new CharFormat());

        private CharFormat() : base(TokenType.SpecialLiteral, pattern, FormatPriority.AlwaysLeft) { }

        protected override Val OnParse(Match m) {
            return new RealVal(Unescape(m.Groups["char"].Value), new FormatHint(this));
        }

        protected override string OnFormat(Val val, FormatSettings fs) {
            if (!val.IsInteger) {
                // 整数でない場合はデフォルトの数値表現を使用
                return base.OnFormat(val, fs);
            }

            var ival = val.AsDecimal;
            if (ival < char.MinValue || char.MaxValue < ival) {
                // 小数やcharの範囲外の値はデフォルトの数値表現を使用
                return base.OnFormat(val, fs);
            }

            // エスケープしてクォーテーションで囲う
            var sb = new StringBuilder();
            sb.Append("'");
            Escape(sb, (char)ival, false);
            sb.Append("'");
            return sb.ToString();
        }

    }
}
