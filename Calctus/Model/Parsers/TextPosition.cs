using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Parsers {
    // todo 廃止 TextPosition
    struct DeprecatedTextPosition {
        public static readonly DeprecatedTextPosition Empty = new DeprecatedTextPosition(0);
        public static readonly DeprecatedTextPosition Nowhere = new DeprecatedTextPosition(-1);

        public int Index;
        public DeprecatedTextPosition(int pos = 0) {
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
            if (other is DeprecatedTextPosition otherTp) {
                return Index == otherTp.Index;
            }
            else {
                return false;
            }
        }

        public override int GetHashCode() => Index;

        public override string ToString() => Index.ToString();

        public static bool operator ==(DeprecatedTextPosition left, DeprecatedTextPosition right) => left.Equals(right);
        public static bool operator !=(DeprecatedTextPosition left, DeprecatedTextPosition right) => !left.Equals(right);
    }
}
