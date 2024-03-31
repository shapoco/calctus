using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    static class StringFuncs {
        public static readonly BuiltInFuncDef trim = new BuiltInFuncDef(
            "trim(*s)", (e, a) => new StrVal(a[0].AsString.Trim()),
            "Removes whitespace characters from both ends of string `s`.");

        public static readonly BuiltInFuncDef trimStart = new BuiltInFuncDef(
            "trimStart(*s)", (e, a) => new StrVal(a[0].AsString.TrimStart()),
            "Removes whitespace characters from near end of string `s`.");

        public static readonly BuiltInFuncDef trimEnd = new BuiltInFuncDef(
            "trimEnd(*s)", (e, a) => new StrVal(a[0].AsString.TrimEnd()),
            "Removes whitespace characters from far end of string `s`.");

        public static readonly BuiltInFuncDef replace = new BuiltInFuncDef(
            "replace(*s,old,new)", (e, a) => new StrVal(a[0].AsString.Replace(a[1].AsString, a[2].AsString)),
            "Replaces the string `old` in string `s` with the string `new`.");

        public static readonly BuiltInFuncDef toLower = new BuiltInFuncDef(
            "toLower(*s)", (e, a) => new StrVal(a[0].AsString.ToLower()),
            "Converts alphabetic characters in string `s` to lowercase.");

        public static readonly BuiltInFuncDef toUpper = new BuiltInFuncDef(
            "toUpper(*s)", (e, a) => new StrVal(a[0].AsString.ToUpper()),
            "Converts alphabetic characters in string `s` to uppercase.");

        public static readonly BuiltInFuncDef startsWith = new BuiltInFuncDef(
            "startsWith(*s,key)", (e, a) => BoolVal.FromBool(a[0].AsString.StartsWith(a[1].AsString)),
            "Whether string `s` starts with string `key` or not");

        public static readonly BuiltInFuncDef endsWith = new BuiltInFuncDef(
            "endsWith(*s,key)", (e, a) => BoolVal.FromBool(a[0].AsString.EndsWith(a[1].AsString)),
            "Whether string `s` ends with string `key` or not");

        public static readonly BuiltInFuncDef split = new BuiltInFuncDef(
            "split(sep,s)", (e, a) => {
                var sep = a[0].AsString;
                var s = a[1].AsString;
                var array = s.Split(new string[] { sep }, StringSplitOptions.None); ;
                return new ArrayVal(array.Select(p => new StrVal(p)).ToArray());
            }, "Splits string `s` using string `sep` as delimiter.");

        public static readonly BuiltInFuncDef join = new BuiltInFuncDef(
            "join(sep,array[]...)", (e, a) => {
                var sep = a[0].AsString;
                var array = ((Val[])((ArrayVal)a[1]).Raw).Select(p => p.AsString).ToArray();
                return new StrVal(String.Join(sep, array));
            }, "Concatenates all elements of `array` using string `sep` as delimiter.");
    }
}
