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
        protected abstract Val OnUnaryPlus(EvalContext ctx);
        protected abstract Val OnAtirhInv(EvalContext ctx);
        protected abstract Val OnBitNot(EvalContext ctx);
        //public static Val operator +(Val a) => a.UnaryPlus();
        //public static Val operator -(Val a) => a.ArithInv();
        //public static Val operator ~(Val a) => a.BitNot();

        // 算術演算
        // 右項に精度を合わせるため UpConvert する
        public Val Add(EvalContext ctx, Val b) => this.UpConvert(ctx, b).OnAdd(ctx, b).Format(FormatHint);
        public Val Sub(EvalContext ctx, Val b) => this.UpConvert(ctx, b).OnSub(ctx, b).Format(FormatHint);
        public Val Mul(EvalContext ctx, Val b) => this.UpConvert(ctx, b).OnMul(ctx, b).Format(FormatHint);
        public Val Div(EvalContext ctx, Val b) => this.UpConvert(ctx, b).OnDiv(ctx, b).Format(FormatHint);
        public Val IDiv(EvalContext ctx, Val b) => this.UpConvert(ctx, b).OnIDiv(ctx, b).Format(FormatHint);
        public Val Mod(EvalContext ctx, Val b) => this.UpConvert(ctx, b).OnMod(ctx, b).Format(FormatHint);
        protected abstract Val OnAdd(EvalContext ctx, Val b);
        protected abstract Val OnSub(EvalContext ctx, Val b);
        protected abstract Val OnMul(EvalContext ctx, Val b);
        protected abstract Val OnDiv(EvalContext ctx, Val b);
        protected abstract Val OnIDiv(EvalContext ctx, Val b);
        protected abstract Val OnMod(EvalContext ctx, Val b);
        //public static Val operator +(Val a, Val b) => a.Add(b);
        //public static Val operator -(Val a, Val b) => a.Sub(b);
        //public static Val operator *(Val a, Val b) => a.Mul(b);
        //public static Val operator /(Val a, Val b) => a.Div(b);
        //public static Val operator %(Val a, Val b) => a.Mod(b);

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
        // 論理シフトと算術シフトが紛らわしいので演算子オーバーロードはしない
        //public static Val operator <<(Val a, int b) => a.LogicShiftL(b);
        //public static Val operator >>(Val a, int b) => a.ArithShiftR(b);

        // ビット演算
        public Val BitAnd(EvalContext ctx, Val b) => this.UpConvert(ctx, b).OnBitAnd(ctx, b).Format(FormatHint);
        public Val BitXor(EvalContext ctx, Val b) => this.UpConvert(ctx, b).OnBitXor(ctx, b).Format(FormatHint);
        public Val BitOr(EvalContext ctx, Val b) => this.UpConvert(ctx, b).OnBitOr(ctx, b).Format(FormatHint);
        protected abstract Val OnBitAnd(EvalContext ctx, Val b);
        protected abstract Val OnBitXor(EvalContext ctx, Val b);
        protected abstract Val OnBitOr(EvalContext ctx, Val b);
        //public static Val operator &(Val a, Val b) => a.BitAnd(b);
        //public static Val operator ^(Val a, Val b) => a.BitXor(b);
        //public static Val operator |(Val a, Val b) => a.BitOr(b);

        // 論理演算
        //public Val LogicAnd(Val b) => this.UpConvert(b)._add(b).Format(FormatHint);
        //public Val LogicOr(Val b) => this.UpConvert(b)._add(b).Format(FormatHint);
        //protected abstract Val _logicand(Val b);
        //protected abstract Val _logicor(Val b);

        public override string ToString() => this.ToString(new EvalContext());
        public abstract string ToString(EvalContext e);
    }
}
