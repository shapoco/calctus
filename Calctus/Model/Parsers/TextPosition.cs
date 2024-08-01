using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Parsers {
    // todo 廃止 TextPosition
    struct TextPosition {
        public static readonly TextPosition Empty = new TextPosition(0);
        public static readonly TextPosition Nowhere = new TextPosition(-1);

        public int Index;
        public TextPosition(int pos = 0) {
            Index = pos;
        }
        public void Count(int c) {
            if (c < 0) return;
            Index++;
        }
        public void Count(string s) {
            foreach (var c in s) Count((int)c);
        }

        public override bool Equals(object other) {
            if (other is TextPosition otherTp) {
                return Index == otherTp.Index;
            }
            else {
                return false;
            }
        }

        public override int GetHashCode() => Index;

        public override string ToString() => Index.ToString();

        public static bool operator ==(TextPosition left, TextPosition right) => left.Equals(right);
        public static bool operator !=(TextPosition left, TextPosition right) => !left.Equals(right);
    }
}
