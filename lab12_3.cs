using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace lab12_2
{
    public partial class Form1 : Form
    {

        double[] xe, ye;
        int N, Ngr, m;
        public Form1()
        {
            InitializeComponent();
            dataGridView1.ColumnCount = 2;
            dataGridView1.Columns[0].HeaderText = "Xe";
            dataGridView1.Columns[1].HeaderText = "Ye";
            dataGridView1.Columns[0].Width = 70;
            dataGridView1.Columns[1].Width = 70;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                N = int.Parse(textBox1.Text);
                Ngr = int.Parse(textBox2.Text);
                m = int.Parse(textBox3.Text);
                if (m < 2 || m > 5) throw new Exception();
            }
            catch
            {
                MessageBox.Show("Неправильні параметри!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            dataGridView1.Rows.Clear();
            for (int i = 0; i < N; i++)
                dataGridView1.Rows.Add("", "");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (N == 0)
            {
                MessageBox.Show("Спочатку натисніть кнопку «Введи»", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            pictureBox1.Refresh();

            xe = new double[N];
            ye = new double[N];

            try
            {
                for (int i = 0; i < N; i++)
                {
                    xe[i] = Convert.ToDouble(dataGridView1.Rows[i].Cells[0].Value);
                    ye[i] = Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value);
                }
            }
            catch
            {
                MessageBox.Show("Перевірте введені дані в таблиці!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            double[] k = new double[m + 1];
            if (!MNK.Aprox(xe, ye, N, ref k, m))
            {
                MessageBox.Show("Система не має розв'язку!", "Помилка");
                return;
            }

            double minx = xe[0], maxx = xe[0];
            for (int i = 0; i < N; i++)
            {
                if (xe[i] < minx) minx = xe[i];
                if (xe[i] > maxx) maxx = xe[i];
            }

            double[] xg = new double[Ngr];
            double[] yg = new double[Ngr];
            MNK.AprTab(minx, maxx, k, m, Ngr, ref xg, ref yg);

            MNK_Draw draw = new MNK_Draw();
            draw.Build(xe, ye, N, xg, yg, pictureBox1);
        }
    }
}
