using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Shapoco.Calctus.UI {
    public class FlatButton : Button {
        bool _hovered = false;
        bool _mouseDown = false;

        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);
            _hovered = true;
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            _hovered = false;
            this.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs mevent) {
            base.OnMouseDown(mevent);
            _mouseDown = true;
            this.Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs mevent) {
            base.OnMouseUp(mevent);
            _mouseDown = false;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pevent) {
            var g = pevent.Graphics;
            if (_mouseDown) {
                g.Clear(ColorUtils.Grayish(this.BackColor, 20));
            }
            else if (_hovered || this.Focused) {
                g.Clear(ColorUtils.Grayish(this.BackColor, 10));
            }
            else {
                g.Clear(this.BackColor);
            }

            using (var brush = new SolidBrush(this.ForeColor)) 
            using (var format = new StringFormat()) {
                format.LineAlignment = StringAlignment.Center;
                format.Alignment = StringAlignment.Center;
                g.DrawString(this.Text, this.Font, brush, this.ClientRectangle, format);
            }
        }
    }
}
