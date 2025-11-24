using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace lab12_1
{
    internal class PolarDraw
    {
        public void Draw(double xmin, double xmax, int N, double a, PictureBox pb, string curve)
        {
            if (pb == null) return;
            if (N < 2) N = 2;
            if (double.IsNaN(xmin) || double.IsNaN(xmax)) return;
            if (Math.Abs(xmax - xmin) < 1e-12) xmax = xmin + 1e-6;

            var bmp = new Bitmap(pb.Width > 0 ? pb.Width : 1, pb.Height > 0 ? pb.Height : 1);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Beige);
                int L = 1;

                List<double> xs = new List<double>(N + 1);
                List<double> ys = new List<double>(N + 1);
                double minX = double.PositiveInfinity, maxX = double.NegativeInfinity;
                double minY = double.PositiveInfinity, maxY = double.NegativeInfinity;

                for (int i = 0; i <= N; i++)
                {
                    double phi = xmin + (xmax - xmin) * i / N;
                    double r = 0.0;
                    switch (curve)
                    {
                        case "Кардіоїда":
                            r = a * (1.0 + Math.Cos(phi));
                            break;
                        case "Полярна троянда":
                            r = a * Math.Cos(a * phi);
                            break;
                        case "Гіперболічна спіраль":
                            if (Math.Abs(phi) < 1e-12) phi = 1e-12;
                            r = a / phi;
                            break;
                        case "Архімедова спіраль":
                            r = phi;
                            break;
                    }
                    if (double.IsNaN(r) || double.IsInfinity(r)) continue;

                    double x = r * Math.Cos(phi);
                    double y = r * Math.Sin(phi);
                    if (double.IsNaN(x) || double.IsNaN(y) || double.IsInfinity(x) || double.IsInfinity(y)) continue;

                    xs.Add(x);
                    ys.Add(y);
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }

                if (xs.Count < 2)
                {
                    DrawEmptyAxes(g, pb, L);
                    pb.Image?.Dispose();
                    pb.Image = bmp;
                    return;
                }

                double dx = maxX - minX;
                double dy = maxY - minY;
                if (dx <= 0) dx = 1.0;
                if (dy <= 0) dy = 1.0;
                double padFactor = 1.2;
                dx *= padFactor; dy *= padFactor;
                double cx = (maxX + minX) / 2.0;
                double cy = (maxY + minY) / 2.0;
                double widthAvail = Math.Max(pb.Width - 2.0 * L, 1.0);
                double heightAvail = Math.Max(pb.Height - 2.0 * L, 1.0);
                double kx = widthAvail / dx;
                double ky = heightAvail / dy;
                double kscale = Math.Min(kx, ky);

                Func<double, double> TX = X => (X - cx) * kscale + pb.Width / 2.0;
                Func<double, double> TY = Y => (cy - Y) * kscale + pb.Height / 2.0;

                using (Pen grid = new Pen(Color.LightGray, 1) { DashStyle = DashStyle.Dot })
                {
                    int step = 50;
                    for (int x = step; x < pb.Width; x += step) g.DrawLine(grid, x, 0, x, pb.Height);
                    for (int y = step; y < pb.Height; y += step) g.DrawLine(grid, 0, y, pb.Width, y);
                }

                float axisY = SafeFloat(TY(0.0), pb.Height);
                float axisX = SafeFloat(TX(0.0), pb.Width);

                using (Pen axis = new Pen(Color.Blue, 2))
                {
                    g.DrawLine(axis, 0f, axisY, pb.Width, axisY);
                    g.DrawLine(axis, axisX, 0f, axisX, pb.Height);
                }

                Font font = new Font("Arial", 8);
                Brush brush = Brushes.Black;
                StringFormat sf = new StringFormat { Alignment = StringAlignment.Center };

                for (int i = -5; i <= 5; i++)
                {
                    double realX = cx + (dx / 10.0) * i * 2;  
                    float screenX = SafeFloat(TX(realX), pb.Width);
                    if (screenX >= 10 && screenX <= pb.Width - 10)
                    {
                        string label = Math.Round(realX, 2).ToString("0.##");
                        g.DrawString(label, font, brush, screenX, axisY + 4, sf);
                    }
                }

                for (int i = -5; i <= 5; i++)
                {
                    double realY = cy + (dy / 10.0) * i * 2;
                    float screenY = SafeFloat(TY(realY), pb.Height);
                    if (screenY >= 20 && screenY <= pb.Height - 10)
                    {
                        string label = Math.Round(realY, 2).ToString("0.##");
                        g.DrawString(label, font, brush, axisX + 4, screenY - 10);
                    }
                }

                using (Pen pen = new Pen(Color.Red, 2))
                {
                    for (int i = 1; i < xs.Count; i++)
                    {
                        float x1 = SafeFloat(TX(xs[i - 1]), pb.Width);
                        float y1 = SafeFloat(TY(ys[i - 1]), pb.Height);
                        float x2 = SafeFloat(TX(xs[i]), pb.Width);
                        float y2 = SafeFloat(TY(ys[i]), pb.Height);
                        g.DrawLine(pen, x1, y1, x2, y2);
                    }
                }
            }

            pb.Image?.Dispose();
            pb.Image = bmp;
        }

        private static float SafeFloat(double v, int limit)
        {
            if (double.IsNaN(v) || double.IsInfinity(v)) return limit / 2f;
            if (v > float.MaxValue) return float.MaxValue;
            if (v < float.MinValue) return float.MinValue;
            return (float)v;
        }

        private void DrawEmptyAxes(Graphics g, PictureBox pb, int L)
        {
            g.Clear(Color.Beige);
            using (Pen axis = new Pen(Color.Blue, 2))
            {
                float midX = pb.Width / 2f;
                float midY = pb.Height / 2f;
                g.DrawLine(axis, 0f, midY, pb.Width, midY);
                g.DrawLine(axis, midX, 0f, midX, pb.Height);
            }
        }
    }
}

