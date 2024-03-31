using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class EncodingFuncs {
        public static readonly BuiltInFuncDef utf8Enc = new BuiltInFuncDef("utf8Enc(str)", (e, a) => new ArrayVal(Encoding.UTF8.GetBytes(a[0].AsString)), "Encode `str` to UTF8 byte sequence.");
        public static readonly BuiltInFuncDef utf8Dec = new BuiltInFuncDef("utf8Dec(bytes[]...)", (e, a) => new StrVal(Encoding.UTF8.GetString(a[0].AsByteArray)), "Decode UTF8 byte sequence.");

        public static readonly BuiltInFuncDef urlEnc = new BuiltInFuncDef("urlEnc(str)", (e, a) => new StrVal(System.Web.HttpUtility.UrlEncode(a[0].AsString)), "Escape URL string.");
        public static readonly BuiltInFuncDef urlDec = new BuiltInFuncDef("urlDec(str)", (e, a) => new StrVal(System.Web.HttpUtility.UrlDecode(a[0].AsString)), "Unescape URL string.");

        public static readonly BuiltInFuncDef base64Enc = new BuiltInFuncDef("base64Enc(str)", (e, a) => new StrVal(Convert.ToBase64String(Encoding.UTF8.GetBytes(a[0].AsString))), "Encode string to Base64.");
        public static readonly BuiltInFuncDef base64Dec = new BuiltInFuncDef("base64Dec(str)", (e, a) => new StrVal(Encoding.UTF8.GetString(Convert.FromBase64String(a[0].AsString))), "Decode Base64 to string.");
        public static readonly BuiltInFuncDef base64EncBytes = new BuiltInFuncDef("base64EncBytes(bytes[]...)", (e, a) => new StrVal(Convert.ToBase64String(a[0].AsByteArray)), "Encode byte-array to Base64.");
        public static readonly BuiltInFuncDef base64DecBytes = new BuiltInFuncDef("base64DecBytes(str)", (e, a) => new ArrayVal(Convert.FromBase64String(a[0].AsString)), "Decode Base64 to byte-array.");

    }
}
