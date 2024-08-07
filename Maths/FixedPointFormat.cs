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
    }
}
