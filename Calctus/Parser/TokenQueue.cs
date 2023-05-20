using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model;
using Shapoco.Calctus.Model.Syntax;

namespace Shapoco.Calctus.Parser {
    class TokenQueue : List<Token>, ICloneable {
        private TextPosition _lastPos = TextPosition.Empty;

        public void PushBack(Token token) {
            Add(token);
        }

        public Token PopFront() {
            var t = this[0];
            RemoveAt(0);
            _lastPos = t.Position;
            return t;
        }

        public TextPosition Position {
            get {
                if (Count > 0) {
                    return this[0].Position;
                }
                else {
                    return _lastPos;
                }
            }
        }

        public void CompleteParentheses() {
            int depth = 0;
            foreach (var t in this) {
                if (t.Type == TokenType.GeneralSymbol) {
                    if (t.Text == "(") {
                        depth++;
                    }
                    else if (t.Text == ")") {
                        depth--;
                    }
                }
            }
            while (depth < 0) {
                Insert(0, new Token(TokenType.GeneralSymbol, TextPosition.Nowhere, "("));
                depth++;
            }
        }

        public object Clone() {
            var clone = new TokenQueue();
            foreach(var t in this) {
                clone.PushBack(t);
            }
            return clone;
        }
    }
}
