﻿using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsSystem;
using Emgu.CV.ImgHash;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.Packaging.Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using static OfficeOpenXml.ExcelErrorValue;

namespace ElectricalCommands {

  public partial class PanelUserControl : UserControl {
    private PanelCommands myCommandsInstance;
    private MainForm mainForm;
    private NewPanelForm newPanelForm;
    private NoteForm notesForm;
    private List<string> notesStorage = new List<string>();
    private List<DataGridViewCell> selectedCells;
    private List<string> defaultNotes;
    private object oldValue;

    public PanelUserControl(
      PanelCommands myCommands,
      MainForm mainForm,
      NewPanelForm newPanelForm,
      string tabName,
      bool is3PH = false,
      bool isLoadingData = false
    ) {
      InitializeComponent();
      myCommandsInstance = myCommands;
      this.mainForm = mainForm;
      this.newPanelForm = newPanelForm;
      this.Name = tabName;
      this.notesStorage = new List<string>();

      INFO_LABEL.Text = "";

      listen_for_new_rows();
      add_or_remove_panel_grid_columns(is3PH);
      remove_column_header_sorting();

      change_size_of_phase_columns(is3PH);
      add_phase_sum_column(is3PH);

      PANEL_NAME_INPUT.TextChanged += new EventHandler(this.PANEL_NAME_INPUT_TextChanged);
      PANEL_GRID.CellValueChanged += new DataGridViewCellEventHandler(this.PANEL_GRID_CellValueChangedLink);
      PANEL_GRID.Rows.AddCopies(0, 21);
      PANEL_GRID.AllowUserToAddRows = false;

      add_rows_to_datagrid();
      set_default_form_values(tabName);
      deselect_cells();
    }

    private void TogglePrefixInSelectedCells(string prefix) {
      bool allCellsEmptyOrWithPrefix = true;
      List<DataGridViewCell> cellsToUpdate = new List<DataGridViewCell>();

      // First pass: Check if all selected cells are empty or have the prefix
      foreach (DataGridViewCell cell in PANEL_GRID.SelectedCells) {
        if (cell.Value != null && PANEL_GRID.Columns[cell.ColumnIndex].Name.ToLower().Contains("description")) {
          string cellValue = cell.Value.ToString();
          bool hasPrefixAtStart = cellValue.StartsWith(prefix);
          bool hasPrefixAfterSemicolon = cellValue.Contains(";" + prefix);

          if (string.IsNullOrEmpty(cellValue) || hasPrefixAtStart || (cellValue.Contains(";") && hasPrefixAfterSemicolon)) {
            // Cell is empty or has the prefix, do nothing
          }
          else {
            allCellsEmptyOrWithPrefix = false;
          }
          cellsToUpdate.Add(cell);
        }
      }

      // Second pass: Update cell values based on the check
      foreach (DataGridViewCell cell in cellsToUpdate) {
        if (cell.Value != null) {
          string cellValue = cell.Value.ToString();
          if (allCellsEmptyOrWithPrefix) {
            // Remove the prefix at the beginning
            if (cellValue.StartsWith(prefix)) {
              cellValue = cellValue.Substring(prefix.Length);
            }
            // Remove the prefix after the semicolon if it exists
            if (cellValue.Contains(";" + prefix)) {
              cellValue = cellValue.Replace(";" + prefix, ";");
            }
            cell.Value = cellValue;
          }
          else {
            // Add the prefix at the beginning if not present and cell is not empty
            if (!cellValue.StartsWith(prefix) && !string.IsNullOrEmpty(cellValue)) {
              cellValue = prefix + cellValue;
            }
            // Add the prefix after the semicolon if it exists and not already present
            if (cellValue.Contains(";") && !cellValue.Contains(";" + prefix) && !string.IsNullOrEmpty(cellValue)) {
              int semicolonIndex = cellValue.IndexOf(";");
              cellValue = cellValue.Insert(semicolonIndex + 1, prefix);
            }
            cell.Value = cellValue;
          }
        }
      }
    }

    private void EXISTING_BUTTON_Click(object sender, EventArgs e) {
      TogglePrefixInSelectedCells("(E)");
    }

    private void RELOCATE_BUTTON_Click(object sender, EventArgs e) {
      TogglePrefixInSelectedCells("(R)");
    }

    public List<string> getNotesStorage() {
      return this.notesStorage;
    }

    private void add_rows_to_datagrid() {
      PHASE_SUM_GRID.Rows.Add("0", "0");
      TOTAL_VA_GRID.Rows.Add("0");
      PANEL_LOAD_GRID.Rows.Add("0");
      FEEDER_AMP_GRID.Rows.Add("0");
    }

    private void deselect_cells() {
      PHASE_SUM_GRID.DefaultCellStyle.SelectionBackColor = PHASE_SUM_GRID
        .DefaultCellStyle
        .BackColor;
      PHASE_SUM_GRID.DefaultCellStyle.SelectionForeColor = PHASE_SUM_GRID
        .DefaultCellStyle
        .ForeColor;
      TOTAL_VA_GRID.DefaultCellStyle.SelectionBackColor = TOTAL_VA_GRID.DefaultCellStyle.BackColor;
      TOTAL_VA_GRID.DefaultCellStyle.SelectionForeColor = TOTAL_VA_GRID.DefaultCellStyle.ForeColor;
      PANEL_LOAD_GRID.DefaultCellStyle.SelectionBackColor = PANEL_LOAD_GRID
        .DefaultCellStyle
        .BackColor;
      PANEL_LOAD_GRID.DefaultCellStyle.SelectionForeColor = PANEL_LOAD_GRID
        .DefaultCellStyle
        .ForeColor;
      FEEDER_AMP_GRID.DefaultCellStyle.SelectionBackColor = FEEDER_AMP_GRID
        .DefaultCellStyle
        .BackColor;
      FEEDER_AMP_GRID.DefaultCellStyle.SelectionForeColor = FEEDER_AMP_GRID
        .DefaultCellStyle
        .ForeColor;
      PANEL_GRID.ClearSelection();
    }

    private void set_default_form_values(string tabName) {
      // Textboxes
      PANEL_NAME_INPUT.Text = tabName;
      PANEL_LOCATION_INPUT.Text = "ELECTRIC ROOM";
      MAIN_INPUT.Text = "M.L.O.";
      BUS_RATING_INPUT.Text = "100";

      // Comboboxes
      STATUS_COMBOBOX.SelectedIndex = 0;
      MOUNTING_COMBOBOX.SelectedIndex = 0;
      if (PHASE_SUM_GRID.ColumnCount > 2) {
        WIRE_COMBOBOX.SelectedIndex = 1;
        PHASE_COMBOBOX.SelectedIndex = 1;
      }
      else {
        WIRE_COMBOBOX.SelectedIndex = 0;
        PHASE_COMBOBOX.SelectedIndex = 0;
      }
      PHASE_VOLTAGE_COMBOBOX.SelectedIndex = 0;
      LINE_VOLTAGE_COMBOBOX.SelectedIndex = 0;

      // Datagrids
      PHASE_SUM_GRID.Rows[0].Cells[0].Value = "0";
      PHASE_SUM_GRID.Rows[0].Cells[1].Value = "0";
      TOTAL_VA_GRID.Rows[0].Cells[0].Value = "0";
      PANEL_LOAD_GRID.Rows[0].Cells[0].Value = "0";
      FEEDER_AMP_GRID.Rows[0].Cells[0].Value = "0";

      if (PHASE_SUM_GRID.ColumnCount > 2)
        PHASE_SUM_GRID.Rows[0].Cells[2].Value = "0";
    }

    private void remove_column_header_sorting() {
      foreach (DataGridViewColumn column in PANEL_GRID.Columns) {
        column.SortMode = DataGridViewColumnSortMode.NotSortable;
      }
    }

    private void listen_for_new_rows() {
      PANEL_GRID.RowsAdded += new DataGridViewRowsAddedEventHandler(PANEL_GRID_RowsAdded);
    }

    public static double SafeConvertToDouble(string value) {
      if (double.TryParse(value, out double result)) {
        return result;
      }
      return 0;
    }

