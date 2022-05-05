using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model {
    struct TextPosition {
        public int Index;
        public TextPosition(int pos = 0) {
            this.Index = 0;
        }
        public void Count(int c) {
            if (c < 0) return;
            Index++;
        }
        public void Count(string s) {
            foreach (var c in s) Count((int)c);
        }
    }
}
