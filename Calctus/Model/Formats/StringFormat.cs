using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;

namespace Shapoco.Calctus.Model.Formats {
    class StringFormat : ValFormat {
        private static readonly Regex pattern
            = new Regex("\"(?<char>[^\"\\\\]|\\\\[abfnrtv\"\\\\0]|\\\\o[0-7]{3}|\\\\x[0-9a-fA-F]{2}|\\\\u[0-9a-fA-F]{4})*\"");

        private static StringFormat _instance;
        public static StringFormat Instance => (_instance != null) ? _instance : (_instance = new StringFormat());

        private StringFormat() : base(TokenType.SpecialLiteral, pattern, FormatPriority.Strong) { }

        protected override Val OnParse(Match m) {
            var sb = new StringBuilder();
            foreach (Capture cap in m.Groups["char"].Captures) {
                sb.Append(CharFormat.Unescape(cap.Value));
            }
            return new StrVal(sb.ToString());
        }

        protected override string OnFormat(Val val, FormatSettings fs) {
            if (!(val is StrVal strVal)) {
                // 文字列以外にはデフォルトの表現を適用
                return base.OnFormat(val, new FormatSettings());
            }
            else {
                return FormatAsStringLiteral(strVal.AsString);
            }
        }

        public static string FormatAsStringLiteral(string str) {
            // 文字列表現への変換
            var sb = new StringBuilder();
            sb.Append('"');
            foreach (var c in str) {
                CharFormat.Escape(sb, c, true);
            }
            sb.Append('"');
            return sb.ToString();
        }
    }
}
