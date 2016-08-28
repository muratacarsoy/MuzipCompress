using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MuzipCompress
{
    public partial class MuzipAnimation : UserControl
    {
        private Bitmap buffer;
        private TextureBrush brush;
        private Rectangle rect;
        private Timer tmr;
        private int tile;
        private bool tile_change;

        public MuzipAnimation()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.BackColor = Color.Transparent;
            buffer = new Bitmap(this.Width, this.Height);
            rect = new Rectangle(this.Location, new Size(256, 128));
            brush = new TextureBrush(Properties.Resources.animation);
            tmr = new Timer()
            {
                Enabled = true, Interval = 60
            };
            tile = 0; tile_change = true;
            tmr.Tick += tmr_Tick;
        }

        void tmr_Tick(object sender, EventArgs e)
        {
            Draw();
            if (tile_change) { tile++; if (tile > 18) tile_change = false; }
            else { tile--; if (tile < 1) tile_change = true; }
        }

        private void CreateBackBuffer() { if (buffer != null) buffer.Dispose(); buffer = new Bitmap(this.Width, this.Height); }

        private void MuzipAnimation_Load(object sender, EventArgs e) { CreateBackBuffer(); }

        private void MuzipAnimation_Resize(object sender, EventArgs e) { CreateBackBuffer(); }

        private void Draw()
        {
            if (buffer != null)
            {
                Graphics graphics = Graphics.FromImage(buffer);
                graphics.Clear(Color.Transparent);
                int tile_x = tile % 4;
                int tile_y = (tile - tile_x) / 4;
                brush.ResetTransform();
                brush.TranslateTransform(tile_x * 256f, tile_y * 128f);
                graphics.FillRectangle(brush, rect);
                Invalidate();
            }
        }

        private void MuzipAnimation_Paint(object sender, PaintEventArgs e)
        {
            if (buffer != null) e.Graphics.DrawImageUnscaled(buffer, Point.Empty);
        }

        public void StartAnimation() { tmr.Enabled = true; }

        public void StopAnimation() { tmr.Enabled = false; }
    }
}
