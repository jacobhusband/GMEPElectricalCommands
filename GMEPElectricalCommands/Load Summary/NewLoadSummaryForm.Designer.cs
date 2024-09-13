namespace ElectricalCommands.Load_Summary {
  partial class NewLoadSummaryForm {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.NEW_LOAD_SUMMARY_TEXTBOX = new System.Windows.Forms.TextBox();
      this.NEW_LOAD_SUMMARY_LABEL = new System.Windows.Forms.Label();
      this.NEW_LOAD_SUMMARY_ADD_BUTTON = new System.Windows.Forms.Button();
      this.NEW_LOAD_SUMMARY_LABEL.Click += new System.EventHandler(this.NEW_LOAD_SUMMARY_LABEL_Click);
      this.SuspendLayout();
      // 
      // NEW_LOAD_SUMMARY_TEXTBOX
      // 
      this.NEW_LOAD_SUMMARY_TEXTBOX.Location = new System.Drawing.Point(119, 6);
      this.NEW_LOAD_SUMMARY_TEXTBOX.Name = "NEW_LOAD_SUMMARY_TEXTBOX";
      this.NEW_LOAD_SUMMARY_TEXTBOX.Size = new System.Drawing.Size(254, 20);
      this.NEW_LOAD_SUMMARY_TEXTBOX.TabIndex = 0;
      this.NEW_LOAD_SUMMARY_TEXTBOX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // NEW_LOAD_SUMMARY_LABEL
      // 
      this.NEW_LOAD_SUMMARY_LABEL.AutoSize = true;
      this.NEW_LOAD_SUMMARY_LABEL.Location = new System.Drawing.Point(12, 9);
      this.NEW_LOAD_SUMMARY_LABEL.Name = "NEW_LOAD_SUMMARY_LABEL";
      this.NEW_LOAD_SUMMARY_LABEL.Size = new System.Drawing.Size(101, 13);
      this.NEW_LOAD_SUMMARY_LABEL.TabIndex = 1;
      this.NEW_LOAD_SUMMARY_LABEL.Text = "Load Section Name";
      // 
      // NEW_LOAD_SUMMARY_ADD_BUTTON
      // 
      this.NEW_LOAD_SUMMARY_ADD_BUTTON.Location = new System.Drawing.Point(298, 32);
      this.NEW_LOAD_SUMMARY_ADD_BUTTON.Name = "NEW_LOAD_SUMMARY_ADD_BUTTON";
      this.NEW_LOAD_SUMMARY_ADD_BUTTON.Size = new System.Drawing.Size(75, 23);
      this.NEW_LOAD_SUMMARY_ADD_BUTTON.TabIndex = 2;
      this.NEW_LOAD_SUMMARY_ADD_BUTTON.Text = "Add";
      this.NEW_LOAD_SUMMARY_ADD_BUTTON.UseVisualStyleBackColor = true;
      // 
      // NewLoadSummaryForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(394, 66);
      this.Controls.Add(this.NEW_LOAD_SUMMARY_ADD_BUTTON);
      this.Controls.Add(this.NEW_LOAD_SUMMARY_LABEL);
      this.Controls.Add(this.NEW_LOAD_SUMMARY_TEXTBOX);
      this.Name = "NewLoadSummaryForm";
      this.Text = "Add New Load Summary";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox NEW_LOAD_SUMMARY_TEXTBOX;
    private System.Windows.Forms.Label NEW_LOAD_SUMMARY_LABEL;
    private System.Windows.Forms.Button NEW_LOAD_SUMMARY_ADD_BUTTON;
  }
}