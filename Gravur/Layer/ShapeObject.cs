using System;
using MapTools;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using GravurGIS.Shapes;
using System.Drawing;
using GravurGIS.Topology.QuadTree;
using GravurGIS.Topology.Grid;
using GravurGIS.Topology;
using System.Drawing.Imaging;
using GravurGIS.Rendering;

namespace GravurGIS.Layers
{
    public class ShapeObject : Layer, IIdentifyable, IDisposable
    {
        #region Properties

        // Shape related
        private ShapeLib.ShapeType shapeType;
              
        private bool isPoint = false;
        private String m_filePath;
        private String m_fileName;
        private String access;
        private uint numberOfShapes;

        // Daten
        private PointList[] polyLinePointList;
        private Grid partGrid;
        private uint partCount;

        // Framework      
        private LayerManager layerManager;
       
        private Bitmap bitmap;
        private bool prepareBitmap = true;
        private ImageAttributes imageAttributes;

        private LayerInfo layerInfo;
        private VectorInfo vectorInfo;
       
        private bool basePoints = false;
       
        private const double constZoomLevelLimit = 2;
        private bool bOutOfMemory;

        private int[] selectedItems;

        // memory quadtree
        IntPtr quadTree = IntPtr.Zero;


        #endregion

        public ShapeObject(LayerManager layerManager, String filePath) {

            this._layerType = LayerType.Shape;
            this.Description = "ESRI Shapedatei";
            this.Changed = this.Visible = true;

            this.layerManager = layerManager;
            this.LayerInfo = new LayerInfo(filePath);
            this.access = "rb";
            Color tempColor = layerManager.NewLayerStyle.NewColor;
            this.vectorInfo = new VectorInfo(tempColor,Color.White,new Pen(tempColor,1.0f));
            this.m_filePath = System.IO.Path.GetDirectoryName(filePath);
            this.m_fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);

            int i = filePath.LastIndexOf("\\");

            this.LayerName = filePath.Substring(i + 1,
                filePath.LastIndexOf(".") - i - 1);

            // new InmageAttributes
            imageAttributes = new ImageAttributes();
            imageAttributes.SetColorKey(Color.NavajoWhite, Color.NavajoWhite);
            bitmap = new Bitmap(1, 1, PixelFormat.Format16bppRgb555);
        }

        public void init()
        {
            layerManager.generateGeometryStructureInDLL(this, ref partGrid);
        }

        public String getFilePath()
        {
            return LayerInfo.FilePath;
        }

