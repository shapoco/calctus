using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Maths.BitArrays {
    using word = UInt32;
    using wide = UInt64;
    struct BitArray {
        public const int Stride = sizeof(word) * 8;

        public readonly int Width;
        private readonly word[] raw;

        public BitArray(int width) {
            Width = width;
            raw = CreateSegmentArray(width);
        }

        public BitArray(word[] x, int offset, int width, bool forceCopy) {
#if DEBUG
            Assert.ArgInRange(nameof(BitArray), nameof(offset), 0 <= offset && offset < width);
            Assert.ArgInRange(nameof(BitArray), nameof(width), 0 < width && width <= x.Length * Stride);
#endif
            this.Width = width;
            if (offset == 0 && MathEx.CeilDiv(width, Stride) == x.Length) {
                this.raw = forceCopy ? (word[])x.Clone() : x;
            }
            else {
                this.raw = new word[MathEx.CeilDiv(width, Stride)];
                x.CopyBits(offset, this.raw, 0, width);
            }
        }

        public word Msb => this[Width - 1];
        public int LastWordBits {
            get {
                var ret = Width % Stride;
                if (ret == 0) ret = Stride;
                return ret;
            }
        }
        public word LastWordMask => (word)(((wide)1 << LastWordBits) - 1);

        public bool IsZero {
            get {
                for (int i = 0; i < raw.Length- 1; i++) {
                    if (raw[i] != 0) return false;
                }
                return (raw[raw.Length - 1] & LastWordMask) == 0;
            }
        }

        // todo 性能改善: BitArray.this[]
        public word this[int pos] {
            get => raw.GetBit(pos);
            private set => raw.SetBit(pos, value);
        }

        public word Word(int i) => raw[i];

        public word[] GetSegments(bool signExt) => GetSegments(0, 0, signExt);

        // todo 性能改善: BitArray.Extend()
        public void ExtendSign(int pos) {
            var bit = this[pos];
            for (int i = pos + 1; i < raw.Length * Stride; i++) {
                this[i] = bit;
            }
        }

        public word[] GetSegments(int intExtend, int fracExtend, bool signExt) {
            var words = CreateSegmentArray(intExtend + Width + fracExtend);
            CopyTo(0, words, fracExtend, Width);
            if (signExt) words.ExtendSign(fracExtend + Width - 1);
            return words;
        }

        public void CopyTo(int isrc, word[] dest, int idest, int width)
            => raw.CopyBits(isrc, dest, idest, width);

        // todo もうちょっと真面目に実装: BitArray.GetHashCode()
        public override int GetHashCode() {
            word hash = 0;
            for (int i = 0; i < raw.Length; i++) {
                var tmp = raw[i];
                if (i == raw.Length - 1) tmp &= LastWordMask;
                hash ^= tmp;
            }
            return (int)hash;
        }

        public override string ToString() {
            var sb = new StringBuilder();
            var n = raw.Length;
            sb.Append(Convert.ToString(raw[n - 1], 16));
            for (int i = n - 2; i >= 0; i--) {
                sb.Append(" ");
                var tmp = ("0000000" + Convert.ToString(raw[i], 16));
                sb.Append(tmp.Substring(tmp.Length - 7));
            }
            return sb.ToString();
        }

        public static word[] CreateSegmentArray(int width) => new word[MathEx.CeilDiv(width, Stride)];

        public IEnumerable<uint> EnumBits() => EnumBits(0, Width);
        public IEnumerable<uint> EnumBits(int start, int length) => raw.EnumBits(start, length);
    }
}
