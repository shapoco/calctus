using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Maths;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Graphs;
using Shapoco.Calctus.Model.Functions;
using Shapoco.Calctus.Model.Formats;

namespace Shapoco.Calctus.Model.Evaluations {
    class EvalContext {

        private Dictionary<string, Var> _vars = new Dictionary<string, Var>();
        public readonly EvalSettings EvalSettings;
        public readonly FormatSettings FormatSettings;
        public readonly List<PlotCall> PlotCalls = new List<PlotCall>();
        public readonly int Depth;

        public void DefConst(string name, Val val, string desc) {
            _vars.Add(name, new Var(new Token(TokenType.Identifier, DeprecatedTextPosition.Nowhere, name), val, true, desc));
        }

        public EvalContext() {
            EvalSettings = new EvalSettings();
            FormatSettings = new FormatSettings();
            Depth = 0;
            foreach(var constVar in BuiltInConstants.EnumConstants()) {
                _vars.Add(constVar.Name.Text, constVar);
            }
        }

        public EvalContext(EvalContext src) {
            Depth = src.Depth + 1;
            if (Depth >= Settings.Instance.Calculation_Limit_MaxCallRecursions) {
                throw new EvalError(this, null, "Depth of recursion exceeds limit.");
            }
            foreach (var k in src._vars.Keys) {
                _vars.Add(k, (Var)src._vars[k].Clone());
            }
            EvalSettings = (EvalSettings)src.EvalSettings.Clone();
            FormatSettings = (FormatSettings)src.FormatSettings.Clone();
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
            return Ref(new Token(TokenType.Identifier, DeprecatedTextPosition.Nowhere, name), allowCreate);
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

        public IEnumerable<FuncDef> AllNamedFuncs() =>
            BuiltInFuncLibrary.Instance.NativeFunctions.Select(p => (FuncDef)p).Concat(ExternalFuncDef.ExternalFunctions);

        public bool SolveFunc(string name , out FuncDef func) {
            func = AllNamedFuncs().FirstOrDefault(p => p.Name != null && p.Name.Text == name);
            return func != null;
        }

        public bool SolveFunc(string name, Val[] args, out FuncDef func, out string message) {
            var matches = new List<FuncMatch>();
            FuncMatchLevel level = FuncMatchLevel.NameUnmatched;
            scanFunc(matches, ref level, AllNamedFuncs(), name, args);
            if (_vars.TryGetValue(name, out Var v) && v.Value is FuncVal funcVal) {
                matchFunc(matches, ref level, (FuncDef)funcVal.Raw, "", args);
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
