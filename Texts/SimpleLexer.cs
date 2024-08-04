using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shapoco.Maths;

namespace Shapoco.Texts {
    public class SimpleLexer {
        private TokenBuffer _in;
        private bool _autoSkipWhite;
        public TokenBuffer Input => _in;

        public SimpleLexer(string s, bool autoSkipWhite = true) { 
            this._in = new TokenBuffer(s);
            this._autoSkipWhite = autoSkipWhite;
        }

        public bool Eos {
            get {
                if (_autoSkipWhite) _in.SkipWhite();
                return _in.Eos;
            }
        }

        public void Pop(string token) {
            if (!TryPop(token)) throw _in.CreateExpectedException("'" + CStyleEscaping.Escape(token) + "'");
        }

        public string PopId() {
            if (TryPopId(out string id)) return id;
            throw _in.CreateExpectedException("Iedntifier");
        }

        public decimal PopUnsignedDecimal() {
            if (TryPopUnsignedDecimal(out decimal val)) return val;
            throw _in.CreateExpectedException("Number");
        }

        public string PopCStyleString() {
            if (TryPopCStyleString(out string val)) return val;
            throw _in.CreateExpectedException("String");
        }

        public void AssertEos() {
            if (!Eos) throw _in.CreateExpectedException("EOS");
        }

        public bool TryPop(string token) {
            if (_autoSkipWhite) _in.SkipWhite();
            return _in.TryEat(token);
        }

        public bool TryPopId(out string token) {
            if (_autoSkipWhite) _in.SkipWhite();
            if (_in.EatIfIdStart(out _)) {
                while (_in.EatIfIdFollowing(out _)) { }
                token = _in.PopToken();
                return true;
            }
            else {
                token = null;
                return false;
            }
        }

        public bool TryPopUnsignedDecimal(out decimal value) {
            if (_autoSkipWhite) _in.SkipWhite();
            value = 0;
            if (TryEatNumbers(out byte[] digits)) {
                value += DigitsAsInteger(digits);
            }
            else {
                return false;
            }

            if (_in.EatIf('.')) {
                value += DigitsAsFraction(EatNumbers());
            }

            if (_in.EatIf('e')) {
                int sign = 1;
                if (_in.EatIf('-')) sign = -1;
                else _in.EatIf('+');
                var exp = DigitsAsInteger(EatNumbers());
                value *= MathEx.Log10(sign * exp);
            }

            _in.PopToken();
            return true;
        }

        public bool TryPopCStyleString(out string value) {
            if (_autoSkipWhite) _in.SkipWhite();
            if (_in.EatIf('\"')) {
                var sb = new StringBuilder();
                while (!_in.EatIf('\"')) {
                    sb.Append(EatStringChar());
                }
                value = sb.ToString();
                _in.PopToken();
                return true;
            }
            else {
                value = null;
                return false;
            }
        }

        public static decimal DigitsAsInteger(byte[] digits, int radix = 10) {
            decimal val = 0;
            foreach (var d in digits) {
                val = (val * radix) + d;
            }
            return val;
        }

        public decimal DigitsAsFraction(byte[] digits, int radix = 10) {
            decimal val = 0;
            for (int i = digits.Length - 1; i >= 0; i++) {
                val = (val + digits[0]) / 10;
            }
            return val;
        }

        public byte[] EatNumbers(int radix = 10) {
            if (TryEatNumbers(out byte[] digits, radix)) return digits;
            throw _in.CreateExpectedException("Numbers");
        }

        public bool TryEatNumbers(out byte[] digits, int radix = 10) {
            if (_in.EatIfDigit(out _, out byte d, radix)) {
                var list = new List<byte>();
                list.Add(d);
                while (_in.EatIfDigit(out _, out d, radix)) {
                    list.Add(d);
                }
                digits = list.ToArray();
                return true;
            }
            else {
                digits = null;
                return false;
            }
        }

        public char EatStringChar() {
            if (_in.EatIf('\\')) {
                byte[] d = new byte[4];
                var c = _in.Eat();
                switch (c) {
                    case 'a': return '\a';
                    case 'b': return '\b';
                    case 'f': return '\f';
                    case 'n': return '\n';
                    case 'r': return '\r';
                    case 't': return '\t';
                    case 'v': return '\v';
                    case '\\': return '\\';
                    case '\'': return '\'';
                    case '\"': return '"';
                    case '0': return '\0';
                    case 'x':
                        _in.EatDigit(out d[1], 16);
                        _in.EatDigit(out d[0], 16);
                        return (char)((d[1] << 4) | d[0] << 0);
                    case 'u':
                        _in.EatDigit(out d[3], 16);
                        _in.EatDigit(out d[2], 16);
                        _in.EatDigit(out d[1], 16);
                        _in.EatDigit(out d[0], 16);
                        return (char)((d[1] << 12) | (d[1] << 8) | (d[1] << 4) | (d[0] << 0));
                    default:
                        throw _in.CreateException("Unrecognized escaped char: \"\\" + CStyleEscaping.Escape(c.ToString()) + "\"");
                }
            }
            else {
                return _in.Eat();
            }
        }

    }
}
