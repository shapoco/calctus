using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Graphs;

namespace Shapoco.Calctus.Model.Evaluations {
    class EvalContext {
        private Dictionary<string, Var> _vars = new Dictionary<string, Var>();
        public readonly EvalSettings Settings;
        public readonly List<PlotCall> PlotCalls = new List<PlotCall>();

        public void DefConst(string name, Val val, string desc) {
            _vars.Add(name, new Var(new Token(TokenType.Word, TextPosition.Nowhere, name), val, true, desc));
        }

        private void AddConstantReal(string name, real value, string desc) {
            DefConst(name, new RealVal(value), desc);
        }

        private void AddConstantHex(string name, decimal value, string desc) {
            DefConst(name, new RealVal((real)value).FormatHex(), desc);
        }

        public EvalContext() {
            Settings = new EvalSettings();
            this.AddConstantReal("PI", 3.1415926535897932384626433833m, "circle ratio");
            this.AddConstantReal("E", 2.7182818284590452353602874714m, "base of natural logarithm");
            this.AddConstantHex("INT_MIN", Int32.MinValue, "minimum value of 32 bit signed integer");
            this.AddConstantHex("INT_MAX", Int32.MaxValue, "maximum value of 32 bit signed integer");
            this.AddConstantHex("UINT_MIN", UInt32.MinValue, "minimum value of 32 bit unsigned integer");
            this.AddConstantHex("UINT_MAX", UInt32.MaxValue, "maximum value of 32 bit unsigned integer");
            this.AddConstantHex("LONG_MIN", Int64.MinValue, "minimum value of 64 bit signed integer");
            this.AddConstantHex("LONG_MAX", Int64.MaxValue, "maximum value of 64 bit signed integer");
            this.AddConstantHex("ULONG_MIN", UInt64.MinValue, "minimum value of 64 bit unsigned integer");
            this.AddConstantHex("ULONG_MAX", UInt64.MaxValue, "maximum value of 64 bit unsigned integer");
            this.AddConstantReal("DECIMAL_MIN", real.MinValue, "minimum value of Decimal");
            this.AddConstantReal("DECIMAL_MAX", real.MaxValue, "maximum value of Decimal");
        }

        public EvalContext(EvalContext src) {
            foreach(var k in src._vars.Keys) {
                _vars.Add(k, (Var)src._vars[k].Clone());
            }
            Settings = (EvalSettings)src.Settings.Clone();
        }

        public void ApplyFormatSettings() {
            // 設定を評価コンテキストに反映する
            var s = Calctus.Settings.Instance;
            Settings.DecimalLengthToDisplay = s.NumberFormat_Decimal_MaxLen;
            Settings.ENotationEnabled = s.NumberFormat_Exp_Enabled;
            Settings.ENotationExpPositiveMin = s.NumberFormat_Exp_PositiveMin;
            Settings.ENotationExpNegativeMax = s.NumberFormat_Exp_NegativeMax;
            Settings.ENotationAlignment = s.NumberFormat_Exp_Alignment;
        }

        public Var Ref(Token name, bool allowCreate) {
            if (_vars.TryGetValue(name.Text, out Var v)) {
                return v;
            }
            else if (allowCreate) {
                var newVar = new Var(name, "user-defined variable");
                _vars.Add(name.Text, newVar);
                return newVar;
            }
            else {
                throw new EvalError(this, name, "variant not found: " + name);
            }
        }

        public Var Ref(string name, bool allowCreate) {
            return Ref(new Token(TokenType.Word, TextPosition.Nowhere, name), allowCreate);
        }

        public void Undef(string name, bool ignoreError) {
            if (_vars.ContainsKey(name)) { 
                _vars.Remove(name);
            }
            else if (!ignoreError) {
                throw new IndexOutOfRangeException();
            }
        }

        public void Warning(object place, string msg) {
            Console.WriteLine("*WARNING: " + msg);
        }

        public IEnumerable<Var> EnumVars() => _vars.Values;

        public IEnumerable<FuncDef> EnumUserFuncs() => _vars.Values.Where(p => p.Value is FuncVal).Select(p => (FuncDef)((FuncVal)p.Value).Raw);
    }
}
