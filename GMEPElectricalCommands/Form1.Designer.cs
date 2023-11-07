using System.Drawing;

namespace AutoCADCommands
{
  partial class MainForm
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
      this.PANEL_TABS = new System.Windows.Forms.TabControl();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.PANEL_TABS.SuspendLayout();
      this.SuspendLayout();
      // 
      // PANEL_TABS
      // 
      this.PANEL_TABS.Controls.Add(this.tabPage1);
      this.PANEL_TABS.Location = new System.Drawing.Point(1, 0);
      this.PANEL_TABS.Name = "PANEL_TABS";
      this.PANEL_TABS.SelectedIndex = 0;
      this.PANEL_TABS.Size = new System.Drawing.Size(1409, 691);
      this.PANEL_TABS.TabIndex = 76;
      // 
      // tabPage1
      // 
      this.tabPage1.Location = new System.Drawing.Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(1401, 665);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "tabPage1";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1408, 691);
      this.Controls.Add(this.PANEL_TABS);
      this.Name = "MainForm";
      this.Text = "Panel Schedule";
      this.PANEL_TABS.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.TabControl PANEL_TABS;
    private System.Windows.Forms.TabPage tabPage1;
  }
}

