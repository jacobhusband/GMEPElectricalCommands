using System.Drawing;

namespace AutoCADCommands
{
  partial class Form1
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.PANEL_GRID = new System.Windows.Forms.DataGridView();
      this.DESCRIPTIONLEFT = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.PHASEALEFT = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.PHASEBLEFT = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.BKRLEFT = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.CKTNOLEFT = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.CKTNORIGHT = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.BRKRIGHT = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.PHASEARIGHT = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.PHASEBRIGHT = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.DESCRIPTIONRIGHT = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.BUS_RATING_INPUT = new System.Windows.Forms.TextBox();
      this.MAIN_INPUT = new System.Windows.Forms.TextBox();
      this.PANEL_LOCATION_INPUT = new System.Windows.Forms.TextBox();
      this.PANEL_NAME_INPUT = new System.Windows.Forms.TextBox();
      this.label10 = new System.Windows.Forms.Label();
      this.label11 = new System.Windows.Forms.Label();
      this.label12 = new System.Windows.Forms.Label();
      this.label13 = new System.Windows.Forms.Label();
      this.label14 = new System.Windows.Forms.Label();
      this.label15 = new System.Windows.Forms.Label();
      this.label16 = new System.Windows.Forms.Label();
      this.label17 = new System.Windows.Forms.Label();
      this.label18 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.STATUS_COMBOBOX = new System.Windows.Forms.ComboBox();
      this.MOUNTING_COMBOBOX = new System.Windows.Forms.ComboBox();
      this.WIRE_COMBOBOX = new System.Windows.Forms.ComboBox();
      this.PHASE_COMBOBOX = new System.Windows.Forms.ComboBox();
      this.PHASE_VOLTAGE_COMBOBOX = new System.Windows.Forms.ComboBox();
      this.LINE_VOLTAGE_COMBOBOX = new System.Windows.Forms.ComboBox();
      this.ADD_ROW_BUTTON = new System.Windows.Forms.Button();
      this.DELETE_ROW_BUTTON = new System.Windows.Forms.Button();
      this.PHASE_SUM_GRID = new System.Windows.Forms.DataGridView();
      this.TOTAL_PH_A = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.TOTAL_PH_B = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.TOTAL_VA_GRID = new System.Windows.Forms.DataGridView();
      this.TOTAL_VA = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.LCL_GRID = new System.Windows.Forms.DataGridView();
      this.TOTAL_OTHER_LOAD_GRID = new System.Windows.Forms.DataGridView();
      this.TOTAL_OTHER_LOAD = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.PANEL_LOAD_GRID = new System.Windows.Forms.DataGridView();
      this.PANEL_LOAD = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.FEEDER_AMP_GRID = new System.Windows.Forms.DataGridView();
      this.FEEDER_AMPS = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.CREATE_PANEL_BUTTON = new System.Windows.Forms.Button();
      this.LCL_AT_125PC = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.EQUALS = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.SAVE_PANEL_BUTTON = new System.Windows.Forms.Button();
      this.LARGEST_LCL_INPUT = new System.Windows.Forms.TextBox();
      this.LARGEST_LCL_LABEL = new System.Windows.Forms.Label();
      this.LARGEST_LCL_CHECKBOX = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.PANEL_GRID)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.PHASE_SUM_GRID)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.TOTAL_VA_GRID)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.LCL_GRID)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.TOTAL_OTHER_LOAD_GRID)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.PANEL_LOAD_GRID)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.FEEDER_AMP_GRID)).BeginInit();
      this.SuspendLayout();
      // 
      // PANEL_GRID
      // 
      this.PANEL_GRID.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.PANEL_GRID.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DESCRIPTIONLEFT,
            this.PHASEALEFT,
            this.PHASEBLEFT,
            this.BKRLEFT,
            this.CKTNOLEFT,
            this.CKTNORIGHT,
            this.BRKRIGHT,
            this.PHASEARIGHT,
            this.PHASEBRIGHT,
            this.DESCRIPTIONRIGHT});
      this.PANEL_GRID.Location = new System.Drawing.Point(354, 94);
      this.PANEL_GRID.Name = "PANEL_GRID";
      this.PANEL_GRID.Size = new System.Drawing.Size(1047, 489);
      this.PANEL_GRID.TabIndex = 10;
      // 
      // DESCRIPTIONLEFT
      // 
      this.DESCRIPTIONLEFT.HeaderText = "DESCRIPTION";
      this.DESCRIPTIONLEFT.Name = "DESCRIPTIONLEFT";
      // 
      // PHASEALEFT
      // 
      this.PHASEALEFT.HeaderText = "PH A";
      this.PHASEALEFT.Name = "PHASEALEFT";
      // 
      // PHASEBLEFT
      // 
      this.PHASEBLEFT.HeaderText = "PH B";
      this.PHASEBLEFT.Name = "PHASEBLEFT";
      // 
      // BKRLEFT
      // 
      this.BKRLEFT.HeaderText = "BKR";
      this.BKRLEFT.Name = "BKRLEFT";
      // 
      // CKTNOLEFT
      // 
      this.CKTNOLEFT.HeaderText = "CKT NO";
      this.CKTNOLEFT.Name = "CKTNOLEFT";
      // 
      // CKTNORIGHT
      // 
      this.CKTNORIGHT.HeaderText = "CKT NO";
      this.CKTNORIGHT.Name = "CKTNORIGHT";
      // 
      // BRKRIGHT
      // 
      this.BRKRIGHT.HeaderText = "BKR";
      this.BRKRIGHT.Name = "BRKRIGHT";
      // 
      // PHASEARIGHT
      // 
      this.PHASEARIGHT.HeaderText = "PH A";
      this.PHASEARIGHT.Name = "PHASEARIGHT";
      // 
      // PHASEBRIGHT
      // 
      this.PHASEBRIGHT.HeaderText = "PH B";
      this.PHASEBRIGHT.Name = "PHASEBRIGHT";
      // 
      // DESCRIPTIONRIGHT
      // 
      this.DESCRIPTIONRIGHT.HeaderText = "DESCRIPTION";
      this.DESCRIPTIONRIGHT.Name = "DESCRIPTIONRIGHT";
      // 
      // BUS_RATING_INPUT
      // 
      this.BUS_RATING_INPUT.Location = new System.Drawing.Point(235, 197);
      this.BUS_RATING_INPUT.Name = "BUS_RATING_INPUT";
      this.BUS_RATING_INPUT.Size = new System.Drawing.Size(100, 20);
      this.BUS_RATING_INPUT.TabIndex = 4;
      // 
      // MAIN_INPUT
      // 
      this.MAIN_INPUT.Location = new System.Drawing.Point(235, 171);
      this.MAIN_INPUT.Name = "MAIN_INPUT";
      this.MAIN_INPUT.Size = new System.Drawing.Size(100, 20);
      this.MAIN_INPUT.TabIndex = 3;
      // 
      // PANEL_LOCATION_INPUT
      // 
      this.PANEL_LOCATION_INPUT.Location = new System.Drawing.Point(235, 146);
      this.PANEL_LOCATION_INPUT.Name = "PANEL_LOCATION_INPUT";
      this.PANEL_LOCATION_INPUT.Size = new System.Drawing.Size(100, 20);
      this.PANEL_LOCATION_INPUT.TabIndex = 2;
      // 
      // PANEL_NAME_INPUT
      // 
      this.PANEL_NAME_INPUT.Location = new System.Drawing.Point(235, 121);
      this.PANEL_NAME_INPUT.Name = "PANEL_NAME_INPUT";
      this.PANEL_NAME_INPUT.Size = new System.Drawing.Size(100, 20);
      this.PANEL_NAME_INPUT.TabIndex = 1;
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(145, 331);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(66, 13);
      this.label10.TabIndex = 54;
      this.label10.Text = "MOUNTING";
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Location = new System.Drawing.Point(175, 305);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(36, 13);
      this.label11.TabIndex = 52;
      this.label11.Text = "WIRE";
      // 
      // label12
      // 
      this.label12.AutoSize = true;
      this.label12.Location = new System.Drawing.Point(168, 278);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(43, 13);
      this.label12.TabIndex = 50;
      this.label12.Text = "PHASE";
      // 
      // label13
      // 
      this.label13.AutoSize = true;
      this.label13.Location = new System.Drawing.Point(115, 252);
      this.label13.Name = "label13";
      this.label13.Size = new System.Drawing.Size(96, 13);
      this.label13.TabIndex = 48;
      this.label13.Text = "PHASE VOLTAGE";
      // 
      // label14
      // 
      this.label14.AutoSize = true;
      this.label14.Location = new System.Drawing.Point(127, 226);
      this.label14.Name = "label14";
      this.label14.Size = new System.Drawing.Size(84, 13);
      this.label14.TabIndex = 46;
      this.label14.Text = "LINE VOLTAGE";
      // 
      // label15
      // 
      this.label15.AutoSize = true;
      this.label15.Location = new System.Drawing.Point(158, 200);
      this.label15.Name = "label15";
      this.label15.Size = new System.Drawing.Size(73, 13);
      this.label15.TabIndex = 44;
      this.label15.Text = "BUS RATING";
      // 
      // label16
      // 
      this.label16.AutoSize = true;
      this.label16.Location = new System.Drawing.Point(168, 174);
      this.label16.Name = "label16";
      this.label16.Size = new System.Drawing.Size(63, 13);
      this.label16.TabIndex = 42;
      this.label16.Text = "MAIN(AMP)";
      // 
      // label17
      // 
      this.label17.AutoSize = true;
      this.label17.Location = new System.Drawing.Point(169, 150);
      this.label17.Name = "label17";
      this.label17.Size = new System.Drawing.Size(61, 13);
      this.label17.TabIndex = 40;
      this.label17.Text = "LOCATION";
      // 
      // label18
      // 
      this.label18.AutoSize = true;
      this.label18.Location = new System.Drawing.Point(188, 125);
      this.label18.Name = "label18";
      this.label18.Size = new System.Drawing.Size(42, 13);
      this.label18.TabIndex = 37;
      this.label18.Text = "PANEL";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(161, 99);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(50, 13);
      this.label1.TabIndex = 56;
      this.label1.Text = "STATUS";
      // 
      // STATUS_COMBOBOX
      // 
      this.STATUS_COMBOBOX.FormattingEnabled = true;
      this.STATUS_COMBOBOX.Items.AddRange(new object[] {
            "NEW",
            "EXISTING",
            "RELOCATED"});
      this.STATUS_COMBOBOX.Location = new System.Drawing.Point(214, 95);
      this.STATUS_COMBOBOX.Name = "STATUS_COMBOBOX";
      this.STATUS_COMBOBOX.Size = new System.Drawing.Size(121, 21);
      this.STATUS_COMBOBOX.TabIndex = 0;
      // 
      // MOUNTING_COMBOBOX
      // 
      this.MOUNTING_COMBOBOX.FormattingEnabled = true;
      this.MOUNTING_COMBOBOX.Items.AddRange(new object[] {
            "SURFACE",
            "RECESSED"});
      this.MOUNTING_COMBOBOX.Location = new System.Drawing.Point(214, 327);
      this.MOUNTING_COMBOBOX.Name = "MOUNTING_COMBOBOX";
      this.MOUNTING_COMBOBOX.Size = new System.Drawing.Size(121, 21);
      this.MOUNTING_COMBOBOX.TabIndex = 9;
      // 
      // WIRE_COMBOBOX
      // 
      this.WIRE_COMBOBOX.FormattingEnabled = true;
      this.WIRE_COMBOBOX.Items.AddRange(new object[] {
            "3",
            "4"});
      this.WIRE_COMBOBOX.Location = new System.Drawing.Point(214, 301);
      this.WIRE_COMBOBOX.Name = "WIRE_COMBOBOX";
      this.WIRE_COMBOBOX.Size = new System.Drawing.Size(121, 21);
      this.WIRE_COMBOBOX.TabIndex = 8;
      // 
      // PHASE_COMBOBOX
      // 
      this.PHASE_COMBOBOX.FormattingEnabled = true;
      this.PHASE_COMBOBOX.Items.AddRange(new object[] {
            "1",
            "3"});
      this.PHASE_COMBOBOX.Location = new System.Drawing.Point(214, 275);
      this.PHASE_COMBOBOX.Name = "PHASE_COMBOBOX";
      this.PHASE_COMBOBOX.Size = new System.Drawing.Size(121, 21);
      this.PHASE_COMBOBOX.TabIndex = 7;
      // 
      // PHASE_VOLTAGE_COMBOBOX
      // 
      this.PHASE_VOLTAGE_COMBOBOX.FormattingEnabled = true;
      this.PHASE_VOLTAGE_COMBOBOX.Items.AddRange(new object[] {
            "208",
            "240",
            "480"});
      this.PHASE_VOLTAGE_COMBOBOX.Location = new System.Drawing.Point(214, 249);
      this.PHASE_VOLTAGE_COMBOBOX.Name = "PHASE_VOLTAGE_COMBOBOX";
      this.PHASE_VOLTAGE_COMBOBOX.Size = new System.Drawing.Size(121, 21);
      this.PHASE_VOLTAGE_COMBOBOX.TabIndex = 6;
      // 
      // LINE_VOLTAGE_COMBOBOX
      // 
      this.LINE_VOLTAGE_COMBOBOX.FormattingEnabled = true;
      this.LINE_VOLTAGE_COMBOBOX.Items.AddRange(new object[] {
            "120",
            "277"});
      this.LINE_VOLTAGE_COMBOBOX.Location = new System.Drawing.Point(214, 223);
      this.LINE_VOLTAGE_COMBOBOX.Name = "LINE_VOLTAGE_COMBOBOX";
      this.LINE_VOLTAGE_COMBOBOX.Size = new System.Drawing.Size(121, 21);
      this.LINE_VOLTAGE_COMBOBOX.TabIndex = 5;
      // 
      // ADD_ROW_BUTTON
      // 
      this.ADD_ROW_BUTTON.Location = new System.Drawing.Point(353, 589);
      this.ADD_ROW_BUTTON.Name = "ADD_ROW_BUTTON";
      this.ADD_ROW_BUTTON.Size = new System.Drawing.Size(75, 23);
      this.ADD_ROW_BUTTON.TabIndex = 11;
      this.ADD_ROW_BUTTON.Text = "ADD ROW";
      this.ADD_ROW_BUTTON.UseVisualStyleBackColor = true;
      this.ADD_ROW_BUTTON.Click += new System.EventHandler(this.ADD_ROW_BUTTON_CLICK);
      // 
      // DELETE_ROW_BUTTON
      // 
      this.DELETE_ROW_BUTTON.BackColor = System.Drawing.SystemColors.Control;
      this.DELETE_ROW_BUTTON.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
      this.DELETE_ROW_BUTTON.Location = new System.Drawing.Point(1301, 589);
      this.DELETE_ROW_BUTTON.Name = "DELETE_ROW_BUTTON";
      this.DELETE_ROW_BUTTON.Size = new System.Drawing.Size(100, 23);
      this.DELETE_ROW_BUTTON.TabIndex = 12;
      this.DELETE_ROW_BUTTON.Text = "DELETE ROW";
      this.DELETE_ROW_BUTTON.UseVisualStyleBackColor = false;
      this.DELETE_ROW_BUTTON.Click += new System.EventHandler(this.DELETE_ROW_BUTTON_CLICK);
      // 
      // PHASE_SUM_GRID
      // 
      this.PHASE_SUM_GRID.AllowUserToAddRows = false;
      this.PHASE_SUM_GRID.AllowUserToDeleteRows = false;
      this.PHASE_SUM_GRID.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.PHASE_SUM_GRID.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TOTAL_PH_A,
            this.TOTAL_PH_B});
      this.PHASE_SUM_GRID.Location = new System.Drawing.Point(90, 370);
      this.PHASE_SUM_GRID.Name = "PHASE_SUM_GRID";
      this.PHASE_SUM_GRID.ReadOnly = true;
      this.PHASE_SUM_GRID.Size = new System.Drawing.Size(245, 44);
      this.PHASE_SUM_GRID.TabIndex = 65;
      // 
      // TOTAL_PH_A
      // 
      this.TOTAL_PH_A.HeaderText = "PH A";
      this.TOTAL_PH_A.Name = "TOTAL_PH_A";
      this.TOTAL_PH_A.ReadOnly = true;
      // 
      // TOTAL_PH_B
      // 
      this.TOTAL_PH_B.HeaderText = "PH B";
      this.TOTAL_PH_B.Name = "TOTAL_PH_B";
      this.TOTAL_PH_B.ReadOnly = true;
      // 
      // TOTAL_VA_GRID
      // 
      this.TOTAL_VA_GRID.AllowUserToAddRows = false;
      this.TOTAL_VA_GRID.AllowUserToDeleteRows = false;
      this.TOTAL_VA_GRID.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.TOTAL_VA_GRID.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TOTAL_VA});
      this.TOTAL_VA_GRID.Location = new System.Drawing.Point(191, 420);
      this.TOTAL_VA_GRID.Name = "TOTAL_VA_GRID";
      this.TOTAL_VA_GRID.ReadOnly = true;
      this.TOTAL_VA_GRID.Size = new System.Drawing.Size(144, 42);
      this.TOTAL_VA_GRID.TabIndex = 14;
      // 
      // TOTAL_VA
      // 
      this.TOTAL_VA.HeaderText = "TOTAL VA";
      this.TOTAL_VA.Name = "TOTAL_VA";
      this.TOTAL_VA.ReadOnly = true;
      // 
      // LCL_GRID
      // 
      this.LCL_GRID.AllowUserToAddRows = false;
      this.LCL_GRID.AllowUserToDeleteRows = false;
      this.LCL_GRID.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.LCL_GRID.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LCL_AT_125PC,
            this.EQUALS});
      this.LCL_GRID.Location = new System.Drawing.Point(90, 512);
      this.LCL_GRID.Name = "LCL_GRID";
      this.LCL_GRID.ReadOnly = true;
      this.LCL_GRID.Size = new System.Drawing.Size(245, 42);
      this.LCL_GRID.TabIndex = 15;
      // 
      // TOTAL_OTHER_LOAD_GRID
      // 
      this.TOTAL_OTHER_LOAD_GRID.AllowUserToAddRows = false;
      this.TOTAL_OTHER_LOAD_GRID.AllowUserToDeleteRows = false;
      this.TOTAL_OTHER_LOAD_GRID.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.TOTAL_OTHER_LOAD_GRID.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TOTAL_OTHER_LOAD});
      this.TOTAL_OTHER_LOAD_GRID.Location = new System.Drawing.Point(41, 560);
      this.TOTAL_OTHER_LOAD_GRID.Name = "TOTAL_OTHER_LOAD_GRID";
      this.TOTAL_OTHER_LOAD_GRID.ReadOnly = true;
      this.TOTAL_OTHER_LOAD_GRID.Size = new System.Drawing.Size(144, 57);
      this.TOTAL_OTHER_LOAD_GRID.TabIndex = 16;
      // 
      // TOTAL_OTHER_LOAD
      // 
      this.TOTAL_OTHER_LOAD.HeaderText = "TOTAL OTHER LOAD";
      this.TOTAL_OTHER_LOAD.Name = "TOTAL_OTHER_LOAD";
      this.TOTAL_OTHER_LOAD.ReadOnly = true;
      // 
      // PANEL_LOAD_GRID
      // 
      this.PANEL_LOAD_GRID.AllowUserToAddRows = false;
      this.PANEL_LOAD_GRID.AllowUserToDeleteRows = false;
      this.PANEL_LOAD_GRID.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.PANEL_LOAD_GRID.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PANEL_LOAD});
      this.PANEL_LOAD_GRID.Location = new System.Drawing.Point(191, 560);
      this.PANEL_LOAD_GRID.Name = "PANEL_LOAD_GRID";
      this.PANEL_LOAD_GRID.ReadOnly = true;
      this.PANEL_LOAD_GRID.Size = new System.Drawing.Size(144, 43);
      this.PANEL_LOAD_GRID.TabIndex = 17;
      // 
      // PANEL_LOAD
      // 
      this.PANEL_LOAD.HeaderText = "PANEL LOAD";
      this.PANEL_LOAD.Name = "PANEL_LOAD";
      this.PANEL_LOAD.ReadOnly = true;
      // 
      // FEEDER_AMP_GRID
      // 
      this.FEEDER_AMP_GRID.AllowUserToAddRows = false;
      this.FEEDER_AMP_GRID.AllowUserToDeleteRows = false;
      this.FEEDER_AMP_GRID.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.FEEDER_AMP_GRID.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FEEDER_AMPS});
      this.FEEDER_AMP_GRID.Location = new System.Drawing.Point(191, 609);
      this.FEEDER_AMP_GRID.Name = "FEEDER_AMP_GRID";
      this.FEEDER_AMP_GRID.ReadOnly = true;
      this.FEEDER_AMP_GRID.Size = new System.Drawing.Size(144, 56);
      this.FEEDER_AMP_GRID.TabIndex = 18;
      // 
      // FEEDER_AMPS
      // 
      this.FEEDER_AMPS.HeaderText = "FEEDER AMPS";
      this.FEEDER_AMPS.Name = "FEEDER_AMPS";
      this.FEEDER_AMPS.ReadOnly = true;
      // 
      // CREATE_PANEL_BUTTON
      // 
      this.CREATE_PANEL_BUTTON.Location = new System.Drawing.Point(861, 589);
      this.CREATE_PANEL_BUTTON.Name = "CREATE_PANEL_BUTTON";
      this.CREATE_PANEL_BUTTON.Size = new System.Drawing.Size(126, 23);
      this.CREATE_PANEL_BUTTON.TabIndex = 13;
      this.CREATE_PANEL_BUTTON.Text = "CREATE PANEL";
      this.CREATE_PANEL_BUTTON.UseVisualStyleBackColor = true;
      this.CREATE_PANEL_BUTTON.Click += new System.EventHandler(this.CREATE_PANEL_BUTTON_CLICK);
      // 
      // LCL_AT_125PC
      // 
      this.LCL_AT_125PC.HeaderText = "LCL @ 125%";
      this.LCL_AT_125PC.Name = "LCL_AT_125PC";
      this.LCL_AT_125PC.ReadOnly = true;
      // 
      // EQUALS
      // 
      this.EQUALS.HeaderText = "EQUALS";
      this.EQUALS.Name = "EQUALS";
      this.EQUALS.ReadOnly = true;
      // 
      // SAVE_PANEL_BUTTON
      // 
      this.SAVE_PANEL_BUTTON.Location = new System.Drawing.Point(751, 589);
      this.SAVE_PANEL_BUTTON.Name = "SAVE_PANEL_BUTTON";
      this.SAVE_PANEL_BUTTON.Size = new System.Drawing.Size(104, 23);
      this.SAVE_PANEL_BUTTON.TabIndex = 66;
      this.SAVE_PANEL_BUTTON.Text = "SAVE PANEL";
      this.SAVE_PANEL_BUTTON.UseVisualStyleBackColor = true;
      // 
      // LARGEST_LCL_INPUT
      // 
      this.LARGEST_LCL_INPUT.Location = new System.Drawing.Point(189, 484);
      this.LARGEST_LCL_INPUT.Name = "LARGEST_LCL_INPUT";
      this.LARGEST_LCL_INPUT.Size = new System.Drawing.Size(144, 20);
      this.LARGEST_LCL_INPUT.TabIndex = 67;
      // 
      // LARGEST_LCL_LABEL
      // 
      this.LARGEST_LCL_LABEL.AutoSize = true;
      this.LARGEST_LCL_LABEL.Location = new System.Drawing.Point(138, 467);
      this.LARGEST_LCL_LABEL.Name = "LARGEST_LCL_LABEL";
      this.LARGEST_LCL_LABEL.Size = new System.Drawing.Size(197, 13);
      this.LARGEST_LCL_LABEL.TabIndex = 68;
      this.LARGEST_LCL_LABEL.Text = "LARGEST LONG CONTINUOUS LOAD";
      // 
      // LARGEST_LCL_CHECKBOX
      // 
      this.LARGEST_LCL_CHECKBOX.AutoSize = true;
      this.LARGEST_LCL_CHECKBOX.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.LARGEST_LCL_CHECKBOX.Location = new System.Drawing.Point(115, 487);
      this.LARGEST_LCL_CHECKBOX.Name = "LARGEST_LCL_CHECKBOX";
      this.LARGEST_LCL_CHECKBOX.Size = new System.Drawing.Size(68, 17);
      this.LARGEST_LCL_CHECKBOX.TabIndex = 69;
      this.LARGEST_LCL_CHECKBOX.Text = "ENABLE";
      this.LARGEST_LCL_CHECKBOX.UseVisualStyleBackColor = true;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1485, 739);
      this.Controls.Add(this.LARGEST_LCL_CHECKBOX);
      this.Controls.Add(this.LARGEST_LCL_LABEL);
      this.Controls.Add(this.LARGEST_LCL_INPUT);
      this.Controls.Add(this.SAVE_PANEL_BUTTON);
      this.Controls.Add(this.CREATE_PANEL_BUTTON);
      this.Controls.Add(this.FEEDER_AMP_GRID);
      this.Controls.Add(this.PANEL_LOAD_GRID);
      this.Controls.Add(this.TOTAL_OTHER_LOAD_GRID);
      this.Controls.Add(this.LCL_GRID);
      this.Controls.Add(this.TOTAL_VA_GRID);
      this.Controls.Add(this.PHASE_SUM_GRID);
      this.Controls.Add(this.DELETE_ROW_BUTTON);
      this.Controls.Add(this.ADD_ROW_BUTTON);
      this.Controls.Add(this.LINE_VOLTAGE_COMBOBOX);
      this.Controls.Add(this.PHASE_VOLTAGE_COMBOBOX);
      this.Controls.Add(this.PHASE_COMBOBOX);
      this.Controls.Add(this.WIRE_COMBOBOX);
      this.Controls.Add(this.MOUNTING_COMBOBOX);
      this.Controls.Add(this.STATUS_COMBOBOX);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.PANEL_GRID);
      this.Controls.Add(this.BUS_RATING_INPUT);
      this.Controls.Add(this.MAIN_INPUT);
      this.Controls.Add(this.PANEL_LOCATION_INPUT);
      this.Controls.Add(this.PANEL_NAME_INPUT);
      this.Controls.Add(this.label10);
      this.Controls.Add(this.label11);
      this.Controls.Add(this.label12);
      this.Controls.Add(this.label13);
      this.Controls.Add(this.label14);
      this.Controls.Add(this.label15);
      this.Controls.Add(this.label16);
      this.Controls.Add(this.label17);
      this.Controls.Add(this.label18);
      this.Name = "Form1";
      this.Text = "Panel Schedule";
      ((System.ComponentModel.ISupportInitialize)(this.PANEL_GRID)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.PHASE_SUM_GRID)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.TOTAL_VA_GRID)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.LCL_GRID)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.TOTAL_OTHER_LOAD_GRID)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.PANEL_LOAD_GRID)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.FEEDER_AMP_GRID)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.DataGridView PANEL_GRID;
    private System.Windows.Forms.DataGridViewTextBoxColumn DESCRIPTIONLEFT;
    private System.Windows.Forms.DataGridViewTextBoxColumn PHASEALEFT;
    private System.Windows.Forms.DataGridViewTextBoxColumn PHASEBLEFT;
    private System.Windows.Forms.DataGridViewTextBoxColumn BKRLEFT;
    private System.Windows.Forms.DataGridViewTextBoxColumn CKTNOLEFT;
    private System.Windows.Forms.DataGridViewTextBoxColumn CKTNORIGHT;
    private System.Windows.Forms.DataGridViewTextBoxColumn BRKRIGHT;
    private System.Windows.Forms.DataGridViewTextBoxColumn PHASEARIGHT;
    private System.Windows.Forms.DataGridViewTextBoxColumn PHASEBRIGHT;
    private System.Windows.Forms.DataGridViewTextBoxColumn DESCRIPTIONRIGHT;
    private System.Windows.Forms.TextBox BUS_RATING_INPUT;
    private System.Windows.Forms.TextBox MAIN_INPUT;
    private System.Windows.Forms.TextBox PANEL_LOCATION_INPUT;
    private System.Windows.Forms.TextBox PANEL_NAME_INPUT;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.Label label12;
    private System.Windows.Forms.Label label13;
    private System.Windows.Forms.Label label14;
    private System.Windows.Forms.Label label15;
    private System.Windows.Forms.Label label16;
    private System.Windows.Forms.Label label17;
    private System.Windows.Forms.Label label18;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox STATUS_COMBOBOX;
    private System.Windows.Forms.ComboBox MOUNTING_COMBOBOX;
    private System.Windows.Forms.ComboBox WIRE_COMBOBOX;
    private System.Windows.Forms.ComboBox PHASE_COMBOBOX;
    private System.Windows.Forms.ComboBox PHASE_VOLTAGE_COMBOBOX;
    private System.Windows.Forms.ComboBox LINE_VOLTAGE_COMBOBOX;
    private System.Windows.Forms.Button ADD_ROW_BUTTON;
    private System.Windows.Forms.Button DELETE_ROW_BUTTON;
    private System.Windows.Forms.DataGridView PHASE_SUM_GRID;
    private System.Windows.Forms.DataGridViewTextBoxColumn TOTAL_PH_A;
    private System.Windows.Forms.DataGridViewTextBoxColumn TOTAL_PH_B;
    private System.Windows.Forms.DataGridView TOTAL_VA_GRID;
    private System.Windows.Forms.DataGridViewTextBoxColumn TOTAL_VA;
    private System.Windows.Forms.DataGridView LCL_GRID;
    private System.Windows.Forms.DataGridView TOTAL_OTHER_LOAD_GRID;
    private System.Windows.Forms.DataGridViewTextBoxColumn TOTAL_OTHER_LOAD;
    private System.Windows.Forms.DataGridView PANEL_LOAD_GRID;
    private System.Windows.Forms.DataGridViewTextBoxColumn PANEL_LOAD;
    private System.Windows.Forms.DataGridView FEEDER_AMP_GRID;
    private System.Windows.Forms.DataGridViewTextBoxColumn FEEDER_AMPS;
    private System.Windows.Forms.Button CREATE_PANEL_BUTTON;
    private System.Windows.Forms.DataGridViewTextBoxColumn LCL_AT_125PC;
    private System.Windows.Forms.DataGridViewTextBoxColumn EQUALS;
    private System.Windows.Forms.Button SAVE_PANEL_BUTTON;
    private System.Windows.Forms.TextBox LARGEST_LCL_INPUT;
    private System.Windows.Forms.Label LARGEST_LCL_LABEL;
    private System.Windows.Forms.CheckBox LARGEST_LCL_CHECKBOX;
  }
}

