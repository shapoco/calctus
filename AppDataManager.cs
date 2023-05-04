using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco {
    internal static class AppDataManager {
        public static string RoamingUserDataPath {
            get {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    Application.CompanyName + Path.DirectorySeparatorChar
                    + Application.ProductName + Path.DirectorySeparatorChar);
            }
        }

        public static string LocalUserDataPath {
            get {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    Application.CompanyName + Path.DirectorySeparatorChar
                    + Application.ProductName + Path.DirectorySeparatorChar);
            }
        }

        public static void SavePropertiesToRoamingAppData(object targetObject, string fileName) {
            StringBuilder sb = new StringBuilder();
            foreach (var p in targetObject.GetType().GetProperties()) {
                if (p.PropertyType.Equals(typeof(String))) {
                    // 文字列
                    var value = (string)p.GetValue(targetObject);
                    value = value.Replace(@"\", @"\b");
                    value = value.Replace("\r", @"\r");
                    value = value.Replace("\n", @"\n");
                    value = value.Replace("\0", @"\0");
                    value = value.Replace("\t", @"\t");
                    sb.AppendLine(p.Name + "=\"" + value + "\"");
                }
                else if (p.PropertyType.IsPrimitive || p.PropertyType.IsEnum || p.PropertyType.IsValueType) {
                    // プリミティブ型または列挙型
                    sb.AppendLine(p.Name + "=" + FormattableString.Invariant($"{p.GetValue(targetObject)}"));
                }
            }

            var filePath = Path.Combine(RoamingUserDataPath, fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            using (var writer = new StreamWriter(filePath)) {
                writer.Write(sb.ToString());
            }
        }

        public static void LoadPropertiesFromRoamingAppData(object targetObject, string fileName) {
            var dictionary = new Dictionary<string, string>();
            var filePath = Path.Combine(RoamingUserDataPath, fileName);
            if (!File.Exists(filePath)) return;

            // まず全部読み込む
            using (var reader = new StreamReader(filePath)) {
                while (!reader.EndOfStream) {
                    var line = reader.ReadLine();

                    // 空行とコメントは無視
                    if (string.IsNullOrEmpty(line)) continue;
                    if (line.StartsWith("#")) continue;

                    // 左辺と右辺に分解、分解できないものは無視
                    var args = line.Split(new char[] { '=' }, 2);
                    if (args.Length != 2) continue;

                    // 辞書に登録
                    var propName = args[0].Trim();
                    var propValue = args[1].Trim();
                    dictionary[propName] = propValue;
                }
            }

            foreach (var p in targetObject.GetType().GetProperties()) {
                if (!dictionary.ContainsKey(p.Name)) continue;
                var valueStr = dictionary[p.Name];

                try {
                    if (p.PropertyType.Equals(typeof(String))) {
                        // 文字列型
                        // "～" の形式でないものは無視
                        if (!valueStr.StartsWith("\"") || !valueStr.EndsWith("\"")) continue;
                        valueStr = valueStr.Substring(1, valueStr.Length - 2); // "～"の中身を取り出す
                        valueStr = valueStr.Replace(@"\r", "\r");
                        valueStr = valueStr.Replace(@"\n", "\n");
                        valueStr = valueStr.Replace(@"\0", "\0");
                        valueStr = valueStr.Replace(@"\t", "\t");
                        valueStr = valueStr.Replace(@"\b", @"\");
                        p.SetValue(targetObject, valueStr);
                    }
                    else if (p.PropertyType.Equals(typeof(Boolean))) { p.SetValue(targetObject, Boolean.Parse(valueStr)); }
                    else if (p.PropertyType.Equals(typeof(Byte))) { p.SetValue(targetObject, Byte.Parse(valueStr, CultureInfo.InvariantCulture)); }
                    else if (p.PropertyType.Equals(typeof(SByte))) { p.SetValue(targetObject, SByte.Parse(valueStr, CultureInfo.InvariantCulture)); }
                    else if (p.PropertyType.Equals(typeof(Int16))) { p.SetValue(targetObject, Int16.Parse(valueStr, CultureInfo.InvariantCulture)); }
                    else if (p.PropertyType.Equals(typeof(UInt16))) { p.SetValue(targetObject, UInt16.Parse(valueStr, CultureInfo.InvariantCulture)); }
                    else if (p.PropertyType.Equals(typeof(Int32))) { p.SetValue(targetObject, Int32.Parse(valueStr, CultureInfo.InvariantCulture)); }
                    else if (p.PropertyType.Equals(typeof(UInt32))) { p.SetValue(targetObject, UInt32.Parse(valueStr, CultureInfo.InvariantCulture)); }
                    else if (p.PropertyType.Equals(typeof(Int64))) { p.SetValue(targetObject, Int64.Parse(valueStr, CultureInfo.InvariantCulture)); }
                    else if (p.PropertyType.Equals(typeof(UInt64))) { p.SetValue(targetObject, UInt64.Parse(valueStr, CultureInfo.InvariantCulture)); }
                    else if (p.PropertyType.Equals(typeof(Char))) { p.SetValue(targetObject, Char.Parse(valueStr)); }
                    else if (p.PropertyType.Equals(typeof(Double))) { p.SetValue(targetObject, Double.Parse(valueStr, CultureInfo.InvariantCulture)); }
                    else if (p.PropertyType.Equals(typeof(Single))) { p.SetValue(targetObject, Single.Parse(valueStr, CultureInfo.InvariantCulture)); }
                    else if (p.PropertyType.Equals(typeof(Decimal))) { p.SetValue(targetObject, Decimal.Parse(valueStr, CultureInfo.InvariantCulture)); }
                    else if (p.PropertyType.IsEnum) { p.SetValue(targetObject, Enum.Parse(p.PropertyType, valueStr)); }
                    else {
                        throw new NotImplementedException($"Unsupported type {p.PropertyType}.");
                    }
                }
                catch (Exception ex) {
                    Console.Error.WriteLine(ex);
                }
            }

        }

    }
}
