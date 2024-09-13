namespace ElectricalCommands.Load_Summary {
  partial class LoadSummaryListElement {
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.LOAD_CHECK_BOX = new System.Windows.Forms.CheckBox();
      this.LOAD_VALUE_TEXTBOX = new System.Windows.Forms.TextBox();
      this.LOAD_NAME_CHECKBOX = new System.Windows.Forms.TextBox();
      this.LOAD_DELETE_BUTTON = new System.Windows.Forms.Button();
      this.LOAD_A_KVA_COMBOBOX = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // LOAD_CHECK_BOX
      // 
      this.LOAD_CHECK_BOX.AutoSize = true;
      this.LOAD_CHECK_BOX.Location = new System.Drawing.Point(3, 5);
      this.LOAD_CHECK_BOX.Name = "LOAD_CHECK_BOX";
      this.LOAD_CHECK_BOX.Size = new System.Drawing.Size(15, 14);
      this.LOAD_CHECK_BOX.TabIndex = 0;
      this.LOAD_CHECK_BOX.UseVisualStyleBackColor = true;
      // 
      // LOAD_VALUE_TEXTBOX
      // 
      this.LOAD_VALUE_TEXTBOX.BackColor = System.Drawing.SystemColors.Window;
      this.LOAD_VALUE_TEXTBOX.Location = new System.Drawing.Point(263, 2);
      this.LOAD_VALUE_TEXTBOX.Name = "LOAD_VALUE_TEXTBOX";
      this.LOAD_VALUE_TEXTBOX.ReadOnly = true;
      this.LOAD_VALUE_TEXTBOX.Size = new System.Drawing.Size(100, 20);
      this.LOAD_VALUE_TEXTBOX.TabIndex = 1;
      // 
      // LOAD_NAME_CHECKBOX
      // 
      this.LOAD_NAME_CHECKBOX.Location = new System.Drawing.Point(24, 2);
      this.LOAD_NAME_CHECKBOX.Name = "LOAD_NAME_CHECKBOX";
      this.LOAD_NAME_CHECKBOX.Size = new System.Drawing.Size(233, 20);
      this.LOAD_NAME_CHECKBOX.TabIndex = 2;
      this.LOAD_NAME_CHECKBOX.Text = "New Load";
      this.LOAD_NAME_CHECKBOX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.LOAD_CHECK_BOX_KeyPress);
      // 
      // LOAD_DELETE_BUTTON
      // 
      this.LOAD_DELETE_BUTTON.Location = new System.Drawing.Point(479, 1);
      this.LOAD_DELETE_BUTTON.Name = "LOAD_DELETE_BUTTON";
      this.LOAD_DELETE_BUTTON.Size = new System.Drawing.Size(48, 23);
      this.LOAD_DELETE_BUTTON.TabIndex = 3;
      this.LOAD_DELETE_BUTTON.Text = "Delete";
      this.LOAD_DELETE_BUTTON.UseVisualStyleBackColor = true;
      // 
      // LOAD_A_KVA_COMBOBOX
      // 
      this.LOAD_A_KVA_COMBOBOX.FormattingEnabled = true;
      this.LOAD_A_KVA_COMBOBOX.ItemHeight = 13;
      this.LOAD_A_KVA_COMBOBOX.Items.AddRange(new object[] {
            "KVA",
            "A"});
      this.LOAD_A_KVA_COMBOBOX.Location = new System.Drawing.Point(369, 1);
      this.LOAD_A_KVA_COMBOBOX.Name = "LOAD_A_KVA_COMBOBOX";
      this.LOAD_A_KVA_COMBOBOX.Size = new System.Drawing.Size(53, 21);
      this.LOAD_A_KVA_COMBOBOX.TabIndex = 4;
      // 
      // LoadSummaryListElement
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.LOAD_A_KVA_COMBOBOX);
      this.Controls.Add(this.LOAD_DELETE_BUTTON);
      this.Controls.Add(this.LOAD_NAME_CHECKBOX);
      this.Controls.Add(this.LOAD_VALUE_TEXTBOX);
      this.Controls.Add(this.LOAD_CHECK_BOX);
      this.Name = "LoadSummaryListElement";
      this.Size = new System.Drawing.Size(530, 26);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.CheckBox LOAD_CHECK_BOX;
    private System.Windows.Forms.TextBox LOAD_VALUE_TEXTBOX;
    private System.Windows.Forms.TextBox LOAD_NAME_CHECKBOX;
    private System.Windows.Forms.Button LOAD_DELETE_BUTTON;
    private System.Windows.Forms.ComboBox LOAD_A_KVA_COMBOBOX;
  }
}