    public Dictionary<string, object> retrieve_data_from_modal() {
      // Create a new panel
      Dictionary<string, object> panel = new Dictionary<string, object>();

      // Get the value from the main input
      string mainInput = MAIN_INPUT.Text.ToLower();

      if (
        !mainInput.Contains("mlo")
        && !mainInput.Contains("m.l.o")
        && !mainInput.Contains("m.l.o.")
      ) {
        if (mainInput.Contains("amp")) {
          mainInput = mainInput.Replace("amp", "AMP");
        }
        else if (mainInput.Contains("a")) {
          mainInput = mainInput.Replace("a", "AMP");
        }
        else if (mainInput.Contains(" ")) {
          mainInput = mainInput.Replace(" ", " AMP");
        }
        else {
          mainInput += " AMP";
        }
      }

      panel.Add("main", mainInput.ToUpper());

      string GetComboBoxValue(ComboBox comboBox) {
        if (comboBox.SelectedItem != null) {
          return comboBox.SelectedItem.ToString().ToUpper();
        }
        else if (!string.IsNullOrEmpty(comboBox.Text)) {
          return comboBox.Text.ToUpper();
        }
        else {
          return "";
        }
      }

      panel.Add("panel", "'" + PANEL_NAME_INPUT.Text.ToUpper() + "'");
      panel.Add("location", PANEL_LOCATION_INPUT.Text.ToUpper());
      panel.Add("voltage1", GetComboBoxValue(LINE_VOLTAGE_COMBOBOX));
      panel.Add("voltage2", GetComboBoxValue(PHASE_VOLTAGE_COMBOBOX));
      panel.Add("phase", GetComboBoxValue(PHASE_COMBOBOX));
      panel.Add("wire", GetComboBoxValue(WIRE_COMBOBOX));
      panel.Add("mounting", GetComboBoxValue(MOUNTING_COMBOBOX));
      panel.Add("existing", GetComboBoxValue(STATUS_COMBOBOX));
      panel.Add("lcl_override", LCL_OVERRIDE.Checked);
      panel.Add("lml_override", LML_OVERRIDE.Checked);

      panel.Add(
        "subtotal_a",
        Math.Round(Convert.ToDouble(PHASE_SUM_GRID.Rows[0].Cells[0].Value.ToString().ToUpper()))
          .ToString()
      );
      panel.Add(
        "subtotal_b",
        Math.Round(Convert.ToDouble(PHASE_SUM_GRID.Rows[0].Cells[1].Value.ToString().ToUpper()))
          .ToString()
      );

      if (PHASE_SUM_GRID.Columns.Count > 2) {
        panel.Add(
          "subtotal_c",
          Math.Round(Convert.ToDouble(PHASE_SUM_GRID.Rows[0].Cells[2].Value.ToString().ToUpper()))
            .ToString()
        );
      }
      else {
        panel.Add("subtotal_c", "0");
      }
      panel.Add("total_va", TOTAL_VA_GRID.Rows[0].Cells[0].Value.ToString().ToUpper());
      double lcl = SafeConvertToDouble(LCL.Text);
      double lcl125 = Math.Round(lcl * 1.25, 0);
      panel.Add("lcl", lcl);
      panel.Add("lcl125", lcl125);
      double lml = SafeConvertToDouble(LML.Text);
      double lml125 = Math.Round(lml * 1.25, 0);
      panel.Add("lml", lml);
      panel.Add("lml125", lml125);
      panel.Add("kva", PANEL_LOAD_GRID.Rows[0].Cells[0].Value.ToString().ToUpper());
      panel.Add("feeder_amps", FEEDER_AMP_GRID.Rows[0].Cells[0].Value.ToString().ToUpper());
      panel.Add("custom_title", CUSTOM_TITLE_TEXT.Text.ToUpper());

      string busRatingInput = BUS_RATING_INPUT.Text.ToLower();

      if (busRatingInput.Contains("amp")) {
        busRatingInput = busRatingInput.Replace("amp", "A");
      }
      else if (busRatingInput.Contains("a")) {
        busRatingInput = busRatingInput.Replace("a", "A");
      }
      else if (busRatingInput.Contains(" ")) {
        busRatingInput = busRatingInput.Replace(" ", " A");
      }
      else {
        busRatingInput += "A";
      }

      panel.Add("bus_rating", busRatingInput.ToUpper());

      List<bool> description_left_highlights = new List<bool>();
      List<bool> description_right_highlights = new List<bool>();
      List<bool> breaker_left_highlights = new List<bool>();
      List<bool> breaker_right_highlights = new List<bool>();

      List<string> description_left = new List<string>();
      List<string> description_right = new List<string>();
      List<string> phase_a_left = new List<string>();
      List<string> phase_b_left = new List<string>();
      List<string> phase_a_right = new List<string>();
      List<string> phase_b_right = new List<string>();
      List<string> phase_c_left = new List<string>();
      List<string> phase_c_right = new List<string>();
      List<string> breaker_left = new List<string>();
      List<string> breaker_right = new List<string>();
      List<string> circuit_left = new List<string>();
      List<string> circuit_right = new List<string>();
      List<string> phase_a_left_tag = new List<string>();
      List<string> phase_b_left_tag = new List<string>();
      List<string> phase_a_right_tag = new List<string>();
      List<string> phase_b_right_tag = new List<string>();
      List<string> phase_c_left_tag = new List<string>();
      List<string> phase_c_right_tag = new List<string>();

      List<string> description_left_tags = new List<string>();
      List<string> description_right_tags = new List<string>();

      for (int i = 0; i < PANEL_GRID.Rows.Count; i++) {
        string descriptionLeftValue = "";
        if (
          string.IsNullOrEmpty(PANEL_GRID.Rows[i].Cells["description_left"].Value as string)
          && !string.IsNullOrEmpty(PANEL_GRID.Rows[i].Cells["breaker_left"].Value as string)
        ) {
          // check that the breaker value is both an integer and greater than 3
          var breakerValue = PANEL_GRID.Rows[i].Cells["breaker_left"].Value.ToString();
          int breakerValueInt;
          if (int.TryParse(breakerValue, out breakerValueInt) && breakerValueInt > 3) {
            descriptionLeftValue = "SPARE";
          }
        }
        else {
          descriptionLeftValue = string.IsNullOrWhiteSpace(
            PANEL_GRID.Rows[i].Cells["description_left"].Value as string
          )
            ? "SPACE"
            : PANEL_GRID
              .Rows[i]
              .Cells["description_left"]
              .Value.ToString()
              .ToUpper()
              .Replace("\r", "");
        }
        string descriptionRightValue = "";
        if (
          string.IsNullOrEmpty(PANEL_GRID.Rows[i].Cells["description_right"].Value as string)
          && !string.IsNullOrEmpty(PANEL_GRID.Rows[i].Cells["breaker_right"].Value as string)
        ) {
          // check that the breaker value is both an integer and greater than 3
          var breakerValue = PANEL_GRID.Rows[i].Cells["breaker_right"].Value.ToString();
          int breakerValueInt;
          if (int.TryParse(breakerValue, out breakerValueInt) && breakerValueInt > 3) {
            descriptionRightValue = "SPARE";
          }
        }
        else {
          descriptionRightValue = string.IsNullOrWhiteSpace(
            PANEL_GRID.Rows[i].Cells["description_right"].Value as string
          )
            ? "SPACE"
            : PANEL_GRID
              .Rows[i]
              .Cells["description_right"]
              .Value.ToString()
              .ToUpper()
              .Replace("\r", "");
        }

        string breakerLeftValue =
          PANEL_GRID.Rows[i].Cells["breaker_left"].Value?.ToString().ToUpper().Replace("\r", "")
          ?? "";
        string breakerRightValue =
          PANEL_GRID.Rows[i].Cells["breaker_right"].Value?.ToString().ToUpper().Replace("\r", "")
          ?? "";
        string circuitRightValue =
          PANEL_GRID.Rows[i].Cells["circuit_right"].Value?.ToString().ToUpper().Replace("\r", "")
          ?? "";
        string circuitLeftValue =
          PANEL_GRID.Rows[i].Cells["circuit_left"].Value?.ToString().ToUpper().Replace("\r", "")
          ?? "";
        string phaseALeftTag = PANEL_GRID.Rows[i].Cells["phase_a_left"].Tag?.ToString() ?? "";
        string phaseBLeftTag = PANEL_GRID.Rows[i].Cells["phase_b_left"].Tag?.ToString() ?? "";
        string phaseARightTag = PANEL_GRID.Rows[i].Cells["phase_a_right"].Tag?.ToString() ?? "";
        string phaseBRightTag = PANEL_GRID.Rows[i].Cells["phase_b_right"].Tag?.ToString() ?? "";

        string phaseALeftValue = (
          PANEL_GRID
            .Rows[i]
            .Cells["phase_a_left"]
            .Value?.ToString()
            .Replace("\r", "")
            .Replace(" ", "") ?? "0"
        );
        phaseALeftValue =
          phaseALeftValue.Contains(";") || !Regex.IsMatch(phaseALeftValue, @"^\d+$")
            ? phaseALeftValue
            : Math.Round(Convert.ToDouble(phaseALeftValue)).ToString();

        string phaseBLeftValue = (
          PANEL_GRID
            .Rows[i]
            .Cells["phase_b_left"]
            .Value?.ToString()
            .Replace("\r", "")
            .Replace(" ", "") ?? "0"
        );
        phaseBLeftValue =
          phaseBLeftValue.Contains(";") || !Regex.IsMatch(phaseBLeftValue, @"^\d+$")
            ? phaseBLeftValue
            : Math.Round(Convert.ToDouble(phaseBLeftValue)).ToString();

        string phaseARightValue = (
          PANEL_GRID
            .Rows[i]
            .Cells["phase_a_right"]
            .Value?.ToString()
            .Replace("\r", "")
            .Replace(" ", "") ?? "0"
        );
        phaseARightValue =
          phaseARightValue.Contains(";") || !Regex.IsMatch(phaseARightValue, @"^\d+$")
            ? phaseARightValue
            : Math.Round(Convert.ToDouble(phaseARightValue)).ToString();

        string phaseBRightValue = (
          PANEL_GRID
            .Rows[i]
            .Cells["phase_b_right"]
            .Value?.ToString()
            .Replace("\r", "")
            .Replace(" ", "") ?? "0"
        );
        phaseBRightValue =
          phaseBRightValue.Contains(";") || !Regex.IsMatch(phaseBRightValue, @"^\d+$")
            ? phaseBRightValue
            : Math.Round(Convert.ToDouble(phaseBRightValue)).ToString();

        string phaseCLeftValue = "0";
        string phaseCRightValue = "0";
        string phaseCLeftTag = "";
        string phaseCRightTag = "";

        string descriptionLeftTag =
          PANEL_GRID.Rows[i].Cells["description_left"].Tag?.ToString() ?? "";
        string descriptionRightTag =
          PANEL_GRID.Rows[i].Cells["description_right"].Tag?.ToString() ?? "";

        if (PHASE_SUM_GRID.Columns.Count > 2) {
          phaseCLeftTag = PANEL_GRID.Rows[i].Cells["phase_c_left"].Tag?.ToString() ?? "";
          phaseCRightTag = PANEL_GRID.Rows[i].Cells["phase_c_right"].Tag?.ToString() ?? "";
          phaseCLeftValue = (
            PANEL_GRID
              .Rows[i]
              .Cells["phase_c_left"]
              .Value?.ToString()
              .Replace("\r", "")
              .Replace(" ", "") ?? "0"
          );
          phaseCLeftValue =
            phaseCLeftValue.Contains(";") || !Regex.IsMatch(phaseCLeftValue, @"^\d+$")
              ? phaseCLeftValue
              : Math.Round(Convert.ToDouble(phaseCLeftValue)).ToString();
          phaseCRightValue = (
            PANEL_GRID
              .Rows[i]
              .Cells["phase_c_right"]
              .Value?.ToString()
              .Replace("\r", "")
              .Replace(" ", "") ?? "0"
          );
          phaseCRightValue =
            phaseCRightValue.Contains(";") || !Regex.IsMatch(phaseCRightValue, @"^\d+$")
              ? phaseCRightValue
              : Math.Round(Convert.ToDouble(phaseCRightValue)).ToString();
        }

        phase_a_left_tag.Add(phaseALeftTag);
        phase_b_left_tag.Add(phaseBLeftTag);
        phase_a_right_tag.Add(phaseARightTag);
        phase_b_right_tag.Add(phaseBRightTag);
        phase_c_left_tag.Add(phaseCLeftTag);
        phase_c_right_tag.Add(phaseCRightTag);

        description_left_tags.Add(descriptionLeftTag);
        description_right_tags.Add(descriptionRightTag);

        // Checks for Left Side
        bool hasCommaInPhaseLeft =
          phaseALeftValue.Contains(";")
          || phaseBLeftValue.Contains(";")
          || phaseCLeftValue.Contains(";");
        bool shouldDuplicateLeft = hasCommaInPhaseLeft;

        // Checks for Right Side
        bool hasCommaInPhaseRight =
          phaseARightValue.Contains(";")
          || phaseBRightValue.Contains(";")
          || phaseCRightValue.Contains(";");
        bool shouldDuplicateRight = hasCommaInPhaseRight;

        // Handling Phase A Left
        if (phaseALeftValue.Contains(";")) {
          var splitValues = phaseALeftValue.Split(';').Select(str => str.Trim()).ToArray();
          phase_a_left.AddRange(splitValues);
        }
        else {
          phase_a_left.Add(phaseALeftValue);
          phase_a_left.Add("0"); // Default value
        }

        // Handling Phase B Left
        if (phaseBLeftValue.Contains(";")) {
          var splitValues = phaseBLeftValue.Split(';').Select(str => str.Trim()).ToArray();
          phase_b_left.AddRange(splitValues);
        }
        else {
          phase_b_left.Add(phaseBLeftValue);
          phase_b_left.Add("0"); // Default value
        }

        // Handling Phase A Right
        if (phaseARightValue.Contains(";")) {
          var splitValues = phaseARightValue.Split(';').Select(str => str.Trim()).ToArray();
          phase_a_right.AddRange(splitValues);
        }
        else {
          phase_a_right.Add(phaseARightValue);
          phase_a_right.Add("0"); // Default value
        }

        // Handling Phase B Right
        if (phaseBRightValue.Contains(";")) {
          var splitValues = phaseBRightValue.Split(';').Select(str => str.Trim()).ToArray();
          phase_b_right.AddRange(splitValues);
        }
        else {
          phase_b_right.Add(phaseBRightValue);
          phase_b_right.Add("0"); // Default value
        }

        if (PHASE_SUM_GRID.Columns.Count > 2) {
          // Handling Phase C Left
          if (phaseCLeftValue.Contains(";")) {
            var splitValues = phaseCLeftValue.Split(';').Select(str => str.Trim()).ToArray();
            phase_c_left.AddRange(splitValues);
          }
          else {
            phase_c_left.Add(phaseCLeftValue);
            phase_c_left.Add("0"); // Default value
          }

          // Handling Phase C Right
          if (phaseCRightValue.Contains(";")) {
            var splitValues = phaseCRightValue.Split(';').Select(str => str.Trim()).ToArray();
            phase_c_right.AddRange(splitValues);
          }
          else {
            phase_c_right.Add(phaseCRightValue);
            phase_c_right.Add("0"); // Default value
          }
        }

        if (descriptionLeftValue.Contains(";")) {
          // If it contains a comma, split and add both values
          var splitValues = descriptionLeftValue.Split(';').Select(str => str.Trim()).ToArray();
          description_left.AddRange(splitValues);
          circuit_left.Add(circuitLeftValue + "A");
          circuit_left.Add(circuitLeftValue + "B");
        }
        else {
          description_left.Add(descriptionLeftValue);
          description_left.Add(shouldDuplicateLeft ? descriptionLeftValue : "SPACE");

          if (shouldDuplicateLeft) {
            circuit_left.Add(circuitLeftValue + "A");
            circuit_left.Add(circuitLeftValue + "B");
          }
          else {
            circuit_left.Add(circuitLeftValue);
            circuit_left.Add("");
          }
        }

        if (breakerLeftValue.Contains(";")) {
          // If it contains a comma, split and add both values
          var splitValues = breakerLeftValue.Split(';').Select(str => str.Trim()).ToArray();
          breaker_left.AddRange(splitValues);
        }
        else {
          breaker_left.Add(breakerLeftValue);
          breaker_left.Add(shouldDuplicateLeft ? breakerLeftValue : "");
        }

        if (descriptionRightValue.Contains(";")) {
          // If it contains a comma, split and add both values
          var splitValues = descriptionRightValue.Split(';').Select(str => str.Trim()).ToArray();
          description_right.AddRange(splitValues);
          circuit_right.Add(circuitRightValue + "A");
          circuit_right.Add(circuitRightValue + "B");
        }
        else {
          description_right.Add(descriptionRightValue);
          description_right.Add(shouldDuplicateRight ? descriptionRightValue : "SPACE");

          if (shouldDuplicateRight) {
            circuit_right.Add(circuitRightValue + "A");
            circuit_right.Add(circuitRightValue + "B");
          }
          else {
            circuit_right.Add(circuitRightValue);
            circuit_right.Add("");
          }
        }

        if (breakerRightValue.Contains(";")) {
          // If it contains a comma, split and add both values
          var splitValues = breakerRightValue.Split(';').Select(str => str.Trim()).ToArray();
          breaker_right.AddRange(splitValues);
        }
        else {
          breaker_right.Add(breakerRightValue);
          breaker_right.Add(shouldDuplicateRight ? breakerRightValue : "");
        }

        // Left Side
        description_left_highlights.Add(false);
        breaker_left_highlights.Add(false);

        // Right Side
        description_right_highlights.Add(false);
        breaker_right_highlights.Add(false);

        // Default Values for Left Side
        description_left_highlights.Add(false);
        breaker_left_highlights.Add(false);

        // Default Values for Right Side
        description_right_highlights.Add(false);
        breaker_right_highlights.Add(false);
      }

      panel.Add("description_left_highlights", description_left_highlights);
      panel.Add("description_right_highlights", description_right_highlights);
      panel.Add("breaker_left_highlights", breaker_left_highlights);
      panel.Add("breaker_right_highlights", breaker_right_highlights);
      panel.Add("description_left", description_left);
      panel.Add("description_right", description_right);
      panel.Add("phase_a_left", phase_a_left);
      panel.Add("phase_b_left", phase_b_left);
      panel.Add("phase_a_right", phase_a_right);
      panel.Add("phase_b_right", phase_b_right);

      if (PHASE_SUM_GRID.Columns.Count > 2) {
        panel.Add("phase_c_left", phase_c_left);
        panel.Add("phase_c_right", phase_c_right);
      }

      panel.Add("breaker_left", breaker_left);
      panel.Add("breaker_right", breaker_right);
      panel.Add("circuit_left", circuit_left);
      panel.Add("circuit_right", circuit_right);
      panel.Add("phase_a_left_tag", phase_a_left_tag);
      panel.Add("phase_b_left_tag", phase_b_left_tag);
      panel.Add("phase_a_right_tag", phase_a_right_tag);
      panel.Add("phase_b_right_tag", phase_b_right_tag);

      if (PHASE_SUM_GRID.Columns.Count > 2) {
        panel.Add("phase_c_left_tag", phase_c_left_tag);
        panel.Add("phase_c_right_tag", phase_c_right_tag);
      }

      panel.Add("description_left_tags", description_left_tags);
      panel.Add("description_right_tags", description_right_tags);

      panel.Add("notes", notesStorage);

      return panel;
    }