        public override bool Render(RenderProperties rp)
        {
            if (this.shapeType != ShapeLib.ShapeType.NullShape && NumberOfShapes > 0) 
            {
                if (Changed || rp.ScreenChanged)
                {
                    int tempPointSize = layerManager.PointSize;
                    Pen layerPen = this.vectorInfo.LayerPen;

                    if (rp.AbsoluteZoom > constZoomLevelLimit || bOutOfMemory)
                    {
                        // if we are switching to this "mode" (e.g. calculating all the points
                        // again on every change) we must initialize the bitmapList with drawingArea
                        // sized bitmaps since we do not want to create them all the time from scratch
                        if (prepareBitmap)
                        {
                            bitmap.Dispose();
                            bitmap = new Bitmap(rp.DrawingArea.Width,
                                            rp.DrawingArea.Height,
                                            System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
                            prepareBitmap = false;
                        }

                        // this is done because multiplication is faster than division
                        double invAbsZoom = 1.0 / rp.AbsoluteZoom;

						Rectangle collideRec = new Rectangle(1, 1,
							(int)(Math.Ceiling(rp.DrawingArea.Width * invAbsZoom)),
							(int)(Math.Ceiling(rp.DrawingArea.Height * invAbsZoom)));
						//Rectangle collideRec = new Rectangle(1, 1,
						//    (int)(Math.Ceiling(rp.ClipRectangle.Width * invAbsZoom)),
						//    (int)(Math.Ceiling(rp.ClipRectangle.Height * invAbsZoom)));

                        Graphics gx = Graphics.FromImage(bitmap);
                        gx.Clear(Color.NavajoWhite);
						
						
						// clip the collide rectanlge
						//clipRectagle
						
                        // TODO: kann man das nicht irgendwie zusammenfassen / beschleunigen?
						collideRec.X = (int)(rp.DX * invAbsZoom - BoundingBox.Left * rp.Scale * invAbsZoom);
						collideRec.Y = (int)(-(rp.DY * invAbsZoom - BoundingBox.Top * rp.Scale * invAbsZoom));
						//collideRec.X = (int)(  (rp.DX + rp.ClipRectangle.X) * invAbsZoom - BoundingBox.Left * rp.Scale * invAbsZoom);
						//collideRec.Y = (int)(-((rp.DY + rp.ClipRectangle.Y) * invAbsZoom - BoundingBox.Top * rp.Scale * invAbsZoom));
                        
                        
                        long relativeDiffX = Convert.ToInt64(rp.DX - BoundingBox.Left * rp.Scale);
                        //int relativeDiffX = Convert.ToInt32(rp.DX);
                        long relativeDiffY = Convert.ToInt64(BoundingBox.Top * rp.Scale - rp.DY);
                        //int relativeDiffY = Convert.ToInt32(-rp.DY);

                        List<PointList> pl = new List<PointList>();
                        partGrid.CollidingPoints(collideRec, ref pl);
                        if (!isPoint)
                        {
                            Point[] temp;                           

                            // treat polygons special since they can be filled
                            if (this.shapeType == MapTools.ShapeLib.ShapeType.Polygon && vectorInfo.Fill)
                            {
                                Brush layerBrush = new SolidBrush(this.vectorInfo.FillColor);

                                for (int pointList = 0; pointList < pl.Count; pointList++)
                                {
                                    temp = new Point[pl[pointList].Length];
                                    for (int p = 0; p < temp.Length; p++)
                                    {
                                        temp[p].X = (int)(pl[pointList].getDispPoint(p).X - relativeDiffX);
                                        temp[p].Y = (int)(pl[pointList].getDispPoint(p).Y - relativeDiffY);
                                    }
                                    gx.FillPolygon(layerBrush, temp);

                                    gx.DrawLines(layerPen, temp);
                                }
                            }
                            else if (this.shapeType != MapTools.ShapeLib.ShapeType.MultiPoint)
                            {
                                for (int pointList = 0; pointList < pl.Count; pointList++)
                                {
                                    temp = new Point[pl[pointList].Length];

                                    for (int p = 0; p < temp.Length; p++)
                                    {
                                        temp[p].X = (int)(pl[pointList].getDispPoint(p).X - relativeDiffX);
                                        temp[p].Y = (int)(pl[pointList].getDispPoint(p).Y - relativeDiffY);
                                    }

                                    gx.DrawLines(layerPen, temp);
                                }
                            }
                            else
                            {
                                Rectangle drawRect = new Rectangle();
                                Brush tempPointBrush = new SolidBrush(this.VectorInfo.LayerPen.Color);

                                for (int pointList = 0; pointList < pl.Count; pointList++)
                                {
                                    temp = new Point[pl[pointList].Length];
                                    for (int p = 0; p < temp.Length; p++)
                                    {
                                        temp[p].X = (int)(pl[pointList].getDispPoint(p).X - relativeDiffX);
                                        temp[p].Y = (int)(pl[pointList].getDispPoint(p).Y - relativeDiffY);
                                    }

                                    drawRect = new Rectangle();

                                    for (int p = 0; p < temp.Length; p++)
                                    {
                                        drawRect.X = (int)(temp[p].X - (tempPointSize >> 1)); // >>1 is faster thatn *2
                                        drawRect.Y = (int)(temp[p].Y - (tempPointSize >> 1));
                                        drawRect.Width = tempPointSize;
                                        drawRect.Height = tempPointSize;
                                        gx.FillEllipse(tempPointBrush, drawRect);
                                    }

                                }
                            }
                        }
                        else // only one point in shape
                        {
                            gx.DrawEllipse(VectorInfo.LayerPen, new Rectangle((int)(_boundingBox.Left - relativeDiffX),
                                (int)(_boundingBox.Bottom - relativeDiffY), tempPointSize, tempPointSize));
                        }
                        gx.Dispose();
                    }
                    else
                    { // we rendered the image
                        if (Changed)
                        {
                            bitmap.Dispose();

                            try
                            {   //try: drawing Points
                                bitmap = new Bitmap(
                                Convert.ToInt32(Width * rp.Scale + tempPointSize * 0.5 + 1),
                                Convert.ToInt32(Height * rp.Scale + tempPointSize * 0.5 + 1),
                                System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
                            }
                            catch (OutOfMemoryException)
                            {
                                // jump over to resuce plan B :P
                                bOutOfMemory = true;
                                prepareBitmap = true;
                                return false;
                            }

                            Graphics gx = Graphics.FromImage(bitmap);
                            gx.Clear(Color.NavajoWhite);

                            if (shapeType == MapTools.ShapeLib.ShapeType.MultiPoint)
                            {
                                Rectangle drawRect = new Rectangle();

                                Brush tempPointBrush = new SolidBrush(VectorInfo.LayerPen.Color);
                                Point[] intPL;
                                for (int pointList = 0; pointList < polyLinePointList.Length; pointList++)
                                {
                                    intPL = polyLinePointList[pointList].displayPointList;
                                    drawRect.X = (int)(intPL[0].X - (tempPointSize >> 1)); // >>1 is faster than *2
                                    drawRect.Y = (int)(intPL[0].Y - (tempPointSize >> 1));
                                    if (drawRect.X < 0) drawRect.X = 0;
                                    if (drawRect.Y < 0) drawRect.Y = 0;
                                    drawRect.Width = tempPointSize;
                                    drawRect.Height = tempPointSize;
                                    gx.FillEllipse(tempPointBrush, drawRect);
                                }

                            }
                            else
                            {
                                if (vectorInfo.Fill)
                                {
                                    Brush layerBrush = new SolidBrush(vectorInfo.FillColor);

                                    for (int pointList = 0; pointList < polyLinePointList.Length; pointList++)
                                    {
                                        gx.FillPolygon(layerBrush, polyLinePointList[pointList].displayPointList);
                                        gx.DrawLines(VectorInfo.LayerPen, polyLinePointList[pointList].displayPointList);
                                    }
                                }
                                else
                                    for (int pointList = 0; pointList < polyLinePointList.Length; pointList++)
                                        gx.DrawLines(VectorInfo.LayerPen, polyLinePointList[pointList].displayPointList);
                            }

                            if (isPoint)
                            {
                                double tempScale = layerManager.Scale;
                                Brush tempBrush = new SolidBrush(VectorInfo.LayerPen.Color);
                                gx.FillEllipse(tempBrush,
                                    new Rectangle(
                                    (int)((_boundingBox.Left) * tempScale),
                                    (int)((_boundingBox.Bottom) * tempScale),
                                    tempPointSize, tempPointSize));
                            }
                            gx.Dispose();
                            Changed = false;
                        }
                    }
                }

                Rectangle destRect = new Rectangle(rp.DrawingArea.X, rp.DrawingArea.Y,
                    bitmap.Width, bitmap.Height);

                if (rp.AbsoluteZoom <= constZoomLevelLimit && !bOutOfMemory)
                {
                    destRect.X += Convert.ToInt32(BoundingBox.Left * rp.Scale - rp.DX);
                    destRect.Y += Convert.ToInt32(rp.DY - BoundingBox.Top * rp.Scale);
                }

                rp.G.DrawImage(bitmap, destRect, 0, 0,
                            destRect.Width,
                            destRect.Height,
                            GraphicsUnit.Pixel, imageAttributes);


                #region Feature highlighting
                
                // this has to be drawn directly to screen since we do not
                // want to change the stored image
                if (rp.Highlight && selectedItems != null)
                {
                    int tempPointSize = layerManager.PointSize;
                    Point[] intPL;
                    Graphics g = rp.G;
                    int drawingAreaX = rp.DrawingArea.X;
                    int drawingAreaY = rp.DrawingArea.Y;
                    Pen highlightPen = new Pen(Color.Red, this.vectorInfo.LayerPen.Width + 2);

                    if (rp.AbsoluteZoom > constZoomLevelLimit || bOutOfMemory)
                    {
                        #region "direct draw" case
                        //long relativeDiffX = Convert.ToInt64(dX - relativeLeft * absoluteZoom);
                        //long relativeDiffY = Convert.ToInt64(-(dY + relativeTop * absoluteZoom));

                        long relativeDiffX = Convert.ToInt64(rp.DX - BoundingBox.Left * rp.Scale);
                        long relativeDiffY = Convert.ToInt64(BoundingBox.Top * rp.Scale - rp.DY);

                        PointList myPl;

                        if (IsWellDefined)
                        {
                            for (int i = selectedItems.Length - 1; i >= 0; i--)
                            {
                                myPl = polyLinePointList[selectedItems[i]];

                                intPL = new Point[myPl.Length];
                                for (int p = 0; p < intPL.Length; p++)
                                {
                                    intPL[p].X = (int)(myPl.getDispPoint(p).X - relativeDiffX);
                                    intPL[p].Y = (int)(myPl.getDispPoint(p).Y - relativeDiffY);
                                }
                                if (this.shapeType != ShapeLib.ShapeType.MultiPoint)
                                    g.DrawLines(highlightPen, intPL);
                                else
                                {
                                    Rectangle drawRect = new Rectangle();
                                    Brush tempPointBrush = new SolidBrush(Color.Red);

                                    for (int p = 0; p < intPL.Length; p++)
                                    {
                                        drawRect.X = (int)(intPL[p].X - tempPointSize / 2);
                                        drawRect.Y = (int)(intPL[p].Y - tempPointSize / 2);
                                        drawRect.Width = tempPointSize;
                                        drawRect.Height = tempPointSize;
                                        g.FillEllipse(tempPointBrush, drawRect);
                                    }

                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        # region Bitmap drawing case
                        if (IsWellDefined)
                        {
                            if (this.shapeType != ShapeLib.ShapeType.MultiPoint)
                            {
                                for (int i = selectedItems.Length - 1; i >= 0; i--)
                                {
                                    intPL = (Point[])(polyLinePointList[selectedItems[i]].displayPointList.Clone());
                                    for (int j = intPL.Length - 1; j >= 0; j--)
                                    {
                                        intPL[j].X += destRect.X;
                                        intPL[j].Y += destRect.Y;
                                    }
                                    g.DrawLines(highlightPen, intPL);
                                }
                            }
                            else
                            {
                                Rectangle drawRect = new Rectangle();
                                Brush tempPointBrush = new SolidBrush(Color.Red);

                                for (int i = selectedItems.Length - 1; i >= 0; i--)
                                {
                                    intPL = (Point[])(polyLinePointList[selectedItems[i]].displayPointList.Clone());
                                    drawRect.X = (int)(intPL[0].X - tempPointSize * 0.5 + destRect.X);
                                    drawRect.Y = (int)(intPL[0].Y - tempPointSize * 0.5 + destRect.Y);
                                    drawRect.Width = drawRect.Height = tempPointSize;
                                    g.FillEllipse(tempPointBrush, drawRect);
                                }
                            }
                        }
                        #endregion
                    }
                }
                #endregion
            }

            return true;
        }

        public override void reset()
        {
            prepareBitmap = true;
        }

        #region Getter/Setter
        
        public PointList[] PolyLinePointList
        {
            get { return polyLinePointList; }
            set { polyLinePointList = value; }
        }
        public uint NumberOfShapes
        {
            get { return numberOfShapes; }
            set { numberOfShapes = value; }
        }
       
        /// <summary>
        /// The unscaled (!) width of the layer
        /// </summary>
        public new double Width
        {
            get 
            {
                if ((_boundingBox.Width) != 0) return _boundingBox.Width;
                else return layerManager.ShpSize;        
            }
        }
        /// <summary>
        /// The unscaled (!) height of the layer
        /// </summary>
        public new double Height
        {
            get
            {
                if ((_boundingBox.Height) != 0) return _boundingBox.Height;
                else return layerManager.ShpSize;
            }
        }

        public ShapeLib.ShapeType ShapeType
        {
            get { return shapeType; }
            set { shapeType = value; }
        }

        public bool BasePoints
        {
            get { return basePoints; }
        }

        public new WorldBoundingBoxD BoundingBox
        {
            get { return this._boundingBox; }
            set { this._boundingBox = value; }
        }

        public String Access
        {
            get { return access; }
        }

        public uint PartCount
        {
            get { return partCount; }
            set { partCount = value; }
        }

        public bool IsOnePoint
        {
            get { return isPoint; }
            set { isPoint = value; }
        }
        #endregion

        #region ILayer Members


        public override void recalculateData(double absoluteZoom, double scale, double xOff, double yOff)
        {
			if (PartCount > 0) {
				int dispHeight = Convert.ToInt32(_boundingBox.Height * scale);
				
				for (uint pointList = PartCount - 1; pointList != 0; pointList--)
					polyLinePointList[pointList].recalculatePoints(scale, dispHeight, _boundingBox.Left, _boundingBox.Bottom);
				Changed = true;
            }
        }

        public LayerInfo LayerInfo
        {
            get { return layerInfo; }
            set { layerInfo = value; }
        }

        public VectorInfo VectorInfo
        {
            get { return vectorInfo; }
            set { vectorInfo = value; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // delete quadtree
            if (quadTree != IntPtr.Zero)
                MapTools.ShapeLib.SHPDestroyTree(quadTree);
        }

        #endregion

        #region IIdentifyable Members

        public bool IsWellDefined
        {
            get;
            set;
        }

        public bool IsIndexed
        {
            get
            {
                if (this.quadTree == IntPtr.Zero) return false;
                else return true;
            }
        }

        public int[] identify(double x, double y)
        {
            selectedItems = new int[0];
            if (quadTree == IntPtr.Zero)
            {
                this.quadTree = MapTools.ShapeLib.getQuadTree(
                    System.IO.Path.Combine(m_filePath, m_fileName),
                    this.access);
                if (this.quadTree == IntPtr.Zero)
                    MessageBox.Show("Fehler 0x4443: Konnte Index nicht erstellen!");
                return null;
            }

            double[] bbMin = { x - 0, y - 0 };
            double[] bbMax = { x + 0, y + 0 };
            int count = 0;
            IntPtr resultList = MapTools.ShapeLib.SHPTreeFindLikelyShapes(quadTree, bbMin, bbMax, ref count);

            if (count > 0)
            {
                selectedItems = new int[count];
                Marshal.Copy(resultList, selectedItems, 0, count);

                // check which one are the closest
                if (IsWellDefined)
                {
                    double minDist = Double.MaxValue;    

                    if (ShapeType == ShapeLib.ShapeType.MultiPoint)
                    {

                        int minIndex = 0;

                        double bbLeft = this._boundingBox.Left;
                        double bbTop = this._boundingBox.Top;
                        double tempDist = 0;
                        double pntX, pntY;

                        for (int i = count - 1; i >= 0; i--)
                        {
                            tempDist = distance(
                                polyLinePointList[selectedItems[i]].worldPointList[0].x,
                                polyLinePointList[selectedItems[i]].worldPointList[0].y,
                                x, y);

                            if (tempDist < minDist)
                            {
                                minDist = tempDist;
                                minIndex = i;
                            }
                            
                        }
                        selectedItems = new int[1] { selectedItems[minIndex] };
                    }
                    else
                    {
                        if (shapeType == ShapeLib.ShapeType.Polygon ||
                            shapeType == ShapeLib.ShapeType.PolyLine)
                        {
                            int minIndex = 0;

                            double bbLeft = this._boundingBox.Left;
                            double bbTop = this._boundingBox.Top;
                            double tempDist = 0;
                            double pntX, pntY;
                            PointList item;

                            for (int i = count - 1; i >= 0; i--)
                            {
                                item = polyLinePointList[selectedItems[i]];

                                for (int part = item.Length - 1; i > 1; i--)
                                {
                                    tempDist = linePointDist(
                                        item.worldPointList[part].x,
                                        item.worldPointList[part].y,
                                        item.worldPointList[part - 1].x,
                                        item.worldPointList[part - 1].y,
                                        x,
                                        y);

                                    if (tempDist < minDist)
                                    {
                                        minDist = tempDist;
                                        minIndex = i;
                                    }
                                }
                            }
                            selectedItems = new int[1] { selectedItems[minIndex] };
                        }
                    }
                }
            }
            return selectedItems;
        }

        /// <summary>
        /// Compute the dot product of AB * BC
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        double dot(double Ax, double Ay,
            double Bx, double By,
            double Cx, double Cy)
        {
            double[] AB = new double[2];
            double[] BC = new double[2];
            AB[0] = Bx - Ax;
            AB[1] = By - Ay;
            BC[0] = Cx - Bx;
            BC[1] = Cy - By;

            return AB[0] * BC[0] + AB[1] * BC[1];
        }
        
        /// <summary>
        /// Compute the cross product AB x AC
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        double cross(
            double Ax, double Ay,
            double Bx, double By,
            double Cx, double Cy)
        {
            double[] AB = new double[2];
            double[] AC = new double[2];
            AB[0] = Bx - Ax;
            AB[1] = By - Ay;
            AC[0] = Cx - Ax;
            AC[1] = Cy - Ay;
            return AB[0] * AC[1] - AB[1] * AC[0];
        }
        
        /// <summary>
        /// Compute the distance from A to B
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        double distance(double Ax, double Ay,
            double Bx, double By)
        {
            double d1 = Ax - Bx;
            double d2 = Ay - By;
            return Math.Sqrt(d1 * d1 + d2 * d2);
        }

        /// <summary>
        /// Compute the distance from AB to C
        /// if isSegment is true, AB is a segment, not a line.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        double linePointDist(double Ax, double Ay,
            double Bx, double By,
            double Cx, double Cy)
        {
            double dist = cross(Ax, Ay, Bx, By, Cx, Cy) / distance(Ax, Ay, Bx, By);
            double dot1 = dot(Ax, Ay, Bx, By, Cx, Cy);
            if (dot1 > 0) return distance(Bx, By, Cx, Cy);
            double dot2 = dot(Bx, By, Ax, Ay, Cx, Cy);
            if (dot2 > 0) return distance(Ax, Ay, Cx, Cy);
            return Math.Abs(dist);
        }

        public void clearSelection()
        {
            selectedItems = null;
        }

        #endregion
    }
}
