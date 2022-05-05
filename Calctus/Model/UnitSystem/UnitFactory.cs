using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.UnitSystem {
    class UnitFactory {
        public static readonly UnitFactory Default = new UnitFactory();

        /*
        public Unit Solve(Unit unit) {
            return Solve(unit._old_Dictionary);
        }

        public Unit Solve(IReadOnlyDictionary<string, _old_ScalarUnit> dic) {
            // 指数がゼロになっている次元を削除する
            bool allZero = true;
            foreach (var unit in dic.Values) {
                if (unit.IsDimless) continue;
                if (unit.Exponent == 0) {
                    Console.WriteLine("*WARNING: " + unit.Dimension.DisplayName + " の次元の指数がゼロになっています");
                }
                else {
                    allZero = false;
                }
            }

            // 無次元判定
            if (allZero) return _old_BaseUnit.Dimless;

            // 定義済の単位と一致すればそれを返す
            var ret = _old_BaseUnit._old_NativeUnits.FirstOrDefault(p => p.UnitVectorEquals(dic));
            if (ret != null) return ret;

            // 単次元の場合はそれをのまま返す
            if (dic.Count == 1) {
                return dic.Values.First();
            }

            // どれにも一致しない場合は組立単位にして返す
            return new _old_DerivedUnit(UnitSyntax.Unnamed, new UnitVector(dic.Values));
        }
        */

        public Unit Solve(string unitExpr) {
            foreach (var unit in NativeUnits.List) {
                if (unit.Syntax.Symbol == unitExpr) {
                    return unit;
                }
            }
            throw new UnitException("Unit cannot be solved: '" + unitExpr + "'");
        }
    }
}
