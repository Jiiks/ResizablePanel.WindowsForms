internal class ResizablePanel : Panel {

    [Description("Resize border size")]
    public int ResizeBorderSize { get; set; } = 10;
    [Description("Default Cursor")]
    public Cursor NormalCursor { get; set; } = Cursors.Default;

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
                    newLoc = new Point(ptc.X + Location.X, ptc.Y + Location.Y);
                    newSize = new Size(_startSize.Width - newLoc.X + _startPoint.X, _startSize.Height - newLoc.Y + _startPoint.Y);
                    break;
                case ResizeMode.ResizeTop:
                    newLoc = new Point(Location.X, ptc.Y + Location.Y);
                    newSize = new Size(Width, _startSize.Height - newLoc.Y + _startPoint.Y);
                    break;
                case ResizeMode.ResizeTopRight:
                    newLoc = new Point(Location.X, ptc.Y + Location.Y);
                    newSize = new Size(ptc.X, _startSize.Height - newLoc.Y + _startPoint.Y);
                    break;
                case ResizeMode.ResizeRight:
                    newSize = new Size(ptc.X, Height);
                    break;
                case ResizeMode.ResizeBottomRight:
                    newSize = new Size(ptc.X, ptc.Y);
                    break;
                case ResizeMode.ResizeBottom:
                    newSize = new Size(Width, ptc.Y);
                    break;
                case ResizeMode.ResizeBottomLeft:
                    newLoc = new Point(ptc.X + Location.X, Location.Y);
                    newSize = new Size(_startSize.Width - newLoc.X + _startPoint.X, ptc.Y);
                    break;
                case ResizeMode.ResizeLeft:
                    newLoc = new Point(ptc.X + Location.X, Location.Y);
                    newSize = new Size(_startSize.Width - newLoc.X + _startPoint.X, Height);
                    break;
            }

            Size = newSize;
            Location = newLoc;
        }

        base.OnMouseMove(e);
    }

    protected override void OnMouseLeave(EventArgs e) {
        Cursor = Cursors.Default;
        base.OnMouseLeave(e);
    }

    private void ChangeCursor(MouseEventArgs e) {

        if(e.Location.X < ResizeBorderSize) {
            // Top left corner
            if (e.Location.Y < ResizeBorderSize) {
                Cursor = Cursors.SizeNWSE;
                _resizeMode = ResizeMode.ResizeTopLeft;
                return;
            }

            // Bottom left corner
            if(e.Location.Y > Height - ResizeBorderSize) {
                Cursor = Cursors.SizeNESW;
                _resizeMode = ResizeMode.ResizeBottomLeft;
                return;
            }

            // Left edge
            Cursor = Cursors.SizeWE;
            _resizeMode = ResizeMode.ResizeLeft;
            return;
        }

        if (e.Location.X > Width - ResizeBorderSize) {
            // Top right corner
            if(e.Location.Y < ResizeBorderSize) {
                Cursor = Cursors.SizeNESW;
                _resizeMode = ResizeMode.ResizeTopRight;
                return;
            }

            // Bottom right corner
            if (e.Location.Y > Height - ResizeBorderSize) {
                Cursor = Cursors.SizeNWSE;
                _resizeMode = ResizeMode.ResizeBottomRight;
                return;
            }
        }

        if (e.Location.Y > Height - ResizeBorderSize) {
            // Bottom edge
            Cursor = Cursors.SizeNS;
            _resizeMode = ResizeMode.ResizeBottom;
            return;
        }

        if(e.Location.Y < ResizeBorderSize) {
            // Top edge
            Cursor = Cursors.SizeNS;
            _resizeMode = ResizeMode.ResizeTop;
            return;
        }

        if(e.Location.X > Width - ResizeBorderSize) {
            // Right edge
            Cursor = Cursors.SizeWE;
            _resizeMode = ResizeMode.ResizeRight;
            return;
        }

        Cursor = NormalCursor;
        _resizeMode = ResizeMode.None;

    }

}
