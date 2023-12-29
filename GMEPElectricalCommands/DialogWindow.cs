﻿using Autodesk.AutoCAD.DatabaseServices;
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
using System.IO;
using Newtonsoft.Json;
using static OfficeOpenXml.ExcelErrorValue;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.ApplicationServices;

namespace GMEPElectricalCommands
{
  public partial class MainForm : Form
  {
    private MyCommands myCommandsInstance;
    private NEWPANELFORM newPanelForm;
    private List<UserInterface> userControls;

    public MainForm(MyCommands myCommands)
    {
      InitializeComponent();
      this.myCommandsInstance = myCommands;
      this.newPanelForm = new NEWPANELFORM(this);
      this.userControls = new List<UserInterface>();
      this.FormClosing += MAINFORM_FormClosing;
      this.Shown += MAINFORM_SHOWN;
    }

    public List<UserInterface> retrieve_userControls()
    {
      return this.userControls;
    }

    public UserControl findUserControl(string panelName)
    {
      foreach (UserInterface userControl in userControls)
      {
        string userControlName = userControl.Name.Replace("'", "");
        userControlName = userControlName.Replace(" ", "");
        userControlName = userControlName.Replace("-", "");
        userControlName = userControlName.Replace("PANEL", "");

        panelName = panelName.Replace("'", "");
        panelName = panelName.Replace(" ", "");
        panelName = panelName.Replace("-", "");
        panelName = panelName.Replace("PANEL", "");

        if (userControlName == panelName)
        {
          return userControl;
        }
      }

      return null;
    }

    public void initialize_modal()
    {
      PANEL_TABS.TabPages.Clear();

      List<Dictionary<string, object>> panelStorage = retrieve_saved_panel_data();

      if (panelStorage.Count == 0)
      {
        return;
      }
      else
      {
        foreach (Dictionary<string, object> panel in panelStorage)
        {
          string panelName = panel["panel"].ToString();
          bool is3PH = panel.ContainsKey("phase_c_left");
          UserInterface userControl1 = create_new_panel_tab(panelName, is3PH);
          userControl1.clear_and_set_modal_values(panel);
          var notes = JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(panel["notes"].ToString());
          userControl1.update_notes_storage(notes);
        }

        foreach (Dictionary<string, object> panel in panelStorage)
        {
          string panelName = panel["panel"].ToString();
          UserInterface userControl1 = (UserInterface)findUserControl(panelName);
          if (userControl1 == null)
          {
            continue;
          }
          DataGridView panelGrid = userControl1.retrieve_panelGrid();
          foreach (DataGridViewRow row in panelGrid.Rows)
          {
            int rowIndex = row.Index;
            int breaker_left_index = 3;
            int breaker_right_index = 6;
            int phase_a_right_index = 7;
            int phase_b_right_index = 8;
            int description_right_index = 9;

            string phase_a_left_tag = panel["phase_a_left_tag"].ToString();
            List<string> phase_a_left_tag_list = JsonConvert.DeserializeObject<List<string>>(phase_a_left_tag);
            string phase_a_left_tag_value = phase_a_left_tag_list[rowIndex];
            if (phase_a_left_tag_value != "")
            {
              row.Cells[1].Value = phase_a_left_tag_value;
            }
            // repeat for each phase
            string phase_b_left_tag = panel["phase_b_left_tag"].ToString();
            List<string> phase_b_left_tag_list = JsonConvert.DeserializeObject<List<string>>(phase_b_left_tag);
            string phase_b_left_tag_value = phase_b_left_tag_list[rowIndex];
            if (phase_b_left_tag_value != "")
            {
              row.Cells[2].Value = phase_b_left_tag_value;
            }
            // check if c exists
            if (panel.ContainsKey("phase_c_left"))
            {
              string phase_c_left_tag = panel["phase_c_left_tag"].ToString();
              List<string> phase_c_left_tag_list = JsonConvert.DeserializeObject<List<string>>(phase_c_left_tag);
              string phase_c_left_tag_value = phase_c_left_tag_list[rowIndex];
              if (phase_c_left_tag_value != "")
              {
                row.Cells[3].Value = phase_c_left_tag_value;
              }
              breaker_left_index = 4;
              breaker_right_index = 7;
              phase_a_right_index = 8;
              phase_b_right_index = 9;
              description_right_index = 11;
            }
            string phase_a_right_tag = panel["phase_a_right_tag"].ToString();
            List<string> phase_a_right_tag_list = JsonConvert.DeserializeObject<List<string>>(phase_a_right_tag);
            string phase_a_right_tag_value = phase_a_right_tag_list[rowIndex];
            if (phase_a_right_tag_value != "")
            {
              row.Cells[phase_a_right_index].Value = phase_a_right_tag_value;
            }
            // repeat for each phase
            string phase_b_right_tag = panel["phase_b_right_tag"].ToString();
            List<string> phase_b_right_tag_list = JsonConvert.DeserializeObject<List<string>>(phase_b_right_tag);
            string phase_b_right_tag_value = phase_b_right_tag_list[rowIndex];
            if (phase_b_right_tag_value != "")
            {
              row.Cells[phase_b_right_index].Value = phase_b_right_tag_value;
            }
            // check if c exists
            if (panel.ContainsKey("phase_c_right"))
            {
              string phase_c_right_tag = panel["phase_c_right_tag"].ToString();
              List<string> phase_c_right_tag_list = JsonConvert.DeserializeObject<List<string>>(phase_c_right_tag);
              string phase_c_right_tag_value = phase_c_right_tag_list[rowIndex];
              if (phase_c_right_tag_value != "")
              {
                row.Cells[10].Value = phase_c_right_tag_value;
              }
            }
            string description_left_tag = panel["description_left_tags"].ToString();
            List<string> description_left_tag_list = JsonConvert.DeserializeObject<List<string>>(description_left_tag);
            string description_left_tag_value = description_left_tag_list[rowIndex];
            if (description_left_tag_value != "")
            {
              row.Cells[0].Tag = description_left_tag_value;
            }
            string description_right_tag = panel["description_right_tags"].ToString();
            List<string> description_right_tag_list = JsonConvert.DeserializeObject<List<string>>(description_right_tag);
            string description_right_tag_value = description_right_tag_list[rowIndex];
            if (description_right_tag_value != "")
            {
              row.Cells[description_right_index].Tag = description_right_tag_value;
            }
            string breaker_left_tag = panel["breaker_left_tags"].ToString();
            List<string> breaker_left_tag_list = JsonConvert.DeserializeObject<List<string>>(breaker_left_tag);
            string breaker_left_tag_value = breaker_left_tag_list[rowIndex];
            if (breaker_left_tag_value != "")
            {
              row.Cells[breaker_left_index].Tag = breaker_left_tag_value;
            }
            string breaker_right_tag = panel["breaker_right_tags"].ToString();
            List<string> breaker_right_tag_list = JsonConvert.DeserializeObject<List<string>>(breaker_right_tag);
            string breaker_right_tag_value = breaker_right_tag_list[rowIndex];
            if (breaker_right_tag_value != "")
            {
              row.Cells[breaker_right_index].Tag = breaker_right_tag_value;
            }
          }
        }
      }
    }

