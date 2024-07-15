using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class EncodingFuncs : BuiltInFuncCategory {
        private static EncodingFuncs _instance = null;
        public static EncodingFuncs Instance => _instance != null ? _instance : _instance = new EncodingFuncs();
        private EncodingFuncs() { }

        public readonly BuiltInFuncDef utf8Enc = new BuiltInFuncDef("utf8Enc(str)",
            "Encode `str` to UTF8 byte sequence.",
            (e, a) => Encoding.UTF8.GetBytes(a[0].AsString).ToArrayVal());

        public readonly BuiltInFuncDef utf8Dec = new BuiltInFuncDef("utf8Dec(bytes[]...)",
            "Decode UTF8 byte sequence.",
            (e, a) => Encoding.UTF8.GetString(a[0].AsByteArray).ToStrVal());

        public readonly BuiltInFuncDef urlEnc = new BuiltInFuncDef("urlEnc(str)",
            "Escape URL string.",
            (e, a) => System.Web.HttpUtility.UrlEncode(a[0].AsString).ToStrVal());

        public readonly BuiltInFuncDef urlDec = new BuiltInFuncDef("urlDec(str)",
            "Unescape URL string.",
            (e, a) => System.Web.HttpUtility.UrlDecode(a[0].AsString).ToStrVal());

        public readonly BuiltInFuncDef base64Enc = new BuiltInFuncDef("base64Enc(str)",
            "Encode string to Base64.",
            (e, a) => Convert.ToBase64String(Encoding.UTF8.GetBytes(a[0].AsString)).ToStrVal());

        public readonly BuiltInFuncDef base64Dec = new BuiltInFuncDef("base64Dec(str)",
            "Decode Base64 to string.",
            (e, a) => Encoding.UTF8.GetString(Convert.FromBase64String(a[0].AsString)).ToStrVal());

        public readonly BuiltInFuncDef base64EncBytes = new BuiltInFuncDef("base64EncBytes(bytes[]...)",
            "Encode byte-array to Base64.",
            (e, a) => Convert.ToBase64String(a[0].AsByteArray).ToStrVal());

        public readonly BuiltInFuncDef base64DecBytes = new BuiltInFuncDef("base64DecBytes(str)",
            "Decode Base64 to byte-array.",
            (e, a) => Convert.FromBase64String(a[0].AsString).ToArrayVal());
    }
}
