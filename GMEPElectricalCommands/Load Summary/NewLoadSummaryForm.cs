using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElectricalCommands.Load_Summary {
  public partial class NewLoadSummaryForm : Form {
    private DialogWindow dialogWindow;
    public NewLoadSummaryForm(DialogWindow dialogWindow) {
      InitializeComponent();
      this.StartPosition = FormStartPosition.CenterParent;
      this.dialogWindow = dialogWindow;
    }

    private void NEW_LOAD_SUMMARY_LABEL_Click(object sender, EventArgs e) {
      string loadSummaryName = NEW_LOAD_SUMMARY_TEXTBOX.Text;
      if (this.dialogWindow.load_summary_name_exists(loadSummaryName)) {
        MessageBox.Show("Load Section name already exists. Please choose another name.");
        return;
      }
      if (loadSummaryName == "") {
        MessageBox.Show("Load Section name cannot be empty.");
        return;
      }
      if (this.dialogWindow != null) {
        var userControl = this.dialogWindow.create_new_load_summary_tab(loadSummaryName);
        userControl.AddListeners();
      }
    }
  }
}
