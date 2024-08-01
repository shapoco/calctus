using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Maths;


namespace Shapoco.Calctus.Model.Standards {
    static class PreferredNumbers {

        /// <summary>
        /// 最も分圧比が近くなる系列のペアを見つける
        /// </summary>
        public static decimal[] FindSplitPair(decimal[] series, decimal value) {
            if (value >= 1) {
                return new decimal[] { 1, 0 };
            }
            else if (value <= 0) {
                return new decimal[] { 0, 1 };
            }

            // ペアを見つける
            decimal min_diff = decimal.MaxValue;
            decimal min_lo = 0;
            decimal min_hi = 0;
            foreach (var lo in series) {
                decimal hi_floor, hi_ceil;
                FindNearests(series, lo / value - lo, out hi_floor, out hi_ceil);
                decimal diff_floor = Math.Abs(value - lo / (hi_floor + lo));
                decimal diff_ceil = Math.Abs(value - lo / (hi_ceil + lo));
                if (diff_floor < min_diff) {
                    min_diff = diff_floor;
                    min_lo = lo;
                    min_hi = hi_floor;
                }
                if (diff_ceil < min_diff) {
                    min_diff = diff_ceil;
                    min_lo = lo;
                    min_hi = hi_ceil;
                }
            }

            // 両方が 1 以上になるように桁合わせ
            int exp = (int)Math.Floor(MathEx.Log10(Math.Min(min_lo, min_hi)));
            if (exp < 0) {
                min_lo = Shift10(min_lo, -exp);
                min_hi = Shift10(min_hi, -exp);
            }

            return new decimal[] { min_lo, min_hi };
        }

        /// <summary>
        /// 系列で最も近い値のペアのうち小さい方を返す
        /// </summary>
        public static decimal Floor(decimal[] series, decimal value) {
            decimal floor, ceil;
            FindNearests(series, value, out floor, out ceil);
            return floor;
        }

        /// <summary>
        /// 系列で最も近い値のペアのうち大きい方を返す
        /// </summary>
        public static decimal Ceiling(decimal[] series, decimal value) {
            decimal floor, ceil;
            FindNearests(series, value, out floor, out ceil);
            return ceil;
        }

        /// <summary>
        /// 系列で最も近い値のペアのうち誤差が小さい方を返す
        /// </summary>
        public static decimal Round(decimal[] series, decimal value) {
            decimal floor, ceil;
            FindNearests(series, value, out floor, out ceil);
            if (value - floor < ceil - value) {
                return floor;
            }
            else {
                return ceil;
            }
        }

        /// <summary>
        /// 系列で最も近い値のペアを返す
        /// </summary>
        public static void FindNearests(decimal[] series, decimal value, out decimal floor, out decimal ceil) {
            var exp = (int)Math.Floor(MathEx.Log10(value));
            var key = Shift10(value, -exp);
            int i = BinarySearch(series, key);
            floor = Shift10(series[i], exp);
            if (floor == value) {
                ceil = floor;
            }
            else if (i < series.Length - 1) {
                ceil = Shift10(series[i + 1], exp);
            }
            else {
                ceil = Shift10(series[0], exp + 1);
            }
        }

        public static int BinarySearch(decimal[] series, decimal key) {
            return BinarySearch(series, key, 0, series.Length - 1);
        }

        public static int BinarySearch(decimal[] series, decimal key, int i0, int i1) {
            if (i0 == i1) {
                return i0;
            }
            else {
                int im = (i0 + i1) / 2 + 1;
                if (key < series[im]) {
                    return BinarySearch(series, key, i0, im - 1);
                }
                else {
                    return BinarySearch(series, key, im, i1);
                }
            }
        }

        public static decimal Shift10(decimal value, int exp) {
            if (exp >= 0) {
                return value * MathEx.Pow10(exp);
            }
            else {
                return value / MathEx.Pow10(-exp);
            }
        }

        public static void Test() {
            Test_RoundExact(ESeries.E3); Test_RoundUp(ESeries.E3); Test_RoundDown(ESeries.E3);
            Test_RoundExact(ESeries.E6); Test_RoundUp(ESeries.E6); Test_RoundDown(ESeries.E6);
            Test_RoundExact(ESeries.E24); Test_RoundUp(ESeries.E24); Test_RoundDown(ESeries.E24);
            Test_RoundExact(ESeries.E48); Test_RoundUp(ESeries.E48); Test_RoundDown(ESeries.E48);
            Test_RoundExact(ESeries.E96); Test_RoundUp(ESeries.E96); Test_RoundDown(ESeries.E96);
            Test_RoundExact(ESeries.E192); Test_RoundUp(ESeries.E192); Test_RoundDown(ESeries.E192);
            Test_Ratio();
        }

