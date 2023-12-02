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
            var list = new List<int>();
            foreach(Capture cap in m.Groups["char"].Captures) {
                list.Add(CharFormatter.Unescape(cap.Value));
            }
            return new ArrayVal(list.ToArray(), new FormatHint(this));
        }

        protected override string OnFormat(Val val, FormatSettingss fs) {
            if (!(val is ArrayVal aval)) {
                // 配列以外にはデフォルトの表現を適用
                return base.OnFormat(val, fs);
            }

            var vals = (Val[])aval.Raw;
            if (!vals.All(p => p.IsInteger && char.MinValue <= p.AsReal && p.AsReal <= char.MaxValue)) {
                // char の範囲外の値や小数を含む場合はデフォルトの表現を適用
                return base.OnFormat(val, fs);
            }

            // 文字列表現への変換
            var sb = new StringBuilder();
            sb.Append('"');
            foreach(var c in vals) {
                CharFormatter.Escape(sb, (char)c.AsReal, true);
            }
            sb.Append('"');
            return sb.ToString();
        }
    }
}
