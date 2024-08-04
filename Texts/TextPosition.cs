using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Texts {
    public struct TextPosition {
        public bool EndOfString;
        public readonly int Position;
        public TextPosition(bool eos, int pos) {
            this.EndOfString = eos;
            this.Position = pos;
        }

        public override string ToString() {
            if (EndOfString) {
                return "end of string";
            }
            else {
                return "pos=" + Position;
            }
        }
    }
}
