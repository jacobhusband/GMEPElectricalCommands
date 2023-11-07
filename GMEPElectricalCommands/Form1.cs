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
    private UserControl1 userControl1;

    public MainForm(MyCommands myCommands)
    {
      InitializeComponent();
      myCommandsInstance = myCommands;
      this.newPanelForm = new NEWPANELFORM(this);
      this.userControl1 = new UserControl1(myCommands, this, this.newPanelForm);
      this.PANEL_TABS.TabPages[0].Controls.Add(this.userControl1);
    }

    public void create_new_panel_tab_in_modal()
    {
      // Create a new TabPage
      TabPage newTabPage = new TabPage("New Tab");

      // Add the new TabPage to the TabControl
      PANEL_TABS.TabPages.Add(newTabPage);

      // Optional: Select the newly created tab
      PANEL_TABS.SelectedTab = newTabPage;
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