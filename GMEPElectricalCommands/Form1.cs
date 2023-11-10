using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using static OfficeOpenXml.ExcelErrorValue;
using Autodesk.AutoCAD.GraphicsInterface;

namespace AutoCADCommands
{
  public partial class MainForm : Form
  {
    private MyCommands myCommandsInstance;
    private NEWPANELFORM newPanelForm;
    private List<UserControl1> userControls;
    private UserControl1 userControl1;

    public MainForm(MyCommands myCommands)
    {
      InitializeComponent();
      this.myCommandsInstance = myCommands;
      this.newPanelForm = new NEWPANELFORM(this);

      this.userControls = new List<UserControl1>();

      this.FormClosing += MainForm_FormClosing;
    }

    public void InitializeModal()
    {
      PANEL_TABS.TabPages.Clear();
      UserControl1 userControl = new UserControl1(this.myCommandsInstance, this, this.newPanelForm, "Dummy Usercontrol");

      List<Dictionary<string, object>> panelStorage = userControl.retrieve_saved_panel_data();

      string json = JsonConvert.SerializeObject(panelStorage, Formatting.Indented);
      System.IO.File.WriteAllText(@"C:\Users\Public\Documents\panelStorageOpening.json", json);

      if (panelStorage.Count == 0)
      {
        CreateNewPanelTab("A", false);
        return;
      }
      else
      {
        foreach (Dictionary<string, object> panel in panelStorage)
        {
          string panelName = panel["panel"].ToString();
          bool is3PH = panel.ContainsKey("phase_c_left");
          this.myCommandsInstance.WriteMessage("\n" + panelName + " " + is3PH);
          UserControl1 userControl1 = CreateNewPanelTab(panelName, is3PH);
          userControl1.clear_and_set_modal_values(panel);
        }
      }
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (this.userControls.Count == 0) return;

      List<Dictionary<string, object>> panelStorage = new List<Dictionary<string, object>>();

      foreach (UserControl1 userControl in this.userControls)
      {
        panelStorage.Add(userControl.retrieve_data_from_modal());
      }

      string json = JsonConvert.SerializeObject(panelStorage, Formatting.Indented);
      System.IO.File.WriteAllText(@"C:\Users\Public\Documents\panelStorageClosing.json", json);

      this.userControls[0].store_data_in_autocad_file(panelStorage);
      this.userControls = new List<UserControl1>();
    }

    public void AddUserControlToNewTab(UserControl control, TabPage tabPage)
    {
      // Set the user control location and size if needed
      control.Location = new Point(0, 0); // Top-left corner of the tab page
      control.Dock = DockStyle.Fill; // If you want to dock it to fill the tab

      // Add the user control to the controls of the tab page
      tabPage.Controls.Add(control);
    }

    public UserControl1 CreateNewPanelTab(string tabName, bool is3PH)
    {
      // if tabname has "PANEL" in it replace it with "Panel"
      if (tabName.Contains("PANEL") || tabName.Contains("Panel"))
      {
        tabName = tabName.Replace("PANEL", "");
        tabName = tabName.Replace("Panel", "");
      }

      // Create a new TabPage
      TabPage newTabPage = new TabPage(tabName);

      // Add the new TabPage to the TabControl
      PANEL_TABS.TabPages.Add(newTabPage);

      // Optional: Select the newly created tab
      PANEL_TABS.SelectedTab = newTabPage;

      // Create a new UserControl
      UserControl1 userControl1 = new UserControl1(this.myCommandsInstance, this, this.newPanelForm, tabName, is3PH);

      // Add the UserControl to the list of UserControls
      this.userControls.Add(userControl1);

      // Call the method to add the UserControl to the new tab
      AddUserControlToNewTab(userControl1, newTabPage);

      return userControl1;
    }

    public void PANEL_NAME_INPUT_TextChanged(object sender, EventArgs e, string input)
    {
      // Get the selected tab index
      int selectedIndex = PANEL_TABS.SelectedIndex;

      // Check if there is a selected tab
      if (selectedIndex >= 0)
      {
        // Rename the current selected tab
        PANEL_TABS.TabPages[selectedIndex].Text = "Panel " + input;
      }
    }
  }
}