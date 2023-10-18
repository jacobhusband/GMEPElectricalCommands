using System.Windows;
using System.Windows.Forms;
using System.IO;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.EditorInput;
using System.Reflection.Emit;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Linq;
using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.Colors;
using OfficeOpenXml;
using System.Collections;

[assembly: CommandClass(typeof(AutoCADCommands.MyCommands))]

namespace AutoCADCommands
{
  public class MyCommands
  {
    [CommandMethod("HelloWorld")]
    public void HelloWorldCommand()
    {
      Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
      Editor ed = doc.Editor;
      ed.WriteMessage("HelloWorld");
    }

    [CommandMethod("PanelInput")]
    public void PanelInputCommand()
    {
      Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
      Editor ed = doc.Editor;

      // Create the dialog box
      Form dialog = new Form();
      dialog.Text = "2 POLE PANEL";
      dialog.Size = new System.Drawing.Size(900, 600); // Adjusted size for 17x9 grid

      int boxWidth = 80;
      int boxHeight = 25;
      int spacing = 5; // space between cells

      // Usage
      var headers = new System.Windows.Forms.Label[]
      {
        CreateLabel("DESCRIPTION"),
        CreateLabel("VOLT AMPS\nPH A"),
        CreateLabel("VOLT AMPS\nPH B"),
        CreateLabel("BKR"),
        CreateLabel("CKT NO"),
        CreateLabel("CKT NO"),
        CreateLabel("BKR"),
        CreateLabel("VOLT AMPS\nPH A"),
        CreateLabel("VOLT AMPS\nPH B"),
        CreateLabel("DESCRIPTION"),
      };

      TextBox[,] panelInputs = new TextBox[17, 9];

      for (int i = 0; i < 17; i++)
      {
        for (int j = 0; j < 9; j++)
        {
          panelInputs[i, j] = new TextBox();
          panelInputs[i, j].Size = new System.Drawing.Size(boxWidth, boxHeight);
          panelInputs[i, j].Location = new System.Drawing.Point(spacing + (j * (boxWidth + spacing)), spacing + (boxHeight + spacing) + (i * (boxHeight + spacing)));
          dialog.Controls.Add(panelInputs[i, j]);
        }
      }

      // Create and set Send button
      Button sendButton = new Button();
      sendButton.Text = "Send";
      sendButton.Location = new System.Drawing.Point((dialog.Width - sendButton.Width) / 2, 520); // Adjusted location of the button
      dialog.Controls.Add(sendButton);

      // Add button click event
      sendButton.Click += (sender, e) =>
      {
        for (int i = 0; i < 17; i++)
        {
          for (int j = 0; j < 9; j++)
          {
            ed.WriteMessage($"Value at row {i + 1}, column {j + 1} is: {panelInputs[i, j].Text}\n");
          }
        }
        dialog.Close();
      };

      // Show dialog
      dialog.ShowDialog();
    }

    [CommandMethod("InputSwitch")]
    public void InputSwitchCommand()
    {
      Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
      Editor ed = doc.Editor;

      // Create the dialog box
      Form dialog = new Form();
      dialog.Text = "Enter Your Name";
      dialog.Size = new System.Drawing.Size(350, 120); // Initial size

      // Create and set label
      System.Windows.Forms.Label nameLabel = new System.Windows.Forms.Label();
      nameLabel.Text = "Name";
      nameLabel.Location = new System.Drawing.Point(20, 20);
      dialog.Controls.Add(nameLabel);

      // Create and set input box (TextBox)
      TextBox nameInput = new TextBox();
      nameInput.Location = new System.Drawing.Point(70, 20);
      nameInput.Width = 150;
      dialog.Controls.Add(nameInput);

      // Create and set CheckBox (Switch)
      CheckBox nameSwitch = new CheckBox();
      nameSwitch.Text = "Switch";
      nameSwitch.Location = new System.Drawing.Point(230, 20);
      dialog.Controls.Add(nameSwitch);

      // Create and set a second input box below the first one
      TextBox additionalInput = new TextBox();
      additionalInput.Location = new System.Drawing.Point(70, 50); // Positioned below the first TextBox
      additionalInput.Width = 150;
      additionalInput.Visible = false;  // Initially hidden
      dialog.Controls.Add(additionalInput);

      // Create and set Send button
      Button sendButton = new Button();
      sendButton.Text = "Send";
      sendButton.Location = new System.Drawing.Point(70, 80); // Adjusted initial position
      dialog.Controls.Add(sendButton);

      // Add switch toggle event to show/hide additional input box
      nameSwitch.CheckedChanged += (sender, e) =>
      {
        additionalInput.Visible = nameSwitch.Checked;
        if (nameSwitch.Checked)
        {
          dialog.Height = 180; // Adjust the dialog height to show additional input
          sendButton.Location = new System.Drawing.Point(70, 110);
        }
        else
        {
          dialog.Height = 120; // Restore original dialog height
          sendButton.Location = new System.Drawing.Point(70, 80);
        }
      };

      // Add button click event
      sendButton.Click += (sender, e) =>
      {
        ed.WriteMessage($"Your name is: {nameInput.Text}");
        if (nameSwitch.Checked)
        {
          ed.WriteMessage($" Additional Input: {additionalInput.Text}");
        }
        dialog.Close();
      };

      // Show dialog
      dialog.ShowDialog();
    }

    [CommandMethod("ShowMyForm")]
    public void ShowFormCommand()
    {
      Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
      Editor ed = doc.Editor;

      try
      {
        Form1 myForm = new Form1(); // This is assuming your form's name is Form1.
        Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(null, myForm, false);
      }
      catch (System.Exception ex)
      {
        ed.WriteMessage("Error: " + ex.ToString());
      }
    }

    [CommandMethod("CREATEBLOCK")]
    public void CREATEBLOCK()
    {
      var (doc, db, _) = MyCommands.GetGlobals();

      using (Transaction tr = db.TransactionManager.StartTransaction())
      {
        BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);

        BlockTableRecord existingBtr = null;
        ObjectId existingBtrId = ObjectId.Null;

        // Check if block already exists
        if (bt.Has("CIRCLEI"))
        {
          existingBtrId = bt["CIRCLEI"];

          if (existingBtrId != ObjectId.Null)
          {
            existingBtr = (BlockTableRecord)tr.GetObject(existingBtrId, OpenMode.ForWrite);

            if (existingBtr != null && existingBtr.Name == "CIRCLEI")
            {
              doc.Editor.WriteMessage("\nBlock 'CIRCLEI' already exists and matches the new block. Exiting the function.");
              return; // Exit the function if existing block matches the new block
            }
          }
        }

        // Delete existing block and its contents
        if (existingBtr != null)
        {
          foreach (ObjectId id in existingBtr.GetBlockReferenceIds(true, true))
          {
            DBObject obj = tr.GetObject(id, OpenMode.ForWrite);
            obj.Erase(true);
          }

          existingBtr.Erase(true);

          doc.Editor.WriteMessage("\nExisting block 'CIRCLEI' and its contents have been deleted.");
        }

        BlockTableRecord btr = new BlockTableRecord();
        btr.Name = "CIRCLEI";

        bt.UpgradeOpen();
        ObjectId btrId = bt.Add(btr);
        tr.AddNewlyCreatedDBObject(btr, true);

        // Create a circle centered at 0,0 with radius 2.0
        Circle circle = new Circle(new Point3d(0, 0, 0), new Vector3d(0, 0, 1), 0.09);
        circle.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(ColorMethod.ByLayer, 2); // Set circle color to yellow

        btr.AppendEntity(circle);
        tr.AddNewlyCreatedDBObject(circle, true);

        // Create a text entity
        DBText text = new DBText();
        text.Position = new Point3d(-0.042, -0.045, 0); // centered at the origin
        text.Height = 0.09; // Set the text height
        text.TextString = "1";
        text.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(ColorMethod.ByLayer, 2); // Set text color to yellow

        // Check if the text style "ROMANS" exists
        TextStyleTable textStyleTable = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
        if (textStyleTable.Has("ROMANS"))
        {
          text.TextStyleId = textStyleTable["ROMANS"]; // apply the "ROMANS" text style to the text entity
        }

        // Check if the layer "E-TEXT" exists
        LayerTable lt = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);
        if (lt.Has("E-TEXT"))
        {
          circle.Layer = "E-TEXT"; // Set the layer of the circle to "E-TEXT"
          text.Layer = "E-TEXT"; // Set the layer of the text to "E-TEXT"
        }

        btr.AppendEntity(text);
        tr.AddNewlyCreatedDBObject(text, true);

