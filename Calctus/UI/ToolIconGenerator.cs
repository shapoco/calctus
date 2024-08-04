using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Shapoco.Calctus.UI {
    static class ToolIconGenerator {
        private static readonly Dictionary<string, Image> _cache = new Dictionary<string, Image>();
        private static readonly Type _typeOfResources = typeof(Properties.Resources);

        public static Image GenerateToolIcon(Size size, string resourceName) {
            var origImage = (Bitmap)_typeOfResources.GetProperty(resourceName).GetValue(null);
            var darkMode = Settings.Instance.Appearance_IsDarkTheme;

            if (!darkMode && size == origImage.Size) {
                return origImage;
            }

            var cacheKey = resourceName + (darkMode ? "-dark" : "-white");

            Image retBmp = origImage;
            if (_cache.TryGetValue(cacheKey, out Image cacheBmp) && cacheBmp.Size == size) {
                retBmp = cacheBmp;
            }
            else {
                _cache[cacheKey] = retBmp = CreateScaledImage(size, darkMode, origImage);
            }
            return retBmp;
        }

        public static Image CreateScaledImage(Size newSize, bool invert, Image orig) {
            Bitmap newBmp;
            if (newSize == orig.Size) {
                newBmp = new Bitmap(orig);
            }
            else {
                // resize
                newBmp = new Bitmap(newSize.Width, newSize.Height);
                using (var g = Graphics.FromImage(newBmp)) {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                    g.DrawImage(orig,
                        new Rectangle(0, 0, newSize.Width, newSize.Height), 
                        0, 0, orig.Width, orig.Height, GraphicsUnit.Pixel);
                }
            }

            if (invert) {
                // invert color
                var bmpData = newBmp.LockBits(new Rectangle(Point.Empty, newSize),
                    ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                var offset = bmpData.Scan0;
                for (int y = 0; y < newSize.Height; y++) {
                    var p = offset;
                    for (int x = 0; x < newSize.Width; x++) {
                        var c = Marshal.ReadInt32(p);
                        Marshal.WriteInt32(p, c ^ 0xffffff);
                        p += 4;
                    }
                    offset += bmpData.Stride;
                }
                newBmp.UnlockBits(bmpData);
            }

            return newBmp;
        }
    }

}
