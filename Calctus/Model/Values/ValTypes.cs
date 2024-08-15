using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Maths;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Values {
    using DestDict = Dictionary<Type, ValCastRule>;
    using CastDict = Dictionary<Type, Dictionary<Type, ValCastRule>>;

    static class ValTypes {
        public static readonly CastDict Rules = initRules();
        private static CastDict initRules() {
            var srcs = new CastDict();
            addRule<RealVal, FracVal>(srcs, ValCastRuleFlags.Degrading, RealToFrac);
            addRule<FracVal, RealVal>(srcs, ValCastRuleFlags.Degrading, FracToReal);
            return srcs;
        }

        public static FracVal RealToFrac(RealVal val, out bool degr, object tag)
            => new FracVal(rational.FromDecimal(val.Raw, out degr));
        
        public static RealVal FracToReal(FracVal val, out bool degr, object tag)
            => new RealVal(val.Raw.ToDecimal(CastOptions.AllowDegrade, out degr));

        private static void addRule<TSrc, TDest>(CastDict catalog, ValCastRuleFlags flags, CastFunc<TSrc, TDest> castFunc) where TSrc : Val where TDest : Val {
            if (!catalog.TryGetValue(typeof(TSrc), out DestDict rules)) {
                catalog[typeof(TSrc)] = rules = new DestDict();
            }
            if (rules.ContainsKey(typeof(TDest))) {
                throw Log.Here().F(new InvalidOperationException("Cast rule conflicts."));
            }
            rules[typeof(FracVal)] = new CastRule<TSrc, TDest>(flags, castFunc);
        }

        public static Val Cast(EvalContext e, Val srcVal, Type destType, object tag = null) {
            var srcType = srcVal.GetType();
            if (srcType.Equals(destType)) return srcVal;
            if (Rules.TryGetValue(srcType, out DestDict destDictt)) {
                if (destDictt.TryGetValue(destType, out ValCastRule rule)) {
                    rule.Cast(e, srcVal, tag);
                }
            }
            throw Log.Here().E(new InvalidCastException(DisplayTypeNameOf(srcType) + " cannot cast to " + DisplayTypeNameOf(destType)));
        }

        public static TDest Cast<TDest>(EvalContext e, Val srcVal, object tag = null) where TDest : Val {
            if (srcVal is TDest) return (TDest)srcVal;
            return (TDest)Cast(e, srcVal, typeof(TDest), tag);
        }
        
        public static string DisplayTypeNameOf(Type type) {
            const string typeNamePostfix = "Val";
            var typeName = type.Name;
            if (typeName.EndsWith(typeNamePostfix)) {
                typeName = typeName.Substring(0, typeName.Length - typeNamePostfix.Length);
            }
            return typeName;
        }
    }
}
