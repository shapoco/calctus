using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shapoco.Calctus.Model.Syntax;
using Shapoco.Calctus.Model.UnitSystem;

namespace Shapoco.Calctus.Model {
    class EvalContext {
        private Dictionary<string, Var> _vars = new Dictionary<string, Var>();
        public readonly UnitFactory Units = new UnitFactory();

        private void addConstantReal(string name, double value) {
            _vars.Add(name, new Var(new Token(TokenType.Symbol, TextPosition.Empty, name), new RealVal(value), true));
        }

        private void addConstantHex(string name, long value) {
            _vars.Add(name, new Var(new Token(TokenType.Symbol, TextPosition.Empty, name), new RealVal(value).FormatHex(), true));
        }

        public EvalContext() {
            this.addConstantReal("PI", Math.PI);
            this.addConstantReal("E", Math.E);
            this.addConstantHex("INT_MIN", Int32.MinValue);
            this.addConstantHex("INT_MAX", Int32.MaxValue);
            this.addConstantHex("UINT_MIN", UInt32.MinValue);
            this.addConstantHex("UINT_MAX", UInt32.MaxValue);
            this.addConstantReal("FLOAT_MIN", float.MinValue);
            this.addConstantReal("FLOAT_MAX", float.MaxValue);
            this.addConstantReal("DOUBLE_MIN", double.MinValue);
            this.addConstantReal("DOUBLE_MAX", double.MaxValue);
        }

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
