using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Shapoco.Texts;
using Shapoco.Calctus.Model.Values;

namespace Shapoco.Calctus.Model.Parsers {
    // todo StringReader に以降
    class StringReaderDep {
        private readonly string _text;
        private DeprecatedTextPosition _pos;

        private DeprecatedTextPosition _tokenPos;
        private StringBuilder _tokenBuf = new StringBuilder();

        public StringReaderDep(string text, int start = 0) {
            this._text = text;
            this._pos = new DeprecatedTextPosition(start);
        }

        public DeprecatedTextPosition Position => _pos;
        public bool Eos => (_pos.Index >= _text.Length);

        public DeprecatedTextPosition TokenPosition => _tokenPos;
        public string TokenString => _tokenBuf.ToString();

        public void Init() {
            _tokenPos = _pos;
            _tokenBuf.Clear();
        }

        public void Trackback() {
            _pos.Index -= 1;
            _tokenBuf.Remove(_tokenBuf.Length - 1, 1);
        }

        public Token FinishToken(TokenType tokenType, Val val, int postfixLen = 0) {
            return new LiteralToken(tokenType, _tokenPos, _tokenBuf.ToString(), postfixLen, val);
        }

        public Token FinishToken(TokenType tokenType) {
            return new Token(tokenType, _tokenPos, _tokenBuf.ToString());
        }

        public int Peek() {
            if (Eos) return -1;
            return _text[_pos.Index];
        }

        public int Read() {
            if (Eos) throw UnexpectedEos();
            int ci = Peek();
            char c = (char)ci;
            if (char.IsControl(c)) {
                throw new LexerError(_pos, "Unexpected control character: " + CharToString(c));
            }
            if (char.MinValue <= ci && ci <= char.MaxValue) {
                _tokenBuf.Append(c);
            }
            _pos.Count(ci);
            return ci;
        }

        public void Expect(int c) {
            if (!ReadIf(c)) {
                throw ExpectFailed((char)c);
            }
        }

        public bool ReadIf(int c) {
            bool hit = (Peek() == c);
            if (hit) Read();
            return hit;
        }

        public bool ReadIf(char[] charList, out char c) {
            var ci = Peek();
            c = (char)ci;
            bool hit = char.MinValue <= ci && ci <= char.MaxValue && charList.Contains((char)ci);
            if (hit) Read();
            return hit;
        }

        public bool ReadIf(char min, char max, out char c) {
            var ci = Peek();
            bool hit = (min <= ci && ci <= max);
            if (hit) {
                c = (char)Read();
            }
            else {
                c = '\0';
            }
            return hit;
        }

        public bool ReadIf(Func<int, bool> f, out int c) {
            if (f(Peek())) {
                c = Read();
                return true;
            }
            else {
                c = '\0';
                return false;
            }
        }

        public void SkipWhite() {
            while (true) {
                var ci = Peek();
                if (ci != ' ' && ci != '\t' && ci != '\r' && ci != '\n' && ci != '　') break;
                _pos.Count(ci);
            }
        }

        public LexerError UnexpectedEos()
            => new LexerError(_pos, "Unexpected Eos");
        public LexerError ExpectFailed(char c)
            => ExpectFailed(CharToString(c));
        public LexerError ExpectFailed(string pattern)
            => new LexerError(_pos, pattern + " is expected.");
        public LexerError InvalidToken(string msg = "Invalid token")
            => new LexerError(_tokenPos, _pos.Index - _tokenPos.Index, msg);

        public static string CharToString(int c) {
            if (c < -1) {
                return "EOS";
            }
            else {
                return CStyleEscaping.EscapeAndQuote((char)c);
            }
        }
    }
}
