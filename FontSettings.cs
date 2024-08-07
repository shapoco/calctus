using System;
using System.Collections.Generic;
using System.Drawing;
using Shapoco.Texts;
using Shapoco.Maths;

namespace Shapoco {
    class FontSettings : ICloneable, IParsable {
        public const float MinSize = 1f;
        public const float MaxSize = 1000f;

        private string _face = SystemFonts.DefaultFont.Name;
        private float _size = SystemFonts.DefaultFont.Size;
        private FontStyle _style = FontStyle.Regular;
        private Font _fontCache = null;

        public FontSettings(string fontName, float size, FontStyle style) { 
            this._face = fontName;
            this._size = size;
            this._style = style;
        }

        public object Clone() => this.MemberwiseClone();

        public void CopyTo(FontSettings fs) {
            fs.Face = _face;
            fs.Size = _size;
            fs.Style = _style;
        }

        public bool TryParse(string s) {
            try {
                var tmp = (FontSettings)Clone();
                var lex = new SimpleLexer(s);
                lex.Pop("{");
                if (!lex.TryPop("}")) {
                    popProp(lex, tmp);
                    while (!lex.TryPop("}")) {
                        lex.Pop(",");
                        popProp(lex, tmp);
                    }
                    tmp.CopyTo(this);
                }
                lex.AssertEos();
                return true;
            }
            catch (Exception ex) {
                Log.Here().W(ex);
                return false;
            }
        }

        private void popProp(SimpleLexer lex, FontSettings tmp) {
            var propName = lex.PopId();
            lex.Pop(":");
            switch (propName) {
                case "face": tmp.Face = lex.PopCStyleString(); break;
                case "size": tmp.Size = (float)lex.PopUnsignedDecimal(); break;
                case "style": tmp.Style = (FontStyle)(int)lex.PopUnsignedDecimal(); break;
                default:
#if DEBUG
                    Console.WriteLine("*W: " + nameof(FontSettings) + "." + nameof(TryParse) + "() : Unrecognized property '" + propName + "'");
#endif
                    break;
            }
        }

        public override string ToString() {
            return "{face:\"" + CStyleEscaping.Escape(Face) + "\",size:" + Size + ",style:" + (int)Style + "}";
        }

        public string Face {
            get => _face;
            set => updateField(ref _face, string.IsNullOrEmpty(value) ? SystemFonts.DefaultFont.Name : value);
        }

        public float Size {
            get => _size;
            set => updateField(ref _size, MathEx.Clip(MinSize, MaxSize, value));
        }

        public FontStyle Style {
            get => _style;
            set => updateField(ref _style, value);
        }

        public bool Bold {
            get => _style.HasFlag(FontStyle.Bold);
            set => updateFlag(FontStyle.Bold, value);
        }

        public bool Italic {
            get => _style.HasFlag(FontStyle.Italic);
            set => updateFlag(FontStyle.Italic, value);
        }

        public bool Strikeout {
            get => _style.HasFlag(FontStyle.Strikeout);
            set => updateFlag(FontStyle.Strikeout, value);
        }

        public bool Underline {
            get => _style.HasFlag(FontStyle.Underline);
            set => updateFlag(FontStyle.Underline, value);
        }

        private void updateFlag(FontStyle flag, bool value) {
            if (value) {
                updateField(ref _style, _style | flag);
            }
            else {
                updateField(ref _style, _style & ~flag);
            }
        }

        private void updateField<T>(ref T field, T value ) {
            if (!field.Equals(value)) {
                field = value;
                _fontCache = null;
            }
        }

        public Font GetFontObject() {
            if (_fontCache == null) {
                if (tryCreateFont(_face, _size, _style, out _fontCache)) { }
                else if (tryCreateFont(SystemFonts.DefaultFont.Name, _size, _style, out _fontCache)) { }
                else _fontCache = SystemFonts.DefaultFont;
            }
            return _fontCache;
        }

        private static bool tryCreateFont(string name, float size, FontStyle style, out Font font) {
            try {
                font = new Font(name, size, style);
                return true;
            }
            catch (Exception ex) {
                Log.Here().W(ex);
                font = null;
                return false;
            }
        }
    }
}
