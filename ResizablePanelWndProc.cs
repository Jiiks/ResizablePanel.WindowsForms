using System.ComponentModel;

namespace Jiiks.WinForms.Controls;

internal partial class ResizablePanel : Panel {
    private const int WM_NCHITTEST = 0x84;
    private const int HTCLIENT = 0x1;

    private const int HTCAPTION = 0x2;

    private const int HTTOP = 0xC;
    private const int HTBOTTOM = 0xF;
    private const int HTRIGHT = 0xB;
    private const int HTLEFT = 0xA;


    private const int HTTOPLEFT = 0XD;
    private const int HTTOPRIGHT = 0XE;
    private const int HTBOTTOMLEFT = 0x10;
    private const int HTBOTTOMRIGHT = 0x11;

    public ResizablePanel() {
        DoubleBuffered = true;
        SetStyle(ControlStyles.ResizeRedraw, true);
    }

    protected override void WndProc(ref Message m) {
        base.WndProc(ref m);

        if (m.Msg == WM_NCHITTEST) {
            _hitTestLocation = Location;
            var res = (int)m.Result;
            var screenPoint = new Point(m.LParam.ToInt32());
            var clientPoint = PointToClient(screenPoint);

            if (res == HTCLIENT) {
                if ((TopResize || TopMovesInstead) && clientPoint.Y < ResizeBorderSize && clientPoint.X > ResizeBorderSize && clientPoint.X < ClientSize.Width - ResizeBorderSize) {
                    m.Result = TopMovesInstead ? HTCAPTION : HTTOP;
                    return;
                }

                if (BottomRightResize && clientPoint.X >= ClientSize.Width - ResizeBorderSize &&
                    clientPoint.Y >= ClientSize.Height - ResizeBorderSize) {
                    m.Result = HTBOTTOMRIGHT;
                    return;
                }

                if (TopLeftResize && clientPoint.X <= ResizeBorderSize && clientPoint.Y <= ResizeBorderSize) {
                    m.Result = HTTOPLEFT;
                    return;
                }

                if(TopRightResize && clientPoint.X >= ClientSize.Width - ResizeBorderSize &&
                    clientPoint.Y <= ResizeBorderSize) {
                    m.Result = HTTOPRIGHT;
                    return;
                }

                if(BottomLeftResize && clientPoint.X <= ResizeBorderSize &&
                    clientPoint.Y >= ClientSize.Height - ResizeBorderSize) {
                    m.Result = HTBOTTOMLEFT;
                    return;
                }


                if(RightResize && clientPoint.X > ClientSize.Width - ResizeBorderSize) {
                    m.Result = HTRIGHT;
                    return;
                }

                if(TopLeftResize && clientPoint.X < ResizeBorderSize) {
                    m.Result = HTLEFT;
                    return;
                }

                if(BottomResize && clientPoint.Y > ClientSize.Height -  ResizeBorderSize) {
                    m.Result = HTBOTTOM;
                    return;
                }
            }
        }
    }

    protected override void OnPaint(PaintEventArgs e) {
        if (RBorderStyle != ButtonBorderStyle.None)
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

    private Point _hitTestLocation;
    protected override void OnResize(EventArgs eventargs) {
        base.OnResize(eventargs);

        // respect min size
        if (Width < MinimumSize.Width) {
            Width = MinimumSize.Width;
            Location = _hitTestLocation;
        }
        if(Height < MinimumSize.Height) {
            Height = MinimumSize.Height;
            Location = _hitTestLocation;
        }

        if(KeepAspectCornerOnly)
            Width = (int)(Height * Aspect.Width / Aspect.Height);
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
    public bool TopMovesInstead { get; set; } = true;
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
