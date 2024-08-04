using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Texts {
    public class TokenBuffer {
        private string _s;
        private int _i = 0;
        private StringBuilder _buf = new StringBuilder();

        public TokenBuffer(string s) {
            this._s = s;
        }

        public bool Eos => (_i >= _s.Length);
        public TextPosition Position => new TextPosition(Eos, _i);

        public string PopToken() {
            var tok = _buf.ToString();
            _buf.Clear();
            return tok;
        }

        //public void BackTrack(int n = 1) {
        //    if (_i < n || _buf.Length < n) throw CreateException("Token buffer broken.");
        //    _buf.Remove(_buf.Length - n, n);
        //    _i -= n;
        //}

        public char EatDigit(out byte digit, int radix = 10, string rule = "number") {
            if (EatIfDigit(out char c, out digit, radix)) return c;
            throw CreateExpectedException(rule);
        }

        public bool EatIfDigit(out char c, out byte value, int radix = 10) {
            if (radix < 2 || 16 < radix) {
                throw CreateException(nameof(radix) + " out of range for " + nameof(TokenBuffer) + "." + nameof(EatIfDigit) + "()");
            }

            if (EatIf('0', (char)('0' + Math.Min(10, radix) - 1), out c)) {
                value = (byte)(c - '0');
                return true;
            }
            else if (radix >= 10) {
                if (EatIf('a', (char)('a' + (radix - 10) - 1), out c)) {
                    value = (byte)(c - 'a' + 10);
                    return true;
                }
                else if (EatIf('A', (char)('A' + (radix - 10) - 1), out c)) {
                    value = (byte)(c - 'A' + 10);
                    return true;
                }
            }
            value = 0xff;
            return false;
        }

        public bool EatIfLowerAlpha(out char c)
            => EatIf('a', 'z', out c);

        public bool EatIfUpperAlpha(out char c)
            => EatIf('A', 'Z', out c);

        public bool EatIfAlpha(out char c)
            => EatIfLowerAlpha(out c) || EatIfUpperAlpha(out c);

        public char ExpectIdStart(string rule = "identifier") {
            if (EatIfIdStart(out char c)) return c;
            throw CreateExpectedException(rule);
        }

        public bool EatIfIdStart(out char c)
            => EatIfAlpha(out c) || EatIf('_');

        public bool EatIfIdFollowing(out char c)
            => EatIfIdStart(out c) || EatIfDigit(out c, out _);

        private bool EatIf(char min, char max, out char c) {
            var ci = Peek();
            var ret = min <= ci && ci <= max;
            if (ret) c = Eat();
            else c = '\0';
            return ret;
        }

        public bool EatIf(char c) {
            var ret = Peek() == c;
            if (ret) Eat();
            return ret;
        }

        public void SkipWhite() {
            if (_buf.Length != 0) throw CreateException("Token buffer is not empty.");
            while (skipIfWhite()) { }
        }

        private bool skipIfWhite() {
            var ci = Peek();
            var ret = (ci == ' ' || ci == '\t' || ci == '\r' || ci == '\n' || ci == '　');
            if (ret) Eat(eat: false);
            return ret;
        }

        public void Eat(string seq) {
            if (!TryEat(seq)) throw CreateExpectedException(CStyleEscaping.EscapeAndQuote(seq));
        }

        public bool TryEat(string seq) {
            var n = seq.Length;
            var ret = (Peek(n) == seq);
            if (ret) Eat(n);
            return ret;
        }

        public void Eat(int n, StringBuilder sb = null) {
            for (int i = 0; i < n; i++) {
                var c = Eat();
                sb?.Append(c);
            }
        }

        public char Eat(bool eat = true) {
            if (Eos) throw new IndexOutOfRangeException(nameof(TokenBuffer) + "." + nameof(Eat) + "() failed.");
            char c = _s[_i++];
            if (eat) _buf.Append(c);
            return c;
        }

        public string Peek(int n) {
            int remaining = _s.Length - _i;
            if (n <= remaining ) {
                return _s.Substring(_i, n);
            }
            else {
                return _s.Substring(_i, remaining);
            }
        }

        public int Peek() => Eos ? -1 : _s[_i];

        public LexerException CreateExpectedException(string rule)
            => CreateException(rule + " is expected.");

        public LexerException CreateException(string msg)
            => new LexerException(Position, msg);
    }
}
