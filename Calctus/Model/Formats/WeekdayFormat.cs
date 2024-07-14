using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Globalization;
using Shapoco.Calctus.Model.Standards;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Formats {
    class WeekdayFormat : ValFormat {
        private static readonly Regex pattern = new Regex("(" + string.Join("|", Keyword.WeekdayTokens) + ")");

        private static WeekdayFormat _instance;
        public static WeekdayFormat Instance => (_instance != null) ? _instance : (_instance = new WeekdayFormat());

        private WeekdayFormat() : base(TokenType.SpecialLiteral, pattern, FormatPriority.AlwaysLeft) { }

        protected override Val OnParse(Match m) {
            var tok = m.Groups[1].Value;
            var index = Array.IndexOf(Keyword.WeekdayTokens, tok);
            System.Diagnostics.Debug.Assert(RealVal.WeekdayMin <= index && index <= RealVal.WeekdayMax);
            return new RealVal(index, new FormatHint(this));
        }

        protected override string OnFormat(Val val, FormatSettings fs) {
            if (!(val is RealVal)) {
                return base.OnFormat(val, fs);
            }

            var fval = val.AsReal;
            if (!val.IsInteger || fval < RealVal.WeekdayMin || RealVal.WeekdayMax < fval) {
                // 範囲外の値はデフォルトの数値表現を使用
                return base.OnFormat(val, fs);
            }
            else {
                // 曜日を表す識別子に変換
                return Keyword.WeekdayTokens[val.AsInt];
            }
        }
    }
}
