using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shapoco.Calctus.Parser;

namespace Shapoco.Calctus.Model {
    class EvalError : Exception {
        public readonly EvalContext Context;
        public readonly Token Token;
        public EvalError(EvalContext ctx, Token tok, string msg) : base(msg) {
            this.Context = ctx;
            this.Token = tok;
        }
    }
}
