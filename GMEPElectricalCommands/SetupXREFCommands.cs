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
  public class SetupXREFCommands
  {
    [CommandMethod("SETUPXREFS")]
    public void SetupXrefs()
    {
      Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
      Database currentDb = Application.DocumentManager.MdiActiveDocument.Database;

      LocatingAllXrefs(currentDb.Filename);

      //// Prompt user to select DWG files
      //System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog
      //{
      //  Multiselect = true,
      //  Filter = "DWG files (*.dwg)|*.dwg",
      //  Title = "Select DWG Files"
      //};

      //if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      //{
      //  // Call the AttachAllXrefsInFile method for each selected file
      //  foreach (string filePath in ofd.FileNames)
      //  {
      //    AttachAllXrefsInFile(filePath);
      //  }

      //  HashSet<string> allXrefFileNames = ModifySelectedDWGFiles(ed, ofd);

      //  // Convert allXrefFileNames to an array
      //  string[] allXrefFileNamesArray = allXrefFileNames.ToArray();

      //  // Call the AddDwgAsXref method with the selected files, the editor, and the database
      //  AddDwgAsXref(ofd.FileNames, ed, currentDb);

      //  // Call the GrayXref method with the selected files
      //  GrayXref(allXrefFileNamesArray);

      //  // Call the MagentaElectricalLayers method with the selected files
      //  MagentaElectricalLayers(allXrefFileNamesArray);
      //}
    }

    private void LocatingAllXrefs(string filePath)
    {
      Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
      Database db = Application.DocumentManager.MdiActiveDocument.Database;

      using (Transaction tr = db.TransactionManager.StartTransaction())
      {
        XrefGraph xrefGraph = db.GetHostDwgXrefGraph(true);
        ObjectIdCollection xrefIdsToReload = new ObjectIdCollection();

        for (int i = 0; i < xrefGraph.NumNodes; i++)
        {
          XrefGraphNode xrefGraphNode = xrefGraph.GetXrefNode(i);
          if (xrefGraphNode.XrefStatus == XrefStatus.Unresolved || xrefGraphNode.XrefStatus == XrefStatus.FileNotFound)
          {
            if (!xrefGraphNode.BlockTableRecordId.IsNull)
            {
              BlockTableRecord btr = tr.GetObject(xrefGraphNode.BlockTableRecordId, OpenMode.ForRead) as BlockTableRecord;
              string originalPath = btr.PathName;
              string xrefFileName = Path.GetFileName(originalPath);
              string newRelativePath = $"..\\XREF\\{xrefFileName}";

              if (File.Exists(Path.Combine(Path.GetDirectoryName(filePath), newRelativePath)))
              {
                btr.UpgradeOpen();
                btr.PathName = newRelativePath;
                editor.WriteMessage($"Updated Path: {btr.PathName}\n");
                xrefIdsToReload.Add(btr.ObjectId);
              }
              else
              {
                editor.WriteMessage($"File not found at the new relative path: {newRelativePath}\n");
              }
            }
          }
        }

        if (xrefIdsToReload.Count > 0)
        {
          db.ReloadXrefs(xrefIdsToReload);
          editor.WriteMessage("External references reloaded.\n");
        }

        tr.Commit();
      }
    }

    private void AttachAllXrefsInFile(string filePath)
    {
      Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;

      using (Database db = new Database(false, true))
      {
        db.ReadDwgFile(filePath, FileShare.ReadWrite, true, "");

        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
          XrefGraph xrefGraph = db.GetHostDwgXrefGraph(true);

          for (int i = 0; i < xrefGraph.NumNodes; i++)
          {
            XrefGraphNode xrefGraphNode = xrefGraph.GetXrefNode(i);

            if (xrefGraphNode.XrefStatus == XrefStatus.Unresolved && !xrefGraphNode.IsNested)
            {
              if (!xrefGraphNode.BlockTableRecordId.IsNull)
              {
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(xrefGraphNode.BlockTableRecordId, OpenMode.ForWrite);
                if (btr.IsFromOverlayReference)
                {
                  btr.IsFromOverlayReference = false;
                }
              }
            }
          }
          tr.Commit();
        }

        db.SaveAs(filePath, DwgVersion.Current);
      }
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

            // Create a new layer named "0-GMEP-DIMS-LEADS" and set its color to 8 (gray)
            if (!layerTable.Has("0-GMEP-DIMS-LEADS"))
            {
              layerTable.UpgradeOpen();
              LayerTableRecord layerRecordDims = new LayerTableRecord
              {
                Name = "0-GMEP-DIMS-LEADS",
                Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, 8)
              };
              layerTable.Add(layerRecordDims);
              tr.AddNewlyCreatedDBObject(layerRecordDims, true);
            }

            ObjectId zeroLayerId = layerTable["0"];
            ObjectId gmepLayerId = layerTable["0-GMEP"];
            ObjectId gmepDimsLayerId = layerTable["0-GMEP-DIMS-LEADS"];

            BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

            foreach (ObjectId objId in btr)
            {
              Entity ent = tr.GetObject(objId, OpenMode.ForWrite) as Entity;
              if (ent != null && ent.LayerId == zeroLayerId)
              {
                ent.LayerId = gmepLayerId;
              }
              else if (ent != null && (ent is Dimension || ent is RotatedDimension || ent is AlignedDimension || ent is Autodesk.AutoCAD.DatabaseServices.ArcDimension || ent is RadialDimension || ent is DiametricDimension || ent is Leader || ent is MLeader))
              {
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
          ed.WriteMessage($"Error processing file {file}: {ex.Message}\n");
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

                int index = Array.IndexOf(files, file);

                if (index >= 1)
                {
                  center = new Point3d(center.X + 2500 * index, center.Y, center.Z);
                }

                // Add the Xref to the model space at the center of its bounding box
                BlockReference xrefReference = new BlockReference(center, xrefId);
                modelSpace.AppendEntity(xrefReference);
                tr.AddNewlyCreatedDBObject(xrefReference, true);
              }
            }

            LayerTable layerTableMain = (LayerTable)tr.GetObject(currentDb.LayerTableId, OpenMode.ForRead);
            if (layerTableMain.Has(Path.GetFileNameWithoutExtension(file) + "|0-GMEP-DIMS-LEADS"))
            {
              LayerTableRecord layerRecordMain = (LayerTableRecord)tr.GetObject(layerTableMain[Path.GetFileNameWithoutExtension(file) + "|0-GMEP-DIMS-LEADS"], OpenMode.ForWrite);
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