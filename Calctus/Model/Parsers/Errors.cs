using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shapoco.Calctus.Model;

namespace Shapoco.Calctus.Model.Parsers {
    class LexerError : CalctusError {
        public static readonly LexerError EmptyError = new LexerError(new DeprecatedTextPosition(0), "Empty expression");

        public readonly DeprecatedTextPosition Position;
        public readonly int Length;
        public LexerError(DeprecatedTextPosition pos, int length, string msg) : base(msg) {
            this.Position = pos;
            this.Length = length;
        }
        public LexerError(DeprecatedTextPosition pos, string msg) : this(pos, 1, msg) { }
    }

    class ParserError : CalctusError {
        public readonly Token Token;
        public ParserError(Token token, string msg) : base(msg) {
            Token = token;
        }
    }
}
