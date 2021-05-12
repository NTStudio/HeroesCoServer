using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Server.Skills
{
    public enum SkillIDs : ushort
    {
        MonsterCure = 11005,
        //fasta2 = 5135,
        Bless = 9876,
        Riding = 7001,
        Spook = 7002,
        WarCry = 7003,
        FlashStep = 4550,
        TwofoldBlades = 6000,
        ToxicFog = 6001,
        PoisonStar = 6002,
        CounterKill = 6003,
        ArcherBane = 6004,
        ShurikenVortex = 6010,
        FatalStrike = 6011,
        Accuracy = 1015,
        Cyclone = 1110,
        Hercules = 1115,
        SpiritHealing = 1190,
        Shield = 1020,
        SuperMan = 1025,
        Roar = 1040,
        Dash = 1051,
        FlyingMoon = 1320,
        Scatter = 8001,
        RapidFire = 8000,
        AdvancedFly = 8003,
        Intensify = 9000,
        XPFly = 8002,
        ArrowRain = 8030,
        MeshAref = 8204,
        Thunder = 1000,
        Cure = 1005,
        Fire = 1001,
        Meditation = 1195,
        FireRing = 1150,
        FireMeteor = 1180,
        FireCircle = 1120,
        Tornado = 1002,
        Bomb = 1160,
        FireOfHell = 1165,
        Lightning = 1010,
        Volcano = 1125,
        SpeedLightning = 5001,
        HealingRain = 1055,
        StarOfAccuracy = 1085,
        MagicShield = 1090,
        Stigma = 1095,
        Invisibility = 1075,
        Pray = 1100,
        AdvancedCure = 1175,
        Nectar = 1170,
        XPRevive = 1050,
        FastBlade = 1045,
        ScentSword = 1046,
        Phoenix = 5030,
        WideStrike = 1250,
        Boreas = 5050,
        Snow = 5010,
        StrandedMonster = 5020,
        SpeedGun = 1260,
        Penetration = 1290,
        Boom = 5040,
        Halt = 1300,
        Seizer = 7000,
        EarthQuake = 7010,
        Rage = 7020,
        Celestial = 7030,
        Roamer = 7040,
        Robot = 1270,
        WaterElf = 1280,
        DivineHare = 1350,
        NightDevil = 1360,
        Reflect = 3060,
        CruelShade = 3050,
        Dodge = 3080,
        Pervade = 3090,
        SummonGuard = 4000,
        FireEvil = 4060,
        BloodyBat = 4050,
        Skeleton = 4070,
        SummonBat = 4010,
        SummonBatBoss = 4020,
        Dance2 = 1380,
        Dance3 = 1385,
        Dance4 = 1390,
        Dance5 = 1395,
        Dance6 = 1400,
        Dance7 = 1405,
        Dance8 = 1410,
        Restore = 1105,
        Icicle = 5130,
        IceCircle = 5131,
        Avalanche = 5132
    }
    public enum levels : byte
    {
       lvl1 = 1,
       lvl2 = 2,
       lvl3 = 3,
       lvl4 = 4,
       lvl5 = 5,
       lvl6 = 6,
       lvl7 = 7,
       lvl8 = 8,
       lvl9 = 9,
       lvl10 = 10

    }
    public class SkillAdder : Form
    {
        private TabPage SkillListPage;
        private TabPage SkillAddPage;
        private Label label1;
        private ComboBox SkillIDBox;
        private Label label2;
        private NumericUpDown LevelNum;
        private GroupBox groupBox1;
        private Label label5;
        private NumericUpDown ArrowCostNum;
        private Label label4;
        private NumericUpDown StaminaCostNum;
        private Label label3;
        private NumericUpDown ManaCostNum;
        private ComboBox ExtraEff;
        private ComboBox DamageType;
        private ComboBox TargetType;
        private CheckBox EndXP;
        private Label label6;
        private Label label8;
        private Label label7;
        private ListBox SkillListBox;
        private Label label10;
        private NumericUpDown ExpReq;
        private Label label9;
        private NumericUpDown ReqLevel;
        private Label label11;
        private NumericUpDown Damage;
        private NumericUpDown EffectValue;
        private Label label12;
        private NumericUpDown EffectLasts;
        private Label label13;
        private NumericUpDown ActivChance;
        private NumericUpDown SectorSize;
        private NumericUpDown Distance;
        private Label label14;
        private Label label16;
        private Label label15;
        private Button AddButton;
        private Button SelectButton;
        private Button DeleteButton;
        private TabPage tabQueryPage;
        private Label label17;
        private ComboBox comboBox1;
        private Button button1;
        private Button button2;
        private Label label18;
        private Button button3;
        private ComboBox comboBox2;
        private Button button4;
        private Label label19;
        private TabControl tabControl1;

        public SkillAdder()
        {

            InitializeComponent();
            string[] vals = Enum.GetNames(typeof(SkillIDs));
            foreach (string val in vals)
            {
                this.SkillIDBox.Items.Add(val);
            }
            foreach (Skills.SkillsClass.SkillInfo I in Skills.SkillsClass.SkillInfos.Values)
                this.comboBox1.Items.Add(I.ID.ToString() + " " + I.Level.ToString() + " " + ((SkillIDs)I.ID).ToString());


            foreach (Skills.SkillsClass.SkillInfo I in Skills.SkillsClass.SkillInfos.Values)
                this.comboBox2.Items.Add(((SkillIDs)I.ID).ToString() + " " + I.ID.ToString() + " " + I.Level.ToString());


            this.SkillIDBox.SelectedIndex = 0;
            vals = Enum.GetNames(typeof(Skills.SkillsClass.TargetType));
            foreach (string val in vals)
                this.TargetType.Items.Add(val);
            this.TargetType.SelectedIndex = 0;
            vals = Enum.GetNames(typeof(Skills.SkillsClass.DamageType));
            foreach (string val in vals)
                this.DamageType.Items.Add(val);
            this.DamageType.SelectedIndex = 0;
            vals = Enum.GetNames(typeof(Skills.SkillsClass.ExtraEffect));
            foreach (string val in vals)
                this.ExtraEff.Items.Add(val);
            this.ExtraEff.SelectedIndex = 0;

            foreach (Skills.SkillsClass.SkillInfo I in Skills.SkillsClass.SkillInfos.Values)
                this.SkillListBox.Items.Add(((SkillIDs)I.ID).ToString() + " " + I.ID.ToString() + " " + I.Level.ToString());
        }
        void InitializeComponent()
        {
            this.SkillListPage = new System.Windows.Forms.TabPage();
            this.SelectButton = new System.Windows.Forms.Button();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.SkillListBox = new System.Windows.Forms.ListBox();
            this.SkillAddPage = new System.Windows.Forms.TabPage();
            this.AddButton = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.ActivChance = new System.Windows.Forms.NumericUpDown();
            this.SectorSize = new System.Windows.Forms.NumericUpDown();
            this.Distance = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.EffectValue = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.EffectLasts = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.Damage = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.ExpReq = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.ReqLevel = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.ExtraEff = new System.Windows.Forms.ComboBox();
            this.DamageType = new System.Windows.Forms.ComboBox();
            this.TargetType = new System.Windows.Forms.ComboBox();
            this.EndXP = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ArrowCostNum = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.StaminaCostNum = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.ManaCostNum = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.LevelNum = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.SkillIDBox = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabQueryPage = new System.Windows.Forms.TabPage();
            this.label18 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label17 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.label19 = new System.Windows.Forms.Label();
            this.SkillListPage.SuspendLayout();
            this.SkillAddPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ActivChance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SectorSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Distance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EffectValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EffectLasts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Damage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ExpReq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ReqLevel)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ArrowCostNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StaminaCostNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ManaCostNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LevelNum)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabQueryPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // SkillListPage
            // 
            this.SkillListPage.Controls.Add(this.SelectButton);
            this.SkillListPage.Controls.Add(this.DeleteButton);
            this.SkillListPage.Controls.Add(this.SkillListBox);
            this.SkillListPage.Location = new System.Drawing.Point(4, 22);
            this.SkillListPage.Name = "SkillListPage";
            this.SkillListPage.Padding = new System.Windows.Forms.Padding(3);
            this.SkillListPage.Size = new System.Drawing.Size(480, 314);
            this.SkillListPage.TabIndex = 1;
            this.SkillListPage.Text = "Skill List";
            this.SkillListPage.UseVisualStyleBackColor = true;
            // 
            // SelectButton
            // 
            this.SelectButton.Location = new System.Drawing.Point(7, 277);
            this.SelectButton.Name = "SelectButton";
            this.SelectButton.Size = new System.Drawing.Size(75, 23);
            this.SelectButton.TabIndex = 2;
            this.SelectButton.Text = "Edit";
            this.SelectButton.UseVisualStyleBackColor = true;
            this.SelectButton.Click += new System.EventHandler(this.SelectButton_Click);
            // 
            // DeleteButton
            // 
            this.DeleteButton.Location = new System.Drawing.Point(387, 277);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(75, 23);
            this.DeleteButton.TabIndex = 1;
            this.DeleteButton.Text = "Delete";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // SkillListBox
            // 
            this.SkillListBox.FormattingEnabled = true;
            this.SkillListBox.Location = new System.Drawing.Point(7, 7);
            this.SkillListBox.Name = "SkillListBox";
            this.SkillListBox.Size = new System.Drawing.Size(455, 264);
            this.SkillListBox.TabIndex = 0;
            this.SkillListBox.SelectedIndexChanged += new System.EventHandler(this.SkillListBox_SelectedIndexChanged);
            // 
            // SkillAddPage
            // 
            this.SkillAddPage.BackColor = System.Drawing.Color.CornflowerBlue;
            this.SkillAddPage.CausesValidation = false;
            this.SkillAddPage.Controls.Add(this.AddButton);
            this.SkillAddPage.Controls.Add(this.label16);
            this.SkillAddPage.Controls.Add(this.label15);
            this.SkillAddPage.Controls.Add(this.label14);
            this.SkillAddPage.Controls.Add(this.ActivChance);
            this.SkillAddPage.Controls.Add(this.SectorSize);
            this.SkillAddPage.Controls.Add(this.Distance);
            this.SkillAddPage.Controls.Add(this.label13);
            this.SkillAddPage.Controls.Add(this.EffectValue);
            this.SkillAddPage.Controls.Add(this.label12);
            this.SkillAddPage.Controls.Add(this.EffectLasts);
            this.SkillAddPage.Controls.Add(this.label11);
            this.SkillAddPage.Controls.Add(this.Damage);
            this.SkillAddPage.Controls.Add(this.label10);
            this.SkillAddPage.Controls.Add(this.ExpReq);
            this.SkillAddPage.Controls.Add(this.label9);
            this.SkillAddPage.Controls.Add(this.ReqLevel);
            this.SkillAddPage.Controls.Add(this.label8);
            this.SkillAddPage.Controls.Add(this.label7);
            this.SkillAddPage.Controls.Add(this.label6);
            this.SkillAddPage.Controls.Add(this.ExtraEff);
            this.SkillAddPage.Controls.Add(this.DamageType);
            this.SkillAddPage.Controls.Add(this.TargetType);
            this.SkillAddPage.Controls.Add(this.EndXP);
            this.SkillAddPage.Controls.Add(this.groupBox1);
            this.SkillAddPage.Controls.Add(this.label2);
            this.SkillAddPage.Controls.Add(this.LevelNum);
            this.SkillAddPage.Controls.Add(this.label1);
            this.SkillAddPage.Controls.Add(this.SkillIDBox);
            this.SkillAddPage.Location = new System.Drawing.Point(4, 22);
            this.SkillAddPage.Name = "SkillAddPage";
            this.SkillAddPage.Padding = new System.Windows.Forms.Padding(3);
            this.SkillAddPage.Size = new System.Drawing.Size(480, 314);
            this.SkillAddPage.TabIndex = 0;
            this.SkillAddPage.Text = "Skill Add";
            this.SkillAddPage.Click += new System.EventHandler(this.SkillAddPage_Click);
            // 
            // AddButton
            // 
            this.AddButton.Location = new System.Drawing.Point(358, 283);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(75, 23);
            this.AddButton.TabIndex = 28;
            this.AddButton.Text = "Add";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(355, 242);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(94, 13);
            this.label16.TabIndex = 27;
            this.label16.Text = "Activation Chance";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(355, 215);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(60, 13);
            this.label15.TabIndex = 26;
            this.label15.Text = "Sector Size";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(355, 188);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(48, 13);
            this.label14.TabIndex = 25;
            this.label14.Text = "Distance";
            // 
            // ActivChance
            // 
            this.ActivChance.Location = new System.Drawing.Point(228, 235);
            this.ActivChance.Name = "ActivChance";
            this.ActivChance.Size = new System.Drawing.Size(120, 20);
            this.ActivChance.TabIndex = 24;
            // 
            // SectorSize
            // 
            this.SectorSize.Location = new System.Drawing.Point(228, 208);
            this.SectorSize.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.SectorSize.Name = "SectorSize";
            this.SectorSize.Size = new System.Drawing.Size(120, 20);
            this.SectorSize.TabIndex = 23;
            // 
            // Distance
            // 
            this.Distance.Location = new System.Drawing.Point(228, 181);
            this.Distance.Name = "Distance";
            this.Distance.Size = new System.Drawing.Size(120, 20);
            this.Distance.TabIndex = 22;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(355, 140);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(65, 13);
            this.label13.TabIndex = 21;
            this.label13.Text = "Effect Value";
            // 
            // EffectValue
            // 
            this.EffectValue.Location = new System.Drawing.Point(228, 134);
            this.EffectValue.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.EffectValue.Name = "EffectValue";
            this.EffectValue.Size = new System.Drawing.Size(120, 20);
            this.EffectValue.TabIndex = 20;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(354, 113);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(64, 13);
            this.label12.TabIndex = 19;
            this.label12.Text = "Effect Lasts";
            // 
            // EffectLasts
            // 
            this.EffectLasts.Location = new System.Drawing.Point(228, 107);
            this.EffectLasts.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.EffectLasts.Name = "EffectLasts";
            this.EffectLasts.Size = new System.Drawing.Size(120, 20);
            this.EffectLasts.TabIndex = 18;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(354, 86);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(46, 13);
            this.label11.TabIndex = 17;
            this.label11.Text = "Damage";
            // 
            // Damage
            // 
            this.Damage.Location = new System.Drawing.Point(228, 80);
            this.Damage.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.Damage.Name = "Damage";
            this.Damage.Size = new System.Drawing.Size(120, 20);
            this.Damage.TabIndex = 16;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(355, 40);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(47, 13);
            this.label10.TabIndex = 15;
            this.label10.Text = "Exp Req";
            // 
            // ExpReq
            // 
            this.ExpReq.Location = new System.Drawing.Point(228, 33);
            this.ExpReq.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.ExpReq.Name = "ExpReq";
            this.ExpReq.Size = new System.Drawing.Size(120, 20);
            this.ExpReq.TabIndex = 14;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(355, 14);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(54, 13);
            this.label9.TabIndex = 13;
            this.label9.Text = "Level Req";
            // 
            // ReqLevel
            // 
            this.ReqLevel.Location = new System.Drawing.Point(228, 6);
            this.ReqLevel.Maximum = new decimal(new int[] {
            130,
            0,
            0,
            0});
            this.ReqLevel.Name = "ReqLevel";
            this.ReqLevel.Size = new System.Drawing.Size(120, 20);
            this.ReqLevel.TabIndex = 12;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(142, 243);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Extra Effect";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(142, 216);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Damage Type";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(142, 189);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(66, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Target Type";
            // 
            // ExtraEff
            // 
            this.ExtraEff.FormattingEnabled = true;
            this.ExtraEff.Location = new System.Drawing.Point(14, 235);
            this.ExtraEff.Name = "ExtraEff";
            this.ExtraEff.Size = new System.Drawing.Size(121, 21);
            this.ExtraEff.TabIndex = 8;
            // 
            // DamageType
            // 
            this.DamageType.FormattingEnabled = true;
            this.DamageType.Location = new System.Drawing.Point(14, 208);
            this.DamageType.Name = "DamageType";
            this.DamageType.Size = new System.Drawing.Size(121, 21);
            this.DamageType.TabIndex = 7;
            // 
            // TargetType
            // 
            this.TargetType.FormattingEnabled = true;
            this.TargetType.Location = new System.Drawing.Point(14, 181);
            this.TargetType.Name = "TargetType";
            this.TargetType.Size = new System.Drawing.Size(121, 21);
            this.TargetType.TabIndex = 6;
            // 
            // EndXP
            // 
            this.EndXP.AutoSize = true;
            this.EndXP.Location = new System.Drawing.Point(15, 289);
            this.EndXP.Name = "EndXP";
            this.EndXP.Size = new System.Drawing.Size(89, 17);
            this.EndXP.TabIndex = 5;
            this.EndXP.Text = "Ends XP Wait";
            this.EndXP.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.ArrowCostNum);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.StaminaCostNum);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.ManaCostNum);
            this.groupBox1.Location = new System.Drawing.Point(8, 60);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(198, 104);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Skill Costs";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(134, 81);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Arrows";
            // 
            // ArrowCostNum
            // 
            this.ArrowCostNum.Location = new System.Drawing.Point(7, 74);
            this.ArrowCostNum.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.ArrowCostNum.Name = "ArrowCostNum";
            this.ArrowCostNum.Size = new System.Drawing.Size(120, 20);
            this.ArrowCostNum.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(134, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Stamina";
            // 
            // StaminaCostNum
            // 
            this.StaminaCostNum.Location = new System.Drawing.Point(7, 47);
            this.StaminaCostNum.Name = "StaminaCostNum";
            this.StaminaCostNum.Size = new System.Drawing.Size(120, 20);
            this.StaminaCostNum.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(134, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Mana";
            // 
            // ManaCostNum
            // 
            this.ManaCostNum.Location = new System.Drawing.Point(7, 20);
            this.ManaCostNum.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.ManaCostNum.Name = "ManaCostNum";
            this.ManaCostNum.Size = new System.Drawing.Size(120, 20);
            this.ManaCostNum.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(142, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Choose Level";
            // 
            // LevelNum
            // 
            this.LevelNum.Location = new System.Drawing.Point(15, 34);
            this.LevelNum.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.LevelNum.Name = "LevelNum";
            this.LevelNum.Size = new System.Drawing.Size(120, 20);
            this.LevelNum.TabIndex = 2;
            this.LevelNum.ValueChanged += new System.EventHandler(this.LevelNum_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(142, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Choose Skill";
            // 
            // SkillIDBox
            // 
            this.SkillIDBox.FormattingEnabled = true;
            this.SkillIDBox.Location = new System.Drawing.Point(14, 6);
            this.SkillIDBox.Name = "SkillIDBox";
            this.SkillIDBox.Size = new System.Drawing.Size(121, 21);
            this.SkillIDBox.TabIndex = 0;
            this.SkillIDBox.SelectedIndexChanged += new System.EventHandler(this.SkillIDBox_SelectedIndexChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.SkillAddPage);
            this.tabControl1.Controls.Add(this.SkillListPage);
            this.tabControl1.Controls.Add(this.tabQueryPage);
            this.tabControl1.Location = new System.Drawing.Point(1, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(488, 340);
            this.tabControl1.TabIndex = 0;
            // 
            // tabQueryPage
            // 
            this.tabQueryPage.Controls.Add(this.label18);
            this.tabQueryPage.Controls.Add(this.button3);
            this.tabQueryPage.Controls.Add(this.comboBox2);
            this.tabQueryPage.Controls.Add(this.button1);
            this.tabQueryPage.Controls.Add(this.label17);
            this.tabQueryPage.Controls.Add(this.comboBox1);
            this.tabQueryPage.Location = new System.Drawing.Point(4, 22);
            this.tabQueryPage.Name = "tabQueryPage";
            this.tabQueryPage.Padding = new System.Windows.Forms.Padding(3);
            this.tabQueryPage.Size = new System.Drawing.Size(480, 314);
            this.tabQueryPage.TabIndex = 2;
            this.tabQueryPage.Text = "SkillQuery";
            this.tabQueryPage.UseVisualStyleBackColor = true;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(369, 151);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(105, 13);
            this.label18.TabIndex = 7;
            this.label18.Text = "Chose Skill by  Name";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(236, 187);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(215, 37);
            this.button3.TabIndex = 6;
            this.button3.Text = "Edit";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(22, 148);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(320, 21);
            this.comboBox2.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(236, 58);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(216, 36);
            this.button1.TabIndex = 4;
            this.button1.Text = "Edit";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(372, 23);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(92, 13);
            this.label17.TabIndex = 3;
            this.label17.Text = "Choose Skill by ID";
            this.label17.Click += new System.EventHandler(this.label17_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(22, 20);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(320, 21);
            this.comboBox1.TabIndex = 2;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(359, 354);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(130, 27);
            this.button2.TabIndex = 1;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(207, 354);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(123, 27);
            this.button4.TabIndex = 2;
            this.button4.Text = "Save";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(41, 361);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(0, 13);
            this.label19.TabIndex = 3;
            // 
            // SkillAdder
            // 
            this.ClientSize = new System.Drawing.Size(516, 393);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.tabControl1);
            this.Name = "SkillAdder";
            this.Text = "Skill Adder";
            this.SkillListPage.ResumeLayout(false);
            this.SkillAddPage.ResumeLayout(false);
            this.SkillAddPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ActivChance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SectorSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Distance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EffectValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EffectLasts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Damage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ExpReq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ReqLevel)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ArrowCostNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StaminaCostNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ManaCostNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LevelNum)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabQueryPage.ResumeLayout(false);
            this.tabQueryPage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            Skills.SkillsClass.SkillInfo S = new Server.Skills.SkillsClass.SkillInfo();
            S.ID = (ushort)Enum.Parse(typeof(SkillIDs), this.SkillIDBox.SelectedItem.ToString());
            S.Level = (byte)this.LevelNum.Value;
            S.ManaCost = (ushort)this.ManaCostNum.Value;
            S.StaminaCost = (byte)this.StaminaCostNum.Value;
            S.ArrowsCost = (byte)this.ArrowCostNum.Value;
            S.EndsXPWait = this.EndXP.Checked;
            S.ExtraEff = (Skills.SkillsClass.ExtraEffect)Enum.Parse(typeof(Skills.SkillsClass.ExtraEffect), this.ExtraEff.SelectedItem.ToString());
            S.Targetting = (Skills.SkillsClass.TargetType)Enum.Parse(typeof(Skills.SkillsClass.TargetType), this.TargetType.SelectedItem.ToString());
            S.Damageing = (Skills.SkillsClass.DamageType)Enum.Parse(typeof(Skills.SkillsClass.DamageType), this.DamageType.SelectedItem.ToString());
            S.Damage = (uint)this.Damage.Value;
            S.UpgReqExp = (uint)this.ExpReq.Value;
            S.UpgReqLvl = (byte)this.ReqLevel.Value;
            S.SectorSize = (byte)this.SectorSize.Value;
            S.MaxDist = (byte)this.Distance.Value;
            S.EffectLasts = (ushort)this.EffectLasts.Value;
            S.EffectValue = (float)this.EffectValue.Value / 100;
            S.ActivationChance = (byte)this.ActivChance.Value;
            if (Skills.SkillsClass.SkillInfos.Contains(S.ID + " " + S.Level))
                Skills.SkillsClass.SkillInfos.Remove(S.ID + " " + S.Level);
            Skills.SkillsClass.SkillInfos.Add(S.ID + " " + S.Level, S);
            if (this.SkillListBox.Items.Contains(((SkillIDs)S.ID).ToString() + " " + S.ID.ToString() + " " + S.Level.ToString()))
                this.SkillListBox.Items.Remove(((SkillIDs)S.ID).ToString() + " " + S.ID.ToString() + " " + S.Level.ToString());
            this.SkillListBox.Items.Add(((SkillIDs)S.ID).ToString() + " " + S.ID.ToString() + " " + S.Level.ToString());
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            string SelectedI = (string)this.SkillListBox.SelectedItem;
            string[] E = SelectedI.Split(' ');
            if (Skills.SkillsClass.SkillInfos.Contains(ushort.Parse(E[1]) + " " + byte.Parse(E[2])))
                Skills.SkillsClass.SkillInfos.Remove(ushort.Parse(E[1]) + " " + byte.Parse(E[2]));
            if (this.SkillListBox.Items.Contains(((SkillIDs)ushort.Parse(E[1])).ToString() + " " + E[1] + " " + E[2]))
                this.SkillListBox.Items.Remove(((SkillIDs)ushort.Parse(E[1])).ToString() + " " + E[1] + " " + E[2]);
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            string SelectedI = (string)this.SkillListBox.SelectedItem;
            string[] E = SelectedI.Split(' ');
            Skills.SkillsClass.SkillInfo S = (Skills.SkillsClass.SkillInfo)Skills.SkillsClass.SkillInfos[E[1] + " " + E[2]];
            this.SkillIDBox.SelectedIndex = 0;
            this.SkillIDBox.SelectedItem = ((SkillIDs)S.ID).ToString();
            this.LevelNum.Value = S.Level;
            this.ManaCostNum.Value = S.ManaCost;
            this.StaminaCostNum.Value = S.StaminaCost;
            this.ArrowCostNum.Value = S.ArrowsCost;
            this.EndXP.Checked = S.EndsXPWait;
            this.TargetType.SelectedItem = S.Targetting.ToString();
            this.DamageType.SelectedItem = S.Damageing.ToString();
            this.ExtraEff.SelectedItem = S.ExtraEff.ToString();
            this.Damage.Value = S.Damage;
            this.ExpReq.Value = S.UpgReqExp;
            this.ReqLevel.Value = S.UpgReqLvl;
            this.SectorSize.Value = S.SectorSize;
            this.Distance.Value = S.MaxDist;
            this.EffectLasts.Value = S.EffectLasts;
            this.EffectValue.Value = (int)(S.EffectValue * 100);
            this.ActivChance.Value = S.ActivationChance;
        }

        private void SkillIDBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void SkillListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void SkillAddPage_Click(object sender, EventArgs e)
        {

        }

        private void LevelNum_ValueChanged(object sender, EventArgs e)
        {

        }


        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string SelectedI = (string)this.comboBox1.SelectedItem;
            string[] E = SelectedI.Split(' ');
            Skills.SkillsClass.SkillInfo S = (Skills.SkillsClass.SkillInfo)Skills.SkillsClass.SkillInfos[E[0] + " " + E[1]];

            this.SkillIDBox.SelectedIndex = 0;
            this.SkillIDBox.SelectedItem = ((SkillIDs)S.ID).ToString();
            this.LevelNum.Value = S.Level;
            this.ManaCostNum.Value = S.ManaCost;
            this.StaminaCostNum.Value = S.StaminaCost;
            this.ArrowCostNum.Value = S.ArrowsCost;
            this.EndXP.Checked = S.EndsXPWait;
            this.TargetType.SelectedItem = S.Targetting.ToString();
            this.DamageType.SelectedItem = S.Damageing.ToString();
            this.ExtraEff.SelectedItem = S.ExtraEff.ToString();
            this.Damage.Value = S.Damage;
            this.ExpReq.Value = S.UpgReqExp;
            this.ReqLevel.Value = S.UpgReqLvl;
            this.SectorSize.Value = S.SectorSize;
            this.Distance.Value = S.MaxDist;
            this.EffectLasts.Value = S.EffectLasts;
            this.EffectValue.Value = (int)(S.EffectValue * 100);
            this.ActivChance.Value = S.ActivationChance;

            //this.tabQueryPage.BringToFront();
            this.SkillAddPage.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void QueryForSkill_Click(object sender, EventArgs e)
        {
            //
            //this.tabQueryPage.BringToFront();
            //this.tabQueryPage.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string SelectedI = (string)this.comboBox2.SelectedItem;
            string[] E = SelectedI.Split(' ');
            Skills.SkillsClass.SkillInfo S = (Skills.SkillsClass.SkillInfo)Skills.SkillsClass.SkillInfos[E[1] + " " + E[2]];

            this.SkillIDBox.SelectedIndex = 0;
            this.SkillIDBox.SelectedItem = ((SkillIDs)S.ID).ToString();
            this.LevelNum.Value = S.Level;
            this.ManaCostNum.Value = S.ManaCost;
            this.StaminaCostNum.Value = S.StaminaCost;
            this.ArrowCostNum.Value = S.ArrowsCost;
            this.EndXP.Checked = S.EndsXPWait;
            this.TargetType.SelectedItem = S.Targetting.ToString();
            this.DamageType.SelectedItem = S.Damageing.ToString();
            this.ExtraEff.SelectedItem = S.ExtraEff.ToString();
            this.Damage.Value = S.Damage;
            this.ExpReq.Value = S.UpgReqExp;
            this.ReqLevel.Value = S.UpgReqLvl;
            this.SectorSize.Value = S.SectorSize;
            this.Distance.Value = S.MaxDist;
            this.EffectLasts.Value = S.EffectLasts;
            this.EffectValue.Value = (int)(S.EffectValue * 100);
            this.ActivChance.Value = S.ActivationChance;

            //this.tabQueryPage.BringToFront();
            this.SkillAddPage.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Skills.SkillsClass.Save();
            Console.WriteLine("Saved");
            if (label19.Text == "Saved")
                label19.Text = "Saved..>";
            else
                label19.Text = "Saved.";
        }
    }
}
