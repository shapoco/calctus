using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shapoco.Calctus.Model;

namespace Shapoco.Calctus.Parser {
    class LexerError : CalctusError {
        public TextPosition Position { get; private set; }
        public LexerError(TextPosition pos, string msg) : base(msg) {
            this.Position = pos;
        }
    }

    class ParserError : CalctusError {
        public Token Token { get; private set; }
        public ParserError(Token token, string msg) : base(msg) {
            Token = token;
        }
    }
}
