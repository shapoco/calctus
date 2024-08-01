using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Values;

namespace Shapoco.Calctus.Model.Parsers {
    class LiteralToken : Token {
        public readonly Val Value;
        public readonly int PostfixLength;

        public LiteralToken(TokenType t, TextPosition pos, string text, int postfixLen, Val val) : base(t, pos, text) {
#if DEBUG
            System.Diagnostics.Debug.Assert(postfixLen >= 0);
            System.Diagnostics.Debug.Assert(postfixLen < text.Length);
            System.Diagnostics.Debug.Assert(val != null);
#endif
            this.Value = val;
            this.PostfixLength = postfixLen;
        }

    }
}
