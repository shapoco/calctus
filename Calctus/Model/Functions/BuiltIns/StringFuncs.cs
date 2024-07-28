using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class StringFuncs : BuiltInFuncCategory {
        private static StringFuncs _instance = null;
        public static StringFuncs Instance => _instance != null ? _instance : _instance = new StringFuncs();
        private StringFuncs() { }

        public readonly BuiltInFuncDef trim = new BuiltInFuncDef("trim(*s@)",
            "Removes whitespace characters from both ends of string `s`.",
            (e, a) => a[0].AsString.Trim().ToStrVal());

        public readonly BuiltInFuncDef trimStart = new BuiltInFuncDef("trimStart(*s@)",
            "Removes whitespace characters from near end of string `s`.",
            (e, a) => a[0].AsString.TrimStart().ToStrVal());

        public readonly BuiltInFuncDef trimEnd = new BuiltInFuncDef("trimEnd(*s@)",
            "Removes whitespace characters from far end of string `s`.",
            (e, a) => a[0].AsString.TrimEnd().ToStrVal());

        public readonly BuiltInFuncDef replace = new BuiltInFuncDef("replace(*s,old,new)",
            "Replaces the string `old` in string `s` with the string `new`.",
            (e, a) => a[0].AsString.Replace(a[1].AsString, a[2].AsString).ToStrVal());

        public readonly BuiltInFuncDef toLower = new BuiltInFuncDef("toLower(*s)",
            "Converts alphabetic characters in string `s` to lowercase.",
            (e, a) => a[0].AsString.ToLower().ToStrVal());

        public readonly BuiltInFuncDef toUpper = new BuiltInFuncDef("toUpper(*s)",
            "Converts alphabetic characters in string `s` to uppercase.",
            (e, a) => a[0].AsString.ToUpper().ToStrVal());

        public readonly BuiltInFuncDef startsWith = new BuiltInFuncDef("startsWith(*s,key)",
            "Whether string `s` starts with string `key` or not",
            (e, a) => BoolVal.FromBool(a[0].AsString.StartsWith(a[1].AsString)));

        public readonly BuiltInFuncDef endsWith = new BuiltInFuncDef("endsWith(*s,key)",
            "Whether string `s` ends with string `key` or not",
            (e, a) => BoolVal.FromBool(a[0].AsString.EndsWith(a[1].AsString)));


        public readonly BuiltInFuncDef split = new BuiltInFuncDef("split(sep,s)",
            "Splits string `s` using string `sep` as delimiter.",
            (e, a) => {
                var sep = a[0].AsString;
                var s = a[1].AsString;
                var array = s.Split(new string[] { sep }, StringSplitOptions.None);
                return array.ToArrayVal(a[0].FormatHint, null);
            });

        public readonly BuiltInFuncDef join = new BuiltInFuncDef("join(sep,array[]...)",
            "Concatenates all elements of `array` using string `sep` as delimiter.",
            (e, a) => {
                var sep = a[0].AsString;
                var array = ((Val[])((ArrayVal)a[1]).Raw).Select(p => p.AsString).ToArray();
                return String.Join(sep, array).ToStrVal(a[0].FormatHint);
            });
    }
}
