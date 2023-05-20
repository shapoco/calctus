using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shapoco.Calctus.Model.Syntax;

namespace Shapoco.Calctus.Model {
    abstract class Val {
        public readonly ValFormatHint FormatHint;

        public Val(ValFormatHint fmt = null) {
            this.FormatHint = fmt != null ? fmt : ValFormatHint.Default;
        }

        public string ValTypeName => this.GetType().Name;
        public abstract object Raw { get; }

        public abstract bool IsScalar { get; }
        public abstract bool IsInteger { get; }

        // RealVal への変換はフォーマットを確実に引き継ぐため
        // OnAsRealVal() で型変換したあとここで Format() する
        public RealVal AsRealVal() {
            if (this is RealVal thisReal)
                return thisReal;
            else 
                return (RealVal)(OnAsRealVal().Format(FormatHint));
        }
        protected abstract RealVal OnAsRealVal();

        public abstract real AsReal { get; }
        public abstract frac AsFrac { get; }
        public abstract double AsDouble { get; }
        public abstract long AsLong { get; }
        public abstract int AsInt { get; }
        public abstract bool AsBool { get; }

        public abstract real[] AsRealArray { get; }
        public abstract long[] AsLongArray { get; }
        public abstract int[] AsIntArray { get; }

        //public static implicit operator Val(double val) => new RealVal(val);
        //public static implicit operator double(Val val) => val.AsDouble();
        //
        //public static explicit operator Val(long val) => new RealVal((double)val);
        //public static explicit operator long(Val val) => val.AsLong();
        //
        //public static explicit operator Val(int val) => new RealVal((double)val);
        //public static explicit operator int(Val val) => val.AsInt();

        public Val FormatInt() => Format(new ValFormatHint(NumberFormatter.CStyleInt));
        public Val FormatReal() => Format(new ValFormatHint(NumberFormatter.CStyleReal));
        public Val FormatHex() => Format(new ValFormatHint(NumberFormatter.CStyleHex));
        public Val FormatBin() => Format(new ValFormatHint(NumberFormatter.CStyleBin));
        public Val FormatOct() => Format(new ValFormatHint(NumberFormatter.CStyleOct));
        public Val FormatChar() => Format(new ValFormatHint(NumberFormatter.CStyleChar));
        public Val FormatSiPrefix() => Format(new ValFormatHint(NumberFormatter.SiPrefixed));
        public Val FormatBinaryPrefix() => Format(new ValFormatHint(NumberFormatter.BinaryPrefixed));
        public Val FormatDateTime() => Format(new ValFormatHint(NumberFormatter.DateTime));
        public Val FormatWebColor() => Format(new ValFormatHint(NumberFormatter.WebColor));

        public Val Format(ValFormatHint fmt) {
            if (this.FormatHint.Equals(fmt)) 
                return this;
            else
                return OnFormat(fmt);
        }
        protected abstract Val OnFormat(ValFormatHint fmt);

        public Val UpConvert(EvalContext ctx, Val b) => OnUpConvert(ctx,b).Format(FormatHint);
        protected abstract Val OnUpConvert(EvalContext ctx, Val b);

        // 単項演算
        public Val UnaryPlus(EvalContext ctx) => OnUnaryPlus(ctx).Format(FormatHint);
        public Val ArithInv(EvalContext ctx) => OnAtirhInv(ctx).Format(FormatHint);
        public Val BitNot(EvalContext ctx) => OnBitNot(ctx).Format(FormatHint);
        public Val LogicNot(EvalContext ctx) => OnLogicNot(ctx).Format(FormatHint);
        protected abstract Val OnUnaryPlus(EvalContext ctx);
        protected abstract Val OnAtirhInv(EvalContext ctx);
        protected abstract Val OnBitNot(EvalContext ctx);
        protected abstract Val OnLogicNot(EvalContext ctx);