        public static void Test_RoundExact(decimal[] series) {
            for (int exp = 0; exp <= 27; exp++) {
                foreach (var e in series) {
                    decimal value = Shift10(e, exp);
                    Assert.Equal("Floor", value, Floor(series, value));
                    Assert.Equal("Ceiling", value, Ceiling(series, value));
                    Assert.Equal("Round", value, Round(series, value));
                }
            }
            for (int exp = 0; exp >= -26; exp--) {
                foreach (var e in series) {
                    decimal value = Shift10(e, exp);
                    Assert.Equal("Floor", value, Floor(series, value));
                    Assert.Equal("Ceiling", value, Ceiling(series, value));
                    Assert.Equal("Round", value, Round(series, value));
                }
            }
        }

        public static void Test_RoundDown(decimal[] series) {
            for (int exp = 0; exp <= 27; exp++) {
                foreach (var e in series) {
                    decimal value = Shift10(e, exp);
                    Assert.Equal("Floor", value, Floor(series, value * 1.001m));
                    Assert.Equal("Round", value, Round(series, value * 1.001m));
                }
            }
            for (int exp = 0; exp >= -26; exp--) {
                foreach (var e in series) {
                    decimal value = Shift10(e, exp);
                    Assert.Equal("Floor", value, Floor(series, value * 1.001m));
                    Assert.Equal("Round", value, Round(series, value * 1.001m));
                }
            }
        }

        public static void Test_RoundUp(decimal[] series) {
            for (int exp = 0; exp <= 27; exp++) {
                foreach (var e in series) {
                    decimal value = Shift10(e, exp);
                    Assert.Equal("Ceiling", value, Ceiling(series, value / 1.001m));
                    Assert.Equal("Round", value, Round(series, value / 1.001m));
                }
            }
            for (int exp = 0; exp >= -26; exp--) {
                foreach (var e in series) {
                    decimal value = Shift10(e, exp);
                    Assert.Equal("Ceiling", value, Ceiling(series, value / 1.001m));
                    Assert.Equal("Round", value, Round(series, value / 1.001m));
                }
            }
        }

        public static void Test_Ratio() {
            Assert.Equal("Ratio", FindSplitPair(ESeries.E6, 0m), new decimal[] { 0m, 1m });
            Assert.Equal("Ratio", FindSplitPair(ESeries.E6, 0.01m), new decimal[] { 1m, 100m });
            Assert.Equal("Ratio", FindSplitPair(ESeries.E6, 0.02m), new decimal[] { 6.8m, 330m });
            Assert.Equal("Ratio", FindSplitPair(ESeries.E6, 0.05m), new decimal[] { 3.3m, 68m });
            Assert.Equal("Ratio", FindSplitPair(ESeries.E6, 0.1m), new decimal[] { 1m, 10m });
            Assert.Equal("Ratio", FindSplitPair(ESeries.E6, 0.2m), new decimal[] { 1.5m, 6.8m });
            Assert.Equal("Ratio", FindSplitPair(ESeries.E6, 0.3m), new decimal[] { 6.8m, 15m });
            Assert.Equal("Ratio", FindSplitPair(ESeries.E6, 0.4m), new decimal[] { 1m, 1.5m });
            Assert.Equal("Ratio", FindSplitPair(ESeries.E6, 0.5m), new decimal[] { 1m, 1m });
            Assert.Equal("Ratio", FindSplitPair(ESeries.E6, 0.6m), new decimal[] { 1.5m, 1m });
            Assert.Equal("Ratio", FindSplitPair(ESeries.E6, 0.7m), new decimal[] { 15m, 6.8m });
            Assert.Equal("Ratio", FindSplitPair(ESeries.E6, 0.8m), new decimal[] { 6.8m, 1.5m });
            Assert.Equal("Ratio", FindSplitPair(ESeries.E6, 0.9m), new decimal[] { 10m, 1m });
            Assert.Equal("Ratio", FindSplitPair(ESeries.E6, 0.95m), new decimal[] { 68m, 3.3m });
            Assert.Equal("Ratio", FindSplitPair(ESeries.E6, 0.98m), new decimal[] { 330m, 6.8m });
            Assert.Equal("Ratio", FindSplitPair(ESeries.E6, 0.99m), new decimal[] { 100m, 1m });
            Assert.Equal("Ratio", FindSplitPair(ESeries.E6, 1m), new decimal[] { 1m, 0m });
        }
    }
}
