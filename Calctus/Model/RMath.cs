using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model {
    /// <summary>real に対する数学関数の定義</summary>
    static class RMath {
        // 指数関数
        // 暫定的に System.Math の関数を使用する
        public static real Pow(real a, real b) => (real)Math.Pow((double)a.Raw, (double)b.Raw);
        public static real Sqrt(real a) => (real)Math.Sqrt((double)a.Raw);
        public static real Log(real a) => (real)Math.Log((double)a.Raw);
        public static real Log10(real a) => (real)Math.Log10((double)a.Raw);
        
        /// <summary>
        /// 10のe乗を返す。
        /// 数値を文字列に変換する際に桁合わせのために使用する。
        /// Math.Pow() をそのまま使用すると double の仮数部のビット数が足りずに誤差が出るため、10進10桁ずつに分割して計算する。
        /// </summary>
        public static real Pow10(real e) {
            int stride = 10;
            real coeff = (real)Math.Pow(10, stride);
            real pow = 1m;
            if (e >= 0) {
                while (e >= stride) {
                    pow *= coeff;
                    e -= stride;
                }
                pow *= (real)Math.Pow(10, (double)e);
            }
            else {
                while (e <= -stride) {
                    pow /= coeff;
                    e += stride;
                }
                pow /= (real)Math.Pow(10, -(double)e);
            }
            return pow;
        }

        // 三角関数
        // 暫定的に System.Math の関数を使用する
        public static real Sin(real a) => (real)Math.Sin((double)a.Raw);
        public static real Cos(real a) => (real)Math.Cos((double)a.Raw);
        public static real Tan(real a) => (real)Math.Tan((double)a.Raw);
        public static real Asin(real a) => (real)Math.Asin((double)a.Raw);
        public static real Acos(real a) => (real)Math.Acos((double)a.Raw);
        public static real Atan(real a) => (real)Math.Atan((double)a.Raw);
        public static real Atan2(real a, real b) => (real)Math.Atan2((double)a.Raw, (double)b.Raw);

        // 丸め関数
        public static real Floor(real val) => (real)Math.Floor(val.Raw);
        public static real Ceiling(real val) => (real)Math.Ceiling(val.Raw);
        public static real Truncate(real val) => (real)Math.Truncate(val.Raw);
        public static real Round(real val) => (real)Math.Round(val.Raw);
        
        // 絶対値と符号
        public static real Abs(real val) => val >= 0 ? val : -val;
        public static int Sign(real val) => Math.Sign(val.Raw);

        // 最大最小
        public static real Max(real a, real b) => a > b ? a : b;
        public static real Min(real a, real b) => a < b ? a : b;
    }
}
