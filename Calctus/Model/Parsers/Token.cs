using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;

namespace Shapoco.Calctus.Model.Parsers {
    class Token {
        public static readonly Token Empty = new Token(TokenType.Empty, TextPosition.Nowhere, "");

        public readonly TokenType Type;
        public readonly TextPosition Position;
        public readonly string Text;
        public readonly object Hint;
        public Token(TokenType t, TextPosition pos, string text, object hint = null) {
            this.Type = t;
            this.Position = pos;
            this.Text = text;
            this.Hint = hint;
        }
        public override string ToString() {
            if (Type == TokenType.Eos) {
                return "[EOS]";
            }
            else { 
                return "'" + Text + "'";
            }
        }
    }

    class NumberTokenHint {
        public readonly Val Value;
        public NumberTokenHint(Val v) {
            this.Value = v;
        }
    }

}
