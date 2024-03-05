using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricalCommands
{
  public class XrefCommands
  {
    [CommandMethod("SETUPXREFS")]
    public void SetupXrefs()
    {
      Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
      Database currentDb = Application.DocumentManager.MdiActiveDocument.Database;

      // Prompt user to select DWG files
      System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog
      {
        Multiselect = true,
        Filter = "DWG files (*.dwg)|*.dwg",
        Title = "Select DWG Files"
      };

      if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        var databases = GetDatabasesFromDWGFiles(ofd);

        HashSet<string> allXrefFileNames = ModifySelectedDWGFiles(ed, ofd);

        // Convert allXrefFileNames to an array
        string[] allXrefFileNamesArray = allXrefFileNames.ToArray();

        // Call the AddDwgAsXref method with the selected files, the editor, and the database
        AddDwgAsXref(ofd.FileNames, ed, currentDb);

        // Call the GrayXref method with the selected files
        GrayXref(allXrefFileNamesArray);

        // Call the MagentaElectricalLayers method with the selected files
        MagentaElectricalLayers(allXrefFileNamesArray);

        ed.WriteMessage("Processing complete.");
      }
    }

    [CommandMethod("ATTACHXREFS")]
    public void AttachAllXrefs()
    {
      Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
      Database currentDb = Application.DocumentManager.MdiActiveDocument.Database;

      using (Transaction tr = currentDb.TransactionManager.StartTransaction())
      {
        // Get the XrefGraph of the current database
        XrefGraph xrefGraph = currentDb.GetHostDwgXrefGraph(true);

        // Traverse the XrefGraph
        for (int i = 0; i < xrefGraph.NumNodes; i++)
        {
          XrefGraphNode xrefGraphNode = xrefGraph.GetXrefNode(i);

          if (xrefGraphNode.XrefStatus == XrefStatus.Resolved && !xrefGraphNode.IsNested)
          {
            // Check if the BlockTableRecordId is not Null
            if (!xrefGraphNode.BlockTableRecordId.IsNull)
            {
              // Get the BlockTableRecord for the xref
              BlockTableRecord btr = (BlockTableRecord)tr.GetObject(xrefGraphNode.BlockTableRecordId, OpenMode.ForWrite);

              // Detach the xref
              currentDb.DetachXref(btr.ObjectId);

              // Get the full path of the xref file
              string xrefPath = xrefGraphNode.Database.Filename;

              // Reattach the xref as an attachment
              ObjectId xrefId = currentDb.AttachXref(xrefPath, btr.Name);

              if (!xrefId.IsNull)
              {
                // Get the BlockTable
                BlockTable bt = (BlockTable)tr.GetObject(currentDb.BlockTableId, OpenMode.ForRead);

                // Get the BlockTableRecord for the Model Space
                BlockTableRecord modelSpace = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                // Create a new BlockReference for the xref
                BlockReference xrefReference = new BlockReference(Point3d.Origin, xrefId);

                // Add the BlockReference to the Model Space
                modelSpace.AppendEntity(xrefReference);
                tr.AddNewlyCreatedDBObject(xrefReference, true);
              }
            }
            else
            {
              ed.WriteMessage($"The BlockTableRecordId for the xref {xrefGraphNode.Name} is Null.");
            }
          }
        }

        tr.Commit();
      }

      ed.WriteMessage("All overlay xrefs have been changed to attachments.");
    }

    [CommandMethod("ATTACHXREFSINFILE")]
    public void AttachAllXrefsInFileCommand()
    {
      // Prompt the user to select a DWG file
      System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog
      {
        Multiselect = false,
        Filter = "DWG files (*.dwg)|*.dwg",
        Title = "Select a DWG File"
      };

      if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        // Call the AttachAllXrefsInFile method with the selected file
        AttachAllXrefsInFile(ofd.FileName);
      }
    }

    public void AttachAllXrefsInFile(string filePath)
    {
      // Create a new database and read the DWG file
      using (Database db = new Database(false, true))
      {
        db.ReadDwgFile(filePath, FileShare.ReadWrite, true, "");

        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
          // Get the XrefGraph of the current database
          XrefGraph xrefGraph = db.GetHostDwgXrefGraph(true);

          // Traverse the XrefGraph
          for (int i = 0; i < xrefGraph.NumNodes; i++)
          {
            XrefGraphNode xrefGraphNode = xrefGraph.GetXrefNode(i);

            if (xrefGraphNode.XrefStatus == XrefStatus.Resolved && !xrefGraphNode.IsNested)
            {
              // Check if the BlockTableRecordId is not Null
              if (!xrefGraphNode.BlockTableRecordId.IsNull)
              {
                // Get the BlockTableRecord for the xref
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(xrefGraphNode.BlockTableRecordId, OpenMode.ForWrite);

                // Detach the xref
                db.DetachXref(btr.ObjectId);

                // Get the full path of the xref file
                string xrefPath = xrefGraphNode.Database.Filename;

                // Reattach the xref as an attachment
                ObjectId xrefId = db.AttachXref(xrefPath, btr.Name);

                if (!xrefId.IsNull)
                {
                  // Bind the xref, which changes its type to Attach
                  db.BindXrefs(new ObjectIdCollection() { xrefId }, true);

                  // Get the BlockTable
                  BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);

                  // Get the BlockTableRecord for the Model Space
                  BlockTableRecord modelSpace = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                  // Create a new BlockReference for the xref
                  BlockReference xrefReference = new BlockReference(Point3d.Origin, xrefId);

                  // Add the BlockReference to the Model Space
                  modelSpace.AppendEntity(xrefReference);
                  tr.AddNewlyCreatedDBObject(xrefReference, true);
                }
              }
            }
          }

          tr.Commit();
        }

        // Save the changes to the DWG file
        db.SaveAs(filePath, DwgVersion.Current);
      }
    }

    public List<Database> GetDatabasesFromDWGFiles(System.Windows.Forms.OpenFileDialog ofd)
    {
      List<Database> databases = new List<Database>();

      foreach (string file in ofd.FileNames)
      {
        Database db = new Database(false, true);
        db.ReadDwgFile(file, FileShare.ReadWrite, true, "");
        databases.Add(db);
      }

      return databases;
    }

    private HashSet<string> ModifySelectedDWGFiles(Editor ed, System.Windows.Forms.OpenFileDialog ofd)
    {
      HashSet<string> allXrefFileNames = new HashSet<string>();

      foreach (string file in ofd.FileNames)
      {
        allXrefFileNames.Add(file);

        Database db = new Database(false, true);
        try
        {
          db.ReadDwgFile(file, FileShare.ReadWrite, true, "");

          // Get the xref graph of the database
          XrefGraph xrefGraph = db.GetHostDwgXrefGraph(true);

          string[] xrefFileNames = GetXrefsOfXrefFile(db);

          foreach (string xrefFile in xrefFileNames)
          {
            allXrefFileNames.Add(xrefFile);
          }

          using (Transaction tr = db.TransactionManager.StartTransaction())
          {
            // Create a new layer named "0-GMEP" and set its color to 8
            LayerTable layerTable = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);
            if (!layerTable.Has("0-GMEP"))
            {
              layerTable.UpgradeOpen();
              LayerTableRecord layerRecord = new LayerTableRecord
              {
                Name = "0-GMEP",
                Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, 8)
              };
              layerTable.Add(layerRecord);
              tr.AddNewlyCreatedDBObject(layerRecord, true);
            }

            // Create a new layer named "0-GMEP-DIMS" and set its color to 8 (gray)
            if (!layerTable.Has("0-GMEP-DIMS"))
            {
              layerTable.UpgradeOpen();
              LayerTableRecord layerRecordDims = new LayerTableRecord
              {
                Name = "0-GMEP-DIMS",
                Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, 8)
              };
              layerTable.Add(layerRecordDims);
              tr.AddNewlyCreatedDBObject(layerRecordDims, true);
            }

            // Get the ObjectId of the "0" layer and the "0-GMEP" layer
            ObjectId zeroLayerId = layerTable["0"];
            ObjectId gmepLayerId = layerTable["0-GMEP"];
            ObjectId gmepDimsLayerId = layerTable["0-GMEP-DIMS"];

            BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

            foreach (ObjectId objId in btr)
            {
              Entity ent = tr.GetObject(objId, OpenMode.ForWrite) as Entity;
              if (ent != null && ent.LayerId == zeroLayerId)
              {
                // Move the entity from the "0" layer to the "0-GMEP" layer
                ent.LayerId = gmepLayerId;
              }
              else if (ent != null && (ent is Dimension || ent is RotatedDimension || ent is AlignedDimension || ent is Autodesk.AutoCAD.DatabaseServices.ArcDimension || ent is RadialDimension || ent is DiametricDimension || ent is DBText || ent is MText || ent is Leader || ent is MLeader))
              {
                // Move the dimension, text, mtext, and leader entities to the "0-GMEP-DIMS" layer
                ent.LayerId = gmepDimsLayerId;
              }
              SetEntityColorToByLayer(ent, tr, 4);
            }

            tr.Commit();
          }

          db.SaveAs(file, DwgVersion.Current);
        }
        catch (Autodesk.AutoCAD.Runtime.Exception ex)
        {
          ed.WriteMessage($"Error processing file {file}: {ex.Message}");
        }
        finally
        {
          db.Dispose();
        }
      }

      return allXrefFileNames;
    }

    private string[] GetXrefsOfXrefFile(Database db)
    {
      List<string> xrefFileNames = new List<string>();

      // Get the xref graph of the database
      XrefGraph xrefGraph = db.GetHostDwgXrefGraph(true);

      // Traverse the xref graph
      for (int i = 0; i < xrefGraph.NumNodes; i++)
      {
        XrefGraphNode xrefGraphNode = xrefGraph.GetXrefNode(i);

        // Check if the node is an xref (not the main drawing)
        if (xrefGraphNode.XrefStatus == XrefStatus.Resolved)
        {
          // Get the file path of the xref
          string xrefFileName = xrefGraphNode.Name;

          // Add the file path to xrefFileNames
          xrefFileNames.Add(xrefFileName);
        }
      }

      return xrefFileNames.ToArray();
    }

    public void GrayXref(string[] xrefFileNames)
    {
      Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
      Database currentDb = Application.DocumentManager.MdiActiveDocument.Database;

      using (Transaction tr = currentDb.TransactionManager.StartTransaction())
      {
        // Get the LayerTable from the database
        LayerTable layerTable = (LayerTable)tr.GetObject(currentDb.LayerTableId, OpenMode.ForRead);

        // Iterate over all layers in the LayerTable
        foreach (ObjectId layerId in layerTable)
        {
          LayerTableRecord layerRecord = (LayerTableRecord)tr.GetObject(layerId, OpenMode.ForWrite);

          // Extract just the file name without the path and extension from each string in the xrefFileNames array
          string[] xrefFileNamesWithoutPathAndExtension = xrefFileNames.Select(Path.GetFileNameWithoutExtension).ToArray();

          // Check if the layer is from one of the xref files
          if (xrefFileNamesWithoutPathAndExtension.Any(xrefFileName => layerRecord.Name.StartsWith(xrefFileName + "|")))
          {
            // Set the color of the layer to index 8
            layerRecord.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, 8);
          }
        }

        tr.Commit();
      }

      ed.WriteMessage("Processing complete.");
    }

    public void MagentaElectricalLayers(string[] xrefFileNames)
    {
      Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
      Database currentDb = Application.DocumentManager.MdiActiveDocument.Database;

      using (Transaction tr = currentDb.TransactionManager.StartTransaction())
      {
        // Get the LayerTable from the database
        LayerTable layerTable = (LayerTable)tr.GetObject(currentDb.LayerTableId, OpenMode.ForRead);

        // Extract just the file name without the path and extension from each string in the xrefFileNames array
        string[] xrefFileNamesWithoutPathAndExtension = xrefFileNames.Select(Path.GetFileNameWithoutExtension).ToArray();

        // Iterate over all layers in the LayerTable
        foreach (ObjectId layerId in layerTable)
        {
          LayerTableRecord layerRecord = (LayerTableRecord)tr.GetObject(layerId, OpenMode.ForWrite);

          if (xrefFileNamesWithoutPathAndExtension.Any(xrefFileName => layerRecord.Name.StartsWith(xrefFileName + "|")) &&
              (layerRecord.Name.Contains("CLG-LITE") || layerRecord.Name.Contains("CLNG-LITE") || layerRecord.Name.Contains("LIGHTING")))
          {
            // Set the color of the layer to index 6
            layerRecord.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, 6);
          }
        }

        tr.Commit();
      }

      ed.WriteMessage("Processing complete.");
    }

    public void AddDwgAsXref(string[] files, Editor ed, Database currentDb)
    {
      foreach (string file in files)
      {
        using (Transaction tr = currentDb.TransactionManager.StartTransaction())
        {
          BlockTable bt = (BlockTable)tr.GetObject(currentDb.BlockTableId, OpenMode.ForRead);
          BlockTableRecord modelSpace = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

          // Attach the DWG file as an Xref
          ObjectId xrefId = currentDb.AttachXref(file, Path.GetFileNameWithoutExtension(file));

          if (!xrefId.IsNull)
          {
            // Get the bounding box of the xref
            using (Database xrefDb = new Database(false, true))
            {
              xrefDb.ReadDwgFile(file, FileOpenMode.OpenForReadAndAllShare, true, null);
              using (Transaction xrefTr = xrefDb.TransactionManager.StartTransaction())
              {
                BlockTableRecord xrefBtr = (BlockTableRecord)xrefTr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(xrefDb), OpenMode.ForRead);
                Extents3d? extents = null;
                foreach (ObjectId id in xrefBtr)
                {
                  Entity ent = (Entity)xrefTr.GetObject(id, OpenMode.ForRead);
                  if (ent.Bounds.HasValue)
                  {
                    if (extents.HasValue)
                    {
                      extents.Value.AddExtents(ent.Bounds.Value);
                    }
                    else
                    {
                      extents = ent.Bounds.Value;
                    }
                  }
                }

                Point3d center = extents.HasValue ? new Point3d(
                    -(extents.Value.MinPoint.X + (extents.Value.MaxPoint.X - extents.Value.MinPoint.X) / 2),
                    -(extents.Value.MinPoint.Y + (extents.Value.MaxPoint.Y - extents.Value.MinPoint.Y) / 2),
                    -(extents.Value.MinPoint.Z + (extents.Value.MaxPoint.Z - extents.Value.MinPoint.Z) / 2)) : Point3d.Origin;

                // Add the Xref to the model space at the center of its bounding box
                BlockReference xrefReference = new BlockReference(center, xrefId);
                modelSpace.AppendEntity(xrefReference);
                tr.AddNewlyCreatedDBObject(xrefReference, true);
              }
            }

            LayerTable layerTableMain = (LayerTable)tr.GetObject(currentDb.LayerTableId, OpenMode.ForRead);
            if (layerTableMain.Has(Path.GetFileNameWithoutExtension(file) + "|0-GMEP-DIMS"))
            {
              LayerTableRecord layerRecordMain = (LayerTableRecord)tr.GetObject(layerTableMain[Path.GetFileNameWithoutExtension(file) + "|0-GMEP-DIMS"], OpenMode.ForWrite);
              layerRecordMain.IsFrozen = true;
            }
          }

          tr.Commit();
        }
      }
    }

    private void SetEntityColorToByLayer(Entity ent, Transaction tr, int depth)
    {
      if (ent != null)
      {
        ent.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 256);
      }

      if (depth <= 0)
      {
        return;
      }

      BlockReference blockRef = ent as BlockReference;
      if (blockRef != null)
      {
        blockRef.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 256);

        // Iterate over the entities in the block
        BlockTableRecord blockDef = (BlockTableRecord)tr.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead);
        foreach (ObjectId entId in blockDef)
        {
          Entity blockEnt = tr.GetObject(entId, OpenMode.ForWrite) as Entity;
          SetEntityColorToByLayer(blockEnt, tr, depth - 1);
        }
      }
    }
  }
}