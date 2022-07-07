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

        private void AddConstantReal(string name, double value) {
            _vars.Add(name, new Var(new Token(TokenType.Symbol, TextPosition.Empty, name), new RealVal(value), true));
        }

        private void AddConstantHex(string name, long value) {
            _vars.Add(name, new Var(new Token(TokenType.Symbol, TextPosition.Empty, name), new RealVal(value).FormatHex(), true));
        }

        public EvalContext() {
            this.AddConstantReal("PI", Math.PI);
            this.AddConstantReal("E", Math.E);
            this.AddConstantReal("NaN", double.NaN);
            this.AddConstantReal("∞", double.PositiveInfinity);
            this.AddConstantHex("INT_MIN", Int32.MinValue);
            this.AddConstantHex("INT_MAX", Int32.MaxValue);
            this.AddConstantHex("UINT_MIN", UInt32.MinValue);
            this.AddConstantHex("UINT_MAX", UInt32.MaxValue);
            this.AddConstantReal("FLOAT_MIN", float.MinValue);
            this.AddConstantReal("FLOAT_MAX", float.MaxValue);
            this.AddConstantReal("DOUBLE_MIN", double.MinValue);
            this.AddConstantReal("DOUBLE_MAX", double.MaxValue);
        }

        public Var Ref(Token name, bool allowCreate) {
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

        public Var Ref(string name, bool allowCreate) {
            return Ref(new Token(TokenType.Word, TextPosition.Empty, name), allowCreate);
        }

        public void Warning(object place, string msg) {
            Console.WriteLine("*WARNING: " + msg);
        }
    }
}