    internal void delete_panel(UserInterface userControl1)
    {
      // remove the tab and remove the usercontrol from the list, prompt the user first so they have a chance to say no
      DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this panel?", "Delete Panel", MessageBoxButtons.YesNo);
      if (dialogResult == DialogResult.Yes)
      {
        this.userControls.Remove(userControl1);
        PANEL_TABS.TabPages.Remove(userControl1.Parent as TabPage);
      }
    }

    internal bool panel_name_exists(string panelName)
    {
      foreach (TabPage tabPage in PANEL_TABS.TabPages)
      {
        if (tabPage.Text.Split(' ')[1] == panelName)
        {
          return true;
        }
      }
      return false;
    }

    public void add_usercontrol_to_new_tab(UserControl control, TabPage tabPage)
    {
      // Set the user control location and size if needed
      control.Location = new Point(0, 0); // Top-left corner of the tab page
      control.Dock = DockStyle.Fill; // If you want to dock it to fill the tab

      // Add the user control to the controls of the tab page
      tabPage.Controls.Add(control);
    }

    public UserInterface create_new_panel_tab(string tabName, bool is3PH)
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
      UserInterface userControl1 = new UserInterface(this.myCommandsInstance, this, this.newPanelForm, tabName, is3PH);

      // Add the UserControl to the list of UserControls
      this.userControls.Add(userControl1);

      // Call the method to add the UserControl to the new tab
      add_usercontrol_to_new_tab(userControl1, newTabPage);

      return userControl1;
    }

