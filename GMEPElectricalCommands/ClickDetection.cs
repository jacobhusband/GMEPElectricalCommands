using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricalCommands
{
  public class ClickDetection
  {
    [CommandMethod("Recep")]
    public void Recep()
    {
      Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
      Editor ed = doc.Editor;

      PromptPointResult ppr = ed.GetPoint("\nClick a point: ");
      if (ppr.Status != PromptStatus.OK) return;
      Point3d clickPoint = ppr.Value;

      System.Windows.Point point = ed.PointToScreen(clickPoint, 0);

      System.Drawing.Point cursorPosition = System.Windows.Forms.Cursor.Position;

      int captureWidth = 200;
      int captureHeight = 200;

      int centerX = captureWidth / 2;
      int centerY = captureHeight / 2;

      Rectangle captureRectangle = new Rectangle(cursorPosition.X - centerX, cursorPosition.Y - centerY, captureWidth, captureHeight);

      using (Bitmap bitmap = new Bitmap(captureRectangle.Width, captureRectangle.Height))
      {
        using (Graphics g = Graphics.FromImage(bitmap))
        {
          g.CopyFromScreen(captureRectangle.Location, Point.Empty, captureRectangle.Size);
        }

        // Create a new bitmap to hold the circular image
        Bitmap circularBitmap = new Bitmap(bitmap.Width, bitmap.Height, bitmap.PixelFormat);

        using (Graphics g = Graphics.FromImage(circularBitmap))
        {
          // Create a circular path that fits within the bounds of the bitmap
          using (GraphicsPath path = new GraphicsPath())
          {
            path.AddEllipse(0, 0, bitmap.Width, bitmap.Height);
            g.SetClip(path);

            // Draw the original bitmap onto the graphics of the new bitmap
            g.DrawImage(bitmap, 0, 0);
          }
        }

        // Step 1: Get a list of every gray pixel
        List<System.Drawing.Point> grayPixels = new List<System.Drawing.Point>();
        for (int x = 0; x < bitmap.Width; x++)
        {
          for (int y = 0; y < bitmap.Height; y++)
          {
            System.Drawing.Color pixelColor = bitmap.GetPixel(x, y);
            if (pixelColor.R == pixelColor.G && pixelColor.G == pixelColor.B && pixelColor.R != 0)
            {
              grayPixels.Add(new System.Drawing.Point(x, y));
            }
          }
        }

        var closestGrayPixel = FindClosestGrayPixelWithBlackBorder(grayPixels, bitmap, centerX, centerY);
        var closestGrayPixelsInPoint = FindGrayPixelsAroundClosest(grayPixels, bitmap, centerX, centerY, closestGrayPixel);
        var closestGrayPixelsInLine = FindClosestGrayPixelsWithBlackBorders(grayPixels, bitmap, 30, centerX, centerY);

        Tuple<List<double>, List<double>> xyVals = SplitPointsIntoXListAndYList(closestGrayPixelsInLine);
        List<double> xVals = xyVals.Item1;
        List<double> yVals = xyVals.Item2;

        double[] xValsArray = xVals.ToArray();
        double[] yValsArray = yVals.ToArray();

        double rSquared, yIntercept, slope;

        // Call the LinearRegression method
        LinearRegression(xValsArray, yValsArray, out rSquared, out yIntercept, out slope);

        // Create two points
        System.Drawing.PointF pt1 = new System.Drawing.PointF(0, (float)yIntercept);
        System.Drawing.PointF pt2 = new System.Drawing.PointF(1, (float)(slope + yIntercept));

        // Create a line as a tuple of two points
        Tuple<System.Drawing.PointF, System.Drawing.PointF> line = new Tuple<System.Drawing.PointF, System.Drawing.PointF>(pt1, pt2);

        line = EnsureCounterClockwise(line, centerX, centerY);

        var unitVector = GetOrthogonalVector(line);

        var averageX = closestGrayPixelsInPoint.Average(p => p.X);
        var averageY = closestGrayPixelsInPoint.Average(p => p.Y);
        var averagePoint = new System.Drawing.Point((int)averageX, (int)averageY);

        Vector2d vectorToCenter = new Vector2d(centerX - averagePoint.X, centerY - averagePoint.Y);

        var newPoint = new System.Windows.Point(point.X - vectorToCenter.X, point.Y - vectorToCenter.Y);

        var convertedBackPoint = ed.PointToWorld(newPoint, 0);

        MakeRecepBlockReference(doc, convertedBackPoint, unitVector);
      }
    }

    public Tuple<System.Drawing.PointF, System.Drawing.PointF> EnsureCounterClockwise(Tuple<System.Drawing.PointF, System.Drawing.PointF> line, float centerX, float centerY)
    {
      var pt1 = line.Item1;
      var pt2 = line.Item2;

      // Calculate the angles of the points from the positive x-axis
      var angle1 = Math.Atan2(pt1.Y - centerY, pt1.X - centerX);
      var angle2 = Math.Atan2(pt2.Y - centerY, pt2.X - centerX);

      // If the angle of pt2 is less than the angle of pt1, swap the points
      if (angle2 < angle1)
      {
        var temp = pt1;
        pt1 = pt2;
        pt2 = temp;
      }

      return new Tuple<System.Drawing.PointF, System.Drawing.PointF>(pt1, pt2);
    }

    private Tuple<List<double>, List<double>> SplitPointsIntoXListAndYList(List<System.Drawing.Point> points)
    {
      List<double> xVals = new List<double>();
      List<double> yVals = new List<double>();

      foreach (var point in points)
      {
        xVals.Add(point.X);
        yVals.Add(point.Y);
      }

      return new Tuple<List<double>, List<double>>(xVals, yVals);
    }

    private Vector3d GetOrthogonalVector(Tuple<System.Drawing.PointF, System.Drawing.PointF> line)
    {
      // Calculate the difference in x and y coordinates between the end point and the start point
      double dx = line.Item2.X - line.Item1.X;
      double dy = line.Item2.Y - line.Item1.Y;

      // Create a vector from the differences
      Vector3d vector = new Vector3d(dx, dy, 0);

      // Create a vector pointing in the 0,0,1 direction
      Vector3d upVector = new Vector3d(0, 0, 1);

      // Calculate the cross product of the two vectors
      Vector3d crossProduct = vector.CrossProduct(upVector);

      // Normalize the cross product to get a unit vector
      Vector3d unitVector = crossProduct.GetNormal();

      return unitVector;
    }

    public static void LinearRegression(double[] xVals, double[] yVals, out double rSquared, out double yIntercept, out double slope)
    {
      if (xVals.Length != yVals.Length)
      {
        throw new System.Exception("Input values should be with the same length.");
      }

      double sumOfX = 0;
      double sumOfY = 0;
      double sumOfXSq = 0;
      double sumOfYSq = 0;
      double sumCodeviates = 0;

      for (var i = 0; i < xVals.Length; i++)
      {
        var x = xVals[i];
        var y = yVals[i];
        sumCodeviates += x * y;
        sumOfX += x;
        sumOfY += y;
        sumOfXSq += x * x;
        sumOfYSq += y * y;
      }

      var count = xVals.Length;
      var ssX = sumOfXSq - ((sumOfX * sumOfX) / count);
      var ssY = sumOfYSq - ((sumOfY * sumOfY) / count);

      var rNumerator = (count * sumCodeviates) - (sumOfX * sumOfY);
      var rDenom = (count * sumOfXSq - (sumOfX * sumOfX)) * (count * sumOfYSq - (sumOfY * sumOfY));
      var sCo = sumCodeviates - ((sumOfX * sumOfY) / count);

      var meanX = sumOfX / count;
      var meanY = sumOfY / count;
      var dblR = rNumerator / Math.Sqrt(rDenom);

      rSquared = dblR * dblR;
      yIntercept = meanY - ((sCo / ssX) * meanX);
      slope = sCo / ssX;
    }

    private List<System.Drawing.Point> FindClosestGrayPixelsWithBlackBorders(List<System.Drawing.Point> grayPixels, Bitmap bitmap, int amount, int centerImageX, int centerImageY)
    {
      var grayPixelsWithBlackBorders = FindGrayPixelsWithBlackBorders(grayPixels, bitmap, centerImageX, centerImageY);

      var distances = grayPixelsWithBlackBorders.Select(p => new
      {
        Pixel = p,
        Distance = Math.Sqrt(Math.Pow(p.X - centerImageX, 2) + Math.Pow(p.Y - centerImageY, 2))
      });

      var closestGrayPixels = distances.OrderBy(d => d.Distance).Take(amount).Select(d => d.Pixel).ToList();

      return closestGrayPixels;
    }

    private List<System.Drawing.Point> FindGrayPixelsWithBlackBorders(List<System.Drawing.Point> grayPixels, Bitmap bitmap, int centerImageX, int centerImageY)
    {
      List<System.Drawing.Point> grayPixelsWithBlackBorders = new List<System.Drawing.Point>();

      foreach (var grayPixel in grayPixels)
      {
        List<System.Drawing.Point> blackBorderPixels = new List<System.Drawing.Point>();

        // Check the pixel to the left
        if (grayPixel.X > 0 && bitmap.GetPixel(grayPixel.X - 1, grayPixel.Y).R == 0)
        {
          blackBorderPixels.Add(new System.Drawing.Point(grayPixel.X - 1, grayPixel.Y));
        }
        // Check the pixel to the right
        if (grayPixel.X < bitmap.Width - 1 && bitmap.GetPixel(grayPixel.X + 1, grayPixel.Y).R == 0)
        {
          blackBorderPixels.Add(new System.Drawing.Point(grayPixel.X + 1, grayPixel.Y));
        }
        // Check the pixel above
        if (grayPixel.Y > 0 && bitmap.GetPixel(grayPixel.X, grayPixel.Y - 1).R == 0)
        {
          blackBorderPixels.Add(new System.Drawing.Point(grayPixel.X, grayPixel.Y - 1));
        }
        // Check the pixel below
        if (grayPixel.Y < bitmap.Height - 1 && bitmap.GetPixel(grayPixel.X, grayPixel.Y + 1).R == 0)
        {
          blackBorderPixels.Add(new System.Drawing.Point(grayPixel.X, grayPixel.Y + 1));
        }

        double grayPixelDistance = Math.Sqrt(Math.Pow(grayPixel.X - centerImageX, 2) + Math.Pow(grayPixel.Y - centerImageY, 2));

        foreach (var blackPixel in blackBorderPixels)
        {
          double blackPixelDistance = Math.Sqrt(Math.Pow(blackPixel.X - centerImageX, 2) + Math.Pow(blackPixel.Y - centerImageY, 2));

          if (blackPixelDistance < grayPixelDistance)
          {
            grayPixelsWithBlackBorders.Add(grayPixel);
            break;
          }
        }
      }
      return grayPixelsWithBlackBorders;
    }

    private void MakeRecepBlockReference(Document doc, Point3d convertedBackPoint, Vector3d unitVector)
    {
      var blockName = "RECEP";
      using (Transaction tr = doc.Database.TransactionManager.StartTransaction())
      {
        BlockTable bt = (BlockTable)tr.GetObject(doc.Database.BlockTableId, OpenMode.ForRead);
        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

        BlockReference br = new BlockReference(convertedBackPoint, bt[blockName]);

        // Calculate the angle of rotation from the vector
        double angle = Math.Atan2(unitVector.Y, unitVector.X);

        // Correct the angle for AutoCAD's coordinate system
        angle = Math.PI / 2 - angle;

        if (angle < 0)
        {
          angle += 2 * Math.PI;
        }

        // Set the rotation of the block reference
        br.Rotation = angle;

        btr.AppendEntity(br);
        tr.AddNewlyCreatedDBObject(br, true);

        tr.Commit();
      }
    }

    private List<System.Drawing.Point> FindGrayPixelsAroundClosest(List<System.Drawing.Point> grayPixels, Bitmap bitmap, int centerImageX, int centerImageY, System.Drawing.Point? closestGrayPixel)
    {
      if (!closestGrayPixel.HasValue)
      {
        return new List<System.Drawing.Point>();
      }

      var surroundingGrayPixels = new List<System.Drawing.Point>();

      for (int x = closestGrayPixel.Value.X - 2; x <= closestGrayPixel.Value.X + 2; x++)
      {
        for (int y = closestGrayPixel.Value.Y - 2; y <= closestGrayPixel.Value.Y + 2; y++)
        {
          if (x >= 0 && x < bitmap.Width && y >= 0 && y < bitmap.Height)
          {
            System.Drawing.Color pixelColor = bitmap.GetPixel(x, y);
            if (pixelColor.R == pixelColor.G && pixelColor.G == pixelColor.B && pixelColor.R != 0)
            {
              surroundingGrayPixels.Add(new System.Drawing.Point(x, y));
            }
          }
        }
      }

      return surroundingGrayPixels;
    }

    private System.Drawing.Point? FindClosestGrayPixelWithBlackBorder(List<System.Drawing.Point> grayPixels, Bitmap bitmap, int centerImageX, int centerImageY)
    {
      System.Drawing.Point? closestGrayPixel = null;
      double closestDistance = double.MaxValue;

      foreach (var grayPixel in grayPixels)
      {
        // Check the surrounding pixels for a black one
        var surroundingPoints = new List<System.Drawing.Point>
        {
            new System.Drawing.Point(grayPixel.X - 1, grayPixel.Y),
            new System.Drawing.Point(grayPixel.X + 1, grayPixel.Y),
            new System.Drawing.Point(grayPixel.X, grayPixel.Y - 1),
            new System.Drawing.Point(grayPixel.X, grayPixel.Y + 1)
        };

        foreach (var point in surroundingPoints)
        {
          if (point.X >= 0 && point.X < bitmap.Width && point.Y >= 0 && point.Y < bitmap.Height)
          {
            System.Drawing.Color pixelColor = bitmap.GetPixel(point.X, point.Y);
            if (pixelColor.R == 0 && pixelColor.G == 0 && pixelColor.B == 0) // if the pixel is black
            {
              double distance = Math.Sqrt(Math.Pow(grayPixel.X - centerImageX, 2) + Math.Pow(grayPixel.Y - centerImageY, 2));
              if (distance < closestDistance)
              {
                closestDistance = distance;
                closestGrayPixel = grayPixel;
              }
              break;
            }
          }
        }
      }

      return closestGrayPixel;
    }

    private void SaveDataToDesktop(object data, string fileName)
    {
      string json = Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
      string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
      System.IO.File.WriteAllText(filePath, json);
    }
  }
}