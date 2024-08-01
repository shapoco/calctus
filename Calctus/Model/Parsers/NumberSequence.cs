using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Parsers {
    class NumberSequence {
        private readonly List<int> _buf = new List<int>();
        public Radix Radix { get; private set; }
        public readonly TextPosition Start;
        public int Length { get; private set; } = 0;

        public NumberSequence(Radix radix, TextPosition start) { 
            this.Radix = radix;
            this.Start = start;
        }

        public void Append(char c) {
            if ('0' <= c && c <= '9') {
                _buf.Add(c - '0');
            }
            else if (Radix == Radix.Hexadecimal && 'a' <= c && c <= 'f') {
                _buf.Add(10 + (c - 'a'));
            }
            else if (Radix == Radix.Hexadecimal && 'A' <= c && c <= 'F') {
                _buf.Add(10 + (c - 'A'));
            }
            Length += 1;
        }

        public void ChangeRadix(Radix radix) {
            this.Radix = radix;
        }

        public decimal ToDecimal(string label = "Number", decimal min = 0, decimal max = decimal.MaxValue) {
            int radixNum = Radix.GetBaseNumber();
            decimal result = 0;
            for (int i = 0; i < _buf.Count; i++) {
                int n = _buf[i];
                if (n >= radixNum) {
                    throw new LexerError(Start, Length, n + " is not " + Radix + " number.");
                }
                result *= radixNum;
                result += n;
            }
            if (min <= result && result <= max) {
                return result;
            }
            else {
                throw new LexerError(Start, Length, label + " out of range.");
            }
        }

        public int ToInt(string label = "Number", int min = 0, int max = int.MaxValue) {
            return (int)ToDecimal(label, min, max);
        }

        public decimal ToFraction() {
            int radixNum = Radix.GetBaseNumber();
            decimal result = 0;
            for (int i = _buf.Count - 1; i >= 0; i--) {
                int n = _buf[i];
                if (n >= radixNum) {
                    throw new LexerError(Start, Length, n + " is not " + Radix + " number.");
                }
                result += n;
                result /= radixNum;
            }
            return result;
        }
    }
}
