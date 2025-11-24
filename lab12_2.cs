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

namespace lab12_1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(textBox1.Text, out double xmin)) { MessageBox.Show("Введіть коректну ліву межу"); return; }
            if (!double.TryParse(textBox2.Text, out double xmax)) { MessageBox.Show("Введіть коректну праву межу"); return; }
            if (!int.TryParse(textBox3.Text, out int N) || N < 10) { MessageBox.Show("Введіть кількість точок ≥ 10"); return; }
            if (!double.TryParse(textBox4.Text, out double a)) { MessageBox.Show("Введіть коректний параметр функції"); return; }
            string curve = comboBox1.SelectedItem.ToString();
            PolarDraw draw = new PolarDraw();
            draw.Draw(xmin, xmax, N, a, pictureBox1, curve);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.SelectedItem.ToString() == "Архімедова спіраль")
            {
                label4.Visible = false;
                textBox4.Visible = false;
                textBox4.Text = "1";
            }
            else
            {
                label4.Visible = true;
                textBox4.Visible = true;
                textBox4.Text = null;
            }
        }
    }
}
