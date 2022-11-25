using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Standard {
    static class PreferredNumbers {

        /// <summary>
        /// 最も分圧比が近くなる系列のペアを見つける
        /// </summary>
        public static real[] FindSplitPair(real[] series, real value) {
            if (value >= 1) {
                return new real[] { 1, 0 };
            }
            else if (value <= 0) {
                return new real[] { 0, 1 };
            }

            // ペアを見つける
            real min_diff = real.MaxValue;
            real min_lo = 0;
            real min_hi = 0;
            foreach (var lo in series) {
                real hi_floor, hi_ceil;
                FindNearests(series, lo / value - lo, out hi_floor, out hi_ceil);
                real diff_floor = RMath.Abs(value - lo / (hi_floor + lo));
                real diff_ceil = RMath.Abs(value - lo / (hi_ceil + lo));
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
            int exp = (int)RMath.Floor(RMath.Log10(RMath.Min(min_lo, min_hi)));
            if (exp < 0) {
                min_lo = Shift10(min_lo, -exp);
                min_hi = Shift10(min_hi, -exp);
            }

            return new real[] { min_lo, min_hi };
        }

        /// <summary>
        /// 系列で最も近い値のペアのうち小さい方を返す
        /// </summary>
        public static real Floor(real[] series, real value) {
            real floor, ceil;
            FindNearests(series, value, out floor, out ceil);
            return floor;
        }

        /// <summary>
        /// 系列で最も近い値のペアのうち大きい方を返す
        /// </summary>
        public static real Ceiling(real[] series, real value) {
            real floor, ceil;
            FindNearests(series, value, out floor, out ceil);
            return ceil;
        }

        /// <summary>
        /// 系列で最も近い値のペアのうち誤差が小さい方を返す
        /// </summary>
        public static real Round(real[] series, real value) {
            real floor, ceil;
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
        public static void FindNearests(real[] series, real value, out real floor, out real ceil) {
            var exp = (int)RMath.Floor(RMath.Log10(value));
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

        public static int BinarySearch(real[] series, real key) {
            return BinarySearch(series, key, 0, series.Length - 1);
        }

        public static int BinarySearch(real[] series, real key, int i0, int i1) {
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

        public static real Shift10(real value, int exp) {
            if (exp >= 0) {
                return value * RMath.Pow10(exp);
            }
            else {
                return value / RMath.Pow10(-exp);
            }
        }

        public static void Test() {
            Test_RoundExact(Eseries.E3); Test_RoundUp(Eseries.E3); Test_RoundDown(Eseries.E3);
            Test_RoundExact(Eseries.E6); Test_RoundUp(Eseries.E6); Test_RoundDown(Eseries.E6);
            Test_RoundExact(Eseries.E24); Test_RoundUp(Eseries.E24); Test_RoundDown(Eseries.E24);
            Test_RoundExact(Eseries.E48); Test_RoundUp(Eseries.E48); Test_RoundDown(Eseries.E48);
            Test_RoundExact(Eseries.E96); Test_RoundUp(Eseries.E96); Test_RoundDown(Eseries.E96);
            Test_RoundExact(Eseries.E192); Test_RoundUp(Eseries.E192); Test_RoundDown(Eseries.E192);
            Test_Ratio();
        }

        public static void Test_RoundExact(real[] series) {
            for (int exp = 0; exp <= 27; exp++) {
                foreach (var e in series) {
                    real value = Shift10(e, exp);
                    Assert.Equal("Floor", value, Floor(series, value));
                    Assert.Equal("Ceiling", value, Ceiling(series, value));
                    Assert.Equal("Round", value, Round(series, value));
                }
            }
            for (int exp = 0; exp >= -26; exp--) {
                foreach (var e in series) {
                    real value = Shift10(e, exp);
                    Assert.Equal("Floor", value, Floor(series, value));
                    Assert.Equal("Ceiling", value, Ceiling(series, value));
                    Assert.Equal("Round", value, Round(series, value));
                }
            }
        }

        public static void Test_RoundDown(real[] series) {
            for (int exp = 0; exp <= 27; exp++) {
                foreach (var e in series) {
                    real value = Shift10(e, exp);
                    Assert.Equal("Floor", value, Floor(series, value * 1.001m));
                    Assert.Equal("Round", value, Round(series, value * 1.001m));
                }
            }
            for (int exp = 0; exp >= -26; exp--) {
                foreach (var e in series) {
                    real value = Shift10(e, exp);
                    Assert.Equal("Floor", value, Floor(series, value * 1.001m));
                    Assert.Equal("Round", value, Round(series, value * 1.001m));
                }
            }
        }

        public static void Test_RoundUp(real[] series) {
            for (int exp = 0; exp <= 27; exp++) {
                foreach (var e in series) {
                    real value = Shift10(e, exp);
                    Assert.Equal("Ceiling", value, Ceiling(series, value / 1.001m));
                    Assert.Equal("Round", value, Round(series, value / 1.001m));
                }
            }
            for (int exp = 0; exp >= -26; exp--) {
                foreach (var e in series) {
                    real value = Shift10(e, exp);
                    Assert.Equal("Ceiling", value, Ceiling(series, value / 1.001m));
                    Assert.Equal("Round", value, Round(series, value / 1.001m));
                }
            }
        }

        public static void Test_Ratio() {
            Assert.Equal("Ratio", FindSplitPair(Eseries.E6, 0m), new real[] { 0, 1 });
            Assert.Equal("Ratio", FindSplitPair(Eseries.E6, 0.01m), new real[] { 1, 100 });
            Assert.Equal("Ratio", FindSplitPair(Eseries.E6, 0.02m), new real[] { 6.8, 330 });
            Assert.Equal("Ratio", FindSplitPair(Eseries.E6, 0.05m), new real[] { 3.3, 68 });
            Assert.Equal("Ratio", FindSplitPair(Eseries.E6, 0.1m), new real[] { 1, 10 });
            Assert.Equal("Ratio", FindSplitPair(Eseries.E6, 0.2m), new real[] { 1.5, 6.8 });
            Assert.Equal("Ratio", FindSplitPair(Eseries.E6, 0.3m), new real[] { 6.8, 15 });
            Assert.Equal("Ratio", FindSplitPair(Eseries.E6, 0.4m), new real[] { 1, 1.5 });
            Assert.Equal("Ratio", FindSplitPair(Eseries.E6, 0.5m), new real[] { 1, 1 });
            Assert.Equal("Ratio", FindSplitPair(Eseries.E6, 0.6m), new real[] { 1.5, 1 });
            Assert.Equal("Ratio", FindSplitPair(Eseries.E6, 0.7m), new real[] { 15, 6.8 });
            Assert.Equal("Ratio", FindSplitPair(Eseries.E6, 0.8m), new real[] { 6.8, 1.5 });
            Assert.Equal("Ratio", FindSplitPair(Eseries.E6, 0.9m), new real[] { 10, 1 });
            Assert.Equal("Ratio", FindSplitPair(Eseries.E6, 0.95m), new real[] { 68, 3.3 });
            Assert.Equal("Ratio", FindSplitPair(Eseries.E6, 0.98m), new real[] { 330, 6.8 });
            Assert.Equal("Ratio", FindSplitPair(Eseries.E6, 0.99m), new real[] { 100, 1 });
            Assert.Equal("Ratio", FindSplitPair(Eseries.E6, 1m), new real[] { 1, 0 });
        }
    }
}
