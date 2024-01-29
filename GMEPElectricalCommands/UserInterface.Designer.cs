namespace ElectricalCommands
{
  partial class UserInterface
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.PANEL_GRID = new System.Windows.Forms.DataGridView();
      this.description_left = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.phase_a_left = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.phase_b_left = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.breaker_left = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.circuit_left = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.circuit_right = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.breaker_right = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.phase_a_right = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.phase_b_right = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.description_right = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.label18 = new System.Windows.Forms.Label();
      this.LARGEST_LCL_CHECKBOX = new System.Windows.Forms.CheckBox();
      this.label17 = new System.Windows.Forms.Label();
      this.LARGEST_LCL_LABEL = new System.Windows.Forms.Label();
      this.label16 = new System.Windows.Forms.Label();
      this.label15 = new System.Windows.Forms.Label();
      this.CREATE_PANEL_BUTTON = new System.Windows.Forms.Button();
      this.label14 = new System.Windows.Forms.Label();
      this.FEEDER_AMP_GRID = new System.Windows.Forms.DataGridView();
      this.FEEDER_AMPS = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.label13 = new System.Windows.Forms.Label();
      this.PANEL_LOAD_GRID = new System.Windows.Forms.DataGridView();
      this.PANEL_LOAD = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.label12 = new System.Windows.Forms.Label();
      this.TOTAL_OTHER_LOAD_GRID = new System.Windows.Forms.DataGridView();
      this.TOTAL_OTHER_LOAD = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.label11 = new System.Windows.Forms.Label();
      this.LCL_GRID = new System.Windows.Forms.DataGridView();
      this.LCL_AT_100PC = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.LCL_AT_125PC = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.label10 = new System.Windows.Forms.Label();
      this.TOTAL_VA_GRID = new System.Windows.Forms.DataGridView();
      this.TOTAL_VA = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.PANEL_NAME_INPUT = new System.Windows.Forms.TextBox();
      this.PHASE_SUM_GRID = new System.Windows.Forms.DataGridView();
      this.TOTAL_PH_A = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.TOTAL_PH_B = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.PANEL_LOCATION_INPUT = new System.Windows.Forms.TextBox();
      this.DELETE_ROW_BUTTON = new System.Windows.Forms.Button();
      this.MAIN_INPUT = new System.Windows.Forms.TextBox();
      this.ADD_ROW_BUTTON = new System.Windows.Forms.Button();
      this.BUS_RATING_INPUT = new System.Windows.Forms.TextBox();
      this.LINE_VOLTAGE_COMBOBOX = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.PHASE_VOLTAGE_COMBOBOX = new System.Windows.Forms.ComboBox();
      this.STATUS_COMBOBOX = new System.Windows.Forms.ComboBox();
      this.PHASE_COMBOBOX = new System.Windows.Forms.ComboBox();
      this.MOUNTING_COMBOBOX = new System.Windows.Forms.ComboBox();
      this.WIRE_COMBOBOX = new System.Windows.Forms.ComboBox();
      this.DELETE_PANEL_BUTTON = new System.Windows.Forms.Button();
      this.INFO_LABEL = new System.Windows.Forms.Label();
      this.APPLY_BUTTON = new System.Windows.Forms.Button();
      this.APPLY_COMBOBOX = new System.Windows.Forms.ComboBox();
      this.MODIFY_NOTES_BUTTON = new System.Windows.Forms.Button();
      this.CUSTOM_TITLE_TEXT = new System.Windows.Forms.TextBox();
      this.CUSTOM_TITLE_LABEL = new System.Windows.Forms.Label();
      this.REMOVE_NOTE_BUTTON = new System.Windows.Forms.Button();
      this.MAX_DESCRIPTION_CELL_CHAR_LABEL = new System.Windows.Forms.Label();
      this.MAX_DESCRIPTION_CELL_CHAR_TEXTBOX = new System.Windows.Forms.TextBox();
      this.AUTO_CHECKBOX = new System.Windows.Forms.CheckBox();
      this.LARGEST_LCL_INPUT = new System.Windows.Forms.TextBox();
      this.ALL_EXISTING_BUTTON = new System.Windows.Forms.Button();
      this.REMOVE_EXISTING_CHECKBOX = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.PANEL_GRID)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.FEEDER_AMP_GRID)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.PANEL_LOAD_GRID)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.TOTAL_OTHER_LOAD_GRID)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.LCL_GRID)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.TOTAL_VA_GRID)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.PHASE_SUM_GRID)).BeginInit();
      this.SuspendLayout();
      // 
      // PANEL_GRID
      // 
      this.PANEL_GRID.AllowUserToResizeColumns = false;
      this.PANEL_GRID.AllowUserToResizeRows = false;
      this.PANEL_GRID.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.PANEL_GRID.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.description_left,
            this.phase_a_left,
            this.phase_b_left,
            this.breaker_left,
            this.circuit_left,
            this.circuit_right,
            this.breaker_right,
            this.phase_a_right,
            this.phase_b_right,
            this.description_right});
      this.PANEL_GRID.Location = new System.Drawing.Point(316, 43);
      this.PANEL_GRID.Name = "PANEL_GRID";
      this.PANEL_GRID.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
      this.PANEL_GRID.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.PANEL_GRID.Size = new System.Drawing.Size(1047, 489);
      this.PANEL_GRID.TabIndex = 154;
      // 
      // description_left
      // 
      this.description_left.HeaderText = "DESCRIPTION";
      this.description_left.Name = "description_left";
      // 
      // phase_a_left
      // 
      this.phase_a_left.HeaderText = "PH A";
      this.phase_a_left.Name = "phase_a_left";
      // 
      // phase_b_left
      // 
      this.phase_b_left.HeaderText = "PH B";
      this.phase_b_left.Name = "phase_b_left";
      // 
      // breaker_left
      // 
      this.breaker_left.HeaderText = "BKR";
      this.breaker_left.Name = "breaker_left";
      // 
      // circuit_left
      // 
      this.circuit_left.HeaderText = "CKT NO";
      this.circuit_left.Name = "circuit_left";
      // 
      // circuit_right
      // 
      this.circuit_right.HeaderText = "CKT NO";
      this.circuit_right.Name = "circuit_right";
      // 
      // breaker_right
      // 
      this.breaker_right.HeaderText = "BKR";
      this.breaker_right.Name = "breaker_right";
      // 
      // phase_a_right
      // 
      this.phase_a_right.HeaderText = "PH A";
      this.phase_a_right.Name = "phase_a_right";
      // 
      // phase_b_right
      // 
      this.phase_b_right.HeaderText = "PH B";
      this.phase_b_right.Name = "phase_b_right";
      // 
      // description_right
      // 
      this.description_right.HeaderText = "DESCRIPTION";
      this.description_right.Name = "description_right";
      // 
      // label18
      // 
      this.label18.AutoSize = true;
      this.label18.Location = new System.Drawing.Point(150, 74);
      this.label18.Name = "label18";
      this.label18.Size = new System.Drawing.Size(42, 13);
      this.label18.TabIndex = 163;
      this.label18.Text = "PANEL";
      // 
      // LARGEST_LCL_CHECKBOX
      // 
      this.LARGEST_LCL_CHECKBOX.AutoSize = true;
      this.LARGEST_LCL_CHECKBOX.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.LARGEST_LCL_CHECKBOX.Location = new System.Drawing.Point(81, 436);
      this.LARGEST_LCL_CHECKBOX.Name = "LARGEST_LCL_CHECKBOX";
      this.LARGEST_LCL_CHECKBOX.Size = new System.Drawing.Size(68, 17);
      this.LARGEST_LCL_CHECKBOX.TabIndex = 176;
      this.LARGEST_LCL_CHECKBOX.Text = "ENABLE";
      this.LARGEST_LCL_CHECKBOX.UseVisualStyleBackColor = true;
      this.LARGEST_LCL_CHECKBOX.CheckedChanged += new System.EventHandler(this.LARGEST_LCL_CHECKBOX_CheckedChanged);
      // 
      // label17
      // 
      this.label17.AutoSize = true;
      this.label17.Location = new System.Drawing.Point(131, 99);
      this.label17.Name = "label17";
      this.label17.Size = new System.Drawing.Size(61, 13);
      this.label17.TabIndex = 164;
      this.label17.Text = "LOCATION";
      // 
      // LARGEST_LCL_LABEL
      // 
      this.LARGEST_LCL_LABEL.AutoSize = true;
      this.LARGEST_LCL_LABEL.Location = new System.Drawing.Point(22, 417);
      this.LARGEST_LCL_LABEL.Name = "LARGEST_LCL_LABEL";
      this.LARGEST_LCL_LABEL.Size = new System.Drawing.Size(268, 13);
      this.LARGEST_LCL_LABEL.TabIndex = 175;
      this.LARGEST_LCL_LABEL.Text = "LARGEST LONG CONTINUOUS LOAD (LCL @ 100%)";
      // 
      // label16
      // 
      this.label16.AutoSize = true;
      this.label16.Location = new System.Drawing.Point(141, 123);
      this.label16.Name = "label16";
      this.label16.Size = new System.Drawing.Size(50, 13);
      this.label16.TabIndex = 165;
      this.label16.Text = "MAIN (A)";
      // 
      // label15
      // 
      this.label15.AutoSize = true;
      this.label15.Location = new System.Drawing.Point(102, 149);
      this.label15.Name = "label15";
      this.label15.Size = new System.Drawing.Size(89, 13);
      this.label15.TabIndex = 166;
      this.label15.Text = "BUS RATING (A)";
      // 
      // CREATE_PANEL_BUTTON
      // 
      this.CREATE_PANEL_BUTTON.Location = new System.Drawing.Point(396, 538);
      this.CREATE_PANEL_BUTTON.Name = "CREATE_PANEL_BUTTON";
      this.CREATE_PANEL_BUTTON.Size = new System.Drawing.Size(103, 23);
      this.CREATE_PANEL_BUTTON.TabIndex = 157;
      this.CREATE_PANEL_BUTTON.Text = "CREATE PANEL";
      this.CREATE_PANEL_BUTTON.UseVisualStyleBackColor = true;
      this.CREATE_PANEL_BUTTON.Click += new System.EventHandler(this.CREATE_PANEL_BUTTON_Click);
      // 
      // label14
      // 
      this.label14.AutoSize = true;
      this.label14.Location = new System.Drawing.Point(70, 175);
      this.label14.Name = "label14";
      this.label14.Size = new System.Drawing.Size(100, 13);
      this.label14.TabIndex = 167;
      this.label14.Text = "LINE VOLTAGE (V)";
      // 
      // FEEDER_AMP_GRID
      // 
      this.FEEDER_AMP_GRID.AllowUserToAddRows = false;
      this.FEEDER_AMP_GRID.AllowUserToDeleteRows = false;
      this.FEEDER_AMP_GRID.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.FEEDER_AMP_GRID.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FEEDER_AMPS});
      this.FEEDER_AMP_GRID.Location = new System.Drawing.Point(122, 605);
      this.FEEDER_AMP_GRID.Name = "FEEDER_AMP_GRID";
      this.FEEDER_AMP_GRID.ReadOnly = true;
      this.FEEDER_AMP_GRID.Size = new System.Drawing.Size(175, 40);
      this.FEEDER_AMP_GRID.TabIndex = 162;
      // 
      // FEEDER_AMPS
      // 
      this.FEEDER_AMPS.HeaderText = "FEEDER AMPS (A)";
      this.FEEDER_AMPS.Name = "FEEDER_AMPS";
      this.FEEDER_AMPS.ReadOnly = true;
      this.FEEDER_AMPS.Width = 130;
      // 
      // label13
      // 
      this.label13.AutoSize = true;
      this.label13.Location = new System.Drawing.Point(58, 201);
      this.label13.Name = "label13";
      this.label13.Size = new System.Drawing.Size(112, 13);
      this.label13.TabIndex = 168;
      this.label13.Text = "PHASE VOLTAGE (V)";
      // 
      // PANEL_LOAD_GRID
      // 
      this.PANEL_LOAD_GRID.AllowUserToAddRows = false;
      this.PANEL_LOAD_GRID.AllowUserToDeleteRows = false;
      this.PANEL_LOAD_GRID.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.PANEL_LOAD_GRID.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PANEL_LOAD});
      this.PANEL_LOAD_GRID.Location = new System.Drawing.Point(114, 556);
      this.PANEL_LOAD_GRID.Name = "PANEL_LOAD_GRID";
      this.PANEL_LOAD_GRID.ReadOnly = true;
      this.PANEL_LOAD_GRID.Size = new System.Drawing.Size(183, 43);
      this.PANEL_LOAD_GRID.TabIndex = 161;
      // 
      // PANEL_LOAD
      // 
      this.PANEL_LOAD.HeaderText = "PANEL LOAD (KVA)";
      this.PANEL_LOAD.Name = "PANEL_LOAD";
      this.PANEL_LOAD.ReadOnly = true;
      this.PANEL_LOAD.Width = 140;
      // 
      // label12
      // 
      this.label12.AutoSize = true;
      this.label12.Location = new System.Drawing.Point(130, 227);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(43, 13);
      this.label12.TabIndex = 169;
      this.label12.Text = "PHASE";
      // 
      // TOTAL_OTHER_LOAD_GRID
      // 
      this.TOTAL_OTHER_LOAD_GRID.AllowUserToAddRows = false;
      this.TOTAL_OTHER_LOAD_GRID.AllowUserToDeleteRows = false;
      this.TOTAL_OTHER_LOAD_GRID.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.TOTAL_OTHER_LOAD_GRID.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TOTAL_OTHER_LOAD});
      this.TOTAL_OTHER_LOAD_GRID.Location = new System.Drawing.Point(83, 509);
      this.TOTAL_OTHER_LOAD_GRID.Name = "TOTAL_OTHER_LOAD_GRID";
      this.TOTAL_OTHER_LOAD_GRID.ReadOnly = true;
      this.TOTAL_OTHER_LOAD_GRID.Size = new System.Drawing.Size(214, 41);
      this.TOTAL_OTHER_LOAD_GRID.TabIndex = 160;
      // 
      // TOTAL_OTHER_LOAD
      // 
      this.TOTAL_OTHER_LOAD.HeaderText = "TOTAL OTHER LOAD (VA)";
      this.TOTAL_OTHER_LOAD.Name = "TOTAL_OTHER_LOAD";
      this.TOTAL_OTHER_LOAD.ReadOnly = true;
      this.TOTAL_OTHER_LOAD.Width = 170;
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Location = new System.Drawing.Point(137, 254);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(36, 13);
      this.label11.TabIndex = 170;
      this.label11.Text = "WIRE";
      // 
      // LCL_GRID
      // 
      this.LCL_GRID.AllowUserToAddRows = false;
      this.LCL_GRID.AllowUserToDeleteRows = false;
      this.LCL_GRID.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.LCL_GRID.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LCL_AT_100PC,
            this.LCL_AT_125PC});
      this.LCL_GRID.Location = new System.Drawing.Point(13, 461);
      this.LCL_GRID.Name = "LCL_GRID";
      this.LCL_GRID.ReadOnly = true;
      this.LCL_GRID.Size = new System.Drawing.Size(284, 42);
      this.LCL_GRID.TabIndex = 159;
      // 
      // LCL_AT_100PC
      // 
      this.LCL_AT_100PC.HeaderText = "LCL @ 100% (VA)";
      this.LCL_AT_100PC.Name = "LCL_AT_100PC";
      this.LCL_AT_100PC.ReadOnly = true;
      this.LCL_AT_100PC.Width = 120;
      // 
      // LCL_AT_125PC
      // 
      this.LCL_AT_125PC.HeaderText = "LCL @ 125% (VA)";
      this.LCL_AT_125PC.Name = "LCL_AT_125PC";
      this.LCL_AT_125PC.ReadOnly = true;
      this.LCL_AT_125PC.Width = 120;
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(107, 280);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(66, 13);
      this.label10.TabIndex = 171;
      this.label10.Text = "MOUNTING";
      // 
      // TOTAL_VA_GRID
      // 
      this.TOTAL_VA_GRID.AllowUserToAddRows = false;
      this.TOTAL_VA_GRID.AllowUserToDeleteRows = false;
      this.TOTAL_VA_GRID.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.TOTAL_VA_GRID.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TOTAL_VA});
      this.TOTAL_VA_GRID.Location = new System.Drawing.Point(153, 369);
      this.TOTAL_VA_GRID.Name = "TOTAL_VA_GRID";
      this.TOTAL_VA_GRID.ReadOnly = true;
      this.TOTAL_VA_GRID.Size = new System.Drawing.Size(144, 42);
      this.TOTAL_VA_GRID.TabIndex = 158;
      // 
      // TOTAL_VA
      // 
      this.TOTAL_VA.HeaderText = "TOTAL (VA)";
      this.TOTAL_VA.Name = "TOTAL_VA";
      this.TOTAL_VA.ReadOnly = true;
      // 
      // PANEL_NAME_INPUT
      // 
      this.PANEL_NAME_INPUT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.PANEL_NAME_INPUT.Location = new System.Drawing.Point(197, 70);
      this.PANEL_NAME_INPUT.Name = "PANEL_NAME_INPUT";
      this.PANEL_NAME_INPUT.Size = new System.Drawing.Size(100, 20);
      this.PANEL_NAME_INPUT.TabIndex = 145;
      // 
      // PHASE_SUM_GRID
      // 
      this.PHASE_SUM_GRID.AllowUserToAddRows = false;
      this.PHASE_SUM_GRID.AllowUserToDeleteRows = false;
      this.PHASE_SUM_GRID.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.PHASE_SUM_GRID.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TOTAL_PH_A,
            this.TOTAL_PH_B});
      this.PHASE_SUM_GRID.Location = new System.Drawing.Point(52, 319);
      this.PHASE_SUM_GRID.Name = "PHASE_SUM_GRID";
      this.PHASE_SUM_GRID.ReadOnly = true;
      this.PHASE_SUM_GRID.Size = new System.Drawing.Size(245, 44);
      this.PHASE_SUM_GRID.TabIndex = 173;
      // 
      // TOTAL_PH_A
      // 
      this.TOTAL_PH_A.HeaderText = "PH A (VA)";
      this.TOTAL_PH_A.Name = "TOTAL_PH_A";
      this.TOTAL_PH_A.ReadOnly = true;
      // 
      // TOTAL_PH_B
      // 
      this.TOTAL_PH_B.HeaderText = "PH B (VA)";
      this.TOTAL_PH_B.Name = "TOTAL_PH_B";
      this.TOTAL_PH_B.ReadOnly = true;
      // 
      // PANEL_LOCATION_INPUT
      // 
      this.PANEL_LOCATION_INPUT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.PANEL_LOCATION_INPUT.Location = new System.Drawing.Point(197, 95);
      this.PANEL_LOCATION_INPUT.Name = "PANEL_LOCATION_INPUT";
      this.PANEL_LOCATION_INPUT.Size = new System.Drawing.Size(100, 20);
      this.PANEL_LOCATION_INPUT.TabIndex = 146;
      // 
      // DELETE_ROW_BUTTON
      // 
      this.DELETE_ROW_BUTTON.BackColor = System.Drawing.SystemColors.Control;
      this.DELETE_ROW_BUTTON.ForeColor = System.Drawing.Color.DarkRed;
      this.DELETE_ROW_BUTTON.Location = new System.Drawing.Point(1263, 538);
      this.DELETE_ROW_BUTTON.Name = "DELETE_ROW_BUTTON";
      this.DELETE_ROW_BUTTON.Size = new System.Drawing.Size(100, 23);
      this.DELETE_ROW_BUTTON.TabIndex = 156;
      this.DELETE_ROW_BUTTON.Text = "DELETE ROW";
      this.DELETE_ROW_BUTTON.UseVisualStyleBackColor = false;
      this.DELETE_ROW_BUTTON.Click += new System.EventHandler(this.DELETE_ROW_BUTTON_Click);
      // 
      // MAIN_INPUT
      // 
      this.MAIN_INPUT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.MAIN_INPUT.Location = new System.Drawing.Point(197, 120);
      this.MAIN_INPUT.Name = "MAIN_INPUT";
      this.MAIN_INPUT.Size = new System.Drawing.Size(100, 20);
      this.MAIN_INPUT.TabIndex = 147;
      // 
      // ADD_ROW_BUTTON
      // 
      this.ADD_ROW_BUTTON.Location = new System.Drawing.Point(315, 538);
      this.ADD_ROW_BUTTON.Name = "ADD_ROW_BUTTON";
      this.ADD_ROW_BUTTON.Size = new System.Drawing.Size(75, 23);
      this.ADD_ROW_BUTTON.TabIndex = 155;
      this.ADD_ROW_BUTTON.Text = "ADD ROW";
      this.ADD_ROW_BUTTON.UseVisualStyleBackColor = true;
      this.ADD_ROW_BUTTON.Click += new System.EventHandler(this.ADD_ROW_BUTTON_Click);
      // 
      // BUS_RATING_INPUT
      // 
      this.BUS_RATING_INPUT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.BUS_RATING_INPUT.Location = new System.Drawing.Point(197, 146);
      this.BUS_RATING_INPUT.Name = "BUS_RATING_INPUT";
      this.BUS_RATING_INPUT.Size = new System.Drawing.Size(100, 20);
      this.BUS_RATING_INPUT.TabIndex = 148;
      // 
      // LINE_VOLTAGE_COMBOBOX
      // 
      this.LINE_VOLTAGE_COMBOBOX.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.LINE_VOLTAGE_COMBOBOX.FormattingEnabled = true;
      this.LINE_VOLTAGE_COMBOBOX.Items.AddRange(new object[] {
            "120",
            "277"});
      this.LINE_VOLTAGE_COMBOBOX.Location = new System.Drawing.Point(176, 172);
      this.LINE_VOLTAGE_COMBOBOX.Name = "LINE_VOLTAGE_COMBOBOX";
      this.LINE_VOLTAGE_COMBOBOX.Size = new System.Drawing.Size(121, 21);
      this.LINE_VOLTAGE_COMBOBOX.TabIndex = 149;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(62, 47);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(108, 13);
      this.label1.TabIndex = 172;
      this.label1.Text = "STATUS (N, EX, RE)";
      // 
      // PHASE_VOLTAGE_COMBOBOX
      // 
      this.PHASE_VOLTAGE_COMBOBOX.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.PHASE_VOLTAGE_COMBOBOX.FormattingEnabled = true;
      this.PHASE_VOLTAGE_COMBOBOX.Items.AddRange(new object[] {
            "208",
            "240",
            "480"});
      this.PHASE_VOLTAGE_COMBOBOX.Location = new System.Drawing.Point(176, 198);
      this.PHASE_VOLTAGE_COMBOBOX.Name = "PHASE_VOLTAGE_COMBOBOX";
      this.PHASE_VOLTAGE_COMBOBOX.Size = new System.Drawing.Size(121, 21);
      this.PHASE_VOLTAGE_COMBOBOX.TabIndex = 150;
      // 
      // STATUS_COMBOBOX
      // 
      this.STATUS_COMBOBOX.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.STATUS_COMBOBOX.FormattingEnabled = true;
      this.STATUS_COMBOBOX.Items.AddRange(new object[] {
            "NEW",
            "EXISTING",
            "RELOCATED"});
      this.STATUS_COMBOBOX.Location = new System.Drawing.Point(176, 44);
      this.STATUS_COMBOBOX.Name = "STATUS_COMBOBOX";
      this.STATUS_COMBOBOX.Size = new System.Drawing.Size(121, 21);
      this.STATUS_COMBOBOX.TabIndex = 144;
      this.STATUS_COMBOBOX.SelectedIndexChanged += new System.EventHandler(this.STATUS_COMBOBOX_SelectedIndexChanged);
      // 
      // PHASE_COMBOBOX
      // 
      this.PHASE_COMBOBOX.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.PHASE_COMBOBOX.Enabled = false;
      this.PHASE_COMBOBOX.FormattingEnabled = true;
      this.PHASE_COMBOBOX.Items.AddRange(new object[] {
            "1",
            "3"});
      this.PHASE_COMBOBOX.Location = new System.Drawing.Point(176, 224);
      this.PHASE_COMBOBOX.Name = "PHASE_COMBOBOX";
      this.PHASE_COMBOBOX.Size = new System.Drawing.Size(121, 21);
      this.PHASE_COMBOBOX.TabIndex = 151;
      // 
      // MOUNTING_COMBOBOX
      // 
      this.MOUNTING_COMBOBOX.FormattingEnabled = true;
      this.MOUNTING_COMBOBOX.Items.AddRange(new object[] {
            "SURFACE",
            "RECESSED"});
      this.MOUNTING_COMBOBOX.Location = new System.Drawing.Point(176, 276);
      this.MOUNTING_COMBOBOX.Name = "MOUNTING_COMBOBOX";
      this.MOUNTING_COMBOBOX.Size = new System.Drawing.Size(121, 21);
      this.MOUNTING_COMBOBOX.TabIndex = 153;
      // 
      // WIRE_COMBOBOX
      // 
      this.WIRE_COMBOBOX.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.WIRE_COMBOBOX.Enabled = false;
      this.WIRE_COMBOBOX.FormattingEnabled = true;
      this.WIRE_COMBOBOX.Items.AddRange(new object[] {
            "3",
            "4"});
      this.WIRE_COMBOBOX.Location = new System.Drawing.Point(176, 250);
      this.WIRE_COMBOBOX.Name = "WIRE_COMBOBOX";
      this.WIRE_COMBOBOX.Size = new System.Drawing.Size(121, 21);
      this.WIRE_COMBOBOX.TabIndex = 152;
      // 
      // DELETE_PANEL_BUTTON
      // 
      this.DELETE_PANEL_BUTTON.BackColor = System.Drawing.SystemColors.Control;
      this.DELETE_PANEL_BUTTON.ForeColor = System.Drawing.Color.DarkRed;
      this.DELETE_PANEL_BUTTON.Location = new System.Drawing.Point(1252, 14);
      this.DELETE_PANEL_BUTTON.Name = "DELETE_PANEL_BUTTON";
      this.DELETE_PANEL_BUTTON.Size = new System.Drawing.Size(111, 23);
      this.DELETE_PANEL_BUTTON.TabIndex = 179;
      this.DELETE_PANEL_BUTTON.Text = "DELETE PANEL";
      this.DELETE_PANEL_BUTTON.UseVisualStyleBackColor = false;
      this.DELETE_PANEL_BUTTON.Click += new System.EventHandler(this.DELETE_PANEL_BUTTON_Click);
      // 
      // INFO_LABEL
      // 
      this.INFO_LABEL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
      this.INFO_LABEL.AutoSize = true;
      this.INFO_LABEL.Location = new System.Drawing.Point(313, 19);
      this.INFO_LABEL.Name = "INFO_LABEL";
      this.INFO_LABEL.Size = new System.Drawing.Size(29, 13);
      this.INFO_LABEL.TabIndex = 180;
      this.INFO_LABEL.Text = "label";
      this.INFO_LABEL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.INFO_LABEL.Click += new System.EventHandler(this.INFO_LABEL_CLICK);
      // 
      // APPLY_BUTTON
      // 
      this.APPLY_BUTTON.Location = new System.Drawing.Point(886, 623);
      this.APPLY_BUTTON.Name = "APPLY_BUTTON";
      this.APPLY_BUTTON.Size = new System.Drawing.Size(86, 23);
      this.APPLY_BUTTON.TabIndex = 181;
      this.APPLY_BUTTON.Text = "APPLY NOTE";
      this.APPLY_BUTTON.UseVisualStyleBackColor = true;
      this.APPLY_BUTTON.Click += new System.EventHandler(this.APPLY_BUTTON_Click);
      // 
      // APPLY_COMBOBOX
      // 
      this.APPLY_COMBOBOX.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.APPLY_COMBOBOX.FormattingEnabled = true;
      this.APPLY_COMBOBOX.Location = new System.Drawing.Point(423, 624);
      this.APPLY_COMBOBOX.Name = "APPLY_COMBOBOX";
      this.APPLY_COMBOBOX.Size = new System.Drawing.Size(457, 21);
      this.APPLY_COMBOBOX.TabIndex = 182;
      this.APPLY_COMBOBOX.SelectedIndexChanged += new System.EventHandler(this.APPLY_COMBOBOX_SelectedIndexChanged);
      // 
      // MODIFY_NOTES_BUTTON
      // 
      this.MODIFY_NOTES_BUTTON.Location = new System.Drawing.Point(316, 623);
      this.MODIFY_NOTES_BUTTON.Name = "MODIFY_NOTES_BUTTON";
      this.MODIFY_NOTES_BUTTON.Size = new System.Drawing.Size(101, 23);
      this.MODIFY_NOTES_BUTTON.TabIndex = 183;
      this.MODIFY_NOTES_BUTTON.Text = "MODIFY NOTES";
      this.MODIFY_NOTES_BUTTON.UseVisualStyleBackColor = true;
      this.MODIFY_NOTES_BUTTON.Click += new System.EventHandler(this.NOTES_BUTTON_Click);
      // 
      // CUSTOM_TITLE_TEXT
      // 
      this.CUSTOM_TITLE_TEXT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.CUSTOM_TITLE_TEXT.Location = new System.Drawing.Point(1170, 624);
      this.CUSTOM_TITLE_TEXT.Name = "CUSTOM_TITLE_TEXT";
      this.CUSTOM_TITLE_TEXT.Size = new System.Drawing.Size(193, 20);
      this.CUSTOM_TITLE_TEXT.TabIndex = 184;
      // 
      // CUSTOM_TITLE_LABEL
      // 
      this.CUSTOM_TITLE_LABEL.AutoSize = true;
      this.CUSTOM_TITLE_LABEL.Location = new System.Drawing.Point(1079, 628);
      this.CUSTOM_TITLE_LABEL.Name = "CUSTOM_TITLE_LABEL";
      this.CUSTOM_TITLE_LABEL.Size = new System.Drawing.Size(86, 13);
      this.CUSTOM_TITLE_LABEL.TabIndex = 185;
      this.CUSTOM_TITLE_LABEL.Text = "CUSTOM TITLE";
      // 
      // REMOVE_NOTE_BUTTON
      // 
      this.REMOVE_NOTE_BUTTON.Location = new System.Drawing.Point(977, 623);
      this.REMOVE_NOTE_BUTTON.Name = "REMOVE_NOTE_BUTTON";
      this.REMOVE_NOTE_BUTTON.Size = new System.Drawing.Size(96, 23);
      this.REMOVE_NOTE_BUTTON.TabIndex = 186;
      this.REMOVE_NOTE_BUTTON.Text = "REMOVE NOTE";
      this.REMOVE_NOTE_BUTTON.UseVisualStyleBackColor = true;
      this.REMOVE_NOTE_BUTTON.Click += new System.EventHandler(this.REMOVE_NOTE_BUTTON_Click);
      // 
      // MAX_DESCRIPTION_CELL_CHAR_LABEL
      // 
      this.MAX_DESCRIPTION_CELL_CHAR_LABEL.AutoSize = true;
      this.MAX_DESCRIPTION_CELL_CHAR_LABEL.BackColor = System.Drawing.Color.Transparent;
      this.MAX_DESCRIPTION_CELL_CHAR_LABEL.ForeColor = System.Drawing.SystemColors.ControlText;
      this.MAX_DESCRIPTION_CELL_CHAR_LABEL.Location = new System.Drawing.Point(1078, 604);
      this.MAX_DESCRIPTION_CELL_CHAR_LABEL.Margin = new System.Windows.Forms.Padding(0);
      this.MAX_DESCRIPTION_CELL_CHAR_LABEL.Name = "MAX_DESCRIPTION_CELL_CHAR_LABEL";
      this.MAX_DESCRIPTION_CELL_CHAR_LABEL.Size = new System.Drawing.Size(240, 13);
      this.MAX_DESCRIPTION_CELL_CHAR_LABEL.TabIndex = 189;
      this.MAX_DESCRIPTION_CELL_CHAR_LABEL.Text = "MAXIMUM DESCRIPTION CELL CHARACTERS";
      // 
      // MAX_DESCRIPTION_CELL_CHAR_TEXTBOX
      // 
      this.MAX_DESCRIPTION_CELL_CHAR_TEXTBOX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.MAX_DESCRIPTION_CELL_CHAR_TEXTBOX.Location = new System.Drawing.Point(1324, 600);
      this.MAX_DESCRIPTION_CELL_CHAR_TEXTBOX.Name = "MAX_DESCRIPTION_CELL_CHAR_TEXTBOX";
      this.MAX_DESCRIPTION_CELL_CHAR_TEXTBOX.Size = new System.Drawing.Size(39, 20);
      this.MAX_DESCRIPTION_CELL_CHAR_TEXTBOX.TabIndex = 190;
      this.MAX_DESCRIPTION_CELL_CHAR_TEXTBOX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // AUTO_CHECKBOX
      // 
      this.AUTO_CHECKBOX.AutoSize = true;
      this.AUTO_CHECKBOX.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.AUTO_CHECKBOX.Checked = true;
      this.AUTO_CHECKBOX.CheckState = System.Windows.Forms.CheckState.Checked;
      this.AUTO_CHECKBOX.Location = new System.Drawing.Point(22, 436);
      this.AUTO_CHECKBOX.Name = "AUTO_CHECKBOX";
      this.AUTO_CHECKBOX.Size = new System.Drawing.Size(56, 17);
      this.AUTO_CHECKBOX.TabIndex = 191;
      this.AUTO_CHECKBOX.Text = "AUTO";
      this.AUTO_CHECKBOX.UseVisualStyleBackColor = true;
      this.AUTO_CHECKBOX.CheckedChanged += new System.EventHandler(this.AUTO_CHECKBOX_CheckedChanged);
      // 
      // LARGEST_LCL_INPUT
      // 
      this.LARGEST_LCL_INPUT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.LARGEST_LCL_INPUT.Enabled = false;
      this.LARGEST_LCL_INPUT.Location = new System.Drawing.Point(155, 434);
      this.LARGEST_LCL_INPUT.Name = "LARGEST_LCL_INPUT";
      this.LARGEST_LCL_INPUT.Size = new System.Drawing.Size(142, 20);
      this.LARGEST_LCL_INPUT.TabIndex = 192;
      // 
      // ALL_EXISTING_BUTTON
      // 
      this.ALL_EXISTING_BUTTON.Location = new System.Drawing.Point(505, 538);
      this.ALL_EXISTING_BUTTON.Name = "ALL_EXISTING_BUTTON";
      this.ALL_EXISTING_BUTTON.Size = new System.Drawing.Size(103, 23);
      this.ALL_EXISTING_BUTTON.TabIndex = 193;
      this.ALL_EXISTING_BUTTON.Text = "ALL EXISTING";
      this.ALL_EXISTING_BUTTON.UseVisualStyleBackColor = true;
      this.ALL_EXISTING_BUTTON.Click += new System.EventHandler(this.ALL_EXISTING_BUTTON_Click);
      // 
      // REMOVE_EXISTING_CHECKBOX
      // 
      this.REMOVE_EXISTING_CHECKBOX.AutoSize = true;
      this.REMOVE_EXISTING_CHECKBOX.Location = new System.Drawing.Point(616, 541);
      this.REMOVE_EXISTING_CHECKBOX.Name = "REMOVE_EXISTING_CHECKBOX";
      this.REMOVE_EXISTING_CHECKBOX.Size = new System.Drawing.Size(192, 17);
      this.REMOVE_EXISTING_CHECKBOX.TabIndex = 194;
      this.REMOVE_EXISTING_CHECKBOX.Text = "REMOVE EXISTING ON CHANGE";
      this.REMOVE_EXISTING_CHECKBOX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.REMOVE_EXISTING_CHECKBOX.UseVisualStyleBackColor = true;
      // 
      // UserInterface
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.WhiteSmoke;
      this.Controls.Add(this.REMOVE_EXISTING_CHECKBOX);
      this.Controls.Add(this.ALL_EXISTING_BUTTON);
      this.Controls.Add(this.LARGEST_LCL_INPUT);
      this.Controls.Add(this.AUTO_CHECKBOX);
      this.Controls.Add(this.MAX_DESCRIPTION_CELL_CHAR_TEXTBOX);
      this.Controls.Add(this.MAX_DESCRIPTION_CELL_CHAR_LABEL);
      this.Controls.Add(this.REMOVE_NOTE_BUTTON);
      this.Controls.Add(this.CUSTOM_TITLE_LABEL);
      this.Controls.Add(this.CUSTOM_TITLE_TEXT);
      this.Controls.Add(this.MODIFY_NOTES_BUTTON);
      this.Controls.Add(this.APPLY_COMBOBOX);
      this.Controls.Add(this.APPLY_BUTTON);
      this.Controls.Add(this.INFO_LABEL);
      this.Controls.Add(this.DELETE_PANEL_BUTTON);
      this.Controls.Add(this.PANEL_GRID);
      this.Controls.Add(this.label18);
      this.Controls.Add(this.LARGEST_LCL_CHECKBOX);
      this.Controls.Add(this.label17);
      this.Controls.Add(this.LARGEST_LCL_LABEL);
      this.Controls.Add(this.label16);
      this.Controls.Add(this.label15);
      this.Controls.Add(this.CREATE_PANEL_BUTTON);
      this.Controls.Add(this.label14);
      this.Controls.Add(this.FEEDER_AMP_GRID);
      this.Controls.Add(this.label13);
      this.Controls.Add(this.PANEL_LOAD_GRID);
      this.Controls.Add(this.label12);
      this.Controls.Add(this.TOTAL_OTHER_LOAD_GRID);
      this.Controls.Add(this.label11);
      this.Controls.Add(this.LCL_GRID);
      this.Controls.Add(this.label10);
      this.Controls.Add(this.TOTAL_VA_GRID);
      this.Controls.Add(this.PANEL_NAME_INPUT);
      this.Controls.Add(this.PHASE_SUM_GRID);
      this.Controls.Add(this.PANEL_LOCATION_INPUT);
      this.Controls.Add(this.DELETE_ROW_BUTTON);
      this.Controls.Add(this.MAIN_INPUT);
      this.Controls.Add(this.ADD_ROW_BUTTON);
      this.Controls.Add(this.BUS_RATING_INPUT);
      this.Controls.Add(this.LINE_VOLTAGE_COMBOBOX);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.PHASE_VOLTAGE_COMBOBOX);
      this.Controls.Add(this.STATUS_COMBOBOX);
      this.Controls.Add(this.PHASE_COMBOBOX);
      this.Controls.Add(this.MOUNTING_COMBOBOX);
      this.Controls.Add(this.WIRE_COMBOBOX);
      this.Name = "UserInterface";
      this.Size = new System.Drawing.Size(1382, 663);
      ((System.ComponentModel.ISupportInitialize)(this.PANEL_GRID)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.FEEDER_AMP_GRID)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.PANEL_LOAD_GRID)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.TOTAL_OTHER_LOAD_GRID)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.LCL_GRID)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.TOTAL_VA_GRID)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.PHASE_SUM_GRID)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.DataGridView PANEL_GRID;
    private System.Windows.Forms.DataGridViewTextBoxColumn description_left;
    private System.Windows.Forms.DataGridViewTextBoxColumn phase_a_left;
    private System.Windows.Forms.DataGridViewTextBoxColumn phase_b_left;
    private System.Windows.Forms.DataGridViewTextBoxColumn breaker_left;
    private System.Windows.Forms.DataGridViewTextBoxColumn circuit_left;
    private System.Windows.Forms.DataGridViewTextBoxColumn circuit_right;
    private System.Windows.Forms.DataGridViewTextBoxColumn breaker_right;
    private System.Windows.Forms.DataGridViewTextBoxColumn phase_a_right;
    private System.Windows.Forms.DataGridViewTextBoxColumn phase_b_right;
    private System.Windows.Forms.DataGridViewTextBoxColumn description_right;
    private System.Windows.Forms.Label label18;
    private System.Windows.Forms.CheckBox LARGEST_LCL_CHECKBOX;
    private System.Windows.Forms.Label label17;
    private System.Windows.Forms.Label LARGEST_LCL_LABEL;
    private System.Windows.Forms.Label label16;
    private System.Windows.Forms.Label label15;
    private System.Windows.Forms.Button CREATE_PANEL_BUTTON;
    private System.Windows.Forms.Label label14;
    private System.Windows.Forms.DataGridView FEEDER_AMP_GRID;
    private System.Windows.Forms.DataGridViewTextBoxColumn FEEDER_AMPS;
    private System.Windows.Forms.Label label13;
    private System.Windows.Forms.DataGridView PANEL_LOAD_GRID;
    private System.Windows.Forms.DataGridViewTextBoxColumn PANEL_LOAD;
    private System.Windows.Forms.Label label12;
    private System.Windows.Forms.DataGridView TOTAL_OTHER_LOAD_GRID;
    private System.Windows.Forms.DataGridViewTextBoxColumn TOTAL_OTHER_LOAD;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.DataGridView LCL_GRID;
    private System.Windows.Forms.DataGridViewTextBoxColumn LCL_AT_100PC;
    private System.Windows.Forms.DataGridViewTextBoxColumn LCL_AT_125PC;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.DataGridView TOTAL_VA_GRID;
    private System.Windows.Forms.DataGridViewTextBoxColumn TOTAL_VA;
    private System.Windows.Forms.TextBox PANEL_NAME_INPUT;
    private System.Windows.Forms.DataGridView PHASE_SUM_GRID;
    private System.Windows.Forms.DataGridViewTextBoxColumn TOTAL_PH_A;
    private System.Windows.Forms.DataGridViewTextBoxColumn TOTAL_PH_B;
    private System.Windows.Forms.TextBox PANEL_LOCATION_INPUT;
    private System.Windows.Forms.Button DELETE_ROW_BUTTON;
    private System.Windows.Forms.TextBox MAIN_INPUT;
    private System.Windows.Forms.Button ADD_ROW_BUTTON;
    private System.Windows.Forms.TextBox BUS_RATING_INPUT;
    private System.Windows.Forms.ComboBox LINE_VOLTAGE_COMBOBOX;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox PHASE_VOLTAGE_COMBOBOX;
    private System.Windows.Forms.ComboBox STATUS_COMBOBOX;
    private System.Windows.Forms.ComboBox PHASE_COMBOBOX;
    private System.Windows.Forms.ComboBox MOUNTING_COMBOBOX;
    private System.Windows.Forms.ComboBox WIRE_COMBOBOX;
    private System.Windows.Forms.Button DELETE_PANEL_BUTTON;
    private System.Windows.Forms.Label INFO_LABEL;
    private System.Windows.Forms.Button APPLY_BUTTON;
    private System.Windows.Forms.ComboBox APPLY_COMBOBOX;
    private System.Windows.Forms.Button MODIFY_NOTES_BUTTON;
    private System.Windows.Forms.TextBox CUSTOM_TITLE_TEXT;
    private System.Windows.Forms.Label CUSTOM_TITLE_LABEL;
    private System.Windows.Forms.Button REMOVE_NOTE_BUTTON;
    private System.Windows.Forms.Label MAX_DESCRIPTION_CELL_CHAR_LABEL;
    private System.Windows.Forms.TextBox MAX_DESCRIPTION_CELL_CHAR_TEXTBOX;
    private System.Windows.Forms.CheckBox AUTO_CHECKBOX;
    private System.Windows.Forms.TextBox LARGEST_LCL_INPUT;
    private System.Windows.Forms.Button ALL_EXISTING_BUTTON;
    private System.Windows.Forms.CheckBox REMOVE_EXISTING_CHECKBOX;
  }
}
