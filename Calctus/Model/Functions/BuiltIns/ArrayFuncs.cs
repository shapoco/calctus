using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    static class ArrayFuncs {
        public static readonly BuiltInFuncDef len = new BuiltInFuncDef("len(array)", (e, a) 
            => new RealVal(((ArrayVal)a[0]).Length), "Length of `array`");
        
        public static readonly BuiltInFuncDef range_2 = new BuiltInFuncDef("range(start, stop)", (e, a) 
            => new ArrayVal(RMath.Range(a[0].AsReal, a[1].AsReal, 0, false)),
            "Returns an array consists of sequence of numbers greater than or equal to `start` and less than `stop`.");
        
        public static readonly BuiltInFuncDef range_3 = new BuiltInFuncDef("range(start, stop, step)", (e, a) 
            => new ArrayVal(RMath.Range(a[0].AsReal, a[1].AsReal, a[2].AsReal, false)), 
            "Returns an array consists of sequence of numbers greater than or equal to `start` and less than `stop` with common difference `step`.");
        
        public static readonly BuiltInFuncDef rangeInclusive_2 = new BuiltInFuncDef("rangeInclusive(start, stop)", (e, a) 
            => new ArrayVal(RMath.Range(a[0].AsReal, a[1].AsReal, 0, true)), 
            "Returns an array consists of sequence of numbers greater than or equal to `start` and less than or equal to `stop`.");
        
        public static readonly BuiltInFuncDef rangeInclusive_3 = new BuiltInFuncDef("rangeInclusive(start, stop, step)", (e, a) 
            => new ArrayVal(RMath.Range(a[0].AsReal, a[1].AsReal, a[2].AsReal, true)), 
            "Returns an array consists of sequence of numbers greater than or equal to `start` and less than or equal to `stop` with common difference `step`.");
        
        public static readonly BuiltInFuncDef reverseArray = new BuiltInFuncDef("reverseArray(array)", (e, a) => {
            var array = (Val[])((ArrayVal)a[0]).Raw;
            Array.Reverse(array);
            return new ArrayVal(array, a[0].FormatHint);
        }, "Reverses the order of elements of `array`");


        public static readonly BuiltInFuncDef map = new BuiltInFuncDef("map(array,func)", (e, a) => {
            var array = (Val[])a[0].AsArrayVal().Raw;
            var func = (FuncDef)a[1].Raw;
            return new ArrayVal(array.Select(p => func.Call(e, p)).ToArray());
        }, "Map the `array` using a converter function `func(x)`.");

        public static readonly BuiltInFuncDef filter = new BuiltInFuncDef("filter(array,func)", (e, a) => {
            var array = (Val[])a[0].AsArrayVal().Raw;
            var func = (FuncDef)a[1].Raw;
            return new ArrayVal(array.Where(p => func.Call(e, p).AsBool).ToArray(), a[0].FormatHint);
        }, "Filter the `array` using a tester function `func(x)`.");

        public static readonly BuiltInFuncDef count = new BuiltInFuncDef("count(array,func)", (e, a) => {
            var array = (Val[])a[0].AsArrayVal().Raw;
            var func = (FuncDef)a[1].Raw;
            return new RealVal(array.Count(p => func.Call(e, p).AsBool));
        }, "Count specific elements in the `array` using a tester function `func(x)`.");

        public static readonly BuiltInFuncDef sort_1 = new BuiltInFuncDef("sort(array)", (e, a) => {
            var array = (Val[])a[0].AsArrayVal().Raw;
            return new ArrayVal(array.OrderBy(p => p, new ValComparer(e)).ToArray(), a[0].FormatHint);
        }, "Sort the `array`.");

        public static readonly BuiltInFuncDef sort_2 = new BuiltInFuncDef("sort(array,func)", (e, a) => {
            var array = (Val[])a[0].AsArrayVal().Raw;
            var func = (FuncDef)a[1].Raw;
            return new ArrayVal(array.OrderBy(p => func.Call(e, p).AsReal).ToArray(), a[0].FormatHint);
        }, "Sort the `array` using a evaluation function `func(x)`.");

        public static readonly BuiltInFuncDef extend = new BuiltInFuncDef("extend(array,func,count)", (e, a) => {
            var seedArray = a[0].AsArrayVal();
            var list = ((Val[])seedArray.Raw).ToList();
            var func = (FuncDef)a[1].Raw;
            var countReal = a[2].AsReal;
            if (!RMath.IsInteger(countReal) || countReal <= 0) throw new ArgumentOutOfRangeException();
            int count = (int)countReal;
            ArrayVal.CheckArrayLength(list.Count + count);
            for (int i = 0; i < count; i++) {
                list.Add(func.Call(e, new ArrayVal(list.ToArray())));
            }
            return new ArrayVal(list.ToArray(), seedArray.FormatHint);
        }, "Extends the `array` using converter function `func(array)`.");

        public static readonly BuiltInFuncDef aggregate = new BuiltInFuncDef("aggregate(array,func)", (e, a) => {
            var array = (Val[])a[0].AsArrayVal().Raw;
            var func = (FuncDef)a[1].Raw;
            return array.Aggregate((p, q) => func.Call(e, p, q));
        }, "Apply the aggregate function `func(a,b)` for the `array`.");

        public static readonly BuiltInFuncDef all_1 = new BuiltInFuncDef("all(array)", (e, a) => {
            var array = (Val[])a[0].AsArrayVal().Raw;
            return BoolVal.FromBool(array.All(p => p.AsBool));
        }, "Returns true if all `array` elements are true.");

        public static readonly BuiltInFuncDef all_2 = new BuiltInFuncDef("all(array,func)", (e, a) => {
            var array = (Val[])a[0].AsArrayVal().Raw;
            var func = (FuncDef)a[1].Raw;
            return BoolVal.FromBool(array.All(p => func.Call(e, p).AsBool));
        }, "Returns true if tester function `func(x)` returns true for all elements of the `array`.");

        public static readonly BuiltInFuncDef any_1 = new BuiltInFuncDef("any(array)", (e, a) => {
            var array = (Val[])a[0].AsArrayVal().Raw;
            return BoolVal.FromBool(array.Any(p => p.AsBool));
        }, "Returns true if at least one element is true.");

        public static readonly BuiltInFuncDef any_2 = new BuiltInFuncDef("any(array,func)", (e, a) => {
            var array = (Val[])a[0].AsArrayVal().Raw;
            var func = (FuncDef)a[1].Raw;
            return BoolVal.FromBool(array.Any(p => func.Call(e, p).AsBool));
        }, "Returns true if tester function `func(x)` returns true for at least one element of the `array`.");

        public static readonly BuiltInFuncDef unique_1 = new BuiltInFuncDef("unique(array)", (e, a) => {
            var array = (Val[])a[0].AsArrayVal().Raw;
            return new ArrayVal(array.Distinct(new ValEqualityComparer(e)).ToArray(), a[0].FormatHint);
        }, "Returns an array of unique elements.");

        public static readonly BuiltInFuncDef unique_2 = new BuiltInFuncDef("unique(array,func)", (e, a) => {
            var array = (Val[])a[0].AsArrayVal().Raw;
            var func = (FuncDef)a[1].Raw;
            return new ArrayVal(array.Distinct(new EqualityComparerFunc(e, func)).ToArray(), a[0].FormatHint);
        }, "Return unique elements using comparer function `func(a,b)`.");

        public static readonly BuiltInFuncDef except = new BuiltInFuncDef("except(array0,array1)", (e, a) => {
            var array0 = (Val[])a[0].AsArrayVal().Raw;
            var array1 = (Val[])a[1].AsArrayVal().Raw;
            return new ArrayVal(array0.Except(array1, new ValEqualityComparer(e)).ToArray(), a[0].FormatHint);
        }, "Returns the difference set of the two arrays.");

        public static readonly BuiltInFuncDef intersect = new BuiltInFuncDef("intersect(array0,array1)", (e, a) => {
            var array0 = (Val[])a[0].AsArrayVal().Raw;
            var array1 = (Val[])a[1].AsArrayVal().Raw;
            return new ArrayVal(array0.Intersect(array1, new ValEqualityComparer(e)).ToArray(), a[0].FormatHint);
        }, "Returns the product set of the two arrays.");

        public static readonly BuiltInFuncDef union = new BuiltInFuncDef("union(array0,array1)", (e, a) => {
            var array0 = (Val[])a[0].AsArrayVal().Raw;
            var array1 = (Val[])a[1].AsArrayVal().Raw;
            return new ArrayVal(array0.Union(array1, new ValEqualityComparer(e)).ToArray(), a[0].FormatHint);
        }, "Returns the union of the two arrays.");

        public static readonly BuiltInFuncDef indexOf = new BuiltInFuncDef("indexOf(array,*val)", (e, a) => {
            if (a[0] is StrVal sVal0 && a[1] is StrVal sVal1) {
                return new RealVal(sVal0.AsString.IndexOf(sVal1.AsString));
            }
            else {
                var array = (Val[])a[0].AsArrayVal().Raw;
                if (a[1] is FuncVal fVal) {
                    var func = (FuncDef)fVal.Raw;
                    for (int i = 0; i < array.Length; i++) {
                        if (func.Call(e, array[i]).AsBool) return new RealVal(i);
                    }
                }
                else {
                    for (int i = 0; i < array.Length; i++) {
                        if (array[i].Equals(e, a[1]).AsBool) return new RealVal(i);
                    }
                }
                return new RealVal(-1);
            }
        }, "Returns the index of the first element in the `array` whose value matches `val`.");

        public static readonly BuiltInFuncDef lastIndexOf = new BuiltInFuncDef("lastIndexOf(array,*val)", (e, a) => {
            if (a[0] is StrVal sVal0 && a[1] is StrVal sVal1) {
                return new RealVal(sVal0.AsString.LastIndexOf(sVal1.AsString));
            }
            else {
                var array = (Val[])a[0].AsArrayVal().Raw;
                if (a[1] is FuncVal fVal) {
                    var func = (FuncDef)fVal.Raw;
                    for (int i = array.Length - 1; i >= 0; i--) {
                        if (func.Call(e, array[i]).AsBool) return new RealVal(i);
                    }
                }
                else {
                    for (int i = array.Length - 1; i >= 0; i--) {
                        if (array[i].Equals(e, a[1]).AsBool) return new RealVal(i);
                    }
                }
                return new RealVal(-1);
            }
        }, "Returns the index of the last element in the `array` whose value matches `val`.");

        public static readonly BuiltInFuncDef contains = new BuiltInFuncDef("contains(array,*val)", (e, a) => {
            return indexOf.Call(e, a[0], a[1]).GraterEqual(e, new RealVal(0));
        }, "Returns whether the `array` contains `val`.");
    }
}
