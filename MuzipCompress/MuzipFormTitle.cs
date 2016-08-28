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
    public partial class MuzipFormTitle : UserControl
    {
        public Color Color1 { get; set; }
        public Color Color2 { get; set; }
        public string ButtonText { get; set; }
        public Font ButtonFont { get; set; }

        private Bitmap buffer;
        private bool mouse_focused;
        private LinearGradientBrush brush;
        private SolidBrush font_brush;
        private GraphicsPath path;

        public MuzipFormTitle()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            mouse_focused = Cursor.Position.X > this.Left && Cursor.Position.X < this.Right && 
                Cursor.Position.Y > this.Top && Cursor.Position.Y < this.Bottom;
            ButtonFont = new Font("MS Sans Serif", 10f, FontStyle.Bold);
        }

        public void CreateBrushes()
        {
            brush = new LinearGradientBrush(new Point(this.Left, this.Height / 2), new Point(this.Right, this.Height / 2),
                Color1, Color2);
            font_brush = new SolidBrush(Color.FromArgb(228, 16, 16, 16));
        }

        private void CreateBackBuffer() { if (buffer != null) buffer.Dispose(); buffer = new Bitmap(this.Width, this.Height); }

        private void Draw()
        {
            if (buffer != null && brush != null && path != null && font_brush != null)
            {
                Graphics graphics = Graphics.FromImage(buffer);
                graphics.Clear(Color.Transparent);

                graphics.FillPath(brush, path);

                SizeF text_size = graphics.MeasureString(ButtonText, ButtonFont);
                float pos_x = ((float)this.Width - text_size.Width) / 2, pos_y = ((float)this.Height - text_size.Height) / 2;
                graphics.DrawString(ButtonText, ButtonFont, font_brush, new PointF(pos_x, pos_y));
                
                Invalidate();
            }
        }

        private void CreatePath()
        {
            Rectangle bounds = new Rectangle(this.Location, new Size(this.Size.Width, this.Size.Height));
            int diameter = 10;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            path = new GraphicsPath();
            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
        }

        private void MuzipFormTitle_Load(object sender, EventArgs e) { CreateBrushes(); CreatePath(); CreateBackBuffer(); Draw(); }

        private void MuzipFormTitle_Paint(object sender, PaintEventArgs e) { if (buffer != null) e.Graphics.DrawImageUnscaled(buffer, Point.Empty); }

        private void MuzipFormTitle_Resize(object sender, EventArgs e) { CreateBrushes(); CreatePath(); CreateBackBuffer(); Draw(); }
    }
}
