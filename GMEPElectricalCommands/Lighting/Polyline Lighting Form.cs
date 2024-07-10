using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Accord.MachineLearning;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Windows;
using Dreambuild.AutoCAD;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Newtonsoft.Json;
using TriangleNet.Meshing.Algorithm;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace ElectricalCommands
{
  public partial class POLYLINE_LIGHTING_FORM : Form
  {
    private Editor ed;
    public Database db;
    private ObjectId polyId;
    public ObjectId _rectPolyID = ObjectId.Null;
    public ObjectId _hatchID = ObjectId.Null;
    public Point3d _panelPoint = Point3d.Origin;
    private List<ObjectId> _splineIds = new List<ObjectId>();
    private List<ObjectId> _textObjectIds = new List<ObjectId>();
    private List<MagentaObject> _magentaObjects;
    private string _capturedImagePath;
    private double _textSize;
    private INITIALIZE_LIGHTING_FORM _parentForm;

    public POLYLINE_LIGHTING_FORM(
      INITIALIZE_LIGHTING_FORM parentForm,
      Editor ed,
      Database db,
      ObjectId polyId,
      Point3d panelPoint,
      double textSize
    )
    {
      InitializeComponent();
      this.ed = ed;
      this.db = db;
      this.polyId = polyId;
      _panelPoint = panelPoint;
      _textSize = textSize;
      _parentForm = parentForm;

      ZoomIn();

      this.Load += (sender, e) => this.SetCircuitAndLetter();
    }

    private void ZoomIn()
    {
      using (Transaction tr = db.TransactionManager.StartTransaction())
      {
        Polyline poly = tr.GetObject(polyId, OpenMode.ForRead) as Polyline;
        Extents3d ext = poly.GeometricExtents;
        ed.Zoom(ext);
        tr.Commit();
      }
    }

    private void SetCircuitAndLetter()
    {
      CIRCUIT_NUMBER.Text = _parentForm.CurrentCircuitNumber.ToString();
      INITIAL_DIMMER_LETTER.Text = _parentForm.CurrentInitialLetter;

      MoveFormToOtherScreen();
    }

    public int? GetNumberOfRooms()
    {
      if (string.IsNullOrWhiteSpace(NUMBER_OF_ROOMS.Text))
      {
        return null;
      }

      if (int.TryParse(NUMBER_OF_ROOMS.Text, out int numRooms))
      {
        return numRooms;
      }
      else
      {
        throw new InvalidOperationException("Invalid number of rooms entered.");
      }
    }

    private void CreateOuterPolylineAndHatch(Editor ed, Database db, ObjectId polyId)
    {
      using (Transaction tr = db.TransactionManager.StartTransaction())
      {
        // Get the selected polyline
        Polyline poly = tr.GetObject(polyId, OpenMode.ForRead) as Polyline;
        if (poly != null && poly.Closed)
        {
          // Get the polyline's bounding box
          Extents3d ext = poly.GeometricExtents;
          ed.Zoom(ext);
          Point3d min = ext.MinPoint;
          Point3d max = ext.MaxPoint;

          _rectPolyID = CreateOuterPolyline(db, tr, ref min, ref max);

          ObjectId[] polys = new ObjectId[] { polyId, _rectPolyID };

          _hatchID = Hatch(polys);
        }
        tr.Commit();

        ed.UpdateScreen();
      }
    }

    private ObjectId CreateOuterPolyline(
      Database db,
      Transaction tr,
      ref Point3d min,
      ref Point3d max
    )
    {
      // Create a rectangular polyline around the selected polyline
      double offset = 1.0; // Adjust the offset value as needed
      Point2d pt1 = new Point2d(min.X - offset, min.Y - offset);
      Point2d pt2 = new Point2d(max.X + offset, min.Y - offset);
      Point2d pt3 = new Point2d(max.X + offset, max.Y + offset);
      Point2d pt4 = new Point2d(min.X - offset, max.Y + offset);

      Polyline rectPoly = new Polyline();
      rectPoly.AddVertexAt(0, pt1, 0, 0, 0);
      rectPoly.AddVertexAt(1, pt2, 0, 0, 0);
      rectPoly.AddVertexAt(2, pt3, 0, 0, 0);
      rectPoly.AddVertexAt(3, pt4, 0, 0, 0);
      rectPoly.Closed = true;

      // Add the rectangular polyline to the database
      BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
      ObjectId polylineId = btr.AppendEntity(rectPoly);
      tr.AddNewlyCreatedDBObject(rectPoly, true);

      return polylineId;
    }

    public ObjectId Hatch(
      ObjectId[] loopIds,
      string hatchName = "SOLID",
      double scale = 1,
      double angle = 0,
      bool associative = false
    )
    {
      var db = GetDatabase(loopIds);
      using (var trans = db.TransactionManager.StartTransaction())
      {
        var hatch = new Hatch();
        var space = trans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
        ObjectId hatchId = space.AppendEntity(hatch);
        trans.AddNewlyCreatedDBObject(hatch, true);

        hatch.SetDatabaseDefaults();
        hatch.Normal = new Vector3d(0, 0, 1);
        hatch.Elevation = 0.0;
        hatch.Associative = associative;
        hatch.PatternScale = scale;
        hatch.SetHatchPattern(HatchPatternType.PreDefined, hatchName);
        hatch.ColorIndex = 0;
        hatch.PatternAngle = angle;
        hatch.HatchStyle = Autodesk.AutoCAD.DatabaseServices.HatchStyle.Outer;
        for (int i = 0; i < loopIds.Length; i++)
        {
          ObjectId loop = loopIds[i];
          hatch.AppendLoop(HatchLoopTypes.External, new ObjectIdCollection(new[] { loop }));
        }
        hatch.EvaluateHatch(true);

        trans.Commit();
        return hatchId;
      }
    }

    internal Database GetDatabase(IEnumerable<ObjectId> objectIds)
    {
      return objectIds.Select(id => id.Database).Distinct().Single();
    }

    private void GENERATE_BUTTON_Click(object sender, EventArgs e)
    {
      string initialDimmerLetter = INITIAL_DIMMER_LETTER.Text ?? "";

      RemoveSplineAndTextObjects();
      RemoveOuterPolylineAndHatch(db);

      _splineIds.Clear();
      _textObjectIds.Clear();

      CreateOuterPolylineAndHatch(ed, db, polyId);

      string imagePath = "CapturedPolylineArea.png";
      int HEADER_HEIGHT = 32;
      MoveFormToOtherScreen();

      var (magentaObjects, capturedImagePath) = CaptureScreenshotAndGetMagentaObjects(
        ed.Document,
        HEADER_HEIGHT,
        imagePath
      );

      _magentaObjects = magentaObjects;
      _capturedImagePath = capturedImagePath;

      if (_magentaObjects != null)
      {
        var numRooms = GetNumberOfRooms();
        if (numRooms == null)
        {
          MessageBox.Show("Please enter the number of rooms.");
          return;
        }
        else
        {
          GenerateTextAndSplines(
            _magentaObjects,
            _capturedImagePath,
            initialDimmerLetter,
            numRooms
          );
          ed.UpdateScreen();
        }
      }
    }

    private void MoveFormToOtherScreen()
    {
      // Get the screen where AutoCAD is currently showing
      Screen acadScreen = Screen.FromHandle(
        Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle
      );

      // Find the opposite screen
      Screen oppositeScreen = Screen.AllScreens.FirstOrDefault(s => !s.Equals(acadScreen));

      if (oppositeScreen != null)
      {
        // Move the POLYLINE_LIGHTING_FORM to the opposite screen
        this.StartPosition = FormStartPosition.Manual;
        this.Location = oppositeScreen.WorkingArea.Location;
      }
    }

    private (
      List<MagentaObject> magentaObjects,
      string imagePath
    ) CaptureScreenshotAndGetMagentaObjects(Document doc, int HEADER_HEIGHT, string imagePath)
    {
      using (Transaction tr = db.TransactionManager.StartTransaction())
      {
        Polyline poly = tr.GetObject(polyId, OpenMode.ForRead) as Polyline;
        if (poly != null && poly.Closed)
        {
          Extents3d ext = poly.GeometricExtents;
          ed.Zoom(ext);
          Point3d min = ext.MinPoint;
          Point3d max = ext.MaxPoint;

          // Convert the polyline's extents to screen coordinates
          var screenMin = ed.PointToScreen(min, 0);
          var screenMax = ed.PointToScreen(max, 0);

          // Get the top-left corner of the AutoCAD document window
          int screenHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
          System.Windows.Point documentLocation = doc.Window.DeviceIndependentLocation;
          screenMin.Y += documentLocation.Y + HEADER_HEIGHT;
          screenMax.Y += documentLocation.Y + HEADER_HEIGHT;
          screenMin.X -= documentLocation.X;
          screenMax.X -= documentLocation.X;

          // Calculate the width and height of the captured area
          int width = (int)(screenMax.X - screenMin.X);
          int height = (int)(screenMin.Y - screenMax.Y);

          // Create a new bitmap to hold the captured image
          using (Bitmap bitmap = new Bitmap(width, height))
          {
            CaptureAScreenshot(
              ed,
              poly,
              screenMin,
              screenMax,
              documentLocation,
              width,
              height,
              bitmap
            );
            bitmap.Save(imagePath, ImageFormat.Png);
            var magentaObjects = LocateMagentaObjects(imagePath, ed, min, max, width, height);

            return (magentaObjects, imagePath);
          }
        }
      }

      return (null, null);
    }

    private (
      List<List<MagentaObject>> subClusters,
      int[] labels,
      double[][] centroids
    ) PerformClustering(int numRooms, List<MagentaObject> magentaObjects)
    {
      List<List<MagentaObject>> subClusters = new List<List<MagentaObject>>();
      int[] labels;
      double[][] centroids = null;
      double areaPercentage = 0.3;

      if (numRooms > 1)
      {
        double[][] data = magentaObjects
          .Select(obj => new double[] { obj.CenterNode.X, obj.CenterNode.Y })
          .ToArray();

        KMeans kmeans = new KMeans(numRooms);
        var clusters = kmeans.Learn(data);
        labels = clusters.Decide(data);
        centroids = clusters.Centroids;

        Dictionary<int, List<MagentaObject>> groupedClusters =
          new Dictionary<int, List<MagentaObject>>();
        for (int i = 0; i < labels.Length; i++)
        {
          int clusterLabel = labels[i];
          if (!groupedClusters.ContainsKey(clusterLabel))
          {
            groupedClusters[clusterLabel] = new List<MagentaObject>();
          }
          groupedClusters[clusterLabel].Add(magentaObjects[i]);
        }

        foreach (var clusterGroup in groupedClusters.Values)
        {
          subClusters.AddRange(GroupMagentaObjectsByArea(clusterGroup, areaPercentage));
        }
      }
      else
      {
        subClusters = GroupMagentaObjectsByArea(magentaObjects, areaPercentage);
        labels = new int[magentaObjects.Count];
        centroids = new double[subClusters.Count][];

        for (int i = 0; i < subClusters.Count; i++)
        {
          double sumX = 0;
          double sumY = 0;
          int count = 0;

          foreach (var obj in subClusters[i])
          {
            int index = magentaObjects.IndexOf(obj);
            if (index != -1)
            {
              labels[index] = i;
              sumX += obj.CenterNode.X;
              sumY += obj.CenterNode.Y;
              count++;
            }
          }

          if (count > 0)
          {
            centroids[i] = new double[] { sumX / count, sumY / count };
          }
        }
      }

      return (subClusters, labels, centroids);
    }

    private void GenerateTextAndSplines(
      List<MagentaObject> magentaObjects,
      string imagePath,
      string initialDimmerLetter,
      int? numRooms
    )
    {
      int numClusters = numRooms ?? 0;
      if (numClusters == 0)
      {
        return;
      }

      (List<List<MagentaObject>> subClusters, int[] labels, double[][] centroids) =
        PerformClustering(numClusters, magentaObjects);

      using (Transaction tr = db.TransactionManager.StartTransaction())
      {
        PutTextNearMagentaObjects(magentaObjects, ed, db, initialDimmerLetter, labels);

        var userPt = _panelPoint;

        if (centroids != null)
        {
          var orderedIndices = GetOrderedIndicesOfCentroidsFromUserClick(centroids, userPt);

          List<Edge> clusterEdges = new List<Edge>();
          HashSet<int> visitedClusters = new HashSet<int>();

          int startClusterIndex = orderedIndices[0];
          PrimVariation(
            startClusterIndex,
            magentaObjects,
            labels,
            visitedClusters,
            clusterEdges,
            userPt
          );

          foreach (var edge in clusterEdges)
          {
            CreateSplinesFromEdges(db, new List<Edge> { edge }, magentaObjects);
          }
        }

        var mpg = new MPolygon();
        var tolerance = Tolerance.Global.EqualPoint;
        var poly = db.TransactionManager.GetObject(polyId, OpenMode.ForRead) as Polyline;

        mpg.AppendLoopFromBoundary(poly, true, tolerance);

        for (int i = 0; i < subClusters.Count; i++)
        {
          List<MagentaObject> subClusterObjects = subClusters[i];
          int[] subClusterLabels = Enumerable.Repeat(i, subClusterObjects.Count).ToArray();

          List<TriangleNet.Geometry.Vertex> vertices = subClusterObjects
            .Select(obj => new TriangleNet.Geometry.Vertex(obj.CenterNode.X, obj.CenterNode.Y))
            .ToList();

          if (vertices.Count == 2)
          {
            var filteredEdges = new List<Edge>() { new Edge(vertices[0], vertices[1]) };
            CreateSplinesFromEdges(db, filteredEdges, subClusterObjects);
          }
          else if (vertices.Count > 2)
          {
            var triangulator = new Dwyer();

            var mesh = triangulator.Triangulate(vertices, new TriangleNet.Configuration());

            var edges = ConvertEdgesToPoints(mesh, subClusterObjects, mpg, tolerance);

            var edgeStats = CalculateEdgeLengthStatistics(edges);
            double meanLength = edgeStats.mean;
            double stdDevLength = edgeStats.stdDev;

            List<Edge> filteredEdges = edges
              .Where(edge =>
              {
                bool isLengthWithinRange = edge.Length() <= meanLength + stdDevLength;
                bool isNode1Connected = CountConnectedEdges(edge.Point1, edges) >= 3;
                bool isNode2Connected = CountConnectedEdges(edge.Point2, edges) >= 3;
                return isLengthWithinRange || !isNode1Connected || !isNode2Connected;
              })
              .ToList();

            MagentaObject closestObject = FindClosestMagentaObject(userPt, subClusterObjects);

            TriangleNet.Geometry.Vertex startNode = closestObject.CenterPointAsVertex();

            filteredEdges = BreadthFirstSearch(filteredEdges, startNode, userPt);

            CreateSplinesFromEdges(db, filteredEdges, subClusterObjects);
          }
        }

        tr.Commit();
      }
    }

    private void RemoveSplineAndTextObjects()
    {
      using (Transaction tr = db.TransactionManager.StartTransaction())
      {
        foreach (var splineId in _splineIds)
        {
          if (!splineId.IsNull && !splineId.IsErased)
          {
            DBObject spline = tr.GetObject(splineId, OpenMode.ForWrite);
            spline.Erase();
          }
        }

        foreach (var textObjectId in _textObjectIds)
        {
          if (!textObjectId.IsNull && !textObjectId.IsErased)
          {
            DBObject textObject = tr.GetObject(textObjectId, OpenMode.ForWrite);
            textObject.Erase();
          }
        }

        tr.Commit();
      }
    }

    public void RemoveOuterPolylineAndHatch(Database db)
    {
      using (Transaction tr = db.TransactionManager.StartTransaction())
      {
        if (!_rectPolyID.IsNull && !_rectPolyID.IsErased)
        {
          DBObject outerPoly = tr.GetObject(_rectPolyID, OpenMode.ForWrite);
          outerPoly.Erase();
        }

        if (!_hatchID.IsNull && !_hatchID.IsErased)
        {
          DBObject hatch = tr.GetObject(_hatchID, OpenMode.ForWrite);
          hatch.Erase();
        }

        tr.Commit();
      }
    }

    private static void CaptureAScreenshot(
      Editor ed,
      Polyline poly,
      System.Windows.Point screenMin,
      System.Windows.Point screenMax,
      System.Windows.Point documentLocation,
      int width,
      int height,
      Bitmap bitmap
    )
    {
      using (Graphics g = Graphics.FromImage(bitmap))
      {
        // Set the clipping boundary around the polyline
        GraphicsPath path = new GraphicsPath();
        // Get the polyline points
        int vertices = poly.NumberOfVertices;
        List<System.Drawing.PointF> polylinePoints = new List<System.Drawing.PointF>();
        for (int i = 0; i < vertices; i++)
        {
          Point3d point = poly.GetPoint3dAt(i);
          System.Windows.Point screenPoint = ed.PointToScreen(point, 0);
          screenPoint.X -= documentLocation.X;
          screenPoint.Y -= documentLocation.Y;
          polylinePoints.Add(
            new System.Drawing.PointF(
              (float)(screenPoint.X - screenMin.X),
              (float)(screenMin.Y - screenPoint.Y)
            )
          );
        }
        path.AddPolygon(polylinePoints.ToArray());
        g.SetClip(path);

        // Capture the contents of the AutoCAD window inside the polyline
        g.CopyFromScreen(
          (int)screenMin.X,
          (int)screenMax.Y,
          0,
          0,
          new System.Drawing.Size(width, height)
        );
      }
    }

    private List<MagentaObject> LocateMagentaObjects(
      string imagePath,
      Editor editor,
      Point3d min,
      Point3d max,
      int width,
      int height
    )
    {
      int hue = 150;
      double saturation = 1.0;
      double value = 0.4784;

      // Create the target HSV color
      Hsv targetHsv = new Hsv(hue, saturation * 255, value * 255);

      // Define the lower and upper bounds for the target color in HSV
      Hsv lowerBound = new Hsv(targetHsv.Hue - 5, 10, 10);
      Hsv upperBound = new Hsv(targetHsv.Hue + 5, 255, 255);

      // Convert the Hsv objects to ScalarArray
      ScalarArray lowerBoundScalar = new ScalarArray(
        new MCvScalar(lowerBound.Hue, lowerBound.Satuation, lowerBound.Value)
      );
      ScalarArray upperBoundScalar = new ScalarArray(
        new MCvScalar(upperBound.Hue, upperBound.Satuation, upperBound.Value)
      );

      // Load the captured image using Emgu CV
      Mat image = CvInvoke.Imread(imagePath);

      // Convert the image to the HSV color space
      Mat hsvImage = new Mat();
      CvInvoke.CvtColor(image, hsvImage, ColorConversion.Bgr2Hsv);

      // Create a binary mask based on the color range
      Mat mask = new Mat();
      CvInvoke.InRange(hsvImage, lowerBoundScalar, upperBoundScalar, mask);

      // Apply morphological operations
      Mat kernel = CvInvoke.GetStructuringElement(
        ElementShape.Rectangle,
        new Size(5, 5),
        new Point(-1, -1)
      );
      CvInvoke.Dilate(
        mask,
        mask,
        kernel,
        new Point(-1, -1),
        5,
        Emgu.CV.CvEnum.BorderType.Default,
        new MCvScalar(0, 0, 0)
      );
      CvInvoke.Erode(
        mask,
        mask,
        kernel,
        new Point(-1, -1),
        5,
        Emgu.CV.CvEnum.BorderType.Default,
        new MCvScalar(0, 0, 0)
      );

      // Save the binary mask image
      string binaryMaskPath = "BinaryMask.png";
      CvInvoke.Imwrite(binaryMaskPath, mask);

      // Find contours in the binary mask
      VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
      CvInvoke.FindContours(
        mask,
        contours,
        null,
        RetrType.External,
        ChainApproxMethod.ChainApproxSimple
      );

      List<MagentaObject> magentaObjects = new List<MagentaObject>();

      // Create a copy of the original image for drawing contours
      Mat contoursImage = image.Clone();

      // Iterate through the contours and create MagentaObject instances
      for (int i = 0; i < contours.Size; i++)
      {
        double epsilon = 0.02 * CvInvoke.ArcLength(contours[i], true);
        VectorOfPoint approx = new VectorOfPoint();
        CvInvoke.ApproxPolyDP(contours[i], approx, epsilon, true);

        List<Point3d> boundaryPointsAutoCAD = new List<Point3d>();

        for (int j = 0; j < approx.Size; j++)
        {
          Point pixelPoint = new Point(approx[j].X, approx[j].Y);
          Point3d autoCADPoint = ConvertPixelToAutoCAD(pixelPoint, editor, min, max, width, height);
          boundaryPointsAutoCAD.Add(autoCADPoint);
        }

        UpdateBoundaryPoints(boundaryPointsAutoCAD, min, max);

        if (boundaryPointsAutoCAD.Count > 2)
        {
          MagentaObject magentaObject = new MagentaObject(boundaryPointsAutoCAD);
          magentaObjects.Add(magentaObject);
        }

        // Draw the contour on the contours image
        CvInvoke.DrawContours(
          contoursImage,
          new VectorOfVectorOfPoint(approx),
          -1,
          new Bgr(0, 255, 0).MCvScalar,
          2
        );
      }

      // Save the image with the detected magenta objects being outlined
      string contoursImagePath = "ContoursImage.png";
      CvInvoke.Imwrite(contoursImagePath, contoursImage);

      //magentaObjects = RemoveMagentaObjectsInsideOtherMagentaObjects(magentaObjects);

      return magentaObjects;
    }

    public static void SaveDataToJsonFileOnDesktop(
      object data,
      string fileName,
      bool noOverride = false
    )
    {
      string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
      string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
      string fullPath = Path.Combine(desktopPath, fileName);

      if (noOverride && File.Exists(fullPath))
      {
        int fileNumber = 1;
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        string fileExtension = Path.GetExtension(fileName);

        while (File.Exists(fullPath))
        {
          string newFileName = $"{fileNameWithoutExtension} ({fileNumber}){fileExtension}";
          fullPath = Path.Combine(desktopPath, newFileName);
          fileNumber++;
        }
      }

      File.WriteAllText(fullPath, jsonData);
    }

    private void PutTextNearMagentaObjects(
      List<MagentaObject> magentaObjects,
      Editor ed,
      Database db,
      string initialDimmerLetter,
      int[] labels
    )
    {
      using (Transaction tr = db.TransactionManager.StartTransaction())
      {
        LayerTable lt = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
        LayerTableRecord ltr = tr.GetObject(lt["E-TXT1"], OpenMode.ForWrite) as LayerTableRecord;
        BlockTable blockTable = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
        BlockTableRecord modelSpace =
          tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite)
          as BlockTableRecord;

        Dictionary<int, string> clusterLabels = new Dictionary<int, string>();

        string currentLetter = initialDimmerLetter;

        foreach (int label in labels.Distinct())
        {
          clusterLabels[label] = currentLetter;
          currentLetter = GetNextCircuitLetter(currentLetter);
        }

        foreach (var obj in magentaObjects)
        {
          double maxX = obj.BoundaryPoints.Max(p => p.X);
          double maxY = obj.BoundaryPoints.Max(p => p.Y);
          Point3d position = new Point3d(maxX + _textSize / 8, maxY + _textSize / 8, 0);
          int clusterLabel = labels[magentaObjects.IndexOf(obj)];
          string letter = clusterLabels[clusterLabel];

          DBText dbText = new DBText();
          dbText.SetDatabaseDefaults();
          dbText.Normal = Vector3d.ZAxis;
          dbText.Position = position;
          dbText.Height = _textSize;
          dbText.TextString = $"{CIRCUIT_NUMBER.Text}{letter}";

          TextStyleTable textStyleTable =
            tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
          if (textStyleTable.Has("rpm"))
          {
            ObjectId textStyleId = textStyleTable["rpm"];
            dbText.TextStyleId = textStyleId;
          }
          else
          {
            ObjectId standardTextStyleId = textStyleTable["Standard"];
            dbText.TextStyleId = standardTextStyleId;
          }

          dbText.Layer = ltr.Name;
          modelSpace.AppendEntity(dbText);
          tr.AddNewlyCreatedDBObject(dbText, true);

          // Add the text object ID to the list
          _textObjectIds.Add(dbText.ObjectId);
        }

        tr.Commit();
      }
    }

    private void PrimVariation(
      int startClusterIndex,
      List<MagentaObject> magentaObjects,
      int[] labels,
      HashSet<int> visitedClusters,
      List<Edge> clusterEdges,
      Point3d userPt
    )
    {
      visitedClusters.Add(startClusterIndex);

      while (visitedClusters.Count < labels.Distinct().Count())
      {
        double minDistance = double.MaxValue;
        int closestClusterIndex = -1;
        Edge closestEdge = null;

        foreach (int visitedClusterIndex in visitedClusters)
        {
          for (int i = 0; i < labels.Length; i++)
          {
            if (!visitedClusters.Contains(labels[i]))
            {
              Edge edge = FindClosestEdgeBetweenClusters(
                visitedClusterIndex,
                labels[i],
                magentaObjects,
                labels,
                userPt
              );
              if (edge != null && edge.Length() < minDistance)
              {
                minDistance = edge.Length();
                closestClusterIndex = labels[i];
                closestEdge = edge;
              }
            }
          }
        }

        if (closestClusterIndex != -1)
        {
          visitedClusters.Add(closestClusterIndex);
          clusterEdges.Add(closestEdge);
        }
      }
    }

    private Edge FindClosestEdgeBetweenClusters(
      int cluster1Label,
      int cluster2Label,
      List<MagentaObject> magentaObjects,
      int[] labels,
      Point3d userPt
    )
    {
      List<MagentaObject> cluster1Objects = magentaObjects
        .Where((obj, idx) => labels[idx] == cluster1Label)
        .ToList();
      List<MagentaObject> cluster2Objects = magentaObjects
        .Where((obj, idx) => labels[idx] == cluster2Label)
        .ToList();

      double minDistance = double.MaxValue;
      MagentaObject closestObject1 = null;
      MagentaObject closestObject2 = null;

      foreach (var obj1 in cluster1Objects)
      {
        foreach (var obj2 in cluster2Objects)
        {
          double firstObjectDistanceToPanel = obj1.CenterNode.DistanceTo(userPt);
          double secondObjectDistanceToPanel = obj2.CenterNode.DistanceTo(userPt);
          double firstObjectDistanceToSecondObject = obj1.CenterNode.DistanceTo(obj2.CenterNode);
          double totalDistance =
            (firstObjectDistanceToSecondObject * 0.7)
            + (firstObjectDistanceToPanel * 0.15)
            + (secondObjectDistanceToPanel * 0.15);

          if (totalDistance < minDistance)
          {
            minDistance = totalDistance;
            closestObject1 = obj1;
            closestObject2 = obj2;
          }
        }
      }

      if (closestObject1 != null && closestObject2 != null)
      {
        TriangleNet.Geometry.Vertex vertex1 = closestObject1.CenterPointAsVertex();
        TriangleNet.Geometry.Vertex vertex2 = closestObject2.CenterPointAsVertex();
        return new Edge(vertex1, vertex2);
      }

      return null;
    }

    private List<int> GetOrderedIndicesOfCentroidsFromUserClick(
      double[][] centroids,
      Point3d userPt
    )
    {
      List<int> indices = new List<int>();

      foreach (var centroid in centroids)
      {
        centroid[0] = Math.Abs(centroid[0] - userPt.X);
        centroid[1] = Math.Abs(centroid[1] - userPt.Y);
      }

      var orderedCentroids = centroids
        .Select((c, i) => new { Index = i, Distance = Math.Sqrt(c[0] * c[0] + c[1] * c[1]) })
        .OrderBy(c => c.Distance)
        .ToList();

      foreach (var centroid in orderedCentroids)
      {
        indices.Add(centroid.Index);
      }

      return indices;
    }

    private void CreateSplinesFromEdges(
      Database db,
      List<Edge> edges,
      List<MagentaObject> magentaObjects
    )
    {
      using (Transaction trSplines = db.TransactionManager.StartTransaction())
      {
        BlockTable blockTable =
          trSplines.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
        BlockTableRecord modelSpace =
          trSplines.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite)
          as BlockTableRecord;

        foreach (var edge in edges)
        {
          TriangleNet.Geometry.Vertex vertex1 = edge.Point1;
          TriangleNet.Geometry.Vertex vertex2 = edge.Point2;

          MagentaObject magentaObject1 = magentaObjects.FirstOrDefault(obj =>
            obj.CenterNode.X == vertex1.X && obj.CenterNode.Y == vertex1.Y
          );
          MagentaObject magentaObject2 = magentaObjects.FirstOrDefault(obj =>
            obj.CenterNode.X == vertex2.X && obj.CenterNode.Y == vertex2.Y
          );

          if (magentaObject1 != null && magentaObject2 != null)
          {
            Point3d closestPoint1 = FindClosestPoint(
              magentaObject1.MidpointsBetweenBoundaryPoints,
              magentaObject2.CenterNode
            );
            Point3d closestPoint2 = FindClosestPoint(
              magentaObject2.MidpointsBetweenBoundaryPoints,
              magentaObject1.CenterNode
            );

            Spline spline = CreateSpline(closestPoint1, closestPoint2);

            modelSpace.AppendEntity(spline);
            trSplines.AddNewlyCreatedDBObject(spline, true);

            // Add the spline ID to the list
            _splineIds.Add(spline.ObjectId);
          }
        }

        trSplines.Commit();
      }
    }

    private static Spline CreateSpline(Point3d startPoint, Point3d endPoint)
    {
      Spline spline = new Spline(
        new Point3dCollection(new[] { startPoint, endPoint }),
        Vector3d.ZAxis,
        Vector3d.ZAxis,
        3,
        0.0
      );

      // Calculate the midpoint between the start and end points
      Point3d midPoint = new Point3d(
        (startPoint.X + endPoint.X) / 2,
        (startPoint.Y + endPoint.Y) / 2,
        0
      );

      // Calculate the direction vector from start point to end point
      Vector3d direction = endPoint - startPoint;

      // Calculate the perpendicular vector to the direction vector
      Vector3d perpendicular = new Vector3d(-direction.Y, direction.X, 0);

      // Normalize the perpendicular vector
      perpendicular = perpendicular.GetNormal();

      // Calculate the distance between the start and end points
      double distance = startPoint.DistanceTo(endPoint);

      // Calculate the offset distance for the control points
      double offsetDistance = distance * 0.2; // Adjust this value to control the curvature

      // Calculate the control points by adding the scaled perpendicular vector to the midpoint
      Point3d controlPoint1 = midPoint + perpendicular * offsetDistance;

      // Set the second control point to be the same as the end point
      Point3d controlPoint2 = endPoint;

      // Add the control points to the spline
      spline.SetControlPointAt(1, controlPoint1);
      spline.SetControlPointAt(2, controlPoint2);

      return spline;
    }

    private static Point3d FindClosestPoint(
      List<Point3d> midpointsBetweenBoundaryPoints,
      Point3d targetNode
    )
    {
      Point3d closestPoint = Point3d.Origin;
      double minDistance = double.MaxValue;

      foreach (var point in midpointsBetweenBoundaryPoints)
      {
        double distance = point.DistanceTo(new Point3d(targetNode.X, targetNode.Y, 0));
        if (distance < minDistance)
        {
          minDistance = distance;
          closestPoint = point;
        }
      }

      return closestPoint;
    }

    private int CountConnectedEdges(TriangleNet.Geometry.Vertex vertex, List<Edge> edges)
    {
      int count = 0;
      foreach (var edge in edges)
      {
        if (edge.Point1.Equals(vertex) || edge.Point2.Equals(vertex))
        {
          count++;
        }
      }
      return count;
    }

    private (double mean, double stdDev) CalculateEdgeLengthStatistics(List<Edge> edges)
    {
      double sum = 0;
      foreach (var edge in edges)
      {
        sum += edge.Length();
      }
      double mean = sum / edges.Count;

      double sumSquaredDiff = 0;
      foreach (var edge in edges)
      {
        double diff = edge.Length() - mean;
        sumSquaredDiff += diff * diff;
      }
      double variance = sumSquaredDiff / edges.Count;
      double stdDev = Math.Sqrt(variance);

      return (mean, stdDev);
    }

    private List<Edge> ConvertEdgesToPoints(
      TriangleNet.Meshing.IMesh mesh,
      List<MagentaObject> magentaObjects,
      MPolygon mpg,
      double tolerance
    )
    {
      List<Edge> edges = new List<Edge>();

      foreach (var edge in mesh.Edges)
      {
        var vert1 = mesh.Vertices.First(vertex => vertex.ID == edge.P0);
        var vert2 = mesh.Vertices.First(vertex => vertex.ID == edge.P1);

        // Check if the edge cuts through any magenta object
        if (!EdgeCutsThroughMagentaObject(vert1, vert2, magentaObjects, mpg, tolerance))
        {
          edges.Add(new Edge(vert1, vert2));
        }
      }

      return edges;
    }

    private bool EdgeCutsThroughMagentaObject(
      TriangleNet.Geometry.Vertex vert1,
      TriangleNet.Geometry.Vertex vert2,
      List<MagentaObject> magentaObjects,
      MPolygon mpg,
      double tolerance
    )
    {
      double stepSize = 1.0; // Adjust this value based on your desired granularity

      Vector3d edgeVector = new Vector3d(vert2.X - vert1.X, vert2.Y - vert1.Y, 0);
      double edgeLength = edgeVector.Length;
      int numSteps = (int)(edgeLength / stepSize);

      MagentaObject magentaObject1 = magentaObjects.FirstOrDefault(obj =>
        obj.CenterNode.X == vert1.X && obj.CenterNode.Y == vert1.Y
      );
      MagentaObject magentaObject2 = magentaObjects.FirstOrDefault(obj =>
        obj.CenterNode.X == vert2.X && obj.CenterNode.Y == vert2.Y
      );

      // Check if any of the 4 points at 20%, 40%, 60%, and 80% distance are outside the inner polyline
      for (int i = 1; i <= 4; i++)
      {
        double t = (double)i / 5;
        double x = vert1.X + t * (vert2.X - vert1.X);
        double y = vert1.Y + t * (vert2.Y - vert1.Y);
        Point3d point = new Point3d(x, y, 0);

        if (!IsPointOutsideInnerPolyline(point, mpg, tolerance))
        {
          return true;
        }
      }

      // Check if any point along the edge is inside a magenta object (excluding the start and end magenta objects)
      for (int i = 0; i <= numSteps; i++)
      {
        double t = (double)i / numSteps;
        double x = vert1.X + t * (vert2.X - vert1.X);
        double y = vert1.Y + t * (vert2.Y - vert1.Y);
        Point3d point = new Point3d(x, y, 0);

        foreach (var magentaObject in magentaObjects)
        {
          if (
            magentaObject != magentaObject1
            && magentaObject != magentaObject2
            && IsPointInsideMagentaObject(point, magentaObject)
          )
          {
            return true;
          }
        }
      }

      return false;
    }

    private bool IsPointOutsideInnerPolyline(Point3d point, MPolygon mpg, double tolerance)
    {
      using (Transaction tr = db.TransactionManager.StartTransaction())
      {
        Polyline poly = tr.GetObject(polyId, OpenMode.ForRead) as Polyline;
        if (poly != null && poly.Closed)
        {
          return IsPointInside(mpg, point, tolerance);
        }
        tr.Commit();
      }
      return false;
    }

    private bool IsPointInsideMagentaObject(Point3d point, MagentaObject magentaObject)
    {
      Point3d min = magentaObject.BoundaryPoints.Aggregate(
        (p1, p2) => new Point3d(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), 0)
      );
      Point3d max = magentaObject.BoundaryPoints.Aggregate(
        (p1, p2) => new Point3d(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y), 0)
      );

      return point.X >= min.X && point.X <= max.X && point.Y >= min.Y && point.Y <= max.Y;
    }

    private List<Edge> BreadthFirstSearch(
      List<Edge> edges,
      TriangleNet.Geometry.Vertex startNode,
      Point3d userPt
    )
    {
      var bfsEdges = new List<Edge>();
      var visited = new HashSet<TriangleNet.Geometry.Vertex>();
      var queue = new Queue<TriangleNet.Geometry.Vertex>();

      visited.Add(startNode);
      queue.Enqueue(startNode);

      while (queue.Count > 0)
      {
        var currentNode = queue.Dequeue();

        var neighborEdges = edges
          .Where(e => e.Point1.Equals(currentNode) || e.Point2.Equals(currentNode))
          .OrderBy(e =>
          {
            var neighborNode = e.Point1.Equals(currentNode) ? e.Point2 : e.Point1;
            return IsHorizontalOrVertical(currentNode, neighborNode) ? 0 : 1;
          })
          .ThenBy(e =>
          {
            var neighborNode = e.Point1.Equals(currentNode) ? e.Point2 : e.Point1;
            return new Point3d(neighborNode.X, neighborNode.Y, 0).DistanceTo(userPt);
          })
          .ToList();

        foreach (var edge in neighborEdges)
        {
          var neighborNode = edge.Point1.Equals(currentNode) ? edge.Point2 : edge.Point1;

          if (!visited.Contains(neighborNode))
          {
            visited.Add(neighborNode);
            queue.Enqueue(neighborNode);
            bfsEdges.Add(edge);
          }
        }
      }

      return bfsEdges;
    }

    private bool IsHorizontalOrVertical(
      TriangleNet.Geometry.Vertex point1,
      TriangleNet.Geometry.Vertex point2
    )
    {
      return Math.Abs(point1.X - point2.X) < 3 || Math.Abs(point1.Y - point2.Y) < 3;
    }

    private MagentaObject FindClosestMagentaObject(
      Point3d userPoint,
      List<MagentaObject> clusterObjects
    )
    {
      double minDistance = double.MaxValue;
      MagentaObject closestObject = null;

      foreach (var obj in clusterObjects)
      {
        double distance = userPoint.DistanceTo(obj.CenterNode);

        if (distance < minDistance)
        {
          minDistance = distance;
          closestObject = obj;
        }
      }

      return closestObject;
    }

    private static void UpdateBoundaryPoints(
      List<Point3d> boundaryPointsAutoCAD,
      Point3d min,
      Point3d max
    )
    {
      double baseScaleFactor = 1000; // Adjust this value based on your desired base scale factor

      // Calculate the extents of the model space
      double extentX = max.X - min.X;
      double extentY = max.Y - min.Y;

      // Calculate the scale factor based on the extents
      double scaleFactor = Math.Max(extentX, extentY) / baseScaleFactor;

      // Calculate the adjusted deltaX and deltaY values proportional to the scale factor
      double deltaX = 1 * scaleFactor;
      double deltaY = 0.2466 * scaleFactor / 1000;

      for (int i = 0; i < boundaryPointsAutoCAD.Count; i++)
      {
        Point3d point = boundaryPointsAutoCAD[i];
        point = new Point3d(point.X + deltaX, point.Y + deltaY, 0);
        boundaryPointsAutoCAD[i] = point;
      }
    }

    private static Point3d ConvertPixelToAutoCAD(
      Point pixelPoint,
      Editor editor,
      Point3d min,
      Point3d max,
      int width,
      int height
    )
    {
      double worldX = min.X + (pixelPoint.X * (max.X - min.X)) / width;
      double worldY = max.Y - (pixelPoint.Y * (max.Y - min.Y)) / height;

      return new Point3d(worldX, worldY, 0);
    }

    public static bool IsPointInside(MPolygon _mpg, Point3d point, double tolerance)
    {
      return _mpg.IsPointInsideMPolygon(point, tolerance).Count == 1;
    }

    private void FIX_TEXT_BUTTON_Click(object sender, EventArgs e)
    {
      using (Transaction tr = db.TransactionManager.StartTransaction())
      {
        foreach (var textObjectId in _textObjectIds)
        {
          if (!textObjectId.IsNull && !textObjectId.IsErased)
          {
            DBText textObject = tr.GetObject(textObjectId, OpenMode.ForWrite) as DBText;
            if (textObject != null)
            {
              List<ObjectId> nearestSplineIds = GetNearestSplineIds(textObject, 3);
              if (nearestSplineIds.Count > 0)
              {
                Point3d initialPosition = textObject.Position;
                bool intersects = true;
                double yOffset = 0.0;
                double yStep = -_textSize / 10.0; // Adjust the step value as needed
                double xOffset = 0.0;
                double xStep = _textSize / 20.0; // Adjust the step value as needed

                while (intersects)
                {
                  Point3d newPosition = new Point3d(
                    initialPosition.X + xOffset,
                    initialPosition.Y + yOffset,
                    0
                  );
                  textObject.Position = newPosition;

                  intersects = CheckIntersectionWithSplines(textObject, nearestSplineIds);

                  if (!intersects)
                  {
                    break;
                  }

                  yOffset += yStep;
                  xOffset += xStep;
                }
              }
            }
          }
        }
        tr.Commit();
      }
    }

    private List<ObjectId> GetNearestSplineIds(DBText textObject, int count)
    {
      List<ObjectId> nearestSplineIds = new List<ObjectId>();
      Point3d textCenter = textObject.Position;

      var splineDistances = _splineIds
        .Select(splineId =>
        {
          if (!splineId.IsNull && !splineId.IsErased)
          {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
              Spline spline = tr.GetObject(splineId, OpenMode.ForRead) as Spline;
              if (spline != null)
              {
                Point3d closestPoint = spline.GetClosestPointTo(textCenter, false);
                double distance = textCenter.DistanceTo(closestPoint);
                return new { SplineId = splineId, Distance = distance };
              }
            }
          }
          return null;
        })
        .Where(item => item != null)
        .OrderBy(item => item.Distance)
        .Take(count)
        .Select(item => item.SplineId)
        .ToList();

      nearestSplineIds.AddRange(splineDistances);

      return nearestSplineIds;
    }

    private bool CheckIntersectionWithSplines(DBText textObject, List<ObjectId> splineIds)
    {
      foreach (var splineId in splineIds)
      {
        if (!splineId.IsNull && !splineId.IsErased)
        {
          using (Transaction tr = db.TransactionManager.StartTransaction())
          {
            Spline spline = tr.GetObject(splineId, OpenMode.ForRead) as Spline;
            if (spline != null)
            {
              Point3dCollection intersectionPoints = new Point3dCollection();
              textObject.IntersectWith(
                spline,
                Autodesk.AutoCAD.DatabaseServices.Intersect.OnBothOperands,
                intersectionPoints,
                IntPtr.Zero,
                IntPtr.Zero
              );

              if (intersectionPoints.Count > 0)
              {
                return true;
              }
            }
          }
        }
      }
      return false;
    }

    public string GetNextCircuitLetter(string currentLetter)
    {
      if (string.IsNullOrEmpty(currentLetter))
      {
        return "";
      }
      else
      {
        if (currentLetter == "z")
        {
          return "aa";
        }
        else if (currentLetter.Length == 1)
        {
          char nextChar = (char)(currentLetter[0] + 1);
          if (nextChar == 'i' || nextChar == 'o' || nextChar == 'l')
          {
            nextChar++;
          }
          return nextChar.ToString();
        }
        else
        {
          int index = currentLetter.Length - 1;
          while (index >= 0 && currentLetter[index] == 'z')
          {
            index--;
          }
          if (index < 0)
          {
            return new string('a', currentLetter.Length + 1);
          }
          else
          {
            char[] chars = currentLetter.ToCharArray();
            chars[index] = (char)(chars[index] + 1);
            if (chars[index] == 'i' || chars[index] == 'o' || chars[index] == 'l')
            {
              chars[index]++;
            }
            for (int i = index + 1; i < chars.Length; i++)
            {
              chars[i] = 'a';
            }
            return new string(chars);
          }
        }
      }
    }

    private void PROCEED_BUTTON_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private double CalculateAreaThreshold()
    {
      double areaThreshold = 0.2;
      return areaThreshold;
    }

    private List<List<MagentaObject>> GroupMagentaObjectsByArea(
      List<MagentaObject> magentaObjects,
      double areaThreshold
    )
    {
      List<List<MagentaObject>> groups = new List<List<MagentaObject>>();

      foreach (var obj in magentaObjects)
      {
        double objectArea = CalculateArea(obj);

        bool addedToGroup = false;
        foreach (var group in groups)
        {
          double groupArea = CalculateArea(group[0]);
          if (Math.Abs(objectArea - groupArea) / groupArea <= areaThreshold)
          {
            group.Add(obj);
            addedToGroup = true;
            break;
          }
        }

        if (!addedToGroup)
        {
          groups.Add(new List<MagentaObject> { obj });
        }
      }

      return groups;
    }

    private double CalculateArea(MagentaObject obj)
    {
      double width = obj.MaxPoint.X - obj.MinPoint.X;
      double height = obj.MaxPoint.Y - obj.MinPoint.Y;
      return width * height;
    }
  }
}