    public double CalculateTotalVA(double sum) {
      return Math.Round(sum, 0);
    }

    public double CalculatePanelLoad(double sum) {
      return Math.Round(sum / 1000, 1);
    }

    public double GetPanelLoad() {
      if (PANEL_LOAD_GRID != null && PANEL_LOAD_GRID.Rows.Count > 0 && PANEL_LOAD_GRID.Columns.Count > 0) {
        object cellValue = PANEL_LOAD_GRID.Rows[0].Cells[0].Value;
        if (cellValue != null && double.TryParse(cellValue.ToString(), out double totalKVA)) {
          return totalKVA;
        }
      }
      return 0.0;
    }

    public List<string> GetSubPanels() {
      List<string> subPanels = new List<string>();
      string pattern = @"(PANEL|SUBPANEL)\s+(?:'?([^']+)'?)";
      Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

      foreach (DataGridViewRow row in PANEL_GRID.Rows) {
        foreach (DataGridViewCell cell in row.Cells) {
          string cellValue = cell.Value?.ToString() ?? "";
          MatchCollection matches = regex.Matches(cellValue);

          foreach (Match match in matches) {
            if (match.Groups.Count > 2) {
              string panelName = match.Groups[2].Value;
              if (!subPanels.Contains(panelName)) {
                subPanels.Add(panelName.ToUpper());
              }
            }
          }
        }
      }

      return subPanels;
    }

    public string GetPanelName() {
      return PANEL_NAME_INPUT.Text;
    }

    public double StoreItemsAndWattage(string note) {
      int phaseCount = PHASE_SUM_GRID.ColumnCount;
      string[] columnNames = GetColumnNames(phaseCount);

      List<PanelItem> items = new List<PanelItem>();

      // Process left side
      ProcessSide(columnNames, true, items, note);

      // Process right side
      ProcessSide(columnNames, false, items, note);

      return items.Count > 0
            ? items.Max(item => item.Wattage)
            : 0;
    }

    private void ProcessSide(string[] columnNames, bool isLeftSide, List<PanelItem> items, string note) {
      int startIndex = isLeftSide ? 0 : 1;
      for (int rowIndex = 0; rowIndex < PANEL_GRID.Rows.Count; rowIndex++) {
        DataGridViewRow row = PANEL_GRID.Rows[rowIndex];
        for (int i = startIndex; i < columnNames.Length; i += 2) {
          string colName = columnNames[i];
          bool descriptionHasNote = DescriptionHasNote(row, isLeftSide, note);
          int breakerValue = BreakerToInt(row, isLeftSide);

          if (row.Cells[colName].Value != null && descriptionHasNote && breakerValue > 3) {
            double phaseValue;
            if (!TryParseDouble(row.Cells[colName].Value, out phaseValue)) {
              continue;
            }
            string description = GetDescription(row, isLeftSide);
            PanelItem item = new PanelItem { Description = description };

            // Check for condition 1
            if (rowIndex + 2 < PANEL_GRID.Rows.Count) {
              DataGridViewRow secondRow = PANEL_GRID.Rows[rowIndex + 1];
              DataGridViewRow thirdRow = PANEL_GRID.Rows[rowIndex + 2];
              int secondBreakerValue = BreakerToInt(secondRow, isLeftSide);
              string thirdBreakerValue = thirdRow.Cells[isLeftSide ? "breaker_left" : "breaker_right"].Value?.ToString();

              if (secondBreakerValue == 0 && thirdBreakerValue == "3") {
                item.Wattage = phaseValue * 3;
                item.Poles = 3;
                rowIndex += 2;
                items.Add(item);
                continue;
              }
            }

            // Check for condition 2
            if (rowIndex + 1 < PANEL_GRID.Rows.Count) {
              DataGridViewRow secondRow = PANEL_GRID.Rows[rowIndex + 1];
              string secondBreakerValue = secondRow.Cells[isLeftSide ? "breaker_left" : "breaker_right"].Value?.ToString();

              if (secondBreakerValue == "2") {
                item.Wattage = phaseValue * 2;
                item.Poles = 2;
                rowIndex += 1; // Skip 1 row
                items.Add(item);
                continue;
              }
            }

            // Condition 3 (default case)
            item.Wattage = phaseValue;
            item.Poles = 1;
            items.Add(item);
          }
        }
      }
    }

