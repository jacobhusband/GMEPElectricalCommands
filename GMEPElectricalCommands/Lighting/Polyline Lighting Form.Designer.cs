namespace ElectricalCommands.Lighting {
  partial class POLYLINE_LIGHTING_FORM
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
      this.label1 = new System.Windows.Forms.Label();
      this.NUMBER_OF_ROOMS = new System.Windows.Forms.TextBox();
      this.INITIAL_DIMMER_LETTER = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.GENERATE_BUTTON = new System.Windows.Forms.Button();
      this.CIRCUIT_NUMBER = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.FIX_TEXT_BUTTON = new System.Windows.Forms.Button();
      this.PROCEED_BUTTON = new System.Windows.Forms.Button();
      this.CLUSTERING_METHODS = new System.Windows.Forms.GroupBox();
      this.KMEANS = new System.Windows.Forms.RadioButton();
      this.OBJECT_AREA = new System.Windows.Forms.RadioButton();
      this.CLUSTERING_METHODS.SuspendLayout();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 15);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(92, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Number of Rooms";
      // 
      // NUMBER_OF_ROOMS
      // 
      this.NUMBER_OF_ROOMS.Location = new System.Drawing.Point(117, 12);
      this.NUMBER_OF_ROOMS.Name = "NUMBER_OF_ROOMS";
      this.NUMBER_OF_ROOMS.Size = new System.Drawing.Size(100, 20);
      this.NUMBER_OF_ROOMS.TabIndex = 0;
      // 
      // INITIAL_DIMMER_LETTER
      // 
      this.INITIAL_DIMMER_LETTER.Location = new System.Drawing.Point(117, 38);
      this.INITIAL_DIMMER_LETTER.Name = "INITIAL_DIMMER_LETTER";
      this.INITIAL_DIMMER_LETTER.Size = new System.Drawing.Size(100, 20);
      this.INITIAL_DIMMER_LETTER.TabIndex = 1;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 41);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(99, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Initial Dimmer Letter";
      // 
      // GENERATE_BUTTON
      // 
      this.GENERATE_BUTTON.Location = new System.Drawing.Point(15, 146);
      this.GENERATE_BUTTON.Name = "GENERATE_BUTTON";
      this.GENERATE_BUTTON.Size = new System.Drawing.Size(205, 23);
      this.GENERATE_BUTTON.TabIndex = 3;
      this.GENERATE_BUTTON.Text = "Generate";
      this.GENERATE_BUTTON.UseVisualStyleBackColor = true;
      this.GENERATE_BUTTON.Click += new System.EventHandler(this.GENERATE_BUTTON_Click);
      // 
      // CIRCUIT_NUMBER
      // 
      this.CIRCUIT_NUMBER.Location = new System.Drawing.Point(117, 64);
      this.CIRCUIT_NUMBER.Name = "CIRCUIT_NUMBER";
      this.CIRCUIT_NUMBER.Size = new System.Drawing.Size(100, 20);
      this.CIRCUIT_NUMBER.TabIndex = 2;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(12, 67);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(76, 13);
      this.label3.TabIndex = 5;
      this.label3.Text = "Circuit Number";
      // 
      // FIX_TEXT_BUTTON
      // 
      this.FIX_TEXT_BUTTON.Location = new System.Drawing.Point(15, 175);
      this.FIX_TEXT_BUTTON.Name = "FIX_TEXT_BUTTON";
      this.FIX_TEXT_BUTTON.Size = new System.Drawing.Size(205, 23);
      this.FIX_TEXT_BUTTON.TabIndex = 4;
      this.FIX_TEXT_BUTTON.Text = "Fix Text";
      this.FIX_TEXT_BUTTON.UseVisualStyleBackColor = true;
      this.FIX_TEXT_BUTTON.Click += new System.EventHandler(this.FIX_TEXT_BUTTON_Click);
      // 
      // PROCEED_BUTTON
      // 
      this.PROCEED_BUTTON.Location = new System.Drawing.Point(15, 204);
      this.PROCEED_BUTTON.Name = "PROCEED_BUTTON";
      this.PROCEED_BUTTON.Size = new System.Drawing.Size(205, 23);
      this.PROCEED_BUTTON.TabIndex = 6;
      this.PROCEED_BUTTON.Text = "Proceed";
      this.PROCEED_BUTTON.UseVisualStyleBackColor = true;
      this.PROCEED_BUTTON.Click += new System.EventHandler(this.PROCEED_BUTTON_Click);
      // 
      // CLUSTERING_METHODS
      // 
      this.CLUSTERING_METHODS.Controls.Add(this.OBJECT_AREA);
      this.CLUSTERING_METHODS.Controls.Add(this.KMEANS);
      this.CLUSTERING_METHODS.Location = new System.Drawing.Point(15, 90);
      this.CLUSTERING_METHODS.Name = "CLUSTERING_METHODS";
      this.CLUSTERING_METHODS.Size = new System.Drawing.Size(202, 50);
      this.CLUSTERING_METHODS.TabIndex = 7;
      this.CLUSTERING_METHODS.TabStop = false;
      this.CLUSTERING_METHODS.Text = "Clustering Methodology";
      // 
      // KMEANS
      // 
      this.KMEANS.AutoSize = true;
      this.KMEANS.Checked = true;
      this.KMEANS.Location = new System.Drawing.Point(11, 19);
      this.KMEANS.Name = "KMEANS";
      this.KMEANS.Size = new System.Drawing.Size(63, 17);
      this.KMEANS.TabIndex = 0;
      this.KMEANS.TabStop = true;
      this.KMEANS.Text = "Kmeans";
      this.KMEANS.UseVisualStyleBackColor = true;
      // 
      // OBJECT_AREA
      // 
      this.OBJECT_AREA.AutoSize = true;
      this.OBJECT_AREA.Location = new System.Drawing.Point(80, 19);
      this.OBJECT_AREA.Name = "OBJECT_AREA";
      this.OBJECT_AREA.Size = new System.Drawing.Size(81, 17);
      this.OBJECT_AREA.TabIndex = 1;
      this.OBJECT_AREA.Text = "Object Area";
      this.OBJECT_AREA.UseVisualStyleBackColor = true;
      // 
      // POLYLINE_LIGHTING_FORM
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(233, 240);
      this.Controls.Add(this.CLUSTERING_METHODS);
      this.Controls.Add(this.PROCEED_BUTTON);
      this.Controls.Add(this.FIX_TEXT_BUTTON);
      this.Controls.Add(this.CIRCUIT_NUMBER);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.GENERATE_BUTTON);
      this.Controls.Add(this.INITIAL_DIMMER_LETTER);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.NUMBER_OF_ROOMS);
      this.Controls.Add(this.label1);
      this.Name = "POLYLINE_LIGHTING_FORM";
      this.Text = "Polyline Lighting Form";
      this.CLUSTERING_METHODS.ResumeLayout(false);
      this.CLUSTERING_METHODS.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox NUMBER_OF_ROOMS;
    private System.Windows.Forms.TextBox INITIAL_DIMMER_LETTER;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Button GENERATE_BUTTON;
    private System.Windows.Forms.TextBox CIRCUIT_NUMBER;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button FIX_TEXT_BUTTON;
    private System.Windows.Forms.Button PROCEED_BUTTON;
    private System.Windows.Forms.GroupBox CLUSTERING_METHODS;
    private System.Windows.Forms.RadioButton OBJECT_AREA;
    private System.Windows.Forms.RadioButton KMEANS;
  }
}