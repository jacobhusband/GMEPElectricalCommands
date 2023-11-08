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
      PANEL_TABS.TabPages.Clear();
      CreateNewPanelTab("A");
    }

    public void AddUserControlToNewTab(UserControl control, TabPage tabPage)
    {
      // Set the user control location and size if needed
      control.Location = new Point(0, 0); // Top-left corner of the tab page
      control.Dock = DockStyle.Fill; // If you want to dock it to fill the tab

      // Add the user control to the controls of the tab page
      tabPage.Controls.Add(control);
    }

    public void CreateNewPanelTab(string tabName)
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
      UserControl1 userControl1 = new UserControl1(this.myCommandsInstance, this, this.newPanelForm, tabName);

      // Add the UserControl to the list of UserControls
      this.userControls.Add(userControl1);

      // Call the method to add the UserControl to the new tab
      AddUserControlToNewTab(userControl1, newTabPage);
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