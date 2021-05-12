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
    public partial class onlinePlayers : Form
    {
        Game.Character C;
        public onlinePlayers()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //foreach(Game.Character C in Game.World.H_Chars.Values)
            //{
            //this.characterBindingSource.Add(C);
            //}
            
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            Count();
            foreach (Game.Character C in Game.World.H_Chars.Values)
            {
                if (!this.comboBox1.Items.Contains(C.Name))
                    this.comboBox1.Items.Add(C.Name);
            }
        }
        private void Count()
        {
            label4.Text = Game.World.H_Chars.Count.ToString();
            progressBar1.Value = Game.World.H_Chars.Count;
        }
        // View info
        private void refresh(object sender, EventArgs e)
        {
           Count();
          C = Game.World.CharacterFromName(comboBox1.Text);
          if (C == null)
          {
              this.levelNumericUpDown.Text = "";
              this.jobNumericUpDown.Text = "";
              this.cPsTextBox.Text = "";
              this.silversTextBox.Text = "";
              this.wHSilversTextBox.Text = "";
              this.pKPointsTextBox.Text = "";
              this.rebornsNumericUpDown.Text = "";
              this.wHSilversTextBox.Text = "";
              this.wHPasswordTextBox.Text = "";
              this.AvatarLevelNumberUpDowen.Text = "";
              this.vipLevelNumericUpDown.Text = "";
              this.rebornCheckBox.Checked = false;

              return;
          }
          this.levelNumericUpDown.Text = C.Level.ToString();
          this.jobNumericUpDown.Text = C.Job.ToString();
          this.cPsTextBox.Text = C.CPs.ToString();
          this.silversTextBox.Text = C.Silvers.ToString();
          this.wHSilversTextBox.Text = C.WHSilvers.ToString();
          this.pKPointsTextBox.Text = C.PKPoints.ToString();
          this.rebornsNumericUpDown.Text = C.Reborns.ToString();
          this.wHSilversTextBox.Text = C.WHSilvers.ToString();
          this.wHPasswordTextBox.Text = C.WHPassword.ToString();
          this.AvatarLevelNumberUpDowen.Text = C.AvatarLevel.ToString();
          this.vipLevelNumericUpDown.Text = C.VipLevel.ToString();
          this.rebornCheckBox.Checked = C.Reborn;

          if (C.isAvatar)
              avatarL.Text = "As Avatar";
          else
              avatarL.Text = "";
        }

        // Apaly infos
        private void button2_Click(object sender, EventArgs e)
        {

            try
            {
                if (C != null)
                {
                    C.Job = byte.Parse(this.jobNumericUpDown.Text);
                    C.CPs = uint.Parse(this.cPsTextBox.Text);
                    C.Silvers = uint.Parse(this.silversTextBox.Text);
                    C.WHSilvers = uint.Parse(this.wHSilversTextBox.Text);
                    C.PKPoints = ushort.Parse(this.pKPointsTextBox.Text);
                    C.Reborns = byte.Parse(this.rebornsNumericUpDown.Text);
                    C.WHSilvers = uint.Parse(this.wHSilversTextBox.Text);
                    C.WHPassword = this.wHPasswordTextBox.Text;
                    C.VipLevel = byte.Parse(this.vipLevelNumericUpDown.Text);
                    C.AvatarLevel = byte.Parse(this.AvatarLevelNumberUpDowen.Text);
                }
            }
            catch (Exception exc) { MessageBox.Show(exc.ToString()); }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (C != null)
                {
                    C.Level = byte.Parse(this.levelNumericUpDown.Text);
                }
            }
            catch (Exception exc) { MessageBox.Show(exc.ToString()); }
        }

        private void onlinePlayers_Enter(object sender, EventArgs e)
        {
            refresh(sender,e);
        }

        private void onlinePlayers_MouseDown(object sender, MouseEventArgs e)
        {
            refresh(sender, e);
        }
      

       
    }
}
