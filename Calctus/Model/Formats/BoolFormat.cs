using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Standards;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;

namespace Shapoco.Calctus.Model.Formats {
    class BoolFormat : ValFormat {
        private static readonly Regex pattern = new Regex("(" + Keyword.True.Token + "|" + Keyword.False.Token + ")");

        public static string FormatAsStringLiteral(bool val) => val ? Keyword.True.Token : Keyword.False.Token;

        private static BoolFormat _instance;
        public static BoolFormat Instance => (_instance != null) ? _instance : (_instance = new BoolFormat());

        private BoolFormat() : base(TokenType.SpecialLiteral, pattern, FormatPriority.AlwaysLeft) { }

        protected override Val OnParse(Match m) {
            var tok = m.Groups[1].Value;
            if (tok == Keyword.True.Token) return BoolVal.True;
            if (tok == Keyword.False.Token) return BoolVal.False;
            throw new CalctusError("'" + tok + "' is not bool value.");
        }

        protected override string OnFormat(Val val, FormatSettings fs) {
            if (!(val is BoolVal boolVal)) {
                return base.OnFormat(val, fs);
            }
            return FormatAsStringLiteral(boolVal.AsBool); 
        }
    }
}

