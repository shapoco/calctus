using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Mathematics;
using Shapoco.Calctus.Model.Graphs;
using Shapoco.Calctus.Model.Functions;

namespace Shapoco.Calctus.Model.Evaluations {
    class EvalContext {
        private Dictionary<string, Var> _vars = new Dictionary<string, Var>();
        private List<UserFuncDef> _userFuncs = new List<UserFuncDef>();
        public readonly EvalSettings EvalSettings;
        public readonly List<PlotCall> PlotCalls = new List<PlotCall>();
        public readonly int Depth;

        public void DefConst(string name, Val val, string desc) {
            _vars.Add(name, new Var(new Token(TokenType.Word, TextPosition.Nowhere, name), val, true, desc));
        }

        public void DefFunc(UserFuncDef func) {
            if (string.IsNullOrEmpty(func.Name.Text)) {
                throw new CalctusError("Function name is required.");
            }
            _userFuncs.Add(func);
        }

        private void AddConstantReal(string name, real value, string desc) {
            DefConst(name, new RealVal(value), desc);
        }

        private void AddConstantHex(string name, decimal value, string desc) {
            DefConst(name, new RealVal((real)value).FormatHex(), desc);
        }

        public EvalContext() {
            EvalSettings = new EvalSettings();
            Depth = 0;
            this.AddConstantReal("PI", RMath.PI, "circle ratio");
            this.AddConstantReal("E", RMath.E, "base of natural logarithm");
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
            Depth = src.Depth + 1;
            if (Depth >= Settings.Instance.Calculation_Limit_MaxCallRecursions) {
                throw new EvalError(this, null, "Depth of recursion exceeds limit.");
            }
            foreach (var k in src._vars.Keys) {
                _vars.Add(k, (Var)src._vars[k].Clone());
            }
            _userFuncs.AddRange(src._userFuncs);
            EvalSettings = (EvalSettings)src.EvalSettings.Clone();
        }

        public bool Ref(Token name , bool allowCreate, out Var v) {
            if (_vars.TryGetValue(name.Text, out v)) {
                return true;
            }
            else if (allowCreate) {
                v = new Var(name, "user-defined variable");
                _vars.Add(name.Text, v);
                return true;
            }
            else {
                return false;
            }
        }

        public Var Ref(Token name, bool allowCreate) {
            if (Ref(name, allowCreate, out Var v)) {
                return v;
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

        public IEnumerable<UserFuncDef> EnumUserFuncs() => _userFuncs;

        public IEnumerable<FuncDef> AllNamedFuncs() =>
            EmbeddedFuncDef.NativeFunctions.Select(p => (FuncDef)p).Concat(ExternalFuncDef.ExternalFunctions).Concat(_userFuncs);

        public bool SolveFunc(string name , out FuncDef func) {
            func = AllNamedFuncs().FirstOrDefault(p => p.Name != null && p.Name.Text == name);
            return func != null;
        }

        public bool SolveFunc(string name, Val[] args, out FuncDef func, out string message) {
            var matches = new List<FuncMatch>();
            FuncMatchLevel level = FuncMatchLevel.NameUnmatched;
            scanFunc(matches, ref level, AllNamedFuncs(), name, args);
            if (_vars.TryGetValue(name, out Var v) && v.Value is FuncVal funcVal) {
                matchFunc(matches, ref level, (UserFuncDef)funcVal.Raw, "", args);
            }
            func = null;
            message = null;
            switch (level) {
                case FuncMatchLevel.NameUnmatched:
                    message = "Function not found.";
                    break;
                case FuncMatchLevel.ArgumentsUnmatched:
                    if (matches.Count == 1) {
                        message = matches[0].Message;
                    }
                    else {
                        var sb = new StringBuilder();
                        var numArgs = matches.Select(p => p.Func.Args.Count).Distinct().OrderBy(p => p).ToArray();
                        int n = numArgs.Length;
                        for (int i = 0; i <n; i++) {
                            if (0 < i) {
                                if (i < n - 1) sb.Append(", ");
                                else sb.Append(" or ");
                            }
                            sb.Append(numArgs[i]);
                        }
                        message = sb.ToString() + " args are expected.";
                    }
                    break;
                case FuncMatchLevel.Matched:
                    if (matches.Count == 1) {
                        func = matches[0].Func;
                    }
                    else {
                        message = "Multiple (" + matches.Count + ") function definitions found.";
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            return (func != null);
        }

        private void scanFunc(List<FuncMatch> matches, ref FuncMatchLevel level, IEnumerable<FuncDef> funcs, string name, Val[] args) {
            foreach (var f in funcs) {
                matchFunc(matches, ref level, f, name, args);
            }
        }

        private void matchFunc(List<FuncMatch> matches, ref FuncMatchLevel level, FuncDef func, string name, Val[] args) {
            FuncMatch m = func.Match(name, args);
            if (m.MatchLevel == level) {
                matches.Add(m);
            }
            else if (m.MatchLevel >= level) {
                matches.Clear();
                matches.Add(m);
                level = m.MatchLevel;
            }
        }


    }
}
