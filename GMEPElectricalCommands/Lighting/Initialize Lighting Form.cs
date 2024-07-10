using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace ElectricalCommands
{
  public partial class INITIALIZE_LIGHTING_FORM : Form
  {
    public Point3d _panelPoint = Point3d.Origin;
    public List<ObjectId> _polylines = new List<ObjectId>();
    public int CurrentCircuitNumber { get; set; } = 2;
    public string CurrentInitialLetter { get; set; } = "a";

    public Dictionary<string, double> _scales = new Dictionary<string, double>()
    {
      { "1/4", 4.5 },
      { "3/16", 6.75 },
      { "1/8", 9 },
      { "3/32", 11.25 },
      { "1/16", 18 },
      { "1/32", 36 }
    };

    public double _textSize = 4.5;

    public INITIALIZE_LIGHTING_FORM()
    {
      InitializeComponent();
      SCALE_COMBOBOX.SelectedIndex = 0;
    }

    private void SET_WINDOW_BUTTON_Click(object sender, EventArgs e)
    {
      PositionApplicationWindow();
      this.BringToFront();
      this.Focus();
    }

    public static void PositionApplicationWindow()
    {
      // Get the main AutoCAD window
      var mainWindow = Application.MainWindow;

      mainWindow.WindowState = Window.State.Normal;

      // Set the position of the Application window
      System.Windows.Point ptApp = new System.Windows.Point(0, 0);
      mainWindow.DeviceIndependentLocation = ptApp;

      // Get the screen dimensions
      Rectangle screenBounds = Screen.GetBounds(Point.Empty);

      // Set the size of the Application window to match the screen dimensions
      System.Windows.Size szApp = new System.Windows.Size(screenBounds.Width, screenBounds.Height);
      mainWindow.DeviceIndependentSize = szApp;

      Document acDoc = Application.DocumentManager.MdiActiveDocument;
      acDoc.Window.WindowState = Window.State.Maximized;
    }

    private void SET_PANEL_LOCATION_BUTTON_Click(object sender, EventArgs e)
    {
      Document acDoc = FocusAutoCADAndSwitchToModelSpace();

      Editor ed = acDoc.Editor;
      _panelPoint = PromptUserForElectricalPanelPoint(ed);

      this.BringToFront();
      this.Focus();
    }

    private static Document FocusAutoCADAndSwitchToModelSpace()
    {
      Document acDoc = Autodesk
        .AutoCAD
        .ApplicationServices
        .Application
        .DocumentManager
        .MdiActiveDocument;

      using (acDoc.LockDocument())
      {
        Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Focus();

        acDoc.Database.TileMode = true;
      }

      return acDoc;
    }

    private static Point3d PromptUserForElectricalPanelPoint(Editor ed)
    {
      PromptPointOptions ppo = new PromptPointOptions("\nClick on the electrical panel: ");
      ppo.AllowNone = false; // User must select a point

      PromptPointResult ppr = ed.GetPoint(ppo);
      if (ppr.Status != PromptStatus.OK)
      {
        ed.WriteMessage("\nPrompt was cancelled.");
        return Point3d.Origin; // Return the origin point if the prompt was cancelled
      }

      return ppr.Value;
    }

    private void SELECT_POLYLINES_BUTTON_Click(object sender, EventArgs e)
    {
      Document doc = FocusAutoCADAndSwitchToModelSpace();
      Editor ed = doc.Editor;
      Database db = doc.Database;

      var polylineIds = PromptUserForPolylines(ed);
      if (polylineIds == null || polylineIds.Count == 0)
        return;

      using (doc.LockDocument())
      {
        for (int i = 0; i < polylineIds.Count; i++)
        {
          var polyId = polylineIds[i];
          ClosePolyline(db, polyId);

          var polylineLightingForm = new POLYLINE_LIGHTING_FORM(
            this,
            ed,
            db,
            polyId,
            _panelPoint,
            _textSize
          );
          polylineLightingForm.FormClosed += (s, args) =>
            HandlePolylineLightingFormClosed(s, args, i, polylineIds.Count, polylineLightingForm);
          polylineLightingForm.ShowDialog();

          if (polylineLightingForm.DialogResult == DialogResult.Cancel)
          {
            break;
          }
        }
      }

      this.BringToFront();
      this.Focus();
    }

    private void HandlePolylineLightingFormClosed(
      object sender,
      FormClosedEventArgs e,
      int currentIndex,
      int totalCount,
      POLYLINE_LIGHTING_FORM polyForm
    )
    {
      var form = sender as POLYLINE_LIGHTING_FORM;

      polyForm.RemoveOuterPolylineAndHatch(polyForm.db);

      if (form.DialogResult == DialogResult.OK)
      {
        int? numRooms = form.GetNumberOfRooms();
        for (int i = 0; i < numRooms; i++)
        {
          CurrentInitialLetter = form.GetNextCircuitLetter(CurrentInitialLetter);
        }

        if (numRooms == null || numRooms == 0)
        {
          this.DialogResult = DialogResult.Abort;
          this.Close();
          return;
        }

        if (currentIndex < totalCount - 1)
        {
          return;
        }
        else
        {
          this.Close();
        }
      }
      else if (form.DialogResult == DialogResult.Abort)
      {
        this.DialogResult = DialogResult.Abort;
        this.Close();
      }
    }

    private List<ObjectId> PromptUserForPolylines(Editor ed)
    {
      List<ObjectId> polylineIds = new List<ObjectId>();

      PromptSelectionOptions pso = new PromptSelectionOptions();
      pso.MessageForAdding = "";
      pso.AllowDuplicates = false;

      PromptSelectionResult psr = ed.GetSelection(pso);

      if (psr.Status == PromptStatus.OK)
      {
        SelectionSet ss = psr.Value;

        foreach (SelectedObject so in ss)
        {
          if (
            so != null
            && so.ObjectId != ObjectId.Null
            && !so.ObjectId.IsErased
            && so.ObjectId.ObjectClass.IsDerivedFrom(RXClass.GetClass(typeof(Polyline)))
          )
          {
            if (!_polylines.Contains(so.ObjectId))
            {
              polylineIds.Add(so.ObjectId);
            }
            else
            {
              ed.WriteMessage("\nThe selected polyline has already been added.");
            }
          }
        }
      }

      return polylineIds;
    }

    private void ClosePolyline(Database db, ObjectId polyId)
    {
      using (Transaction tr = db.TransactionManager.StartTransaction())
      {
        // Get the selected polyline
        Polyline poly = tr.GetObject(polyId, OpenMode.ForWrite) as Polyline;
        if (poly != null)
        {
          _polylines.Add(polyId);
          if (!poly.Closed)
          {
            poly.Closed = true;
            tr.Commit();
          }
        }
      }
    }

    private void SCALE_COMBOBOX_SelectedIndexChanged(object sender, EventArgs e)
    {
      string selectedScale = SCALE_COMBOBOX.SelectedItem?.ToString();
      if (
        !string.IsNullOrEmpty(selectedScale) && _scales.TryGetValue(selectedScale, out double size)
      )
      {
        _textSize = size;
      }
    }
  }
}
