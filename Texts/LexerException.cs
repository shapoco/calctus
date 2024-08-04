using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Texts {
    public class LexerException : Exception {
        public readonly TextPosition Position;
        public LexerException(TextPosition pos, string msg) : base(msg + " (" + pos.ToString() + ")") {
            this.Position = pos;
        }
    }
}
