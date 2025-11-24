using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab12_2
{
    internal class MNK_Draw
    {
        double kx, ky, zx, zy, gx = 0, gy = 0;
        int L = 20, krokx, kroky;

        void GetBounds(double[] x, double[] y, int n, out double minx, out double maxx, out double miny, out double maxy)
        {
            minx = maxx = x[0];
            miny = maxy = y[0];
            for (int i = 0; i < n; i++)
            {
                if (x[i] < minx) minx = x[i];
                if (x[i] > maxx) maxx = x[i];
                if (y[i] < miny) miny = y[i];
                if (y[i] > maxy) maxy = y[i];
            }
            double dy = (maxy - miny) * 0.1;
            miny -= dy; maxy += dy;
        }

        public void Build(double[] xe, double[] ye, int ne, double[] xg, double[] yg, PictureBox pb)
        {
            double minx, maxx, miny, maxy;
            GetBounds(xe, ye, ne, out minx, out maxx, out miny, out maxy);

            Graphics g = pb.CreateGraphics();
            Pen pen = new Pen(Color.Black);

            kx = (pb.Width - 2 * L) / (maxx - minx);
            ky = (pb.Height - 2 * L) / (miny - maxy);
            zx = (pb.Width * minx - L * (minx + maxx)) / (minx - maxx);
            zy = (pb.Height * maxy - L * (miny + maxy)) / (maxy - miny);

            gx = (minx * maxx <= 0) ? 0 : (minx < 0 ? maxx : minx);
            gy = (miny * maxy <= 0) ? 0 : (miny < 0 ? maxy : miny);

            krokx = (pb.Width - 2 * L) / 10;
            kroky = (pb.Height - 2 * L) / 10;

            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            for (int i = 1; i <= 10; i++)
            {
                int yy = (int)(ky * gy + zy);
                int xx = (int)(kx * gx + zx);
                g.DrawLine(pen, L, yy - i * kroky, pb.Width - L, yy - i * kroky);
                g.DrawLine(pen, L, yy + i * kroky, pb.Width - L, yy + i * kroky);
                g.DrawLine(pen, xx - i * krokx, L, xx - i * krokx, pb.Height - L);
                g.DrawLine(pen, xx + i * krokx, L, xx + i * krokx, pb.Height - L);
            }

            pen.Color = Color.Blue; pen.Width = 2; pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            int y0 = (int)(ky * gy + zy);
            int x0 = (int)(kx * gx + zx);
            g.DrawLine(pen, L, y0, pb.Width - L, y0); 
            g.DrawLine(pen, x0, L, x0, pb.Height - L);  

            for (int i = 0; i <= 10; i++)
            {
                double valx = minx + i * (maxx - minx) / 10;
                int px = L + i * krokx;
                g.DrawString(Math.Round(valx, 2).ToString(),
                    new Font("Arial", 8), Brushes.Black, px - 15, y0 + 8);
            }

            for (int i = 0; i <= 10; i++)
            {
                double valy = maxy - i * (maxy - miny) / 10;
                int py = L + i * kroky;
                g.DrawString(Math.Round(valy, 2).ToString(),
                    new Font("Arial", 8), Brushes.Black, x0 + 8, py - 10);
            }

            for (int i = 0; i < ne; i++)
            {
                int px = (int)(kx * xe[i] + zx);
                int py = (int)(ky * ye[i] + zy);
                g.FillRectangle(Brushes.Blue, px - 5, py - 5, 10, 10);
                Thread.Sleep(25);
            }

            pen.Color = Color.Red; pen.Width = 2;
            for (int i = 1; i < yg.Length; i++)
            {
                int x1 = (int)(kx * xg[i - 1] + zx);
                int y1 = (int)(ky * yg[i - 1] + zy);
                int x2 = (int)(kx * xg[i] + zx);
                int y2 = (int)(ky * yg[i] + zy);
                g.DrawLine(pen, x1, y1, x2, y2);
            }
        }
    }

    static class MNK
    {
        static double Sum(int n, double[] a) { double s = 0; for (int i = 0; i < n; i++) s += a[i]; return s; }
        static void Step(int n, double[] x, ref double[] pow) { for (int i = 0; i < n; i++) pow[i] *= x[i]; }

        public static bool Gauss(double[,] A, double[] B, int n, ref double[] X)
        {
            for (int i = 0; i < n; i++)
            {
                int max = i;
                for (int j = i + 1; j < n; j++)
                    if (Math.Abs(A[j, i]) > Math.Abs(A[max, i])) max = j;
                for (int j = 0; j < n; j++) { double t = A[i, j]; A[i, j] = A[max, j]; A[max, j] = t; }
                double t2 = B[i]; B[i] = B[max]; B[max] = t2;

                for (int j = i + 1; j < n; j++)
                {
                    double c = A[j, i] / A[i, i];
                    for (int k = i; k < n; k++) A[j, k] -= c * A[i, k];
                    B[j] -= c * B[i];
                }
            }
            for (int i = n - 1; i >= 0; i--)
            {
                X[i] = B[i];
                for (int j = i + 1; j < n; j++) X[i] -= A[i, j] * X[j];
                X[i] /= A[i, i];
            }
            return true;
        }

        public static bool Aprox(double[] xe, double[] ye, int ne, ref double[] k, int m)
        {
            int s = m + 1;
            double[] pow = new double[ne];
            double[,] a = new double[s, s];
            double[] b = new double[s];

            for (int i = 0; i < ne; i++) pow[i] = 1.0;

            for (int j = 0; j < s; j++)
            {
                a[0, j] = Sum(ne, pow);
                Step(ne, xe, ref pow);
            }

            Array.Copy(pow, 0, pow, 0, ne);
            for (int i = 1; i < s; i++)
            {
                a[i, s - 1] = Sum(ne, pow);
                Step(ne, xe, ref pow);
            }

            for (int j = 1; j < s; j++)
                for (int i = 1; i <= j; i++)
                    a[i, j - i] = a[0, j];

            for (int i = 0; i < s - 1; i++)
                for (int j = i + 1; j < s; j++)
                    a[j, i] = a[i, s - 1];

            for (int i = 0; i < ne; i++) pow[i] = ye[i];
            for (int j = 0; j < s; j++)
            {
                b[j] = Sum(ne, pow);
                Step(ne, xe, ref pow);
            }

            return Gauss(a, b, s, ref k);
        }

        public static void AprTab(double al, double bl, double[] k, int m, int ngr, ref double[] xg, ref double[] yg)
        {
            double h = (bl - al) / (ngr - 1);
            double x = al;
            for (int i = 0; i < ngr; i++)
            {
                double y = k[0];
                double p = 1;
                for (int j = 1; j <= m; j++)
                {
                    p *= x;
                    y += k[j] * p;
                }
                xg[i] = x; yg[i] = y;
                x += h;
            }
        }
    }
}
