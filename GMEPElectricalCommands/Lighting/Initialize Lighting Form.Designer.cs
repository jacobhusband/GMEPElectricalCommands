namespace CADTestingGround
{
  partial class INITIALIZE_LIGHTING_FORM
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
      this.SET_WINDOW_BUTTON = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.SET_PANEL_LOCATION_BUTTON = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.SELECT_POLYLINES_BUTTON = new System.Windows.Forms.Button();
      this.label4 = new System.Windows.Forms.Label();
      this.SCALE_COMBOBOX = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // SET_WINDOW_BUTTON
      // 
      this.SET_WINDOW_BUTTON.Location = new System.Drawing.Point(34, 9);
      this.SET_WINDOW_BUTTON.Name = "SET_WINDOW_BUTTON";
      this.SET_WINDOW_BUTTON.Size = new System.Drawing.Size(75, 23);
      this.SET_WINDOW_BUTTON.TabIndex = 0;
      this.SET_WINDOW_BUTTON.Text = "Set Window";
      this.SET_WINDOW_BUTTON.UseVisualStyleBackColor = true;
      this.SET_WINDOW_BUTTON.Click += new System.EventHandler(this.SET_WINDOW_BUTTON_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 14);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(16, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "1.";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(121, 14);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(16, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "2.";
      // 
      // SET_PANEL_LOCATION_BUTTON
      // 
      this.SET_PANEL_LOCATION_BUTTON.Location = new System.Drawing.Point(143, 9);
      this.SET_PANEL_LOCATION_BUTTON.Name = "SET_PANEL_LOCATION_BUTTON";
      this.SET_PANEL_LOCATION_BUTTON.Size = new System.Drawing.Size(116, 23);
      this.SET_PANEL_LOCATION_BUTTON.TabIndex = 1;
      this.SET_PANEL_LOCATION_BUTTON.Text = "Set Panel Location";
      this.SET_PANEL_LOCATION_BUTTON.UseVisualStyleBackColor = true;
      this.SET_PANEL_LOCATION_BUTTON.Click += new System.EventHandler(this.SET_PANEL_LOCATION_BUTTON_Click);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(274, 14);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(46, 13);
      this.label3.TabIndex = 5;
      this.label3.Text = "3. Scale";
      // 
      // SELECT_POLYLINES_BUTTON
      // 
      this.SELECT_POLYLINES_BUTTON.Location = new System.Drawing.Point(450, 8);
      this.SELECT_POLYLINES_BUTTON.Name = "SELECT_POLYLINES_BUTTON";
      this.SELECT_POLYLINES_BUTTON.Size = new System.Drawing.Size(99, 23);
      this.SELECT_POLYLINES_BUTTON.TabIndex = 3;
      this.SELECT_POLYLINES_BUTTON.Text = "Select Polylines";
      this.SELECT_POLYLINES_BUTTON.UseVisualStyleBackColor = true;
      this.SELECT_POLYLINES_BUTTON.Click += new System.EventHandler(this.SELECT_POLYLINES_BUTTON_Click);
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(428, 13);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(16, 13);
      this.label4.TabIndex = 6;
      this.label4.Text = "4.";
      // 
      // SCALE_COMBOBOX
      // 
      this.SCALE_COMBOBOX.FormattingEnabled = true;
      this.SCALE_COMBOBOX.Items.AddRange(new object[] {
            "1/4",
            "3/16",
            "1/8",
            "3/32",
            "1/16",
            "1/32"});
      this.SCALE_COMBOBOX.Location = new System.Drawing.Point(326, 10);
      this.SCALE_COMBOBOX.Name = "SCALE_COMBOBOX";
      this.SCALE_COMBOBOX.Size = new System.Drawing.Size(87, 21);
      this.SCALE_COMBOBOX.TabIndex = 2;
      this.SCALE_COMBOBOX.SelectedIndexChanged += new System.EventHandler(this.SCALE_COMBOBOX_SelectedIndexChanged);
      // 
      // INITIALIZE_LIGHTING_FORM
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(568, 41);
      this.Controls.Add(this.SCALE_COMBOBOX);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.SELECT_POLYLINES_BUTTON);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.SET_PANEL_LOCATION_BUTTON);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.SET_WINDOW_BUTTON);
      this.Name = "INITIALIZE_LIGHTING_FORM";
      this.Text = "Initialize Lighting";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button SET_WINDOW_BUTTON;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Button SET_PANEL_LOCATION_BUTTON;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button SELECT_POLYLINES_BUTTON;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.ComboBox SCALE_COMBOBOX;
  }
}