using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab12_3
{
    internal class Draw
    {
        // Початкові наближення для двох Гаусіанів:
        double[] k = { 79.0, 0.005, 127.0, 38.0, 0.004, 103.0 };
        int iterCount = 0;

        public void Run(DataGridView dgvExp, int ngr, PictureBox pb, DataGridView dgvRes)
        {
            if (dgvExp.Rows.Count == 0) return;
            if (pb.Width <= 0 || pb.Height <= 0) return;

            int n = dgvExp.Rows.Count;
            if (dgvExp.AllowUserToAddRows && dgvExp.Rows[n - 1].IsNewRow) n--;

            double[] Xe = new double[n];
            double[] Ye = new double[n];

            for (int i = 0; i < n; i++)
            {
                object vx = dgvExp.Rows[i].Cells[0].Value;
                object vy = dgvExp.Rows[i].Cells[1].Value;
                Xe[i] = Convert.ToDouble(vx);
                Ye[i] = Convert.ToDouble(vy);
            }

            NewtonMethod(Xe, Ye);

            dgvRes.Rows.Clear();
            dgvRes.Rows.Add("A1", k[0].ToString("F8"));
            dgvRes.Rows.Add("σ1²", k[1].ToString("F8"));
            dgvRes.Rows.Add("x01", k[2].ToString("F8"));
            dgvRes.Rows.Add("A2", k[3].ToString("F8"));
            dgvRes.Rows.Add("σ2²", k[4].ToString("F8"));
            dgvRes.Rows.Add("x02", k[5].ToString("F8"));
            dgvRes.Rows.Add("Z (сума квадратів)", CalculateZ(Xe, Ye).ToString("F8"));
            dgvRes.Rows.Add("Ітерацій", iterCount.ToString());

            DrawGraphToBitmap(pb, Xe, Ye, ngr);
        }

        private double Model(double x)
            => k[0] * Math.Exp(-k[1] * (x - k[2]) * (x - k[2])) +
               k[3] * Math.Exp(-k[4] * (x - k[5]) * (x - k[5]));

        private void NewtonMethod(double[] xe, double[] ye)
        {
            iterCount = 0;
            const int maxIter = 500;
            const double tol = 1e-9;
            double damping = 0.65;

            for (iterCount = 0; iterCount < maxIter; iterCount++)
            {
                double[,] J = new double[6, 6]; 
                double[] F = new double[6];  

                for (int i = 0; i < xe.Length; i++)
                {
                    double x = xe[i];
                    double model = Model(x);
                    double diff = model - ye[i];

                    double g1 = k[0] * Math.Exp(-k[1] * (x - k[2]) * (x - k[2])); 
                    double g2 = k[3] * Math.Exp(-k[4] * (x - k[5]) * (x - k[5]));

                    double[] dg = new double[6];
                    dg[0] = Math.Exp(-k[1] * (x - k[2]) * (x - k[2]));   
                    dg[1] = -(x - k[2]) * (x - k[2]) * g1 / k[0]; 
                    dg[1] = -(x - k[2]) * (x - k[2]) * Math.Exp(-k[1] * (x - k[2]) * (x - k[2]));
                    dg[2] = 2.0 * k[1] * (x - k[2]) * Math.Exp(-k[1] * (x - k[2]) * (x - k[2])); 

                    dg[3] = Math.Exp(-k[4] * (x - k[5]) * (x - k[5]));            
                    dg[4] = -(x - k[5]) * (x - k[5]) * Math.Exp(-k[4] * (x - k[5]) * (x - k[5]));  
                    dg[5] = 2.0 * k[4] * (x - k[5]) * Math.Exp(-k[4] * (x - k[5]) * (x - k[5]));  

                    for (int p = 0; p < 6; p++)
                    {
                        double dmodel_dkp;
                        if (p == 0) dmodel_dkp = dg[0];
                        else if (p == 3) dmodel_dkp = dg[3];
                        else if (p == 1) dmodel_dkp = k[0] * dg[1]; 
                        else if (p == 2) dmodel_dkp = k[0] * dg[2];
                        else if (p == 4) dmodel_dkp = k[3] * dg[4];
                        else dmodel_dkp = k[3] * dg[5];

                        F[p] += diff * dmodel_dkp;

                        for (int q = 0; q < 6; q++)
                        {
                            double dmodel_dkq;
                            if (q == 0) dmodel_dkq = dg[0];
                            else if (q == 3) dmodel_dkq = dg[3];
                            else if (q == 1) dmodel_dkq = k[0] * dg[1];
                            else if (q == 2) dmodel_dkq = k[0] * dg[2];
                            else if (q == 4) dmodel_dkq = k[3] * dg[4];
                            else dmodel_dkq = k[3] * dg[5];

                            J[p, q] += dmodel_dkp * dmodel_dkq;
                        }
                    }
                }

                double[] dk = new double[6];
                bool ok = GaussSolve(J, F, dk);
                if (!ok) break; 

                double sumsq = 0.0;
                for (int i = 0; i < 6; i++)
                {
                    k[i] -= damping * dk[i];
                    sumsq += dk[i] * dk[i];
                }

                if (Math.Sqrt(sumsq) < tol) break;
            } 
        }

        private bool GaussSolve(double[,] a, double[] b, double[] x)
        {
            int n = b.Length;
            double[,] A = (double[,])a.Clone();
            double[] B = (double[])b.Clone();

            for (int i = 0; i < n; i++)
            {
                int max = i;
                double maxAbs = Math.Abs(A[i, i]);
                for (int r = i + 1; r < n; r++)
                {
                    double val = Math.Abs(A[r, i]);
                    if (val > maxAbs) { maxAbs = val; max = r; }
                }

                if (Math.Abs(A[max, i]) < 1e-15) return false;

                if (max != i)
                {
                    for (int c = i; c < n; c++)
                    {
                        double tmp = A[i, c]; A[i, c] = A[max, c]; A[max, c] = tmp;
                    }
                    double tb = B[i]; B[i] = B[max]; B[max] = tb;
                }

                for (int r = i + 1; r < n; r++)
                {
                    double factor = A[r, i] / A[i, i];
                    for (int c = i; c < n; c++)
                        A[r, c] -= factor * A[i, c];
                    B[r] -= factor * B[i];
                }
            }

            for (int i = n - 1; i >= 0; i--)
            {
                double s = B[i];
                for (int c = i + 1; c < n; c++) s -= A[i, c] * x[c];
                if (Math.Abs(A[i, i]) < 1e-15) return false;
                x[i] = s / A[i, i];
            }
            return true;
        }

        private double CalculateZ(double[] xe, double[] ye)
        {
            double s = 0;
            for (int i = 0; i < xe.Length; i++)
            {
                double d = Model(xe[i]) - ye[i];
                s += d * d;
            }
            return s;
        }

        private void DrawGraphToBitmap(PictureBox pb, double[] xe, double[] ye, int ngr)
        {
            int W = Math.Max(100, pb.Width);
            int H = Math.Max(100, pb.Height);

            Bitmap bmp = new Bitmap(W, H);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                int L = 40; 
                double minx = xe.Min();
                double maxx = xe.Max();
                double miny = ye.Min();
                double maxy = ye.Max();

                double padX = (maxx - minx) * 0.03;
                double padY = (maxy - miny) * 0.06;
                if (padX == 0) padX = 1.0;
                if (padY == 0) padY = 1.0;
                minx -= padX;
                maxx += padX;
                miny = Math.Min(0, miny - padY);
                maxy += padY;

                double kx = (W - 2 * L) / (maxx - minx);
                double ky = (H - 2 * L) / (maxy - miny);

                Func<double, int> toScrX = (x) => L + (int)Math.Round((x - minx) * kx);
                Func<double, int> toScrY = (y) => H - L - (int)Math.Round((y - miny) * ky);

                Pen gridPen = new Pen(Color.FromArgb(180, 220, 180)) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot };
                int nx = 10, ny = 10;
                for (int i = 0; i <= nx; i++)
                {
                    int sx = L + (int)Math.Round(i * (double)(W - 2 * L) / nx);
                    g.DrawLine(gridPen, sx, L, sx, H - L);
                }
                for (int j = 0; j <= ny; j++)
                {
                    int sy = L + (int)Math.Round(j * (double)(H - 2 * L) / ny);
                    g.DrawLine(gridPen, L, sy, W - L, sy);
                }

                Pen axisPen = new Pen(Color.Blue, 2f);
                double yAxisReal = 0;
                if (miny <= 0 && maxy >= 0) yAxisReal = 0;
                else if (miny > 0) yAxisReal = miny;
                else yAxisReal = maxy;
                int yAxisScreen = toScrY(yAxisReal);
                g.DrawLine(axisPen, L, yAxisScreen, W - L, yAxisScreen);

                double xAxisReal = 0;
                if (minx <= 0 && maxx >= 0) xAxisReal = 0;
                else if (minx > 0) xAxisReal = minx;
                else xAxisReal = maxx;
                int xAxisScreen = toScrX(xAxisReal);
                g.DrawLine(axisPen, xAxisScreen, L, xAxisScreen, H - L);

                using (Font f = new Font("Arial", 8))
                {
                    for (int i = 0; i <= nx; i++)
                    {
                        double vx = minx + i * (maxx - minx) / nx;
                        int sx = toScrX(vx);
                        string txt = Math.Round(vx, 1).ToString();
                        g.DrawString(txt, f, Brushes.Black, sx - 15, yAxisScreen + 4);
                    }
                    for (int i = 0; i <= ny; i++)
                    {
                        double vy = maxy - i * (maxy - miny) / ny;
                        int sy = toScrY(vy);
                        string txt = Math.Round(vy, 1).ToString();
                        g.DrawString(txt, f, Brushes.Black, xAxisScreen - 35, sy - 8);
                    }
                }

                foreach (var i in Enumerable.Range(0, xe.Length))
                {
                    int px = toScrX(xe[i]);
                    int py = toScrY(ye[i]);
                    g.FillRectangle(Brushes.White, px - 5, py - 5, 10, 10);
                    g.DrawRectangle(Pens.Blue, px - 4, py - 4, 8, 8);
                }

                Pen curvePen = new Pen(Color.Red, 2f);
                double h = (maxx - minx) / Math.Max(1, (ngr - 1));
                double prevX = minx;
                double prevY = Model(prevX);
                for (int i = 1; i <= ngr; i++)
                {
                    double curX = minx + i * h;
                    double curY = Model(curX);
                    g.DrawLine(curvePen, toScrX(prevX), toScrY(prevY), toScrX(curX), toScrY(curY));
                    prevX = curX; prevY = curY;
                }
            } 

            Image old = pb.Image;
            pb.Image = bmp;
            if (old != null) old.Dispose();
        }
    }
}
