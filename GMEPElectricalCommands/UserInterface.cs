﻿using Autodesk.AutoCAD.DatabaseServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.GraphicsSystem;
using OfficeOpenXml.Packaging.Ionic.Zlib;

namespace GMEPElectricalCommands
{
  public partial class UserInterface : UserControl
  {
    private MyCommands myCommandsInstance;
    private MainForm mainForm;
    private NEWPANELFORM newPanelForm;
    private noteForm notesForm;
    private List<string> notesStorage = new List<string>();

    private bool initialization;
    private object oldValue;

    public UserInterface(MyCommands myCommands, MainForm mainForm, NEWPANELFORM newPanelForm, string tabName, bool is3PH = false)
    {
      InitializeComponent();
      myCommandsInstance = myCommands;
      this.mainForm = mainForm;
      this.newPanelForm = newPanelForm;
      this.initialization = false;
      this.Name = tabName;
      this.notesStorage =
      [
        "ADD SUFFIX (E). * NOT ADDED AS NOTE *",
        "ADD SUFFIX (R). * NOT ADDED AS NOTE *",
        "APPLY LCL LOAD REDUCTION (USE 80 % OF THE MCA LOAD). * NOT ADDED AS NOTE *",
      ];

      INFO_LABEL.Text = "";

      listen_for_new_rows();
      add_or_remove_panel_grid_columns(is3PH);
      remove_column_header_sorting();

      change_size_of_phase_columns(is3PH);
      add_phase_sum_column(is3PH);

      PANEL_GRID.Rows.AddCopies(0, 21);
      PANEL_GRID.AllowUserToAddRows = false;
      PANEL_GRID.KeyDown += new KeyEventHandler(this.PANEL_GRID_KeyDown);
      PANEL_GRID.CellBeginEdit += new DataGridViewCellCancelEventHandler(this.PANEL_GRID_CellBeginEdit);
      PANEL_GRID.CellValueChanged += new DataGridViewCellEventHandler(this.PANEL_GRID_CellValueChanged);
      PHASE_SUM_GRID.CellValueChanged += new DataGridViewCellEventHandler(this.PHASE_SUM_GRID_CellValueChanged);
      PANEL_NAME_INPUT.TextChanged += new EventHandler(this.PANEL_NAME_INPUT_TextChanged);
      PANEL_GRID.CellFormatting += PANEL_GRID_CellFormatting;
      PANEL_GRID.CellClick += new DataGridViewCellEventHandler(this.PANEL_GRID_CellClick);

      add_rows_to_datagrid();
      set_default_form_values(tabName);
      deselect_cells();

      this.initialization = true;
    }

    public List<string> getNotesStorage()
    {
      return this.notesStorage;
    }

    private void add_rows_to_datagrid()
    {
      // Datagrids
      PHASE_SUM_GRID.Rows.Add("0", "0");
      TOTAL_VA_GRID.Rows.Add("0");
      LCL_GRID.Rows.Add("0", "0");
      TOTAL_OTHER_LOAD_GRID.Rows.Add("0");
      PANEL_LOAD_GRID.Rows.Add("0");
      FEEDER_AMP_GRID.Rows.Add("0");
    }

    private void deselect_cells()
    {
      PHASE_SUM_GRID.DefaultCellStyle.SelectionBackColor = PHASE_SUM_GRID.DefaultCellStyle.BackColor;
      PHASE_SUM_GRID.DefaultCellStyle.SelectionForeColor = PHASE_SUM_GRID.DefaultCellStyle.ForeColor;
      TOTAL_VA_GRID.DefaultCellStyle.SelectionBackColor = TOTAL_VA_GRID.DefaultCellStyle.BackColor;
      TOTAL_VA_GRID.DefaultCellStyle.SelectionForeColor = TOTAL_VA_GRID.DefaultCellStyle.ForeColor;
      LCL_GRID.DefaultCellStyle.SelectionBackColor = LCL_GRID.DefaultCellStyle.BackColor;
      LCL_GRID.DefaultCellStyle.SelectionForeColor = LCL_GRID.DefaultCellStyle.ForeColor;
      TOTAL_OTHER_LOAD_GRID.DefaultCellStyle.SelectionBackColor = TOTAL_OTHER_LOAD_GRID.DefaultCellStyle.BackColor;
      TOTAL_OTHER_LOAD_GRID.DefaultCellStyle.SelectionForeColor = TOTAL_OTHER_LOAD_GRID.DefaultCellStyle.ForeColor;
      PANEL_LOAD_GRID.DefaultCellStyle.SelectionBackColor = PANEL_LOAD_GRID.DefaultCellStyle.BackColor;
      PANEL_LOAD_GRID.DefaultCellStyle.SelectionForeColor = PANEL_LOAD_GRID.DefaultCellStyle.ForeColor;
      FEEDER_AMP_GRID.DefaultCellStyle.SelectionBackColor = FEEDER_AMP_GRID.DefaultCellStyle.BackColor;
      FEEDER_AMP_GRID.DefaultCellStyle.SelectionForeColor = FEEDER_AMP_GRID.DefaultCellStyle.ForeColor;
      PANEL_GRID.ClearSelection();
    }

    private void set_default_form_values(string tabName)
    {
      // Textboxes
      PANEL_NAME_INPUT.Text = tabName;
      PANEL_LOCATION_INPUT.Text = "ELECTRIC ROOM";
      MAIN_INPUT.Text = "M.L.O.";
      BUS_RATING_INPUT.Text = "100";

      // Comboboxes
      STATUS_COMBOBOX.SelectedIndex = 0;
      MOUNTING_COMBOBOX.SelectedIndex = 0;
      if (PHASE_SUM_GRID.ColumnCount > 2)
      {
        WIRE_COMBOBOX.SelectedIndex = 1;
        PHASE_COMBOBOX.SelectedIndex = 1;
      }
      else
      {
        WIRE_COMBOBOX.SelectedIndex = 0;
        PHASE_COMBOBOX.SelectedIndex = 0;
      }
      PHASE_VOLTAGE_COMBOBOX.SelectedIndex = 0;
      LINE_VOLTAGE_COMBOBOX.SelectedIndex = 0;

      // Datagrids
      PHASE_SUM_GRID.Rows[0].Cells[0].Value = "0";
      PHASE_SUM_GRID.Rows[0].Cells[1].Value = "0";
      TOTAL_VA_GRID.Rows[0].Cells[0].Value = "0";
      LCL_GRID.Rows[0].Cells[0].Value = "0";
      LCL_GRID.Rows[0].Cells[1].Value = "0";
      TOTAL_OTHER_LOAD_GRID.Rows[0].Cells[0].Value = "0";
      PANEL_LOAD_GRID.Rows[0].Cells[0].Value = "0";
      FEEDER_AMP_GRID.Rows[0].Cells[0].Value = "0";

      if (PHASE_SUM_GRID.ColumnCount > 2) PHASE_SUM_GRID.Rows[0].Cells[2].Value = "0";
    }

    private void remove_column_header_sorting()
    {
      foreach (DataGridViewColumn column in PANEL_GRID.Columns)
      {
        column.SortMode = DataGridViewColumnSortMode.NotSortable;
      }
    }

    private void listen_for_new_rows()
    {
      PANEL_GRID.RowsAdded += new DataGridViewRowsAddedEventHandler(PANEL_GRID_RowsAdded);
    }

