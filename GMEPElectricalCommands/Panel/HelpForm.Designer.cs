namespace ElectricalCommands
{
  partial class HelpForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HelpForm));
      this.HELP_TEXTBOX = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // HELP_TEXTBOX
      // 
      this.HELP_TEXTBOX.Location = new System.Drawing.Point(12, 12);
      this.HELP_TEXTBOX.Multiline = true;
      this.HELP_TEXTBOX.Name = "HELP_TEXTBOX";
      this.HELP_TEXTBOX.ReadOnly = true;
      this.HELP_TEXTBOX.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.HELP_TEXTBOX.Size = new System.Drawing.Size(651, 656);
      this.HELP_TEXTBOX.TabIndex = 0;
      this.HELP_TEXTBOX.Text = resources.GetString("HELP_TEXTBOX.Text");
      // 
      // HelpForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(675, 680);
      this.Controls.Add(this.HELP_TEXTBOX);
      this.Name = "HelpForm";
      this.Text = "Help";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox HELP_TEXTBOX;
  }
}