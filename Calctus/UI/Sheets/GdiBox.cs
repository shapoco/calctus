using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Shapoco.Calctus.UI.Sheets {
    class GdiBox : IDisposable {
        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;
        public event KeyPressEventHandler KeyPress;

        public readonly ObservableCollection<GdiBox> Children = new ObservableCollection<GdiBox>();
        private List<GdiBox> _tabOrderList = new List<GdiBox>();
        public GdiBox Parent { get; protected set; }

        private GdiControl _owner;
        private Rectangle _bounds;
        private bool _visible = true;
        private Color _backColor = Color.Transparent;
        private bool _disposed = false;
        public Cursor Cursor = Cursors.Default;

        public bool Focusable = false;

        public GdiBox(GdiControl owner) {
            _owner = owner;
            Children.CollectionChanged += Children_CollectionChanged;
        }

        ~GdiBox() {
            Dispose(false);
        }

        public GdiControl Owner => _owner;

        public void Invalidate() => _owner.Invalidate();

        public Rectangle Bounds {
            get => _bounds;
            set {
                if (value == _bounds) return;
                var oldSize = _bounds.Size;
                _bounds = value;
                if (value.Size != oldSize) OnResize();
                Invalidate();
            }
        }
        public Point Location {
            get => _bounds.Location;
            set {
                if (value == _bounds.Location) return;
                _bounds.Location = value;
                Invalidate();
            }
        }
        public Size Size {
            get => _bounds.Size;
            set {
                if (value == _bounds.Size) return;
                _bounds.Size = value;
                OnResize();
                Invalidate();
            }
        }
        public int Left {
            get => _bounds.X;
            set {
                if (value == _bounds.X) return;
                _bounds.X = value;
                Invalidate();
            }
        }
        public int Top {
            get => _bounds.Y;
            set {
                if (value == _bounds.Y) return;
                _bounds.Y = value;
                Invalidate();
            }
        }
        public int Right => _bounds.Right;
        public int Bottom => _bounds.Bottom;
        public int Width {
            get => _bounds.Width;
            set {
                if (value == _bounds.Width) return;
                _bounds.Width = value;
                OnResize();
                Invalidate();
            }
        }
        public int Height {
            get => _bounds.Height;
            set {
                if (value == _bounds.Height) return;
                _bounds.Height = value;
                OnResize();
                Invalidate();
            }
        }
        public void SetBounds(int l, int t, int w, int h) => Bounds = new Rectangle(l, t, w, h);
        protected virtual void OnResize() { }

        public bool Visible {
            get => _visible;
            set {
                if (value == _visible) return;
                _visible = value;
                Invalidate();
            }
        }

        public Rectangle ClientBounds => new Rectangle(Point.Empty, Size);

        public Point PointToScreen(Point point) {
            point.Offset(GetRootBounds().Location);
            return Owner.PointToScreen(point);
        }

        public Color BackColor {
            get => _backColor;
            set {
                if (value == _backColor) return;
                _backColor = value;
                Invalidate();
            }
        }

        public bool HitTest(Point offset, Point testPos, out GdiBox hitBox, out Rectangle hitBounds) {
            var testBounds = new Rectangle(offset, Size);
            if (_visible && testBounds.Contains(testPos)) {
                foreach (var child in Children) {
                    var childOffset = offset;
                    childOffset.Offset(child.Location);
                    if (child.HitTest(childOffset, testPos, out GdiBox childHitBox, out Rectangle childHitBounds)) {
                        hitBox = childHitBox;
                        hitBounds = childHitBounds;
                        return true;
                    }
                }
                hitBox = this;
                hitBounds = testBounds;
                return true;
            }
            hitBox = null;
            hitBounds = Rectangle.Empty;
            return false;
        }

        public Rectangle GetRootBounds() { 
            var bounds = Bounds;
            var p = Parent;
            while (p != null) {
                bounds.Offset(p.Bounds.Location);
                p = p.Parent;
            }
            return bounds;
        }

        public virtual Size GetPreferredSize() => Size.Empty;

        public void Focus() { if (Focusable) Focused = true; }
        public bool Focused {
            get => _owner.FocusedBox == this;
            set { if (Focusable) _owner.FocusedBox = this; }
        }

        public void PerformGotFocus() => OnGotFocus();
        public void PerformLostFocus() => OnLostFocus();
        protected virtual void OnGotFocus() { }
        protected virtual void OnLostFocus() { }

        public void PerformKeyDown(KeyEventArgs e) => OnKeyDown(e);
        public void PerformKeyUp(KeyEventArgs e) => OnKeyUp(e);
        public void PerformKeyPress(KeyPressEventArgs e) => OnKeyPress(e);
        public void PerformMouseDown(MouseEventArgs e) => OnMouseDown(e);
        public void PerformMouseMove(MouseEventArgs e) => OnMouseMove(e);
        public void PerformMouseUp(MouseEventArgs e) => OnMouseUp(e);
        public void PerformDoubleClick(EventArgs e) => OnDoubleClick(e);
        protected virtual void OnKeyDown(KeyEventArgs e) { KeyDown?.Invoke(this, e); }
        protected virtual void OnKeyUp(KeyEventArgs e) { KeyUp?.Invoke(this, e); }
        protected virtual void OnKeyPress(KeyPressEventArgs e) { KeyPress?.Invoke(this, e); }
        protected virtual void OnMouseDown(MouseEventArgs e) { }
        protected virtual void OnMouseMove(MouseEventArgs e) { }
        protected virtual void OnMouseUp(MouseEventArgs e) { }
        protected virtual void OnDoubleClick(EventArgs e) { }

        public void PerformPaint(PaintEventArgs e) {
            if (!_visible) return;
            if (_backColor.A != 0) {
                using (var brush = new SolidBrush(_backColor)) {
                    e.Graphics.FillRectangle(brush, ClientBounds);
                }
            }
            OnPaint(e);
        }
        protected virtual void OnPaint(PaintEventArgs e) {
#if DEBUG
            if (Control.ModifierKeys == Keys.Control) {
                e.Graphics.DrawRectangle(Pens.Blue, 0, 0, Width - 1, Height - 1);
            }
#endif
        }

        public virtual Point GetCursorPosition() => Point.Empty;

        public bool SelectNextBox(GdiBox child, bool forward) {
            int n = _tabOrderList.Count;
            int childIndex = -1;
            if (child != null) childIndex = _tabOrderList.IndexOf(child);
            int start;
            if (childIndex < 0) {
                start = forward ? 0 : n - 1;
            }
            else {
                start = forward ? childIndex + 1 : childIndex - 1;
            }

            if (forward) {
                for (int i = start; i < n; i++) {
                    var cand = _tabOrderList[i];
                    if (cand.Focusable && cand.Visible) {
                        cand.Focus();
                        return true;
                    }
                    else if (cand.SelectNextBox(null, forward)) {
                        return true;
                    }
                }
            }
            else {
                for (int i = start; i >= 0; i--) {
                    var cand = _tabOrderList[i];
                    if (cand.Focusable && cand.Visible) {
                        cand.Focus();
                        return true;
                    }
                    else if (cand.SelectNextBox(null, forward)) {
                        return true;
                    }
                }
            }

            if (Parent !=null) {
                return Parent.SelectNextBox(this, forward);
            }

            return false;
        }

        public int TabIndex {
            get {
                if (Parent == null) return 0;
                return Parent.getChildTabIndex(this);
            }
            set {
                if (value == TabIndex) return;
                Parent?.setChildTabIndex(this, value);
            }
        }
        protected int getChildTabIndex(GdiBox child) => _tabOrderList.IndexOf(child);
        protected void setChildTabIndex(GdiBox child, int newIndex) {
            _tabOrderList.Remove(child);
            _tabOrderList.Insert(newIndex, child);
        }

        public void Dispose() => Dispose(true);
        protected virtual void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    Children.Clear();
                }
                _disposed = true;
            }
        }

        private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Add) {
                foreach (var item in e.NewItems) {
                    var box = (GdiBox)item;
                    box.Parent = this;
                    _tabOrderList.Add(box);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove) {
                foreach (var item in e.OldItems) {
                    var box = (GdiBox)item;
                    box.Parent = null;
                    _tabOrderList.Remove(box);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace) {
                foreach (var item in e.OldItems) {
                    var box = (GdiBox)item;
                    box.Parent = null;
                    _tabOrderList.Remove(box);
                }
                foreach (var item in e.NewItems) {
                    var box = (GdiBox)item;
                    box.Parent = this;
                    _tabOrderList.Add(box);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset) {
                foreach(var item in _tabOrderList) {
                    item.Parent = null;
                }
                _tabOrderList.Clear();
            }
            Invalidate();
        }
    }
}
