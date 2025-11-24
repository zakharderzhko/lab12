using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace lab12_3
{
    public partial class Form1 : Form
    {
        double[] Xe = { 8.51602E+01, 8.735245E+01,8.929174E+01,9.114671E+01,9.274874E+01,
            9.401349E+01,9.544688E+01,9.578415E+01,9.738617E+01,9.806071E+01,9.881956E+01,
            1.0008432E+02,1.0160202E+02,1.039629E+02,1.0581788E+02,1.0834739E+02,1.118887E+02,
            1.1416526E+02,1.1585160E+02,1.1661046E+02,1.1770658E+02,1.1956155E+02,1.2082631E+02,
            1.2158516E+02,1.2352445E+02,1.2546374E+02,1.2698145E+02,1.2917369E+02,1.3102867E+02,
            1.3229342E+02,1.3465430E+02,1.3591906E+02,1.3844857E+02,1.4072513E+02,1.4401349E+02,
            1.4721754E+02,1.5286678E+02,1.5868465E+02,1.6627319E+02,1.7841484E+02,1.8701518E+02,
            1.9915683E+02,2.1576728E+02,2.3473862E+02,2.6087690E+02,2.9519393E+02 };

        double[] Ye = { 2.409070E+00,3.448280E+00,5.999060E+00,9.541800E+00,1.388758E+01,1.761927E+01,
            2.281530E+01,2.678318E+01,3.278224E+01,3.684459E+01,3.930090E+01,4.034010E+01,4.100142E+01,
            4.095418E+01,4.038734E+01,4.152102E+01,4.633916E+01,5.040151E+01,5.592820E+01,6.079358E+01,
            6.447803E+01,7.482286E+01,7.789325E+01,7.987718E+01,8.134152E+01,8.049126E+01,7.817667E+01,
            7.057156E+01,6.017950E+01,4.657534E+01,3.401039E+01,2.616911E+01,1.880019E+01,1.341521E+01,
            8.786020E+00,6.188000E+00,4.251300E+00,3.070380E+00,2.786960E+00,2.172890E+00,1.700520E+00,
            1.794990E+00,1.747760E+00,1.653280E+00,1.417100E+00,1.322630E+00 };

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Заповніть усі поля!");
                return;
            }

            if (!int.TryParse(textBox1.Text, out int Ne) ||
                !int.TryParse(textBox2.Text, out int m) ||
                !int.TryParse(textBox3.Text, out int ngr))
            {
                MessageBox.Show("Неправильний формат чисел!");
                return;
            }

            // --- Заповнення таблиці ---
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("X", "X");
            dataGridView1.Columns.Add("Y", "Y");

            for (int i = 0; i < Xe.Length; i++)
                dataGridView1.Rows.Add(Xe[i], Ye[i]);

            // --- Таблиця результатів ---
            dataGridView2.Rows.Clear();
            dataGridView2.Columns.Clear();
            dataGridView2.Columns.Add("param", "Параметр");
            dataGridView2.Columns.Add("value", "Значення");

            // --- Запуск побудови графіка ---
            var draw = new Draw();
            draw.Run(dataGridView1, ngr, pictureBox1, dataGridView2);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}
