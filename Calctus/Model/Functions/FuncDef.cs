using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Standards;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Formats;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Functions {
    abstract class FuncDef {
        public static readonly Regex PrototypePattern = new Regex(@"^(?<name>\w+)\((?<args>\*?\w+@?(, *\*?\w+@?)*((\[\])?\.\.\.)?)?\)$");

        public readonly Token Name;
        public readonly ArgDefList Args;
        public readonly string Description;

        public FuncDef(Token name, ArgDefList args, string desc) {
            this.Name = name;
            this.Args = args;
            this.Description = desc;
        }

        public FuncDef(string prototype, string desc) {
            var m = PrototypePattern.Match(prototype);
            if (!m.Success) throw new CalctusError("Invalid function prototype: \"" + prototype + "\"");
            var args = new List<ArgDef>();
            VariadicMode mode = VariadicMode.None;
            var vecArgIndex = -1;
            var fmtSrcArgIndex = -1;
            if (m.Groups["args"].Success) {
                var argsStr = m.Groups["args"].Value;
                if (argsStr.EndsWith("[]...")) {
                    mode = VariadicMode.Array;
                    argsStr = argsStr.Substring(0, argsStr.Length - 5);
                }
                else if (argsStr.EndsWith("...")) {
                    mode = VariadicMode.Flatten;
                    argsStr = argsStr.Substring(0, argsStr.Length - 3);
                }
                var caps = argsStr.Split(',').Select(p => p.Trim()).ToArray();
                for (int i = 0; i < caps.Length; i++) {
                    var argName = caps[i];
                    if (argName.StartsWith("*")) {
                        if (vecArgIndex >= 0) throw new CalctusError("Only one argument is vectorizable.");
                        if (mode != VariadicMode.None) throw new CalctusError("Variadic argument and vectorizable argument cannot coexist.");
                        vecArgIndex = i;
                        argName = argName.Substring(1);
                    }
                    if (argName.EndsWith("@")) {
                        if (fmtSrcArgIndex >= 0) throw new CalctusError("Only one argument is marked as format source.");
                        fmtSrcArgIndex = i;
                        argName = argName.Substring(0, argName.Length - 1);
                    }
                    argName = argName.Trim();
                    args.Add(new ArgDef(argName));
                }
            }
            this.Name = new Token(TokenType.Identifier, DeprecatedTextPosition.Nowhere, m.Groups["name"].Value);
            this.Args = new ArgDefList(args.ToArray(), mode, vecArgIndex, fmtSrcArgIndex);
            this.Description = desc;
        }

        public FuncMatch Match(string name, Val[] args) {
            if (string.IsNullOrEmpty(name) || (!Token.IsNullOrEmpty(Name) && name == this.Name.Text)) {
                if (Args.Mode != VariadicMode.None) {
                    if (args.Length >= this.Args.Items.Length - 1) {
                        return new FuncMatch(this, FuncMatchLevel.Matched);
                    }
                    else {
                        return new FuncMatch(this, FuncMatchLevel.ArgumentsUnmatched, "Least " + this.Args.Count + " args are required.");
                    }
                }
                else {
                    if (args.Length == this.Args.Count) {
                        return new FuncMatch(this, FuncMatchLevel.Matched);
                    }
                    else {
                        return new FuncMatch(this, FuncMatchLevel.ArgumentsUnmatched, this.Args.Count + " args are expected.");
                    }
                }
            }
            else {
                return new FuncMatch(this, FuncMatchLevel.NameUnmatched, "Name unmatched.");
            }
        }

        public Val Call(EvalContext e, params Val[] args) {
            if (Args.Mode == VariadicMode.Flatten) {
                // 可変長引数はフラットに展開する
                if (args.Length < Args.Count - 1) {
                    throw new CalctusError("Too few arguments");
                }
                else if (Args.Count == args.Length && args[args.Length - 1] is ListVal extVals) {
                    // 可変長引数部分が1個の配列の場合はフラットに展開する
                    var tempArgs = new Val[this.Args.Count - 1 + extVals.Length];
                    Array.Copy(args, tempArgs, args.Length - 1);
                    Array.Copy((Val[])extVals.Raw, 0, tempArgs, this.Args.Count - 1, extVals.Length);
                    return OnCallWrap(e, tempArgs);
                }
                else {
                    // 上記以外はそのまま
                    return OnCallWrap(e, args);
                }
            }
            else if (Args.Mode == VariadicMode.Array) {
                // 可変長引数は配列にまとめる
                if (args.Length < Args.Count - 1) {
                    throw new CalctusError("Too few arguments");
                }
                else if (args.Length == Args.Count - 1) {
                    // 可変長配列部分が空配列
                    var tempArgs = new Val[this.Args.Count];
                    Array.Copy(args, tempArgs, Args.Count - 1);
                    tempArgs[this.Args.Count - 1] = new ListVal(new Val[0]);
                    return OnCallWrap(e, tempArgs);
                }
                else if (Args.Count == args.Length && args[args.Length - 1] is ListVal) {
                    // 配列で渡された場合はそのまま
                    return OnCallWrap(e, args);
                }
                else {
                    // 可変長配列部分を配列にまとめる
                    var tempArgs = new Val[this.Args.Count];
                    Array.Copy(args, tempArgs, Args.Count - 1);
                    var array = new Val[args.Length - Args.Count + 1];
                    Array.Copy(args, Args.Count - 1, array, 0, array.Length);
                    tempArgs[Args.Count - 1] = new ListVal(array);
                    return OnCallWrap(e, tempArgs);
                }
            }
            else if (Args.VectorizableArgIndex >= 0 && args[Args.VectorizableArgIndex] is ListVal vecVals) {
                // ベクトル化
                var tempArgs = new Val[args.Length];
                Array.Copy(args, tempArgs, args.Length);
                var results = new Val[vecVals.Length];
                for (int i = 0; i < vecVals.Length; i++) {
                    tempArgs[Args.VectorizableArgIndex] = vecVals[i];
                    results[i] = OnCallWrap(e, tempArgs);
                }
                return new ListVal(results);
            }
            else {
                return OnCallWrap(e, args);
            }
        }

        private Val OnCallWrap(EvalContext e, Val[] args) {
            var retVal = OnCall(e, args);
            if (Args.FormatSourceArgIndex >= 0) {
                retVal = retVal.Format(args[Args.FormatSourceArgIndex].FormatHint);
            }
            return retVal;
        }

        protected abstract Val OnCall(EvalContext e, Val[] args);

        public override string ToString() => GetDeclarationText();

        public string GetDeclarationText() {
            var sb = new StringBuilder();
            if (Token.IsNullOrEmpty(Name)) {
                sb.Append("<unnamed>");
            }
            else {
                sb.Append(Name.Text);
            }
            GetArgListString(sb);
            return sb.ToString();
        }

        public string GetArgListString() {
            var sb = new StringBuilder();
            GetArgListString(sb);
            return sb.ToString(); ;
        }

        public void GetArgListString(StringBuilder sb) {
            sb.Append('(');
            for (int i = 0; i < Args.Count; i++) {
                if (i > 0) sb.Append(", ");
                if (i == Args.VectorizableArgIndex) sb.Append('*');
                sb.Append(Args[i].ToString());
            }
            switch (Args.Mode) {
                case VariadicMode.Flatten: sb.Append("..."); break;
                case VariadicMode.Array: sb.Append("[]..."); break;
            }
            sb.Append(')');
        }

        /// <summary>
        /// 関数本体の実装をシンプルにするために引数を Val と real の間で変換する。
        /// </summary>
        public static Func<EvalContext, Val[], Val> ArgToDecimal(Func<EvalContext, decimal[], decimal> func, FormatHint fmt = null) {
            fmt = fmt.OrDefault();
            return (e, a) => func(e, a.Select(p => p.AsDecimal).ToArray()).ToVal(fmt);
        }

        /// <summary>
        /// 関数本体の実装をシンプルにするために引数を Val と long の間で変換する。
        /// </summary>
        public static Func<EvalContext, Val[], Val> ArgToLong(Func<EvalContext, long[], long> func, FormatHint fmt = null) {
            fmt = fmt.OrDefault();
            return (e, a) => func(e, a.Select(p => p.AsLong).ToArray()).ToRealVal(fmt);
        }
    }
}
