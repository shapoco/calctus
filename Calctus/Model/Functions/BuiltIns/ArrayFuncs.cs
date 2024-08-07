using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Maths;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Expressions;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class ArrayFuncs : BuiltInFuncCategory {
        private static ArrayFuncs _instance = null;
        public static ArrayFuncs Instance => _instance != null ? _instance : _instance = new ArrayFuncs();
        private ArrayFuncs() { }

        public readonly BuiltInFuncDef len = new BuiltInFuncDef("len(array)",
            "Length of `array`",
            (e, a) => a[0].ToCollectionVal().Length.ToRealVal(),
            new FuncTest("[]", "0"),
            new FuncTest("[12,34,56,78]", "4"),
            new FuncTest("\"Hello\"", "5"));

        public readonly BuiltInFuncDef range_2 = new BuiltInFuncDef("range(start, stop)",
            "Returns an array consists of sequence of numbers greater than or equal to `start` and less than `stop`.",
            (e, a) => MathEx.Range(a[0].AsDecimal, a[1].AsDecimal, 0, false).ToVal(a[0].FormatHint),
            new FuncTest("3,9", "[3,4,5,6,7,8]"));

        public readonly BuiltInFuncDef range_3 = new BuiltInFuncDef("range(start, stop, step)",
            "Returns an array consists of sequence of numbers greater than or equal to `start` and less than `stop` with common difference `step`.",
            (e, a) => MathEx.Range(a[0].AsDecimal, a[1].AsDecimal, a[2].AsDecimal, false).ToVal(a[0].FormatHint),
            new FuncTest("3,9,2", "[3,5,7]"));

        public readonly BuiltInFuncDef rangeInclusive_2 = new BuiltInFuncDef("rangeInclusive(start, stop)",
            "Returns an array consists of sequence of numbers greater than or equal to `start` and less than or equal to `stop`.",
            (e, a) => MathEx.Range(a[0].AsDecimal, a[1].AsDecimal, 0, true).ToVal(a[0].FormatHint),
            new FuncTest("3,9", "[3,4,5,6,7,8,9]"));

        public readonly BuiltInFuncDef rangeInclusive_3 = new BuiltInFuncDef("rangeInclusive(start, stop, step)",
            "Returns an array consists of sequence of numbers greater than or equal to `start` and less than or equal to `stop` with common difference `step`.",
            (e, a) => MathEx.Range(a[0].AsDecimal, a[1].AsDecimal, a[2].AsDecimal, true).ToVal(a[0].FormatHint),
            new FuncTest("3,9,2", "[3,5,7,9]"));

        public readonly BuiltInFuncDef arrayComp = new BuiltInFuncDef("arrayComp(array0, array1)",
            "Returns true if all elements of the two arrays are the same.",
            (e, a) => {
                var array0 = a[0].ToCollectionVal().ToValArray();
                var array1 = a[1].ToCollectionVal().ToValArray();
                if (array0.Length != array1.Length) return BoolVal.False;
                for (int i =0;i < array0.Length; i++) {
                    var elmA = array0[i];
                    var elmB = array1[i];
                    BinaryOpExpr.TryAutoCast(e, ref elmA, ref elmB);
                    if (!elmA.Equals(e, elmB)) return BoolVal.False;
                }
                return BoolVal.True;
            },
            new FuncTest("[1,2,3], [1,2,3]", "true"),
            new FuncTest("[1,2,3], [1,2,3,4]", "false"),
            new FuncTest("[1,2,3], [1,2,10]", "false"),
            new FuncTest("[], []", "true"),
            new FuncTest("[], [1,2,3]", "false"));

        public readonly BuiltInFuncDef reverseArray = new BuiltInFuncDef("reverseArray(array@)",
            "Reverses the order of elements of `array`",
            (e, a) => {
                var array = a[0].ToCollectionVal().ToValArray();
                Array.Reverse(array);
                return new ListVal(array);
            },
            new FuncTest("[12,3,4567,890]", "[890,4567,3,12]"));

        public readonly BuiltInFuncDef map = new BuiltInFuncDef("map(array,func)",
            "Map the `array` using a converter function `func(x)`.",
            (e, a) => {
                var array = a[0].ToCollectionVal().ToValArray();
                var func = (FuncDef)a[1].Raw;
                return new ListVal(array.Select(p => func.Call(e, p)).ToArray());
            },
            new FuncTest("0..5,x=>x*x", "[0,1,4,9,16]"));

        public readonly BuiltInFuncDef filter = new BuiltInFuncDef("filter(array,func)",
            "Filter the `array` using a tester function `func(x)`.",
            (e, a) => {
                var array = a[0].ToCollectionVal().ToValArray();
                var func = (FuncDef)a[1].Raw;
                return new ListVal(array.Where(p => func.Call(e, p).ToBool()).ToArray());
            },
            new FuncTest("0..10,x=>x%3==0", "[0,3,6,9]"));

        public readonly BuiltInFuncDef count = new BuiltInFuncDef("count(array,func)",
            "Count specific elements in the `array` using a tester function `func(x)`.",
            (e, a) => {
                var array = a[0].ToCollectionVal().ToValArray();
                var func = (FuncDef)a[1].Raw;
                return new RealVal(array.Count(p => func.Call(e, p).ToBool()));
            },
            new FuncTest("0..10,x=>x%3==0", "4"));

        public readonly BuiltInFuncDef sort_1 = new BuiltInFuncDef("sort(array@)",
            "Sort the `array`.",
            (e, a) => {
                var array = a[0].ToCollectionVal().ToValArray();
                return new ListVal(array.OrderBy(p => p, new ValComparer(e)).ToArray());
            },
            new FuncTest("[4,3,5,1,2]", "[1,2,3,4,5]"));

        public readonly BuiltInFuncDef sort_2 = new BuiltInFuncDef("sort(array@,func)",
            "Sort the `array` using a evaluator function `func(x)`.",
            (e, a) => {
                var array = a[0].ToCollectionVal().ToValArray();
                var func = (FuncDef)a[1].Raw;
                return new ListVal(array.OrderBy(p => func.Call(e, p).AsDecimal).ToArray());
            },
            new FuncTest("[14,23,35,41,52],x=>x%10", "[41,52,23,14,35]"));

        public readonly BuiltInFuncDef extend = new BuiltInFuncDef("extend(array,func,count)",
            "Extends the `array` using converter function `func(array)`.",
            (e, a) => {
                var list = a[0].ToCollectionVal().ToValArray().ToList();
                var func = (FuncDef)a[1].Raw;
                var countDecimal = a[2].AsDecimal;
                if (!countDecimal.IsInteger() || countDecimal <= 0) throw new ArgumentOutOfRangeException();
                int count = (int)countDecimal;
                ListVal.CheckArrayLength(list.Count + count);
                for (int i = 0; i < count; i++) {
                    list.Add(func.Call(e, new ListVal(list.ToArray())));
                }
                return new ListVal(list.ToArray());
            },
            new FuncTest("[0,1],a=>a[-2]+a[-1],5", "[0,1,1,2,3,5,8]"));

        public readonly BuiltInFuncDef aggregate = new BuiltInFuncDef("aggregate(array,func)",
            "Apply the aggregate function `func(a,b)` for the `array`.",
            (e, a) => {
                var array = a[0].ToCollectionVal().ToValArray();
                var func = (FuncDef)a[1].Raw;
                return array.Aggregate((p, q) => func.Call(e, p, q));
            },
            new FuncTest("[1,2,3,4,5],(a,b)=>a+b", "1+2+3+4+5"));

        public readonly BuiltInFuncDef all_1 = new BuiltInFuncDef("all(array)",
            "Returns true if all `array` elements are true.",
            (e, a) => {
                var array = a[0].ToCollectionVal().ToValArray();
                return BoolVal.From(array.All(p => p.ToBool()));
            },
            new FuncTest("[true,false,true]", "false"),
            new FuncTest("[true,true,true]", "true"));

        public readonly BuiltInFuncDef all_2 = new BuiltInFuncDef("all(array,func)",
            "Returns true if tester function `func(x)` returns true for all elements of the `array`.",
            (e, a) => {
                var array = a[0].ToCollectionVal().ToValArray();
                var func = (FuncDef)a[1].Raw;
                return BoolVal.From(array.All(p => func.Call(e, p).ToBool()));
            },
            new FuncTest("[2,3,5,7],isPrime", "true"),
            new FuncTest("[2,3,5,7,9],isPrime", "false"));

        public readonly BuiltInFuncDef any_1 = new BuiltInFuncDef("any(array)",
            "Returns true if at least one element is true.",
            (e, a) => {
                var array = a[0].ToCollectionVal().ToValArray();
                return BoolVal.From(array.Any(p => p.ToBool()));
            },
            new FuncTest("[false,false,false]", "false"),
            new FuncTest("[false,true,false]", "true"));

        public readonly BuiltInFuncDef any_2 = new BuiltInFuncDef("any(array,func)",
            "Returns true if tester function `func(x)` returns true for at least one element of the `array`.",
            (e, a) => {
                var array = a[0].ToCollectionVal().ToValArray();
                var func = (FuncDef)a[1].Raw;
                return BoolVal.From(array.Any(p => func.Call(e, p).ToBool()));
            },
            new FuncTest("[1,2,3,4,5],x=>x%2==0", "true"),
            new FuncTest("[1,3,5,7,9],x=>x%2==0", "false"));

        public readonly BuiltInFuncDef concat = new BuiltInFuncDef("concat(array0@,array1)",
            "Concatenate array0 and array1.",
            (e, a) => {
                var array0 = a[0].ToCollectionVal().ToValArray();
                var array1 = a[1].ToCollectionVal().ToValArray();
                return new ListVal(array0.Concat(array1).ToArray());
            },
            new FuncTest("[1,2,3],[4,5,6]", "[1,2,3,4,5,6]"));

        public readonly BuiltInFuncDef unique_1 = new BuiltInFuncDef("unique(array@)",
            "Returns an array of unique elements.",
            (e, a) => {
                var array = a[0].ToCollectionVal().ToValArray();
                return new ListVal(array.Distinct(new ValEqualityComparer(e)).ToArray());
            },
            new FuncTest("[3,0,1,2,2,1,3,4]", "[3,0,1,2,4]"));

        public readonly BuiltInFuncDef unique_2 = new BuiltInFuncDef("unique(array@,func)",
            "Return unique elements using evaluator function `func(x)`.",
            (e, a) => {
                var array = a[0].ToCollectionVal().ToValArray();
                var func = (FuncDef)a[1].Raw;
                return new ListVal(array.Distinct(new EqualityComparerFunc(e, func)).ToArray());
            },
            new FuncTest("[13,20,31,42,52,61,73,84],(a,b)=>a%10==b%10", "[13,20,31,42,84]"));

        public readonly BuiltInFuncDef except = new BuiltInFuncDef("except(array0@,array1)",
            "Returns the difference set of the two arrays.",
            (e, a) => {
                var array0 = a[0].ToCollectionVal().ToValArray();
                var array1 = a[1].ToCollectionVal().ToValArray();
                return new ListVal(array0.Except(array1, new ValEqualityComparer(e)).ToArray());
            },
            new FuncTest("0..10,[1,4,7,10,13,16]", "[0,2,3,5,6,8,9]"));

        public readonly BuiltInFuncDef intersect = new BuiltInFuncDef("intersect(array0@,array1)",
            "Returns the product set of the two arrays.",
            (e, a) => {
                var array0 = a[0].ToCollectionVal().ToValArray();
                var array1 = a[1].ToCollectionVal().ToValArray();
                return new ListVal(array0.Intersect(array1, new ValEqualityComparer(e)).ToArray());
            },
            new FuncTest("0..10,[1,4,7,10,13,16]", "[1,4,7]"));

        public readonly BuiltInFuncDef union = new BuiltInFuncDef("union(array0@,array1)",
            "Returns the union of the two arrays.",
            (e, a) => {
                var array0 = a[0].ToCollectionVal().ToValArray();
                var array1 = a[1].ToCollectionVal().ToValArray();
                return new ListVal(array0.Union(array1, new ValEqualityComparer(e)).ToArray());
            },
            new FuncTest("0..10,[1,4,7,10,13,16]", "[0,1,2,3,4,5,6,7,8,9,10,13,16]"));

        public readonly BuiltInFuncDef indexOf = new BuiltInFuncDef("indexOf(array,*val)",
            "Returns the index of the first element in the `array` whose value matches `val`.",
            (e, a) => indexOfCore(e, a[0], a[1]).ToRealVal(),
            new FuncTest("[8,1,4,0,7,2,5,4,3],4", "2"),
            new FuncTest("[8,1,4,0,7,2,5,4,3],6", "-1"));

        public readonly BuiltInFuncDef lastIndexOf = new BuiltInFuncDef("lastIndexOf(array,*val)",
            "Returns the index of the last element in the `array` whose value matches `val`.",
            (e, a) => lastIndexOfCore(e, a[0], a[1]).ToRealVal(),
            new FuncTest("[8,1,4,0,7,2,5,4,3],4", "7"),
            new FuncTest("[8,1,4,0,7,2,5,4,3],6", "-1"));

        public readonly BuiltInFuncDef contains = new BuiltInFuncDef("contains(array,*val)",
            "Returns whether the `array` contains `val`.",
            (e, a) => BoolVal.From(indexOfCore(e, a[0], a[1]) >= 0),
            new FuncTest("[8,1,4,0,7,2,5,4,3],4", "true"),
            new FuncTest("[8,1,4,0,7,2,5,4,3],6", "false"));

        private static int indexOfCore(EvalContext e, Val arrayVal, Val keyVal) {
            if (arrayVal is StrVal sVal0 && keyVal is StrVal sVal1) {
                return sVal0.Raw.IndexOf(sVal1.Raw);
            }
            else {
                var array = arrayVal.ToCollectionVal().ToValArray();
                if (keyVal is FuncVal keyFuncVal) {
                    var func = (FuncDef)keyFuncVal.Raw;
                    for (int i = 0; i < array.Length; i++) {
                        if (func.Call(e, array[i]).ToBool()) return i;
                    }
                }
                else {
                    for (int i = 0; i < array.Length; i++) {
                        if (array[i].Equals(e, keyVal)) return i;
                    }
                }
                return -1;
            }
        }

        private static int lastIndexOfCore(EvalContext e, Val arrayVal, Val keyVal) {
            if (arrayVal is StrVal sVal0 && keyVal is StrVal sVal1) {
                return sVal0.Raw.LastIndexOf(sVal1.Raw);
            }
            else {
                var array = arrayVal.ToCollectionVal().ToValArray();
                if (keyVal is FuncVal keyFuncValVal) {
                    var func = (FuncDef)keyFuncValVal.Raw;
                    for (int i = array.Length - 1; i >= 0; i--) {
                        if (func.Call(e, array[i]).ToBool()) return i;
                    }
                }
                else {
                    for (int i = array.Length - 1; i >= 0; i--) {
                        if (array[i].Equals(e, keyVal)) return i;
                    }
                }
                return -1;
            }
        }
    }
}
