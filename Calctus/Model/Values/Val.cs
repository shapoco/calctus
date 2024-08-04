using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Maths;
using Shapoco.Calctus.Model;
using Shapoco.Calctus.Model.Formats;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Values {
    abstract class Val {
        public virtual string CalctusTypeName {
            get {
                const string typeNamePostfix = "Val";
                var typeName = GetType().Name;
                if (typeName.EndsWith(typeNamePostfix)) {
                    typeName = typeName.Substring(0, typeName.Length - typeNamePostfix.Length);
                }
                return typeName;
            }
        }

        public object Raw => OnGetRaw();
        protected abstract object OnGetRaw();

        public abstract bool IsScalar { get; }
        public abstract bool IsInteger { get; }
        public abstract bool IsSerializable { get; }

        public virtual FormatFlags FormatFlags => FormatFlags.Default;
        public virtual Val Format(FormatFlags fmt) => this;

        // RealVal への変換はフォーマットを確実に引き継ぐため
        // OnAsRealVal() で型変換したあとここで Format() する
        public RealVal AsRealVal() {
            if (this is RealVal thisReal)
                return thisReal;
            else
                return OnAsRealVal();
        }
        protected abstract RealVal OnAsRealVal();

        public abstract decimal AsDecimal { get; }
        public abstract Frac AsFrac { get; }
        public abstract double AsDouble { get; }
        public abstract long AsLong { get; }
        public abstract int AsInt { get; }
        public abstract char AsChar { get; }
        public abstract byte AsByte { get; }

        public bool ToBool() => ((BoolVal)this).Raw;
        public ICollectionVal ToCollectionVal() => (ICollectionVal)this;

        public abstract decimal[] AsDecimalArray { get; }
        public abstract long[] AsLongArray { get; }
        public abstract int[] AsIntArray { get; }
        public abstract byte[] AsByteArray { get; }

        // 単項演算
        public virtual Val UnaryPlus(EvalContext ctx) => throw new NotSupportedException();
        public virtual Val ArithInv(EvalContext ctx) => throw new NotSupportedException();
        public virtual Val BitNot(EvalContext ctx) => throw new NotSupportedException();
        public virtual Val LogicNot(EvalContext ctx) => throw new NotSupportedException();

        // 算術演算
        public virtual Val Add(EvalContext ctx, Val b) => throw new NotSupportedException();
        public virtual Val Sub(EvalContext ctx, Val b) => throw new NotSupportedException();
        public virtual Val Mul(EvalContext ctx, Val b) => throw new NotSupportedException();
        public virtual Val Div(EvalContext ctx, Val b) => throw new NotSupportedException();
        public virtual Val IDiv(EvalContext ctx, Val b) => throw new NotSupportedException();
        public virtual Val Mod(EvalContext ctx, Val b) => throw new NotSupportedException();

        // シフト演算
        // 右項と型を合わせる必要無いので UpConvert しない
        public virtual Val LogicShiftL(EvalContext ctx, Val b) => throw new NotSupportedException();
        public virtual Val LogicShiftR(EvalContext ctx, Val b) => throw new NotSupportedException();
        public virtual Val ArithShiftL(EvalContext ctx, Val b) => throw new NotSupportedException();
        public virtual Val ArithShiftR(EvalContext ctx, Val b) => throw new NotSupportedException();

        // 比較演算
        public virtual bool Grater(EvalContext ctx, Val b) => throw new NotSupportedException();
        public bool Less(EvalContext ctx, Val b) => b.Grater(ctx, this);
        public virtual bool Equals(EvalContext ctx, Val b) => throw new NotSupportedException();
        public bool NotEqual(EvalContext ctx, Val b) => !Equals(ctx, b);
        public bool GraterEqual(EvalContext ctx, Val b) => this.Grater(ctx, b) || this.Equals(ctx, b);
        public bool LessEqual(EvalContext ctx, Val b) => b.Grater(ctx, this) || b.Equals(ctx, this);

        // ビット演算
        public virtual Val BitAnd(EvalContext ctx, Val b) => throw new NotSupportedException();
        public virtual Val BitXor(EvalContext ctx, Val b) => throw new NotSupportedException();
        public virtual Val BitOr(EvalContext ctx, Val b) => throw new NotSupportedException();

        // 論理演算
        public virtual Val LogicAnd(EvalContext ctx, Val b) => throw new NotSupportedException();
        public virtual Val LogicOr(EvalContext ctx, Val b) => throw new NotSupportedException();

        public override string ToString() => ToStringForDisplay();
        public string ToString(ToStringArgs args) => Formatter.ObjectToString(Raw, new ToStringArgs(args, FormatFlags));
        public string ToStringForDisplay() => Formatter.ObjectToString(Raw, new ToStringArgs(FormatFlags, new FormatSettings(), StringUsage.ForDisplay));
        public string ToStringForValue(EvalContext e) {
            if (IsSerializable) {
                return Formatter.ObjectToString(Raw, ToStringArgs.ForValue(e, FormatFlags));
            }
            else {
                throw new InvalidCastException(this.CalctusTypeName + " cannot be converted to string.");
            }
        }
        public string ToStringForLiteral() => Formatter.ObjectToString(Raw, ToStringArgs.ForLiteral());

        public override int GetHashCode() => Raw.GetHashCode();
    }
}