    public Dictionary<string, object> retrieve_data_from_modal()
    {
      // Create a new panel
      Dictionary<string, object> panel = new Dictionary<string, object>();

      // Get the value from the main input
      string mainInput = MAIN_INPUT.Text.ToLower();

      if (!mainInput.Contains("mlo") && !mainInput.Contains("m.l.o") && !mainInput.Contains("m.l.o."))
      {
        if (mainInput.Contains("amp"))
        {
          mainInput = mainInput.Replace("amp", "AMP");
        }
        else if (mainInput.Contains("a"))
        {
          mainInput = mainInput.Replace("a", "AMP");
        }
        else if (mainInput.Contains(" "))
        {
          mainInput = mainInput.Replace(" ", " AMP");
        }
        else
        {
          mainInput += " AMP";
        }
      }

      panel.Add("main", mainInput.ToUpper());

      string GetComboBoxValue(ComboBox comboBox)
      {
        if (comboBox.SelectedItem != null)
        {
          return comboBox.SelectedItem.ToString().ToUpper();
        }
        else if (!string.IsNullOrEmpty(comboBox.Text))
        {
          return comboBox.Text.ToUpper();
        }
        else
        {
          return ""; // Default value or you can return null
        }
      }

      // Add simple values in uppercase
      panel.Add("panel", "'" + PANEL_NAME_INPUT.Text.ToUpper() + "'");
      panel.Add("location", PANEL_LOCATION_INPUT.Text.ToUpper());
      panel.Add("voltage1", GetComboBoxValue(LINE_VOLTAGE_COMBOBOX));
      panel.Add("voltage2", GetComboBoxValue(PHASE_VOLTAGE_COMBOBOX));
      panel.Add("phase", GetComboBoxValue(PHASE_COMBOBOX));
      panel.Add("wire", GetComboBoxValue(WIRE_COMBOBOX));
      panel.Add("mounting", GetComboBoxValue(MOUNTING_COMBOBOX));
      panel.Add("existing", GetComboBoxValue(STATUS_COMBOBOX));

      panel.Add("subtotal_a", PHASE_SUM_GRID.Rows[0].Cells[0].Value.ToString().ToUpper());
      panel.Add("subtotal_b", PHASE_SUM_GRID.Rows[0].Cells[1].Value.ToString().ToUpper());

      if (PHASE_SUM_GRID.Columns.Count > 2)
      {
        panel.Add("subtotal_c", PHASE_SUM_GRID.Rows[0].Cells[2].Value.ToString().ToUpper());
      }
      else
      {
        panel.Add("subtotal_c", "0");
      }
      panel.Add("total_va", TOTAL_VA_GRID.Rows[0].Cells[0].Value.ToString().ToUpper());
      panel.Add("lcl", LCL_GRID.Rows[0].Cells[1].Value.ToString().ToUpper());
      panel.Add("lcl_125", LCL_GRID.Rows[0].Cells[0].Value.ToString().ToUpper());
      panel.Add("total_other_load", TOTAL_OTHER_LOAD_GRID.Rows[0].Cells[0].Value.ToString().ToUpper());
      panel.Add("kva", PANEL_LOAD_GRID.Rows[0].Cells[0].Value.ToString().ToUpper());
      panel.Add("feeder_amps", FEEDER_AMP_GRID.Rows[0].Cells[0].Value.ToString().ToUpper());
      panel.Add("custom_title", CUSTOM_TITLE_TEXT.Text.ToUpper());

      // Add "A" to the bus rating value if it consists of digits only, then convert to uppercase
      string busRatingInput = BUS_RATING_INPUT.Text.ToLower();

      if (busRatingInput.Contains("amp"))
      {
        busRatingInput = busRatingInput.Replace("amp", "A");
      }
      else if (busRatingInput.Contains("a"))
      {
        busRatingInput = busRatingInput.Replace("a", "A");
      }
      else if (busRatingInput.Contains(" "))
      {
        busRatingInput = busRatingInput.Replace(" ", " A");
      }
      else
      {
        busRatingInput += " A";
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

      for (int i = 0; i < PANEL_GRID.Rows.Count; i++)
      {
        string descriptionLeftValue = "";
        if (PANEL_GRID.Rows[i].Cells["description_left"].Value == null && PANEL_GRID.Rows[i].Cells["breaker_left"].Value != null)
        {
          descriptionLeftValue = "SPARE";
        }
        else
        {
          descriptionLeftValue = PANEL_GRID.Rows[i].Cells["description_left"].Value?.ToString().ToUpper() ?? "SPACE";
        }
        string descriptionRightValue = "";
        if (PANEL_GRID.Rows[i].Cells["description_right"].Value == null && PANEL_GRID.Rows[i].Cells["breaker_right"].Value != null)
        {
          descriptionRightValue = "SPARE";
        }
        else
        {
          descriptionRightValue = PANEL_GRID.Rows[i].Cells["description_right"].Value?.ToString().ToUpper() ?? "SPACE";
        }
        string breakerLeftValue = PANEL_GRID.Rows[i].Cells["breaker_left"].Value?.ToString().ToUpper() ?? "";
        string breakerRightValue = PANEL_GRID.Rows[i].Cells["breaker_right"].Value?.ToString().ToUpper() ?? "";
        string circuitRightValue = PANEL_GRID.Rows[i].Cells["circuit_right"].Value?.ToString().ToUpper() ?? "";
        string circuitLeftValue = PANEL_GRID.Rows[i].Cells["circuit_left"].Value?.ToString().ToUpper() ?? "";
        string phaseALeftValue = PANEL_GRID.Rows[i].Cells["phase_a_left"].Value?.ToString() ?? "0";
        string phaseALeftTag = PANEL_GRID.Rows[i].Cells["phase_a_left"].Tag?.ToString() ?? "";
        string phaseBLeftValue = PANEL_GRID.Rows[i].Cells["phase_b_left"].Value?.ToString() ?? "0";
        string phaseBLeftTag = PANEL_GRID.Rows[i].Cells["phase_b_left"].Tag?.ToString() ?? "";
        string phaseARightValue = PANEL_GRID.Rows[i].Cells["phase_a_right"].Value?.ToString() ?? "0";
        string phaseARightTag = PANEL_GRID.Rows[i].Cells["phase_a_right"].Tag?.ToString() ?? "";
        string phaseBRightValue = PANEL_GRID.Rows[i].Cells["phase_b_right"].Value?.ToString() ?? "0";
        string phaseBRightTag = PANEL_GRID.Rows[i].Cells["phase_b_right"].Tag?.ToString() ?? "";
        string phaseCLeftValue = "0";
        string phaseCRightValue = "0";
        string phaseCLeftTag = "";
        string phaseCRightTag = "";

        string descriptionLeftTag = PANEL_GRID.Rows[i].Cells["description_left"].Tag?.ToString() ?? "";
        string descriptionRightTag = PANEL_GRID.Rows[i].Cells["description_right"].Tag?.ToString() ?? "";

        if (PHASE_SUM_GRID.Columns.Count > 2)
        {
          phaseCLeftValue = PANEL_GRID.Rows[i].Cells["phase_c_left"].Value?.ToString() ?? "0";
          phaseCLeftTag = PANEL_GRID.Rows[i].Cells["phase_c_left"].Tag?.ToString() ?? "";
          phaseCRightValue = PANEL_GRID.Rows[i].Cells["phase_c_right"].Value?.ToString() ?? "0";
          phaseCRightTag = PANEL_GRID.Rows[i].Cells["phase_c_right"].Tag?.ToString() ?? "";
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
        bool hasCommaInPhaseLeft = phaseALeftValue.Contains(",") || phaseBLeftValue.Contains(",") || phaseCLeftValue.Contains(",");
        bool shouldDuplicateLeft = hasCommaInPhaseLeft;

        // Checks for Right Side
        bool hasCommaInPhaseRight = phaseARightValue.Contains(",") || phaseBRightValue.Contains(",") || phaseCRightValue.Contains(",");
        bool shouldDuplicateRight = hasCommaInPhaseRight;

        // Handling Phase A Left
        if (phaseALeftValue.Contains(","))
        {
          var splitValues = phaseALeftValue.Split(',').Select(str => str.Trim()).ToArray();
          phase_a_left.AddRange(splitValues);
        }
        else
        {
          phase_a_left.Add(phaseALeftValue);
          phase_a_left.Add("0"); // Default value
        }

        // Handling Phase B Left
        if (phaseBLeftValue.Contains(","))
        {
          var splitValues = phaseBLeftValue.Split(',').Select(str => str.Trim()).ToArray();
          phase_b_left.AddRange(splitValues);
        }
        else
        {
          phase_b_left.Add(phaseBLeftValue);
          phase_b_left.Add("0"); // Default value
        }

        // Handling Phase A Right
        if (phaseARightValue.Contains(","))
        {
          var splitValues = phaseARightValue.Split(',').Select(str => str.Trim()).ToArray();
          phase_a_right.AddRange(splitValues);
        }
        else
        {
          phase_a_right.Add(phaseARightValue);
          phase_a_right.Add("0"); // Default value
        }

        // Handling Phase B Right
        if (phaseBRightValue.Contains(","))
        {
          var splitValues = phaseBRightValue.Split(',').Select(str => str.Trim()).ToArray();
          phase_b_right.AddRange(splitValues);
        }
        else
        {
          phase_b_right.Add(phaseBRightValue);
          phase_b_right.Add("0"); // Default value
        }

        if (PHASE_SUM_GRID.Columns.Count > 2)
        {
          // Handling Phase C Left
          if (phaseCLeftValue.Contains(","))
          {
            var splitValues = phaseCLeftValue.Split(',').Select(str => str.Trim()).ToArray();
            phase_c_left.AddRange(splitValues);
          }
          else
          {
            phase_c_left.Add(phaseCLeftValue);
            phase_c_left.Add("0"); // Default value
          }

          // Handling Phase C Right
          if (phaseCRightValue.Contains(","))
          {
            var splitValues = phaseCRightValue.Split(',').Select(str => str.Trim()).ToArray();
            phase_c_right.AddRange(splitValues);
          }
          else
          {
            phase_c_right.Add(phaseCRightValue);
            phase_c_right.Add("0"); // Default value
          }
        }

        if (descriptionLeftValue.Contains(","))
        {
          // If it contains a comma, split and add both values
          var splitValues = descriptionLeftValue.Split(',')
                                                .Select(str => str.Trim())
                                                .ToArray();
          description_left.AddRange(splitValues);
          circuit_left.Add(circuitLeftValue + "A");
          circuit_left.Add(circuitLeftValue + "B");
        }
        else
        {
          description_left.Add(descriptionLeftValue);
          description_left.Add(shouldDuplicateLeft ? descriptionLeftValue : "SPACE");

          if (shouldDuplicateLeft)
          {
            circuit_left.Add(circuitLeftValue + "A");
            circuit_left.Add(circuitLeftValue + "B");
          }
          else
          {
            circuit_left.Add(circuitLeftValue);
            circuit_left.Add("");
          }
        }

        if (breakerLeftValue.Contains(","))
        {
          // If it contains a comma, split and add both values
          var splitValues = breakerLeftValue.Split(',')
                                                .Select(str => str.Trim())
                                                .ToArray();
          breaker_left.AddRange(splitValues);
        }
        else
        {
          breaker_left.Add(breakerLeftValue);
          breaker_left.Add(shouldDuplicateLeft ? breakerLeftValue : "");
        }

        if (descriptionRightValue.Contains(","))
        {
          // If it contains a comma, split and add both values
          var splitValues = descriptionRightValue.Split(',')
                                                .Select(str => str.Trim())
                                                .ToArray();
          description_right.AddRange(splitValues);
          circuit_right.Add(circuitRightValue + "A");
          circuit_right.Add(circuitRightValue + "B");
        }
        else
        {
          description_right.Add(descriptionRightValue);
          description_right.Add(shouldDuplicateRight ? descriptionRightValue : "SPACE");

          if (shouldDuplicateRight)
          {
            circuit_right.Add(circuitRightValue + "A");
            circuit_right.Add(circuitRightValue + "B");
          }
          else
          {
            circuit_right.Add(circuitRightValue);
            circuit_right.Add("");
          }
        }

        if (breakerRightValue.Contains(","))
        {
          // If it contains a comma, split and add both values
          var splitValues = breakerRightValue.Split(',')
                                                .Select(str => str.Trim())
                                                .ToArray();
          breaker_right.AddRange(splitValues);
        }
        else
        {
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

      if (PHASE_SUM_GRID.Columns.Count > 2)
      {
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

      if (PHASE_SUM_GRID.Columns.Count > 2)
      {
        panel.Add("phase_c_left_tag", phase_c_left_tag);
        panel.Add("phase_c_right_tag", phase_c_right_tag);
      }

      panel.Add("description_left_tags", description_left_tags);
      panel.Add("description_right_tags", description_right_tags);

      panel.Add("notes", notesStorage);

      return panel;
    }

    private void calculate_totalva_panelload_feederamps_lcl(double sum)
    {
      // Update total VA
      TOTAL_VA_GRID.Rows[0].Cells[0].Value = Math.Round(sum, 0); // Rounded to 0 decimal places

      // Update panel load grid including the other load value
      double otherLoad = Convert.ToDouble(TOTAL_OTHER_LOAD_GRID[0, 0].Value);
      PANEL_LOAD_GRID.Rows[0].Cells[0].Value = Math.Round((otherLoad + sum) / 1000, 1); // Rounded to 1 decimal place

      // Update feeder amps
      double panelLoadValue = Convert.ToDouble(Convert.ToDouble(TOTAL_VA_GRID.Rows[0].Cells[0].Value) + otherLoad) / (120 * PHASE_SUM_GRID.ColumnCount);
      FEEDER_AMP_GRID.Rows[0].Cells[0].Value = Math.Round(panelLoadValue, 1); // Rounded to 1 decimal place
    }

    private void calculate_totalva_panelload_feederamps_regular(double value1, double value2, double value3, double sum)
    {
      // Update total VA and panel load without other loads
      TOTAL_VA_GRID.Rows[0].Cells[0].Value = Math.Round(sum, 0); // Rounded to 1 decimal place
      PANEL_LOAD_GRID.Rows[0].Cells[0].Value = Math.Round(sum / 1000, 1); // Rounded to 1 decimal place

      // Update feeder amps
      double maxVal = Math.Max(Math.Max(Convert.ToDouble(value1), Convert.ToDouble(value2)), Convert.ToDouble(value3));

      object lineVoltageObj = LINE_VOLTAGE_COMBOBOX.SelectedItem;
      if (lineVoltageObj != null)
      {
        double lineVoltage = Convert.ToDouble(lineVoltageObj);
        if (lineVoltage != 0)
        {
          double panelLoadValue = maxVal / lineVoltage;
          FEEDER_AMP_GRID.Rows[0].Cells[0].Value = Math.Round(panelLoadValue, 1); // Rounded to 1 decimal place
        }
      }
    }

    private void panel_cell_changed_3P(DataGridViewCellEventArgs e)
    {
      // Function to parse and sum a cell's value
      double ParseAndSumCell(string cellValue)
      {
        double sum = 0;
        if (!string.IsNullOrEmpty(cellValue))
        {
          var parts = cellValue.Split(',');
          foreach (var part in parts)
          {
            if (double.TryParse(part, out double value))
            {
              sum += value;
            }
          }
        }
        return sum;
      }

      if (e.ColumnIndex == 1 || e.ColumnIndex == 8)
      {
        double sum = 0;
        foreach (DataGridViewRow row in PANEL_GRID.Rows)
        {
          if (row.Cells[1].Value != null)
            sum += ParseAndSumCell(row.Cells[1].Value.ToString());

          if (row.Cells[8].Value != null)
            sum += ParseAndSumCell(row.Cells[8].Value.ToString());
        }

        // Update the sum in dataGridView2, row 0, column 0
        PHASE_SUM_GRID.Rows[0].Cells[0].Value = sum;
      }

      if (e.ColumnIndex == 2 || e.ColumnIndex == 9)
      {
        double sum = 0;
        foreach (DataGridViewRow row in PANEL_GRID.Rows)
        {
          if (row.Cells[2].Value != null)
            sum += ParseAndSumCell(row.Cells[2].Value.ToString());

          if (row.Cells[9].Value != null)
            sum += ParseAndSumCell(row.Cells[9].Value.ToString());
        }

        // Update the sum in dataGridView2, row 0, column 1
        PHASE_SUM_GRID.Rows[0].Cells[1].Value = sum;
      }

      if (e.ColumnIndex == 3 || e.ColumnIndex == 10)
      {
        double sum = 0;
        foreach (DataGridViewRow row in PANEL_GRID.Rows)
        {
          if (row.Cells[3].Value != null)
            sum += ParseAndSumCell(row.Cells[3].Value.ToString());

          if (row.Cells[10].Value != null)
            sum += ParseAndSumCell(row.Cells[10].Value.ToString());
        }

        // Update the sum in dataGridView2, row 0, column 2
        PHASE_SUM_GRID.Rows[0].Cells[2].Value = sum;
      }

      // Capture the new value for potential further use
      object newValue = PANEL_GRID.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
    }

    private void panel_cell_changed_2P(DataGridViewCellEventArgs e)
    {
      // Function to parse and sum a cell's value
      double ParseAndSumCell(string cellValue)
      {
        double sum = 0;
        if (!string.IsNullOrEmpty(cellValue))
        {
          var parts = cellValue.Split(',');
          foreach (var part in parts)
          {
            if (double.TryParse(part, out double value))
            {
              sum += value;
            }
          }
        }
        return sum;
      }

      // Check if the modified cell is in column 2 or 8
      if (e.ColumnIndex == 1 || e.ColumnIndex == 7)
      {
        double sum = 0;
        foreach (DataGridViewRow row in PANEL_GRID.Rows)
        {
          if (row.Cells[1].Value != null)
            sum += ParseAndSumCell(row.Cells[1].Value.ToString());

          if (row.Cells[7].Value != null)
            sum += ParseAndSumCell(row.Cells[7].Value.ToString());
        }

        // Update the sum in dataGridView2, row 0, column 0
        PHASE_SUM_GRID.Rows[0].Cells[0].Value = sum;
      }

      // Check if the modified cell is in column 3 or 9
      if (e.ColumnIndex == 2 || e.ColumnIndex == 8)
      {
        double sum = 0;
        foreach (DataGridViewRow row in PANEL_GRID.Rows)
        {
          if (row.Cells[2].Value != null)
            sum += ParseAndSumCell(row.Cells[2].Value.ToString());

          if (row.Cells[8].Value != null)
            sum += ParseAndSumCell(row.Cells[8].Value.ToString());
        }

        // Update the sum in dataGridView2, row 0, column 1
        PHASE_SUM_GRID.Rows[0].Cells[1].Value = sum;
      }

      object newValue = PANEL_GRID.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
    }

    private void listen_for_3P_rows_added(DataGridViewRowsAddedEventArgs e)
    {
      Color grayColor = Color.LightGray;

      for (int i = 0; i < e.RowCount; i++)
      {
        int rowIndex = e.RowIndex + i;

        // Set common column values
        PANEL_GRID.Rows[rowIndex].Cells[0].Value = "SPARE";
        PANEL_GRID.Rows[rowIndex].Cells[4].Value = "20";
        PANEL_GRID.Rows[rowIndex].Cells[5].Value = ((rowIndex + 1) * 2) - 1;
        PANEL_GRID.Rows[rowIndex].Cells[6].Value = (rowIndex + 1) * 2;
        PANEL_GRID.Rows[rowIndex].Cells[7].Value = "20";
        PANEL_GRID.Rows[rowIndex].Cells[11].Value = "SPARE";

        // Determine the row pattern (zig-zag) for gray background
        int pattern = rowIndex % 3;

        // Apply pattern for two sets of columns based on the row pattern
        if (pattern == 0)
        {
          PANEL_GRID.Rows[rowIndex].Cells[1].Style.BackColor = grayColor;
          PANEL_GRID.Rows[rowIndex].Cells[8].Style.BackColor = grayColor;
        }
        else if (pattern == 1)
        {
          PANEL_GRID.Rows[rowIndex].Cells[2].Style.BackColor = grayColor;
          PANEL_GRID.Rows[rowIndex].Cells[9].Style.BackColor = grayColor;
        }
        else
        {
          PANEL_GRID.Rows[rowIndex].Cells[3].Style.BackColor = grayColor;
          PANEL_GRID.Rows[rowIndex].Cells[10].Style.BackColor = grayColor;
        }
      }
    }

    private void listen_for_2P_rows_added(DataGridViewRowsAddedEventArgs e)
    {
      for (int i = 0; i < e.RowCount; i++)
      {
        int rowIndex = e.RowIndex + i;

        // Set common column values
        PANEL_GRID.Rows[rowIndex].Cells[0].Value = "SPARE";
        PANEL_GRID.Rows[rowIndex].Cells[3].Value = "20";
        PANEL_GRID.Rows[rowIndex].Cells[4].Value = ((rowIndex + 1) * 2) - 1;
        PANEL_GRID.Rows[rowIndex].Cells[5].Value = (rowIndex + 1) * 2;
        PANEL_GRID.Rows[rowIndex].Cells[6].Value = "20";
        PANEL_GRID.Rows[rowIndex].Cells[9].Value = "SPARE";

        // Zig-zag pattern for columns 2, 3, 8, and 9
        if ((rowIndex + 1) % 2 == 1) // Odd rows
        {
          PANEL_GRID.Rows[rowIndex].Cells[1].Style.BackColor = Color.LightGray; // Column 2
          PANEL_GRID.Rows[rowIndex].Cells[7].Style.BackColor = Color.LightGray; // Column 8
        }
        else // Even rows
        {
          PANEL_GRID.Rows[rowIndex].Cells[2].Style.BackColor = Color.LightGray; // Column 3
          PANEL_GRID.Rows[rowIndex].Cells[8].Style.BackColor = Color.LightGray; // Column 9
        }
      }
    }

    private void calculate_lcl_otherload_panelload_feederamps()
    {
      if (LARGEST_LCL_CHECKBOX.Checked)
      {
        // 1. Check if the textbox is empty
        string largestLclInputText = LARGEST_LCL_INPUT.Text;
        if (string.IsNullOrEmpty(largestLclInputText))
        {
          return;
        }

        // 2. If it has a number, put that number in col 0 row 0 of "LCL_GRID".
        double largestLclInputValue;
        if (double.TryParse(largestLclInputText, out largestLclInputValue))
        {
          LCL_GRID[0, 0].Value = largestLclInputValue;

          // 3. Multiply by 125% and put that in col 1 row 0 of "LCL_GRID".
          double value125 = largestLclInputValue * 1.25;
          LCL_GRID[1, 0].Value = value125;

          // 4. Subtract value in col 1 row 0 from value in col 0 row 0 of "LCL_GRID".
          double difference = value125 - largestLclInputValue;

          // 5. Put that number in col 0 row 0 of "TOTAL_OTHER_LOAD_GRID".
          TOTAL_OTHER_LOAD_GRID[0, 0].Value = difference;

          // 6. Add to the value in "PANEL_LOAD_GRID".
          double panelLoad = Convert.ToDouble(TOTAL_VA_GRID[0, 0].Value);
          double total_KVA = (panelLoad + difference) / 1000;
          PANEL_LOAD_GRID[0, 0].Value = Math.Round(total_KVA, 1);

          // 7. Divide by value in "PHASE_VOLTAGE_COMBOBOX".
          double lineVoltage = Convert.ToDouble(LINE_VOLTAGE_COMBOBOX.SelectedItem);
          if (lineVoltage != 0)
          {
            double result = (panelLoad + difference) / (lineVoltage * 2);
            FEEDER_AMP_GRID[0, 0].Value = Math.Round(result, 1);
          }
        }
        else
        {
          MessageBox.Show("Invalid number format in LARGEST_LCL_INPUT.");
        }
      }
      else
      {
        LCL_GRID[0, 0].Value = "0";
        LCL_GRID[1, 0].Value = "0";
        TOTAL_OTHER_LOAD_GRID[0, 0].Value = "0";

        // Retrieve the values from PHASE_SUM_GRID, row 0, column 0 and 1
        double value1 = Convert.ToDouble(PHASE_SUM_GRID.Rows[0].Cells[0].Value);
        double value2 = Convert.ToDouble(PHASE_SUM_GRID.Rows[0].Cells[1].Value);
        double value3 = 0;

        if (PHASE_SUM_GRID.ColumnCount > 2)
        {
          value3 = Convert.ToDouble(PHASE_SUM_GRID.Rows[0].Cells[2].Value);
        }

        // Perform the sum (checking for null values)
        double sum = value1 + value2 + value3;

        calculate_totalva_panelload_feederamps_regular(value1, value2, value3, sum);
      }
    }

    public void clear_and_set_modal_values(Dictionary<string, object> selectedPanelData)
    {
      clear_current_modal_data();
      remove_rows();

      // add the number of rows based on the number of rows in the selected panel data
      int numberOfRows = ((Newtonsoft.Json.Linq.JArray)selectedPanelData["description_left"]).ToObject<List<string>>().Count / 2;
      PANEL_GRID.Rows.Add(numberOfRows);

      populate_modal_with_panel_data(selectedPanelData);
    }

    internal DataGridView retrieve_panelGrid()
    {
      return PANEL_GRID;
    }

    private void remove_rows()
    {
      // remove rows
      while (PANEL_GRID.Rows.Count >= 1)
      {
        PANEL_GRID.Rows.RemoveAt(0);
      }
    }

    private void populate_modal_with_panel_data(Dictionary<string, object> selectedPanelData)
    {
      // Set TextBoxes
      MAIN_INPUT.Text = selectedPanelData["main"].ToString().Replace("AMP", "").Replace("A", "").Replace(" ", "");
      PANEL_NAME_INPUT.Text = selectedPanelData["panel"].ToString().Replace("'", "");
      PANEL_LOCATION_INPUT.Text = selectedPanelData["location"].ToString();
      BUS_RATING_INPUT.Text = selectedPanelData["bus_rating"].ToString().Replace("AMP", "").Replace("A", "").Replace(" ", "");

      // Set ComboBoxes
      STATUS_COMBOBOX.SelectedItem = selectedPanelData["existing"].ToString();
      MOUNTING_COMBOBOX.SelectedItem = selectedPanelData["mounting"].ToString();
      WIRE_COMBOBOX.SelectedItem = selectedPanelData["wire"].ToString();
      PHASE_COMBOBOX.SelectedItem = selectedPanelData["phase"].ToString();
      PHASE_VOLTAGE_COMBOBOX.SelectedItem = selectedPanelData["voltage2"].ToString();
      LINE_VOLTAGE_COMBOBOX.SelectedItem = selectedPanelData["voltage1"].ToString();

      // Set DataGridViews
      PHASE_SUM_GRID.Rows[0].Cells[0].Value = selectedPanelData["subtotal_a"].ToString();
      PHASE_SUM_GRID.Rows[0].Cells[1].Value = selectedPanelData["subtotal_b"].ToString();
      TOTAL_VA_GRID.Rows[0].Cells[0].Value = selectedPanelData["total_va"].ToString();
      LCL_GRID.Rows[0].Cells[0].Value = selectedPanelData["lcl_125"].ToString();
      LCL_GRID.Rows[0].Cells[1].Value = selectedPanelData["lcl"].ToString();
      TOTAL_OTHER_LOAD_GRID.Rows[0].Cells[0].Value = selectedPanelData["total_other_load"].ToString();
      PANEL_LOAD_GRID.Rows[0].Cells[0].Value = selectedPanelData["kva"].ToString();
      FEEDER_AMP_GRID.Rows[0].Cells[0].Value = selectedPanelData["feeder_amps"].ToString();

      // Set Custom Title if it exists
      if (selectedPanelData.ContainsKey("custom_title"))
      {
        CUSTOM_TITLE_TEXT.Text = selectedPanelData["custom_title"].ToString();
      }

      List<string> multi_row_datagrid_keys = new List<string> { "description_left", "description_right", "phase_a_left", "phase_b_left", "phase_a_right", "phase_b_right", "breaker_left", "breaker_right", "circuit_left", "circuit_right" };

      // check if the panel is three phase and if so add the third phase to the list of keys
      if (selectedPanelData["phase"].ToString() == "3")
      {
        multi_row_datagrid_keys.AddRange(new List<string> { "phase_c_left", "phase_c_right" });
      }

      int length = ((Newtonsoft.Json.Linq.JArray)selectedPanelData["description_left"]).ToObject<List<string>>().Count;

      for (int i = 0; i < length * 2; i += 2)
      {
        foreach (string key in multi_row_datagrid_keys)
        {
          if (selectedPanelData[key] is Newtonsoft.Json.Linq.JArray)
          {
            List<string> values = ((Newtonsoft.Json.Linq.JArray)selectedPanelData[key]).ToObject<List<string>>();

            if (i < values.Count)
            {
              string currentValue = values[i];
              string nextValue = i + 1 < values.Count ? values[i + 1] : null;

              if (key.Contains("description") && currentValue == "SPACE")
              {
                currentValue = string.Empty;
              }

              if (key.Contains("phase") && currentValue == "0")
              {
                continue; // skip this iteration if the value is "0" for phases
              }

              if (nextValue != null)
              {
                if (key.Contains("phase") && nextValue != "0")
                {
                  currentValue = $"{currentValue},{nextValue}";
                }
                else if (key.Contains("description") && nextValue != "SPACE")
                {
                  currentValue = $"{currentValue},{nextValue}";
                }
                else if (key.Contains("circuit"))
                {
                  currentValue = currentValue.Replace("A", "");
                }
                else if (key.Contains("breaker") && nextValue != "")
                {
                  currentValue = $"{currentValue},{nextValue}";
                }
              }

              PANEL_GRID.Rows[i / 2].Cells[key].Value = currentValue;
            }
          }
          else
          {
            // Log or handle the unexpected type
            Console.WriteLine($"Warning: Value for key {key} is not a JArray");
          }
        }
      }
    }

    private void clear_current_modal_data()
    {
      // Clear TextBoxes
      BUS_RATING_INPUT.Text = string.Empty;
      MAIN_INPUT.Text = string.Empty;
      PANEL_LOCATION_INPUT.Text = string.Empty;
      PANEL_NAME_INPUT.Text = string.Empty;
      LARGEST_LCL_INPUT.Text = string.Empty;

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
      LCL_GRID.Rows[0].Cells[0].Value = "0";
      LCL_GRID.Rows[0].Cells[1].Value = "0";
      TOTAL_OTHER_LOAD_GRID.Rows[0].Cells[0].Value = "0";
      PANEL_LOAD_GRID.Rows[0].Cells[0].Value = "0";
      FEEDER_AMP_GRID.Rows[0].Cells[0].Value = "0";

      // Clear DataGridViews
      for (int i = 0; i < PANEL_GRID.Rows.Count; i++)
      {
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

    private void add_phase_sum_column(bool is3PH)
    {
      if (is3PH)
      {
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
      else
      {
        if (PHASE_SUM_GRID.Columns.Count > 2)
        {
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

    private void link_cell_to_phase(string cellValue, DataGridViewRow row, DataGridViewColumn col)
    {
      var (panel_name, phase) = convert_cell_value_to_panel_name_and_phase(cellValue);

      if (panel_name.ToLower() == PANEL_NAME_INPUT.Text.ToLower())
      {
        return;
      }

      var isPanelReal = this.mainForm.panel_name_exists(panel_name);

      if (isPanelReal)
      {
        UserControl panelControl = mainForm.findUserControl(panel_name);

        if (panelControl != null)
        {
          DataGridView panelControl_phaseSumGrid = panelControl.Controls.Find("PHASE_SUM_GRID", true).FirstOrDefault() as DataGridView;
          DataGridView this_panelGrid = this.Controls.Find("PANEL_GRID", true).FirstOrDefault() as DataGridView;
          this_panelGrid.Rows[row.Index].Cells[col.Index].Tag = cellValue;
          listenForPhaseChanges(panelControl_phaseSumGrid, phase, row, col, this_panelGrid);
        }
      }
    }

    private void listenForPhaseChanges(DataGridView panelControl_phaseSumGrid, string phase, DataGridViewRow row, DataGridViewColumn col, DataGridView panelGrid)
    {
      var phaseSumGrid_row = 0;
      var phaseSumGrid_col = 0;

      DataGridViewCellEventHandler eventHandler = null;
      DataGridViewCellEventHandler panelGrid_eventHandler = null;

      if (phase == "A")
      {
        phaseSumGrid_col = 0;
      }
      else if (phase == "B")
      {
        phaseSumGrid_col = 1;
      }
      else if (phase == "C")
      {
        phaseSumGrid_col = 2;
      }

      var newCellValue = panelControl_phaseSumGrid.Rows[phaseSumGrid_row].Cells[phaseSumGrid_col].Value.ToString();
      panelGrid.Rows[row.Index].Cells[col.Index].Value = newCellValue;
      panelGrid.Rows[row.Index].Cells[col.Index].Style.BackColor = Color.LightGreen;

      eventHandler = (sender, e) =>
      {
        if (e.RowIndex == phaseSumGrid_row && e.ColumnIndex == phaseSumGrid_col)
        {
          var newCellValue = panelControl_phaseSumGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
          panelGrid.Rows[row.Index].Cells[col.Index].Value = newCellValue;
          panelGrid.Rows[row.Index].Cells[col.Index].Style.BackColor = Color.LightGreen;
        }
      };

      panelControl_phaseSumGrid.CellValueChanged += eventHandler;

      panelGrid_eventHandler = (sender, e) =>
      {
        if (e.RowIndex == row.Index && e.ColumnIndex == col.Index)
        {
          var newCellValue = panelGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
          var phaseSumGridValue = panelControl_phaseSumGrid.Rows[phaseSumGrid_row].Cells[phaseSumGrid_col].Value?.ToString();
          if (newCellValue != phaseSumGridValue)
          {
            panelControl_phaseSumGrid.CellValueChanged -= eventHandler;
            panelGrid.CellValueChanged -= panelGrid_eventHandler;
            panelGrid.Rows[row.Index].Cells[col.Index].Style.BackColor = Color.LightGray;
            if (panelGrid.Rows[row.Index].Cells[col.Index].Tag != null)
            {
              panelGrid.Rows[row.Index].Cells[col.Index].Tag = null;
            }
          }
        }
      };

      panelGrid.CellValueChanged += panelGrid_eventHandler;
    }

    private (string, string) convert_cell_value_to_panel_name_and_phase(string cellValue)
    {
      // an example cell value is "=PA-A" with "PA" being the panel name and "A" being the phase, the panel name can be any length of string and the phase can only be "A, B, or C" and always comes after a dash
      string[] splitCellValue = cellValue.Split('-');
      string panelName = splitCellValue[0].Replace("=", "");
      string phase = splitCellValue[1];

      return (panelName, phase);
    }

    private void change_size_of_phase_columns(bool is3PH)
    {
      // when phase c is added, reduce the size of phase a and phase b, and increase the size of phase c until the 3 columns are equal in size and match the width of phase a and phase b combined
      if (is3PH)
      {
        // Left Side
        PANEL_GRID.Columns["phase_a_left"].Width = 67;
        PANEL_GRID.Columns["phase_b_left"].Width = 67;
        PANEL_GRID.Columns["phase_c_left"].Width = 67;

        // Right Side
        PANEL_GRID.Columns["phase_a_right"].Width = 67;
        PANEL_GRID.Columns["phase_b_right"].Width = 67;
        PANEL_GRID.Columns["phase_c_right"].Width = 67;
      }
      else
      {
        // Left Side
        PANEL_GRID.Columns["phase_a_left"].Width = 100;
        PANEL_GRID.Columns["phase_b_left"].Width = 100;

        // Right Side
        PANEL_GRID.Columns["phase_a_right"].Width = 100;
        PANEL_GRID.Columns["phase_b_right"].Width = 100;
      }
    }

    private void add_or_remove_panel_grid_columns(bool is3PH)
    {
      // insert a column with the text "PH C" into PANEL_GRID after phase b on the left and right side if the checkbox is checked
      // give the column the name "phase_c_left" and "phase_c_right"
      if (is3PH)
      {
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
      else
      {
        if (PANEL_GRID.Columns.Count > 10)
        {
          // Left Side
          PANEL_GRID.Columns.Remove("phase_c_left");

          // Right Side
          PANEL_GRID.Columns.Remove("phase_c_right");
        }
      }
    }

    private void update_apply_combobox_to_match_storage()
    {
      var apply_combobox_items = new List<string>();
      foreach (var note in this.notesStorage)
      {
        if (!apply_combobox_items.Contains(note))
        {
          apply_combobox_items.Add(note);
        }
      }
      APPLY_COMBOBOX.DataSource = apply_combobox_items;
    }

    private void remove_tags_from_cells(string tag)
    {
      foreach (DataGridViewRow row in PANEL_GRID.Rows)
      {
        foreach (DataGridViewCell cell in row.Cells)
        {
          if (cell.Tag != null)
          {
            string cellTag = cell.Tag.ToString();
            if (cellTag.Contains(tag))
            {
              cellTag = cellTag.Replace(tag, "");
              if (cellTag.EndsWith("|"))
              {
                cellTag = cellTag.Substring(0, cellTag.Length - 1);
              }
              cell.Tag = cellTag;
            }
          }
        }
      }
    }

    public void update_notes_storage(List<string> notesStorage)
    {
      this.notesStorage = notesStorage;
      update_apply_combobox_to_match_storage();
    }

    private void PANEL_NAME_INPUT_TextChanged(object sender, EventArgs e)
    {
      this.mainForm.PANEL_NAME_INPUT_TextChanged(sender, e, PANEL_NAME_INPUT.Text.ToUpper());
    }

    private void PANEL_GRID_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
    {
      if (PHASE_SUM_GRID.ColumnCount > 2)
      {
        listen_for_3P_rows_added(e);
      }
      else
      {
        listen_for_2P_rows_added(e);
      }
    }

    private void PANEL_GRID_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
      // Check if the current cell is the one being formatted and the DataGridView doesn't have focus
      if (this.PANEL_GRID.CurrentCell != null
          && e.RowIndex == this.PANEL_GRID.CurrentCell.RowIndex
          && e.ColumnIndex == this.PANEL_GRID.CurrentCell.ColumnIndex
          && !this.PANEL_GRID.Focused)
      {
        // Change the back color and fore color to make the current cell less noticeable
        e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
        e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;
      }
    }

    private void PANEL_GRID_CellValueChanged(object sender, DataGridViewCellEventArgs e)
    {
      if (PHASE_SUM_GRID.ColumnCount > 2)
      {
        panel_cell_changed_3P(e);
      }
      else
      {
        panel_cell_changed_2P(e);
      }
      if (PANEL_GRID.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null || PANEL_GRID.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "") return;
      var cellValue = PANEL_GRID.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
      var columnName = PANEL_GRID.Columns[e.ColumnIndex].Name;
      var row = PANEL_GRID.Rows[e.RowIndex];
      var col = PANEL_GRID.Columns[e.ColumnIndex];
      if (cellValue.StartsWith("="))
      {
        link_cell_to_phase(cellValue, row, col);
      }
    }

    private void PANEL_GRID_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
    {
      oldValue = PANEL_GRID.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
    }

    private void PANEL_GRID_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Control && e.KeyCode == Keys.V)
      {
        // Get text from clipboard
        string text = Clipboard.GetText();

        if (!string.IsNullOrEmpty(text))
        {
          // Split clipboard text into lines
          string[] lines = text.Split('\n');

          if (lines.Length > 0 && string.IsNullOrWhiteSpace(lines[lines.Length - 1]))
          {
            Array.Resize(ref lines, lines.Length - 1);
          }

          // Get start cell for pasting
          int rowIndex = PANEL_GRID.CurrentCell.RowIndex;
          int colIndex = PANEL_GRID.CurrentCell.ColumnIndex;
          int startRowIndex = PANEL_GRID.CurrentCell.RowIndex;

          // Paste each line into a row
          foreach (string line in lines)
          {
            string[] parts = line.Split('\t');

            for (int i = 0; i < parts.Length; i++)
            {
              if (rowIndex < PANEL_GRID.RowCount && colIndex + i < PANEL_GRID.ColumnCount)
              {
                try
                {
                  PANEL_GRID[colIndex + i, rowIndex].Value = parts[i];
                }
                catch (FormatException)
                {
                  // Handle format exception

                  // Set to default value
                  PANEL_GRID[colIndex + i, rowIndex].Value = 0;

                  // Or leave cell blank
                  //dataGridView1[colIndex + i, rowIndex].Value = DBNull.Value;

                  // Or notify user
                  MessageBox.Show("Invalid format in cell!");
                }
              }
            }

            rowIndex++;
          }
          // Reset row index after loop
          rowIndex = startRowIndex;
        }

        e.Handled = true;
      }
      // Check if Ctrl+C was pressed
      else if (e.Control && e.KeyCode == Keys.C)
      {
        StringBuilder copiedText = new StringBuilder();

        // Loop through selected cells
        foreach (DataGridViewCell cell in PANEL_GRID.SelectedCells)
        {
          copiedText.AppendLine(cell.Value?.ToString() ?? string.Empty);
        }

        // Loop through selected rows
        foreach (DataGridViewRow row in PANEL_GRID.SelectedRows)
        {
          List<string> cellValues = new List<string>();
          foreach (DataGridViewCell cell in row.Cells)
          {
            if (cell.Selected)
            {
              cellValues.Add(cell.Value?.ToString() ?? string.Empty);
            }
          }
          if (cellValues.Count > 0)
          {
            copiedText.AppendLine(string.Join("\t", cellValues));
          }
        }

        if (copiedText.Length > 0)
        {
          Clipboard.SetText(copiedText.ToString());
        }

        e.Handled = true;
      }

      // Existing code for handling the Delete key
      else if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
      {
        foreach (DataGridViewCell cell in PANEL_GRID.SelectedCells)
        {
          cell.Value = null;
        }
        e.Handled = true;
      }
    }

    private async void PANEL_GRID_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex == -1)
      {
        return;
      }

      if (e.ColumnIndex < 0)
      {
        return;
      }

      if (!PANEL_GRID.Columns[e.ColumnIndex].Name.Contains("phase"))
      {
        return;
      }

      // Get the selected cell
      DataGridViewCell cell = PANEL_GRID.Rows[e.RowIndex].Cells[e.ColumnIndex];

      // check if the cell has a tag
      if (cell.Tag != null)
      {
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
        if (INFO_LABEL.Text == $"This cell is linked to phase {phase} of panel '{panelName}'.")
        {
          INFO_LABEL.Text = string.Empty;
        }
      }
    }

    private void PHASE_SUM_GRID_CellValueChanged(object sender, DataGridViewCellEventArgs e)
    {
      // Check if the modified cell is in row 0 and column 0, 1, or 2
      if (e.RowIndex == 0 && (e.ColumnIndex == 0 || e.ColumnIndex == 1 || e.ColumnIndex == 2))
      {
        // Retrieve the values from PHASE_SUM_GRID, row 0, column 0 and 1
        double value1 = Convert.ToDouble(PHASE_SUM_GRID.Rows[0].Cells[0].Value ?? 0); // Using null-coalescing operator for safety
        double value2 = Convert.ToDouble(PHASE_SUM_GRID.Rows[0].Cells[1].Value ?? 0); // Using null-coalescing operator for safety
        double value3 = 0;
        double sum = value1 + value2; // Sum of first two columns

        // If the phase sum grid has three columns
        if (PHASE_SUM_GRID.ColumnCount > 2)
        {
          value3 = Convert.ToDouble(PHASE_SUM_GRID.Rows[0].Cells[2].Value ?? 0); // Using null-coalescing operator for safety
          sum += value3; // Adding value of the third column if three-phase is checked
        }

        // Conditional execution based on the state of LARGEST_LCL_CHECKBOX and the presence of a value in TOTAL_OTHER_LOAD_GRID
        if (LARGEST_LCL_CHECKBOX.Checked && TOTAL_OTHER_LOAD_GRID[0, 0].Value != null)
        {
          calculate_totalva_panelload_feederamps_lcl(sum);
        }
        else
        {
          // You might need to adjust the parameters passed to this function based on your needs
          calculate_totalva_panelload_feederamps_regular(value1, value2, value3, sum);
        }
      }
    }

    private void ADD_ROW_BUTTON_Click(object sender, EventArgs e)
    {
      PANEL_GRID.Rows.Add();

      if (PANEL_GRID.Rows.Count > 21)
      {
        PANEL_GRID.Width = 1047 + 15;
      }
    }

    private void CREATE_PANEL_BUTTON_Click(object sender, EventArgs e)
    {
      Dictionary<string, object> panelDataList = retrieve_data_from_modal();

      using (DocumentLock docLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument())
      {
        this.mainForm.Close();
        myCommandsInstance.Create_Panel(panelDataList);
      }
    }

    private void DELETE_ROW_BUTTON_Click(object sender, EventArgs e)
    {
      if (PANEL_GRID.Rows.Count > 0)
      {
        var lastRow = PANEL_GRID.Rows[PANEL_GRID.Rows.Count - 1];
        var phaseCells = new List<string> { "phase_a_left", "phase_b_left", "phase_a_right", "phase_b_right" };

        if (PHASE_SUM_GRID.ColumnCount > 2)
        {
          phaseCells.Add("phase_c_left");
          phaseCells.Add("phase_c_right");
        }

        foreach (var cell in phaseCells)
        {
          lastRow.Cells[cell].Value = "0";
        }

        PANEL_GRID.Rows.RemoveAt(PANEL_GRID.Rows.Count - 1);

        if (PANEL_GRID.Rows.Count <= 21)
        {
          PANEL_GRID.Width = 1047;
        }
      }
    }

    private void LARGEST_LCL_CHECKBOX_CheckedChanged(object sender, EventArgs e)
    {
      calculate_lcl_otherload_panelload_feederamps();
    }

    private void LARGEST_LCL_INPUT_TextChanged(object sender, EventArgs e)
    {
      calculate_lcl_otherload_panelload_feederamps();
    }

    private void DELETE_PANEL_BUTTON_Click(object sender, EventArgs e)
    {
      this.mainForm.delete_panel(this);
    }

    private void INFO_LABEL_CLICK(object sender, EventArgs e)
    {
    }

    private void NOTES_BUTTON_Click(object sender, EventArgs e)
    {
      if (this.notesForm == null || this.notesForm.IsDisposed)
      {
        this.notesForm = new noteForm(this);
        this.notesForm.Show();
        this.notesForm.Text = $"Panel '{PANEL_NAME_INPUT.Text}' Notes";
      }
      else
      {
        if (!this.notesForm.Visible)
        {
          this.notesForm.Show();
        }
        this.notesForm.BringToFront();
      }
    }

    private void APPLY_BUTTON_Click(object sender, EventArgs e)
    {
      string selectedValue = APPLY_COMBOBOX.SelectedItem.ToString();

      // make a list with the column names "description" and "breaker" which is used when the selected item value has "BREAKER" in it, otherwise, just use "description"
      List<string> columnNames = new List<string> { "description" };

      // get all the cells which are selected in the panel grid
      foreach (DataGridViewCell cell in PANEL_GRID.SelectedCells)
      {
        // check if the cell owning column name contains either of the column names in the list
        if (columnNames.Any(cell.OwningColumn.Name.Contains))
        {
          // update the tag to include the selected value, if the tag already has a value then comma separate and append the new value
          if (cell.Tag == null)
          {
            cell.Tag = selectedValue;
          }
          else
          {
            cell.Tag = $"{cell.Tag}|{selectedValue}";
          }
          // turn the background of the cell to a yellow color
          cell.Style.BackColor = Color.Yellow;
        }
      }
    }

    private void APPLY_COMBOBOX_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (APPLY_COMBOBOX.SelectedItem == null)
      {
        return;
      }

      // go through each panel grid breaker and description cell and remove the background color
      foreach (DataGridViewRow row in PANEL_GRID.Rows)
      {
        foreach (DataGridViewCell cell in row.Cells)
        {
          if (cell.OwningColumn.Name.Contains("description") || cell.OwningColumn.Name.Contains("breaker"))
          {
            cell.Style.BackColor = Color.Empty;
          }
        }
      }

      foreach (DataGridViewRow row in PANEL_GRID.Rows)
      {
        foreach (DataGridViewCell cell in row.Cells)
        {
          if (cell.OwningColumn.Name.Contains("description") || cell.OwningColumn.Name.Contains("breaker"))
          {
            if (cell.Tag == null)
            {
              continue;
            }
            if (cell.Tag.ToString().Contains(APPLY_COMBOBOX.SelectedItem.ToString()))
            {
              // turn the background of the cell to a yellow color
              cell.Style.BackColor = Color.Yellow;
            }
          }
        }
      }
    }

    private void STATUS_COMBOBOX_SelectedIndexChanged(object sender, EventArgs e)
    {
      var default_existing_message = "DENOTES EXISTING CIRCUIT BREAKER TO REMAIN; ALL OTHERS ARE NEW.";
      var default_new_message = "65 KAIC SERIES RATED OR MATCH FAULT CURRENT AT SITE.";

      if (STATUS_COMBOBOX.SelectedItem != null)
      {
        if (STATUS_COMBOBOX.SelectedItem.ToString() == "EXISTING" || STATUS_COMBOBOX.SelectedItem.ToString() == "RELOCATED")
        {
          if (!this.notesStorage.Contains(default_existing_message))
          {
            this.notesStorage.Add(default_existing_message);
          }
          if (this.notesStorage.Contains(default_new_message))
          {
            this.notesStorage.Remove(default_new_message);
          }
          remove_tags_from_cells(default_new_message);
        }
        else
        {
          if (!this.notesStorage.Contains(default_new_message))
          {
            this.notesStorage.Add(default_new_message);
          }
          if (this.notesStorage.Contains(default_existing_message))
          {
            this.notesStorage.Remove(default_existing_message);
          }
          remove_tags_from_cells(default_existing_message);
        }
        update_apply_combobox_to_match_storage();
      }
    }

    public void save_the_notes_storage_as_json_to_desktop()
    {
      string json = JsonConvert.SerializeObject(this.notesStorage, Formatting.Indented);
      string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
      string fileName = "notesStorage.json";
      string fullPath = Path.Combine(path, fileName);
      File.WriteAllText(fullPath, json);
    }

    private void REMOVE_NOTE_BUTTON_Click(object sender, EventArgs e)
    {
      string selectedValue = APPLY_COMBOBOX.SelectedItem.ToString();

      // make a list with the column names "description" and "breaker" which is used when the selected item value has "BREAKER" in it, otherwise, just use "description"
      List<string> columnNames = new List<string> { "description" };

      // get all the cells which are selected in the panel grid
      foreach (DataGridViewCell cell in PANEL_GRID.SelectedCells)
      {
        // check if the cell owning column name contains either of the column names in the list
        if (columnNames.Any(cell.OwningColumn.Name.Contains))
        {
          // check if the tag contains the selected value
          if (cell.Tag != null && cell.Tag.ToString().Contains(selectedValue))
          {
            // remove the selected value from the tag
            cell.Tag = cell.Tag.ToString().Replace(selectedValue, "").Trim('|');

            cell.Style.BackColor = Color.Empty;
          }
        }
      }
    }
  }
}