using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shapoco.Calctus.Model.Syntax;

namespace Shapoco.Calctus.Model {
    class EvalContext {
        private Dictionary<string, Var> _vars = new Dictionary<string, Var>();
        public readonly EvalSettings Settings = new EvalSettings();

        private void AddConstantReal(string name, real value) {
            _vars.Add(name, new Var(new Token(TokenType.Symbol, TextPosition.Empty, name), new RealVal(value), true));
        }

        private void AddConstantHex(string name, decimal value) {
            _vars.Add(name, new Var(new Token(TokenType.Symbol, TextPosition.Empty, name), new RealVal((real)value).FormatHex(), true));
        }

        public EvalContext() {
            this.AddConstantReal("PI", (real)Math.PI);
            this.AddConstantReal("E", (real)Math.E);
            this.AddConstantHex("INT_MIN", Int32.MinValue);
            this.AddConstantHex("INT_MAX", Int32.MaxValue);
            this.AddConstantHex("UINT_MIN", UInt32.MinValue);
            this.AddConstantHex("UINT_MAX", UInt32.MaxValue);
            this.AddConstantHex("LONG_MIN", Int64.MinValue);
            this.AddConstantHex("LONG_MAX", Int64.MaxValue);
            this.AddConstantHex("ULONG_MIN", UInt64.MinValue);
            this.AddConstantHex("ULONG_MAX", UInt64.MaxValue);
            this.AddConstantReal("DECIMAL_MIN", real.MinValue);
            this.AddConstantReal("DECIMAL_MAX", real.MaxValue);
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
