using System.ComponentModel;

namespace Jiiks.WinForms.Controls;

internal partial class ResizablePanel : Panel {

    protected override void InitLayout() {
        if (KeepAspectCornerOnly) {
            TopResize = false;
            RightResize = false;
            BottomResize = false;
            LeftResize = false;
        }
        base.InitLayout();
    }

    private enum ResizeMode {
        None,
        ResizeTopLeft,
        ResizeTop,
        ResizeTopRight,
        ResizeRight,
        ResizeBottomRight,
        ResizeBottom,
        ResizeBottomLeft,
        ResizeLeft
    }

    private bool _mouseDown;
    private ResizeMode _resizeMode;
    private Point _startPoint;
    private Size _startSize;

    protected override void OnMouseDown(MouseEventArgs e) {
        var ptc = PointToClient(Cursor.Position);
        _startPoint = new Point(ptc.X + Location.X, ptc.Y + Location.Y);
        _startSize = Size;
        _mouseDown = true;
        base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseEventArgs e) {
        _mouseDown = false;
        base.OnMouseUp(e);
    }

    protected override void OnMouseMove(MouseEventArgs e) {
        ChangeCursor(e);
        if(_mouseDown) {
            var newSize = Size;
            var newLoc = Location;
            var ptc = PointToClient(Cursor.Position);
            switch (_resizeMode) {
                case ResizeMode.ResizeTopLeft:
                    if (!TopLeftResize) return;
                    newLoc = new Point(ptc.X + Location.X, ptc.Y + Location.Y);
                    newSize = new Size(_startSize.Width - newLoc.X + _startPoint.X, _startSize.Height - newLoc.Y + _startPoint.Y);
                    break;
                case ResizeMode.ResizeTop:
                    if (!TopResize) return;
                    newLoc = new Point(Location.X, ptc.Y + Location.Y);
                    newSize = new Size(Width, _startSize.Height - newLoc.Y + _startPoint.Y);
                    break;
                case ResizeMode.ResizeTopRight:
                    if (!TopRightResize) return;
                    newLoc = new Point(Location.X, ptc.Y + Location.Y);
                    newSize = new Size(ptc.X, _startSize.Height - newLoc.Y + _startPoint.Y);
                    break;
                case ResizeMode.ResizeRight:
                    if (!RightResize) return;
                    newSize = new Size(ptc.X, Height);
                    break;
                case ResizeMode.ResizeBottomRight:
                    if (!BottomRightResize) return;
                    newSize = new Size(ptc.X, ptc.Y);
                    break;
                case ResizeMode.ResizeBottom:
                    if (!BottomResize) return;
                    newSize = new Size(Width, ptc.Y);
                    break;
                case ResizeMode.ResizeBottomLeft:
                    if (!BottomLeftResize) return;
                    newLoc = new Point(ptc.X + Location.X, Location.Y);
                    newSize = new Size(_startSize.Width - newLoc.X + _startPoint.X, ptc.Y);
                    break;
                case ResizeMode.ResizeLeft:
                    if (!LeftResize) return;
                    newLoc = new Point(ptc.X + Location.X, Location.Y);
                    newSize = new Size(_startSize.Width - newLoc.X + _startPoint.X, Height);
                    break;
            }

            Size = newSize;
            Location = newLoc;
            Refresh();
        }

        base.OnMouseMove(e);
    }

    protected override void OnMouseLeave(EventArgs e) {
        Cursor = NormalCursor;
        base.OnMouseLeave(e);
    }

    private void ChangeCursor(MouseEventArgs e) {

        if (e.Location.X < ResizeBorderSize) {
            // Top left corner
            if (TopLeftResize && e.Location.Y < ResizeBorderSize) {
                Cursor = Cursors.SizeNWSE;
                _resizeMode = ResizeMode.ResizeTopLeft;
                return;
            }

            // Bottom left corner
            if (BottomLeftResize && e.Location.Y > Height - ResizeBorderSize) {
                Cursor = Cursors.SizeNESW;
                _resizeMode = ResizeMode.ResizeBottomLeft;
                return;
            }

            if (LeftResize) { 
                // Left edge
                Cursor = Cursors.SizeWE;
                _resizeMode = ResizeMode.ResizeLeft;
                return;
            }
        }

        if (e.Location.X > Width - ResizeBorderSize) {
            // Top right corner
            if(TopRightResize && e.Location.Y < ResizeBorderSize) {
                Cursor = Cursors.SizeNESW;
                _resizeMode = ResizeMode.ResizeTopRight;
                return;
            }

            // Bottom right corner
            if (BottomLeftResize && e.Location.Y > Height - ResizeBorderSize) {
                Cursor = Cursors.SizeNWSE;
                _resizeMode = ResizeMode.ResizeBottomRight;
                return;
            }
        }

        if (BottomResize && e.Location.Y > Height - ResizeBorderSize) {
            // Bottom edge
            Cursor = Cursors.SizeNS;
            _resizeMode = ResizeMode.ResizeBottom;
            return;
        }

        if(TopResize && e.Location.Y < ResizeBorderSize) {
            // Top edge
            Cursor = Cursors.SizeNS;
            _resizeMode = ResizeMode.ResizeTop;
            return;
        }

        if(RightResize && e.Location.X > Width - ResizeBorderSize) {
            // Right edge
            Cursor = Cursors.SizeWE;
            _resizeMode = ResizeMode.ResizeRight;
            return;
        }

        Cursor = NormalCursor;
        _resizeMode = ResizeMode.None;

    }

    protected override void OnPaint(PaintEventArgs e) {
        if(RBorderStyle != ButtonBorderStyle.None)
            ControlPaint.DrawBorder(
                    e.Graphics,
                    ClientRectangle,
                    BorderColour,
                    BorderWidth,
                    RBorderStyle,
                    BorderColour,
                    BorderWidth,
                    RBorderStyle,
                    BorderColour,
                    BorderWidth,
                    RBorderStyle,
                    BorderColour,
                    BorderWidth,
                    RBorderStyle);

        base.OnPaint(e);
    }

}


internal partial class ResizablePanel {
    [Description("Resize border size")]
    public int ResizeBorderSize { get; set; } = 10;
    [Description("Default Cursor")]
    public Cursor NormalCursor { get; set; } = Cursors.Default;
    [Description("Border Colour")]
    public Color BorderColour { get; set; } = Color.Black;
    [Description("Border Style")]
    public ButtonBorderStyle RBorderStyle { get; set; } = ButtonBorderStyle.Solid;
    [Description("Border Width")]
    public int BorderWidth { get; set; } = 1;
    [Description("Keep Aspect")]
    public bool KeepAspectCornerOnly { get; set; } = false;
    public Size Aspect { get; set; } = new Size(16, 9);

    [Category("Resize Flags")]
    public bool TopLeftResize { get; set; } = true;
    [Category("Resize Flags")]
    public bool TopResize { get; set; } = true;
    [Category("Resize Flags")]
    public bool TopRightResize { get; set; } = true;
    [Category("Resize Flags")]
    public bool RightResize { get; set; } = true;
    [Category("Resize Flags")]
    public bool BottomRightResize { get; set; } = true;
    [Category("Resize Flags")]
    public bool BottomResize { get; set; } = true;
    [Category("Resize Flags")]
    public bool BottomLeftResize { get; set; } = true;
    [Category("Resize Flags")]
    public bool LeftResize { get; set; } = true;
}
