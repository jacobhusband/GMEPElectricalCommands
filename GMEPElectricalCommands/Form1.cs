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
using static OfficeOpenXml.ExcelErrorValue;

namespace AutoCADCommands
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
      Listen_For_New_Rows();
      Remove_Column_Header_Sorting();

      PANEL_GRID.Rows.AddCopies(0, 21);
      PANEL_GRID.AllowUserToAddRows = false;
      PANEL_GRID.KeyDown += new KeyEventHandler(this.PANEL_GRID_KeyDown);
      PANEL_GRID.CellBeginEdit += new DataGridViewCellCancelEventHandler(this.PANEL_GRID_CellBeginEdit);
      PANEL_GRID.CellValueChanged += new DataGridViewCellEventHandler(this.PANEL_GRID_CellValueChanged);
      PHASE_SUM_GRID.CellValueChanged += new DataGridViewCellEventHandler(this.PHASE_SUM_GRID_CellValueChanged);

      Set_Default_Form_Values();
      Deselect_Cells();
    }

    private object oldValue;

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
      else if (e.KeyCode == Keys.Delete)
      {
        foreach (DataGridViewCell cell in PANEL_GRID.SelectedCells)
        {
          cell.Value = null;
        }
        e.Handled = true;
      }
    }

    private void PHASE_SUM_GRID_CellValueChanged(object sender, DataGridViewCellEventArgs e)
    {
      // Check if the modified cell is in row 0 and column 0 or 1
      if (e.RowIndex == 0 && (e.ColumnIndex == 0 || e.ColumnIndex == 1))
      {
        // Retrieve the values from PHASE_SUM_GRID, row 0, column 0 and 1
        double value1 = Convert.ToDouble(PHASE_SUM_GRID.Rows[0].Cells[0].Value);
        double value2 = Convert.ToDouble(PHASE_SUM_GRID.Rows[0].Cells[1].Value);

        // Perform the sum (checking for null values)
        double sum = value1 + value2;

        if (LARGEST_LCL_CHECKBOX.Checked && TOTAL_OTHER_LOAD_GRID[0, 0].Value != null)
        {
          calculate_totalva_panelload_feederamps_lcl(sum);
        }
        else
        {
          calculate_totalva_panelload_feederamps_regular(value1, value2, sum);
        }
      }
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

    private void calculate_totalva_panelload_feederamps_regular(double value1, double value2, double sum)
    {
      // Update total VA and panel load without other loads
      TOTAL_VA_GRID.Rows[0].Cells[0].Value = Math.Round(sum, 0); // Rounded to 1 decimal place
      PANEL_LOAD_GRID.Rows[0].Cells[0].Value = Math.Round(sum / 1000, 1); // Rounded to 1 decimal place

      // Update feeder amps
      double maxVal = Math.Max(Convert.ToDouble(value1), Convert.ToDouble(value2));

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

    private void PANEL_GRID_CellValueChanged(object sender, DataGridViewCellEventArgs e)
    {
      // Check if the modified cell is in column 2 or 8
      if (e.ColumnIndex == 1 || e.ColumnIndex == 7)
      {
        // Sum the values of all cells in column 2 and 8
        double sum = 0;
        foreach (DataGridViewRow row in PANEL_GRID.Rows)
        {
          if (row.Cells[1].Value != null)
            sum += Convert.ToDouble(row.Cells[1].Value);

          if (row.Cells[7].Value != null)
            sum += Convert.ToDouble(row.Cells[7].Value);
        }

        // Update the sum in dataGridView2, row 0, column 0
        PHASE_SUM_GRID.Rows[0].Cells[0].Value = sum;
      }

      // Check if the modified cell is in column 3 or 9
      if (e.ColumnIndex == 2 || e.ColumnIndex == 8)
      {
        // Sum the values of all cells in column 3 and 9
        double sum = 0;
        foreach (DataGridViewRow row in PANEL_GRID.Rows)
        {
          if (row.Cells[2].Value != null)
            sum += Convert.ToDouble(row.Cells[2].Value);

          if (row.Cells[8].Value != null)
            sum += Convert.ToDouble(row.Cells[8].Value);
        }

        // Update the sum in dataGridView2, row 0, column 1
        PHASE_SUM_GRID.Rows[0].Cells[1].Value = sum;
      }

      object newValue = PANEL_GRID.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
      oldValue = null;
    }

    private void Deselect_Cells()
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
    }

    private void Set_Default_Form_Values()
    {
      // Textboxes
      PANEL_NAME_INPUT.Text = "A";
      PANEL_LOCATION_INPUT.Text = "ELECTRIC ROOM";
      MAIN_INPUT.Text = "M.L.O";
      BUS_RATING_INPUT.Text = "100";

      // Comboboxes
      STATUS_COMBOBOX.SelectedIndex = 0;
      MOUNTING_COMBOBOX.SelectedIndex = 0;
      WIRE_COMBOBOX.SelectedIndex = 1;
      PHASE_COMBOBOX.SelectedIndex = 1;
      PHASE_VOLTAGE_COMBOBOX.SelectedIndex = 0;
      LINE_VOLTAGE_COMBOBOX.SelectedIndex = 0;

      // Datagrids
      PHASE_SUM_GRID.Rows.Add("0", "0");
      TOTAL_VA_GRID.Rows.Add("0");
      LCL_GRID.Rows.Add("0", "0");
      TOTAL_OTHER_LOAD_GRID.Rows.Add("0");
      PANEL_LOAD_GRID.Rows.Add("0");
      FEEDER_AMP_GRID.Rows.Add("0");
    }

    private void Remove_Column_Header_Sorting()
    {
      foreach (DataGridViewColumn column in PANEL_GRID.Columns)
      {
        column.SortMode = DataGridViewColumnSortMode.NotSortable;
      }
    }

    private void Listen_For_New_Rows()
    {
      PANEL_GRID.RowsAdded += new DataGridViewRowsAddedEventHandler(PANEL_GRID_RowsAdded);
    }

    private void PANEL_GRID_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
    {
      for (int i = 0; i < e.RowCount; i++)
      {
        int rowIndex = e.RowIndex + i;

        // Set common column values
        PANEL_GRID.Rows[rowIndex].Cells[3].Value = "20";
        PANEL_GRID.Rows[rowIndex].Cells[4].Value = ((rowIndex + 1) * 2) - 1;
        PANEL_GRID.Rows[rowIndex].Cells[5].Value = (rowIndex + 1) * 2;
        PANEL_GRID.Rows[rowIndex].Cells[6].Value = "20";

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

    private void ADD_ROW_BUTTON_CLICK(object sender, EventArgs e)
    {
      PANEL_GRID.Rows.Add();
    }

    private void DELETE_ROW_BUTTON_CLICK(object sender, EventArgs e)
    {
      if (PANEL_GRID.Rows.Count > 0)
      {
        PANEL_GRID.Rows.RemoveAt(PANEL_GRID.Rows.Count - 1);
      }
    }

    private void CREATE_PANEL_BUTTON_CLICK(object sender, EventArgs e)
    {
      List<Dictionary<string, object>> panelDataList = Retrieve_Data_From_Modal();

      Print_Panels(panelDataList);
    }

    private void Print_Panels(List<Dictionary<string, object>> panels)
    {
      foreach (Dictionary<string, object> panel in panels)
      {
        Console.WriteLine("Panel:");

        // Print simple values
        Console.WriteLine("  Panel: {0}", panel["panel"]);
        Console.WriteLine("  Location: {0}", panel["location"]);

        // Print combo box selections
        Console.WriteLine("  Existing: {0}", panel["existing"]);
        Console.WriteLine("  Mounting: {0}", panel["mounting"]);

        Console.WriteLine(); // Add blank line between panels
      }
    }

    private List<Dictionary<string, object>> Retrieve_Data_From_Modal()
    {
      List<Dictionary<string, object>> panels = new List<Dictionary<string, object>>();

      // Create a new panel
      Dictionary<string, object> panel = new Dictionary<string, object>();

      // Add simple values
      panel.Add("panel", PANEL_NAME_INPUT.Text);
      panel.Add("location", PANEL_LOCATION_INPUT.Text);
      panel.Add("bus_rating", BUS_RATING_INPUT.Text);
      panel.Add("main", MAIN_INPUT.Text);
      panel.Add("voltage1", LINE_VOLTAGE_COMBOBOX.SelectedItem.ToString());
      panel.Add("voltage2", PHASE_VOLTAGE_COMBOBOX.SelectedItem.ToString());
      panel.Add("phase", PHASE_COMBOBOX.SelectedItem.ToString());
      panel.Add("wire", WIRE_COMBOBOX.SelectedItem.ToString());
      panel.Add("mounting", MOUNTING_COMBOBOX.SelectedItem.ToString());
      panel.Add("existing", STATUS_COMBOBOX.SelectedItem.ToString());

      // Add datagrid values

      panels.Add(panel);
      return panels;
    }

    private void LARGEST_LCL_CHECKBOX_CheckedChanged(object sender, EventArgs e)
    {
      calculate_lcl_otherload_panelload_feederamps();
    }

    private void LARGEST_LCL_INPUT_TextChanged(object sender, EventArgs e)
    {
      calculate_lcl_otherload_panelload_feederamps();
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

        // Perform the sum (checking for null values)
        double sum = value1 + value2;

        calculate_totalva_panelload_feederamps_regular(value1, value2, sum);
      }
    }
  }
}