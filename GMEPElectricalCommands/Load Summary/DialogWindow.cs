using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.GraphicsInterface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElectricalCommands.Load_Summary {
  public partial class DialogWindow : Form {
    private LoadSummaryCommands myCommandsInstance;
    private NewLoadSummaryForm newLoadSummaryForm;
    private List<LoadSummaryForm> userControls;
    private Document acDoc;
    public DialogWindow(LoadSummaryCommands myCommands) {
      InitializeComponent();
      this.myCommandsInstance = myCommands;
      this.newLoadSummaryForm = new NewLoadSummaryForm(this);
      this.userControls = new List<LoadSummaryForm>();
      this.Shown += DIALOG_WINDOW_SHOWN;
      this.FormClosing += DIALOG_WINDOW_CLOSING;
      this.KeyPreview = true;
      this.KeyDown += new KeyEventHandler(DIALOG_WINDOW_KEYDOWN);
      this.Deactivate += DIALOG_WINDOW_DEACTIVATE;
      this.acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
      this.acDoc.BeginDocumentClose -= new DocumentBeginCloseEventHandler(docBeginDocClose);
      this.acDoc.BeginDocumentClose += new DocumentBeginCloseEventHandler(docBeginDocClose);
    }

    private void DIALOG_WINDOW_SHOWN(object sender, EventArgs e) {
      if (this.userControls.Count > 0) {
        newLoadSummaryForm.ShowDialog();
      }
    }

    private void DIALOG_WINDOW_CLOSING(object sender, EventArgs e) {
      this.acDoc.BeginDocumentClose -= new DocumentBeginCloseEventHandler(docBeginDocClose);
      SaveLoadSummaryDataToLocalJsonFile();
    }

    private void DIALOG_WINDOW_KEYDOWN(object sender, KeyEventArgs e) {
      if (e.Control && e.KeyCode == Keys.S) {
        SaveLoadSummaryDataToLocalJsonFile();
      }
    }

    private void DIALOG_WINDOW_DEACTIVATE(object sender, EventArgs e) {
      foreach (LoadSummaryForm userControl in userControls) {
        FlowLayoutPanel aggFlowLayout = userControl.retrieve_aggregated_loads_flow_layout();
        aggFlowLayout.Controls.Clear();
        FlowLayoutPanel remFlowLayout = userControl.retrieve_removed_loads_flow_layout();
        remFlowLayout.Controls.Clear();
      }
    }

    private void docBeginDocClose(object sender, DocumentBeginCloseEventArgs e) {
      SaveLoadSummaryDataToLocalJsonFile();
      string fileName = this.acDoc.Name;
      this.acDoc.Database.SaveAs(
        fileName,
        true,
        DwgVersion.Current,
        this.acDoc.Database.SecurityParameters
      );
    }

    private void SaveLoadSummaryDataToLocalJsonFile() {
      List<Dictionary<string, object>> loadSummaryStorage = new List<Dictionary<string, object>>();

      if (this.acDoc != null) {
        using (DocumentLock docLock = this.acDoc.LockDocument()) {
          foreach (LoadSummaryForm userControl in this.userControls) {
            loadSummaryStorage.Add(userControl.retrieve_data_from_modal());
          }

          StoreDataInJsonFile(loadSummaryStorage);
        }
      }
    }

    public void StoreDataInJsonFile(List<Dictionary<string, object>> saveData) {
      string acDocPath = Path.GetDirectoryName(this.acDoc.Name);
      string savesDirectory = Path.Combine(acDocPath, "Saves");
      string loadSummarySavesDirectory = Path.Combine(savesDirectory, "LoadSummary");

      // Create the "Saves" directory if it doesn't exist
      if (!Directory.Exists(savesDirectory)) {
        Directory.CreateDirectory(savesDirectory);
      }

      // Create the "Saves/LoadSummary" directory if it doesn't exist
      if (!Directory.Exists(loadSummarySavesDirectory)) {
        Directory.CreateDirectory(loadSummarySavesDirectory);
      }

      // Create a JSON file name based on the AutoCAD file name and the current timestamp
      string acDocFileName = Path.GetFileNameWithoutExtension(acDoc.Name);
      string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
      string jsonFileName = acDocFileName + "_" + timestamp + ".json";
      string jsonFilePath = Path.Combine(loadSummarySavesDirectory, jsonFileName);

      // Serialize all the load summary data to JSON
      string jsonData = JsonConvert.SerializeObject(saveData, Formatting.Indented);

      // Write the JSON data to the file
      File.WriteAllText(jsonFilePath, jsonData);
    }

    private void DialogWindow_Load(object sender, EventArgs e) {

    }

    private void button1_Click(object sender, EventArgs e) {

    }

    public void initialize_modal() {
      LOAD_SUMMARY_TABS.TabPages.Clear();

      List<Dictionary<string, object>> loadSummaryStorage = retrieve_saved_load_summary_data();

      if (loadSummaryStorage.Count == 0) {
        return;
      }
      else {
        MakeTabsAndPopulate(loadSummaryStorage);
        //this.initialized = true;
      }
    }

    public List<Dictionary<string, object>> retrieve_saved_load_summary_data() {
      List<Dictionary<string, object>> allLoadSummaryData = new List<Dictionary<string, object>>();

      string acDocPath = Path.GetDirectoryName(this.acDoc.Name);
      string savesDirectory = Path.Combine(acDocPath, "Saves");
      string loadSummarySavesDirectory = Path.Combine(savesDirectory, "LoadSummary");

      // Check if the "Saves/LoadSummary" directory exists
      if (Directory.Exists(loadSummarySavesDirectory)) {
        // Get all JSON files in the directory
        string[] jsonFiles = Directory.GetFiles(loadSummarySavesDirectory, "*.json");

        // If there are any JSON files, find the most recent one
        if (jsonFiles.Length > 0) {
          string mostRecentJsonFile = jsonFiles
              .OrderByDescending(f => File.GetLastWriteTime(f))
              .First();

          // Read the JSON data from the file
          string jsonData = File.ReadAllText(mostRecentJsonFile);

          // Deserialize the JSON data to a list of dictionaries
          allLoadSummaryData = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonData);
        }
      }

      return allLoadSummaryData;
    }
    private void MakeTabsAndPopulate(List<Dictionary<string, object>> loadSummaryStorage) {
      set_up_field_values_from_load_summary_data(loadSummaryStorage);

      var sortedLoadSummariess = loadSummaryStorage.OrderBy(loadSummary => loadSummary["load_summary"].ToString()).ToList();

      foreach (Dictionary<string, object> loadSummary in sortedLoadSummariess) {
        string loadSummaryName = loadSummary["load_summary"].ToString();
        LoadSummaryForm userControl = (LoadSummaryForm)findUserControl(loadSummaryName);
        if (userControl == null) {
          continue;
        }
        //userControl.AddListeners();
      }
    }

    public UserControl findUserControl(string panelName) {
      foreach (LoadSummaryForm userControl in userControls) {
        string userControlName = userControl.Name.Replace("'", "");
        userControlName = userControlName.Replace(" ", "");
        userControlName = userControlName.Replace("-", "");
        userControlName = userControlName.Replace("PANEL", "");

        panelName = panelName.Replace("'", "");
        panelName = panelName.Replace(" ", "");
        panelName = panelName.Replace("-", "");
        panelName = panelName.Replace("PANEL", "");

        if (userControlName.ToLower() == panelName.ToLower()) {
          return userControl;
        }
      }

      return null;
    }

    private void set_up_field_values_from_load_summary_data(List<Dictionary<string, object>> loadSummaryStorage) {
      var sortedLoadSummaries = loadSummaryStorage.OrderBy(loadSummary => loadSummary["load_summary"].ToString()).ToList();

      foreach (Dictionary<string, object> loadSummary in sortedLoadSummaries) {
        string loadSummaryName = loadSummary["load_summary"].ToString();
        LoadSummaryForm loadSummaryForm = create_new_load_summary_tab(loadSummaryName);
        //loadSummaryForm.clear_modal_and_remove_rows(loadSummary);
        //loadSummaryForm.populate_modal_with_panel_data(loadSummary);
        var notes = JsonConvert.DeserializeObject<List<string>>(loadSummary["notes"].ToString());
        //loadSummaryForm.update_notes_storage(notes);
      }
    }

    internal bool load_summary_name_exists(string loadSummaryName) {
      foreach (TabPage tabPage in LOAD_SUMMARY_TABS.TabPages) {
        if (tabPage.Text.Split(' ')[1].ToLower() == loadSummaryName.ToLower()) {
          return true;
        }
      }
      return false;
    }

    public LoadSummaryForm create_new_load_summary_tab(string tabName) {
      TabPage newTabPage = new TabPage(tabName);

      LOAD_SUMMARY_TABS.TabPages.Add(newTabPage);
      LOAD_SUMMARY_TABS.SelectedTab = newTabPage;
      LoadSummaryForm loadSummaryForm = new LoadSummaryForm(this.myCommandsInstance, this, this.newLoadSummaryForm, tabName);
      this.userControls.Add(loadSummaryForm);
      add_load_summary_form_to_new_tab(loadSummaryForm, newTabPage);
      return loadSummaryForm;
    }

    public void add_load_summary_form_to_new_tab(UserControl control, TabPage tabPage) {
      control.Location = new Point(0, 0);
      control.Dock = DockStyle.Fill;
      tabPage.Controls.Add(control);
    }
    public void clear_modal_and_remove_rows(Dictionary<string, object> selectedLoadSummaryData) {
      clear_current_modal_data();
      //remove_rows();

      //int numberOfRows =
      //  ((Newtonsoft.Json.Linq.JArray)selectedPanelData["description_left"])
      //    .ToObject<List<string>>()
      //    .Count / 2;
      //PANEL_GRID.Rows.Add(numberOfRows);
    }

    private void clear_current_modal_data() {
      
    }

  }
}
