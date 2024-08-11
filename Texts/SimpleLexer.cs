using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shapoco.Maths;

namespace Shapoco.Texts {
    class SimpleLexer {
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
            if (!TryPop(token)) throw _in.CreateExpectedException(CStyleEscaping.EscapeAndQuote(token));
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

        public string TryPopDigitSequence(Radix radix, out byte[] digits) {
            if (TryEatNumbers(out digits, radix)) return _in.PopToken();
            throw _in.CreateExpectedException("Digit sequence");
        }

        public void AssertEos() {
            if (!Eos) throw _in.CreateExpectedException("EOS");
        }

        public void Pop(char[] cands, out char token) {
            if (!TryPop(cands, out token)) _in.CreateExpectedException(cands.ToStringForDisplay());
        }

        public bool TryPop(char[] cands, out char token) {
            foreach(var c in cands) {
                if (_in.TryEat(c)) {
                    token = c;
                    _in.PopToken();
                    return true;
                }
            }
            token = '\0';
            return false;
        }

        public bool TryPop(char token) {
            if (_autoSkipWhite) _in.SkipWhite();
            if (_in.TryEat(token)) {
                _in.PopToken();
                return true;
            }
            else {
                return false;
            }
        }

        public bool TryPop(string token) {
            if (_autoSkipWhite) _in.SkipWhite();
            if (_in.TryEat(token)) {
                _in.PopToken();
                return true;
            }
            else {
                return false;
            }
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

            if (_in.TryEat('.')) {
                value += DigitsAsFraction(EatNumbers());
            }

            if (_in.TryEat('e')) {
                int sign = 1;
                if (_in.TryEat('-')) sign = -1;
                else _in.TryEat('+');
                var exp = DigitsAsInteger(EatNumbers());
                value *= MathEx.Log10(sign * exp);
            }

            _in.PopToken();
            return true;
        }

        public bool TryPopCStyleString(out string value) {
            if (_autoSkipWhite) _in.SkipWhite();
            if (_in.TryEat('\"')) {
                var sb = new StringBuilder();
                while (!_in.TryEat('\"')) {
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

        public static decimal DigitsAsInteger(byte[] digits, Radix radix = Radix.Decimal) {
            var baseNumber = radix.ToBaseNumber();
            decimal val = 0;
            foreach (var d in digits) {
                val = (val * baseNumber) + d;
            }
            return val;
        }

        public decimal DigitsAsFraction(byte[] digits) {
            decimal val = 0;
            for (int i = digits.Length - 1; i >= 0; i++) {
                val = (val + digits[0]) / 10;
            }
            return val;
        }

        public byte[] EatNumbers(Radix radix = Radix.Decimal) {
            if (TryEatNumbers(out byte[] digits, radix)) return digits;
            throw _in.CreateExpectedException("Numbers");
        }

        public bool TryEatNumbers(out byte[] digits, Radix radix = Radix.Decimal) {
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
            if (_in.TryEat('\\')) {
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
                        _in.EatDigit(out d[1], Radix.Hexadecimal);
                        _in.EatDigit(out d[0], Radix.Hexadecimal);
                        return (char)((d[1] << 4) | d[0] << 0);
                    case 'u':
                        _in.EatDigit(out d[3], Radix.Hexadecimal);
                        _in.EatDigit(out d[2], Radix.Hexadecimal);
                        _in.EatDigit(out d[1], Radix.Hexadecimal);
                        _in.EatDigit(out d[0], Radix.Hexadecimal);
                        return (char)((d[1] << 12) | (d[1] << 8) | (d[1] << 4) | (d[0] << 0));
                    default:
                        throw _in.CreateException("Unrecognized escaped char: \"\\" + CStyleEscaping.Escape(c.ToString()) + "\"");
                }
            }
            else {
                return _in.Eat();
            }
        }

        public void SkipWhite() => _in.SkipWhite();
    }
}
