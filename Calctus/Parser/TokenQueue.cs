using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model;
using Shapoco.Calctus.Model.Syntax;

namespace Shapoco.Calctus.Parser {
    class TokenQueue {
        private List<Token> _tokens = new List<Token>();
        private Token _eos = null;

        public void Enqueue(Token token) {
            _tokens.Add(token);
        }

        public Token Dequeue() {
            if (_tokens.Count > 0) {
                var t = _tokens[0];
                _tokens.RemoveAt(0);
                if (t.Type == TokenType.Eos) {
                    _eos = t;
                }
                return t;
            }
            else {
                return _eos;
            }
        }

        public TextPosition Position {
            get {
                if (_tokens.Count > 0) {
                    return _tokens[0].Position;
                }
                else {
                    return _eos.Position;
                }
            }
        }

        public Token this[int index] => _tokens[index];
        public int Count => _tokens.Count;
    }
}
