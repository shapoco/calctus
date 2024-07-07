using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Shapoco.Calctus.UI {
    static class ToolIconGenerator {
        public static Image GenerateToolIcon(Size size, Image image) {
            var darkMode = Settings.Instance.GetIsDarkMode();
            if (!darkMode && size == image.Size) {
                return image;
            }
            else {
                return CreateNegativeImage(size, darkMode, image);
            }
        }

        // 色を反転させた画像（ネガティブイメージ）を表示する - .NET Tips (VB.NET,C#...)
        // https://dobon.net/vb/dotnet/graphics/drawnegativeimage.html
        /// <summary>
        /// 指定された画像からネガティブイメージを作成する
        /// </summary>
        /// <param name="origImage">基の画像</param>
        /// <returns>作成されたネガティブイメージ</returns>
        public static Image CreateNegativeImage(Size newSize, bool invert, Image origImage) {
            //ネガティブイメージの描画先となるImageオブジェクトを作成
            Bitmap newBmp = new Bitmap(newSize.Width, newSize.Height);

            //negaImgのGraphicsオブジェクトを取得
            using (var g = Graphics.FromImage(newBmp)) {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

                if (invert) {
                    //ColorMatrixオブジェクトの作成
                    System.Drawing.Imaging.ColorMatrix cm =
                        new System.Drawing.Imaging.ColorMatrix();
                    //ColorMatrixの行列の値を変更して、色が反転されるようにする
                    cm.Matrix00 = -1;
                    cm.Matrix11 = -1;
                    cm.Matrix22 = -1;
                    cm.Matrix33 = 1;
                    cm.Matrix40 = cm.Matrix41 = cm.Matrix42 = cm.Matrix44 = 1;

                    //ImageAttributesオブジェクトの作成
                    System.Drawing.Imaging.ImageAttributes ia =
                        new System.Drawing.Imaging.ImageAttributes();
                    //ColorMatrixを設定する
                    ia.SetColorMatrix(cm);

                    //ImageAttributesを使用して色が反転した画像を描画
                    g.DrawImage(origImage,
                        new Rectangle(0, 0, newSize.Width, newSize.Height),
                        0, 0, origImage.Width, origImage.Height, GraphicsUnit.Pixel, ia);
                }
                else {
                    g.DrawImage(origImage,
                        new Rectangle(0, 0, newSize.Width, newSize.Height),
                        0, 0, origImage.Width, origImage.Height, GraphicsUnit.Pixel);
                }
            }

            return newBmp;
        }
    }

}
