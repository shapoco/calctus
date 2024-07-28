using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class ArrayFuncs : BuiltInFuncCategory {
        private static ArrayFuncs _instance = null;
        public static ArrayFuncs Instance => _instance != null ? _instance : _instance = new ArrayFuncs();
        private ArrayFuncs() { }

        public readonly BuiltInFuncDef len = new BuiltInFuncDef("len(array)",
            "Length of `array`",
            (e, a) => a[0].AsArrayVal().Length.ToRealVal(),
            new FuncTest("[]", "0"),
            new FuncTest("[12,34,56,78]", "4"),
            new FuncTest("\"Hello\"", "5"));

        public readonly BuiltInFuncDef range_2 = new BuiltInFuncDef("range(start, stop)",
            "Returns an array consists of sequence of numbers greater than or equal to `start` and less than `stop`.",
            (e, a) => DMath.Range(a[0].AsDecimal, a[1].AsDecimal, 0, false).ToArrayVal(a[0].FormatHint, null),
            new FuncTest("3,9", "[3,4,5,6,7,8]"));

        public readonly BuiltInFuncDef range_3 = new BuiltInFuncDef("range(start, stop, step)",
            "Returns an array consists of sequence of numbers greater than or equal to `start` and less than `stop` with common difference `step`.",
            (e, a) => DMath.Range(a[0].AsDecimal, a[1].AsDecimal, a[2].AsDecimal, false).ToArrayVal(a[0].FormatHint, null),
            new FuncTest("3,9,2", "[3,5,7]"));

        public readonly BuiltInFuncDef rangeInclusive_2 = new BuiltInFuncDef("rangeInclusive(start, stop)",
            "Returns an array consists of sequence of numbers greater than or equal to `start` and less than or equal to `stop`.",
            (e, a) => DMath.Range(a[0].AsDecimal, a[1].AsDecimal, 0, true).ToArrayVal(a[0].FormatHint, null),
            new FuncTest("3,9", "[3,4,5,6,7,8,9]"));

        public readonly BuiltInFuncDef rangeInclusive_3 = new BuiltInFuncDef("rangeInclusive(start, stop, step)",
            "Returns an array consists of sequence of numbers greater than or equal to `start` and less than or equal to `stop` with common difference `step`.",
            (e, a) => DMath.Range(a[0].AsDecimal, a[1].AsDecimal, a[2].AsDecimal, true).ToArrayVal(a[0].FormatHint, null),
            new FuncTest("3,9,2", "[3,5,7,9]"));

        public readonly BuiltInFuncDef reverseArray = new BuiltInFuncDef("reverseArray(array@)",
            "Reverses the order of elements of `array`",
            (e, a) => {
                var array = (Val[])a[0].AsArrayVal().Raw;
                Array.Reverse(array);
                return new ArrayVal(array);
            },
            new FuncTest("[12,3,4567,890]", "[890,4567,3,12]"));

        public readonly BuiltInFuncDef map = new BuiltInFuncDef("map(array,func)",
            "Map the `array` using a converter function `func(x)`.",
            (e, a) => {
                var array = (Val[])a[0].AsArrayVal().Raw;
                var func = (FuncDef)a[1].Raw;
                return new ArrayVal(array.Select(p => func.Call(e, p)).ToArray());
            });

        public readonly BuiltInFuncDef filter = new BuiltInFuncDef("filter(array,func)",
            "Filter the `array` using a tester function `func(x)`.",
            (e, a) => {
                var array = (Val[])a[0].AsArrayVal().Raw;
                var func = (FuncDef)a[1].Raw;
                return new ArrayVal(array.Where(p => func.Call(e, p).AsBool).ToArray(), a[0].FormatHint);
            });

        public readonly BuiltInFuncDef count = new BuiltInFuncDef("count(array,func)",
            "Count specific elements in the `array` using a tester function `func(x)`.",
            (e, a) => {
                var array = (Val[])a[0].AsArrayVal().Raw;
                var func = (FuncDef)a[1].Raw;
                return new RealVal(array.Count(p => func.Call(e, p).AsBool));
            });

        public readonly BuiltInFuncDef sort_1 = new BuiltInFuncDef("sort(array@)",
            "Sort the `array`.",
            (e, a) => {
                var array = (Val[])a[0].AsArrayVal().Raw;
                return new ArrayVal(array.OrderBy(p => p, new ValComparer(e)).ToArray(), a[0].FormatHint);
            });

        public readonly BuiltInFuncDef sort_2 = new BuiltInFuncDef("sort(array@,func)",
            "Sort the `array` using a evaluator function `func(x)`.",
            (e, a) => {
                var array = (Val[])a[0].AsArrayVal().Raw;
                var func = (FuncDef)a[1].Raw;
                return new ArrayVal(array.OrderBy(p => func.Call(e, p).AsDecimal).ToArray(), a[0].FormatHint);
            });

        public readonly BuiltInFuncDef extend = new BuiltInFuncDef("extend(array,func,count)",
            "Extends the `array` using converter function `func(array)`.",
            (e, a) => {
                var seedArray = a[0].AsArrayVal();
                var list = ((Val[])seedArray.Raw).ToList();
                var func = (FuncDef)a[1].Raw;
                var countDecimal = a[2].AsDecimal;
                if (!countDecimal.IsInteger() || countDecimal <= 0) throw new ArgumentOutOfRangeException();
                int count = (int)countDecimal;
                ArrayVal.CheckArrayLength(list.Count + count);
                for (int i = 0; i < count; i++) {
                    list.Add(func.Call(e, new ArrayVal(list.ToArray())));
                }
                return new ArrayVal(list.ToArray(), seedArray.FormatHint);
            });

        public readonly BuiltInFuncDef aggregate = new BuiltInFuncDef("aggregate(array,func)",
            "Apply the aggregate function `func(a,b)` for the `array`.",
            (e, a) => {
                var array = (Val[])a[0].AsArrayVal().Raw;
                var func = (FuncDef)a[1].Raw;
                return array.Aggregate((p, q) => func.Call(e, p, q));
            },
            new FuncTest("[1,2,3,4,5],(a,b)=>a+b", "1+2+3+4+5"));

        public readonly BuiltInFuncDef all_1 = new BuiltInFuncDef("all(array)",
            "Returns true if all `array` elements are true.",
            (e, a) => {
                var array = (Val[])a[0].AsArrayVal().Raw;
                return BoolVal.FromBool(array.All(p => p.AsBool));
            },
            new FuncTest("[true,false,true]", "false"),
            new FuncTest("[true,true,true]", "true"));

        public readonly BuiltInFuncDef all_2 = new BuiltInFuncDef("all(array,func)",
            "Returns true if tester function `func(x)` returns true for all elements of the `array`.",
            (e, a) => {
                var array = (Val[])a[0].AsArrayVal().Raw;
                var func = (FuncDef)a[1].Raw;
                return BoolVal.FromBool(array.All(p => func.Call(e, p).AsBool));
            },
            new FuncTest("[2,3,5,7],isPrime", "true"),
            new FuncTest("[2,3,5,7,9],isPrime", "false"));

        public readonly BuiltInFuncDef any_1 = new BuiltInFuncDef("any(array)",
            "Returns true if at least one element is true.",
            (e, a) => {
                var array = (Val[])a[0].AsArrayVal().Raw;
                return BoolVal.FromBool(array.Any(p => p.AsBool));
            },
            new FuncTest("[false,false,false]", "false"),
            new FuncTest("[false,true,false]", "true"));

        public readonly BuiltInFuncDef any_2 = new BuiltInFuncDef("any(array,func)",
            "Returns true if tester function `func(x)` returns true for at least one element of the `array`.",
            (e, a) => {
                var array = (Val[])a[0].AsArrayVal().Raw;
                var func = (FuncDef)a[1].Raw;
                return BoolVal.FromBool(array.Any(p => func.Call(e, p).AsBool));
            });

        public readonly BuiltInFuncDef concat = new BuiltInFuncDef("concat(array0@,array1)",
            "Concatenate array0 and array1.",
            (e, a) => {
                var array0 = (Val[])a[0].AsArrayVal().Raw;
                var array1 = (Val[])a[1].AsArrayVal().Raw;
                return new ArrayVal(array0.Concat(array1).ToArray());
            });

        public readonly BuiltInFuncDef unique_1 = new BuiltInFuncDef("unique(array@)",
            "Returns an array of unique elements.",
            (e, a) => {
                var array = (Val[])a[0].AsArrayVal().Raw;
                return new ArrayVal(array.Distinct(new ValEqualityComparer(e)).ToArray());
            });

        public readonly BuiltInFuncDef unique_2 = new BuiltInFuncDef("unique(array@,func)",
            "Return unique elements using evaluator function `func(x)`.",
            (e, a) => {
                var array = (Val[])a[0].AsArrayVal().Raw;
                var func = (FuncDef)a[1].Raw;
                return new ArrayVal(array.Distinct(new EqualityComparerFunc(e, func)).ToArray());
            });

        public readonly BuiltInFuncDef except = new BuiltInFuncDef("except(array0@,array1)",
            "Returns the difference set of the two arrays.",
            (e, a) => {
                var array0 = (Val[])a[0].AsArrayVal().Raw;
                var array1 = (Val[])a[1].AsArrayVal().Raw;
                return new ArrayVal(array0.Except(array1, new ValEqualityComparer(e)).ToArray());
            });

        public readonly BuiltInFuncDef intersect = new BuiltInFuncDef("intersect(array0@,array1)",
            "Returns the product set of the two arrays.",
            (e, a) => {
                var array0 = (Val[])a[0].AsArrayVal().Raw;
                var array1 = (Val[])a[1].AsArrayVal().Raw;
                return new ArrayVal(array0.Intersect(array1, new ValEqualityComparer(e)).ToArray());
            });

        public readonly BuiltInFuncDef union = new BuiltInFuncDef("union(array0@,array1)",
            "Returns the union of the two arrays.",
            (e, a) => {
                var array0 = (Val[])a[0].AsArrayVal().Raw;
                var array1 = (Val[])a[1].AsArrayVal().Raw;
                return new ArrayVal(array0.Union(array1, new ValEqualityComparer(e)).ToArray());
            });

        public readonly BuiltInFuncDef indexOf = new BuiltInFuncDef("indexOf(array,*val)",
            "Returns the index of the first element in the `array` whose value matches `val`.",
            (e, a) => indexOfCore(e, a[0], a[1]).ToRealVal());

        public readonly BuiltInFuncDef lastIndexOf = new BuiltInFuncDef("lastIndexOf(array,*val)",
            "Returns the index of the last element in the `array` whose value matches `val`.",
            (e, a) => lastIndexOfCore(e, a[0], a[1]).ToRealVal());

        public readonly BuiltInFuncDef contains = new BuiltInFuncDef("contains(array,*val)",
            "Returns whether the `array` contains `val`.",
            (e, a) => BoolVal.FromBool(indexOfCore(e, a[0], a[1]) >= 0));

        private static int indexOfCore(EvalContext e, Val arrayVal, Val keyVal) {
            if (arrayVal is StrVal sVal0 && keyVal is StrVal sVal1) {
                return sVal0.AsString.IndexOf(sVal1.AsString);
            }
            else {
                var array = (Val[])arrayVal.AsArrayVal().Raw;
                if (keyVal is FuncVal keyFuncVal) {
                    var func = (FuncDef)keyFuncVal.Raw;
                    for (int i = 0; i < array.Length; i++) {
                        if (func.Call(e, array[i]).AsBool) return i;
                    }
                }
                else {
                    for (int i = 0; i < array.Length; i++) {
                        if (array[i].Equals(e, keyVal).AsBool) return i;
                    }
                }
                return -1;
            }
        }

        private static int lastIndexOfCore(EvalContext e, Val arrayVal, Val keyVal) {
            if (arrayVal is StrVal sVal0 && keyVal is StrVal sVal1) {
                return sVal0.AsString.LastIndexOf(sVal1.AsString);
            }
            else {
                var array = (Val[])arrayVal.AsArrayVal().Raw;
                if (keyVal is FuncVal keyFuncValVal) {
                    var func = (FuncDef)keyFuncValVal.Raw;
                    for (int i = array.Length - 1; i >= 0; i--) {
                        if (func.Call(e, array[i]).AsBool) return i;
                    }
                }
                else {
                    for (int i = array.Length - 1; i >= 0; i--) {
                        if (array[i].Equals(e, keyVal).AsBool) return i;
                    }
                }
                return -1;
            }
        }
    }
}
