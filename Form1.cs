using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SweepSecond
{
    public partial class Form1 : Form
    {
        static Brush brushHour = Brushes.DeepSkyBlue;
        static Brush brushMin = Brushes.LawnGreen;
        static Brush brushSec = Brushes.Crimson;

        public Form1()
        {
            InitializeComponent();
        }

        double toRadians(double ratio)
        {
            return -ratio * Math.PI * 2.0 + Math.PI;
        }

        void drawHand(Graphics g, Point center, double radius, double radians, Brush brush, double sideWeight)
        {
            var pTo = new Point(
                (int)(center.X + radius * Math.Sin(radians)),
                (int)(center.Y + radius * Math.Cos(radians)));
            var pFrom = new Point(
                (int)(center.X + radius / 8.0 * Math.Sin(radians - Math.PI)),
                (int)(center.Y + radius / 8.0 * Math.Cos(radians - Math.PI)));
            var pSide1 = new Point(
                (int)(center.X + radius / 20.0 * sideWeight * Math.Sin(radians - Math.PI / 2.0)),
                (int)(center.Y + radius / 20.0 * sideWeight * Math.Cos(radians - Math.PI / 2.0)));
            var pSide2 = new Point(
                (int)(center.X + radius / 20.0 * sideWeight * Math.Sin(radians + Math.PI / 2.0)),
                (int)(center.Y + radius / 20.0 * sideWeight * Math.Cos(radians + Math.PI / 2.0)));

            g.FillPolygon(brush, new[] { pFrom, pSide1, pTo, pSide2 });
        }

        void drawTick(Graphics g, Point center, double radius, double length, double radians, Pen pen)
        {
            var pTo = new Point(
                (int)(center.X + radius * Math.Sin(radians)),
                (int)(center.Y + radius * Math.Cos(radians)));
            var pFrom = new Point(
                (int)(center.X + radius * (1.0 - length) * Math.Sin(radians)),
                (int)(center.Y + radius * (1.0 - length) * Math.Cos(radians)));

            g.DrawLine(pen, pFrom, pTo);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var now = DateTime.Now;
            var hour = now.Hour;
            var minute = now.Minute;
            var second = (double)now.Second + (double)now.Millisecond / 1000.0;

            var r = Math.Min(this.ClientRectangle.Width, this.ClientRectangle.Height) / 2;
            var center = new Point(this.ClientRectangle.Width / 2, this.ClientRectangle.Height / 2);

            var radSec = toRadians(second / 60.0);
            var radMin = toRadians((minute + second / 60.0) / 60.0);
            var radHour = toRadians(((hour % 12) + minute / 60.0) / 12.0);

            var g = e.Graphics;
            g.Clear(Color.Black);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // date
            g.DrawString(now.ToString(" d"), new Font(FontFamily.GenericSansSerif, Math.Max(25.0f * r / 200.0f, 1.0f)), Brushes.White, new PointF((float)(this.ClientRectangle.Width / 2.0f + r * 0.45f), (float)(this.ClientRectangle.Height / 2.0f - r * 0.1f)));

            // minute ticks
            var pen = new Pen(Color.White, r / 120.0f);
            for (double rad = 0.0; rad < Math.PI * 2.0; rad += Math.PI * 2.0 / 60.0)
            {
                drawTick(g, center, r * 0.95, 0.06, rad, pen);
            }
            pen.Dispose();

            // hour ticks
            pen = new Pen(Color.White, r / 60.0f);
            for (double rad = 0.0; rad < Math.PI * 2.0; rad += Math.PI * 2.0 / 12.0)
            {
                drawTick(g, center, r * 0.95, 0.2, rad, pen);
            }
            pen.Dispose();

            // 0 3 6 9 ticks
            pen = new Pen(Color.White, r / 30.0f);
            for (double rad = 0.0; rad < Math.PI * 2.0; rad += Math.PI * 2.0 / 12.0 * 3.0)
            {
                drawTick(g, center, r * 0.95, 0.2, rad, pen);
            }
            pen.Dispose();

            // rim
            g.DrawEllipse(new Pen(Color.White, r / 60.0f), new Rectangle(center.X - r, center.Y - r, r * 2, r * 2));

            // hands
            drawHand(g, center, r * 0.6, radHour, brushHour, 1.2 / 0.5);
            drawHand(g, center, r * 0.89, radMin, brushMin, 1.0 / 0.8);
            drawHand(g, center, r * 0.95, radSec, brushSec, 0.5 / 0.9);

            // center
            var r2 = (int)(r / 20.0);
            g.FillEllipse(Brushes.White, new Rectangle(center.X - r2, center.Y - r2, r2 * 2, r2 * 2));
        }
    }
}