    private bool TryParseDouble(object value, out double result) {
      result = 0;
      if (value == null) return false;

      string stringValue = value.ToString().Trim();
      if (string.IsNullOrEmpty(stringValue)) return false;

      // Try parsing with invariant culture (uses period as decimal separator)
      if (double.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
        return true;

      // If that fails, try parsing with the current culture
      return double.TryParse(stringValue, NumberStyles.Any, CultureInfo.CurrentCulture, out result);
    }

    private bool DescriptionHasNote(DataGridViewRow row, bool isLeftSide, string note) {
      string columnName = isLeftSide ? "description_left" : "description_right";
      DataGridViewCell cell = row.Cells[columnName];

      if (cell.Tag == null) {
        return false;
      }

      string tagValue = cell.Tag.ToString();
      return tagValue.Contains(note);
    }

    private int BreakerToInt(DataGridViewRow row, bool isLeftSide) {
      string columnName = isLeftSide ? "breaker_left" : "breaker_right";
      var cellValue = row.Cells[columnName].Value;

      if (cellValue == null || string.IsNullOrEmpty(cellValue.ToString())) {
        return 0;
      }

      string breakerValue = cellValue.ToString();

      if (breakerValue.Contains(",")) {
        breakerValue = breakerValue.Split(',')[0];
      }

      if (int.TryParse(breakerValue, out int result)) {
        return result;
      }

      return 0;
    }

    private string GetDescription(DataGridViewRow row, bool isLeftSide) {
      string columnPrefix = isLeftSide ? "left" : "right";
      return row.Cells[$"description_{columnPrefix}"].Value?.ToString() ?? string.Empty;
    }

    public double CalculateWattageSum(string note) {
      int phaseCount = PHASE_SUM_GRID.ColumnCount;
      if (phaseCount < 2 || phaseCount > 3) {
        throw new ArgumentException("Unsupported phase count. Must be 2 or 3.");
      }
      return CalculateWattageSumForPhases(phaseCount, note);
    }

    private double CalculateWattageSumForPhases(int phaseCount, string note) {
      string[] columnNames = GetColumnNames(phaseCount);
      double[] sums = new double[phaseCount];

      foreach (DataGridViewRow row in PANEL_GRID.Rows) {
        for (int i = 0; i < columnNames.Length; i += 2) {
          for (int j = 0; j < 2; j++) {
            string colName = columnNames[i + j];
            if (row.Cells[colName].Value != null) {
              bool hasNoteApplied = BreakerContainsNote(row.Index, colName, note);
              if (hasNoteApplied) {
                sums[i / 2] += ParseAndSumCell(row.Cells[colName].Value.ToString(), 1);
              }
            }
          }
        }
      }

      return sums.Sum();
    }

    public void CalculateBreakerLoad() {
      int phaseCount = PHASE_SUM_GRID.ColumnCount;
      if (phaseCount < 2 || phaseCount > 3) {
        throw new ArgumentException("Unsupported phase count. Must be 2 or 3.");
      }

      CalculateBreakerLoadForPhases(phaseCount);
    }

    private void CalculateBreakerLoadForPhases(int phaseCount) {
      string[] columnNames = GetColumnNames(phaseCount);
      int breakersWithKitchenDemand = BreakersWithNote("KITCHEN DEMAND");
      double demandFactor = KitchenDemandFactor(breakersWithKitchenDemand);
      double[] sums = new double[phaseCount];

      foreach (DataGridViewRow row in PANEL_GRID.Rows) {
        for (int i = 0; i < columnNames.Length; i += 2) {
          for (int j = 0; j < 2; j++) {
            string colName = columnNames[i + j];
            if (row.Cells[colName].Value != null) {
              bool hasKitchenDemandApplied = BreakerContainsNote(row.Index, colName, "KITCHEN DEMAND");
              sums[i / 2] += ParseAndSumCell(
                  row.Cells[colName].Value.ToString(),
                  hasKitchenDemandApplied ? demandFactor : 1.0
              );
            }
          }
        }
      }

      for (int i = 0; i < phaseCount; i++) {
        PHASE_SUM_GRID.Rows[0].Cells[i].Value = sums[i];
      }
    }

    private string[] GetColumnNames(int phaseCount) {
      if (phaseCount == 2) {
        return new[] { "phase_a_left", "phase_a_right", "phase_b_left", "phase_b_right" };
      }
      else {
        return new[] { "phase_a_left", "phase_a_right", "phase_b_left", "phase_b_right", "phase_c_left", "phase_c_right" };
      }
    }

    private int BreakersWithNote(string note) {
      return PANEL_GRID.Rows.Cast<DataGridViewRow>()
          .Sum(row => new[] { "description_left", "description_right" }
              .Count(colName => CellHasNote(colName, row, note)));
    }

    private bool CellHasNote(string columnName, DataGridViewRow row, string note) {
      if (row == null || !PANEL_GRID.Columns.Contains(columnName))
        return false;

      var cell = row.Cells[columnName];
      if (cell == null)
        return false;

      string cellValueString = cell.Value?.ToString() ?? "";
      string cellTagString = cell.Tag?.ToString() ?? "";

      return !string.IsNullOrEmpty(cellValueString) && cellTagString.Contains(note);
    }

    private double ParseAndSumCell(
      string cellValue,
      double demandFactor
    ) {
      double sum = 0;
      if (!string.IsNullOrEmpty(cellValue)) {
        var parts = cellValue.Split(';');
        foreach (var part in parts) {
          if (double.TryParse(part, out double value)) {
            if (demandFactor != 1.00) {
              sum += value * demandFactor;
            }
            else {
              sum += value;
            }
          }
        }
      }
      return Math.Ceiling(sum);
    }

    private double KitchenDemandFactor(int numberOfBreakersWithKitchenDemand) {
      if (numberOfBreakersWithKitchenDemand == 1 || numberOfBreakersWithKitchenDemand == 2) {
        return 1.00;
      }
      else if (numberOfBreakersWithKitchenDemand == 3) {
        return 0.90;
      }
      else if (numberOfBreakersWithKitchenDemand == 4) {
        return 0.80;
      }
      else if (numberOfBreakersWithKitchenDemand == 5) {
        return 0.70;
      }
      else if (numberOfBreakersWithKitchenDemand >= 6) {
        return 0.65;
      }
      else {
        return 1.00;
      }
    }

    private void listen_for_3P_rows_added(DataGridViewRowsAddedEventArgs e) {
      Color grayColor = Color.LightGray;

      for (int i = 0; i < e.RowCount; i++) {
        int rowIndex = e.RowIndex + i;

        // Set common column values
        PANEL_GRID.Rows[rowIndex].Cells["description_left"].Value = "SPARE";
        PANEL_GRID.Rows[rowIndex].Cells["breaker_left"].Value = "20";
        PANEL_GRID.Rows[rowIndex].Cells["circuit_left"].Value = ((rowIndex + 1) * 2) - 1;
        PANEL_GRID.Rows[rowIndex].Cells["circuit_right"].Value = (rowIndex + 1) * 2;
        PANEL_GRID.Rows[rowIndex].Cells["breaker_right"].Value = "20";
        PANEL_GRID.Rows[rowIndex].Cells["description_right"].Value = "SPARE";

        // Determine the row pattern (zig-zag) for gray background
        int pattern = rowIndex % 3;

        // Apply pattern for two sets of columns based on the row pattern
        if (pattern == 0) {
          PANEL_GRID.Rows[rowIndex].Cells["phase_a_left"].Style.BackColor = grayColor;
          PANEL_GRID.Rows[rowIndex].Cells["phase_a_right"].Style.BackColor = grayColor;
        }
        else if (pattern == 1) {
          PANEL_GRID.Rows[rowIndex].Cells["phase_b_left"].Style.BackColor = grayColor;
          PANEL_GRID.Rows[rowIndex].Cells["phase_b_right"].Style.BackColor = grayColor;
        }
        else {
          PANEL_GRID.Rows[rowIndex].Cells["phase_c_left"].Style.BackColor = grayColor;
          PANEL_GRID.Rows[rowIndex].Cells["phase_c_right"].Style.BackColor = grayColor;
        }
      }
    }

    private void listen_for_2P_rows_added(DataGridViewRowsAddedEventArgs e) {
      for (int i = 0; i < e.RowCount; i++) {
        int rowIndex = e.RowIndex + i;

        // Set common column values
        PANEL_GRID.Rows[rowIndex].Cells["description_left"].Value = "SPARE";
        PANEL_GRID.Rows[rowIndex].Cells["breaker_left"].Value = "20";
        PANEL_GRID.Rows[rowIndex].Cells["circuit_left"].Value = ((rowIndex + 1) * 2) - 1;
        PANEL_GRID.Rows[rowIndex].Cells["circuit_right"].Value = (rowIndex + 1) * 2;
        PANEL_GRID.Rows[rowIndex].Cells["breaker_right"].Value = "20";
        PANEL_GRID.Rows[rowIndex].Cells["description_right"].Value = "SPARE";

        // Zig-zag pattern for columns 2, 3, 8, and 9
        if ((rowIndex + 1) % 2 == 1) // Odd rows
        {
          PANEL_GRID.Rows[rowIndex].Cells["phase_a_left"].Style.BackColor = Color.LightGray; // Column 2
          PANEL_GRID.Rows[rowIndex].Cells["phase_a_right"].Style.BackColor = Color.LightGray; // Column 8
        }
        else // Even rows
        {
          PANEL_GRID.Rows[rowIndex].Cells["phase_b_left"].Style.BackColor = Color.LightGray; // Column 3
          PANEL_GRID.Rows[rowIndex].Cells["phase_b_right"].Style.BackColor = Color.LightGray; // Column 9
        }
      }
    }

    public void clear_modal_and_remove_rows(Dictionary<string, object> selectedPanelData) {
      clear_current_modal_data();
      remove_rows();

      int numberOfRows =
        ((Newtonsoft.Json.Linq.JArray)selectedPanelData["description_left"])
          .ToObject<List<string>>()
          .Count / 2;
      PANEL_GRID.Rows.Add(numberOfRows);
    }

    internal DataGridView retrieve_panelGrid() {
      return PANEL_GRID;
    }

    private void remove_rows() {
      // remove rows
      while (PANEL_GRID.Rows.Count >= 1) {
        PANEL_GRID.Rows.RemoveAt(0);
      }
    }

    public void populate_modal_with_panel_data(Dictionary<string, object> selectedPanelData) {
      string GetSafeString(string key) {
        return selectedPanelData.TryGetValue(key, out object value) ? value?.ToString() ?? "" : "";
      }

      bool GetSafeBoolean(string key) {
        if (selectedPanelData.TryGetValue(key, out object value)) {
          if (value is bool boolValue) {
            return boolValue;
          }
          if (value is string stringValue) {
            return bool.TryParse(stringValue, out bool result) && result;
          }
        }
        return false;
      }

      // Set TextBoxes
      MAIN_INPUT.Text = GetSafeString("main").Replace("AMP", "").Replace("A", "").Replace(" ", "");
      PANEL_NAME_INPUT.Text = GetSafeString("panel").Replace("'", "");
      PANEL_LOCATION_INPUT.Text = GetSafeString("location");
      BUS_RATING_INPUT.Text = GetSafeString("bus_rating").Replace("AMP", "").Replace("A", "").Replace(" ", "");
      LCL.Text = GetSafeString("lcl");
      LML.Text = GetSafeString("lml");

      // Set Checkboxes
      LCL_OVERRIDE.Checked = GetSafeBoolean("lcl_override");
      LML_OVERRIDE.Checked = GetSafeBoolean("lml_override");

      // Set ComboBoxes
      STATUS_COMBOBOX.SelectedItem = GetSafeString("existing");
      MOUNTING_COMBOBOX.SelectedItem = GetSafeString("mounting");
      WIRE_COMBOBOX.SelectedItem = GetSafeString("wire");
      PHASE_COMBOBOX.SelectedItem = GetSafeString("phase");
      PHASE_VOLTAGE_COMBOBOX.SelectedItem = GetSafeString("voltage2");
      LINE_VOLTAGE_COMBOBOX.SelectedItem = GetSafeString("voltage1");

      // Set DataGridViews
      PHASE_SUM_GRID.Rows[0].Cells[0].Value = GetSafeString("subtotal_a");
      PHASE_SUM_GRID.Rows[0].Cells[1].Value = GetSafeString("subtotal_b");
      if (PHASE_SUM_GRID.ColumnCount > 2) {
        PHASE_SUM_GRID.Rows[0].Cells[2].Value = GetSafeString("subtotal_c");
      }
      TOTAL_VA_GRID.Rows[0].Cells[0].Value = GetSafeString("total_va");
      PANEL_LOAD_GRID.Rows[0].Cells[0].Value = GetSafeString("kva");
      FEEDER_AMP_GRID.Rows[0].Cells[0].Value = GetSafeString("feeder_amps");

      // Set Custom Title if it exists
      if (selectedPanelData.TryGetValue("custom_title", out object customTitle)) {
        CUSTOM_TITLE_TEXT.Text = customTitle?.ToString() ?? "";
      }

      List<string> multi_row_datagrid_keys = new List<string>
    {
        "description_left",
        "description_right",
        "phase_a_left",
        "phase_b_left",
        "phase_a_right",
        "phase_b_right",
        "breaker_left",
        "breaker_right",
        "circuit_left",
        "circuit_right"
    };

      // Check if the panel is three phase and if so add the third phase to the list of keys
      if (selectedPanelData["phase"].ToString() == "3") {
        multi_row_datagrid_keys.AddRange(new List<string> { "phase_c_left", "phase_c_right" });
      }

      int length = ((Newtonsoft.Json.Linq.JArray)selectedPanelData["description_left"]).ToObject<List<string>>().Count;

      for (int i = 0; i < length * 2; i += 2) {
        foreach (string key in multi_row_datagrid_keys) {
          if (selectedPanelData[key] is Newtonsoft.Json.Linq.JArray) {
            List<string> values = ((Newtonsoft.Json.Linq.JArray)selectedPanelData[key]).ToObject<List<string>>();

            if (i < values.Count) {
              string currentValue = values[i];
              string nextValue = i + 1 < values.Count ? values[i + 1] : null;

              if (key.Contains("description") && currentValue == "SPACE") {
                currentValue = string.Empty;
              }

              if (key.Contains("phase") && currentValue == "0") {
                continue; // Skip this iteration if the value is "0" for phases
              }

              if (nextValue != null) {
                if (key.Contains("phase") && nextValue != "0") {
                  currentValue = $"{currentValue};{nextValue}";
                }
                else if (key.Contains("description") && nextValue != "SPACE") {
                  currentValue = $"{currentValue};{nextValue}";
                }
                else if (key.Contains("circuit")) {
                  currentValue = currentValue.Replace("A", "");
                }
                else if (key.Contains("breaker") && nextValue != "") {
                  currentValue = $"{currentValue};{nextValue}";
                }
              }

              // Check if PANEL_GRID has enough rows
              if (PANEL_GRID.Rows.Count <= i / 2) {
                Console.WriteLine($"Warning: PANEL_GRID does not have enough rows. Expected row: {i / 2}");
                continue;
              }

              // Check if PANEL_GRID has the specified column
              if (!PANEL_GRID.Columns.Contains(key)) {
                Console.WriteLine($"Warning: PANEL_GRID does not contain column: {key}");
                continue;
              }

              // Log values before assignment
              Console.WriteLine($"Setting PANEL_GRID.Rows[{i / 2}].Cells[{key}].Value = {currentValue}");

              // Check if the column index for the key is valid
              int columnIndex = PANEL_GRID.Columns[key].Index;
              if (columnIndex < 0 || columnIndex >= PANEL_GRID.ColumnCount) {
                Console.WriteLine($"Warning: Column index for {key} is out of range.");
                continue;
              }

              // Set the cell value
              PANEL_GRID.Rows[i / 2].Cells[columnIndex].Value = currentValue;
            }
            else {
              Console.WriteLine($"Warning: Index {i} is out of range for values in key {key}");
            }
          }
          else {
            // Log or handle the unexpected type
            Console.WriteLine($"Warning: Value for key {key} is not a JArray");
          }
        }
      }
    }

    private void clear_current_modal_data() {
      // Clear TextBoxes
      BUS_RATING_INPUT.Text = string.Empty;
      MAIN_INPUT.Text = string.Empty;
      PANEL_LOCATION_INPUT.Text = string.Empty;
      PANEL_NAME_INPUT.Text = string.Empty;
      LCL.Text = "0";
      LML.Text = "0";

      // Clear ComboBoxes
      STATUS_COMBOBOX.SelectedIndex = -1; // This will unselect all items
      MOUNTING_COMBOBOX.SelectedIndex = -1;
      WIRE_COMBOBOX.SelectedIndex = -1;
      PHASE_COMBOBOX.SelectedIndex = -1;
      PHASE_VOLTAGE_COMBOBOX.SelectedIndex = -1;
      LINE_VOLTAGE_COMBOBOX.SelectedIndex = -1;

      // Clear DataGridViews
      PHASE_SUM_GRID.Rows[0].Cells[0].Value = "0";
      PHASE_SUM_GRID.Rows[0].Cells[1].Value = "0";
      TOTAL_VA_GRID.Rows[0].Cells[0].Value = "0";
      PANEL_LOAD_GRID.Rows[0].Cells[0].Value = "0";
      FEEDER_AMP_GRID.Rows[0].Cells[0].Value = "0";

      // Clear DataGridViews
      for (int i = 0; i < PANEL_GRID.Rows.Count; i++) {
        PANEL_GRID.Rows[i].Cells["description_left"].Value = string.Empty;
        PANEL_GRID.Rows[i].Cells["description_right"].Value = string.Empty;
        PANEL_GRID.Rows[i].Cells["phase_a_left"].Value = string.Empty;
        PANEL_GRID.Rows[i].Cells["phase_b_left"].Value = string.Empty;
        PANEL_GRID.Rows[i].Cells["phase_a_right"].Value = string.Empty;
        PANEL_GRID.Rows[i].Cells["phase_b_right"].Value = string.Empty;
        PANEL_GRID.Rows[i].Cells["breaker_left"].Value = string.Empty;
        PANEL_GRID.Rows[i].Cells["breaker_right"].Value = string.Empty;
        PANEL_GRID.Rows[i].Cells["circuit_left"].Value = string.Empty;
        PANEL_GRID.Rows[i].Cells["circuit_right"].Value = string.Empty;
      }
    }

    private void add_phase_sum_column(bool is3PH) {
      if (is3PH) {
        PHASE_SUM_GRID.Columns.Add(PHASE_SUM_GRID.Columns[0].Clone() as DataGridViewColumn);
        PHASE_SUM_GRID.Columns[2].HeaderText = "PH C (VA)";
        PHASE_SUM_GRID.Columns[2].Name = "TOTAL_PH_C";

        // Set the width of the new column
        PHASE_SUM_GRID.Columns[2].Width = 80;

        // Set the width of the other columns
        PHASE_SUM_GRID.Columns[0].Width = 80;
        PHASE_SUM_GRID.Columns[1].Width = 80;

        // set the width of the grid
        PHASE_SUM_GRID.Width = 285;
        PHASE_SUM_GRID.Location = new System.Drawing.Point(12, 319);
      }
      else {
        if (PHASE_SUM_GRID.Columns.Count > 2) {
          PHASE_SUM_GRID.Columns.Remove("TOTAL_PH_C");
        }

        // Set the width of the other columns
        PHASE_SUM_GRID.Columns[0].Width = 100;
        PHASE_SUM_GRID.Columns[1].Width = 100;

        // set the width of the grid
        PHASE_SUM_GRID.Width = 245;
        PHASE_SUM_GRID.Location = new System.Drawing.Point(52, 319);
      }
    }

    private void link_cell_to_phase(string cellValue, DataGridViewRow row, DataGridViewColumn col) {
      var (panel_name, phase) = convert_cell_value_to_panel_name_and_phase(cellValue);
      if (panel_name.ToLower() == PANEL_NAME_INPUT.Text.ToLower()) {
        return;
      }

      var isPanelReal = this.mainForm.panel_name_exists(panel_name);

      if (isPanelReal) {
        UserControl panelControl = mainForm.findUserControl(panel_name);

        if (panelControl != null) {
          DataGridView panelControl_phaseSumGrid =
            panelControl.Controls.Find("PHASE_SUM_GRID", true).FirstOrDefault() as DataGridView;
          DataGridView this_panelGrid =
            this.Controls.Find("PANEL_GRID", true).FirstOrDefault() as DataGridView;
          this_panelGrid.Rows[row.Index].Cells[col.Index].Tag = cellValue;
          listenForPhaseChanges(panelControl_phaseSumGrid, phase, row, col, this_panelGrid);
        }
      }
    }

    private void listenForPhaseChanges(
      DataGridView panelControl_phaseSumGrid,
      string phase,
      DataGridViewRow row,
      DataGridViewColumn col,
      DataGridView panelGrid
    ) {
      var phaseSumGrid_row = 0;
      var phaseSumGrid_col = 0;

      DataGridViewCellEventHandler eventHandler = null;
      DataGridViewCellEventHandler panelGrid_eventHandler = null;

      if (phase == "A") {
        phaseSumGrid_col = 0;
      }
      else if (phase == "B") {
        phaseSumGrid_col = 1;
      }
      else if (phase == "C") {
        phaseSumGrid_col = 2;
      }

      var newCellValue = panelControl_phaseSumGrid
        .Rows[phaseSumGrid_row]
        .Cells[phaseSumGrid_col]
        .Value.ToString();
      panelGrid.Rows[row.Index].Cells[col.Index].Value = newCellValue;
      panelGrid.Rows[row.Index].Cells[col.Index].Style.BackColor = Color.LightGreen;

      eventHandler = (sender, e) => {
        if (e.RowIndex == phaseSumGrid_row && e.ColumnIndex == phaseSumGrid_col) {
          var newCellValue = panelControl_phaseSumGrid
            .Rows[e.RowIndex]
            .Cells[e.ColumnIndex]
            .Value.ToString();
          panelGrid.Rows[row.Index].Cells[col.Index].Value = newCellValue;
          panelGrid.Rows[row.Index].Cells[col.Index].Style.BackColor = Color.LightGreen;
        }
      };

      panelControl_phaseSumGrid.CellValueChanged += eventHandler;

      panelGrid_eventHandler = (sender, e) => {
        if (e.RowIndex == row.Index && e.ColumnIndex == col.Index) {
          var newCellValue = panelGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
          var phaseSumGridValue = panelControl_phaseSumGrid
            .Rows[phaseSumGrid_row]
            .Cells[phaseSumGrid_col]
            .Value?.ToString();
          if (newCellValue != phaseSumGridValue) {
            panelControl_phaseSumGrid.CellValueChanged -= eventHandler;
            panelGrid.CellValueChanged -= panelGrid_eventHandler;
            panelGrid.Rows[row.Index].Cells[col.Index].Style.BackColor = Color.LightGray;
            if (panelGrid.Rows[row.Index].Cells[col.Index].Tag != null) {
              panelGrid.Rows[row.Index].Cells[col.Index].Tag = null;
            }
          }
        }
      };

      panelGrid.CellValueChanged += panelGrid_eventHandler;
    }

    private (string, string) convert_cell_value_to_panel_name_and_phase(string cellValue) {
      cellValue = cellValue.ToUpper();
      Regex regex = new Regex(@"^=[a-zA-Z0-9]*-[A-C]$");
      if (!regex.IsMatch(cellValue)) {
        return ("", "");
      }

      string[] splitCellValue = cellValue.Split('-');
      string panelName = splitCellValue[0].Replace("=", "");
      string phase = splitCellValue[1];

      return (panelName, phase);
    }

    private void change_size_of_phase_columns(bool is3PH) {
      if (is3PH) {
        // Left Side
        PANEL_GRID.Columns["phase_a_left"].Width = 67;
        PANEL_GRID.Columns["phase_b_left"].Width = 67;
        PANEL_GRID.Columns["phase_c_left"].Width = 67;

        // Right Side
        PANEL_GRID.Columns["phase_a_right"].Width = 67;
        PANEL_GRID.Columns["phase_b_right"].Width = 67;
        PANEL_GRID.Columns["phase_c_right"].Width = 67;
      }
      else {
        // Left Side
        PANEL_GRID.Columns["phase_a_left"].Width = 100;
        PANEL_GRID.Columns["phase_b_left"].Width = 100;

        // Right Side
        PANEL_GRID.Columns["phase_a_right"].Width = 100;
        PANEL_GRID.Columns["phase_b_right"].Width = 100;
      }
    }

    private void add_or_remove_panel_grid_columns(bool is3PH) {
      if (is3PH) {
        // Left Side
        DataGridViewTextBoxColumn phase_c_left = new DataGridViewTextBoxColumn();
        phase_c_left.HeaderText = "PH C";
        phase_c_left.Name = "phase_c_left";
        phase_c_left.Width = 50;
        PANEL_GRID.Columns.Insert(3, phase_c_left);

        // Right Side
        DataGridViewTextBoxColumn phase_c_right = new DataGridViewTextBoxColumn();
        phase_c_right.HeaderText = "PH C";
        phase_c_right.Name = "phase_c_right";
        phase_c_right.Width = 50;
        PANEL_GRID.Columns.Insert(10, phase_c_right);
      }
      else {
        if (PANEL_GRID.Columns.Count > 10) {
          // Left Side
          PANEL_GRID.Columns.Remove("phase_c_left");

          // Right Side
          PANEL_GRID.Columns.Remove("phase_c_right");
        }
      }
    }

    private void update_apply_combobox_to_match_storage() {
      var apply_combobox_items = new List<string>();
      foreach (var note in this.notesStorage) {
        if (!apply_combobox_items.Contains(note)) {
          apply_combobox_items.Add(note);
        }
      }

      APPLY_COMBOBOX.DataSource = apply_combobox_items;
    }

    private void remove_tags_from_cells(string tag) {
      foreach (DataGridViewRow row in PANEL_GRID.Rows) {
        foreach (DataGridViewCell cell in row.Cells) {
          if (cell.Tag != null) {
            string cellTag = cell.Tag.ToString();
            if (cellTag.Contains(tag)) {
              cellTag = cellTag.Replace(tag, "");
              if (cellTag.EndsWith("|")) {
                cellTag = cellTag.Substring(0, cellTag.Length - 1);
              }
              cell.Tag = cellTag;
            }
          }
        }
      }
    }

    public void update_cell_background_color() {
      if (APPLY_COMBOBOX.SelectedItem == null) {
        return;
      }

      foreach (DataGridViewRow row in PANEL_GRID.Rows) {
        foreach (DataGridViewCell cell in row.Cells) {
          if (cell.OwningColumn.Name.Contains("description")) {
            cell.Style.BackColor = Color.Empty;
          }
          else if (cell.OwningColumn.Name.Contains("phase") && cell.Tag != null && cell.Tag.ToString().Contains("LCL125")) {
            cell.Style.BackColor = Color.Salmon;
          }
          else if (cell.OwningColumn.Name.Contains("phase") && cell.Tag != null && cell.Tag.ToString().Contains("LCL80")) {
            cell.Style.BackColor = Color.Gold;
          }
        }
      }

      foreach (DataGridViewRow row in PANEL_GRID.Rows) {
        foreach (DataGridViewCell cell in row.Cells) {
          if (cell.OwningColumn.Name.Contains("description")) {
            if (cell.Tag == null) {
              continue;
            }
            if (cell.Tag.ToString().Split('|').Contains(APPLY_COMBOBOX.SelectedItem.ToString())) {
              // turn the background of the cell to a yellow color
              cell.Style.BackColor = Color.Yellow;
            }
          }
        }
      }
    }

    public void update_notes_storage(List<string> notesStorage) {
      this.notesStorage = notesStorage;
      update_apply_combobox_to_match_storage();
    }

    private void update_panel_grid(
      int phaseSumGridColumnCount,
      int panelPhaseSumGridColumnCount,
      int rowIndex,
      DataGridViewColumn col,
      string panelName
    ) {
      if (phaseSumGridColumnCount == panelPhaseSumGridColumnCount) {
        int rowCount = phaseSumGridColumnCount == 3 ? 3 : 2;
        if (PANEL_GRID.Rows.Count > rowIndex + rowCount - 1) {
          var cellValueA = "=" + panelName.ToUpper() + "-A";
          var cellValueB = "=" + panelName.ToUpper() + "-B";
          var cellValueC = phaseSumGridColumnCount == 3 ? "=" + panelName.ToUpper() + "-C" : null;

          string side = col.Name.Contains("left") ? "left" : "right";
          List<DataGridViewRow> rows = new List<DataGridViewRow>();
          for (int i = 0; i < rowCount; i++) {
            rows.Add(PANEL_GRID.Rows[rowIndex + i]);
          }

          List<string> phases = new List<string> { "phase_a_", "phase_b_" };
          if (phaseSumGridColumnCount == 3) {
            phases.Add("phase_c_");
          }

          List<string> cellValues = new List<string> { cellValueA, cellValueB };
          if (cellValueC != null) {
            cellValues.Add(cellValueC);
          }

          for (int i = 0; i < phases.Count; i++) {
            foreach (DataGridViewRow gridRow in rows) {
              string cellName = phases[i] + side;
              if (
                gridRow.Cells[cellName].Style.BackColor == Color.LightGray
                || gridRow.Cells[cellName].Style.BackColor == Color.LightGreen
              ) {
                gridRow.Cells[cellName].Value = cellValues[i];
              }
            }
          }
        }
      }
      else if (phaseSumGridColumnCount == 2 && panelPhaseSumGridColumnCount == 3) {
        if (PANEL_GRID.Rows.Count > rowIndex + 1) {
          var phases = new List<string> { "A", "B" };
          if (col.Name.Contains("left")) {
            for (int i = rowIndex; i < rowIndex + 2; i++) // Loop for the first two rows
            {
              foreach (string colName in new[] { "phase_a_left", "phase_b_left", "phase_c_left" }) // Loop for the specified columns
              {
                var cell = PANEL_GRID.Rows[i].Cells[colName];
                if (
                  cell.Style.BackColor == Color.LightGray
                  || cell.Style.BackColor == Color.LightGreen
                ) // Check the background color
                {
                  cell.Value = "=" + panelName + "-" + phases[i - rowIndex];
                }
              }
            }
          }
          else {
            for (int i = rowIndex; i < rowIndex + 2; i++) // Loop for the first two rows
            {
              foreach (
                string colName in new[] { "phase_a_right", "phase_b_right", "phase_c_right" }
              ) // Loop for the specified columns
              {
                var cell = PANEL_GRID.Rows[i].Cells[colName];
                if (
                  cell.Style.BackColor == Color.LightGray
                  || cell.Style.BackColor == Color.LightGreen
                ) // Check the background color
                {
                  cell.Value = "=" + panelName + "-" + phases[i - rowIndex];
                }
              }
            }
          }
        }
      }
    }

    private bool BreakerContainsNote(int rowIndex, string columnName, string note) {
      string columnPrefix = columnName.Contains("left") ? "left" : "right";
      var descriptionCellTag = PANEL_GRID.Rows[rowIndex].Cells[$"description_{columnPrefix}"].Tag;
      var descriptionCellValue = PANEL_GRID
        .Rows[rowIndex]
        .Cells[$"description_{columnPrefix}"]
        .Value;
      var breakerCellValue = PANEL_GRID.Rows[rowIndex].Cells[$"breaker_{columnPrefix}"].Value;

      if (descriptionCellTag != null && descriptionCellTag.ToString().Contains(note)) {
        return true;
      }

      if (
        descriptionCellValue == null
        || descriptionCellValue.ToString() == ""
        || descriptionCellValue.ToString().All(c => c == '-')
      ) {
        if (
          breakerCellValue != null
          && (breakerCellValue.ToString() == "2" || breakerCellValue.ToString() == "3")
        ) {
          int rowsAbove = breakerCellValue.ToString() == "2" ? 1 : 2;
          if (rowIndex < rowsAbove) {
            return false;
          }
          var descriptionCellTagAbove = PANEL_GRID
            .Rows[rowIndex - rowsAbove]
            .Cells[$"description_{columnPrefix}"]
            .Tag;
          if (descriptionCellTagAbove != null && descriptionCellTagAbove.ToString().Contains(note)) {
            return true;
          }
        }

        if (breakerCellValue == null || breakerCellValue.ToString() == "") {
          if (rowIndex == PANEL_GRID.Rows.Count - 1) {
            return false;
          }
          var nextBreakerCellValue = PANEL_GRID
            .Rows[rowIndex + 1]
            .Cells[$"breaker_{columnPrefix}"]
            .Value;
          if (nextBreakerCellValue != null && nextBreakerCellValue.ToString() == "3") {
            var descriptionCellTagAbove = PANEL_GRID
              .Rows[rowIndex - 1]
              .Cells[$"description_{columnPrefix}"]
              .Tag;
            if (
              descriptionCellTagAbove != null
              && descriptionCellTagAbove.ToString().Contains(note)
            ) {
              return true;
            }
          }
        }
      }
      return false;
    }

    private string calculate_cell_or_link_panel(
      DataGridViewCellEventArgs e,
      string cellValue,
      DataGridViewRow row,
      DataGridViewColumn col
    ) {
      if (cellValue.StartsWith("=")) {
        if (col.Name.Contains("phase")) {
          cellValue = cellValue.Replace(" ", "");
        }
        if (
          cellValue.All(c =>
            char.IsDigit(c)
            || c == '.'
            || c == '='
            || c == '-'
            || c == '*'
            || c == '+'
            || c == '/'
            || c == '('
            || c == ')'
          )
        ) {
          var result = new System.Data.DataTable().Compute(cellValue.Replace("=", ""), null);
          PANEL_GRID.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Math.Ceiling(Convert.ToDouble(result));
        }
        else {
          link_cell_to_phase(cellValue, row, col);
        }
      }

      return cellValue;
    }

    private void auto_link_subpanels(string cellValue, DataGridViewRow row, DataGridViewColumn col) {
      if (col.Name.Contains("description")) {
        if (cellValue.ToLower().Contains("panel")) {
          var panelName = cellValue.ToLower().Split(' ').Last();

          if (panelName.Contains("'") || panelName.Contains("`")) {
            panelName = panelName.Replace("'", "").Replace("`", "");
          }

          if (panelName.ToUpper() == PANEL_NAME_INPUT.Text.ToUpper()) return;

          var isPanelReal = this.mainForm.panel_name_exists(panelName);

          if (isPanelReal) {
            UserControl panelControl = mainForm.findUserControl(panelName);
            DataGridView panelControl_phaseSumGrid =
              panelControl.Controls.Find("PHASE_SUM_GRID", true).FirstOrDefault() as DataGridView;
            var phaseSumGridColumnCount = panelControl_phaseSumGrid.ColumnCount;
            var panelPhaseSumGridColumnCount = PHASE_SUM_GRID.ColumnCount;

            update_panel_grid(
              phaseSumGridColumnCount,
              panelPhaseSumGridColumnCount,
              row.Index,
              col,
              panelName
            );
            UpdatePerCellValueChange();
          }
        }
      }
    }

    private void remove_existing_breaker_note(DataGridViewCell dataGridViewCell) {
      if (!dataGridViewCell.OwningColumn.Name.Contains("breaker")) {
        return;
      }

      var side = dataGridViewCell.OwningColumn.Name.Contains("left") ? "left" : "right";

      if (PANEL_GRID.Rows[dataGridViewCell.RowIndex].Cells["description_" + side].Tag == null) {
        return;
      }

      var descriptionCellTag = PANEL_GRID
        .Rows[dataGridViewCell.RowIndex]
        .Cells["description_" + side]
        .Tag;
      descriptionCellTag = descriptionCellTag
        .ToString()
        .Replace("DENOTES EXISTING CIRCUIT BREAKER TO REMAIN; ALL OTHERS ARE NEW.", "");
      descriptionCellTag = descriptionCellTag.ToString().TrimEnd('|');

      PANEL_GRID.Rows[dataGridViewCell.RowIndex].Cells["description_" + side].Tag =
        descriptionCellTag;

      if (
        APPLY_COMBOBOX.SelectedItem != null
        && APPLY_COMBOBOX.SelectedItem.ToString()
          == "DENOTES EXISTING CIRCUIT BREAKER TO REMAIN; ALL OTHERS ARE NEW."
      ) {
        PANEL_GRID.Rows[dataGridViewCell.RowIndex].Cells["description_" + side].Style.BackColor =
          Color.White;
      }
    }

    private void remove_existing_from_description(DataGridViewCell dataGridViewCell) {
      if (!dataGridViewCell.OwningColumn.Name.Contains("description")) {
        return;
      }

      if (dataGridViewCell.Tag == null) {
        return;
      }

      var cellTag = dataGridViewCell.Tag.ToString();
      cellTag = cellTag.Replace("ADD SUFFIX (E). *NOT ADDED AS NOTE*", "");
      cellTag = cellTag.ToString().TrimEnd('|');

      dataGridViewCell.Tag = cellTag;

      if (
        APPLY_COMBOBOX.SelectedItem != null
        && APPLY_COMBOBOX.SelectedItem.ToString() == "ADD SUFFIX (E). *NOT ADDED AS NOTE*"
      ) {
        dataGridViewCell.Style.BackColor = Color.White;
      }
    }

    private void PANEL_NAME_INPUT_TextChanged(object sender, EventArgs e) {
      this.mainForm.PANEL_NAME_INPUT_TextChanged(sender, e, PANEL_NAME_INPUT.Text.ToUpper());
    }

    private void PANEL_GRID_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e) {
      if (PHASE_SUM_GRID.ColumnCount > 2) {
        listen_for_3P_rows_added(e);
      }
      else {
        listen_for_2P_rows_added(e);
      }
    }

