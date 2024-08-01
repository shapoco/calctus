using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class StringFuncs : BuiltInFuncCategory {
        private static StringFuncs _instance = null;
        public static StringFuncs Instance => _instance != null ? _instance : _instance = new StringFuncs();
        private StringFuncs() { }

        public readonly BuiltInFuncDef trim = new BuiltInFuncDef("trim(*s@)",
            "Removes whitespace characters from both ends of string `s`.",
            (e, a) => a[0].ToStringForValue(e).Trim().ToVal());

        public readonly BuiltInFuncDef trimStart = new BuiltInFuncDef("trimStart(*s@)",
            "Removes whitespace characters from near end of string `s`.",
            (e, a) => a[0].ToStringForValue(e).TrimStart().ToVal());

        public readonly BuiltInFuncDef trimEnd = new BuiltInFuncDef("trimEnd(*s@)",
            "Removes whitespace characters from far end of string `s`.",
            (e, a) => a[0].ToStringForValue(e).TrimEnd().ToVal());

        public readonly BuiltInFuncDef replace = new BuiltInFuncDef("replace(*s,old,new)",
            "Replaces the string `old` in string `s` with the string `new`.",
            (e, a) => a[0].ToStringForValue(e).Replace(a[1].ToStringForValue(e), a[2].ToStringForValue(e)).ToVal());

        public readonly BuiltInFuncDef toLower = new BuiltInFuncDef("toLower(*s)",
            "Converts alphabetic characters in string `s` to lowercase.",
            (e, a) => a[0].ToStringForValue(e).ToLower().ToVal());

        public readonly BuiltInFuncDef toUpper = new BuiltInFuncDef("toUpper(*s)",
            "Converts alphabetic characters in string `s` to uppercase.",
            (e, a) => a[0].ToStringForValue(e).ToUpper().ToVal());

        public readonly BuiltInFuncDef startsWith = new BuiltInFuncDef("startsWith(*s,key)",
            "Whether string `s` starts with string `key` or not",
            (e, a) => BoolVal.From(a[0].ToStringForValue(e).StartsWith(a[1].ToStringForValue(e))));

        public readonly BuiltInFuncDef endsWith = new BuiltInFuncDef("endsWith(*s,key)",
            "Whether string `s` ends with string `key` or not",
            (e, a) => BoolVal.From(a[0].ToStringForValue(e).EndsWith(a[1].ToStringForValue(e))));


        public readonly BuiltInFuncDef split = new BuiltInFuncDef("split(sep,s)",
            "Splits string `s` using string `sep` as delimiter.",
            (e, a) => {
                var sep = a[0].ToStringForValue(e);
                var s = a[1].ToStringForValue(e);
                var strArray = s.Split(new string[] { sep }, StringSplitOptions.None);
                return strArray.ToVal();
            });

        public readonly BuiltInFuncDef join = new BuiltInFuncDef("join(sep,array[]...)",
            "Concatenates all elements of `array` using string `sep` as delimiter.",
            (e, a) => {
                var sep = a[0].ToStringForValue(e);
                var array = ((Val[])((ListVal)a[1]).Raw).Select(p => p.ToStringForValue(e)).ToArray();
                return String.Join(sep, array).ToVal();
            });
    }
}
