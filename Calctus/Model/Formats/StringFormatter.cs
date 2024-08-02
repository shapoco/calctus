using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Formats {
    class StringFormatter : NumberFormatter {
        public StringFormatter() : base(new Regex("\"(?<char>[^\"\\\\]|\\\\[abfnrtv\"\\\\0]|\\\\o[0-7]{3}|\\\\x[0-9a-fA-F]{2}|\\\\u[0-9a-fA-F]{4})*\""), FormatPriority.Strong) { }

        public override Val Parse(Match m) {
            var sb = new StringBuilder();
            foreach (Capture cap in m.Groups["char"].Captures) {
                sb.Append(CharFormatter.Unescape(cap.Value));
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
                CharFormatter.Escape(sb, c, true);
            }
            sb.Append('"');
            return sb.ToString();
        }
    }
}
