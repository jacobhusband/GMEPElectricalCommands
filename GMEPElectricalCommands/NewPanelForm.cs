using AutoCADCommands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoCADCommands
{
  public partial class NEWPANELFORM : Form
  {
    private MainForm _mainForm;

    public NEWPANELFORM(MainForm mainForm)
    {
      InitializeComponent();
      _mainForm = mainForm;
    }

    private void CREATEPANEL_Click(object sender, EventArgs e)
    {
      // get the state of the checkbox
      bool is3PH = CHECKBOX3PH.Checked;

      // get the value of the textbox
      string panelName = CREATEPANELNAME.Text;

      // call a method on the main form
      if (_mainForm != null)
      {
        _mainForm.create_new_panel_tab(panelName, is3PH);
      }
    }
  }
}