        tr.Commit();
      }
    }

    [CommandMethod("KEEPBREAKERS")]
    public void KEEPBREAKERS()
    {
      var (doc, db, ed) = MyCommands.GetGlobals();

      using (var tr = db.TransactionManager.StartTransaction())
      {
        var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);

        if (!bt.Has("CIRCLEI"))
        {
          PromptKeywordOptions pko = new PromptKeywordOptions("\nThe block 'CIRCLEI' does not exist. Do you want to create it? [Yes/No] ", "Yes No");
          pko.AllowNone = true;
          PromptResult pr = ed.GetKeywords(pko);
          String prompt = pr.StringResult.ToLower();
          if (prompt == "no" || prompt == "n")
            return;
          else if (prompt == "yes" || prompt == "y")
            CREATEBLOCK(); // Assuming this function exists and will create the required block
        }

        var btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.PaperSpace], OpenMode.ForWrite);

        var point1 = ed.GetPoint("\nSelect first point: ").Value;
        var point2 = ed.GetPoint("\nSelect second point: ").Value;

        // Swap the points if the y-coordinate of the first point is lower than that of the second point
        if (point1.Y < point2.Y)
        {
          Point3d tempPoint = point1;
          point1 = point2;
          point2 = tempPoint;
        }

        var point3 = ed.GetPoint("\nSelect third point: ").Value;

        var direction = point3.X > point1.X ? 1 : -1;
        var dist = (point1 - point2).Length;

        var line1Start = new Point3d(point1.X + direction * 0.05, point1.Y, 0);
        var line1End = new Point3d(line1Start.X + direction * 0.2, line1Start.Y, 0);
        var line2Start = new Point3d(line1Start.X, point2.Y, 0);
        var line2End = new Point3d(line1End.X, line2Start.Y, 0);

        string layerName = CreateOrGetLayer("E-TEXT", db, tr);

        var line1 = new Line(line1Start, line1End) { Layer = layerName };
        var line2 = new Line(line2Start, line2End) { Layer = layerName };

        var mid1 = new Point3d((line1Start.X + line1End.X) / 2, (line1Start.Y + line1End.Y) / 2, 0);
        var mid2 = new Point3d((line2Start.X + line2End.X) / 2, (line2Start.Y + line2End.Y) / 2, 0);
        var mid3 = new Point3d((mid1.X + mid2.X) / 2, (mid1.Y + mid2.Y) / 2, 0);

        var circleTop = new Point3d(mid3.X, mid3.Y + 0.09, 0);
        var circleBottom = new Point3d(mid3.X, mid3.Y - 0.09, 0);

        var line3 = new Line(mid1, circleTop) { Layer = layerName };
        var line4 = new Line(mid2, circleBottom) { Layer = layerName };

        if (dist > 0.3)
        {
          btr.AppendEntity(line1);
          btr.AppendEntity(line2);
          tr.AddNewlyCreatedDBObject(line1, true);
          tr.AddNewlyCreatedDBObject(line2, true);
        }

        var blkRef = new BlockReference(mid3, bt["CIRCLEI"]) { Layer = layerName };
        btr.AppendEntity(blkRef);
        tr.AddNewlyCreatedDBObject(blkRef, true);

        if (dist > 0.3)
        {
          btr.AppendEntity(line3);
          btr.AppendEntity(line4);
          tr.AddNewlyCreatedDBObject(line3, true);
          tr.AddNewlyCreatedDBObject(line4, true);
        }

        tr.Commit();
      }
    }

    [CommandMethod("IMPORTPANEL")]
    public void IMPORTPANEL()
    {
      var (doc, db, ed) = MyCommands.GetGlobals();

      List<Dictionary<string, object>> panelDataList = ImportExcelData();

      var spaceId = (db.TileMode == true) ? SymbolUtilityServices.GetBlockModelSpaceId(db) : SymbolUtilityServices.GetBlockPaperSpaceId(db);

      // Get the insertion point from the user
      var promptOptions = new PromptPointOptions("\nSelect top right corner point: ");
      var promptResult = ed.GetPoint(promptOptions);
      if (promptResult.Status != PromptStatus.OK)
        return;

      // Initial point
      var topRightCorner = promptResult.Value;
      var originalTopRightCorner = promptResult.Value;

      // Lowest Y point
      double lowestY = topRightCorner.Y;

      int counter = 0;

      foreach (var panelData in panelDataList)
      {
        bool is2Pole = !panelData.ContainsKey("phase_c_left");
        var endPoint = new Point3d(0, 0, 0);

        using (var tr = db.TransactionManager.StartTransaction())
        {
          var btr = (BlockTableRecord)tr.GetObject(spaceId, OpenMode.ForWrite);

          CREATEBLOCK();

          // Create initial values
          var startPoint = new Point3d(topRightCorner.X - 8.9856, topRightCorner.Y, 0);
          var layerName = "0";

          // Create the independent header text objects
          CreateTextsWithoutPanelData(tr, layerName, startPoint, is2Pole);

          // Create the dependent header text objects
          CreateTextsWithPanelData(tr, layerName, startPoint, panelData);

          // Create breaker text objects
          ProcessTextData(tr, btr, startPoint, panelData, is2Pole);

          // Get end of data
          var endOfDataY = GetEndOfDataY((List<string>)panelData["description_left"], startPoint);
          endPoint = new Point3d(topRightCorner.X, endOfDataY - 0.2533, 0);

          // Create all the data lines
          ProcessLineData(tr, btr, startPoint, endPoint, endOfDataY, is2Pole);

          // Create footer text objects
          CreateFooterText(tr, endPoint, panelData, is2Pole);

          // Create the middle lines
          CreateCenterLines(btr, tr, startPoint, endPoint, is2Pole);

          // Create the notes section
          CreateNotes(btr, tr, startPoint, endPoint, panelData["existing"] as string);

          // Create the calculations section
          CreateCalculations(btr, tr, startPoint, endPoint, panelData);

          // Create the border of the panel
          CreateRectangle(btr, tr, topRightCorner, startPoint, endPoint, layerName);

          tr.Commit();
        }

        // Check if the endPoint.Y is the lowest point
        if (endPoint.Y < lowestY)
        {
          lowestY = endPoint.Y;
        }

        counter++;

        // After printing 3 panels, reset X and decrease Y by 5
        if (counter % 3 == 0)
        {
          topRightCorner = new Point3d(originalTopRightCorner.X, lowestY - 1.5, 0);
          // Reset lowestY
          lowestY = topRightCorner.Y;
        }
        else
        {
          // Increase x-coordinate by 10 for the next panel
          topRightCorner = new Point3d(topRightCorner.X - 9.6, topRightCorner.Y, 0);
        }
      }
    }

    private System.Windows.Forms.Label CreateLabel(string text)
    {
      return new System.Windows.Forms.Label() { Text = text };
    }

    public static (Document doc, Database db, Editor ed) GetGlobals()
    {
      var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
      var db = doc.Database;
      var ed = doc.Editor;

      return (doc, db, ed);
    }

    private static FileInfo GetFileInfo(string filePath)
    {
      var fileInfo = new FileInfo(filePath);
      if (!fileInfo.Exists)
      {
        throw new FileNotFoundException($"The file {filePath} does not exist.");
      }
      return fileInfo;
    }

    private static ExcelPackage GetExcelPackage(FileInfo fileInfo)
    {
      return new ExcelPackage(fileInfo);
    }

    private static bool ValidateWorkbook(ExcelWorkbook workbook)
    {
      if (workbook == null)
      {
        Console.WriteLine("Workbook not found.");
        return false;
      }
      return true;
    }

    private static Dictionary<string, object> ProcessThreePolePanel(ExcelWorksheet selectedWorksheet, int row, int col)
    {
      List<string> descriptionLeft = new List<string>();
      List<string> phaseALeft = new List<string>();
      List<string> phaseBLeft = new List<string>();
      List<string> phaseCLeft = new List<string>();
      List<string> breakerLeft = new List<string>();
      List<string> circuitLeft = new List<string>();
      List<string> circuitRight = new List<string>();
      List<string> breakerRight = new List<string>();
      List<string> phaseARight = new List<string>();
      List<string> phaseBRight = new List<string>();
      List<string> phaseCRight = new List<string>();
      List<string> descriptionRight = new List<string>();

      int panelRow = row + 4;
      int lastRow = panelRow;

      while (selectedWorksheet.Cells[lastRow + 2, col + 6].Value != null)
      {
        lastRow += 2;
      }

      List<bool> descriptionLeftHighlights = new List<bool>();
      List<bool> descriptionRightHighlights = new List<bool>();
      List<bool> breakerLeftHighlights = new List<bool>();
      List<bool> breakerRightHighlights = new List<bool>();

      for (int i = panelRow; i <= lastRow; i++)
      {
        string description = selectedWorksheet.Cells[i, col].Value?.ToString().ToUpper() ?? "SPACE";
        string phaseA = selectedWorksheet.Cells[i, col + 2].Value?.ToString() ?? "0";
        string phaseB = selectedWorksheet.Cells[i, col + 3].Value?.ToString() ?? "0";
        string phaseC = selectedWorksheet.Cells[i, col + 4].Value?.ToString() ?? "0";
        string breaker = selectedWorksheet.Cells[i, col + 5].Value?.ToString() ?? "";
        string circuitL = selectedWorksheet.Cells[i, col + 6].Value?.ToString() ?? "";
        string circuitR = selectedWorksheet.Cells[i, col + 7].Value?.ToString() ?? "";
        string breakerR = selectedWorksheet.Cells[i, col + 8].Value?.ToString() ?? "";
        string phaseAR = selectedWorksheet.Cells[i, col + 9].Value?.ToString() ?? "0";
        string phaseBR = selectedWorksheet.Cells[i, col + 10].Value?.ToString() ?? "0";
        string phaseCR = selectedWorksheet.Cells[i, col + 11].Value?.ToString() ?? "0";
        string descriptionR = selectedWorksheet.Cells[i, col + 12].Value?.ToString().ToUpper() ?? "SPACE";

        bool isLeftHighlighted = selectedWorksheet.Cells[i, col].Style.Fill.BackgroundColor.LookupColor() != "#FF000000";
        bool isRightHighlighted = selectedWorksheet.Cells[i, col + 12].Style.Fill.BackgroundColor.LookupColor() != "#FF000000";
        bool isLeftBreakerHighlighted = selectedWorksheet.Cells[i, col + 5].Style.Fill.BackgroundColor.LookupColor() != "#FF000000";
        bool isRightBreakerHighlighted = selectedWorksheet.Cells[i, col + 8].Style.Fill.BackgroundColor.LookupColor() != "#FF000000";

        descriptionLeft.Add(description);
        phaseALeft.Add(phaseA);
        phaseBLeft.Add(phaseB);
        phaseCLeft.Add(phaseC);
        breakerLeft.Add(breaker);
        circuitLeft.Add(circuitL);
        circuitRight.Add(circuitR);
        breakerRight.Add(breakerR);
        phaseARight.Add(phaseAR);
        phaseBRight.Add(phaseBR);
        phaseCRight.Add(phaseCR);
        descriptionRight.Add(descriptionR);
        descriptionLeftHighlights.Add(isLeftHighlighted);
        descriptionRightHighlights.Add(isRightHighlighted);
        breakerLeftHighlights.Add(isLeftBreakerHighlighted);
        breakerRightHighlights.Add(isRightBreakerHighlighted);
      }

      string panelCellValue = selectedWorksheet.Cells[row, col + 2].Value?.ToString() ?? "";
      if (!panelCellValue.StartsWith("'") || !panelCellValue.EndsWith("'"))
      {
        panelCellValue = "'" + panelCellValue.Trim('\'') + "'";
      }

      var panel = new Dictionary<string, object>
      {
        ["panel"] = panelCellValue,
        ["location"] = selectedWorksheet.Cells[row, col + 5].Value?.ToString() ?? "",
        ["bus_rating"] = selectedWorksheet.Cells[row, col + 9].Value?.ToString() ?? "",
        ["voltage1"] = selectedWorksheet.Cells[row, col + 10].Value?.ToString() ?? "0",
        ["voltage2"] = selectedWorksheet.Cells[row, col + 11].Value?.ToString() ?? "0",
        ["phase"] = selectedWorksheet.Cells[row, col + 12].Value?.ToString() ?? "",
        ["wire"] = selectedWorksheet.Cells[row, col + 13].Value?.ToString() ?? "",
        ["main"] = selectedWorksheet.Cells[row + 1, col + 5].Value?.ToString() ?? "",
        ["mounting"] = selectedWorksheet.Cells[row + 1, col + 12].Value?.ToString() ?? "",
        ["subtotal_a"] = selectedWorksheet.Cells[row + 2, col + 17].Value?.ToString() ?? "0",
        ["subtotal_b"] = selectedWorksheet.Cells[row + 2, col + 18].Value?.ToString() ?? "0",
        ["subtotal_c"] = selectedWorksheet.Cells[row + 2, col + 19].Value?.ToString() ?? "0",
        ["total_va"] = selectedWorksheet.Cells[row + 4, col + 17].Value?.ToString() ?? "0",
        ["lcl"] = selectedWorksheet.Cells[row + 7, col + 17].Value?.ToString() ?? "0",
        ["total_other_load"] = selectedWorksheet.Cells[row + 10, col + 17].Value?.ToString() ?? "0",
        ["kva"] = selectedWorksheet.Cells[row + 13, col + 17].Value?.ToString() ?? "0",
        ["feeder_amps"] = selectedWorksheet.Cells[row + 16, col + 17].Value?.ToString() ?? "0",
        ["existing"] = selectedWorksheet.Cells[row + 2, col + 20].Value?.ToString() ?? "",
        ["description_left_highlights"] = descriptionLeftHighlights,
        ["description_right_highlights"] = descriptionRightHighlights,
        ["breaker_left_highlights"] = breakerLeftHighlights,
        ["breaker_right_highlights"] = breakerRightHighlights,
        ["description_left"] = descriptionLeft,
        ["phase_a_left"] = phaseALeft,
        ["phase_b_left"] = phaseBLeft,
        ["phase_c_left"] = phaseCLeft,
        ["breaker_left"] = breakerLeft,
        ["circuit_left"] = circuitLeft,
        ["circuit_right"] = circuitRight,
        ["breaker_right"] = breakerRight,
        ["phase_a_right"] = phaseARight,
        ["phase_b_right"] = phaseBRight,
        ["phase_c_right"] = phaseCRight,
        ["description_right"] = descriptionRight,
      };

      ReplaceInPanel(panel, "voltage2", "V");
      ReplaceInPanel(panel, "phase", "PH");
      ReplaceInPanel(panel, "wire", "W");

      return panel;
    }

    private static Dictionary<string, object> ProcessTwoPolePanel(ExcelWorksheet selectedWorksheet, int row, int col)
    {
      List<string> descriptionLeft = new List<string>();
      List<string> phaseALeft = new List<string>();
      List<string> phaseBLeft = new List<string>();
      List<string> breakerLeft = new List<string>();
      List<string> circuitLeft = new List<string>();
      List<string> circuitRight = new List<string>();
      List<string> breakerRight = new List<string>();
      List<string> phaseARight = new List<string>();
      List<string> phaseBRight = new List<string>();
      List<string> descriptionRight = new List<string>();
      List<bool> descriptionLeftHighlights = new List<bool>();
      List<bool> descriptionRightHighlights = new List<bool>();
      List<bool> breakerLeftHighlights = new List<bool>();
      List<bool> breakerRightHighlights = new List<bool>();

      int panelRow = row + 4;
      int lastRow = panelRow;

      // check for a circuit column cell value of NULL to end the loop
      while (selectedWorksheet.Cells[lastRow + 2, col + 6].Value != null)
      {
        lastRow += 2;
      }

      // add cell values to lists
      for (int i = panelRow; i <= lastRow; i++)
      {
        string description = selectedWorksheet.Cells[i, col].Value?.ToString().ToUpper() ?? "SPACE";
        string phaseA = selectedWorksheet.Cells[i, col + 3].Value?.ToString() ?? "0";
        string phaseB = selectedWorksheet.Cells[i, col + 4].Value?.ToString() ?? "0";
        string breakerL = selectedWorksheet.Cells[i, col + 5].Value?.ToString() ?? "";
        string circuitL = selectedWorksheet.Cells[i, col + 6].Value?.ToString() ?? "";
        string circuitR = selectedWorksheet.Cells[i, col + 7].Value?.ToString() ?? "";
        string breakerR = selectedWorksheet.Cells[i, col + 8].Value?.ToString() ?? "";
        string phaseAR = selectedWorksheet.Cells[i, col + 9].Value?.ToString() ?? "0";
        string phaseBR = selectedWorksheet.Cells[i, col + 10].Value?.ToString() ?? "0";
        string descriptionR = selectedWorksheet.Cells[i, col + 11].Value?.ToString().ToUpper() ?? "SPACE";

        bool isLeftHighlighted = selectedWorksheet.Cells[i, col].Style.Fill.BackgroundColor.LookupColor() != "#FF000000";
        bool isRightHighlighted = selectedWorksheet.Cells[i, col + 11].Style.Fill.BackgroundColor.LookupColor() != "#FF000000";
        bool isLeftBreakerHighlighted = selectedWorksheet.Cells[i, col + 5].Style.Fill.BackgroundColor.LookupColor() != "#FF000000";
        bool isRightBreakerHighlighted = selectedWorksheet.Cells[i, col + 8].Style.Fill.BackgroundColor.LookupColor() != "#FF000000";

        descriptionLeft.Add(description);
        phaseALeft.Add(phaseA);
        phaseBLeft.Add(phaseB);
        breakerLeft.Add(breakerL);
        circuitLeft.Add(circuitL);
        circuitRight.Add(circuitR);
        breakerRight.Add(breakerR);
        phaseARight.Add(phaseAR);
        phaseBRight.Add(phaseBR);
        descriptionRight.Add(descriptionR);
        descriptionLeftHighlights.Add(isLeftHighlighted);
        descriptionRightHighlights.Add(isRightHighlighted);
        breakerLeftHighlights.Add(isLeftBreakerHighlighted);
        breakerRightHighlights.Add(isRightBreakerHighlighted);
      }

      string panelCellValue = selectedWorksheet.Cells[row, col + 2].Value?.ToString() ?? "";
      if (!panelCellValue.StartsWith("'") || !panelCellValue.EndsWith("'"))
      {
        panelCellValue = "'" + panelCellValue.Trim('\'') + "'";
      }

      var panel = new Dictionary<string, object>
      {
        ["panel"] = panelCellValue,
        ["location"] = selectedWorksheet.Cells[row, col + 5].Value?.ToString() ?? "",
        ["bus_rating"] = selectedWorksheet.Cells[row, col + 9].Value?.ToString() ?? "",
        ["voltage1"] = selectedWorksheet.Cells[row, col + 10].Value?.ToString() ?? "0",
        ["voltage2"] = selectedWorksheet.Cells[row, col + 11].Value?.ToString() ?? "0",
        ["phase"] = selectedWorksheet.Cells[row, col + 12].Value?.ToString() ?? "",
        ["wire"] = selectedWorksheet.Cells[row, col + 13].Value?.ToString() ?? "",
        ["main"] = selectedWorksheet.Cells[row + 1, col + 5].Value?.ToString() ?? "",
        ["mounting"] = selectedWorksheet.Cells[row + 1, col + 12].Value?.ToString() ?? "",
        ["subtotal_a"] = selectedWorksheet.Cells[row + 2, col + 17].Value?.ToString() ?? "0",
        ["subtotal_b"] = selectedWorksheet.Cells[row + 2, col + 18].Value?.ToString() ?? "0",
        ["subtotal_c"] = selectedWorksheet.Cells[row + 2, col + 19].Value?.ToString() ?? "0",
        ["total_va"] = selectedWorksheet.Cells[row + 4, col + 17].Value?.ToString() ?? "0",
        ["lcl"] = selectedWorksheet.Cells[row + 7, col + 17].Value?.ToString() ?? "0",
        ["total_other_load"] = selectedWorksheet.Cells[row + 10, col + 17].Value?.ToString() ?? "0",
        ["kva"] = selectedWorksheet.Cells[row + 13, col + 17].Value?.ToString() ?? "0",
        ["feeder_amps"] = selectedWorksheet.Cells[row + 16, col + 17].Value?.ToString() ?? "0",
        ["existing"] = selectedWorksheet.Cells[row + 2, col + 20].Value?.ToString() ?? "",
        ["description_left_highlights"] = descriptionLeftHighlights,
        ["description_right_highlights"] = descriptionRightHighlights,
        ["breaker_left_highlights"] = breakerLeftHighlights,
        ["breaker_right_highlights"] = breakerRightHighlights,
        ["description_left"] = descriptionLeft,
        ["phase_a_left"] = phaseALeft,
        ["phase_b_left"] = phaseBLeft,
        ["breaker_left"] = breakerLeft,
        ["circuit_left"] = circuitLeft,
        ["circuit_right"] = circuitRight,
        ["breaker_right"] = breakerRight,
        ["phase_a_right"] = phaseARight,
        ["phase_b_right"] = phaseBRight,
        ["description_right"] = descriptionRight,
      };

      ReplaceInPanel(panel, "voltage2", "V");
      ReplaceInPanel(panel, "phase", "PH");
      ReplaceInPanel(panel, "wire", "W");

      return panel;
    }

    private static void ReplaceInPanel(Dictionary<string, object> panel, string key, string toRemove)
    {
      if (panel.ContainsKey(key))
      {
        if (panel[key] is string value)
        {
          value = value.Replace(toRemove, "");
          panel[key] = value;
        }
      }
    }

    private static List<Dictionary<string, object>> ProcessWorksheet(ExcelWorksheet selectedWorksheet)
    {
      var (doc, db, ed) = GetGlobals();
      var panels = new List<Dictionary<string, object>>();
      int rowCount = selectedWorksheet.Dimension.Rows;
      int colCount = selectedWorksheet.Dimension.Columns;

      for (int row = 1; row <= rowCount; row++)
      {
        for (int col = 1; col <= colCount; col++)
        {
          string cellValue = selectedWorksheet.Cells[row, col].Value?.ToString();
          string phaseAMaybe = selectedWorksheet.Cells[row + 3, col + 2].Value?.ToString();
          if (cellValue == "PANEL:" && phaseAMaybe == "PH A")
          {
            panels.Add(ProcessThreePolePanel(selectedWorksheet, row, col));
          }
          else if (cellValue == "PANEL:")
          {
            panels.Add(ProcessTwoPolePanel(selectedWorksheet, row, col));
          }
        }
      }
      return panels;
    }

    private static void HandleExceptions(System.Exception ex)
    {
      Console.WriteLine(ex.Message);
    }

    public void KeepBreakersGivenPoints(Point3d point1, Point3d point2, Point3d point3)
    {
      var (doc, db, ed) = MyCommands.GetGlobals();

      using (var tr = db.TransactionManager.StartTransaction())
      {
        var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);

        // Get the active space block table record instead of the paperspace block table record
        var activeSpaceId = db.CurrentSpaceId;
        var btr = (BlockTableRecord)tr.GetObject(activeSpaceId, OpenMode.ForWrite);

        // Rest of the code remains the same...

        // Swap the points if the y-coordinate of the first point is lower than that of the second point
        if (point1.Y < point2.Y)
        {
          Point3d tempPoint = point1;
          point1 = point2;
          point2 = tempPoint;
        }

        var direction = point3.X > point1.X ? 1 : -1;
        var dist = (point1 - point2).Length;

        var line1Start = new Point3d(point1.X + direction * 0.05, point1.Y, 0);
        var line1End = new Point3d(line1Start.X + direction * 0.2, line1Start.Y, 0);
        var line2Start = new Point3d(line1Start.X, point2.Y, 0);
        var line2End = new Point3d(line1End.X, line2Start.Y, 0);

        string layerName = CreateOrGetLayer("E-TEXT", db, tr);

        var line1 = new Line(line1Start, line1End) { Layer = layerName, Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(ColorMethod.ByLayer, 2) };
        var line2 = new Line(line2Start, line2End) { Layer = layerName, Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(ColorMethod.ByLayer, 2) };

        var mid1 = new Point3d((line1Start.X + line1End.X) / 2, (line1Start.Y + line1End.Y) / 2, 0);
        var mid2 = new Point3d((line2Start.X + line2End.X) / 2, (line2Start.Y + line2End.Y) / 2, 0);
        var mid3 = new Point3d((mid1.X + mid2.X) / 2, (mid1.Y + mid2.Y) / 2, 0);

        var circleTop = new Point3d(mid3.X, mid3.Y + 0.09, 0);
        var circleBottom = new Point3d(mid3.X, mid3.Y - 0.09, 0);

        var line3 = new Line(mid1, circleTop) { Layer = layerName, Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(ColorMethod.ByLayer, 2) };
        var line4 = new Line(mid2, circleBottom) { Layer = layerName, Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(ColorMethod.ByLayer, 2) };

        if (dist > 0.3)
        {
          btr.AppendEntity(line1);
          btr.AppendEntity(line2);
          tr.AddNewlyCreatedDBObject(line1, true);
          tr.AddNewlyCreatedDBObject(line2, true);
        }

        var blkRef = new BlockReference(mid3, bt["CIRCLEI"]) { Layer = layerName, Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(ColorMethod.ByLayer, 2) };
        btr.AppendEntity(blkRef);
        tr.AddNewlyCreatedDBObject(blkRef, true);

        if (dist > 0.3)
        {
          btr.AppendEntity(line3);
          btr.AppendEntity(line4);
          tr.AddNewlyCreatedDBObject(line3, true);
          tr.AddNewlyCreatedDBObject(line4, true);
        }

        tr.Commit();
      }
    }

    private static List<Dictionary<string, object>> ImportExcelData()
    {
      var (doc, db, ed) = GetGlobals();

      var openFileDialog = new System.Windows.Forms.OpenFileDialog
      {
        Filter = "Excel Files|*.xlsx;*.xls",
        Title = "Select Excel File"
      };

      List<Dictionary<string, object>> panels = new List<Dictionary<string, object>>();

      if (openFileDialog.ShowDialog() == DialogResult.OK)
      {
        string filePath = openFileDialog.FileName;
        try
        {
          FileInfo fileInfo = GetFileInfo(filePath);
          using (var package = GetExcelPackage(fileInfo))
          {
            ExcelWorkbook workbook = package.Workbook;
            if (ValidateWorkbook(workbook))
            {
              foreach (var selectedWorksheet in workbook.Worksheets)
              {
                if (selectedWorksheet.Name.ToLower().Contains("panel"))
                {
                  panels.AddRange(ProcessWorksheet(selectedWorksheet));
                }
              }
            }
          }
        }
        catch (FileNotFoundException ex)
        {
          HandleExceptions(ex);
        }
        catch (Autodesk.AutoCAD.Runtime.Exception ex)
        {
          HandleExceptions(ex);
        }
      }
      else
      {
        Console.WriteLine("No file selected.");
      }

      return panels;
    }

    public string CreateOrGetLayer(string layerName, Database db, Transaction tr)
    {
      var lt = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);

      if (!lt.Has(layerName)) // check if layer exists
      {
        lt.UpgradeOpen(); // switch to write mode
        LayerTableRecord ltr = new LayerTableRecord();
        ltr.Name = layerName;
        lt.Add(ltr);
        tr.AddNewlyCreatedDBObject(ltr, true);
      }

      return layerName;
    }

    private void CreateCalculations(BlockTableRecord btr, Transaction tr, Point3d startPoint, Point3d endPoint, Dictionary<string, object> panelData)
    {
      var (_, _, ed) = GetGlobals();
      double kvaValue;
      double feederAmpsValue;
      var kvaParseResult = double.TryParse(panelData["kva"] as string, out kvaValue);
      var feederAmpsParseResult = double.TryParse(panelData["feeder_amps"] as string, out feederAmpsValue);

      ed.WriteMessage($"KVA VALUE: {panelData["kva"] as string}\n");
      ed.WriteMessage($"FEEDER AMPS VALUE: {panelData["feeder_amps"] as string}\n");

      if (kvaParseResult)
      {
        CreateAndPositionRightText(tr, Math.Round(kvaValue, 1).ToString("0.0") + " KVA", "ROMANS", 0.09375, 1, 2, "PNLTXT", new Point3d(endPoint.X - 6.69695957617801, endPoint.Y - 0.785594790702817, 0));
      }
      else
      {
        ed.WriteMessage($"Error: Unable to convert 'kva' value to double: {panelData["kva"]}");
      }

      if (feederAmpsParseResult)
      {
        CreateAndPositionRightText(tr, Math.Round(feederAmpsValue, 1).ToString("0.0") + " A", "ROMANS", 0.09375, 1, 2, "PNLTXT", new Point3d(endPoint.X - 6.70142386189229, endPoint.Y - 0.970762733814496, 0));
      }
      else
      {
        ed.WriteMessage($"Error: Unable to convert 'feeder_amps' value to double: {panelData["feeder_amps"]}");
      }

      // Create the calculation lines
      CreateLine(tr, btr, endPoint.X - 6.17759999999998, endPoint.Y - 0.0846396524177919, endPoint.X - 8.98559999999998, endPoint.Y - 0.0846396524177919, "0");
      CreateLine(tr, btr, endPoint.X - 6.17759999999998, endPoint.Y - 1.02063965241777, endPoint.X - 6.17759999999998, endPoint.Y - 0.0846396524177919, "0");
      CreateLine(tr, btr, endPoint.X - 8.98559999999998, endPoint.Y - 1.02063965241777, endPoint.X - 8.98559999999998, endPoint.Y - 0.0846396524177919, "0");
      CreateLine(tr, btr, endPoint.X - 6.17759999999998, endPoint.Y - 1.02063965241777, endPoint.X - 8.98559999999998, endPoint.Y - 1.02063965241777, "0");
      CreateLine(tr, btr, endPoint.X - 6.17759999999998, endPoint.Y - 0.833439652417809, endPoint.X - 8.98559999999998, endPoint.Y - 0.833439652417809, "0");
      CreateLine(tr, btr, endPoint.X - 6.17759999999998, endPoint.Y - 0.64623965241779, endPoint.X - 8.98559999999998, endPoint.Y - 0.64623965241779, "0");
      CreateLine(tr, btr, endPoint.X - 6.17759999999998, endPoint.Y - 0.459039652417772, endPoint.X - 8.98559999999998, endPoint.Y - 0.459039652417772, "0");
      CreateLine(tr, btr, endPoint.X - 6.17759999999998, endPoint.Y - 0.27183965241781, endPoint.X - 8.98559999999998, endPoint.Y - 0.27183965241781, "0");
      CreateLine(tr, btr, endPoint.X - 6.17759999999998, endPoint.Y - 0.0846396524177919, endPoint.X - 8.98559999999998, endPoint.Y - 0.0846396524177919, "0");

      // Create the text
      CreateAndPositionText(tr, "TOTAL CONNECTED VA", "Standard", 0.1248, 0.75, 256, "0", new Point3d(endPoint.X - 8.93821353998555, endPoint.Y - 0.244065644556514, 0));
      CreateAndPositionRightText(tr, panelData["total_va"] as string, "ROMANS", 0.09375, 1, 2, "PNLTXT", new Point3d(endPoint.X - 6.69695957617801, endPoint.Y - 0.222040136230106, 0));
      CreateAndPositionText(tr, "=", "Standard", 0.1248, 0.75, 256, "0", new Point3d(endPoint.X - 7.03028501835593, endPoint.Y - 0.242614932747216, 0));
      CreateAndPositionText(tr, "LCL @ 125 %          ", "Standard", 0.1248, 0.75, 256, "0", new Point3d(endPoint.X - 8.91077927366155, endPoint.Y - 0.432165907882307, 0));
      CreateAndPositionRightText(tr, "0", "ROMANS", 0.09375, 1, 2, "PNLTXT", new Point3d(endPoint.X - 7.59414061117746, endPoint.Y - 0.413648726513742, 0));
      CreateAndPositionText(tr, "=", "Standard", 0.1248, 0.75, 256, "0", new Point3d(endPoint.X - 7.03028501835593, endPoint.Y - 0.437756414851634, 0));
      CreateAndPositionRightText(tr, panelData["lcl"] as string, "ROMANS", 0.09375, 1, 2, "PNLTXT", new Point3d(endPoint.X - 6.69695957617801, endPoint.Y - 0.413648726513742, 0));
      CreateAndPositionText(tr, "TOTAL OTHER LOAD", "Standard", 0.1248, 0.75, 256, "0", new Point3d(endPoint.X - 8.9456956126196, endPoint.Y - 0.616854044919108, 0));
      CreateAndPositionText(tr, "=", "Standard", 0.1248, 0.75, 256, "0", new Point3d(endPoint.X - 7.03028501835593, endPoint.Y - 0.618180694030713, 0));
      CreateAndPositionRightText(tr, panelData["total_other_load"] as string, "ROMANS", 0.09375, 1, 2, "PNLTXT", new Point3d(endPoint.X - 6.69695957617801, endPoint.Y - 0.597206513223341, 0));
      CreateAndPositionText(tr, "PANEL LOAD", "Standard", 0.1248, 0.75, 256, "0", new Point3d(endPoint.X - 8.92075537050664, endPoint.Y - 0.804954308244959, 0));
      CreateAndPositionText(tr, "=", "Standard", 0.1248, 0.75, 256, "0", new Point3d(endPoint.X - 7.03028501835593, endPoint.Y - 0.809218166102625, 0));
      CreateAndPositionText(tr, "FEEDER AMPS", "Standard", 0.1248, 0.75, 256, "0", new Point3d(endPoint.X - 8.9120262857673, endPoint.Y - 0.994381220682413, 0));
      CreateAndPositionText(tr, "=", "Standard", 0.1248, 0.75, 256, "0", new Point3d(endPoint.X - 7.03028501835593, endPoint.Y - 0.998928989062989, 0));
    }

    private void CreateNotes(BlockTableRecord btr, Transaction tr, Point3d startPoint, Point3d endPoint, string panelType)
    {
      if (panelType.ToLower() == "existing")
      {
        CreateAndPositionText(tr, "(EXISTING PANEL)", "ROMANC", 0.1498, 0.75, 2, "0", new Point3d(startPoint.X + 0.236635303895696, startPoint.Y + 0.113254677317428, 0));

        // Create the text
        CreateAndPositionText(tr, "NOTES:", "Standard", 0.1248, 0.75, 256, "0", new Point3d(endPoint.X - 5.96783070435049, endPoint.Y - 0.23875904811004, 0));
        CreateAndPositionText(tr, "DENOTES EXISTING CIRCUIT BREAKER TO REMAIN; ALL OTHERS ARE NEW", "ROMANS", 0.09375, 1, 2, "0", new Point3d(endPoint.X - 5.61904201783966, endPoint.Y - 0.405747901076808, 0));
        CreateAndPositionText(tr, "TO MATCH EXISTING.", "ROMANS", 0.09375, 1, 2, "0", new Point3d(endPoint.X - 5.61904201783966, endPoint.Y - 0.610352149436778, 0));
      }
      else if (panelType.ToLower() == "relocated")
      {
        CreateAndPositionText(tr, "(EXISTING TO BE RELOCATED PANEL)", "ROMANC", 0.1498, 0.75, 2, "0", new Point3d(startPoint.X + 0.236635303895696, startPoint.Y + 0.113254677317428, 0));

        // Create the text
        CreateAndPositionText(tr, "NOTES:", "Standard", 0.1248, 0.75, 256, "0", new Point3d(endPoint.X - 5.96783070435049, endPoint.Y - 0.23875904811004, 0));
        CreateAndPositionText(tr, "DENOTES EXISTING CIRCUIT BREAKER TO REMAIN; ALL OTHERS ARE NEW", "ROMANS", 0.09375, 1, 2, "0", new Point3d(endPoint.X - 5.61904201783966, endPoint.Y - 0.405747901076808, 0));
        CreateAndPositionText(tr, "TO MATCH EXISTING.", "ROMANS", 0.09375, 1, 2, "0", new Point3d(endPoint.X - 5.61904201783966, endPoint.Y - 0.610352149436778, 0));
      }
      else
      {
        CreateAndPositionText(tr, "(NEW PANEL)", "ROMANC", 0.1498, 0.75, 2, "0", new Point3d(startPoint.X + 0.236635303895696, startPoint.Y + 0.113254677317428, 0));

        // Create the text
        CreateAndPositionText(tr, "NOTES:", "Standard", 0.1248, 0.75, 256, "0", new Point3d(endPoint.X - 5.96783070435049, endPoint.Y - 0.23875904811004, 0));
        CreateAndPositionText(tr, "65 KAIC SERIES RATED OR MATCH FAULT CURRENT AT SITE.", "ROMANS", 0.09375, 1, 2, "0", new Point3d(endPoint.X - 5.61904201783966, endPoint.Y - 0.405747901076808, 0));
      }

      // Create the lines
      CreateLine(tr, btr, endPoint.X, endPoint.Y - 0.0846396524177919, endPoint.X - 6.07359999999994, endPoint.Y - 0.0846396524177919, "0");
      CreateLine(tr, btr, endPoint.X, endPoint.Y - 0.0846396524177919, endPoint.X, endPoint.Y - 1.02063965241777, "0");
      CreateLine(tr, btr, endPoint.X - 6.07359999999994, endPoint.Y - 1.02063965241777, endPoint.X - 6.07359999999994, endPoint.Y + -0.0846396524177919, "0");
      CreateLine(tr, btr, endPoint.X, endPoint.Y - 1.02063965241777, endPoint.X - 6.07359999999994, endPoint.Y - 1.02063965241777, "0");
      CreateLine(tr, btr, endPoint.X, endPoint.Y - 0.27183965241781, endPoint.X - 6.07359999999994, endPoint.Y - 0.27183965241781, "0");
      CreateLine(tr, btr, endPoint.X, endPoint.Y - 0.459039652417772, endPoint.X - 6.07359999999994, endPoint.Y - 0.459039652417772, "0");
      CreateLine(tr, btr, endPoint.X, endPoint.Y - 0.64623965241779, endPoint.X - 6.07359999999994, endPoint.Y + -0.64623965241779, "0");
      CreateLine(tr, btr, endPoint.X, endPoint.Y + -0.833439652417809, endPoint.X - 6.07359999999994, endPoint.Y + -0.833439652417809, "0");

      // Create the circle
      CreateCircle(btr, tr, new Point3d(endPoint.X - 5.8088, endPoint.Y - 0.3664, 0), 0.09, 2, false);

      // Create the 1
      CreateAndPositionCenteredText(tr, "1", "ROMANS", 0.09375, 1, 2, "0", new Point3d(endPoint.X - 5.85897687070053 - 0.145, endPoint.Y - 0.410151417346867, 0));
    }

    private void CreateCenterLines(BlockTableRecord btr, Transaction tr, Point3d startPoint, Point3d endPoint, bool is2Pole)
    {
      // Create horizontal line above
      CreateLine(tr, btr, startPoint.X, endPoint.Y + 0.2533, endPoint.X, endPoint.Y + 0.2533, "0");

      if (is2Pole)
      {
        // Create the slashed line
        CreateLine(tr, btr, endPoint.X - 6.47536463134611, endPoint.Y + 0.0841286798547145, endPoint.X - 6.3618865297326, endPoint.Y + 0.216793591015815, "0");
        CreateLine(tr, btr, endPoint.X - 4.541248855498, endPoint.Y + 0.0739331046861764, endPoint.X - 4.42777075388449, endPoint.Y + 0.206598015847277, "0");

        // Create the vertical center lines
        CreateLine(tr, btr, endPoint.X - 4.67999999999984, endPoint.Y + 0.277148912917994, endPoint.X - 4.67999999999984, startPoint.Y - 0.7488, "0");
        CreateLine(tr, btr, endPoint.X - 4.30668381424084, endPoint.Y + 0.277148912917994, endPoint.X - 4.30668381424084, startPoint.Y - 0.7488, "0");
      }
      else
      {
        // Create the slashed line
        CreateLine(tr, btr, endPoint.X - 6.47536463134611, endPoint.Y + 0.0663322399757078, endPoint.X - 6.36188652973283, endPoint.Y + 0.198997151136808, "0");
        CreateLine(tr, btr, endPoint.X - 4.63023473191822, endPoint.Y + 0.0663322399757078, endPoint.X - 4.51675663030494, endPoint.Y + 0.198997151136808, "0");
        CreateLine(tr, btr, endPoint.X - 2.06710862405464, endPoint.Y + 0.0663322399757078, endPoint.X - 1.95363052244136, endPoint.Y + 0.198997151136808, "0");

        // Create the vertical center lines
        CreateLine(tr, btr, endPoint.X - 4.30559999999991, endPoint.Y + 0.253292187434056, endPoint.X - 4.30559999999991, startPoint.Y - 0.7488, "0");
        CreateLine(tr, btr, endPoint.X - 4.49279999999999, endPoint.Y + 0.253292187434056, endPoint.X - 4.49279999999999, startPoint.Y - 0.7488, "0");
        CreateLine(tr, btr, endPoint.X - 4.67999999999984, endPoint.Y + 0.253292187434056, endPoint.X - 4.67999999999984, startPoint.Y - 0.7488, "0");
      }
      // Create the circle and lines in the center of the panel
      CreateCenterLinePattern(tr, btr, startPoint, endPoint, is2Pole);
    }

    private void CreateCenterLinePattern(Transaction tr, BlockTableRecord btr, Point3d startPoint, Point3d endPoint, bool is2Pole)
    {
      double maxY = endPoint.Y + 0.2533;
      double increaseY = 0.1872;
      double increaseX = 0.1872;
      double baseX = startPoint.X + 4.3056;
      double currentX = baseX;
      double currentY = startPoint.Y - 0.8424;
      int num = 3;

      if (is2Pole)
      {
        baseX = startPoint.X + 4.30560000000014;
        currentX = baseX;
        increaseX = 0.3733;
        num = 2;
      }

      bool conditionMet = false;

      while (currentY >= maxY && !conditionMet)
      {
        for (int i = 0; i < num; i++)
        {
          if (currentY < maxY)
          {
            conditionMet = true;
            break;
          }

          // Create the center line circles
          CreateCircle(btr, tr, new Point3d(currentX, currentY, 0), 0.0312, 7);

          // Create the horizontal center lines
          CreateLine(tr, btr, startPoint.X + 4.22905693965708, currentY, startPoint.X + 4.75654306034312, currentY, "0");

          currentX += increaseX;
          currentY -= increaseY;
        }

        // reset x value
        currentX = baseX;
      }
    }

    public void CreateCircle(BlockTableRecord btr, Transaction tr, Point3d center, double radius, int colorIndex, bool doHatch = true)
    {
      using (Circle circle = new Circle())
      {
        circle.Center = center;
        circle.Radius = radius;
        circle.ColorIndex = colorIndex;  // Setting the color
        circle.Layer = "0";  // Setting the layer to "0"
        btr.AppendEntity(circle);
        tr.AddNewlyCreatedDBObject(circle, true);

        if (doHatch)
        {
          // Creating a Hatch
          using (Hatch hatch = new Hatch())
          {
            hatch.Layer = "0";  // Setting the layer to "0"
            btr.AppendEntity(hatch);
            tr.AddNewlyCreatedDBObject(hatch, true);
            hatch.SetHatchPattern(HatchPatternType.PreDefined, "SOLID");
            hatch.Associative = true;

            // Associating the hatch with the circle
            ObjectIdCollection objIds = new ObjectIdCollection();
            objIds.Add(circle.ObjectId);
            hatch.AppendLoop(HatchLoopTypes.Default, objIds);
            hatch.EvaluateHatch(true);
          }
        }
      }
    }

    private void CreateFooterText(Transaction tr, Point3d endPoint, Dictionary<string, object> panelData, bool is2Pole)
    {
      if (is2Pole)
      {
        CreateAndPositionText(tr, "SUB-TOTAL", "Standard", 0.1248, 0.75, 256, "0", new Point3d(endPoint.X - 8.91077927366177, endPoint.Y + 0.0697891578365528, 0));
        CreateAndPositionText(tr, "OA", "Standard", 0.1248, 0.75, 256, "0", new Point3d(endPoint.X - 6.45042438923338, endPoint.Y + 0.0920885745242259, 0));
        CreateAndPositionText(tr, "OB", "Standard", 0.1248, 0.75, 256, "0", new Point3d(endPoint.X - 4.51630861338526, endPoint.Y + 0.0818929993556878, 0));
        CreateAndPositionText(tr, "=", "Standard", 0.1248, 0.75, 256, "0", new Point3d(endPoint.X - 6.24591440390805, endPoint.Y + 0.0920885745242259, 0));
        CreateAndPositionText(tr, "=", "Standard", 0.1248, 0.75, 256, "0", new Point3d(endPoint.X - 4.32551576122205, endPoint.Y + 0.0818929993556878, 0));
        CreateAndPositionText(tr, panelData["subtotal_a"] as string + "VA", "ROMANS", 0.09375, 1, 2, "0", new Point3d(endPoint.X - 6.08199405082502, endPoint.Y + 0.108280531454625, 0));
        CreateAndPositionText(tr, panelData["subtotal_b"] as string + "VA", "ROMANS", 0.09375, 1, 2, "0", new Point3d(endPoint.X - 4.15962890179264, endPoint.Y + 0.0980849562860868, 0));
      }
      else
      {
        CreateAndPositionText(tr, "SUB-TOTAL", "Standard", 0.1248, 1, 7, "0", new Point3d(endPoint.X - 8.91077927366155, endPoint.Y + 0.0689855381989162, 0));
        CreateAndPositionText(tr, "OA", "Standard", 0.1248, 0.75, 7, "0", new Point3d(endPoint.X - 6.45042438923338, endPoint.Y + 0.0742921346453898, 0));
        CreateAndPositionText(tr, "OB", "Standard", 0.1248, 0.75, 7, "0", new Point3d(endPoint.X - 4.60529448980549, endPoint.Y + 0.0742921346453898, 0));
        CreateAndPositionText(tr, "OC", "Standard", 0.1248, 0.75, 7, "0", new Point3d(endPoint.X - 2.04216838194191, endPoint.Y + 0.0742921346453898, 0));
        CreateAndPositionText(tr, panelData["subtotal_a"] as string + "VA", "ROMANS", 0.09375, 1, 2, "0", new Point3d(endPoint.X - 6.07732066030258, endPoint.Y + 0.0948263267698053, 0));
        CreateAndPositionText(tr, panelData["subtotal_b"] as string + "VA", "ROMANS", 0.09375, 1, 2, "0", new Point3d(endPoint.X - 4.23219076087469, endPoint.Y + 0.0948263267698053, 0));
        CreateAndPositionText(tr, panelData["subtotal_c"] as string + "VA", "ROMANS", 0.09375, 1, 2, "0", new Point3d(endPoint.X - 1.66906465301099, endPoint.Y + 0.0948263267698053, 0));
        CreateAndPositionText(tr, "=", "Standard", 0.1248, 0.75, 256, "0", new Point3d(endPoint.X - 6.24591440390827, endPoint.Y + 0.0742921346453898, 0));
        CreateAndPositionText(tr, "=", "Standard", 0.1248, 0.75, 256, "0", new Point3d(endPoint.X - 4.40078450448038, endPoint.Y + 0.0742921346453898, 0));
        CreateAndPositionText(tr, "=", "Standard", 0.1248, 0.75, 256, "0", new Point3d(endPoint.X - 1.8376583966168, endPoint.Y + 0.0742921346453898, 0));
      }
    }

    private double GetEndOfDataY(List<string> list, Point3d startPoint)
    {
      var rowHeight = 0.1872;
      var headerHeight = 0.7488;
      return startPoint.Y - (headerHeight + (rowHeight * ((list.Count + 1) / 2)));
    }

    private void CreateRectangle(BlockTableRecord btr, Transaction tr, Point3d topRightCorner, Point3d startPoint, Point3d endPoint, string layerName)
    {
      // Create the rectangle
      var rect = new Autodesk.AutoCAD.DatabaseServices.Polyline(4);
      rect.AddVertexAt(0, new Point2d(startPoint.X, startPoint.Y), 0, 0, 0);
      rect.AddVertexAt(1, new Point2d(startPoint.X, endPoint.Y), 0, 0, 0);
      rect.AddVertexAt(2, new Point2d(endPoint.X, endPoint.Y), 0, 0, 0);
      rect.AddVertexAt(3, new Point2d(endPoint.X, startPoint.Y), 0, 0, 0);
      rect.Closed = true;

      // Set the global width property
      rect.ConstantWidth = 0.02;

      // Set the layer to "0"
      rect.Layer = layerName;

      btr.AppendEntity(rect);
      tr.AddNewlyCreatedDBObject(rect, true);
    }

    private static ObjectId CreateText(string content, string style, TextHorizontalMode horizontalMode, TextVerticalMode verticalMode, double height, double widthFactor, Autodesk.AutoCAD.Colors.Color color, string layer)
    {
      var (doc, db, _) = MyCommands.GetGlobals();

      // Check if the layer exists
      using (var tr = db.TransactionManager.StartTransaction())
      {
        var layerTable = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);

        if (!layerTable.Has(layer))
        {
          // Layer doesn't exist, create it
          var newLayer = new LayerTableRecord();
          newLayer.Name = layer;

          layerTable.UpgradeOpen();
          layerTable.Add(newLayer);
          tr.AddNewlyCreatedDBObject(newLayer, true);
        }

        tr.Commit();
      }

      using (var tr = doc.TransactionManager.StartTransaction())
      {
        var textStyleId = GetTextStyleId(style);
        var textStyle = (TextStyleTableRecord)tr.GetObject(textStyleId, OpenMode.ForRead);

        var text = new DBText
        {
          TextString = content,
          Height = height,
          WidthFactor = widthFactor,
          Color = color,
          Layer = layer,
          TextStyleId = textStyleId,
          HorizontalMode = horizontalMode,
          VerticalMode = verticalMode,
          Justify = AttachmentPoint.BaseLeft
        };

        var currentSpace = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
        currentSpace.AppendEntity(text);
        tr.AddNewlyCreatedDBObject(text, true);

        tr.Commit();

        return text.ObjectId;
      }
    }

    private static void CreateAndPositionText(Transaction tr, string content, string style, double height, double widthFactor, int colorIndex, string layerName, Point3d position, TextHorizontalMode horizontalMode = TextHorizontalMode.TextLeft, TextVerticalMode verticalMode = TextVerticalMode.TextBase)
    {
      var color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, (short)colorIndex);
      var textId = CreateText(content, style, horizontalMode, verticalMode, height, widthFactor, color, layerName);
      var text = (DBText)tr.GetObject(textId, OpenMode.ForWrite);
      text.Position = position;
    }

    private void CreateAndPositionFittedText(Transaction tr, string content, string style, double height, double widthFactor, int colorIndex, string layerName, Point3d position, double length, TextHorizontalMode horizontalMode = TextHorizontalMode.TextLeft, TextVerticalMode verticalMode = TextVerticalMode.TextBase)
    {
      var color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, (short)colorIndex);
      var textId = CreateText(content, style, horizontalMode, verticalMode, height, widthFactor, color, layerName);
      var text = (DBText)tr.GetObject(textId, OpenMode.ForWrite);

      double naturalWidth = text.GeometricExtents.MaxPoint.X - text.GeometricExtents.MinPoint.X;
      text.WidthFactor = length / naturalWidth; // This will stretch or squeeze text to fit between points
      text.Position = position;
    }

    private void CreateAndPositionCenteredText(Transaction tr, string content, string style, double height, double widthFactor, int colorIndex, string layerName, Point3d position, TextHorizontalMode horizontalMode = TextHorizontalMode.TextLeft, TextVerticalMode verticalMode = TextVerticalMode.TextBase)
    {
      var color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, (short)colorIndex);
      var textId = CreateText(content, style, horizontalMode, verticalMode, height, widthFactor, color, layerName);
      var text = (DBText)tr.GetObject(textId, OpenMode.ForWrite);
      text.Justify = AttachmentPoint.BaseCenter;
      double x = position.X;
      text.AlignmentPoint = new Point3d(x + 0.1903, position.Y, 0);
    }

    private void CreateAndPositionRightText(Transaction tr, string content, string style, double height, double widthFactor, int colorIndex, string layerName, Point3d position, TextHorizontalMode horizontalMode = TextHorizontalMode.TextLeft, TextVerticalMode verticalMode = TextVerticalMode.TextBase)
    {
      var color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, (short)colorIndex);
      var textId = CreateText(content, style, horizontalMode, verticalMode, height, widthFactor, color, layerName);
      var text = (DBText)tr.GetObject(textId, OpenMode.ForWrite);
      text.Justify = AttachmentPoint.BaseRight;
      double x = position.X;
      text.AlignmentPoint = new Point3d(x + 0.46, position.Y, 0);
    }

    private void CreateTextsWithoutPanelData(Transaction tr, string layerName, Point3d startPoint, bool is2Pole)
    {
      CreateAndPositionText(tr, "PANEL", "ROMANC", 0.1872, 0.75, 0, layerName, new Point3d(startPoint.X + 0.231944251649111, startPoint.Y - 0.299822699224023, 0));
      CreateAndPositionText(tr, "DESCRIPTION", "Standard", 0.1248, 0.75, 256, "0", new Point3d(startPoint.X + 0.305517965881791, startPoint.Y - 0.638118222684739, 0));
      CreateAndPositionText(tr, "W", "Standard", 0.101088, 0.75, 0, layerName, new Point3d(startPoint.X + 8.64365164909793, startPoint.Y - 0.155688865359394, 0));
      CreateAndPositionText(tr, "VOLT AMPS", "Standard", 0.11232, 0.75, 256, "0", new Point3d(startPoint.X + 1.9015733562577, startPoint.Y - 0.532524377875689, 0));
      CreateAndPositionText(tr, "L", "Standard", 0.07488, 0.75, 256, "0", new Point3d(startPoint.X + 2.97993751651882, startPoint.Y - 0.483601235896458, 0));
      CreateAndPositionText(tr, "T", "Standard", 0.07488, 0.75, 256, "0", new Point3d(startPoint.X + 2.97993751651882, startPoint.Y - 0.59526740969153, 0));
      CreateAndPositionText(tr, "G", "Standard", 0.07488, 0.75, 256, "0", new Point3d(startPoint.X + 2.97993751651882, startPoint.Y - 0.702157646684782, 0));
      CreateAndPositionText(tr, "R", "Standard", 0.07488, 0.75, 256, "0", new Point3d(startPoint.X + 3.20889406685785, startPoint.Y - 0.482921120531671, 0));
      CreateAndPositionText(tr, "E", "Standard", 0.07488, 0.75, 256, "0", new Point3d(startPoint.X + 3.20889406685785, startPoint.Y - 0.594587294326715, 0));
      CreateAndPositionText(tr, "C", "Standard", 0.07488, 0.75, 256, "0", new Point3d(startPoint.X + 3.20889406685785, startPoint.Y - 0.701477531319966, 0));
      CreateAndPositionText(tr, "M", "Standard", 0.07488, 0.75, 256, "0", new Point3d(startPoint.X + 3.43493724520761, startPoint.Y - 0.482921120531671, 0));
      CreateAndPositionText(tr, "I", "Standard", 0.07488, 0.75, 256, "0", new Point3d(startPoint.X + 3.4427934214732, startPoint.Y - 0.594587294326715, 0));
      CreateAndPositionText(tr, "S", "Standard", 0.07488, 0.75, 256, "0", new Point3d(startPoint.X + 3.43493724520761, startPoint.Y - 0.701477531319966, 0));
      CreateAndPositionText(tr, "BKR", "Standard", 0.09152, 0.75, 256, "0", new Point3d(startPoint.X + 3.63691080609988, startPoint.Y - 0.61662650707666, 0));
      CreateAndPositionText(tr, "CKT", "Standard", 0.0832, 0.75, 256, "0", new Point3d(startPoint.X + 3.94429929014041, startPoint.Y - 0.529332995532684, 0));
      CreateAndPositionText(tr, " NO", "Standard", 0.0832, 0.75, 256, "0", new Point3d(startPoint.X + 3.90688892697108, startPoint.Y - 0.673306258645766, 0));
      CreateAndPositionText(tr, "BUS", "Standard", 0.11232, 0.75, 256, "0", new Point3d(startPoint.X + 4.32282163085404, startPoint.Y - 0.527068325709052, 0));
      CreateAndPositionText(tr, "CKT", "Standard", 0.07488, 0.75, 256, "0", new Point3d(startPoint.X + 4.88897460099258, startPoint.Y - 0.535275052777223, 0));
      CreateAndPositionText(tr, " NO", "Standard", 0.07488, 0.75, 256, "0", new Point3d(startPoint.X + 4.85530527414039, startPoint.Y - 0.664850989579008, 0));
      CreateAndPositionText(tr, "BKR", "Standard", 0.082368, 0.75, 256, "0", new Point3d(startPoint.X + 5.14497871612878, startPoint.Y - 0.612478980835647, 0));
      CreateAndPositionText(tr, "M", "Standard", 0.07488, 0.75, 256, "0", new Point3d(startPoint.X + 5.4736003885796, startPoint.Y - 0.483601235896458, 0));
      CreateAndPositionText(tr, "I", "Standard", 0.07488, 0.75, 256, "0", new Point3d(startPoint.X + 5.48257887574016, startPoint.Y - 0.59526740969153, 0));
      CreateAndPositionText(tr, "S", "Standard", 0.07488, 0.75, 256, "0", new Point3d(startPoint.X + 5.4736003885796, startPoint.Y - 0.702157646684782, 0));
      CreateAndPositionText(tr, "R", "Standard", 0.07488, 0.75, 256, "0", new Point3d(startPoint.X + 5.70588736710022, startPoint.Y - 0.482921120531671, 0));
      CreateAndPositionText(tr, "E", "Standard", 0.07488, 0.75, 256, "0", new Point3d(startPoint.X + 5.70588736710022, startPoint.Y - 0.594587294326715, 0));
      CreateAndPositionText(tr, "C", "Standard", 0.07488, 0.75, 256, "0", new Point3d(startPoint.X + 5.70588736710022, startPoint.Y - 0.701477531319966, 0));
      CreateAndPositionText(tr, "L", "Standard", 0.07488, 0.75, 256, "0", new Point3d(startPoint.X + 5.93367350805136, startPoint.Y - 0.484281352862808, 0));
      CreateAndPositionText(tr, "T", "Standard", 0.07488, 0.75, 256, "0", new Point3d(startPoint.X + 5.93367350805136, startPoint.Y - 0.595947526657881, 0));
      CreateAndPositionText(tr, "G", "Standard", 0.07488, 0.75, 256, "0", new Point3d(startPoint.X + 5.93367350805136, startPoint.Y - 0.702837763651132, 0));
      CreateAndPositionText(tr, "VOLT AMPS", "Standard", 0.11232, 0.75, 256, "0", new Point3d(startPoint.X + 6.32453930091015, startPoint.Y - 0.532297673821773, 0));
      CreateAndPositionText(tr, "DESCRIPTION", "Standard", 0.1248, 0.75, 256, "0", new Point3d(startPoint.X + 7.68034755863846, startPoint.Y - 0.636791573573134, 0));
      CreateAndPositionText(tr, "LOCATION", "Standard", 0.11232, 0.75, 0, layerName, new Point3d(startPoint.X + 2.32067207718262, startPoint.Y - 0.155059196495415, 0));
      CreateAndPositionText(tr, "MAIN (AMP)", "Standard", 0.11232, 0.75, 0, layerName, new Point3d(startPoint.X + 2.32089885857886, startPoint.Y - 0.338479316609039, 0));
      CreateAndPositionText(tr, "BUS RATING", "Standard", 0.1248, 0.75, 0, layerName, new Point3d(startPoint.X + 5.18507633525223, startPoint.Y - 0.271963067880222, 0));
      CreateAndPositionText(tr, "MOUNTING:", "Standard", 0.11232, 0.75, 0, layerName, new Point3d(startPoint.X + 7.01560982102967, startPoint.Y - 0.329154148660905, 0));
      CreateAndPositionText(tr, "V", "Standard", 0.09984, 0.75, 0, layerName, new Point3d(startPoint.X + 7.80112268015148, startPoint.Y - 0.158231303238949, 0));
      CreateAndPositionText(tr, "O", "Standard", 0.101088, 0.75, 0, layerName, new Point3d(startPoint.X + 8.30325740318381, startPoint.Y - 0.151432601608803, 0));

      if (is2Pole)
      {
        CreateAndPositionText(tr, "OA", "Standard", 0.1248, 0.75, 256, "0", new Point3d(startPoint.X + 1.87939466183889, startPoint.Y - 0.720370467604425, 0));
        CreateAndPositionText(tr, "OB", "Standard", 0.1248, 0.75, 256, "0", new Point3d(startPoint.X + 2.50641160863188, startPoint.Y - 0.720370467604425, 0));
        CreateAndPositionText(tr, "OA", "Standard", 0.1248, 0.75, 256, "0", new Point3d(startPoint.X + 4.19245469916268, startPoint.Y - 0.720370467604425, 0));
        CreateAndPositionText(tr, "OB", "Standard", 0.1248, 0.75, 256, "0", new Point3d(startPoint.X + 4.59766144739842, startPoint.Y - 0.720370467604425, 0));
        CreateAndPositionText(tr, "OA", "Standard", 0.1248, 0.75, 256, "0", new Point3d(startPoint.X + 6.2528343633212, startPoint.Y - 0.720370467604425, 0));
        CreateAndPositionText(tr, "OB", "Standard", 0.1248, 0.75, 256, "0", new Point3d(startPoint.X + 6.91903366501083, startPoint.Y - 0.720370467604425, 0));
      }
      else
      {
        CreateAndPositionText(tr, "OA", "Standard", 0.11232, 0.75, 256, "0", new Point3d(startPoint.X + 1.75923320841673, startPoint.Y - 0.715823582939777, 0));
        CreateAndPositionText(tr, "OB", "Standard", 0.11232, 0.75, 256, "0", new Point3d(startPoint.X + 2.17200390182074, startPoint.Y - 0.714690056264089, 0));
        CreateAndPositionText(tr, "OC", "Standard", 0.11232, 0.75, 256, "0", new Point3d(startPoint.X + 2.57149885762158, startPoint.Y - 0.718725420176355, 0));
        CreateAndPositionText(tr, "OA", "Standard", 0.11232, 0.75, 256, "0", new Point3d(startPoint.X + 4.22229040316483, startPoint.Y - 0.714236644953189, 0));
        CreateAndPositionText(tr, "OB", "Standard", 0.11232, 0.75, 256, "0", new Point3d(startPoint.X + 4.42655098606872, startPoint.Y - 0.714236644953189, 0));
        CreateAndPositionText(tr, "OC", "Standard", 0.11232, 0.75, 256, "0", new Point3d(startPoint.X + 4.63417850165774, startPoint.Y - 0.713042660752734, 0));
        CreateAndPositionText(tr, "OA", "Standard", 0.11232, 0.75, 256, "0", new Point3d(startPoint.X + 6.22324655852697, startPoint.Y - 0.71537017323044, 0));
        CreateAndPositionText(tr, "OB", "Standard", 0.11232, 0.75, 256, "0", new Point3d(startPoint.X + 6.63397621936463, startPoint.Y - 0.714690057865624, 0));
        CreateAndPositionText(tr, "OC", "Standard", 0.11232, 0.75, 256, "0", new Point3d(startPoint.X + 7.03324439586629, startPoint.Y - 0.718272010467018, 0));
      }
    }

    private void CreateTextsWithPanelData(Transaction tr, string layerName, Point3d startPoint, Dictionary<string, object> panelData)
    {
      CreateAndPositionText(tr, panelData["panel"] as string, "ROMANC", 0.1872, 0.75, 2, layerName, new Point3d(startPoint.X + 1.17828457810867, startPoint.Y - 0.299822699224023, 0));
      CreateAndPositionText(tr, panelData["location"] as string, "ROMANS", 0.09375, 1, 2, layerName, new Point3d(startPoint.X + 3.19605976175148, startPoint.Y - 0.137807184107345, 0));
      CreateAndPositionText(tr, panelData["main"] as string, "ROMANS", 0.09375, 1, 2, layerName, new Point3d(startPoint.X + 3.24033367283675, startPoint.Y - 0.32590837886957, 0));
      CreateAndPositionText(tr, panelData["bus_rating"] as string, "ROMANS", 0.12375, 1, 2, layerName, new Point3d(startPoint.X + 6.2073642121926, startPoint.Y - 0.274622599308543, 0));
      CreateAndPositionText(tr, panelData["voltage1"] as string + "/" + panelData["voltage2"] as string, "ROMANS", 0.09375, 1, 2, layerName, new Point3d(startPoint.X + 7.04393671550224, startPoint.Y - 0.141653203021775, 0));
      CreateAndPositionText(tr, panelData["mounting"] as string, "ROMANS", 0.09375, 1, 2, layerName, new Point3d(startPoint.X + 7.87802551675406, startPoint.Y - 0.331292901876935, 0));
      CreateAndPositionText(tr, panelData["phase"] as string, "ROMANS", 0.09375, 1, 2, layerName, new Point3d(startPoint.X + 8.1253996026328, startPoint.Y - 0.141653203021775, 0));
      CreateAndPositionText(tr, panelData["wire"] as string, "ROMANS", 0.09375, 1, 2, layerName, new Point3d(startPoint.X + 8.50104048135836, startPoint.Y - 0.141653203021775, 0));
    }

    private void CreateVerticalLines(Transaction tr, BlockTableRecord btr, Point3d startPoint, double[] distances, double startY, double endY, string layer)
    {
      foreach (double distance in distances)
      {
        var lineStart = new Point3d(startPoint.X + distance, startY, 0);
        var lineEnd = new Point3d(startPoint.X + distance, endY, 0);
        var line = new Line(lineStart, lineEnd);
        line.Layer = layer;

        btr.AppendEntity(line);
        tr.AddNewlyCreatedDBObject(line, true);
      }
    }

    private void CreateLines(Transaction tr, BlockTableRecord btr, IEnumerable<(double startX, double startY, double endX, double endY, string layer)> lines)
    {
      foreach (var (startX, startY, endX, endY, layer) in lines)
      {
        CreateLine(tr, btr, startX, startY, endX, endY, layer);
      }
    }

    private void CreateLine(Transaction tr, BlockTableRecord btr, double startX, double startY, double endX, double endY, string layer)
    {
      var lineStart = new Point3d(startX, startY, 0);
      var lineEnd = new Point3d(endX, endY, 0);
      var line = new Line(lineStart, lineEnd);
      line.Layer = layer;

      btr.AppendEntity(line);
      tr.AddNewlyCreatedDBObject(line, true);
    }

    private void ProcessTextData(Transaction tr, BlockTableRecord btr, Point3d startPoint, Dictionary<string, object> panelData, bool is2Pole)
    {
      var (_, _, ed) = GetGlobals();

      List<string> leftDescriptions = (List<string>)panelData["description_left"];
      List<string> leftBreakers = (List<string>)panelData["breaker_left"];
      List<string> leftCircuits = (List<string>)panelData["circuit_left"];
      List<string> leftPhaseA = (List<string>)panelData["phase_a_left"];
      List<string> leftPhaseB = (List<string>)panelData["phase_b_left"];

      List<string> rightDescriptions = (List<string>)panelData["description_right"];
      List<string> rightBreakers = (List<string>)panelData["breaker_right"];
      List<string> rightCircuits = (List<string>)panelData["circuit_right"];
      List<string> rightPhaseA = (List<string>)panelData["phase_a_right"];
      List<string> rightPhaseB = (List<string>)panelData["phase_b_right"];

      List<bool> leftBreakersHighlight = (List<bool>)panelData["breaker_left_highlights"];
      List<bool> rightBreakersHighlight = (List<bool>)panelData["breaker_right_highlights"];

      if (!is2Pole)
      {
        List<string> leftPhaseC = (List<string>)panelData["phase_c_left"];
        List<string> rightPhaseC = (List<string>)panelData["phase_c_right"];
        // Use the ProcessSideData for the left side
        ProcessSideData(tr, btr, startPoint, leftDescriptions, leftBreakers, leftCircuits, leftPhaseA, leftPhaseB, (List<string>)leftPhaseC, (List<bool>)panelData["description_left_highlights"], (List<bool>)panelData["breaker_left_highlights"], true);

        // Use the ProcessSideData for the right side
        ProcessSideData(tr, btr, startPoint, rightDescriptions, rightBreakers, rightCircuits, rightPhaseA, rightPhaseB, (List<string>)rightPhaseC, (List<bool>)panelData["description_right_highlights"], (List<bool>)panelData["breaker_right_highlights"], false);
      }
      else
      {
        // Use the ProcessSideData for the left side
        ProcessSideData2P(tr, btr, startPoint, leftDescriptions, leftBreakers, leftCircuits, leftPhaseA, leftPhaseB, (List<bool>)panelData["description_left_highlights"], (List<bool>)panelData["breaker_left_highlights"], true);

        // Use the ProcessSideData for the right side
        ProcessSideData2P(tr, btr, startPoint, rightDescriptions, rightBreakers, rightCircuits, rightPhaseA, rightPhaseB, (List<bool>)panelData["description_right_highlights"], (List<bool>)panelData["breaker_right_highlights"], false);
      }

      InsertKeepBreakers(startPoint, leftBreakersHighlight, true);
      InsertKeepBreakers(startPoint, rightBreakersHighlight, false);
    }

    private void InsertKeepBreakers(Point3d startPoint, List<bool> breakersHighlight, bool left)
    {
      var (_, _, ed) = GetGlobals();
      double header_height = 0.7488;
      double panel_width = 8.9856;
      double row_height = 0.1872;
      double left_start_x = startPoint.X;
      double left_start_y = startPoint.Y - header_height;
      double right_start_x = startPoint.X + panel_width;
      double right_start_y = left_start_y;
      double start_x, start_y, displacement;
      Point3d topPoint = new Point3d(0, 0, 0);
      Point3d botPoint = new Point3d(0, 0, 0);
      bool currentlyKeeping = false;

      ed.WriteMessage(breakersHighlight.Count.ToString());
      for (int i = 0; i < breakersHighlight.Count; i += 2)
      {
        if (left)
        {
          start_x = left_start_x;
          start_y = left_start_y;
          displacement = -1;
        }
        else
        {
          start_x = right_start_x;
          start_y = right_start_y;
          displacement = 1;
        }

        ed.WriteMessage($"The value of breakersHighlight[i]: {breakersHighlight[i]} \nThe value of i: {i}\n");

        if (breakersHighlight[i] && !currentlyKeeping)
        {
          topPoint = new Point3d(start_x, start_y - (row_height * (i / 2)), 0);
          currentlyKeeping = true;
        }
        else if (!breakersHighlight[i] && currentlyKeeping)
        {
          botPoint = new Point3d(start_x, start_y - (row_height * (i / 2)), 0);
          currentlyKeeping = false;
          KeepBreakersGivenPoints(topPoint, botPoint, new Point3d(topPoint.X + displacement, topPoint.Y, 0));
          ed.WriteMessage($"\nWent inside to make the breakers, value of i: {i}\n");
          ed.WriteMessage($"\nTop point value: {topPoint}\nBot point value: {botPoint}\n");
        }
        else if (breakersHighlight[i] && currentlyKeeping && i >= breakersHighlight.Count - 2)
        {
          botPoint = new Point3d(start_x, start_y - (row_height * ((i + 2) / 2)), 0);
          KeepBreakersGivenPoints(topPoint, botPoint, new Point3d(topPoint.X + displacement, topPoint.Y, 0));
        }
      }
    }

    private void ProcessSideData(Transaction tr, BlockTableRecord btr, Point3d startPoint, List<string> descriptions, List<string> breakers, List<string> circuits, List<string> phaseA, List<string> phaseB, List<string> phaseC, List<bool> descriptionHighlights, List<bool> breakerHighlights, bool left)
    {
      var (_, _, ed) = GetGlobals();

      Dictionary<string, double> data = new Dictionary<string, double>();

      data.Add("row height y", 0.1872);
      data.Add("half row height y", 0.0936);
      data.Add("initial half breaker text y", -0.816333638994546);
      data.Add("header height", 0.7488);

      for (int i = 0; i < descriptions.Count; i += 2)
      {
        double phase = GetPhase(breakers, circuits, i);

        if (phase == 0.5)
        {
          CreateHalfBreaker(tr, btr, startPoint, descriptions, phaseA, phaseB, phaseC, breakers, circuits, descriptionHighlights, breakerHighlights, data, left, i);
        }
        else if (phase == 1.0)
        {
          Create1PoleBreaker(tr, btr, startPoint, descriptions, phaseA, phaseB, phaseC, breakers, circuits, descriptionHighlights, breakerHighlights, data, left, i);
        }
        else if (phase == 2.0)
        {
          Create2PoleBreaker(tr, btr, startPoint, descriptions, phaseA, phaseB, phaseC, breakers, circuits, descriptionHighlights, breakerHighlights, data, left, i);
          i += 2;
        }
        else
        {
          Create3PoleBreaker(tr, btr, startPoint, descriptions, phaseA, phaseB, phaseC, breakers, circuits, descriptionHighlights, breakerHighlights, data, left, i);
          i += 4;
        }
      }
    }

    private void ProcessSideData2P(Transaction tr, BlockTableRecord btr, Point3d startPoint, List<string> descriptions, List<string> breakers, List<string> circuits, List<string> phaseA, List<string> phaseB, List<bool> descriptionHighlights, List<bool> breakerHighlights, bool left)
    {
      var (_, _, ed) = GetGlobals();

      Dictionary<string, double> data = new Dictionary<string, double>
      {
        { "row height y", 0.1872 },
        { "half row height y", 0.0936 },
        { "initial half breaker text y", -0.816333638994546 },
        { "header height", 0.7488 }
      };

      for (int i = 0; i < descriptions.Count; i += 2)
      {
        double phase = GetPhase(breakers, circuits, i);

        if (phase == 0.5)
        {
          CreateHalfBreaker2P(tr, btr, startPoint, descriptions, phaseA, phaseB, breakers, circuits, descriptionHighlights, breakerHighlights, data, left, i);
        }
        else if (phase == 1.0)
        {
          Create1PoleBreaker2P(tr, btr, startPoint, descriptions, phaseA, phaseB, breakers, circuits, descriptionHighlights, breakerHighlights, data, left, i);
        }
        else
        {
          Create2PoleBreaker2P(tr, btr, startPoint, descriptions, phaseA, phaseB, breakers, circuits, descriptionHighlights, breakerHighlights, data, left, i);
          i += 2;
        }
      }
    }

    private void CreateHalfBreaker(Transaction tr, BlockTableRecord btr, Point3d startPoint, List<string> descriptions, List<string> phaseA, List<string> phaseB, List<string> phaseC, List<string> breakers, List<string> circuits, List<bool> descriptionHighlights, List<bool> breakerHighlights, Dictionary<string, double> data, bool left, int i)
    {
      List<string> phaseList = GetPhaseList(i, phaseA, phaseB, phaseC);

      double descriptionX = GetDescriptionX(startPoint, left);
      double phaseX = GetPhaseX(i, startPoint, left);
      double breakerX = GetBreakerX(startPoint, left);
      double circuitX = GetCircuitX(startPoint, left);

      string circuit = circuits[i];

      for (var j = i; j <= i + 1; j++)
      {
        string description = (descriptionHighlights[j] && descriptions[j] != "EXISTING LOAD") ? "(E)" + descriptions[j] : descriptions[j];
        string breaker = breakers[j] + "-1";
        string phase = phaseList[j];
        circuit = circuits[j];
        double height = startPoint.Y + (-0.816333638994546 - ((double)j / 2 * 0.1872));

        CreateAndPositionText(tr, description, "ROMANS", 0.046875, 1.0, 2, "0", new Point3d(descriptionX, height, 0));
        if (phase != "0") CreateAndPositionCenteredText(tr, phase, "ROMANS", 0.046875, 1.0, 2, "0", new Point3d(phaseX, height, 0));
        CreateAndPositionText(tr, breaker, "ROMANS", 0.046875, 1.0, 2, "0", new Point3d(breakerX, height, 0));
        CreateAndPositionText(tr, circuit, "ROMANS", 0.046875, 1.0, 7, "0", new Point3d(circuitX, height, 0));
      }

      CreateHorizontalLine(startPoint.X, startPoint.Y, circuit, left, tr, btr);
    }

    private void CreateHalfBreaker2P(Transaction tr, BlockTableRecord btr, Point3d startPoint, List<string> descriptions, List<string> phaseA, List<string> phaseB, List<string> breakers, List<string> circuits, List<bool> descriptionHighlights, List<bool> breakerHighlights, Dictionary<string, double> data, bool left, int i)
    {
      List<string> phaseList = GetPhaseList2P(i, phaseA, phaseB);

      double descriptionX = GetDescriptionX2P(startPoint, left);
      double phaseX = GetPhaseX2P(i, startPoint, left);
      double breakerX = GetBreakerX(startPoint, left);
      double circuitX = GetCircuitX(startPoint, left);

      string circuit = circuits[i];

      for (var j = i; j <= i + 1; j++)
      {
        string description = (descriptionHighlights[j] && descriptions[j] != "EXISTING LOAD") ? "(E)" + descriptions[j] : descriptions[j];
        string breaker = breakers[j] + "-1";
        string phase = phaseList[j];
        circuit = circuits[j];
        double height = startPoint.Y + (-0.816333638994546 - ((double)j / 2 * 0.1872));

        CreateAndPositionText(tr, description, "ROMANS", 0.046875, 1.0, 2, "0", new Point3d(descriptionX, height, 0));
        if (phase != "0") CreateAndPositionCenteredText(tr, phase, "ROMANS", 0.046875, 1.0, 2, "0", new Point3d(phaseX, height, 0));
        CreateAndPositionText(tr, breaker, "ROMANS", 0.046875, 1.0, 2, "0", new Point3d(breakerX, height, 0));
        CreateAndPositionText(tr, circuit, "ROMANS", 0.046875, 1.0, 7, "0", new Point3d(circuitX, height, 0));
      }

      CreateHorizontalLine(startPoint.X, startPoint.Y, circuit, left, tr, btr);
    }

    private void Create1PoleBreaker(Transaction tr, BlockTableRecord btr, Point3d startPoint, List<string> descriptions, List<string> phaseA, List<string> phaseB, List<string> phaseC, List<string> breakers, List<string> circuits, List<bool> descriptionHighlights, List<bool> breakerHighlights, Dictionary<string, double> data, bool left, int i)
    {
      List<string> phaseList = GetPhaseList(i, phaseA, phaseB, phaseC);

      string description = (descriptionHighlights[i] && descriptions[i] != "EXISTING LOAD") ? "(E)" + descriptions[i] : descriptions[i];
      string breaker = breakers[i] + "-1";
      string phase = phaseList[i];
      string circuit = circuits[i];
      double height = startPoint.Y + (-0.890211813771344 - ((i / 2) * 0.1872));
      double descriptionX = GetDescriptionX(startPoint, left);
      double phaseX = GetPhaseX(i, startPoint, left);
      double breakerX = GetBreakerX(startPoint, left);
      double circuitX = GetCircuitX(startPoint, left);
      double length = 0.2300;

      CreateAndPositionText(tr, description, "ROMANS", 0.09375, 1.0, 2, "0", new Point3d(descriptionX, height, 0));
      if (phase != "0") CreateAndPositionCenteredText(tr, phase, "ROMANS", 0.09375, 1.0, 2, "0", new Point3d(phaseX, height, 0));
      if (breaker != "-1") CreateAndPositionFittedText(tr, breaker, "ROMANS", 0.09375, 1.0, 2, "0", new Point3d(breakerX, height, 0), length);
      CreateAndPositionText(tr, circuit, "ROMANS", 0.09375, 1.0, 7, "0", new Point3d(circuitX, height, 0));
      CreateHorizontalLine(startPoint.X, startPoint.Y, circuit, left, tr, btr);
    }

    private void Create1PoleBreaker2P(Transaction tr, BlockTableRecord btr, Point3d startPoint, List<string> descriptions, List<string> phaseA, List<string> phaseB, List<string> breakers, List<string> circuits, List<bool> descriptionHighlights, List<bool> breakerHighlights, Dictionary<string, double> data, bool left, int i)
    {
      List<string> phaseList = GetPhaseList2P(i, phaseA, phaseB);

      string description = (descriptionHighlights[i] && descriptions[i] != "EXISTING LOAD") ? "(E)" + descriptions[i] : descriptions[i];
      string breaker = breakers[i] + "-1";
      string phase = phaseList[i];
      string circuit = circuits[i];
      double height = startPoint.Y + (-0.890211813771344 - ((i / 2) * 0.1872));
      double descriptionX = GetDescriptionX2P(startPoint, left);
      double phaseX = GetPhaseX2P(i, startPoint, left);
      double breakerX = GetBreakerX(startPoint, left);
      double circuitX = GetCircuitX(startPoint, left);
      double length = 0.2300;

      CreateAndPositionText(tr, description, "ROMANS", 0.09375, 1.0, 2, "0", new Point3d(descriptionX, height, 0));
      if (phase != "0") CreateAndPositionCenteredText(tr, phase, "ROMANS", 0.09375, 1.0, 2, "0", new Point3d(phaseX, height, 0));
      if (breaker != "-1") CreateAndPositionFittedText(tr, breaker, "ROMANS", 0.09375, 1.0, 2, "0", new Point3d(breakerX, height, 0), length);
      CreateAndPositionText(tr, circuit, "ROMANS", 0.09375, 1.0, 7, "0", new Point3d(circuitX, height, 0));
      CreateHorizontalLine(startPoint.X, startPoint.Y, circuit, left, tr, btr);
    }

    private void Create2PoleBreaker(Transaction tr, BlockTableRecord btr, Point3d startPoint, List<string> descriptions, List<string> phaseA, List<string> phaseB, List<string> phaseC, List<string> breakers, List<string> circuits, List<bool> descriptionHighlights, List<bool> breakerHighlights, Dictionary<string, double> data, bool left, int i)
    {
      double descriptionX = GetDescriptionX(startPoint, left);
      double breakerX = GetBreakerX(startPoint, left);
      double circuitX = GetCircuitX(startPoint, left);
      double length = 0.14;

      for (var j = i; j <= i + 2; j += 2)
      {
        List<string> phaseList = GetPhaseList(j, phaseA, phaseB, phaseC);
        string description = (descriptionHighlights[j] && descriptions[j] != "EXISTING LOAD") ? "(E)" + descriptions[j] : descriptions[j];
        string breaker = breakers[j];
        string phase = phaseList[j];
        string circuit = circuits[j];
        double height = startPoint.Y + (-0.890211813771344 - ((j / 2) * 0.1872));
        double phaseX = GetPhaseX(j, startPoint, left);

        if (j == i + 2)
        {
          description = "---";
          breakerX += 0.16;
          length = 0.07;
        }

        CreateAndPositionText(tr, description, "ROMANS", 0.09375, 1.0, 2, "0", new Point3d(descriptionX, height, 0));
        if (phase != "0") CreateAndPositionCenteredText(tr, phase, "ROMANS", 0.09375, 1.0, 2, "0", new Point3d(phaseX, height, 0));
        if (breaker != "") CreateAndPositionFittedText(tr, breaker, "ROMANS", 0.09375, 1.0, 2, "0", new Point3d(breakerX, height, 0), length);
        CreateAndPositionText(tr, circuit, "ROMANS", 0.09375, 1.0, 7, "0", new Point3d(circuitX, height, 0));
        CreateHorizontalLine(startPoint.X, startPoint.Y, circuit, left, tr, btr);
      }

      CreateBreakerLine(startPoint, i, left, tr, btr, 4);
    }

    private void Create2PoleBreaker2P(Transaction tr, BlockTableRecord btr, Point3d startPoint, List<string> descriptions, List<string> phaseA, List<string> phaseB, List<string> breakers, List<string> circuits, List<bool> descriptionHighlights, List<bool> breakerHighlights, Dictionary<string, double> data, bool left, int i)
    {
      var (_, _, ed) = GetGlobals();
      double descriptionX = GetDescriptionX2P(startPoint, left);
      double breakerX = GetBreakerX(startPoint, left);
      double circuitX = GetCircuitX(startPoint, left);
      double length = 0.14;

      for (var j = i; j <= i + 2; j += 2)
      {
        List<string> phaseList = GetPhaseList2P(j, phaseA, phaseB);
        string description = (descriptionHighlights[j] && descriptions[j] != "EXISTING LOAD") ? "(E)" + descriptions[j] : descriptions[j];
        string breaker = breakers[j];
        string phase = phaseList[j];
        string circuit = circuits[j];
        double height = startPoint.Y + (-0.890211813771344 - ((j / 2) * 0.1872));
        double phaseX = GetPhaseX2P(j, startPoint, left);

        if (j == i + 2)
        {
          description = "---";
          breakerX += 0.16;
          length = 0.07;
        }

        CreateAndPositionText(tr, description, "ROMANS", 0.09375, 1.0, 2, "0", new Point3d(descriptionX, height, 0));
        if (phase != "0") CreateAndPositionCenteredText(tr, phase, "ROMANS", 0.09375, 1.0, 2, "0", new Point3d(phaseX, height, 0));
        ed.WriteMessage($"The value of breaker is: {breaker}\nThe value of breakerX is: {breakerX}\nThe value of length is: {length}");
        if (breaker != "") CreateAndPositionFittedText(tr, breaker, "ROMANS", 0.09375, 1.0, 2, "0", new Point3d(breakerX, height, 0), length);
        CreateAndPositionText(tr, circuit, "ROMANS", 0.09375, 1.0, 7, "0", new Point3d(circuitX, height, 0));
        CreateHorizontalLine(startPoint.X, startPoint.Y, circuit, left, tr, btr);
      }

      CreateBreakerLine(startPoint, i, left, tr, btr, 4);
    }

    private void Create3PoleBreaker(Transaction tr, BlockTableRecord btr, Point3d startPoint, List<string> descriptions, List<string> phaseA, List<string> phaseB, List<string> phaseC, List<string> breakers, List<string> circuits, List<bool> descriptionHighlights, List<bool> breakerHighlights, Dictionary<string, double> data, bool left, int i)
    {
      double descriptionX = GetDescriptionX(startPoint, left);
      double breakerX = GetBreakerX(startPoint, left);
      double circuitX = GetCircuitX(startPoint, left);
      double length = 0.14;

      for (var j = i; j <= i + 4; j += 2)
      {
        List<string> phaseList = GetPhaseList(j, phaseA, phaseB, phaseC);
        string description = (descriptionHighlights[j] && descriptions[j] != "EXISTING LOAD") ? "(E)" + descriptions[j] : descriptions[j];
        string breaker = breakers[j];
        string phase = phaseList[j];
        string circuit = circuits[j];
        double height = startPoint.Y + (-0.890211813771344 - ((j / 2) * 0.1872));
        double phaseX = GetPhaseX(j, startPoint, left);

        if (j == i + 2)
        {
          description = "---";
        }
        else if (j == i + 4)
        {
          description = "---";
          breakerX += 0.16;
          length = 0.07;
        }

        CreateAndPositionText(tr, description, "ROMANS", 0.09375, 1.0, 2, "0", new Point3d(descriptionX, height, 0));
        if (phase != "0") CreateAndPositionCenteredText(tr, phase, "ROMANS", 0.09375, 1.0, 2, "0", new Point3d(phaseX, height, 0));
        if (j != i + 2) CreateAndPositionFittedText(tr, breaker, "ROMANS", 0.09375, 1.0, 2, "0", new Point3d(breakerX, height, 0), length);
        CreateAndPositionText(tr, circuit, "ROMANS", 0.09375, 1.0, 7, "0", new Point3d(circuitX, height, 0));
        CreateHorizontalLine(startPoint.X, startPoint.Y, circuit, left, tr, btr);
      }

      CreateBreakerLine(startPoint, i, left, tr, btr, 6);
    }

    private void CreateBreakerLine(Point3d startPoint, int i, bool left, Transaction tr, BlockTableRecord btr, int span)
    {
      double x1, x2;
      double height = startPoint.Y + (-0.7488 - (((i + span) / 2) * 0.1872));
      double y1 = height;
      double y2 = height + (span / 2) * 0.1872;

      if (left)
      {
        x1 = startPoint.X + 3.588;
        x2 = startPoint.X + 3.9;
      }
      else
      {
        x1 = startPoint.X + 5.0856;
        x2 = startPoint.X + 5.3976;
      }

      var lineStart = new Point3d(x1, y1, 0);
      var lineEnd = new Point3d(x2, y2, 0);
      var line = new Line(lineStart, lineEnd);
      line.Layer = "0";
      line.ColorIndex = 2;

      btr.AppendEntity(line);
      tr.AddNewlyCreatedDBObject(line, true);
    }

    private double GetCircuitX(Point3d startPoint, bool left)
    {
      if (left)
      {
        return startPoint.X + 3.93681721750636;
      }
      else
      {
        return startPoint.X + 4.87281721750651;
      }
    }

    private double GetBreakerX(Point3d startPoint, bool left)
    {
      if (left)
      {
        return startPoint.X + 3.60379818231218;
      }
      else
      {
        return startPoint.X + 5.10947444486385;
      }
    }

    private double GetDescriptionX(Point3d startPoint, bool left)
    {
      if (left)
      {
        return startPoint.X + 0.063560431161136;
      }
      else
      {
        return startPoint.X + 7.43528640590171;
      }
    }

    private double GetDescriptionX2P(Point3d startPoint, bool left)
    {
      if (left)
      {
        return startPoint.X + 0.0536663060360638;
      }
      else
      {
        return startPoint.X + 7.40509162108179;
      }
    }

    public double GetPhaseX(int i, Point3d startPoint, bool left)
    {
      if (left)
      {
        if (i % 6 == 0)
        {
          return startPoint.X + 1.64526228334811;
        }
        else if (i % 6 == 2)
        {
          return startPoint.X + 2.0792421731542;
        }
        else
        {
          return startPoint.X + 2.50445478897294;
        }
      }
      else
      {
        if (i % 6 == 0)
        {
          return startPoint.X + 6.11211889838299;
        }
        else if (i % 6 == 2)
        {
          return startPoint.X + 6.53328984899773;
        }
        else
        {
          return startPoint.X + 6.96804695722213;
        }
      }
    }

    public double GetPhaseX2P(int i, Point3d startPoint, bool left)
    {
      if (left)
      {
        if (i % 4 == 0)
        {
          return startPoint.X + 1.8390082793234;
        }
        else
        {
          return startPoint.X + 2.39546408826883;
        }
      }
      else
      {
        if (i % 4 == 0)
        {
          return startPoint.X + 6.21960728338948;
        }
        else
        {
          return startPoint.X + 6.83021158846114;
        }
      }
    }

    public List<string> GetPhaseList(int i, List<string> phaseA, List<string> phaseB, List<string> phaseC)
    {
      if (i % 6 == 0)
      {
        return phaseA;
      }
      else if (i % 6 == 2)
      {
        return phaseB;
      }
      else
      {
        return phaseC;
      }
    }

    public List<string> GetPhaseList2P(int i, List<string> phaseA, List<string> phaseB)
    {
      if (i % 4 == 0)
      {
        return phaseA;
      }
      else
      {
        return phaseB;
      }
    }

    private double GetPhase(List<string> breakers, List<string> circuits, int i)
    {
      // If index i is out of range for the circuits list, return 0.0
      if (i >= circuits.Count) return 0.0;

      // Check if circuit at index i contains 'A' or 'B'
      if (circuits[i].Contains('A') || circuits[i].Contains('B'))
      {
        return 0.5;
      }
      // Check if breakers has a value at [i+2] and if it is '3'
      else if ((i + 4) < breakers.Count && breakers[i + 4] == "3")
      {
        return 3.0;
      }
      // Check if breakers has a value at [i+1] and if it is '2'
      else if ((i + 2) < breakers.Count && breakers[i + 2] == "2")
      {
        return 2.0;
      }
      else
      {
        return 1.0;
      }
    }

    private void ProcessLineData(Transaction tr, BlockTableRecord btr, Point3d startPoint, Point3d endPoint, double endOfDataY, bool is2Pole)
    {
      string layerName = "0";
      double left1, left2, left3, right1, right2, right3;

      if (is2Pole)
      {
        left1 = 1.7549;
        left2 = 2.3023;
        left3 = 0;
        right1 = 6.7259;
        right2 = 0;
        right3 = 7.3632;
        CreateLine(tr, btr, startPoint.X + 1.85445441972615, startPoint.Y - 0.728330362273937, startPoint.X + 1.96793252133921, startPoint.Y - 0.595665451113291, "0");
        CreateLine(tr, btr, startPoint.X + 2.48147136651869, startPoint.Y - 0.728330362273937, startPoint.X + 2.5949494681322, startPoint.Y - 0.595665451113291, "0");
        CreateLine(tr, btr, startPoint.X + 4.16751445704995, startPoint.Y - 0.728330362273937, startPoint.X + 4.280992558663, startPoint.Y - 0.595665451113291, "0");
        CreateLine(tr, btr, startPoint.X + 4.57272120528569, startPoint.Y - 0.728330362273937, startPoint.X + 4.68619930689874, startPoint.Y - 0.595665451113291, "0");
        CreateLine(tr, btr, startPoint.X + 6.22789412120846, startPoint.Y - 0.728330362273937, startPoint.X + 6.34137222282197, startPoint.Y - 0.595665451113291, "0");
        CreateLine(tr, btr, startPoint.X + 6.8940934228981, startPoint.Y - 0.728330362273937, startPoint.X + 7.0075715245116, startPoint.Y - 0.595665451113291, "0");
      }
      else
      {
        left1 = 1.6224;
        left2 = 2.0488;
        left3 = 2.4752;
        right1 = 6.5104;
        right2 = 6.9368;
        right3 = 7.3632;
        CreateLine(tr, btr, startPoint.X + 1.8219640114711, startPoint.Y + -0.587355127494504, startPoint.X + 1.75209866364992, startPoint.Y + -0.732578250164607, "0");
        CreateLine(tr, btr, startPoint.X + 2.2392415498706, startPoint.Y + -0.587355127494504, startPoint.X + 2.16937620204942, startPoint.Y + -0.732578250164607, "0");
        CreateLine(tr, btr, startPoint.X + 2.64064439053459, startPoint.Y + -0.587355127494504, startPoint.X + 2.57077904271353, startPoint.Y + -0.732578250164607, "0");
        CreateLine(tr, btr, startPoint.X + 4.28558110707343, startPoint.Y + -0.581743684459752, startPoint.X + 4.21812491713047, startPoint.Y + -0.728299423953047, "0");
        CreateLine(tr, btr, startPoint.X + 4.4919520727949, startPoint.Y + -0.581743684459752, startPoint.X + 4.42449588285183, startPoint.Y + -0.728299423953047, "0");
        CreateLine(tr, btr, startPoint.X + 4.69832301754843, startPoint.Y + -0.581743684459752, startPoint.X + 4.63086682760547, startPoint.Y + -0.728299423953047, "0");
        CreateLine(tr, btr, startPoint.X + 6.28764850398056, startPoint.Y + -0.586040159740406, startPoint.X + 6.21478297900239, startPoint.Y + -0.730701330926394, "0");
        CreateLine(tr, btr, startPoint.X + 6.69812260049363, startPoint.Y + -0.586040159740406, startPoint.X + 6.62525707551547, startPoint.Y + -0.730701330926394, "0");
        CreateLine(tr, btr, startPoint.X + 7.10859666269549, startPoint.Y + -0.586040159740406, startPoint.X + 7.03573113771732, startPoint.Y + -0.730701330926394, "0");
      }

      var linesData = new (double[] distances, double startY, double endY, string layer)[]
          {
                    (new double[] { 2.2222, 5.0666, 6.9368 }, startPoint.Y, startPoint.Y - 0.3744, layerName),
                    (new double[] { left1, 2.9016, 3.1304, 3.3592, 3.5880, 3.9000, 4.1496, 4.8360, 5.0856, 5.3976, 5.6264, 5.8552, 6.0840, right3 }, startPoint.Y - 0.3744, endOfDataY, layerName),
                    (new double[] { left2, left3, right1, right2 }, startPoint.Y - 0.3744 - (0.3744 / 2), endOfDataY, layerName)
          };

      foreach (var lineData in linesData)
      {
        CreateVerticalLines(tr, btr, startPoint, lineData.distances, lineData.startY, lineData.endY, lineData.layer);
      }

      var linesData2 = new (double startX, double startY, double endX, double endY, string layer)[]
      {
                    (startPoint.X, startPoint.Y - 0.3744, endPoint.X, startPoint.Y - 0.3744, layerName),
                    (startPoint.X, startPoint.Y - 0.7488, endPoint.X, startPoint.Y - 0.7488, layerName),
                    (startPoint.X + 2.2222, startPoint.Y - (0.3744/2), startPoint.X + 5.0666, startPoint.Y - (0.3744/2), layerName),
                    (startPoint.X + 6.9368, startPoint.Y - (0.3744/2), endPoint.X, startPoint.Y - (0.3744/2), layerName),
                    (startPoint.X + left1, startPoint.Y - (0.3744/2) - 0.3744, startPoint.X + 2.9016, startPoint.Y - (0.3744/2) - 0.3744, layerName),
                    (startPoint.X + 6.0840, startPoint.Y - (0.3744/2) - 0.3744, startPoint.X + 7.3632, startPoint.Y - (0.3744/2) - 0.3744, layerName),
                    (startPoint.X + 4.1496, startPoint.Y - (0.3744/2) - 0.3744, startPoint.X + 4.8360, startPoint.Y - (0.3744/2) - 0.3744, layerName),
                    (startPoint.X + 8.28490642235897, startPoint.Y + -0.153581773169606, startPoint.X + 8.37682368466574, startPoint.Y + -0.0461231951291552, "0")
      };

      CreateLines(tr, btr, linesData2);
    }

    private static ObjectId GetTextStyleId(string styleName)
    {
      var (doc, db, _) = MyCommands.GetGlobals();
      var textStyleTable = (TextStyleTable)db.TextStyleTableId.GetObject(OpenMode.ForRead);

      if (textStyleTable.Has(styleName))
      {
        return textStyleTable[styleName];
      }
      else
      {
        // Return the ObjectId of the "Standard" style
        return textStyleTable["Standard"];
      }
    }

    private double CreateHorizontalLine(double startPointX, double startPointY, string circuitNumber, bool left, Transaction tr, BlockTableRecord btr)
    {
      int circuitNumReducer;
      double lineStartX;
      double lineStartX2;
      int circuitNumInt;
      double deltaY = 0.187200000000021; // Change this value if needed

      if (left)
      {
        circuitNumReducer = 1;
        lineStartX = startPointX;
        lineStartX2 = startPointX + 4.14960000000019;
      }
      else
      {
        circuitNumReducer = 2;
        lineStartX = startPointX + 4.8360;
        lineStartX2 = startPointX + 8.9856;
      }

      if (circuitNumber.Contains('A') || circuitNumber.Contains('B'))
      {
        // Remove 'A' or 'B' from the string
        circuitNumber = circuitNumber.Replace("A", "").Replace("B", "");
        circuitNumInt = int.Parse(circuitNumber);
        CreateCircuitLine(circuitNumInt, circuitNumReducer, startPointY, deltaY, lineStartX, lineStartX2, tr, btr, true);
      }
      else
      {
        circuitNumInt = int.Parse(circuitNumber);
      }

      return CreateCircuitLine(circuitNumInt, circuitNumReducer, startPointY, deltaY, lineStartX, lineStartX2, tr, btr);
    }

    private double CreateCircuitLine(int circuitNumInt, int circuitNumReducer, double startPointY, double deltaY, double lineStartX, double lineStartX2, Transaction tr, BlockTableRecord btr, bool half = false)
    {
      circuitNumInt = (circuitNumInt - circuitNumReducer) / 2;
      double lineStartY = startPointY - (0.935999999999979 + (deltaY * circuitNumInt));
      if (half) lineStartY += deltaY / 2;
      double lineEndY = lineStartY;

      var lineStart = new Point3d(lineStartX, lineStartY, 0);
      var lineEnd = new Point3d(lineStartX2, lineEndY, 0);
      var line = new Line(lineStart, lineEnd)
      {
        Layer = "0"
      };

      btr.AppendEntity(line);
      tr.AddNewlyCreatedDBObject(line, true);

      return line.StartPoint.Y;
    }

    public static void CreatePolyline(
    Color color,
    string layer,
    Point2d[] vertices,
    double startWidth,
    double endWidth)
    {
      Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
      Database acCurDb = acDoc.Database;
      Editor ed = acDoc.Editor;

      using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
      {
        BlockTable acBlkTbl;
        acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

        // Get the current space (either ModelSpace or the current layout)
        BlockTableRecord acBlkTblRec;
        acBlkTblRec = acTrans.GetObject(acCurDb.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

        if (acBlkTblRec == null)
        {
          ed.WriteMessage("\nFailed to retrieve the current space block record.");
          return;
        }

        Polyline acPoly = new Polyline();
        for (int i = 0; i < vertices.Length; i++)
        {
          acPoly.AddVertexAt(i, vertices[i], 0, startWidth, endWidth);
        }

        acPoly.Color = color;
        acPoly.Layer = layer;

        acBlkTblRec.AppendEntity(acPoly);
        acTrans.AddNewlyCreatedDBObject(acPoly, true);

        acTrans.Commit();

        // Outputting details for debugging
        ed.WriteMessage($"\nPolyline created in layer: {layer} with color: {color.ColorName}. StartPoint: {vertices[0].ToString()} EndPoint: {vertices[vertices.Length - 1].ToString()}");
      }
    }
  }
}