        // 算術演算
        // 右項に精度を合わせるため UpConvert する
        public Val Add(EvalContext ctx, Val b) => this.UpConvert(ctx, b).OnAdd(ctx, b).Format(FormatHint.Select(b.FormatHint));
        public Val Sub(EvalContext ctx, Val b) => this.UpConvert(ctx, b).OnSub(ctx, b).Format(FormatHint.Select(b.FormatHint));
        public Val Mul(EvalContext ctx, Val b) => this.UpConvert(ctx, b).OnMul(ctx, b).Format(FormatHint.Select(b.FormatHint));
        public Val Div(EvalContext ctx, Val b) => this.UpConvert(ctx, b).OnDiv(ctx, b).Format(FormatHint.Select(b.FormatHint));
        public Val IDiv(EvalContext ctx, Val b) => this.UpConvert(ctx, b).OnIDiv(ctx, b).Format(FormatHint.Select(b.FormatHint));
        public Val Mod(EvalContext ctx, Val b) => this.UpConvert(ctx, b).OnMod(ctx, b).Format(FormatHint.Select(b.FormatHint));
        protected abstract Val OnAdd(EvalContext ctx, Val b);
        protected abstract Val OnSub(EvalContext ctx, Val b);
        protected abstract Val OnMul(EvalContext ctx, Val b);
        protected abstract Val OnDiv(EvalContext ctx, Val b);
        protected abstract Val OnIDiv(EvalContext ctx, Val b);
        protected abstract Val OnMod(EvalContext ctx, Val b);

        // シフト演算
        // 右項と型を合わせる必要無いので UpConvert しない
        public Val LogicShiftL(EvalContext ctx, Val b) => this.OnLogicShiftL(ctx, b).Format(FormatHint);
        public Val LogicShiftR(EvalContext ctx, Val b) => this.OnLogicShiftR(ctx, b).Format(FormatHint);
        public Val ArithShiftL(EvalContext ctx, Val b) => this.OnArithShiftL(ctx, b).Format(FormatHint);
        public Val ArithShiftR(EvalContext ctx, Val b) => this.OnArithShiftR(ctx, b).Format(FormatHint);
        protected abstract Val OnLogicShiftL(EvalContext ctx, Val b);
        protected abstract Val OnLogicShiftR(EvalContext ctx, Val b);
        protected abstract Val OnArithShiftL(EvalContext ctx, Val b);
        protected abstract Val OnArithShiftR(EvalContext ctx, Val b);

        // 比較演算
        public Val Grater(EvalContext ctx, Val b) => UpConvert(ctx, b).OnGrater(ctx, b);
        public Val Less(EvalContext ctx, Val b) => b.UpConvert(ctx, this).OnGrater(ctx, this);
        public Val Equal(EvalContext ctx, Val b) => UpConvert(ctx, b).OnEqual(ctx, b);
        public Val GraterEqual(EvalContext ctx, Val b) {
            var a = UpConvert(ctx, b);
            return a.OnGrater(ctx, b).OnLogicOr(ctx, a.OnEqual(ctx, b));
        }
        public Val LessEqual(EvalContext ctx, Val b) {
            b = b.UpConvert(ctx, this);
            return b.OnGrater(ctx, this).OnLogicOr(ctx, b.OnEqual(ctx, this));
        }
        public Val NotEqual(EvalContext ctx, Val b) => UpConvert(ctx, b).OnEqual(ctx, b).OnLogicNot(ctx);
        protected abstract Val OnGrater(EvalContext ctx, Val b);
        protected abstract Val OnEqual(EvalContext ctx, Val b);

        // ビット演算
        public Val BitAnd(EvalContext ctx, Val b) => this.UpConvert(ctx, b).OnBitAnd(ctx, b).Format(FormatHint.Select(b.FormatHint));
        public Val BitXor(EvalContext ctx, Val b) => this.UpConvert(ctx, b).OnBitXor(ctx, b).Format(FormatHint.Select(b.FormatHint));
        public Val BitOr(EvalContext ctx, Val b) => this.UpConvert(ctx, b).OnBitOr(ctx, b).Format(FormatHint.Select(b.FormatHint));
        protected abstract Val OnBitAnd(EvalContext ctx, Val b);
        protected abstract Val OnBitXor(EvalContext ctx, Val b);
        protected abstract Val OnBitOr(EvalContext ctx, Val b);

        // 論理演算
        public Val LogicAnd(EvalContext ctx, Val b) => this.UpConvert(ctx, b).OnLogicAnd(ctx, b).Format(FormatHint);
        public Val LogicOr(EvalContext ctx, Val b) => this.UpConvert(ctx, b).OnLogicOr(ctx, b).Format(FormatHint);
        protected abstract Val OnLogicAnd(EvalContext ctx, Val b);
        protected abstract Val OnLogicOr(EvalContext ctx, Val b);

        public override string ToString() => this.ToString(new EvalContext());
        public abstract string ToString(EvalContext e);
    }
}
