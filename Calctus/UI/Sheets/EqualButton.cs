using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Shapoco.Calctus.UI.Sheets {
    class EqualButton : GdiBox {
        private SheetView _view;

        public EqualButton(SheetView view) : base(view) {
            _view = view;
            //Cursor = Cursors.SizeWE;
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            var g = e.Graphics;
            using (var brush = new SolidBrush(ExprBoxCoreLayout.SymbolColor))
            using (var sf = new StringFormat()) {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                g.DrawString("=", Owner.Font, brush, ClientBounds, sf);
            }
        }
    }
}
