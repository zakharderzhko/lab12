using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System;

namespace WindowsForms_My_Grf
{
    internal class Draw
    {
        double h, krx, kry, maxx, maxy, minx, miny, maxxg, maxyg, minxg, minyg, kx, ky, zx, zy, gx = 0, gy = 0, xx, yy;
        int L, krokx, kroky;
        double[] xe = new double[1000];
        double[] ye = new double[1000];
        double[] xg = new double[1000];
        double[] yg = new double[1000];
        private double f1(double x) // 1-ша функція
        {
            return Math.Sin(x);
        }
        private double f2(double x) // 2-га функція
        {
            return Math.Sin(x);
        }
        // Метод для знаходження максимального і мінімального значень у векторі y
        public void getmaxmin(out double max, out double min, int n, double[] y)
        {
            max = y[0];
            min = y[0];
            for (int i = 0; i <= n - 1; i++)
            {
                if (max < y[i]) max = y[i];
                if (min > y[i]) min = y[i];
            }
        }
        // Метод для побудови графіка
        public void Build(double al, double bl, int ne, int ngr, PictureBox pictureBox1)
        {
            Pen p = new Pen(Color.Black);
            p.DashStyle = DashStyle.Dot;
            p.Color = Color.Black;
            Graphics pb1 = pictureBox1.CreateGraphics();
            L = 15;
            // Табулювання 1-ї функції
            h = (bl - al) / (ne - 1);
            xe[0] = al;
            for (int i = 0; i <= ne - 1; i++)
            {
                ye[i] = f1(xe[i]);
                xe[i + 1] = xe[i] + h;
            }
            maxx = xe[ne - 1];
            minx = xe[0];
            getmaxmin(out maxy, out miny, ne, ye); // знаходження максимального і мінімального значень уe
                                                   // Табулювання 2-ї функції
            h = (bl - al) / (ngr - 1);
            xg[0] = al;
            for (int i = 0; i <= ngr - 1; i++)
            {
                yg[i] = f2(xg[i]);
                xg[i + 1] = xg[i] + h;
            }
            maxxg = xg[ngr - 1];
            minxg = xg[0];
            getmaxmin(out maxyg, out minyg, ngr, yg); // знаходження максимального і мінімального значень уg
            if (maxyg > maxy) maxy = maxyg;
            if (minyg < miny) miny = minyg;
            // Обчислення коефіцієнтів масштабування
            kx = (pictureBox1.Width - 2 * L) / (maxx - minx);
            ky = (pictureBox1.Height - 2 * L) / (miny - maxy);
            zx = (pictureBox1.Width * minx - L * (minx + maxx)) / (minx - maxx);
            zy = (pictureBox1.Height * maxy - L * (miny + maxy)) / (maxy - miny);
            // Обчислення значень параметрів для побудови рухомих осей
            if (minx * maxx <= 0) gx = 0;
            if (minx * maxx > 0) gx = minx;
            if ((minx * maxx > 0) && (minx < 0)) gx = maxx;
            if (miny * maxy <= 0) gy = 0;
            if ((miny * maxy > 0) && (miny > 0)) gy = miny;
            if ((miny * maxy > 0) && (miny < 0)) gy = maxy;
            // Обчислення відстаней між лініями гратки х та у
            krokx = (pictureBox1.Width - 2 * L) / 10;
            kroky = (pictureBox1.Height - 2 * L) / 10;
            // Побудова гратки
            int mr1, mr2, mr3, mr4, mr5;
            int mxe1, mxe2, mxe3, mxe4;
            int mxg1, mxg2;
            for (int i = 0; i <= 10; i++)
            {
                mr1 = (int)Math.Round(ky * gy + zy);
                mr2 = (int)Math.Round(ky * gy + zy);
                mr3 = (int)Math.Round(ky * gy + zy);
                mr4 = (int)Math.Round(ky * gy + zy);
                mr5 = (int)Math.Round(kx * gx + zx);
                pb1.DrawLine(p, L, mr3 - i * kroky, pictureBox1.Width - L, mr4 - i * kroky);
                pb1.DrawLine(p, L, mr3 + i * kroky, pictureBox1.Width - L, mr4 + i * kroky);
                pb1.DrawLine(p, mr5 + i * krokx, L, mr5 + i * krokx, pictureBox1.Height - L);
                pb1.DrawLine(p, mr5 - i * krokx, L, mr5 - i * krokx, pictureBox1.Height - L);
            }
            // Побудова рухомих осей координат
            p.Color = Color.Blue;
            p.DashStyle = DashStyle.Solid;
            p.Width = 2;
            int w1 = (int)Math.Round(ky * gy + zy),
            w2 = (int)pictureBox1.Width - L,
            w3 = (int)Math.Round(ky * gy + zy),
            w4 = (int)Math.Round(kx * gx + zx),
            w5 = (int)Math.Round(kx * gx + zx);
            pb1.DrawLine(p, L, w1, w2, w3);
            pb1.DrawLine(p, w4, L, w5, pictureBox1.Height - L);
            xx = minx;
            yy = maxy;
            // Обчислення відстаней між масштабними підписами на графіку
            krx = (maxx - minx) / 10;
            kry = (maxy - miny) / 10;
            // Виведення маcштабних підписів
            for (int i = 0; i <= 10; i++)
            {
                mr1 = (int)Math.Round(ky * gy + zy);
                mr2 = (int)Math.Round(kx * gx + zx);
                pb1.DrawString(Convert.ToString(Math.Round(xx, 1)),
                new Font(new FontFamily("Arial"), 8), Brushes.Black, new Point(i * krokx, mr1 - L + 15));
                pb1.DrawString(Convert.ToString(Math.Round(yy, 1)),
                new Font(new FontFamily("Arial"), 8), Brushes.Black, new Point(mr2 - L + 15, L + i * kroky - 4));
                xx = xx + krx; yy = yy - kry;
            }
            p.Color = Color.Red;
            p.Width = 2;
            // Побудова 1-го графіка відрізками ліній
            for (int i = 1; i < ne - 1; i++)
            {
                Thread.Sleep(6); // сповільнення процесу візуалізації побудови графіка
                mxe1 = (int)Math.Round(kx * xe[i - 1] + zx);
                mxe2 = (int)Math.Round(ky * ye[i - 1] + zy);
                mxe3 = (int)Math.Round(kx * xe[i] + zx);
                mxe4 = (int)Math.Round(ky * ye[i] + zy);
                pb1.DrawLine(p, new Point(mxe1, mxe2), new Point(mxe3, mxe4));
            }
            p.Color = Color.Green;
            // Побудова 2-го графіка квадратиками
            for (int i = 0; i < ngr; i++)
            {
                Thread.Sleep(6); // сповільнення процесу візуалізації побудови графіка
                mxg1 = Convert.ToInt32(kx * xg[i] + zx);
                mxg2 = Convert.ToInt32(ky * yg[i] + zy);
                pb1.DrawRectangle(p, mxg1 - 3, mxg2 - 3, 6, 6);
            }
        }
    }
}
