using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Maths {
    struct FixedPointFormat {
        public static readonly FixedPointFormat Empty = new FixedPointFormat();

        public readonly bool Signed;
        public readonly int Width;
        public readonly int FracWidth;
        public int IntWidth => Width - FracWidth;
        public int SignWidth => Signed ? 1 : 0;
        public int WidthWithoutSign => Width - SignWidth;
        public int IntWidthWithoutSign => IntWidth - SignWidth;

        public IntFormat RawFormat => new IntFormat(Signed, Width);

        private FixedPointFormat(bool dummy = false) {
            this.Signed = false;
            this.Width = 0;
            this.FracWidth = 0;
        }

        public FixedPointFormat(bool signed, int width, int fracWidth) {
            this.Signed = signed;
            this.Width = width;
            this.FracWidth = fracWidth;
        }

        public override string ToString() {
            return (Signed ? "s" : "u") + IntWidth + "." + FracWidth;
        }
    }
}
