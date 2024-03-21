﻿using System.Drawing;

namespace ElectricalCommands
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
      this.CREATE_ALL_PANELS_BUTTON = new System.Windows.Forms.Button();
      this.NEW_PANEL_BUTTON = new System.Windows.Forms.Button();
      this.HELP_BUTTON = new System.Windows.Forms.Button();
      this.SAVE_BUTTON = new System.Windows.Forms.Button();
      this.LOAD_BUTTON = new System.Windows.Forms.Button();
      this.DUPLICATE_PANEL_BUTTON = new System.Windows.Forms.Button();
      this.PANEL_TABS.SuspendLayout();
      this.SuspendLayout();
      // 
      // PANEL_TABS
      // 
      this.PANEL_TABS.Controls.Add(this.tabPage1);
      this.PANEL_TABS.Location = new System.Drawing.Point(176, 4);
      this.PANEL_TABS.Name = "PANEL_TABS";
      this.PANEL_TABS.SelectedIndex = 0;
      this.PANEL_TABS.Size = new System.Drawing.Size(1409, 691);
      this.PANEL_TABS.TabIndex = 76;
      // 
      // tabPage1
      // 
      this.tabPage1.BackColor = System.Drawing.Color.WhiteSmoke;
      this.tabPage1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.tabPage1.Location = new System.Drawing.Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(1401, 665);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "tabPage1";
      // 
      // CREATE_ALL_PANELS_BUTTON
      // 
      this.CREATE_ALL_PANELS_BUTTON.Location = new System.Drawing.Point(13, 55);
      this.CREATE_ALL_PANELS_BUTTON.Name = "CREATE_ALL_PANELS_BUTTON";
      this.CREATE_ALL_PANELS_BUTTON.Size = new System.Drawing.Size(148, 23);
      this.CREATE_ALL_PANELS_BUTTON.TabIndex = 179;
      this.CREATE_ALL_PANELS_BUTTON.Text = "CREATE ALL PANELS";
      this.CREATE_ALL_PANELS_BUTTON.UseVisualStyleBackColor = true;
      this.CREATE_ALL_PANELS_BUTTON.Click += new System.EventHandler(this.CREATE_ALL_PANELS_BUTTON_Click);
      // 
      // NEW_PANEL_BUTTON
      // 
      this.NEW_PANEL_BUTTON.Location = new System.Drawing.Point(61, 26);
      this.NEW_PANEL_BUTTON.Name = "NEW_PANEL_BUTTON";
      this.NEW_PANEL_BUTTON.Size = new System.Drawing.Size(100, 23);
      this.NEW_PANEL_BUTTON.TabIndex = 178;
      this.NEW_PANEL_BUTTON.Text = "NEW PANEL";
      this.NEW_PANEL_BUTTON.UseVisualStyleBackColor = true;
      this.NEW_PANEL_BUTTON.Click += new System.EventHandler(this.NEW_PANEL_BUTTON_Click);
      // 
      // HELP_BUTTON
      // 
      this.HELP_BUTTON.Location = new System.Drawing.Point(110, 84);
      this.HELP_BUTTON.Name = "HELP_BUTTON";
      this.HELP_BUTTON.Size = new System.Drawing.Size(51, 23);
      this.HELP_BUTTON.TabIndex = 180;
      this.HELP_BUTTON.Text = "HELP";
      this.HELP_BUTTON.UseVisualStyleBackColor = true;
      this.HELP_BUTTON.Click += new System.EventHandler(this.HELP_BUTTON_Click);
      // 
      // SAVE_BUTTON
      // 
      this.SAVE_BUTTON.Location = new System.Drawing.Point(110, 113);
      this.SAVE_BUTTON.Name = "SAVE_BUTTON";
      this.SAVE_BUTTON.Size = new System.Drawing.Size(51, 23);
      this.SAVE_BUTTON.TabIndex = 181;
      this.SAVE_BUTTON.Text = "SAVE";
      this.SAVE_BUTTON.UseVisualStyleBackColor = true;
      this.SAVE_BUTTON.Click += new System.EventHandler(this.SAVE_BUTTON_Click);
      // 
      // LOAD_BUTTON
      // 
      this.LOAD_BUTTON.Location = new System.Drawing.Point(110, 142);
      this.LOAD_BUTTON.Name = "LOAD_BUTTON";
      this.LOAD_BUTTON.Size = new System.Drawing.Size(51, 23);
      this.LOAD_BUTTON.TabIndex = 182;
      this.LOAD_BUTTON.Text = "LOAD";
      this.LOAD_BUTTON.UseVisualStyleBackColor = true;
      this.LOAD_BUTTON.Click += new System.EventHandler(this.LOAD_BUTTON_Click);
      // 
      // DUPLICATE_PANEL_BUTTON
      // 
      this.DUPLICATE_PANEL_BUTTON.Location = new System.Drawing.Point(24, 171);
      this.DUPLICATE_PANEL_BUTTON.Name = "DUPLICATE_PANEL_BUTTON";
      this.DUPLICATE_PANEL_BUTTON.Size = new System.Drawing.Size(137, 23);
      this.DUPLICATE_PANEL_BUTTON.TabIndex = 196;
      this.DUPLICATE_PANEL_BUTTON.Text = "DUPLICATE PANEL";
      this.DUPLICATE_PANEL_BUTTON.UseVisualStyleBackColor = true;
      this.DUPLICATE_PANEL_BUTTON.Click += new System.EventHandler(this.DUPLICATE_PANEL_BUTTON_Click);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1586, 696);
      this.Controls.Add(this.DUPLICATE_PANEL_BUTTON);
      this.Controls.Add(this.LOAD_BUTTON);
      this.Controls.Add(this.SAVE_BUTTON);
      this.Controls.Add(this.HELP_BUTTON);
      this.Controls.Add(this.CREATE_ALL_PANELS_BUTTON);
      this.Controls.Add(this.NEW_PANEL_BUTTON);
      this.Controls.Add(this.PANEL_TABS);
      this.Name = "MainForm";
      this.Text = "Panel Schedule";
      this.PANEL_TABS.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.TabControl PANEL_TABS;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.Button NEW_PANEL_BUTTON;
    private System.Windows.Forms.Button CREATE_ALL_PANELS_BUTTON;
    private System.Windows.Forms.Button HELP_BUTTON;
    private System.Windows.Forms.Button SAVE_BUTTON;
    private System.Windows.Forms.Button LOAD_BUTTON;
    private System.Windows.Forms.Button DUPLICATE_PANEL_BUTTON;
  }
}

