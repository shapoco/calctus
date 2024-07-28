using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model;
using Shapoco.Calctus.Model.Maths;

namespace Shapoco.Calctus {
    public static class Assert {
        public static void Fail(string msg) {
            Console.Error.WriteLine("TEST FAILD!! : " + msg);
        }

        public static void Equal<T>(string subject, T a, T b) {
            if (!a.Equals(b)) {
                Fail(subject + " : " + a + " != " + b);
            }
        }

        public static void Equal<T>(string subject, T[] a, T[] b) {
            if (a.Length != b.Length) {
                Fail(subject + " : a.Length(" + a.Length + ") != b.Length(" + b.Length + ")");
                return;
            }
            for (int i = 0; i < a.Length; i++) {
                if (!a[i].Equals(b[i])) {
                    Fail(subject + " : a[" + i + "](" + a[i] + ") != b[" + i + "](" + b[i] + ")");
                }
            }
        }

        public static void ArgInRange(string funcName, string argName, bool cond, string msg = null) {
            if (!cond) throw new CalctusArgError(funcName, argName, msg);
        }
        public static void ArgInRange(string funcName, bool cond, string msg = null)
            => ArgInRange(funcName, null, cond, msg);
        
        public static void AssertArgNonEmpty(string funcName, Array array)
            => AssertArgNonEmpty(funcName, null, array);
        public static void AssertArgNonEmpty(string funcName, string argName, Array array)
            => ArgInRange(funcName, argName, array.Length > 0, "Empty array");

        public static void ArgIsInteger(string funcName, decimal value)
            => ArgIsInteger(funcName, null, value);
        public static void ArgIsInteger(string funcName, string argName, decimal value)
            => ArgInRange(funcName, argName, value.IsInteger(), "Not integer");
    }

    public class TestFailedException : Exception {
        public TestFailedException(string msg) : base(msg) { } 
    }
}
