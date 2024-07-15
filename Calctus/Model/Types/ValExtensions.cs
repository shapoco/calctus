using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model;
using Shapoco.Calctus.Model.Formats;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Types {
    static class ValExtensions {
        public static RealVal ToRealVal(this real val, FormatHint fh = null) => new RealVal(val, fh);
        public static RealVal ToRealVal(this int val, FormatHint fh = null) => new RealVal(val, fh);
        public static RealVal ToRealVal(this long val, FormatHint fh = null) => new RealVal(val, fh);
        public static RealVal ToRealVal(this char val, FormatHint fh = null) => new RealVal(val, fh);

        public static RealVal ToIntVal(this real val) => new RealVal(val, FormatHint.CStyleInt);
        public static RealVal ToIntVal(this int val) => new RealVal(val, FormatHint.CStyleInt);
        public static RealVal ToIntVal(this long val) => new RealVal(val, FormatHint.CStyleInt);
        public static RealVal ToIntVal(this char val) => new RealVal(val, FormatHint.CStyleInt);

        public static RealVal ToHexVal(this real val) => new RealVal(val, FormatHint.CStyleHex);
        public static RealVal ToHexVal(this int val) => new RealVal(val, FormatHint.CStyleHex);
        public static RealVal ToHexVal(this long val) => new RealVal(val, FormatHint.CStyleHex);
        public static RealVal ToHexVal(this char val) => new RealVal(val, FormatHint.CStyleChar);

        public static RealVal ToCharVal(this real val) => new RealVal(val, FormatHint.CStyleChar);
        public static RealVal ToCharVal(this int val) => new RealVal(val, FormatHint.CStyleChar);
        public static RealVal ToCharVal(this long val) => new RealVal(val, FormatHint.CStyleChar);
        public static RealVal ToCharVal(this char val) => new RealVal(val, FormatHint.CStyleChar);

        public static RealVal ToDateTimeVal(this real val) => new RealVal(val, FormatHint.DateTime);
        public static RealVal ToDateTimeVal(this int val) => new RealVal(val, FormatHint.DateTime);
        public static RealVal ToDateTimeVal(this long val) => new RealVal(val, FormatHint.DateTime);

        public static RealVal ToColorVal(this real val) => new RealVal(val, FormatHint.WebColor);
        public static RealVal ToColorVal(this int val) => new RealVal(val, FormatHint.WebColor);
        public static RealVal ToColorVal(this long val) => new RealVal(val, FormatHint.WebColor);

        public static FracVal ToFracVal(this frac val, FormatHint fh = null) => new FracVal(val, fh);

        public static StrVal ToStrVal(this string val, FormatHint fh = null) => new StrVal(val, fh);

        public static Val[] ToValArray(this real[] val, FormatHint fh = null) {
            ArrayVal.CheckArrayLength(val.Length);
            var array = new Val[val.Length];
            for(int i = 0; i < val.Length; i++) {
                array[i] = new RealVal(val[i], fh);
            }
            return array;
        }
        public static Val[] ToValArray(this byte[] val, FormatHint fh = null) {
            ArrayVal.CheckArrayLength(val.Length);
            var array = new Val[val.Length];
            for (int i = 0; i < val.Length; i++) {
                array[i] = new RealVal(val[i], fh);
            }
            return array;
        }
        public static Val[] ToValArray(this int[] val, FormatHint fh = null) {
            ArrayVal.CheckArrayLength(val.Length);
            var array = new Val[val.Length];
            for (int i = 0; i < val.Length; i++) {
                array[i] = new RealVal(val[i], fh);
            }
            return array;
        }
        public static Val[] ToValArray(this long[] val, FormatHint fh = null) {
            ArrayVal.CheckArrayLength(val.Length);
            var array = new Val[val.Length];
            for (int i = 0; i < val.Length; i++) {
                array[i] = new RealVal(val[i], fh);
            }
            return array;
        }
        public static Val[] ToValArray(this string[] val, FormatHint fh = null) {
            ArrayVal.CheckArrayLength(val.Length);
            var array = new Val[val.Length];
            for (int i = 0; i < val.Length; i++) {
                array[i] = new StrVal(val[i], fh);
            }
            return array;
        }

        public static ArrayVal ToArrayVal(this Val[] val, FormatHint fh = null) => new ArrayVal(val, fh);
        public static ArrayVal ToArrayVal(this real[] val, FormatHint elemFmt, FormatHint arrayFmt)=> new ArrayVal(val.ToValArray(elemFmt), arrayFmt);
        public static ArrayVal ToArrayVal(this byte[] val, FormatHint elemFmt, FormatHint arrayFmt) => new ArrayVal(val.ToValArray(elemFmt), arrayFmt);
        public static ArrayVal ToArrayVal(this int[] val, FormatHint elemFmt, FormatHint arrayFmt) => new ArrayVal(val.ToValArray(elemFmt), arrayFmt);
        public static ArrayVal ToArrayVal(this long[] val, FormatHint elemFmt, FormatHint arrayFmt) => new ArrayVal(val.ToValArray(elemFmt), arrayFmt);
        public static ArrayVal ToArrayVal(this string[] val, FormatHint elemFmt, FormatHint arrayFmt) => new ArrayVal(val.ToValArray(elemFmt), arrayFmt);
        public static ArrayVal ToArrayVal(this real[] val) => new ArrayVal(val.ToValArray());
        public static ArrayVal ToArrayVal(this byte[] val) => new ArrayVal(val.ToValArray(FormatHint.CStyleHex));
        public static ArrayVal ToArrayVal(this int[] val) => new ArrayVal(val.ToValArray());
        public static ArrayVal ToArrayVal(this long[] val) => new ArrayVal(val.ToValArray());
        public static ArrayVal ToArrayVal(this string[] val) => new ArrayVal(val.ToValArray());

        public static real[] ToRealArray(this Val[] val) {
            var array = new real[val.Length];
            for (int i = 0; i < val.Length; i++) {
                array[i] = val[i].AsReal;
            }
            return array;
        }

        public static real[] ToRealArray(this string val) {
            var array = new real[val.Length];
            for (int i = 0; i < val.Length; i++) {
                array[i] = val[i];
            }
            return array;
        }
    }
}
