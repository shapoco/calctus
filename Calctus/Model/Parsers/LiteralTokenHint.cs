using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;

namespace Shapoco.Calctus.Model.Parsers {
    class LiteralTokenHint {
        public readonly Val Value;
        public LiteralTokenHint(Val v) {
            this.Value = v;
        }
    }
}
