using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.UnitSystem {
    abstract class Unit {
        public readonly UnitSyntax Syntax;

        /*
        public abstract IEnumerable<ScalarUnit> _old_BaseUnits { get; }
        public abstract UnitVector _old_Dictionary { get; }
        public abstract ScalarUnit this[string id] { get; }
        public abstract bool _old_HasDimension(string id);
        */

        public Unit(UnitSyntax syntax) {
            if (syntax == null) syntax = UnitSyntax.Unnamed;
            this.Syntax = syntax;
        }

        /*
        public ScalarUnit this[Dim dim] => this[dim.Id];
        public bool _old_HasDimension(Dim dim) => _old_HasDimension(dim.Id);
        public IEnumerable<Dim> _old_Dimensions => _old_BaseUnits.Select(p => p.Dimension);
        public bool IsDimless => this.Equals(_old_BaseUnit.Dimless);
        */

        /*
        public override bool Equals(object obj) {
            if (obj is Unit b)
                return UnitVectorEquals(b.Dictionary);
            else
                return false;
        }
        public override int GetHashCode() => base.GetHashCode();
        */

        /*

        public void AssertDimensionEquality(EvalContext e, Unit b) {
            if (!this._old_DimensionEquals(b)) throw new UnitException(b, "unit mismatch.");
        }

        public bool UnitVectorEquals(IReadOnlyDictionary<string, ScalarUnit> dic) {
            Dim[] dims = this._old_Dimensions.Concat(dic.Values.Select(p => p.Dimension)).Distinct().ToArray();
            foreach (var dim in dims) {
                if (dim == Dim.Dimless) continue;
                if (!this._old_HasDimension(dim) || !dic.ContainsKey(dim.Id)) return false;
                var elmA = this[dim];
                var elmB = dic[dim.Id];
                if (elmA.Exponent != elmB.Exponent) return false;
                if (elmA.Mult != elmB.Mult) return false;
                if (elmA.Div != elmB.Div) return false;
            }
            return true;
        }

        public bool _old_DimensionEquals(Unit b) {
            Dim[] dims = this._old_Dimensions.Concat(b._old_Dimensions).Distinct().ToArray();
            // todo: 例外のメッセージを詳細にする
            foreach (var dim in dims) {
                if (dim == Dim.Dimless) continue;
                if (!this._old_HasDimension(dim) || !b._old_HasDimension(dim)) return false;
                var elmA = this[dim];
                var elmB = b[dim];
                if (elmA.Exponent != elmB.Exponent) return false;
            }
            return true;
        }

        //public double Mul(Val b, out Unit newUnit) => Mul(b, out newUnit);
        public Unit _old_Mul(Unit b) {
            Dim[] dims = this._old_Dimensions.Concat(b._old_Dimensions).Distinct().ToArray();
            var dic = new Dictionary<string, ScalarUnit>();
            // todo: 例外のメッセージを詳細にする
            foreach (var dim in dims) {
                if (dim == Dim.Dimless) continue;
                bool aHas = this._old_HasDimension(dim);
                bool bHas = b._old_HasDimension(dim);
                if (aHas && bHas) {
                    var elmA = this[dim];
                    var elmB = b[dim];
                    int exp = elmA.Exponent + elmB.Exponent;
                    if (exp != 0) {
                        dic.Add(dim.Id, elmA.GetFrom(elmA.Mult, elmB.Div, exp));
                    }
                }
                else if (aHas) {
                    dic.Add(dim.Id, this[dim]);
                }
                else {
                    dic.Add(dim.Id, b[dim]);
                }
            }
            return UnitFactory.Default.Solve(dic);
        }

        /// <summary>単位のべき乗</summary>
        public Unit _old_Pow(int pwr) {
            var dic = new Dictionary<string, ScalarUnit>();
            foreach (var unit in _old_BaseUnits) {
                if (unit.IsDimless) continue;
                dic.Add(unit.Dimension.Id, unit.GetFrom(unit.Mult, unit.Div, unit.Exponent * pwr));
            }
            return UnitFactory.Default.Solve(dic);
        }

        /// <summary>単位の逆数</summary>
        public Unit _old_Invert() => _old_Pow(-1);

        /// <summary>単位の平方根</summary>
        public Unit _old_Sqrt() {
            var dic = new Dictionary<string, ScalarUnit>();
            foreach (var unit in _old_BaseUnits) {
                if (unit.IsDimless) continue;
                if ((unit.Exponent & 1) != 0) {
                    throw new UnitException(this, "In calculating the square root of an unit, its exponent must be even number.");
                }
                dic.Add(unit.Dimension.Id, unit.GetFrom(unit.Mult, unit.Div, unit.Exponent / 2));
            }
            return UnitFactory.Default.Solve(dic);
        }

        /// <summary>値をスケーリングする</summary>
        public double _old_ScaleValue(double val) {
            double mult = 1;
            double div = 1;
            foreach (var unit in _old_BaseUnits) {
                if (unit.Exponent > 0) {
                    mult *= Math.Pow(unit.Mult, unit.Exponent);
                    div *= Math.Pow(unit.Div, unit.Exponent);
                }
                else {
                    mult *= Math.Pow(unit.Div, -unit.Exponent);
                    div *= Math.Pow(unit.Mult, -unit.Exponent);
                }
            }
            return val * mult / div;
        }

        /// <summary>スケーリングされていない値に戻す</summary>
        public double _old_UnscaleValue(double val) {
            double mult = 1;
            double div = 1;
            foreach (var unit in _old_BaseUnits) {
                if (unit.Exponent > 0) {
                    mult *= Math.Pow(unit.Mult, unit.Exponent);
                    div *= Math.Pow(unit.Div, unit.Exponent);
                }
                else {
                    mult *= Math.Pow(unit.Div, -unit.Exponent);
                    div *= Math.Pow(unit.Mult, -unit.Exponent);
                }
            }
            return val * div / mult;
        }
        */

        /*
        public override string ToString() {
            string multStr = "";
            string divStr = "";
            foreach (var unit in _old_BaseUnits) {
                if (unit.Exponent >= 1) {
                    multStr += unit.Base.Syntax.Symbol;
                    if (unit.Exponent >= 2) multStr += unit.Exponent;
                }
                else if (unit.Exponent <= 1) {
                    divStr += "/" + unit.Base.Syntax.Symbol;
                    if (unit.Exponent <= -2) divStr += (-unit.Exponent);
                }
            }
            return multStr + divStr;
        }

        public void _old_Dump() {
            Console.WriteLine(this.ToString() + ":");
            foreach (var unit in _old_BaseUnits) {
                Console.WriteLine("  " + unit.Base.Syntax.Symbol + "\tx" + unit.Mult + "\t/" + unit.Div + "\t^" + unit.Exponent);
            }
        }
        */

        public override int GetHashCode() => base.GetHashCode();

        public IEnumerable<Unit> MergeUnitsWith(Unit b, UnitEnumMode mode)
            => this.EnumUnits(mode).Concat(b.EnumUnits(mode)).Distinct();

        public override bool Equals(object obj) {
            if (ReferenceEquals(this, obj))
                return true;
            else if (obj is Unit b)
                return Equals(b, UnitEnumMode.Named);
            else
                return false;
        }

        public bool Equals(Unit b, UnitEnumMode mode) {
            if (ReferenceEquals(this, b)) return true;
            foreach (var unit in this.MergeUnitsWith(b, mode)) {
                if (this.GetExpOf(unit) != b.GetExpOf(unit)) {
                    return false;
                }
            }
            return true;
        }

        public void AssertDimensionEquality(Unit b, UnitEnumMode mode) {
            if (!this.Equals(b, mode)) throw new UnitException(b, "Unit mismatch.");
        }

        /// <summary>命名された単位であるか否か</summary>
        public bool HasSymbol => !string.IsNullOrEmpty(this.Syntax.Symbol);

        /// <summary>スケールされていない内部表現に変換する(1km2-->1000000m2)</summary>
        public abstract double UnscaleValue(EvalContext e, double val);

        /// <summary>スケールされた表現に変換する(1000000m2-->km2)</summary>
        public abstract double ScaleValue(EvalContext e, double val);

        public bool IsDimless => this.Equals(NativeUnits.Dimless);

        /// <summary>単位記号</summary>
        public IEnumerable<UnitElement> EnumElements(UnitEnumMode mode) {
            var stack = new Stack<UnitElement>();
            stack.Push(new UnitElement(this, 1));
            while (stack.Count > 0) {
                var elm = stack.Pop();
                bool hit =
                    (mode.HasFlag(UnitEnumMode.Named) && elm.Unit.HasSymbol) ||
                    (mode.HasFlag(UnitEnumMode.Dimension) && elm.Unit is BaseUnit);

                if (hit) yield return elm;

                if (mode.HasFlag(UnitEnumMode.Complete) || !hit) {
                    foreach (var e in elm.Unit.OnEnumElements()) {
                        stack.Push(e);
                    }
                }
            }
        }

        /// <summary>単位記号</summary>
        public IEnumerable<Unit> EnumUnits(UnitEnumMode opt) => EnumElements(opt).Select(p => p.Unit);

        /// <summary>特定の単位に対する指数を返す</summary>
        public int GetExpOf(Unit unit) {
            if (ReferenceEquals(this, unit))
                return 1;
            else {
                int exp = 0;
                foreach (var elm in OnEnumElements()) {
                    exp += elm.Exp * elm.Unit.GetExpOf(unit);
                }
                return exp;
            }
        }

        /// <summary>単位記号</summary>
        protected abstract IEnumerable<UnitElement> OnEnumElements();

        public Unit Mul(EvalContext e, Unit b) => OnMul(e, b);
        public Unit Div(EvalContext e, Unit b) => OnDiv(e, b);
        public Unit Pow(EvalContext e, int exp) => OnPow(e, exp);
        public Unit Invert(EvalContext e) => OnInvert(e);
        public Unit Sqrt(EvalContext e) => OnSqrt(e);

        protected virtual Unit OnMul(EvalContext e, Unit b) {
            var enumOpt = UnitEnumMode.Dimension;
            var units = this.EnumUnits(enumOpt).Concat(b.EnumUnits(enumOpt)).Distinct().ToArray();
            var elms = new List<UnitElement>();
            foreach (var unit in units) {
                int exp = this.GetExpOf(unit) + b.GetExpOf(unit);
                if (exp != 0) elms.Add(new UnitElement(unit, exp));
            }
            if (elms.Count > 0)
                return new DerivedUnit(elms);
            else
                return NativeUnits.Dimless;
        }
        protected virtual Unit OnDiv(EvalContext e, Unit b) => OnMul(e, b.Invert(e));
        protected virtual Unit OnPow(EvalContext e, int exp) => new DerivedUnit(new UnitElement(this, exp));
        protected virtual Unit OnInvert(EvalContext e) => new DerivedUnit(new UnitElement(this, -1));
        protected abstract Unit OnSqrt(EvalContext e);

        public override string ToString() {
            if (this.HasSymbol) {
                return Syntax.Symbol;
            }
            else if (this.IsDimless) {
                return "dimless";
            }
            else {
                string strMul = "";
                string strDiv = "";
                foreach (var unit in EnumUnits(UnitEnumMode.Named)) {
                    int exp = this.GetExpOf(unit);
                    if (exp >= 1) {
                        strMul += unit.ToString();
                        if (exp >= 2) strMul += exp.ToString();
                    }
                    else if (exp <= -1) {
                        strDiv += "/" + unit.ToString();
                        if (exp <= -2) strMul += (-exp).ToString();
                    }
                }
                return strMul + strDiv;
            }
        }

        //public static bool operator ==(Unit a, Unit b) {
        //    if (Object.ReferenceEquals(a, b)) return true;
        //    if ((object)a == null || (object)b == null) return false;
        //    return a.Equals(b);
        //}
        //
        //public static bool operator !=(Unit a, Unit b) {
        //    return !(a == b);
        //}
    }
}
