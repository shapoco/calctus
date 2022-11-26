using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model;

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

    }

    public class TestFailedException : Exception {
        public TestFailedException(string msg) : base(msg) { } 
    }
}
