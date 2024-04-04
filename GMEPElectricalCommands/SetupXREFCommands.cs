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

      // Prompt user to select DWG files
      System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog
      {
        Multiselect = true,
        Filter = "DWG files (*.dwg)|*.dwg",
        Title = "Select DWG Files"
      };

      if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        HashSet<string> allXrefFilePaths = new HashSet<string>();

        foreach (string filePath in ofd.FileNames)
        {
          allXrefFilePaths.UnionWith(LocateXrefsForFile(filePath));
        }

        allXrefFilePaths.UnionWith(ofd.FileNames);

        foreach (string xrefFilePath in allXrefFilePaths)
        {
          AttachAllXrefsInFile(xrefFilePath);
          //RemoveNotLocatedRasterImages(xrefFilePath);
        }

        ZeroLayerFixAndObjectColorToByLayer(ed, allXrefFilePaths);

        string[] allXrefFileNamesArray = allXrefFilePaths.ToArray();

        AddDwgAsXref(ofd.FileNames, ed, currentDb);

        GrayXref(allXrefFileNamesArray);

        MagentaElectricalLayers(allXrefFileNamesArray);

        ed.Regen();
        ed.UpdateScreen();
      }
    }

    private void RemoveNotLocatedRasterImages(string filePath)
    {
      using (Database db = new Database(false, true))
      {
        db.ReadDwgFile(filePath, FileShare.ReadWrite, true, "");

        using (Transaction trans = db.TransactionManager.StartTransaction())
        {
          // Open the block table for read
          BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

          // Open the block table record for read
          BlockTableRecord blockTableRecord = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

          List<ObjectId> rasterImagesToRemove = new List<ObjectId>();

          // Iterate over the entities in the block table record
          foreach (ObjectId entityId in blockTableRecord)
          {
            // Check if the entity is a RasterImage
            RasterImage rasterImage = trans.GetObject(entityId, OpenMode.ForRead) as RasterImage;

            if (rasterImage != null)
            {
              // Get the associated RasterImageDef
              RasterImageDef rasterImageDef = trans.GetObject(rasterImage.ImageDefId, OpenMode.ForRead) as RasterImageDef;

              if (rasterImageDef != null)
              {
                // Check if the image file exists
                if (!File.Exists(rasterImageDef.SourceFileName))
                {
                  rasterImagesToRemove.Add(entityId);
                }
              }
            }
          }

          // Remove the not located raster images
          foreach (ObjectId rasterImageId in rasterImagesToRemove)
          {
            RasterImage rasterImage = trans.GetObject(rasterImageId, OpenMode.ForWrite) as RasterImage;
            rasterImage.Erase(true);
          }

          trans.Commit();
        }

        db.SaveAs(filePath, DwgVersion.Current);
      }
    }

    private void SaveDataInJsonFileOnDesktop(object allXrefFileNames, string v)
    {
      string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
      string filePath = Path.Combine(desktopPath, v);

      string json = Newtonsoft.Json.JsonConvert.SerializeObject(allXrefFileNames, Newtonsoft.Json.Formatting.Indented);
      File.WriteAllText(filePath, json);
    }

    private HashSet<string> LocateXrefsForFile(string filePath)
    {
      HashSet<string> xrefFilePaths = new HashSet<string>();
      Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
      Database db = new Database(false, true);

      try
      {
        db.ReadDwgFile(filePath, FileShare.ReadWrite, true, "");

        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
          XrefGraph xrefGraph = db.GetHostDwgXrefGraph(true);
          ObjectIdCollection xrefIdsToReload = new ObjectIdCollection();
          List<ObjectId> xrefIdsToDetach = new List<ObjectId>();

          string currentDirectory = Path.GetDirectoryName(filePath);
          string xrefFolderPath = null;

          while (currentDirectory != null)
          {
            string potentialXrefFolderPath = Path.Combine(currentDirectory, "XREF");
            if (Directory.Exists(potentialXrefFolderPath))
            {
              xrefFolderPath = potentialXrefFolderPath;
              break;
            }
            currentDirectory = Directory.GetParent(currentDirectory)?.FullName;
          }

          if (xrefFolderPath != null)
          {
            for (int i = 0; i < xrefGraph.NumNodes; i++)
            {
              XrefGraphNode xrefGraphNode = xrefGraph.GetXrefNode(i);
              if (xrefGraphNode.XrefStatus == XrefStatus.Resolved)
              {
                if (!xrefGraphNode.BlockTableRecordId.IsNull)
                {
                  BlockTableRecord btr = tr.GetObject(xrefGraphNode.BlockTableRecordId, OpenMode.ForRead) as BlockTableRecord;
                  string originalPath = btr.PathName;
                  xrefFilePaths.Add(originalPath);
                }
              }
              else if (xrefGraphNode.XrefStatus == XrefStatus.Unresolved || xrefGraphNode.XrefStatus == XrefStatus.FileNotFound)
              {
                if (!xrefGraphNode.BlockTableRecordId.IsNull)
                {
                  BlockTableRecord btr = tr.GetObject(xrefGraphNode.BlockTableRecordId, OpenMode.ForRead) as BlockTableRecord;
                  string originalPath = btr.PathName;
                  string xrefFileName = Path.GetFileName(originalPath);

                  string[] matchingFiles = Directory.GetFiles(xrefFolderPath, xrefFileName, SearchOption.AllDirectories)
                      .Where(f => !Directory.GetParent(f).Name.Contains("backup"))
                      .OrderByDescending(f => Directory.GetCreationTime(Directory.GetParent(f).FullName))
                      .ToArray();

                  if (matchingFiles.Length > 0)
                  {
                    string newRelativePath = matchingFiles[0];
                    xrefFilePaths.Add(newRelativePath);

                    btr.UpgradeOpen();
                    btr.PathName = newRelativePath;
                    editor.WriteMessage($"Updated Path: {btr.PathName}\n");
                    xrefIdsToReload.Add(btr.ObjectId);

                    xrefFilePaths.UnionWith(LocateXrefsForFile(newRelativePath));
                  }
                  else
                  {
                    editor.WriteMessage($"File not found in the XREF folder or its subdirectories: {xrefFileName}\n");
                    xrefIdsToDetach.Add(btr.ObjectId);
                  }
                }
              }
            }
          }
          else
          {
            editor.WriteMessage("XREF folder not found in the current directory or its parent directories.\n");
          }

          if (xrefIdsToReload.Count > 0)
          {
            db.ReloadXrefs(xrefIdsToReload);
            editor.WriteMessage("External references reloaded.\n");
          }

          if (xrefIdsToDetach.Count > 0)
          {
            foreach (ObjectId xrefId in xrefIdsToDetach)
            {
              db.DetachXref(xrefId);
            }
            editor.WriteMessage("External references detached.\n");
          }

          tr.Commit();
        }

        db.SaveAs(filePath, DwgVersion.Current);
      }
      catch (Autodesk.AutoCAD.Runtime.Exception ex)
      {
        editor.WriteMessage($"Error processing file {filePath}: {ex.Message}\n");
      }
      finally
      {
        db.Dispose();
      }

      return xrefFilePaths;
    }

    private void LocatingAllXrefs(string filePath)
    {
      Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
      Database db = Application.DocumentManager.MdiActiveDocument.Database;

      using (Transaction tr = db.TransactionManager.StartTransaction())
      {
        XrefGraph xrefGraph = db.GetHostDwgXrefGraph(true);
        ObjectIdCollection xrefIdsToReload = new ObjectIdCollection();

        string currentDirectory = Path.GetDirectoryName(filePath);
        string xrefFolderPath = null;

        // Search for the "XREF" folder starting from the current directory
        while (currentDirectory != null)
        {
          string potentialXrefFolderPath = Path.Combine(currentDirectory, "XREF");
          if (Directory.Exists(potentialXrefFolderPath))
          {
            xrefFolderPath = potentialXrefFolderPath;
            break;
          }
          currentDirectory = Directory.GetParent(currentDirectory)?.FullName;
        }

        if (xrefFolderPath != null)
        {
          string[] tblkFileNames = { "tblk", "TBLK", "tblk24x36", "TBLK24x36", "tblk30x42", "TBLK30x42", "t-block", "T-BLOCK", "t-blk", "T-BLK", "titleblock", "TITLEBLOCK", "title block", "TITLE BLOCK", "TITLEBLK", "titleblk", "TBLOCK", "tblock", "tblok", "TBLOK" };

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

                string[] matchingFiles;

                if (xrefFileName.IndexOf("tblk", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    xrefFileName.IndexOf("TBLK", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                  matchingFiles = tblkFileNames
                      .SelectMany(tblkFileName => Directory.GetFiles(xrefFolderPath, "*" + tblkFileName + "*", SearchOption.AllDirectories))
                      .Where(f => !Directory.GetParent(f).Name.Contains("backup"))
                      .OrderByDescending(f => Directory.GetCreationTime(Directory.GetParent(f).FullName))
                      .ToArray();
                }
                else
                {
                  matchingFiles = Directory.GetFiles(xrefFolderPath, xrefFileName, SearchOption.AllDirectories)
                      .Where(f => !Directory.GetParent(f).Name.Contains("backup"))
                      .OrderByDescending(f => Directory.GetCreationTime(Directory.GetParent(f).FullName))
                      .ToArray();
                }

                if (matchingFiles.Length > 0)
                {
                  string newRelativePath = Path.Combine("..", "XREF", matchingFiles[0]);

                  btr.UpgradeOpen();
                  btr.PathName = newRelativePath;
                  editor.WriteMessage($"Updated Path: {btr.PathName}\n");
                  xrefIdsToReload.Add(btr.ObjectId);
                }
                else
                {
                  editor.WriteMessage($"No matching file found in the XREF folder or its subdirectories for: {xrefFileName}\n");
                }
              }
            }
          }

          if (xrefIdsToReload.Count > 0)
          {
            db.ReloadXrefs(xrefIdsToReload);
            editor.WriteMessage("External references reloaded.\n");
          }
        }
        else
        {
          editor.WriteMessage("XREF folder not found in the current directory or its parent directories.\n");
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

    private void ZeroLayerFixAndObjectColorToByLayer(Editor ed, HashSet<string> allXrefFilePaths)
    {
      foreach (string file in allXrefFilePaths)
      {
        Database db = new Database(false, true);
        try
        {
          db.ReadDwgFile(file, FileShare.ReadWrite, true, "");

          using (Transaction tr = db.TransactionManager.StartTransaction())
          {
            // Unlock all layers
            UnlockAllLayers(tr, db);

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

            ObjectId zeroLayerId = layerTable["0"];
            ObjectId gmepLayerId = layerTable["0-GMEP"];

            BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

            foreach (ObjectId objId in btr)
            {
              Entity ent = tr.GetObject(objId, OpenMode.ForWrite) as Entity;
              if (ent != null && ent.LayerId == zeroLayerId)
              {
                ent.LayerId = gmepLayerId;
              }
              SetEntityColorToByLayer(ent, tr, 4);
            }

            // Relock previously locked layers
            RelockPreviouslyLockedLayers(tr, db);

            tr.Commit();
          }

          // Save the changes made to the xref file
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
    }

    private void UnlockAllLayers(Transaction tr, Database db)
    {
      LayerTable layerTable = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);
      foreach (ObjectId layerId in layerTable)
      {
        LayerTableRecord layerRecord = (LayerTableRecord)tr.GetObject(layerId, OpenMode.ForWrite);
        layerRecord.IsLocked = false;
      }
    }

    private void RelockPreviouslyLockedLayers(Transaction tr, Database db)
    {
      LayerTable layerTable = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);
      foreach (ObjectId layerId in layerTable)
      {
        LayerTableRecord layerRecord = (LayerTableRecord)tr.GetObject(layerId, OpenMode.ForRead);
        if (layerRecord.Name.ToUpper().Contains("_LOCKED"))
        {
          layerRecord.UpgradeOpen();
          layerRecord.IsLocked = true;
          layerRecord.DowngradeOpen();
        }
      }
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
              (layerRecord.Name.ToUpper().Contains("LITE") || layerRecord.Name.ToUpper().Contains("RECEP") || layerRecord.Name.ToUpper().Contains("POWER")) || layerRecord.Name.ToUpper().Contains("OUTLET") || layerRecord.Name.ToUpper().Contains("LIGHT"))
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