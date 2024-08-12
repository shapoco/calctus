using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Maths {
    struct IntFormat {
        public static readonly IntFormat Empty = new IntFormat(false, 0, false);

        public bool Signed;
        public int Width;

        public int SignWidth => Signed ? 1 : 0;
        public int WidthWithoutSign => Width - SignWidth;

        public bool IsEmpty => Width <= 0;

        public IntFormat(bool signed, int width) {
            this.Signed = signed;
            this.Width = width;
        }

        private IntFormat(bool signed, int width, bool dummy) {
            this.Signed = signed;
            this.Width = width;
        }

        public override string ToString() {
            return (Signed ? "s" : "u") + Width;
        }
    }
}
