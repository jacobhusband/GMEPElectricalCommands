namespace ElectricalCommands.Load_Summary {
  partial class DialogWindow {
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
      this.LOAD_SUMMARY_TABS = new System.Windows.Forms.TabControl();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.ADD_NEW_LOAD_SUMMARY_BUTTON = new System.Windows.Forms.Button();
      this.UPDATE_LOAD_SUMMARIES_BUTTON = new System.Windows.Forms.Button();
      this.LOAD_SUMMARY_TABS.SuspendLayout();
      this.SuspendLayout();
      // 
      // LOAD_SUMMARY_TABS
      // 
      this.LOAD_SUMMARY_TABS.Controls.Add(this.tabPage1);
      this.LOAD_SUMMARY_TABS.Controls.Add(this.tabPage2);
      this.LOAD_SUMMARY_TABS.Location = new System.Drawing.Point(160, 13);
      this.LOAD_SUMMARY_TABS.Name = "LOAD_SUMMARY_TABS";
      this.LOAD_SUMMARY_TABS.SelectedIndex = 0;
      this.LOAD_SUMMARY_TABS.Size = new System.Drawing.Size(619, 680);
      this.LOAD_SUMMARY_TABS.TabIndex = 0;
      // 
      // tabPage1
      // 
      this.tabPage1.Location = new System.Drawing.Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(611, 654);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "tabPage1";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // tabPage2
      // 
      this.tabPage2.Location = new System.Drawing.Point(4, 22);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage2.Size = new System.Drawing.Size(192, 74);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "tabPage2";
      this.tabPage2.UseVisualStyleBackColor = true;
      // 
      // ADD_NEW_LOAD_SUMMARY_BUTTON
      // 
      this.ADD_NEW_LOAD_SUMMARY_BUTTON.Location = new System.Drawing.Point(12, 35);
      this.ADD_NEW_LOAD_SUMMARY_BUTTON.Name = "ADD_NEW_LOAD_SUMMARY_BUTTON";
      this.ADD_NEW_LOAD_SUMMARY_BUTTON.Size = new System.Drawing.Size(142, 23);
      this.ADD_NEW_LOAD_SUMMARY_BUTTON.TabIndex = 1;
      this.ADD_NEW_LOAD_SUMMARY_BUTTON.Text = "New Load Summary";
      this.ADD_NEW_LOAD_SUMMARY_BUTTON.UseVisualStyleBackColor = true;
      this.ADD_NEW_LOAD_SUMMARY_BUTTON.Click += new System.EventHandler(this.button1_Click);
      // 
      // UPDATE_LOAD_SUMMARIES_BUTTON
      // 
      this.UPDATE_LOAD_SUMMARIES_BUTTON.Location = new System.Drawing.Point(12, 64);
      this.UPDATE_LOAD_SUMMARIES_BUTTON.Name = "UPDATE_LOAD_SUMMARIES_BUTTON";
      this.UPDATE_LOAD_SUMMARIES_BUTTON.Size = new System.Drawing.Size(141, 23);
      this.UPDATE_LOAD_SUMMARIES_BUTTON.TabIndex = 2;
      this.UPDATE_LOAD_SUMMARIES_BUTTON.Text = "Update Load Summaries";
      this.UPDATE_LOAD_SUMMARIES_BUTTON.UseVisualStyleBackColor = true;
      // 
      // DialogWindow
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(791, 711);
      this.Controls.Add(this.UPDATE_LOAD_SUMMARIES_BUTTON);
      this.Controls.Add(this.ADD_NEW_LOAD_SUMMARY_BUTTON);
      this.Controls.Add(this.LOAD_SUMMARY_TABS);
      this.Name = "DialogWindow";
      this.Text = "DialogWindow";
      this.Load += new System.EventHandler(this.DialogWindow_Load);
      this.LOAD_SUMMARY_TABS.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl LOAD_SUMMARY_TABS;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.TabPage tabPage2;
    private System.Windows.Forms.Button ADD_NEW_LOAD_SUMMARY_BUTTON;
    private System.Windows.Forms.Button UPDATE_LOAD_SUMMARIES_BUTTON;
  }
}