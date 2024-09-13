using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricalCommands.Load_Summary {
  public class LoadSummaryCommands {
    private DialogWindow myForm;

    [CommandMethod("LOADSUMMARY")]

    public void LOADSUMMARY() {
      Document doc = Autodesk
        .AutoCAD
        .ApplicationServices
        .Application
        .DocumentManager
        .MdiActiveDocument;
      Editor ed = doc.Editor;

      try {
        if (this.myForm != null && !this.myForm.IsDisposed) {
          // Bring myForm to the front
          this.myForm.BringToFront();
        }
        else {
          // Create a new MainForm if it's not already open
          this.myForm = new DialogWindow(this);
          this.myForm.initialize_modal();
          this.myForm.Show();
        }
      }
      catch (System.Exception ex) {
      }
    }
  }
}
