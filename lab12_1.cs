using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsForms_My_Grf
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        double al = 0;
        double bl = 0;
        int ne = 0;
        int ngr = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
            try
            {
                al = Convert.ToDouble(textBox1.Text);
                bl = Convert.ToDouble(textBox2.Text);
                ne = Convert.ToInt32(textBox3.Text);
                ngr = Convert.ToInt32(textBox4.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Межі задано не правильно", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Text = "";
                textBox2.Text = "";
                return;
            }
            Draw draw = new Draw();
            draw.Build(al, bl, ne, ngr, pictureBox1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Дійсно бажаєте вийти?", "Вихід", MessageBoxButtons.YesNo) == DialogResult.Yes) 
            { 
                Close(); 
            }
        }
    }
}
