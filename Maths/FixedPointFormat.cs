using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Maths {
    struct FixedPointFormat {
        public static readonly FixedPointFormat Empty = new FixedPointFormat();

        public IntFormat RawFormat;

        public bool Signed => RawFormat.Signed;
        public int Width => RawFormat.Width;
        public int FracWidth;
        public int IntWidth => Width - FracWidth;
        public int SignWidth => Signed ? 1 : 0;
        public int WidthWithoutSign => Width - SignWidth;
        public int IntWidthWithoutSign => IntWidth - SignWidth;

        public bool IsEmpty => RawFormat.IsEmpty;

        private FixedPointFormat(bool dummy = false) {
            this.RawFormat = IntFormat.Empty;
            this.FracWidth = 0;
        }

        public FixedPointFormat(bool signed, int width, int fracWidth)
            : this(new IntFormat(signed, width), fracWidth) { }

        public FixedPointFormat(IntFormat fmt, int fracWidth) {
            this.RawFormat = fmt;
            this.FracWidth = fracWidth;
        }

        public override string ToString() {
            return (Signed ? "s" : "u") + IntWidth + "." + FracWidth;
        }
    }
}
