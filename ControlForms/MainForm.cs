using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Server
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            onlinePlayers F = new onlinePlayers();
            F.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Server.Skills.SkillAdder S = new Server.Skills.SkillAdder();
            S.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            charDetailsVewer t = new charDetailsVewer();
            t.ShowDialog();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {

            //MessageBox.Show(e.KeyValue.ToString());
            if (((TextBox)sender).Text == "v" && e.KeyValue.ToString() == "13")
            {
                charDetailsVewer t = new charDetailsVewer();
                t.Show();
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
