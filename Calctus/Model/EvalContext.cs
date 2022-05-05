using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shapoco.Calctus.Model.UnitSystem;

namespace Shapoco.Calctus.Model {
    class EvalContext {
        private Dictionary<string, Var> _vars = new Dictionary<string, Var>();
        public readonly UnitFactory Units = new UnitFactory();

        public Var Ref(Token name, bool allowCreate = false) {
            if (_vars.TryGetValue(name.Text, out Var v)) {
                return v;
            }
            else if (allowCreate) {
                var newVar = new Var(name);
                _vars.Add(name.Text, newVar);
                return newVar;
            }
            else {
                throw new EvalError(this, name, "variant not found: " + name);
            }
        }

        public void Warning(object place, string msg) {
            Console.WriteLine("*WARNING: " + msg);
        }
    }
}
