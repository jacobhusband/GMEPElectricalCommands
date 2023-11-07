namespace AutoCADCommands
{
  partial class NEWPANELFORM
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
      this.CHECKBOX3PH = new System.Windows.Forms.CheckBox();
      this.CREATEPANEL = new System.Windows.Forms.Button();
      this.CREATEPANELNAME = new System.Windows.Forms.TextBox();
      this.NEWPANELNAME = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // CHECKBOX3PH
      // 
      this.CHECKBOX3PH.AutoSize = true;
      this.CHECKBOX3PH.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.CHECKBOX3PH.Location = new System.Drawing.Point(12, 30);
      this.CHECKBOX3PH.Name = "CHECKBOX3PH";
      this.CHECKBOX3PH.Size = new System.Drawing.Size(117, 17);
      this.CHECKBOX3PH.TabIndex = 0;
      this.CHECKBOX3PH.Text = "Three Phase Panel";
      this.CHECKBOX3PH.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.CHECKBOX3PH.UseVisualStyleBackColor = true;
      // 
      // CREATEPANEL
      // 
      this.CREATEPANEL.Location = new System.Drawing.Point(241, 26);
      this.CREATEPANEL.Name = "CREATEPANEL";
      this.CREATEPANEL.Size = new System.Drawing.Size(75, 23);
      this.CREATEPANEL.TabIndex = 1;
      this.CREATEPANEL.Text = "Create";
      this.CREATEPANEL.UseVisualStyleBackColor = true;
      this.CREATEPANEL.Click += new System.EventHandler(this.CREATEPANEL_Click);
      // 
      // CREATEPANELNAME
      // 
      this.CREATEPANELNAME.Location = new System.Drawing.Point(135, 28);
      this.CREATEPANELNAME.Name = "CREATEPANELNAME";
      this.CREATEPANELNAME.Size = new System.Drawing.Size(100, 20);
      this.CREATEPANELNAME.TabIndex = 2;
      // 
      // NEWPANELNAME
      // 
      this.NEWPANELNAME.AutoSize = true;
      this.NEWPANELNAME.Location = new System.Drawing.Point(139, 12);
      this.NEWPANELNAME.Name = "NEWPANELNAME";
      this.NEWPANELNAME.Size = new System.Drawing.Size(65, 13);
      this.NEWPANELNAME.TabIndex = 3;
      this.NEWPANELNAME.Text = "Panel Name";
      // 
      // NEWPANELFORM
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(338, 64);
      this.Controls.Add(this.NEWPANELNAME);
      this.Controls.Add(this.CREATEPANELNAME);
      this.Controls.Add(this.CREATEPANEL);
      this.Controls.Add(this.CHECKBOX3PH);
      this.Name = "NEWPANELFORM";
      this.Text = "Create New Panel";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.CheckBox CHECKBOX3PH;
    private System.Windows.Forms.Button CREATEPANEL;
    private System.Windows.Forms.TextBox CREATEPANELNAME;
    private System.Windows.Forms.Label NEWPANELNAME;
  }
}

