using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace MuzipCompress
{
    public partial class MuzipTabs : UserControl
    {
        public class TabSelectedEventArgs : EventArgs
        {
            public int SelectedTab { get; set; }
        }

        public delegate void TabSelectedEventHandler(object sender, TabSelectedEventArgs e);

        public Color Color1 { get; set; }
        public Color Color2 { get; set; }
        public Color FocusedColor1 { get; set; }
        public Color FocusedColor2 { get; set; }
        public Font TextFont { get; set; }
        public int SelectedTab { get; set; }

        private Bitmap buffer;
        private LinearGradientBrush brush, brush_focused;
        private SolidBrush font_brush;
        private List<GraphicsPath> pathes;
        private List<Rectangle> rects;
        private List<string> texts;
        private List<LinearGradientBrush> brushes, brushes_focused;
        private List<bool> focused;
        private Pen draw_pen;

        public event TabSelectedEventHandler TabSelected;

        public void OnTabSelected(TabSelectedEventArgs e)
        {
            TabSelectedEventHandler handler = TabSelected;
            if (handler != null) handler(this, e);
        }

        public LinearGradientBrush GetBrushes(int index) { return brushes[index]; }

        public MuzipTabs()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
            TextFont = new Font("Arial Black", 10f);
            pathes = new List<GraphicsPath>();
            rects = new List<Rectangle>();
            texts = new List<string>();
            brushes = new List<LinearGradientBrush>();
            brushes_focused = new List<LinearGradientBrush>();
            focused = new List<bool>();
            draw_pen = new Pen(Color.FromArgb(150, 20, 20, 20));
        }

        public void AddTab(string text)
        {
            Rectangle _r = new Rectangle(40 + texts.Count * 140, 0, 120, 40);
            LinearGradientBrush _brush = new LinearGradientBrush(new Point(_r.Left - 40, _r.Top / 2), new Point(_r.Right + 40, _r.Top / 2),
                Color1, Color2);
            brushes.Add(_brush); focused.Add(false);
            LinearGradientBrush _foc_brush = new LinearGradientBrush(new Point(_r.Left - 40, _r.Top / 2), new Point(_r.Right + 40, _r.Top / 2),
                FocusedColor1, FocusedColor2);
            brushes_focused.Add(_foc_brush);
            texts.Add(text); rects.Add(_r);
            pathes.Add(CreateTabPath(_r));
            CreateBrushes(); CreateBackBuffer(); Draw();
        }

        private void MuzipTabs_Load(object sender, EventArgs e) { CreateBrushes(); CreateBackBuffer(); Draw(); }

        private void MuzipTabs_Paint(object sender, PaintEventArgs e)
        { if (buffer != null) e.Graphics.DrawImageUnscaled(buffer, Point.Empty); }

        private void MuzipTabs_MouseMove(object sender, MouseEventArgs e) 
        {
            int i = 0, c = rects.Count;
            while (i < c)
            {
                focused[i] = false;
                if (e.X > rects[i].Left - 10 && e.X < rects[i].Right + 10 && e.Y > rects[i].Top && e.Y < rects[i].Bottom)
                { focused[i] = true; } i++;
            }
            Draw();
        }

        private void MuzipTabs_Resize(object sender, EventArgs e) { CreateBrushes(); CreateBackBuffer(); Draw(); }

        private void Draw()
        {
            if (buffer != null && brush != null && brush_focused != null && font_brush != null)
            {
                Graphics graphics = Graphics.FromImage(buffer);
                graphics.Clear(Color.Transparent);

                int i = texts.Count;
                while (i > 0)
                {
                    i--;
                    if (SelectedTab == i) { continue; }
                    graphics.FillPath(focused[i] ? brushes_focused[i] : brushes[i], pathes[i]);
                    graphics.DrawPath(draw_pen, pathes[i]);
                    SizeF sz = graphics.MeasureString(texts[i], TextFont);
                    graphics.DrawString(texts[i], TextFont, font_brush,
                        new PointF(((float)rects[i].Width - sz.Width) / 2 + (float)rects[i].X, ((float)rects[i].Height - sz.Height) / 2));
                }
                i = SelectedTab;
                if (texts.Count != 0)
                {
                    graphics.FillPath(brushes_focused[i], pathes[i]);
                    graphics.DrawPath(draw_pen, pathes[i]);
                    SizeF _sz = graphics.MeasureString(texts[i], TextFont);
                    graphics.DrawString(texts[i], TextFont, font_brush,
                        new PointF(((float)rects[i].Width - _sz.Width) / 2 + (float)rects[i].X, ((float)rects[i].Height - _sz.Height) / 2));
                }
                Invalidate();
            }
        }

        private GraphicsPath CreateTabPath(Rectangle rect)
        {
            GraphicsPath path = new GraphicsPath();
            int left = rect.Left, bottom = rect.Bottom, top = rect.Top, right = rect.Right;;
            path.StartFigure();
            path.AddLine(left - 30, bottom, left - 5, bottom - 35);
            path.AddLine(left - 5, bottom - 35, left, top);
            path.AddLine(left, top, right, top);
            path.AddLine(right, top, right + 5, top + 5);
            path.AddLine(right + 5, top + 5, right + 25, bottom);
            path.CloseFigure();
            return path;
        }

        public void CreateBrushes()
        {
            brush = new LinearGradientBrush(new Point(this.Left, this.Top / 2), new Point(this.Right, this.Top / 2),
                Color1, Color2);
            brush_focused = new LinearGradientBrush(new Point(this.Left, this.Top / 2), new Point(this.Right, this.Top / 2),
                FocusedColor1, FocusedColor2);
            font_brush = new SolidBrush(Color.FromArgb(200, 16, 16, 16));
        }

        private void CreateBackBuffer() { if (buffer != null) buffer.Dispose(); buffer = new Bitmap(this.Width, this.Height); }

        private void MuzipTabs_MouseLeave(object sender, EventArgs e)
        {
            int i = 0, c = focused.Count;
            while (i < c) { focused[i] = false; i++; }
            Draw();
        }

        private void MuzipTabs_MouseClick(object sender, MouseEventArgs e)
        {
            int i = 0, c = rects.Count;
            while (i < c)
            {
                focused[i] = false;
                if (e.X > rects[i].Left - 10 && e.X < rects[i].Right + 10 && e.Y > rects[i].Top && e.Y < rects[i].Bottom)
                { focused[i] = true; SelectedTab = i; OnTabSelected(new TabSelectedEventArgs() { SelectedTab = i }); } i++;
            }
            Draw();
        }
    }
}
