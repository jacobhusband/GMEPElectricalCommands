using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ElectricalCommands
{
  public partial class MainForm : Form
  {
    private PanelCommands myCommandsInstance;
    private NewPanelForm newPanelForm;
    private List<PanelUserControl> userControls;
    private Document acDoc;

    public MainForm(PanelCommands myCommands)
    {
      InitializeComponent();
      this.myCommandsInstance = myCommands;
      this.newPanelForm = new NewPanelForm(this);
      this.userControls = new List<PanelUserControl>();
      this.Shown += MAINFORM_SHOWN;
      this.FormClosing += MAINFORM_CLOSING;
      this.KeyPreview = true;
      this.KeyDown += new KeyEventHandler(MAINFORM_KEYDOWN);
      this.Deactivate += MAINFORM_DEACTIVATE;
      this.acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
      this.acDoc.BeginDocumentClose -= new DocumentBeginCloseEventHandler(docBeginDocClose);
      this.acDoc.BeginDocumentClose += new DocumentBeginCloseEventHandler(docBeginDocClose);
    }

    private void docBeginDocClose(object sender, DocumentBeginCloseEventArgs e)
    {
      SavePanelDataToLocalJsonFile();
      string fileName = this.acDoc.Name;
      this.acDoc.Database.SaveAs(
          fileName,
          true,
          DwgVersion.Current,
          this.acDoc.Database.SecurityParameters
      );
    }

    public List<PanelUserControl> retrieve_userControls()
    {
      return this.userControls;
    }

    public UserControl findUserControl(string panelName)
    {
      foreach (PanelUserControl userControl in userControls)
      {
        string userControlName = userControl.Name.Replace("'", "");
        userControlName = userControlName.Replace(" ", "");
        userControlName = userControlName.Replace("-", "");
        userControlName = userControlName.Replace("PANEL", "");

        panelName = panelName.Replace("'", "");
        panelName = panelName.Replace(" ", "");
        panelName = panelName.Replace("-", "");
        panelName = panelName.Replace("PANEL", "");

        if (userControlName.ToLower() == panelName.ToLower())
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
        MakeTabsAndPopulate(panelStorage);
      }
    }

    public void DuplicatePanel()
    {
      // Get the currently selected tab
      TabPage selectedTab = PANEL_TABS.SelectedTab;

      // Check if a tab is selected
      if (selectedTab != null)
      {
        // Get the UserControl associated with the selected tab
        PanelUserControl selectedUserControl = (PanelUserControl)selectedTab.Controls[0];

        // Retrieve the panel data from the selected UserControl
        Dictionary<string, object> selectedPanelData = selectedUserControl.retrieve_data_from_modal();

        SavePanelDataToLocalJsonFile();

        // Retrieve the saved panel data
        List<Dictionary<string, object>> panelStorage = retrieve_saved_panel_data();

        // Create a copy of the selected panel data
        Dictionary<string, object> duplicatePanelData = new Dictionary<string, object>(selectedPanelData);

        // Get the original panel name
        string originalPanelName = selectedPanelData["panel"].ToString();

        // Generate a new panel name with a number appended
        string newPanelName = GetNewPanelName(originalPanelName);

        // Update the panel name in the duplicate panel data
        duplicatePanelData["panel"] = newPanelName;

        // Add the duplicate panel data to the panel storage
        panelStorage.Add(duplicatePanelData);

        // Save the updated panel data
        StoreDataInJsonFile(panelStorage);

        initialize_modal();
      }
    }

    private string GetNewPanelName(string originalPanelName)
    {
      string newPanelName = originalPanelName;

      // Check if the original panel name ends with a number
      int lastNumber = 0;
      int index = originalPanelName.Length - 1;
      while (index >= 0 && char.IsDigit(originalPanelName[index]))
      {
        lastNumber = lastNumber * 10 + (originalPanelName[index] - '0');
        index--;
      }

      if (lastNumber > 0)
      {
        // Increment the last number
        lastNumber++;
        newPanelName = originalPanelName.Substring(0, index + 1) + lastNumber.ToString();
      }
      else
      {
        // Append "2" to the original panel name
        newPanelName = originalPanelName + "2";
      }

      return newPanelName;
    }

    private void MakeTabsAndPopulate(List<Dictionary<string, object>> panelStorage)
    {
      set_up_cell_values_from_panel_data(panelStorage);
      set_up_tags_from_panel_data(panelStorage);
    }

    public static void put_in_json_file(object thing)
    {
      string json = JsonConvert.SerializeObject(thing, Formatting.Indented);
      string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

      var doc = Autodesk
          .AutoCAD
          .ApplicationServices
          .Application
          .DocumentManager
          .MdiActiveDocument;

      string baseFileName = "Test";

      if (string.IsNullOrEmpty(baseFileName))
      {
        baseFileName = "panel_data";
      }
      string extension = ".json";
      string path = Path.Combine(desktopPath, baseFileName + extension);

      int count = 1;
      while (File.Exists(path))
      {
        string tempFileName = string.Format("{0}({1})", baseFileName, count++);
        path = Path.Combine(desktopPath, tempFileName + extension);
      }

      File.WriteAllText(path, json);
    }

    private void set_up_cell_values_from_panel_data(List<Dictionary<string, object>> panelStorage)
    {
      foreach (Dictionary<string, object> panel in panelStorage)
      {
        string panelName = panel["panel"].ToString();
        bool is3PH = panel.ContainsKey("phase_c_left");
        PanelUserControl userControl1 = create_new_panel_tab(panelName, is3PH);
        userControl1.clear_modal_and_remove_rows(panel);
        userControl1.populate_modal_with_panel_data(panel);
        var notes = JsonConvert.DeserializeObject<List<string>>(panel["notes"].ToString());
        userControl1.update_notes_storage(notes);
      }
    }

    private void set_up_tags_from_panel_data(List<Dictionary<string, object>> panelStorage)
    {
      foreach (Dictionary<string, object> panel in panelStorage)
      {
        string panelName = panel["panel"].ToString();
        PanelUserControl userControl1 = (PanelUserControl)findUserControl(panelName);
        if (userControl1 == null)
        {
          continue;
        }
        DataGridView panelGrid = userControl1.retrieve_panelGrid();
        foreach (DataGridViewRow row in panelGrid.Rows)
        {
          int rowIndex = row.Index;
          var tagNames = new Dictionary<string, int>();
          if (panel.ContainsKey("phase_c_left"))
          {
            tagNames = new Dictionary<string, int>()
                        {
                            { "phase_a_left_tag", 1 },
                            { "phase_b_left_tag", 2 },
                            { "phase_c_left_tag", 3 },
                            { "phase_a_right_tag", 8 },
                            { "phase_b_right_tag", 9 },
                            { "phase_c_right_tag", 10 },
                            { "description_left_tags", 0 },
                            { "description_right_tags", 11 }
                        };
          }
          else
          {
            tagNames = new Dictionary<string, int>()
                        {
                            { "phase_a_left_tag", 1 },
                            { "phase_b_left_tag", 2 },
                            { "phase_a_right_tag", 7 },
                            { "phase_b_right_tag", 8 },
                            { "description_left_tags", 0 },
                            { "description_right_tags", 9 }
                        };
          }

          foreach (var tagName in tagNames)
          {
            if (panel.ContainsKey(tagName.Key))
            {
              set_cell_value(panel, tagName.Key, rowIndex, tagName.Value, row);
            }
          }
        }
        userControl1.update_cell_background_color();
        userControl1.recalculate_breakers();
        userControl1.calculate_lcl_otherload_panelload_feederamps();
      }
    }

    private void set_cell_value(Dictionary<string, object> panel, string key, int rowIndex, int cellIndex, DataGridViewRow row)
    {
      string tag = panel[key].ToString();
      List<string> tagList = JsonConvert.DeserializeObject<List<string>>(tag);
      string tagValue = tagList[rowIndex];
      if (tagValue != "")
      {
        if (key.Contains("phase"))
        {
          row.Cells[cellIndex].Value = tagValue;
        }
        else if (key.Contains("description"))
        {
          row.Cells[cellIndex].Tag = tagValue;
        }
      }
    }

    internal void delete_panel(PanelUserControl userControl1)
    {
      DialogResult dialogResult = MessageBox.Show(
          "Are you sure you want to delete this panel?",
          "Delete Panel",
          MessageBoxButtons.YesNo
      );
      if (dialogResult == DialogResult.Yes)
      {
        this.userControls.Remove(userControl1);
        PANEL_TABS.TabPages.Remove(userControl1.Parent as TabPage);
        using (
            DocumentLock docLock =
                Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument()
        )
        {
          remove_panel_from_storage(userControl1);
        }
      }
    }

    private void remove_panel_from_storage(PanelUserControl userControl1)
    {
      var panelData = retrieve_saved_panel_data();

      foreach (Dictionary<string, object> panel in panelData)
      {
        var panelName = panel["panel"].ToString().Replace("\'", "").Replace("`", "");
        var userControlName = userControl1.Name.Replace("\'", "").Replace("`", "");
        if (panelName == userControlName)
        {
          panelData.Remove(panel);
          break;
        }
      }

      StoreDataInJsonFile(panelData);
    }

    internal bool panel_name_exists(string panelName)
    {
      foreach (TabPage tabPage in PANEL_TABS.TabPages)
      {
        if (tabPage.Text.Split(' ')[1].ToLower() == panelName.ToLower())
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

    public PanelUserControl create_new_panel_tab(string tabName, bool is3PH)
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
      PanelUserControl userControl1 = new PanelUserControl(
          this.myCommandsInstance,
          this,
          this.newPanelForm,
          tabName,
          is3PH
      );

      // Add the UserControl to the list of UserControls
      this.userControls.Add(userControl1);

      // Call the method to add the UserControl to the new tab
      add_usercontrol_to_new_tab(userControl1, newTabPage);

      return userControl1;
    }

    public List<Dictionary<string, object>> retrieve_saved_panel_data()
    {
      List<Dictionary<string, object>> allPanelData = new List<Dictionary<string, object>>();

      string acDocPath = Path.GetDirectoryName(this.acDoc.Name);
      string savesDirectory = Path.Combine(acDocPath, "Saves");
      string panelSavesDirectory = Path.Combine(savesDirectory, "Panel");

      // Check if the "Saves/Panel" directory exists
      if (Directory.Exists(panelSavesDirectory))
      {
        // Get all JSON files in the directory
        string[] jsonFiles = Directory.GetFiles(panelSavesDirectory, "*.json");

        // If there are any JSON files, find the most recent one
        if (jsonFiles.Length > 0)
        {
          string mostRecentJsonFile = jsonFiles
              .OrderByDescending(f => File.GetLastWriteTime(f))
              .First();

          // Read the JSON data from the file
          string jsonData = File.ReadAllText(mostRecentJsonFile);

          // Deserialize the JSON data to a list of dictionaries
          allPanelData = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonData);
        }
      }

      return allPanelData;
    }

    public void SavePanelDataToLocalJsonFile()
    {
      List<Dictionary<string, object>> panelStorage = new List<Dictionary<string, object>>();

      if (this.acDoc != null)
      {
        using (DocumentLock docLock = this.acDoc.LockDocument())
        {
          foreach (PanelUserControl userControl in this.userControls)
          {
            panelStorage.Add(userControl.retrieve_data_from_modal());
          }

          StoreDataInJsonFile(panelStorage);
        }
      }
    }

    public void StoreDataInJsonFile(List<Dictionary<string, object>> saveData)
    {
      string acDocPath = Path.GetDirectoryName(this.acDoc.Name);
      string savesDirectory = Path.Combine(acDocPath, "Saves");
      string panelSavesDirectory = Path.Combine(savesDirectory, "Panel");

      // Create the "Saves" directory if it doesn't exist
      if (!Directory.Exists(savesDirectory))
      {
        Directory.CreateDirectory(savesDirectory);
      }

      // Create the "Saves/Panel" directory if it doesn't exist
      if (!Directory.Exists(panelSavesDirectory))
      {
        Directory.CreateDirectory(panelSavesDirectory);
      }

      // Create a JSON file name based on the AutoCAD file name and the current timestamp
      string acDocFileName = Path.GetFileNameWithoutExtension(acDoc.Name);
      string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
      string jsonFileName = acDocFileName + "_" + timestamp + ".json";
      string jsonFilePath = Path.Combine(panelSavesDirectory, jsonFileName);

      // Serialize all the panel data to JSON
      string jsonData = JsonConvert.SerializeObject(saveData, Formatting.Indented);

      // Write the JSON data to the file
      File.WriteAllText(jsonFilePath, jsonData);
    }

    private void MAINFORM_CLOSING(object sender, FormClosingEventArgs e)
    {
      this.acDoc.BeginDocumentClose -= new DocumentBeginCloseEventHandler(docBeginDocClose);
      SavePanelDataToLocalJsonFile();
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

    private void MAINFORM_DEACTIVATE(object sender, EventArgs e)
    {
      foreach (PanelUserControl userControl in userControls)
      {
        DataGridView panelGrid = userControl.retrieve_panelGrid();
        panelGrid.ClearSelection();
      }
    }

    private void NEW_PANEL_BUTTON_Click(object sender, EventArgs e)
    {
      this.newPanelForm.ShowDialog();
    }

    private void CREATE_ALL_PANELS_BUTTON_Click(object sender, EventArgs e)
    {
      List<PanelUserControl> userControls = retrieve_userControls();
      List<Dictionary<string, object>> panels = new List<Dictionary<string, object>>();

      foreach (PanelUserControl userControl in userControls)
      {
        Dictionary<string, object> panelData = userControl.retrieve_data_from_modal();
        panels.Add(panelData);
      }

      using (
          DocumentLock docLock =
              Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument()
      )
      {
        Close();
        myCommandsInstance.Create_Panels(panels);

        Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.WindowState = Autodesk.AutoCAD.Windows.Window.State.Maximized;
        Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Window.Focus();
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

    private void HELP_BUTTON_Click(object sender, EventArgs e)
    {
      HelpForm helpForm = new HelpForm();

      helpForm.ShowDialog();
    }

    private void SAVE_BUTTON_Click(object sender, EventArgs e)
    {
      SavePanelDataToLocalJsonFile();
    }

    private void MAINFORM_KEYDOWN(object sender, KeyEventArgs e)
    {
      if (e.Control && e.KeyCode == Keys.S)
      {
        SavePanelDataToLocalJsonFile();
      }
    }

    private void LOAD_BUTTON_Click(object sender, EventArgs e)
    {
      Close();
      // Prompt the user to select a JSON file
      OpenFileDialog openFileDialog = new OpenFileDialog
      {
        Filter = "JSON files (*.json)|*.json",
        Title = "Select a JSON file"
      };

      if (openFileDialog.ShowDialog() == DialogResult.OK)
      {
        // Read the JSON data from the file
        string jsonData = File.ReadAllText(openFileDialog.FileName);

        // Deserialize the JSON data to a list of dictionaries
        List<Dictionary<string, object>> panelData = JsonConvert.DeserializeObject<
            List<Dictionary<string, object>>
        >(jsonData);

        // Save the panel data
        StoreDataInJsonFile(panelData);
      }
    }

    private void DUPLICATE_PANEL_BUTTON_Click(object sender, EventArgs e)
    {
      DuplicatePanel();
    }
  }
}