    public void store_data_in_autocad_file(List<Dictionary<string, object>> saveData)
    {
      Autodesk.AutoCAD.ApplicationServices.Document acDoc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
      Autodesk.AutoCAD.DatabaseServices.Database acCurDb = acDoc.Database;
      string jsonDataKey = "JsonSaveData";

      using (Autodesk.AutoCAD.DatabaseServices.Transaction tr = acCurDb.TransactionManager.StartTransaction())
      {
        Autodesk.AutoCAD.DatabaseServices.DBDictionary nod = (Autodesk.AutoCAD.DatabaseServices.DBDictionary)tr.GetObject(acCurDb.NamedObjectsDictionaryId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead);

        Autodesk.AutoCAD.DatabaseServices.DBDictionary userDict;
        if (nod.Contains(jsonDataKey))
        {
          // The dictionary already exists, so we just need to open it for write
          userDict = (Autodesk.AutoCAD.DatabaseServices.DBDictionary)tr.GetObject(nod.GetAt(jsonDataKey), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite);
        }
        else
        {
          // The dictionary doesn't exist, so we create a new one and add it to the NOD
          userDict = new Autodesk.AutoCAD.DatabaseServices.DBDictionary();
          nod.UpgradeOpen();
          nod.SetAt(jsonDataKey, userDict);
          tr.AddNewlyCreatedDBObject(userDict, true);
        }

        // Now let's update or create the Xrecord for each panel
        for (int i = 0; i < saveData.Count; i++)
        {
          string panelKey = "PanelData" + i.ToString("D3");
          Autodesk.AutoCAD.DatabaseServices.Xrecord xRecord;
          if (userDict.Contains(panelKey))
          {
            // The Xrecord exists, open it for write to update
            xRecord = (Autodesk.AutoCAD.DatabaseServices.Xrecord)tr.GetObject(userDict.GetAt(panelKey), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite);
          }
          else
          {
            // The Xrecord does not exist, create a new one
            xRecord = new Autodesk.AutoCAD.DatabaseServices.Xrecord();
            userDict.SetAt(panelKey, xRecord);
            tr.AddNewlyCreatedDBObject(xRecord, true);
          }

          // Update the Xrecord data
          Autodesk.AutoCAD.DatabaseServices.ResultBuffer rb = new Autodesk.AutoCAD.DatabaseServices.ResultBuffer(new Autodesk.AutoCAD.DatabaseServices.TypedValue((int)Autodesk.AutoCAD.DatabaseServices.DxfCode.Text, JsonConvert.SerializeObject(saveData[i], Formatting.Indented)));
          xRecord.Data = new Autodesk.AutoCAD.DatabaseServices.ResultBuffer();
          xRecord.Data = rb;
        }

        tr.Commit();
      }
    }

    public List<Dictionary<string, object>> retrieve_saved_panel_data()
    {
      List<Dictionary<string, object>> allPanelData = new List<Dictionary<string, object>>();

      Autodesk.AutoCAD.ApplicationServices.Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
      Autodesk.AutoCAD.DatabaseServices.Database acCurDb = acDoc.Database;
      string jsonDataKey = "JsonSaveData";

      using (Autodesk.AutoCAD.DatabaseServices.Transaction tr = acCurDb.TransactionManager.StartTransaction())
      {
        Autodesk.AutoCAD.DatabaseServices.DBDictionary nod = (Autodesk.AutoCAD.DatabaseServices.DBDictionary)tr.GetObject(acCurDb.NamedObjectsDictionaryId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead);

        if (nod.Contains(jsonDataKey))
        {
          Autodesk.AutoCAD.DatabaseServices.DBDictionary userDict = (Autodesk.AutoCAD.DatabaseServices.DBDictionary)tr.GetObject(nod.GetAt(jsonDataKey), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead);

          // Iterate over all XRecords in the user dictionary
          foreach (var entry in userDict)
          {
            Autodesk.AutoCAD.DatabaseServices.Xrecord xRecord = (Autodesk.AutoCAD.DatabaseServices.Xrecord)tr.GetObject(entry.Value, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead);
            Autodesk.AutoCAD.DatabaseServices.ResultBuffer rb = xRecord.Data;
            if (rb != null)
            {
              foreach (Autodesk.AutoCAD.DatabaseServices.TypedValue tv in rb)
              {
                if (tv.TypeCode == (int)Autodesk.AutoCAD.DatabaseServices.DxfCode.Text)
                {
                  Dictionary<string, object> panelData = JsonConvert.DeserializeObject<Dictionary<string, object>>(tv.Value.ToString());
                  allPanelData.Add(panelData);
                }
              }
            }
          }
        }
      }
      return allPanelData;
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

    private void MAINFORM_FormClosing(object sender, FormClosingEventArgs e)
    {
      List<Dictionary<string, object>> panelStorage = new List<Dictionary<string, object>>();

      using (DocumentLock docLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument())
      {
        foreach (UserInterface userControl in this.userControls)
        {
          userControl.add_circuits_to_note_storage();
          panelStorage.Add(userControl.retrieve_data_from_modal());
        }
        store_data_in_autocad_file(panelStorage);
        this.userControls = new List<UserInterface>();
      }
    }

    private void NEW_PANEL_BUTTON_Click(object sender, EventArgs e)
    {
      this.newPanelForm.ShowDialog();
    }

    private void CREATE_ALL_PANELS_BUTTON_Click(object sender, EventArgs e)
    {
      List<UserInterface> userControls = retrieve_userControls();
      List<Dictionary<string, object>> panels = new List<Dictionary<string, object>>();

      foreach (UserInterface userControl in userControls)
      {
        Dictionary<string, object> panelData = userControl.retrieve_data_from_modal();
        panels.Add(panelData);
      }

      using (DocumentLock docLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument())
      {
        Close();
        myCommandsInstance.Create_Panels(panels);
      }
    }

    private void MAINFORM_SHOWN(object sender, EventArgs e)
    {
      // Check if the userControls list is empty
      if (this.userControls.Count == 0)
      {
        // If empty, show newPanelForm as a modal dialog
        newPanelForm.ShowDialog(); // or use appropriate method to show it as modal
      }
    }
  }
}