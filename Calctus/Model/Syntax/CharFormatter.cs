using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Syntax {
    class CharFormatter : NumberFormatter {
        public CharFormatter() : base(new Regex("'([^'\\\\]|\\\\[abfnrtv\\\\\'0]|\\\\o[0-7]{3}|\\\\x[0-9a-fA-F]{2}|\\\\u[0-9a-fA-F]{4})'")) { }

        public override Val Parse(Match m) {
            var tok = m.Groups[1].Value;
            char c;
            switch (tok) {
                case "\\a": c = '\a'; break;
                case "\\b": c = '\b'; break;
                case "\\f": c = '\f'; break;
                case "\\n": c = '\n'; break;
                case "\\r": c = '\r'; break;
                case "\\t": c = '\t'; break;
                case "\\v": c = '\v'; break;
                case "\\\\": c = '\\'; break;
                case "\\'": c = '\''; break;
                case "\\0": c = '\0'; break;
                default:
                    if (tok.StartsWith("\\o")) {
                        var code = Convert.ToUInt64(tok.Substring(2), 8);
                        if (code < char.MinValue || char.MaxValue < code) {
                            throw new CalctusError("Char code out of range.");
                        }
                        c = (char)code;
                    }
                    else if (tok.StartsWith("\\x") || tok.StartsWith("\\u")) {
                        var code = Convert.ToUInt64(tok.Substring(2), 16);
                        if (code < char.MinValue || char.MaxValue < code) {
                            throw new CalctusError("Char code out of range.");
                        }
                        c = (char)code;
                    }
                    else {
                        c = tok[0];
                    }
                    break;
            }
            return new RealVal(c, new ValFormatHint(this));
        }

        protected override string OnFormat(Val val, EvalContext e) {
            if (!val.IsInteger) {
                // 整数でない場合はデフォルトの数値表現を使用
                return base.OnFormat(val, e);
            }

            var ival = val.AsReal;
            if (ival < char.MinValue || char.MaxValue < ival) {
                // 小数やcharの範囲外の値はデフォルトの数値表現を使用
                return base.OnFormat(val, e);
            }

            var cval = (char)ival;
            switch (cval) {
                case '\a': return "'\\a'";
                case '\b': return "'\\b'";
                case '\f': return "'\\f'";
                case '\n': return "'\\n'";
                case '\r': return "'\\r'";
                case '\t': return "'\\t'";
                case '\v': return "'\\v'";
                case '\\': return "'\\\\'";
                case '\'': return "'\\''";
                case '\0': return "'\\0'";
                default:
                    if (char.IsLetterOrDigit(cval) || char.IsPunctuation(cval) || char.IsSeparator(cval) || char.IsSymbol(cval)) {
                        return "'" + cval + "'";
                    }
                    else {
                        var hex = "0000" + Convert.ToString(cval, 16);
                        return "'\\u" + hex.Substring(hex.Length - 4, 4) + "'";
                    }
            }
        }
    }
}
