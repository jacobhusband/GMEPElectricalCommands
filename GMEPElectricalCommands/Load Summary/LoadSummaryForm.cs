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
  public partial class LoadSummaryForm : UserControl {
    private LoadSummaryCommands myCommandsInstance;
    private DialogWindow dialogWindow;
    private NewLoadSummaryForm newLoadSummaryForm;
    
    public LoadSummaryForm(
      LoadSummaryCommands myCommands,
      DialogWindow dialogWindow,
      NewLoadSummaryForm newLoadSummaryForm,
      string tabName
    ) {
      InitializeComponent();
      myCommandsInstance = myCommands;
      this.dialogWindow = dialogWindow;
      this.newLoadSummaryForm = newLoadSummaryForm;
      this.Name = tabName;

      listen_for_new_rows();
    }
    private void listen_for_new_rows() {
      // TODO
      REMOVED_LOAD_FLOW_LAYOUT_PANEL.ControlAdded += new ControlEventHandler(REMOVED_LOAD_FLOW_LAYOUT_PANEL_ControlAdded);
    }

    private void REMOVED_LOAD_FLOW_LAYOUT_PANEL_ControlAdded(object sender, ControlEventArgs e) {
      // instantiate new LoadSummaryListElement
      LoadSummaryListElement elem = new LoadSummaryListElement();

    }

    public Dictionary<string, object> retrieve_data_from_modal() {
      Dictionary<string, object> loadSummary = new Dictionary<string, object>();
      string loadSummaryResult = LOAD_SUMMARY_RESULT_TEXTBOX.Text;
      loadSummaryResult = loadSummaryResult.Replace("A", "");
      double parsedLoadSummaryResult = Convert.ToDouble(loadSummaryResult);
      double safetyFactor = 1;
      if (!String.IsNullOrEmpty(SAFETY_FACTOR_TEXTBOX.Text)) {
        safetyFactor = Convert.ToDouble(SAFETY_FACTOR_TEXTBOX.Text);
      }
      string loadSummaryResultWithSafetyFactor;
      bool safetyFactorChecked = SAFETY_FACTOR_CHECKBOX.Checked;
      if (safetyFactorChecked) {
        loadSummaryResultWithSafetyFactor = loadSummaryResult;
        loadSummaryResult = (parsedLoadSummaryResult / safetyFactor).ToString();
      }
      else {
        loadSummaryResultWithSafetyFactor = (parsedLoadSummaryResult * safetyFactor).ToString();
      }
      loadSummary.Add("result", loadSummaryResult);
      loadSummary.Add("twelve_month_utility_bill", TWELVE_MONTH_UTILITY_BILL_TEXTBOX.Text.Trim());
      loadSummary.Add("using_safety_factor", safetyFactorChecked);
      loadSummary.Add("safety_factor", SAFETY_FACTOR_TEXTBOX.Text.Trim());
      loadSummary.Add("system_voltage", SYSTEM_VOLTAGE_COMBO_BOX.Text);
      loadSummary.Add("system_phase", SYSTEM_PHASE_COMBO_BOX.Text);
      loadSummary.Add("override_panels", OVERRIDE_PANEL_LOADS_CHECKBOX.Text);

      // TODO: Iterate over panels, removed loads, and additional loads flow layouts
      /*
      expected data structure for panels:
      {
        id: <panel-uuid>,
        enabled: bool,
        subpanels: [{
          id: <panel-uuid>
        }]
      }

      expected data structure for additional loads:
      {
        id: <uuid>,
        name: string,
        enabled: bool,
        load: double,
        unit: <"KVA" or "A">
      }
      */
      return loadSummary;
    }

    //private System.Windows.Forms.TextBox LOAD_SUMMARY_RESULT_TEXTBOX;
    //private System.Windows.Forms.FlowLayoutPanel REMOVED_LOAD_FLOW_LAYOUT_PANEL;
    //private System.Windows.Forms.FlowLayoutPanel ADDITIONAL_LOAD_FLOW_LAYOUT_PANEL;
    //private System.Windows.Forms.CheckBox OVERRIDE_PANEL_LOADS_CHECKBOX;
    //private System.Windows.Forms.TextBox TWELVE_MONTH_UTILITY_BILL_TEXTBOX;
    //private System.Windows.Forms.CheckBox SAFETY_FACTOR_CHECKBOX;
    //private System.Windows.Forms.ComboBox SYSTEM_PHASE_COMBO_BOX;
    //private System.Windows.Forms.ComboBox SYSTEM_VOLTAGE_COMBO_BOX;

    private void clear_current_modal_data() {
      LOAD_SUMMARY_RESULT_TEXTBOX.Clear();
      ADDITIONAL_LOAD_FLOW_LAYOUT_PANEL.Controls.Clear();
      PANEL_FLOW_LAYOUT_PANEL.Controls.Clear();
      REMOVED_LOAD_FLOW_LAYOUT_PANEL.Controls.Clear();
      OVERRIDE_PANEL_LOADS_CHECKBOX.Checked = false;
      TWELVE_MONTH_UTILITY_BILL_TEXTBOX.Text = "0";
      SAFETY_FACTOR_CHECKBOX.Checked = false;
    }

    private void PANEL_FLOW_LAYOUT_PANEL_Paint(object sender, PaintEventArgs e) {

    }

    

    public void AddListener() {
      
    }

  }
  
}
