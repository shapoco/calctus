using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shapoco.Calctus.Model;

namespace Shapoco.Calctus.Parser {
    class SyntaxError : CalctusError {
        public TextPosition Position { get; private set; }
        public SyntaxError(TextPosition pos, string msg) : base(msg) {
            this.Position = pos;
        }
    }
}