    private void PANEL_GRID_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
      e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
      e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;
    }

    private void PANEL_GRID_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
      remove_existing_from_description(PANEL_GRID.Rows[e.RowIndex].Cells[e.ColumnIndex]);
      remove_existing_breaker_note(PANEL_GRID.Rows[e.RowIndex].Cells[e.ColumnIndex]);

      if (PANEL_GRID.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null) {
        return;
      }

      CalculateBreakerLoad();
    }

    private void PANEL_GRID_CellValueChangedLink(object sender, DataGridViewCellEventArgs e) {
      var cell = PANEL_GRID.Rows[e.RowIndex].Cells[e.ColumnIndex];
      if (cell == null) {
        return;
      }

      string cellValue = cell.Value?.ToString() ?? string.Empty;
      var row = PANEL_GRID.Rows[e.RowIndex];
      var col = PANEL_GRID.Columns[e.ColumnIndex];

      if (row != null && col != null) {
        auto_link_subpanels(cellValue, row, col);
        cellValue = calculate_cell_or_link_panel(e, cellValue, row, col);
      }
    }

    private void PANEL_GRID_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e) {
      selectedCells = new List<DataGridViewCell>(PANEL_GRID.SelectedCells.Cast<DataGridViewCell>());
      oldValue = PANEL_GRID.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
    }

    private void PANEL_GRID_KeyDown(object sender, KeyEventArgs e) {
      if (e.Control && e.KeyCode == Keys.V) {
        // Get text from clipboard
        string text = Clipboard.GetText();

        if (!string.IsNullOrEmpty(text)) {
          // Split clipboard text into lines
          string[] lines = text.Split('\n');

          if (lines.Length > 0 && string.IsNullOrWhiteSpace(lines[lines.Length - 1])) {
            Array.Resize(ref lines, lines.Length - 1);
          }

          // Get start cell for pasting
          int rowIndex = PANEL_GRID.CurrentCell.RowIndex;
          int colIndex = PANEL_GRID.CurrentCell.ColumnIndex;

          // Paste each line into a row
          foreach (string line in lines) {
            string[] parts = line.Split('\t');

            for (int i = 0; i < parts.Length; i++) {
              if (rowIndex < PANEL_GRID.RowCount && colIndex + i < PANEL_GRID.ColumnCount) {
                try {
                  PANEL_GRID[colIndex + i, rowIndex].Value = parts[i].Trim();
                }
                catch (FormatException) {
                  // Set to default value
                  PANEL_GRID[colIndex + i, rowIndex].Value = 0;

                  // Or notify user
                  MessageBox.Show("Invalid format in cell!");
                }
              }
            }

            rowIndex++;
          }
        }

        e.Handled = true;
      }
      // Check if Ctrl+C was pressed
      else if (e.Control && e.KeyCode == Keys.C) {
        StringBuilder copiedText = new StringBuilder();

        // Get the minimum and maximum rowIndex and columnIndex of the selected cells
        int minRowIndex = PANEL_GRID
          .SelectedCells.Cast<DataGridViewCell>()
          .Min(cell => cell.RowIndex);
        int maxRowIndex = PANEL_GRID
          .SelectedCells.Cast<DataGridViewCell>()
          .Max(cell => cell.RowIndex);
        int minColumnIndex = PANEL_GRID
          .SelectedCells.Cast<DataGridViewCell>()
          .Min(cell => cell.ColumnIndex);
        int maxColumnIndex = PANEL_GRID
          .SelectedCells.Cast<DataGridViewCell>()
          .Max(cell => cell.ColumnIndex);

        // Loop through the rows
        for (int rowIndex = minRowIndex; rowIndex <= maxRowIndex; rowIndex++) {
          List<string> cellValues = new List<string>();

          // Loop through the columns
          for (int columnIndex = minColumnIndex; columnIndex <= maxColumnIndex; columnIndex++) {
            DataGridViewCell cell = PANEL_GRID[columnIndex, rowIndex];

            // Only add the cell value to the list if the cell is selected
            if (cell.Selected) {
              cellValues.Add(cell.Value?.ToString() ?? string.Empty);
            }
          }

          // Add the cell values of the row to the copied text
          if (cellValues.Count > 0) {
            copiedText.AppendLine(string.Join("\t", cellValues));
          }
        }

        if (copiedText.Length > 0) {
          Clipboard.SetText(copiedText.ToString());
        }

        e.Handled = true;
      }
      // Existing code for handling the Delete key
      else if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back) {
        foreach (DataGridViewCell cell in PANEL_GRID.SelectedCells) {
          cell.Value = "";
          cell.Tag = null;
        }
        e.Handled = true;

        UpdatePerCellValueChange();
      }
      // Check if Ctrl+D was pressed
      else if (e.Control && e.KeyCode == Keys.D) {
        int rowIndex = PANEL_GRID.CurrentCell.RowIndex;
        int colIndex = PANEL_GRID.CurrentCell.ColumnIndex;

        // Check if there is a row above
        if (rowIndex > 0) {
          // Get the value from the cell above
          object value = PANEL_GRID[colIndex, rowIndex - 1].Value;

          // Paste the value into the current cell
          PANEL_GRID[colIndex, rowIndex].Value = value;
        }

        e.Handled = true;
      }
    }

    private async void PANEL_GRID_CellClick(object sender, DataGridViewCellEventArgs e) {
      if (e.RowIndex == -1) {
        return;
      }

      if (e.ColumnIndex < 0) {
        return;
      }

      if (!PANEL_GRID.Columns[e.ColumnIndex].Name.Contains("phase")) {
        return;
      }

      // Get the selected cell
      DataGridViewCell cell = PANEL_GRID.Rows[e.RowIndex].Cells[e.ColumnIndex];

      // check if the cell has a tag
      if (cell.Tag != null) {
        if (!cell.Tag.ToString().Contains("=")) return;
        // remove the equals from the tag
        string cellValue = cell.Tag.ToString().Replace("=", "");

        // split the cell value by dash, to create two strings, one for the panel name and one for the phase
        string[] splitCellValue = cellValue.Split('-');

        // get the panel name and phase from the split cell value
        string panelName = splitCellValue[0];
        string phase = splitCellValue[1];

        // change the INFO_LABEL.TEXT value to inform the user that the cell is linked to another panel and its phase for 5 seconds, then erase it
        INFO_LABEL.Text = $"This cell is linked to phase {phase} of panel '{panelName}'.";

        // wait for 5 seconds
        await Task.Delay(5000);

        // if the INFO_LABEL.TEXT value is still the same as it was 5 seconds ago, then erase it
        if (INFO_LABEL.Text == $"This cell is linked to phase {phase} of panel '{panelName}'.") {
          INFO_LABEL.Text = string.Empty;
        }
      }
    }

    private void PANEL_GRID_CellPainting(object sender, DataGridViewCellPaintingEventArgs e) {
      if (e.RowIndex >= 0 && e.ColumnIndex >= 0) // Check if it's not the header cell
      {
        var cell = PANEL_GRID[e.ColumnIndex, e.RowIndex];
        if (cell.Selected) {
          e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);

          using (Pen p = new Pen(Color.Black, 2)) // Change to desired border color and size
          {
            Rectangle rect = e.CellBounds;
            rect.Width -= 2;
            rect.Height -= 2;
            e.Graphics.DrawRectangle(p, rect);
          }

          e.Handled = true;
        }
      }
    }

    private void PHASE_SUM_GRID_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
      if (e.RowIndex == 0 && (e.ColumnIndex == 0 || e.ColumnIndex == 1 || e.ColumnIndex == 2)) {
        UpdatePerCellValueChange();
      }
    }

    public void UpdatePerCellValueChange() {
      double phA = Convert.ToDouble(PHASE_SUM_GRID.Rows[0].Cells[0].Value ?? 0);
      double phB = Convert.ToDouble(PHASE_SUM_GRID.Rows[0].Cells[1].Value ?? 0);
      double phC = 0;
      double sum = phA + phB;
      if (PHASE_SUM_GRID.ColumnCount > 2) {
        phC = Convert.ToDouble(PHASE_SUM_GRID.Rows[0].Cells[2].Value ?? 0);
        sum += phC;
      }
      mainForm.UpdateLCLLML();

      TOTAL_VA_GRID.Rows[0].Cells[0].Value = CalculateTotalVA(sum);

      // Handle LCL
      if (!string.IsNullOrEmpty(LCL.Text) && LCL.Text != "0") {
        sum += Math.Round(Convert.ToDouble(LCL.Text) * 0.25, 0);
      }

      // Handle LML
      if (!string.IsNullOrEmpty(LML.Text) && LML.Text != "0") {
        sum += Math.Round(Convert.ToDouble(LML.Text) * 0.25, 0);
      }

      PANEL_LOAD_GRID.Rows[0].Cells[0].Value = CalculatePanelLoad(sum);

      object lineVoltageObj = LINE_VOLTAGE_COMBOBOX.SelectedItem;
      if (lineVoltageObj != null) {
        double lineVoltage = Convert.ToDouble(lineVoltageObj);
        double feederAmps = 0;
        double busRating = Convert.ToDouble(BUS_RATING_INPUT.Text);
        if (LCL.Text == "0" && LML.Text == "0") {
          feederAmps = CalculateFeederAmps(phA, phB, phC, lineVoltage);
          FEEDER_AMP_GRID.Rows[0].Cells[0].Value = feederAmps;
        }
        else {
          feederAmps = Math.Round(sum / (lineVoltage * 3), 1);
          FEEDER_AMP_GRID.Rows[0].Cells[0].Value = feederAmps;
        }
        if (feederAmps > busRating) {
          // turn bg red
          foreach (DataGridViewRow row in FEEDER_AMP_GRID.Rows) {
            foreach (DataGridViewCell cell in row.Cells) {
              cell.Style.SelectionBackColor = Color.Crimson;
              cell.Style.SelectionForeColor = Color.White;
            }
          }
        }
        else if (feederAmps > 0.8 * busRating) {
          // turn bg orange
          foreach (DataGridViewRow row in FEEDER_AMP_GRID.Rows) {
            foreach (DataGridViewCell cell in row.Cells) {
              cell.Style.SelectionBackColor = Color.Orange;
              cell.Style.SelectionForeColor = Color.Black;
            }
          }
        }
        else if (feederAmps > 0.6 * busRating) {
          // turn bg yellow
          foreach (DataGridViewRow row in FEEDER_AMP_GRID.Rows) {
            foreach (DataGridViewCell cell in row.Cells) {
              cell.Style.SelectionBackColor = Color.Gold;
              cell.Style.SelectionForeColor = Color.Black;
            }
          }
        }
        else {
          // turn bg white
          foreach (DataGridViewRow row in FEEDER_AMP_GRID.Rows) {
            foreach (DataGridViewCell cell in row.Cells) {
              cell.Style.SelectionBackColor = Color.DarkSeaGreen;
              cell.Style.SelectionForeColor = Color.Black;
            }
          }
        }
      }
    }

    public double CalculateFeederAmps(double phA, double phB, double phC, double lineVoltage) {
      if (lineVoltage == 0) {
        return 0;
      }

      double maxVal = Math.Max(Math.Max(phA, phB), phC);
      return Math.Round(maxVal / lineVoltage, 1);
    }

    public void UpdateLCLLMLLabels(int lcl, int lml) {
      if (!LCL_OVERRIDE.Checked) {
        LCL.Text = $"{lcl}";
      }
      if (!LML_OVERRIDE.Checked) {
        LML.Text = $"{lml}";
      }
    }

    private void SaveLCLLMLObjectAsJson(object LCLLMLObject) {
      string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
      string filePath = Path.Combine(desktopPath, "LCLLMLObject.json");

      try {
        string jsonString = JsonConvert.SerializeObject(LCLLMLObject, Formatting.Indented);
        File.WriteAllText(filePath, jsonString);
        Console.WriteLine("LCLLMLObject saved successfully.");
      }
      catch (Exception ex) {
        Console.WriteLine($"Error saving LCLLMLObject: {ex.Message}");
      }
    }

    private void ADD_ROW_BUTTON_Click(object sender, EventArgs e) {
      PANEL_GRID.Rows.Add();

      if (PANEL_GRID.Rows.Count > 21) {
        PANEL_GRID.Width = 1047 + 15;
      }
    }

    private void CREATE_PANEL_BUTTON_Click(object sender, EventArgs e) {
      Dictionary<string, object> panelDataList = retrieve_data_from_modal();

      using (
        DocumentLock docLock =
          Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument()
      ) {
        this.mainForm.Close();
        myCommandsInstance.Create_Panel(panelDataList);

        Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.WindowState = Autodesk
          .AutoCAD
          .Windows
          .Window
          .State
          .Maximized;
        Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Window.Focus();
      }
    }

    private void DELETE_ROW_BUTTON_Click(object sender, EventArgs e) {
      if (PANEL_GRID.Rows.Count > 0) {
        var lastRow = PANEL_GRID.Rows[PANEL_GRID.Rows.Count - 1];
        var phaseCells = new List<string>
        {
          "phase_a_left",
          "phase_b_left",
          "phase_a_right",
          "phase_b_right"
        };

        if (PHASE_SUM_GRID.ColumnCount > 2) {
          phaseCells.Add("phase_c_left");
          phaseCells.Add("phase_c_right");
        }

        foreach (var cell in phaseCells) {
          lastRow.Cells[cell].Value = "0";
        }

        PANEL_GRID.Rows.RemoveAt(PANEL_GRID.Rows.Count - 1);

        if (PANEL_GRID.Rows.Count <= 21) {
          PANEL_GRID.Width = 1047;
        }
      }
    }

    private void DELETE_PANEL_BUTTON_Click(object sender, EventArgs e) {
      this.mainForm.delete_panel(this);
    }

    private void INFO_LABEL_CLICK(object sender, EventArgs e) {
    }

    private void NOTES_BUTTON_Click(object sender, EventArgs e) {
      if (this.notesForm == null || this.notesForm.IsDisposed) {
        this.notesForm = new NoteForm(this);
        this.notesForm.Show();
        this.notesForm.Text = $"Panel '{PANEL_NAME_INPUT.Text}' Notes";
      }
      else {
        if (!this.notesForm.Visible) {
          this.notesForm.Show();
        }
        this.notesForm.BringToFront();
      }
    }

    private void APPLY_BUTTON_Click(object sender, EventArgs e) {
      string selectedValue = APPLY_COMBOBOX.SelectedItem.ToString();

      List<string> columnNames = new List<string> { "description" };

      foreach (DataGridViewCell cell in PANEL_GRID.SelectedCells) {
        if (columnNames.Any(cell.OwningColumn.Name.Contains)) {
          if (cell.Tag == null) {
            cell.Tag = selectedValue;
          }
          else {
            cell.Tag = $"{cell.Tag}|{selectedValue}";
          }
          cell.Style.BackColor = Color.Yellow;
        }
      }

      CalculateBreakerLoad();
      UpdatePerCellValueChange();
    }

    private void APPLY_COMBOBOX_SelectedIndexChanged(object sender, EventArgs e) {
      update_cell_background_color();
    }

    private void STATUS_COMBOBOX_SelectedIndexChanged(object sender, EventArgs e) {
      var default_existing_message =
        "DENOTES EXISTING CIRCUIT BREAKER TO REMAIN; ALL OTHERS ARE NEW.";
      var default_new_message = "65 KAIC SERIES RATED OR MATCH FAULT CURRENT AT SITE.";

      if (STATUS_COMBOBOX.SelectedItem != null) {
        if (
          STATUS_COMBOBOX.SelectedItem.ToString() == "EXISTING"
          || STATUS_COMBOBOX.SelectedItem.ToString() == "RELOCATED"
        ) {
          if (!this.notesStorage.Contains(default_existing_message)) {
            this.notesStorage.Add(default_existing_message);
          }
          if (this.notesStorage.Contains(default_new_message)) {
            this.notesStorage.Remove(default_new_message);
          }
          remove_tags_from_cells(default_new_message);
        }
        else {
          if (!this.notesStorage.Contains(default_new_message)) {
            this.notesStorage.Add(default_new_message);
          }
          if (this.notesStorage.Contains(default_existing_message)) {
            this.notesStorage.Remove(default_existing_message);
          }
          remove_tags_from_cells(default_existing_message);
        }
        update_apply_combobox_to_match_storage();
      }
    }

    private void REMOVE_NOTE_BUTTON_Click(object sender, EventArgs e) {
      string selectedValue = APPLY_COMBOBOX.SelectedItem.ToString();
      List<string> columnNames = new List<string> { "description" };

      foreach (DataGridViewCell cell in PANEL_GRID.SelectedCells) {
        if (columnNames.Any(cell.OwningColumn.Name.Contains)) {
          if (cell.Tag != null && cell.Tag.ToString().Contains(selectedValue)) {
            cell.Tag = cell.Tag.ToString().Replace(selectedValue, "").Trim('|');
            cell.Style.BackColor = Color.Empty;
          }
        }
      }

      CalculateBreakerLoad();
      UpdatePerCellValueChange();
    }

    private void REPLACE_BUTTON_Click(object sender, EventArgs e) {
      string findText = FIND_BOX.Text;
      string replaceText = REPLACE_BOX.Text;

      if (string.IsNullOrEmpty(findText)) {
        MessageBox.Show("Please enter a text to find.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      foreach (DataGridViewCell cell in PANEL_GRID.SelectedCells) {
        if (cell.Value != null && cell.Value.ToString().Contains(findText)) {
          cell.Value = cell.Value.ToString().Replace(findText, replaceText);
        }
      }
    }

    public string GetNewOrExisting() {
      return STATUS_COMBOBOX.Text;
    }

    public void AddListeners() {
      PANEL_GRID.KeyDown += new KeyEventHandler(this.PANEL_GRID_KeyDown);
      PANEL_GRID.CellBeginEdit += new DataGridViewCellCancelEventHandler(
        this.PANEL_GRID_CellBeginEdit
      );
      PANEL_GRID.CellValueChanged += new DataGridViewCellEventHandler(
        this.PANEL_GRID_CellValueChanged
      );
      PHASE_SUM_GRID.CellValueChanged += new DataGridViewCellEventHandler(
        this.PHASE_SUM_GRID_CellValueChanged
      );
      PANEL_GRID.CellFormatting += PANEL_GRID_CellFormatting;
      PANEL_GRID.CellClick += new DataGridViewCellEventHandler(this.PANEL_GRID_CellClick);
      PANEL_NAME_INPUT.Click += (sender, e) => {
        PANEL_NAME_INPUT.SelectAll();
      };
      PANEL_LOCATION_INPUT.Click += (sender, e) => {
        PANEL_LOCATION_INPUT.SelectAll();
      };
      PANEL_GRID.CellPainting += new DataGridViewCellPaintingEventHandler(PANEL_GRID_CellPainting);
      MAIN_INPUT.Click += (sender, e) => {
        MAIN_INPUT.SelectAll();
      };
      BUS_RATING_INPUT.Click += (sender, e) => {
        BUS_RATING_INPUT.SelectAll();
      };
      EXISTING_BUTTON.Click += new System.EventHandler(this.EXISTING_BUTTON_Click);
      RELOCATE_BUTTON.Click += new System.EventHandler(this.RELOCATE_BUTTON_Click);
      LCL.TextChanged += new EventHandler(LCL_LML_TextChanged);
      LML.TextChanged += new EventHandler(LCL_LML_TextChanged);
    }

    private void LCL_LML_TextChanged(object sender, EventArgs e) {
      if (LCL_OVERRIDE.Checked || LML_OVERRIDE.Checked) {
        UpdatePerCellValueChange();
      }
    }

    private void LCL_OVERRIDE_CheckedChanged(object sender, EventArgs e) {
      if (LCL.Enabled) {
        LCL.Enabled = false;
        UpdatePerCellValueChange();
      }
      else {
        LCL.Enabled = true;
      }
    }

    private void LML_OVERRIDE_CheckedChanged(object sender, EventArgs e) {
      if (LML.Enabled) {
        LML.Enabled = false;
        UpdatePerCellValueChange();
      }
      else {
        LML.Enabled = true;
      }
    }

    public double GetLCLOverride() {
      if (LCL_OVERRIDE.Checked) {
        double result;
        if (double.TryParse(LCL.Text, out result)) {
          return result;
        }
        else {
          return 0;
        }
      }
      else {
        return 0;
      }
    }

    public double GetLMLOverride() {
      if (LML_OVERRIDE.Checked) {
        double result;
        if (double.TryParse(LML.Text, out result)) {
          return result;
        }
        else {
          return 0;
        }
      }
      else {
        return 0;
      }
    }
    private string ConvertHPtoVA(string hpValue, int numPhases, string voltage) {
      string sanitized = Regex.Replace(hpValue, @" +HP", "HP");
      sanitized = Regex.Replace(sanitized, @"\p{Zs}+", "+");
      sanitized = Regex.Replace(sanitized, @"-+", "+");
      sanitized = sanitized.Replace("HP", "");
      if (!Regex.IsMatch(sanitized, @"^\d+(\+\d\/\d)?$")) {
        return "-1";
      }
      System.Data.DataTable dt = new System.Data.DataTable();
      double sumObject = Convert.ToDouble(dt.Compute(sanitized, null));
      string phaseVA = "";

      if (numPhases == 1) {
        Console.WriteLine("num phases = 1");
        switch (sumObject) {
          case var _ when sumObject > 7.5: { // 10
              if (voltage == "120") {
                phaseVA = "12000";
              }
              if (voltage == "208") {
                phaseVA = "5720";
              }
              if (voltage == "240") {
                phaseVA = "6000";
              }
              break;
            };
          case var _ when sumObject > 5: { // 7.5
              if (voltage == "120") {
                phaseVA = "9600";
              }
              if (voltage == "208") {
                phaseVA = "4576";
              }
              if (voltage == "240") {
                phaseVA = "4800";
              }
              break;
            };
          case var _ when sumObject > 3: { // 5
              if (voltage == "120") {
                phaseVA = "6720";
              }
              if (voltage == "208") {
                phaseVA = "3220";
              }
              if (voltage == "240") {
                phaseVA = "3360";
              }
              break;
            };
          case var _ when sumObject > 2: { // 3
              if (voltage == "120") {
                phaseVA = "4080";
              }
              if (voltage == "208") {
                phaseVA = "1945";
              }
              if (voltage == "240") {
                phaseVA = "2040";
              }
              break;
            };
          case var _ when sumObject > 1.5: { // 2
              if (voltage == "120") {
                phaseVA = "2880";
              }
              if (voltage == "208") {
                phaseVA = "1373";
              }
              if (voltage == "240") {
                phaseVA = "2040";
              }
              break;
            };
          case var _ when sumObject > 1: { // 1 1/2
              if (voltage == "120") {
                phaseVA = "2400";
              }
              if (voltage == "208") {
                phaseVA = "1144";
              }
              if (voltage == "240") {
                phaseVA = "1200";
              }
              break;
            };
          case var _ when sumObject > 0.75: { // 1
              if (voltage == "120") {
                phaseVA = "1920";
              }
              if (voltage == "208") {
                phaseVA = "915";
              }
              if (voltage == "240") {
                phaseVA = "960";
              }
              break;
            };
          case var _ when sumObject > 0.5: { // 3/4
              if (voltage == "120") {
                phaseVA = "1656";
              }
              if (voltage == "208") {
                phaseVA = "791";
              }
              if (voltage == "240") {
                phaseVA = "828";
              }
              break;
            };
          case var _ when sumObject > 0.34: { // 1/2
              if (voltage == "120") {
                phaseVA = "1176";
              }
              if (voltage == "208") {
                phaseVA = "562";
              }
              if (voltage == "240") {
                phaseVA = "588";
              }
              break;
            };
          case var _ when sumObject > 0.25: { // 1/3
              if (voltage == "120") {
                phaseVA = "864";
              }
              if (voltage == "208") {
                phaseVA = "416";
              }
              if (voltage == "240") {
                phaseVA = "432";
              }
              break;
            };
          case var _ when sumObject > 0.167: { // 1/4
              if (voltage == "120") {
                phaseVA = "696";
              }
              if (voltage == "208") {
                phaseVA = "333";
              }
              if (voltage == "240") {
                phaseVA = "348";
              }
              break;
            };
          case var _ when sumObject <= 0.167: { // 1/6
              if (voltage == "120") {
                phaseVA = "528";
              }
              if (voltage == "208") {
                phaseVA = "250";
              }
              if (voltage == "240") {
                phaseVA = "264";
              }
              break;
            };
        }
      }

      if (numPhases == 3) {
        switch (sumObject) {
          case var _ when sumObject > 15: { // 20
              if (voltage == "208") {
                phaseVA = Math.Round(59.4 * 208.0).ToString();
              }
              if (voltage == "480") {
                phaseVA = Math.Round(27 * 460.0).ToString();
              }
              break;
            }
          case var _ when sumObject > 10: { // 15
              if (voltage == "208") {
                phaseVA = Math.Round(46.2 * 208.0).ToString();
              }
              if (voltage == "480") {
                phaseVA = Math.Round(21.0 * 460.0).ToString();
              }
              break;
            };
          case var _ when sumObject > 7.5: { // 10
              if (voltage == "208") {
                phaseVA = Math.Round(30.8 * 208.0).ToString();
              }
              if (voltage == "480") {
                phaseVA = Math.Round(14.0 * 460.0).ToString();
              }
              break;
            };
          case var _ when sumObject > 5: { // 7 1/2
              if (voltage == "208") {
                phaseVA = Math.Round(24.2 * 208.0).ToString();
              }
              if (voltage == "480") {
                phaseVA = Math.Round(11.0 * 460.0).ToString();
              }
              break;
            };
          case var _ when sumObject > 3: { // 5
              if (voltage == "208") {
                phaseVA = Math.Round(16.7 * 208.0).ToString();
              }
              if (voltage == "480") {
                phaseVA = Math.Round(7.6 * 460.0).ToString();
              }
              break;
            };
          case var _ when sumObject > 2: { // 3
              if (voltage == "208") {
                phaseVA = Math.Round(10.6 * 208.0).ToString();
              }
              if (voltage == "480") {
                phaseVA = Math.Round(4.8 * 460.0).ToString();
              }
              break;
            };
          case var _ when sumObject > 1.5: { // 2
              if (voltage == "208") {
                phaseVA = Math.Round(7.5 * 208.0).ToString();
              }
              if (voltage == "480") {
                phaseVA = Math.Round(3.4 * 460.0).ToString();
              }
              break;
            };
          case var _ when sumObject > 1: { // 1 1/2
              if (voltage == "208") {
                phaseVA = Math.Round(6.6 * 208.0).ToString();
              }
              if (voltage == "480") {
                phaseVA = Math.Round(3.0 * 460.0).ToString();
              }
              break;
            };
          case var _ when sumObject > 0.75: { // 1
              if (voltage == "208") {
                phaseVA = Math.Round(4.6 * 208.0).ToString();
              }
              if (voltage == "480") {
                phaseVA = Math.Round(2.1 * 460.0).ToString();
              }
              break;
            };
          case var _ when sumObject > 0.5: { // 3/4
              if (voltage == "208") {
                phaseVA = Math.Round(3.5 * 208.0).ToString();
              }
              if (voltage == "480") {
                phaseVA = Math.Round(1.6 * 460.0).ToString();
              }
              break;
            };
          case var _ when sumObject <= 0.5: { // 1/2
              if (voltage == "208") {
                phaseVA = Math.Round(2.4 * 208.0).ToString();
              }
              if (voltage == "480") {
                phaseVA = Math.Round(1.1 * 460.0).ToString();
              }
              break;
            };
        }
      }
      Console.WriteLine($"phaseVA {phaseVA}");
      return phaseVA;
    }

    private void ConvertHpToVaBySide3Ph(string side) {
      int i = 0;
      while (i < PANEL_GRID.Rows.Count) {
        string phaseA = PANEL_GRID.Rows[i].Cells[$"phase_a_{side}"].Value?.ToString().ToUpper().Replace("\r", "");
        string phaseB = PANEL_GRID.Rows[i].Cells[$"phase_b_{side}"].Value?.ToString().ToUpper().Replace("\r", "");
        string phaseC = PANEL_GRID.Rows[i].Cells[$"phase_c_{side}"].Value?.ToString().ToUpper().Replace("\r", "");

        if (!String.IsNullOrEmpty(phaseA) && phaseA.EndsWith("HP")) {
          if (i + 1 < PANEL_GRID.Rows.Count && PANEL_GRID.Rows[i + 1].Cells[$"breaker_{side}"].Value as string == "2") {
            phaseA = ConvertHPtoVA(phaseA, 1, PHASE_VOLTAGE_COMBOBOX.SelectedItem as string);
            phaseB = phaseA;
            if (phaseA == "-1" || phaseB == "-1") {
              return;
            }
            // set values of PANEL_GRID phase_a_{side} and phase_b_{side}
            PANEL_GRID.Rows[i].Cells[$"phase_a_{side}"].Value = phaseA;
            PANEL_GRID.Rows[i + 1].Cells[$"phase_b_{side}"].Value = phaseB;
            i += 2;
          }
          else if (i + 2 < PANEL_GRID.Rows.Count && PANEL_GRID.Rows[i + 2].Cells[$"breaker_{side}"].Value as string == "3") {
            phaseA = ConvertHPtoVA(phaseA, 3, PHASE_VOLTAGE_COMBOBOX.SelectedItem as string);
            phaseB = phaseA;
            phaseC = phaseA;
            if (phaseA == "-1" || phaseB == "-1" || phaseC == "-1") {
              return;
            }
            // set values of PANEL_GRID phase_a_{side}, phase_b_{side}, phase_c_{side}
            PANEL_GRID.Rows[i].Cells[$"phase_a_{side}"].Value = phaseA;
            PANEL_GRID.Rows[i + 1].Cells[$"phase_b_{side}"].Value = phaseB;
            PANEL_GRID.Rows[i + 2].Cells[$"phase_c_{side}"].Value = phaseC;
            i += 3;
          }
          else {
            phaseA = ConvertHPtoVA(phaseA, 1, LINE_VOLTAGE_COMBOBOX.SelectedItem as string);
            if (phaseA == "-1") {
              return;
            }
            // set values of PANEL_GRID phase_a_{side}
            PANEL_GRID.Rows[i].Cells[$"phase_a_{side}"].Value = phaseA;
            i++;
          }
        }
        if (!String.IsNullOrEmpty(phaseB) && phaseB.EndsWith("HP")) {
          if (i + 1 < PANEL_GRID.Rows.Count && PANEL_GRID.Rows[i + 1].Cells[$"breaker_{side}"].Value as string == "2") {
            phaseB = ConvertHPtoVA(phaseB, 1, PHASE_VOLTAGE_COMBOBOX.SelectedItem as string);
            phaseC = phaseB;
            if (phaseB == "-1" || phaseC == "-1") {
              return;
            }
            // set values of PANEL_GRID phase_a_{side} and phase_b_{side}
            PANEL_GRID.Rows[i].Cells[$"phase_b_{side}"].Value = phaseB;
            PANEL_GRID.Rows[i + 1].Cells[$"phase_c_{side}"].Value = phaseC;
            i += 2;
          }
          else if (i + 2 < PANEL_GRID.Rows.Count && PANEL_GRID.Rows[i + 2].Cells[$"breaker_{side}"].Value as string == "3") {
            phaseB = ConvertHPtoVA(phaseB, 3, PHASE_VOLTAGE_COMBOBOX.SelectedItem as string);
            phaseC = phaseB;
            phaseA = phaseB;
            if (phaseB == "-1" || phaseC == "-1" || phaseA == "-1") {
              return;
            }
            // set values of PANEL_GRID phase_a_{side}, phase_b_{side}, phase_c_{side}
            PANEL_GRID.Rows[i].Cells[$"phase_b_{side}"].Value = phaseB;
            PANEL_GRID.Rows[i + 1].Cells[$"phase_c_{side}"].Value = phaseC;
            PANEL_GRID.Rows[i + 2].Cells[$"phase_a_{side}"].Value = phaseA;
            i += 3;
          }
          else {
            phaseB = ConvertHPtoVA(phaseB, 1, LINE_VOLTAGE_COMBOBOX.SelectedItem as string);
            if (phaseB == "-1") {
              return;
            }
            // set values of PANEL_GRID phase_a_{side}
            PANEL_GRID.Rows[i].Cells[$"phase_b_{side}"].Value = phaseB;
            i++;
          }
        }
        if (!String.IsNullOrEmpty(phaseC) && phaseC.EndsWith("HP")) {
          if (i + 1 < PANEL_GRID.Rows.Count && PANEL_GRID.Rows[i + 1].Cells[$"breaker_{side}"].Value as string == "2") {
            phaseC = ConvertHPtoVA(phaseC, 1, PHASE_VOLTAGE_COMBOBOX.SelectedItem as string);
            phaseA = phaseC;
            if (phaseC == "-1" || phaseA == "-1") {
              return;
            }
            // set values of PANEL_GRID phase_a_{side} and phase_b_{side}
            PANEL_GRID.Rows[i].Cells[$"phase_c_{side}"].Value = phaseC;
            PANEL_GRID.Rows[i + 1].Cells[$"phase_a_{side}"].Value = phaseA;
            i += 2;
          }
          else if (i + 2 < PANEL_GRID.Rows.Count && PANEL_GRID.Rows[i + 2].Cells[$"breaker_{side}"].Value as string == "3") {
            phaseC = ConvertHPtoVA(phaseC, 3, PHASE_VOLTAGE_COMBOBOX.SelectedItem as string);
            phaseA = phaseC;
            phaseB = phaseC;
            if (phaseC == "-1" || phaseA == "-1" || phaseB == "-1") {
              return;
            }
            // set values of PANEL_GRID phase_a_{side}, phase_b_{side}, phase_c_{side}
            PANEL_GRID.Rows[i].Cells[$"phase_c_{side}"].Value = phaseC;
            PANEL_GRID.Rows[i + 1].Cells[$"phase_a_{side}"].Value = phaseA;
            PANEL_GRID.Rows[i + 2].Cells[$"phase_b_{side}"].Value = phaseB;
            i += 3;
          }
          else {
            phaseC = ConvertHPtoVA(phaseC, 1, LINE_VOLTAGE_COMBOBOX.SelectedItem as string);
            if (phaseC == "-1") {
              return;
            }
            // set values of PANEL_GRID phase_a_{side}
            PANEL_GRID.Rows[i].Cells[$"phase_c_{side}"].Value = phaseC;
            i++;
          }
        }
        else {
          i += 1;
        }
      }

    }

    private void ConvertHpToVaBySide2Ph(string side) {
      int i = 0;
      while (i < PANEL_GRID.Rows.Count) {
        string phaseA = PANEL_GRID.Rows[i].Cells[$"phase_a_{side}"].Value?.ToString().ToUpper().Replace("\r", "");
        string phaseB = PANEL_GRID.Rows[i].Cells[$"phase_b_{side}"].Value?.ToString().ToUpper().Replace("\r", "");

        if (!String.IsNullOrEmpty(phaseA) && phaseA.EndsWith("HP")) {
          if (i + 1 < PANEL_GRID.Rows.Count && PANEL_GRID.Rows[i + 1].Cells[$"breaker_{side}"].Value as string == "2") {
            phaseA = ConvertHPtoVA(phaseA, 1, PHASE_VOLTAGE_COMBOBOX.SelectedItem as string);
            phaseB = phaseA;
            if (phaseA == "-1" || phaseB == "-1") {
              return;
            }
            // set values of PANEL_GRID phase_a_{side} and phase_b_{side}
            PANEL_GRID.Rows[i].Cells[$"phase_a_{side}"].Value = phaseA;
            PANEL_GRID.Rows[i + 1].Cells[$"phase_b_{side}"].Value = phaseB;
            i += 2;
          }
          else {
            phaseA = ConvertHPtoVA(phaseA, 1, LINE_VOLTAGE_COMBOBOX.SelectedItem as string);
            if (phaseA == "-1") {
              return;
            }
            // set values of PANEL_GRID phase_a_{side}
            PANEL_GRID.Rows[i].Cells[$"phase_a_{side}"].Value = phaseA;
            i++;
          }
        }
        if (!String.IsNullOrEmpty(phaseB) && phaseB.EndsWith("HP")) {
          if (i + 1 < PANEL_GRID.Rows.Count && PANEL_GRID.Rows[i + 1].Cells[$"breaker_{side}"].Value as string == "2") {
            phaseB = ConvertHPtoVA(phaseB, 1, PHASE_VOLTAGE_COMBOBOX.SelectedItem as string);
            phaseA = phaseB;
            if (phaseA == "-1" || phaseB == "-1") {
              return;
            }
            // set values of PANEL_GRID phase_a_{side} and phase_b_{side}
            PANEL_GRID.Rows[i].Cells[$"phase_b_{side}"].Value = phaseB;
            PANEL_GRID.Rows[i + 1].Cells[$"phase_a_{side}"].Value = phaseA;
            i += 2;
          }
          else {
            phaseB = ConvertHPtoVA(phaseB, 1, LINE_VOLTAGE_COMBOBOX.SelectedItem as string);
            if (phaseB == "-1") {
              return;
            }
            // set values of PANEL_GRID phase_a_{side}
            PANEL_GRID.Rows[i].Cells[$"phase_b_{side}"].Value = phaseB;
            i++;
          }
        }
        else {
          i += 1;
        }
      }

    }

    private void ConvertHpToVa_Click(object sender, EventArgs e) {
      if (PANEL_GRID.Columns["phase_c_left"] != null) {
        ConvertHpToVaBySide3Ph("left");
        ConvertHpToVaBySide3Ph("right");
      }
      else {
        ConvertHpToVaBySide2Ph("left");
        ConvertHpToVaBySide2Ph("right");
      }

    }
  }

  public class PanelItem {
    public string Description { get; set; }
    public double Wattage { get; set; }
    public int Poles { get; set; }
  }
}