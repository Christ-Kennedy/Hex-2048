using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;
namespace Math3
{
    public class classMath3
    {

        public static double dblTwoPi = 2.0 * Math.PI;
        public static double dblHalfPi = Math.PI / 2.0;
  
        public static void cleanAngle(ref double dblAngle)
        {
            dblAngle = cleanAngle(dblAngle);
        }

        public static double cleanAngle(double dblAngle)
        {
            while (dblAngle > TwoPi) dblAngle -= TwoPi;
            while (dblAngle < 0) dblAngle += TwoPi;
            if (double.IsNaN(dblAngle))
                dblAngle = 0;

            return dblAngle;
        }

        public static double ConvertRadiansToDegrees(double dblRadians)
        {
            return 180.0 * dblRadians / Math.PI;
        }

        public static double ConvertDegreesToRadians(double dblDegrees)
        {
            return dblDegrees / 180.0 * Math.PI;
        }

        public static double ConvertDegreesToRadians(int intDegrees)
        {
            return (double)intDegrees / 180.0 * Math.PI;
        }

        public static double RoundNearest(double dblIn, int intDecimalPoint)
        {
            dblIn = dblIn * Math.Pow(10, intDecimalPoint);

            double dblRetVal = (double)(int)dblIn;
            double dblDecimal = dblIn - dblRetVal;
            if (dblDecimal >= 0.5) dblRetVal++;

            dblRetVal = dblRetVal / Math.Pow(10, intDecimalPoint);
            return dblRetVal;
        }


        public static double DotProduct(Point pt1, Point pt2)
        {
            return pt1.X * pt2.X + pt1.Y * pt2.Y;
        }
        public static double DotProduct(PointF ptf1, PointF ptf2)
        {
            return ptf1.X * ptf2.X + ptf1.Y * ptf2.Y;
        }

        public static classPointF3D CrossProduct(classPointF3D A, classPointF3D B)
        {
            return new classPointF3D(A.Y * B.Z - A.Z * B.Y, A.Z * B.X - A.X * B.Z, A.X * B.Y - A.Y * B.X);
        }
        public static double Get_ShortestDistancePointLine(classLineF cLine, PointF ptf, ref PointF ptfNearestOnLine)
        {
            // calculate a vector from 1st pt on line to center of circle 
            PointF ptfA = new PointF(ptf.X - cLine.ptf1.X, ptf.Y - cLine.ptf1.Y);

            // calculate unit normal to line => unit-B = B / |B|
            classRadialCoordinate cRad_Line = new classRadialCoordinate(cLine.ptf2, cLine.ptf1);
            classRadialCoordinate cRad_Normal = new classRadialCoordinate(cRad_Line.Radians + Math.PI / 2, cRad_Line.Magnitude);

            PointF ptfNormal = cRad_Normal.toPointF();
            classLineF cLineNormal = new classLineF(ptf, AddTwoPointFs(ptfNormal, ptf));
            cRad_Normal.Magnitude = 1;

            PointF ptfBUnit = cRad_Normal.toPointF();
            double dblRetVal = Math.Abs(DotProduct(ptfBUnit, ptfA));

            /*          calculating the nearest PointF on line
             line eq'n ->   y = Mx + B
                            at intersection between perpendicular line2 through ball and line1
                            both lines meet at some PointF (x,y)
             normal to M = -1/M
             therefore      y = M1x + B1 = (-1/M1x) + B2

             therefore       x = (b2-b1) / (M1 + 1/M1)

             B2 = y - M2x 
                = y + x/M1  - true for any PointF(x,y) on line2 (e.g. center of the circle)
             */
            cLine.IntersectsLine(ref cLineNormal, ref ptfNearestOnLine);

            return dblRetVal;
        }

        public static double Get_ShortestDistanceCircleLine(classLineF cLine, classCircle cCircle, ref PointF ptfNearestOnLine)
        {
            // calculate a vector from 1st pt on line to center of circle 
            PointF ptfA = new PointF(cCircle.ptfCenter.X - cLine.ptf1.X, cCircle.ptfCenter.Y - cLine.ptf1.Y);

            // calculate unit normal to line => unit-B = B / |B|
            classRadialCoordinate cRad_Line = new classRadialCoordinate(cLine.ptf2, cLine.ptf1);
            classRadialCoordinate cRad_Normal = new classRadialCoordinate(cRad_Line.Radians + Math.PI / 2, cRad_Line.Magnitude);

            PointF ptfNormal = cRad_Normal.toPointF();
            classLineF cLineNormal = new classLineF(cCircle.ptfCenter, AddTwoPointFs(ptfNormal, cCircle.ptfCenter));
            cRad_Normal.Magnitude = 1;

            PointF ptfBUnit = cRad_Normal.toPointF();
            double dblRetVal = Math.Abs(DotProduct(ptfBUnit, ptfA));

            /*          calculating the nearest PointF on line
             line eq'n ->   y = Mx + B
                            at intersection between perpendicular line2 through ball and line1
                            both lines meet at some PointF (x,y)
             normal to M = -1/M
             therefore      y = M1x + B1 = (-1/M1x) + B2

             therefore       x = (b2-b1) / (M1 + 1/M1)

             B2 = y - M2x 
                = y + x/M1  - true for any PointF(x,y) on line2 (e.g. center of the circle)
             */
            cLine.IntersectsLine(ref cLineNormal, ref ptfNearestOnLine);

            return dblRetVal;
        }

        public static int Get_Sign(int intValue)
        {
            if (intValue == 0) return 0;
            else return (int)intValue / Math.Abs(intValue);
        }

        public static int Get_Sign(double dblValue)
        {
            if (dblValue == 0) return 0;
            else return (int)(dblValue / Math.Abs(dblValue));
        }
        public static bool SameSign(int int1, int int2) { return ((int1 < 0 && int2 < 0) || (int1 > 0 && int2 > 0)); }

        public static bool SameSign(double dbl1, double dbl2) { return ((dbl1 < 0 && dbl2 < 0) || (dbl1 > 0 && dbl2 > 0)); }

        public static bool RectanglesIntersect(RectangleF rec1, RectangleF rec2)
        {
            //if (rec1.Width == 0 || rec1.Height == 0
            //  || rec2.Width == 0 || rec2.Height == 0) return false;
            bool bolHorizontal = (rec1.Left >= rec2.Left && rec1.Right < rec2.Right)             // rec 1 inside rec2
                              || (rec1.Left <= rec2.Left && rec1.Right > rec2.Right)             // rec 1 envelopes rec2
                              || (rec1.Left <= rec2.Left && rec1.Right > rec2.Left)              // rec 1 straddles rec2's left side
                              || (rec1.Left <= rec2.Right && rec1.Right > rec2.Right);           // rec 1 straddles rec2's right side

            bool bolVertical = (rec1.Top >= rec2.Top && rec1.Bottom < rec2.Bottom)               // rec 1 inside rec2
                              || (rec1.Top <= rec2.Top && rec1.Bottom > rec2.Bottom)             // rec 1 envelopes rec2
                              || (rec1.Top <= rec2.Top && rec1.Bottom > rec2.Top)                // rec 1 straddles rec2's Top side
                              || (rec1.Top <= rec2.Bottom && rec1.Bottom > rec2.Bottom);         // rec 1 straddles rec2's Bottom side
            return bolVertical && bolHorizontal;
        }
        public static bool RectanglesIntersect(Rectangle rec1, Rectangle rec2)
        {
            //if (rec1.Width == 0 || rec1.Height == 0
            //  || rec2.Width == 0 || rec2.Height == 0) return false;
            bool bolHorizontal = (rec1.Left >= rec2.Left && rec1.Right <= rec2.Right)             // rec 1 inside rec2
                              || (rec1.Left <= rec2.Left && rec1.Right >= rec2.Right)             // rec 1 envelopes rec2
                              || (rec1.Left <= rec2.Left && rec1.Right >= rec2.Left)              // rec 1 straddles rec2's left side
                              || (rec1.Left < rec2.Right && rec1.Right >= rec2.Right);           // rec 1 straddles rec2's right side

            bool bolVertical = (rec1.Top >= rec2.Top && rec1.Bottom <= rec2.Bottom)               // rec 1 inside rec2
                              || (rec1.Top <= rec2.Top && rec1.Bottom >= rec2.Bottom)             // rec 1 envelopes rec2
                              || (rec1.Top <= rec2.Top && rec1.Bottom >= rec2.Top)                // rec 1 straddles rec2's Top side
                              || (rec1.Top <= rec2.Bottom && rec1.Bottom >= rec2.Bottom);         // rec 1 straddles rec2's Bottom side
            return bolVertical && bolHorizontal;
        }
        public static bool RectanglesIntersect(Rectangle rec1, RectangleF rec2)
        {
            //if (rec1.Width == 0 || rec1.Height == 0
            //   || rec2.Width == 0 || rec2.Height == 0) return false;
            bool bolHorizontal = (rec1.Left >= rec2.Left && rec1.Right < rec2.Right)             // rec 1 inside rec2
                              || (rec1.Left <= rec2.Left && rec1.Right > rec2.Right)             // rec 1 envelopes rec2
                              || (rec1.Left <= rec2.Left && rec1.Right > rec2.Left)              // rec 1 straddles rec2's left side
                              || (rec1.Left <= rec2.Right && rec1.Right > rec2.Right);           // rec 1 straddles rec2's right side

            bool bolVertical = (rec1.Top >= rec2.Top && rec1.Bottom < rec2.Bottom)               // rec 1 inside rec2
                              || (rec1.Top <= rec2.Top && rec1.Bottom > rec2.Bottom)             // rec 1 envelopes rec2
                              || (rec1.Top <= rec2.Top && rec1.Bottom > rec2.Top)                // rec 1 straddles rec2's Top side
                              || (rec1.Top <= rec2.Bottom && rec1.Bottom > rec2.Bottom);         // rec 1 straddles rec2's Bottom side
            return bolVertical && bolHorizontal;
        }

        public static Rectangle RectangleFromTwoPoints(Point pt1, Point pt2)
        {
            return new Rectangle(pt1.X <= pt2.X ? pt1.X : pt2.X,
                                 pt1.Y <= pt2.Y ? pt1.Y : pt2.Y,
                                (int)Math.Abs(pt1.X - pt2.X),
                                (int)Math.Abs(pt1.Y - pt2.Y));

        }

        public static Rectangle RectangleFromTwoPointFs(PointF pt1, PointF pt2)
        {
            return new Rectangle(pt1.X <= pt2.X ? (int)pt1.X : (int)pt2.X,
                                 pt1.Y <= pt2.Y ? (int)pt1.Y : (int)pt2.Y,
                                 (int)Math.Abs(pt1.X - pt2.X),
                                 (int)Math.Abs(pt1.Y - pt2.Y));
        }

        public static RectangleF RectangleFFromTwoPoints(Point pt1, Point pt2)
        {
            return new RectangleF(pt1.X <= pt2.X ? pt1.X : pt2.X,
                                 pt1.Y <= pt2.Y ? pt1.Y : pt2.Y,
                                (int)Math.Abs(pt1.X - pt2.X),
                                (int)Math.Abs(pt1.Y - pt2.Y));

        }

        public static RectangleF RectangleThatContainsAllPoints(List<PointF> lstPtf)
        {
            IEnumerable<PointF> query_X = lstPtf.OrderBy(ptf => ptf.X);
            List<PointF> lstPtf_XOrder = (List<PointF>)query_X.ToList<PointF>();

            IEnumerable<PointF> query_Y = lstPtf.OrderBy(ptf => ptf.Y);
            List<PointF> lstPtf_YOrder = (List<PointF>)query_Y.ToList<PointF>();

            return new RectangleF(new PointF(lstPtf_XOrder[0].X, lstPtf_YOrder[0].Y), new SizeF(lstPtf_XOrder[lstPtf_XOrder.Count - 1].X - lstPtf_XOrder[0].X, lstPtf_YOrder[lstPtf_YOrder.Count - 1].Y - lstPtf_YOrder[0].Y));
        }

        public static RectangleF RectangleFFromTwoPointFs(PointF pt1, PointF pt2)
        {
            return new RectangleF(pt1.X <= pt2.X ? (float)pt1.X : (float)pt2.X,
                                 pt1.Y <= pt2.Y ? (float)pt1.Y : (float)pt2.Y,
                                 (float)Math.Abs(pt1.X - pt2.X),
                                 (float)Math.Abs(pt1.Y - pt2.Y));

        }

        /// <summary>
        /// adds an ellipse to a graphics path and returns a description of the same ellipse
        /// </summary>
        /// <param name="grPath">graphic path into which the new graphic ellipse is added</param>
        /// <param name="bolClockwise">rotational direction of arc of the ellipse to be added</param>
        /// <param name="pt0">starting point of the ellipse added to the graphics path</param>
        /// <param name="pt1">end point of the ellipse added to the graphics path</param>
        /// <param name="intSegmentHeight">the height of the ellipse measured as the shortest distance between the output ellipse arc and a point located equidistant from the start and end points</param>
        /// <returns>a description of the arc added to the graphics path</returns>
        public static classGraphicArcDescription GraphicsPath_Add(ref GraphicsPath grPath, bool bolClockwise, PointF pt0, Point pt1, int intSegmentHeight)
        {
            double dblArc_Height = (double)intSegmentHeight;
            classRadialCoordinate cRad = new classRadialCoordinate(pt0, pt1);
            double dblArc_Width = cRad.Magnitude;

            PointF ptfBetweenInputs = new PointF((pt0.X + (pt1.X - pt0.X) / 2), (pt0.Y + (pt1.Y - pt0.Y) / 2));
            float fltBaseAngle = (float)(cRad.Degrees
                                        + (bolClockwise ? -90 : 90));

            if (bolClockwise)
                cRad.Radians -= Math.PI / 2;
            else
                cRad.Radians += Math.PI / 2;

            //https://www.mathopenref.com/arcradius.html 
            // R = h/2 + w^2/(8h)
            double dblRadius = dblArc_Height / 2 + Math.Pow(dblArc_Width, 2) / (8 * dblArc_Height);
            double dblTheta = Math.Asin((dblArc_Width / 2) / dblRadius);
            float fltDegree_Start = fltBaseAngle - (bolClockwise ? 1 : -1) * (float)(180.0 / Math.PI * dblTheta);
            float fltDegree_Sweep = (bolClockwise ? 1 : -1) * (float)(360.0 / Math.PI * dblTheta);
            cRad.Magnitude = dblRadius - dblArc_Height;
            cRad.Radians += Math.PI;
            PointF ptfCenterEllipse = AddTwoPointFs(ptfBetweenInputs, cRad.toPointF());
            RectangleF recEllipse = new RectangleF((float)ptfCenterEllipse.X - (float)dblRadius, (float)ptfCenterEllipse.Y - (float)dblRadius, (2.0f * (float)dblRadius), (2.0f * (float)dblRadius));

            classGraphicArcDescription cRetVal = new classGraphicArcDescription(recEllipse, fltDegree_Start, fltDegree_Sweep);

            grPath.AddArc(recEllipse, fltDegree_Start, fltDegree_Sweep);

            return cRetVal;
        }
        /// <summary>
        /// returns the rectangle that contains the arc of a circle being described
        /// </summary>
        /// <param name="bolClockwise">direction of the arc's rotation from point pt0 to pt1</param>
        /// <param name="pt0">start point of the arc being described</param>
        /// <param name="pt1">end point of the arc being described</param>
        /// <param name="intSegmentHeight">greatest distance between a line segment from pt0 to pt1 and the ellipse being described</param>
        /// <returns>returns the smallest rectangle that can hold the circle which describes this arc</returns>
        public static RectangleF RectangleContainsArc(bool bolClockwise, PointF pt0, Point pt1, int intSegmentHeight)
        {
            float fltStart = 0f;
            float fltSweep = 0f;
            return RectangleContainsArc(bolClockwise, pt0, pt1, intSegmentHeight, ref fltStart, ref fltSweep);
        }
        /// <summary>
        /// returns the rectangle that contains the arc of a circle being described
        /// </summary>
        /// <param name="bolClockwise">direction of the arc's rotation from point pt0 to pt1</param>
        /// <param name="pt0">start point of the arc being described</param>
        /// <param name="pt1">end point of the arc being described</param>
        /// <param name="intSegmentHeight">greatest distance between a line segment from pt0 to pt1 and the ellipse being described</param>
        /// <param name"fltAngle_Start">passed as reference - is set to the start angle of the arc before returning</param>
        /// <param name"fltAngle_Start">passed as reference - is set to the sweep angle of the arc before returning</param>
        /// <returns>returns the smallest rectangle that can hold the circle which describes this arc</returns>
        public static RectangleF RectangleContainsArc(bool bolClockwise, PointF pt0, Point pt1, int intSegmentHeight, ref float fltDegree_Start, ref float fltDegree_Sweep)
        {
            double dblArc_Height = (double)intSegmentHeight;
            classRadialCoordinate cRad = new classRadialCoordinate(pt0, pt1);
            double dblArc_Width = cRad.Magnitude;

            PointF ptfBetweenInputs = new PointF((pt0.X + (pt1.X - pt0.X) / 2), (pt0.Y + (pt1.Y - pt0.Y) / 2));
            float fltBaseAngle = (float)(cRad.Degrees
                                        + (bolClockwise ? -90 : 90));

            if (bolClockwise)
                cRad.Radians -= Math.PI / 2;
            else
                cRad.Radians += Math.PI / 2;

            //https://www.mathopenref.com/arcradius.html 
            // R = h/2 + w^2/(8h)
            double dblRadius = dblArc_Height / 2 + Math.Pow(dblArc_Width, 2) / (8 * dblArc_Height);
            double dblTheta = Math.Asin((dblArc_Width / 2) / dblRadius);
            fltDegree_Start = fltBaseAngle - (bolClockwise ? 1 : -1) * (float)(180.0 / Math.PI * dblTheta);
            fltDegree_Sweep = (bolClockwise ? 1 : -1) * (float)(360.0 / Math.PI * dblTheta);
            cRad.Magnitude = dblRadius - dblArc_Height;
            cRad.Radians += Math.PI;
            PointF ptfCenterEllipse = AddTwoPointFs(ptfBetweenInputs, cRad.toPointF());
            RectangleF recEllipse = new RectangleF((float)ptfCenterEllipse.X - (float)dblRadius, (float)ptfCenterEllipse.Y - (float)dblRadius, (2.0f * (float)dblRadius), (2.0f * (float)dblRadius));
            return recEllipse;
        }

        public class classGraphicArcDescription
        {
            public RectangleF rec = new RectangleF();
            public float flt_Degree_start = 0;
            public float flt_Degree_Sweep = 0;
            public classGraphicArcDescription(RectangleF _rec, float _flt_Degree_start, float _flt_Degree_Sweep)
            {
                rec = _rec;
                flt_Degree_start = _flt_Degree_start;
                flt_Degree_Sweep = _flt_Degree_Sweep;
            }
        }
        

        public static bool CircleIntersectaPolygon(ref PointF[] ptfPolygon, classCircle cCircle)
        {
            for (int intPtfCounter = 0; intPtfCounter < ptfPolygon.Length; intPtfCounter++)
            {
                PointF ptfStart = ptfPolygon[intPtfCounter];
                PointF ptfEnd = ptfPolygon[(intPtfCounter + 1) % ptfPolygon.Length];

                classLineF cLineF = new classLineF(ptfStart, ptfEnd);
                PointF ptfNearestToLine = new PointF();
                Get_ShortestDistanceCircleLine(cLineF, cCircle, ref ptfNearestToLine);
                if (Math.Abs(ptfNearestToLine.X - cCircle.ptfCenter.X) < cCircle.dblRadius && Math.Abs(ptfNearestToLine.Y - cCircle.ptfCenter.Y) < cCircle.dblRadius)
                    return true;
            }
            return false;
        }

        public static Point Get_PointFromElement(XmlNode xPt)
        {
            int intX = 0, intY = 0;
            try
            {
                intX = Convert.ToInt32(xPt.Attributes[0].InnerText);
                intY = Convert.ToInt32(xPt.Attributes[1].InnerText);

            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Error:" + e.Message);
            }

            return new Point(intX, intY);
        }

        const string conXmlField_Point_X = "X";
        const string conXmlField_Point_Y = "Y";
        public static XmlNode Get_NodeFromPointF(ref XmlDocument xDoc, string strName, PointF ptf)
        {
            XmlElement xRetVal = xDoc.CreateElement(strName);

            XmlNode xAtt_X = xDoc.CreateNode(XmlNodeType.Attribute, conXmlField_Point_X, "pt");
            XmlNode xAtt_Y = xDoc.CreateNode(XmlNodeType.Attribute, conXmlField_Point_Y, "pt");

            xAtt_X.Value = ptf.X.ToString();
            xAtt_Y.Value = ptf.Y.ToString();

            xRetVal.Attributes.SetNamedItem(xAtt_X);
            xRetVal.Attributes.SetNamedItem(xAtt_Y);

            return xRetVal;
        }
        public static XmlNode Get_NodeFromPoint(ref XmlDocument xDoc, string strName, Point pt)
        {
            XmlElement xRetVal = xDoc.CreateElement(strName);

            XmlNode xAtt_X = xDoc.CreateNode(XmlNodeType.Attribute, conXmlField_Point_X, "pt");
            XmlNode xAtt_Y = xDoc.CreateNode(XmlNodeType.Attribute, conXmlField_Point_Y, "pt");

            xAtt_X.Value = pt.X.ToString();
            xAtt_Y.Value = pt.Y.ToString();

            xRetVal.Attributes.SetNamedItem(xAtt_X);
            xRetVal.Attributes.SetNamedItem(xAtt_Y);

            return xRetVal;
        }

        public static PointF Get_PointFFromXml(XmlNode xPtf)
        {
            double dblX = 0, dblY = 0;
            try
            {
                dblX = Convert.ToDouble(xPtf.Attributes[0].InnerText);
                dblY = Convert.ToDouble(xPtf.Attributes[1].InnerText);

            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Error:" + e.Message);
            }

            return new PointF((float)dblX, (float)dblY);
        }

        public static PointF PointFFromPoint(Point pt)
        {
            return new PointF(pt.X, pt.Y);
        }


        public static PointF PointF_FactorSize(PointF ptf, float factorSize)
        {
            return new PointF(ptf.X * factorSize, ptf.Y * factorSize);
        }
        public static Point Point_FactorSize(PointF pt, float factorSize) 
        {
            return new Point((int)(pt.X * factorSize), (int)(pt.Y * factorSize));
        }

        public static PointF PointF_FactorSize(PointF pt, double factorSize)
        {
            return new PointF((int)((double)pt.X * factorSize), (int)((double)pt.Y * factorSize));
        }

        public static PointF AverageTwoPointF(PointF ptf1, PointF ptf2)
        {
            return new PointF((ptf1.X + ptf2.X) / 2, (ptf1.Y + ptf2.Y) / 2);
        }

        public static double Get_SweepRadians(double dblStart, double dblFinish)
        {
            dblStart = cleanAngle(dblStart);
            dblFinish = cleanAngle(dblFinish);
            if (dblFinish < dblStart)
                dblFinish += 2 * Math.PI;
            return dblFinish - dblStart;
        }

        public static void Sort(ref List<double> lst)
        {
            for (int intOuterCounter = 0; intOuterCounter < lst.Count - 1; intOuterCounter++)
            {
                for (int intInnerCounter = intOuterCounter + 1; intInnerCounter < lst.Count; intInnerCounter++)
                {
                    if (lst[intOuterCounter] > lst[intInnerCounter])
                    {
                        double dblTemp = lst[intOuterCounter];
                        lst[intOuterCounter] = lst[intInnerCounter];
                        lst[intInnerCounter] = dblTemp;
                    }
                }
            }
        }

        /// <summary>
        /// returns the difference between the start angle and the finish angle rotating in positive direction
        /// </summary>
        /// <param name="dblStart">angle sweep starts</param>
        /// <param name="dblFinish">angle sweep  ends</param>
        /// <returns>the angle that sweeps between start and finish</returns>
        public static double Get_SweepAngle(double dblStart, double dblFinish)
        {
            dblStart = cleanDegrees(dblStart);
            dblFinish = cleanDegrees(dblFinish);
            if (dblStart > dblFinish)
                dblFinish += (2.0 * Math.PI);


            double dblRetVal = cleanDegrees( dblFinish - dblStart);
            return dblRetVal;
        }

        /// <summary>
        /// determines whether a given angle is located within an angle sweep 
        /// </summary>
        /// <param name="dblAngle">angle being tested</param>
        /// <param name="dblStart">start angle of sweep</param>
        /// <param name="dblSweep">number of degrees the angle is to be swept</param>
        /// <returns>true if the input angle is within the sweep</returns>
        public static bool AngleIsInSweep(double dblAngle, double dblStart, double dblSweep)
        {
            dblAngle = cleanDegrees(dblAngle);
            dblStart = cleanDegrees(dblStart);
            dblSweep = cleanDegrees(dblSweep);

            if (dblStart + dblSweep > 360)
            {
                if (dblAngle > dblStart)
                    return true;
                return (dblAngle < cleanDegrees(dblStart + dblSweep));
            }
            else
            {
                return (dblAngle > dblStart && dblAngle < dblStart + dblSweep);
            }
        }

        /// <summary>
        /// determines whether a given angle is located within an angle sweep 
        /// </summary>
        /// <param name="dblRadian">angle being tested</param>
        /// <param name="dblStart">start angle of sweep</param>
        /// <param name="dblSweep">number of Radiansthe angle is to be swept</param>
        /// <returns>true if the input angle is within the sweep</returns>
        public static bool RadianAngleIsInSweep(double dblRadian, double dblStart, double dblSweep)
        {
            dblRadian = cleanAngle(dblRadian);
            dblStart = cleanAngle(dblStart);
            dblSweep = cleanAngle(dblSweep);

            if (dblStart + dblSweep > 2*Math.PI)
            {
                if (dblRadian> dblStart)
                    return true;
                return (dblRadian < cleanAngle(dblStart + dblSweep));
            }
            else
            {
                return (dblRadian > dblStart && dblRadian < dblStart + dblSweep);
            }
        }

        /// <summary>
        /// returns a value between 0 & 360 degrees equivalent to the input value
        /// </summary>
        /// <param name="dblAngle">angle to be replaced with equivalent between 0 & 360</param>
        /// <returns>value equivalent to input between 0 & 360 degrees</returns>
        public static double cleanDegrees(double dblAngle)
        {
            while (dblAngle > 360)
                dblAngle -= 360;
            while (dblAngle < 0)
                dblAngle += 360;
            return dblAngle;

        }
        public const double TwoPi = Math.PI * 2.0;

        public static classRadialCoordinate CarToRad(PointF pt)
        {
            classRadialCoordinate cRad_RetVal = new classRadialCoordinate((double)Math.Atan2(pt.Y, pt.X), (int)Math.Sqrt((double)(pt.X * pt.X) + (double)(pt.Y * pt.Y)));
            return cRad_RetVal;
        }


        public static Point RadToPt(classRadialCoordinate cRad, double dblAngle)
        {
            Point RetVal = new Point();
            RetVal.X = (int)(cRad.Magnitude * Math.Cos(cRad.Radians + dblAngle));
            RetVal.Y = (int)(cRad.Magnitude * Math.Sin(cRad.Radians + dblAngle));
            return RetVal;
        }

        public static Point RadToPt(classRadialCoordinate cRad, double dblAngle, double dblSizeFactor)
        {
            Point RetVal = new Point();
            RetVal.X = (int)(cRad.Magnitude * dblSizeFactor * Math.Cos(cRad.Radians + dblAngle));
            RetVal.Y = (int)(cRad.Magnitude * dblSizeFactor * Math.Sin(cRad.Radians + dblAngle));
            return RetVal;
        }


        public static PointF RadToPtF(classRadialCoordinate cRad, double dblAngle)
        {
            PointF RetVal = new PointF();
            RetVal.X = (int)(cRad.Magnitude * Math.Cos(cRad.Radians + dblAngle));
            RetVal.Y = (int)(cRad.Magnitude * Math.Sin(cRad.Radians + dblAngle));
            return RetVal;
        }

        public static PointF RadToPtF(classRadialCoordinate cRad, double dblAngle, double dblSizeFactor)
        {
            PointF RetVal = new PointF();
            RetVal.X = (int)(cRad.Magnitude * dblSizeFactor * Math.Cos(cRad.Radians + dblAngle));
            RetVal.Y = (int)(cRad.Magnitude * dblSizeFactor * Math.Sin(cRad.Radians + dblAngle));
            return RetVal;
        }
        public static PointF AddTwoDVectors(PointF pt1, PointF fpt2)
        {
            PointF fpt_RetVal = new PointF();
            fpt_RetVal.X = pt1.X + fpt2.X;
            fpt_RetVal.Y = pt1.Y + fpt2.Y;
            return fpt_RetVal;
        }

        public static PointF AddTwoPoints(Point pt1, PointF ptf2) { return new PointF(pt1.X + ptf2.X, pt1.Y + ptf2.Y); }

        public static Point AddTwoPoints(Point pt1, Point pt2) { return new Point(pt1.X + pt2.X, pt1.Y + pt2.Y); }
        public static PointF AddTwoPointFs(PointF ptf1, PointF ptf2) { return new PointF(ptf1.X + ptf2.X, ptf1.Y + ptf2.Y); }

        /// <summary>
        /// returns the vector difference between the two PointFs pt1-pt2
        /// </summary>
        /// <param name="pt1">PointF from which second PointF is subtracted</param>
        /// <param name="pt2">PointF subtracted from the first</param>
        /// <returns></returns>
        public static PointF SubTwoPointFs(PointF pt1, PointF pt2) { return new PointF(pt1.X - pt2.X, pt1.Y - pt2.Y); }

        /// <summary>
        /// returns the vector difference between the two PointFs pt1-pt2
        /// </summary>
        /// <param name="pt1">PointF from which second PointF is subtracted</param>
        /// <param name="pt2">PointF subtracted from the first</param>
        /// <returns></returns>
        public static Point SubTwoPoints(Point pt1, Point pt2) { return new Point(pt1.X - pt2.X, pt1.Y - pt2.Y); }

        public static bool PointIsInsideARectangle(PointF ptf, List<RectangleF> lstRecF)
        {
            for (int intRecCounter = 0; intRecCounter < lstRecF.Count; intRecCounter++)
            {
                if (PointIsInsideARectangle(ptf, lstRecF[intRecCounter]))
                    return true;
            }
            return false;
        }

        public static bool PointIsInsideARectangle(PointF ptf, RectangleF recF)
        {
            return (ptf.X >= recF.Left
                          && ptf.X <= recF.Right
                          && ptf.Y >= recF.Top
                          && ptf.Y <= recF.Bottom);
        }

        public static bool PointFIsInsideAPolygon(PointF[] ptfPolygon, PointF ptf)
        {
            if (ptfPolygon == null
                ||
                ptfPolygon.Length < 3) return false;

            RectangleF recF = RectangleThatContainsAllPoints(ptfPolygon.ToList());
            if (!PointIsInsideARectangle(ptf, recF))
                return false;

            classLineF cLineX = new classLineF();
            cLineX.ptf1 = ptf;
            cLineX.ptf2 = new PointF(float.MaxValue, ptf.Y);

            classLineF cLineTest = new classLineF();

            cLineTest.ptf2 = ptfPolygon[ptfPolygon.Length - 1];
            PointF ptfIntersect = new PointF();
            int intNumIntersect = 0;
            for (int intTestCounter = 0; intTestCounter < ptfPolygon.Length; intTestCounter++)
            {
                cLineTest.ptf1 = cLineTest.ptf2;
                cLineTest.ptf2 = ptfPolygon[intTestCounter];

                if (cLineTest.ptf1.Y == cLineTest.ptf2.Y
                    &&
                    cLineTest.ptf1.Y == cLineX.ptf1.Y)
                {
                    float flt1Min = cLineTest.ptf1.X < cLineTest.ptf2.X
                                                     ? cLineTest.ptf1.X
                                                     : cLineTest.ptf2.X;
                    float flt1Max = cLineTest.ptf1.X > cLineTest.ptf2.X
                                                     ? cLineTest.ptf1.X
                                                     : cLineTest.ptf2.X;
                    float flt2Min = cLineX.ptf1.X < cLineX.ptf2.X
                                                   ? cLineX.ptf1.X
                                                   : cLineX.ptf2.X;
                    float flt2Max = cLineX.ptf1.X > cLineX.ptf2.X
                                                   ? cLineX.ptf1.X
                                                   : cLineX.ptf2.X;
                    if ((flt1Min < flt2Max && flt1Max > flt2Max)       // line1 straddles line2 max
                         || (flt1Min < flt2Min && flt1Max > flt2Min)    // line1 straddles line2 min
                         || (flt2Min < flt1Min && flt2Max > flt1Min)    // line2 straddles line1 min
                         || (flt2Min < flt1Max && flt2Max > flt1Max)    // line2 straddles line1 max
                         || (flt1Min > flt2Min && flt1Max < flt2Max)    // line1 rests inside line2 (line2 covers all of line1)
                         || (flt1Min < flt2Min && flt1Max > flt2Max))    // line1 covers all of line2 (line2 rests inside line1)
                        intNumIntersect++;
                }
                else if (cLineTest.IntersectsLine(ref cLineX, ref ptfIntersect))
                    intNumIntersect++;
            }

            return (intNumIntersect % 2 == 1);
        }

    }

    public class classRotateImage
    {

        static bool bolMaintainSize = false;
        public static bool MaintainSize
        {
            get { return bolMaintainSize; }
            set { bolMaintainSize = value; }
        }
      
        
        static public Bitmap rotateImage(Bitmap bmpInput, double dblRadians)
        {
            // calculate size of output bitmap
            Point ptCenter_Input = new Point(bmpInput.Width / 2, bmpInput.Height / 2);
            List<Point> lstCorners_Input = new List<Point>();
            Point ptTL_Input = new Point(-ptCenter_Input.X, -ptCenter_Input.Y);
            Point ptTR_Input = new Point(ptCenter_Input.X, -ptCenter_Input.Y);
            Point ptBL_Input = new Point(-ptCenter_Input.X, ptCenter_Input.Y);
            Point ptBR_Input = new Point(ptCenter_Input.X, ptCenter_Input.Y);
            lstCorners_Input.Add(ptTL_Input);
            lstCorners_Input.Add(ptTR_Input);
            lstCorners_Input.Add(ptBR_Input);
            lstCorners_Input.Add(ptBL_Input);

            List<Point> lstCorners_Output = new List<Point>();
            Point ptTL_Output = new Point();
            Point ptBR_Output = new Point();
            for (int intCornerCounter = 0; intCornerCounter < 4; intCornerCounter++)
            {
                classRadialCoordinate cRad = new classRadialCoordinate(lstCorners_Input[intCornerCounter]);
                cRad.Radians -= dblRadians;
                Point ptOutput = cRad.toPoint();
                if (ptOutput.X < ptTL_Output.X)
                    ptTL_Output.X = ptOutput.X;
                if (ptOutput.X > ptBR_Output.X)
                    ptBR_Output.X = ptOutput.X;
                if (ptOutput.Y < ptTL_Output.Y)
                    ptTL_Output.Y = ptOutput.Y;
                if (ptOutput.Y > ptBR_Output.Y)
                    ptBR_Output.Y = ptOutput.Y;

                lstCorners_Output.Add(ptOutput);
            }
            // draw frame of output map where rotated image needs to be painted
            Size szOutput = new Size(ptBR_Output.X - ptTL_Output.X + 1, ptBR_Output.Y - ptTL_Output.Y + 1);
            Point ptCenter_Output = new Point(szOutput.Width / 2, szOutput.Height / 2);

            for (int intCornerCounter = 0; intCornerCounter < 4; intCornerCounter++)
                lstCorners_Output[intCornerCounter] = classMath3.AddTwoPoints(lstCorners_Output[intCornerCounter], ptCenter_Output);

            Bitmap bmpTemp = new Bitmap(szOutput.Width, szOutput.Height);
            using (Graphics g = Graphics.FromImage(bmpTemp))
            {
                g.FillRectangle(Brushes.White, new RectangleF(0, 0, szOutput.Width, szOutput.Height));
                for (int intCornerCounter = 0; intCornerCounter < 4; intCornerCounter++)
                {
                    int intP1 = intCornerCounter;
                    int intP2 = (intCornerCounter + 1) % 4;
                    g.DrawLine(Pens.Black, lstCorners_Output[intP1], lstCorners_Output[intP2]);
                }
            }
            // trace frame output bitmap and copy boolean results to BFS array
            List<Point> lstTraceResults = TraceLine(bmpTemp, lstCorners_Output[0]);
            bool[,] bolBFS = new bool[szOutput.Width, szOutput.Height];

            Bitmap bmpRetVal = new Bitmap(szOutput.Width, szOutput.Height);

            using (Graphics g = Graphics.FromImage(bmpRetVal))
                g.FillRectangle(Brushes.White, new RectangleF(0, 0, bmpRetVal.Width, bmpRetVal.Height));
            while (lstTraceResults.Count > 0)
            {
                Point ptTrace = lstTraceResults[0];
                lstTraceResults.RemoveAt(0);
                bolBFS[ptTrace.X, ptTrace.Y] = true;
                bmpRetVal.SetPixel(ptTrace.X, ptTrace.Y, Color.Red);
            }
            using (Graphics g = Graphics.FromImage(bmpRetVal))
                g.FillRectangle(Brushes.White, new RectangleF(0, 0, bmpRetVal.Width, bmpRetVal.Height));

            Point pt = ptCenter_Output;
            List<Point> lstQ = new List<Point>();
            lstQ.Add(pt);

            while (lstQ.Count > 0)
            {
                pt = lstQ[0];
                lstQ.RemoveAt(0);


                if (!bolBFS[pt.X, pt.Y])
                {
                    bolBFS[pt.X, pt.Y] = true;
                    if (pt.X > 1)
                    {
                        Point ptLeft = new Point(pt.X - 1, pt.Y);
                        if (!bolBFS[ptLeft.X, ptLeft.Y])
                        {
                            lstQ.Add(ptLeft);
                        }
                    }

                    if (pt.Y > 1)
                    {
                        Point ptUp = new Point(pt.X, pt.Y - 1);
                        if (!bolBFS[ptUp.X, ptUp.Y])
                        {
                            lstQ.Add(ptUp);
                        }
                    }

                    if (pt.X < bmpRetVal.Width - 1)
                    {
                        Point ptRight = new Point(pt.X + 1, pt.Y);
                        if (!bolBFS[ptRight.X, ptRight.Y])
                        {
                            lstQ.Add(ptRight);
                        }
                    }

                    if (pt.Y < bmpRetVal.Height - 1)
                    {
                        Point ptDown = new Point(pt.X, pt.Y + 1);
                        if (!bolBFS[ptDown.X, ptDown.Y])
                        {
                            lstQ.Add(ptDown);
                        }
                    }

                    // draw pixel
                    {

                        classRadialCoordinate cRad = new classRadialCoordinate(ptCenter_Output, pt);
                        cRad.Radians += dblRadians;
                        Point ptSample = classMath3.AddTwoPoints(ptCenter_Input, cRad.toPoint());
                        if (ptSample.X < 0)
                            ptSample.X = 0;
                        if (ptSample.X >= bmpInput.Width)
                            ptSample.X = bmpInput.Width - 1;
                        if (ptSample.Y < 0)
                            ptSample.Y = 0;
                        if (ptSample.Y >= bmpInput.Height)
                            ptSample.Y = bmpInput.Height - 1;
                        bmpRetVal.SetPixel(pt.X, pt.Y, bmpInput.GetPixel(ptSample.X, ptSample.Y));
                    }
                }
            }

            return bmpRetVal;
        }
        /// <summary>
        /// traces along the line on a bitmap and returns all points on that line with same pixel-color
        /// </summary>
        /// <param name="bmp">input bitmap on which the line is traced</param>
        /// <param name="ptSeed">point on the line located on the bitmap used to seed the search</param>
        /// <returns>returns a complete list of all points of the same color attached to the seed point</returns>
        static List<Point> TraceLine(Bitmap bmp, Point ptSeed)
        {
            if (bmp == null || ptSeed.X < 0 || ptSeed.Y < 0 || ptSeed.X >= bmp.Width || ptSeed.Y >= bmp.Height) return new List<Point>();


            bool[,] bolPixelsSeen = new bool[bmp.Width, bmp.Height];

            // detect the color of the line
            Color clrLine = bmp.GetPixel(ptSeed.X, ptSeed.Y);

            // create a Q to hold points yet to be seen
            List<Point> lstQ = new List<Point>();
            //object objNull = (object)null;

            lstQ.Add(ptSeed);
            bolPixelsSeen[ptSeed.X, ptSeed.Y] = true;

            List<Point> lstRetVal = new List<Point>();
            while (lstQ.Count > 0)
            {
                Point pt = lstQ[0];
                lstQ.RemoveAt(0);

                Color clrTest = bmp.GetPixel(pt.X, pt.Y);
                if (clrTest.R == clrLine.R && clrTest.G == clrLine.G && clrTest.B == clrLine.B && !lstRetVal.Contains(pt))
                {
                    lstRetVal.Add(pt);

                    if (pt.X > 1)
                    {
                        Point ptLeft = new Point(pt.X - 1, pt.Y);
                        if (!bolPixelsSeen[ptLeft.X, ptLeft.Y])
                        {
                            lstQ.Add(ptLeft);
                            bolPixelsSeen[ptLeft.X, ptLeft.Y] = true;
                        }
                    }

                    if (pt.Y > 1)
                    {
                        Point ptUp = new Point(pt.X, pt.Y - 1);
                        if (!bolPixelsSeen[ptUp.X, ptUp.Y])
                        {
                            lstQ.Add(ptUp);
                            bolPixelsSeen[ptUp.X, ptUp.Y] = true;
                        }
                    }

                    if (pt.X < bmp.Width - 1)
                    {
                        Point ptRight = new Point(pt.X + 1, pt.Y);
                        if (!bolPixelsSeen[ptRight.X, ptRight.Y])
                        {
                            lstQ.Add(ptRight);
                            bolPixelsSeen[ptRight.X, ptRight.Y] = true;
                        }
                    }

                    if (pt.Y < bmp.Height - 1)
                    {
                        Point ptDown = new Point(pt.X, pt.Y + 1);
                        if (!bolPixelsSeen[ptDown.X, ptDown.Y])
                        {
                            lstQ.Add(ptDown);
                            bolPixelsSeen[ptDown.X, ptDown.Y] = true;
                        }
                    }

                    if (pt.X > 1 && pt.Y > 1)
                    {
                        Point ptUpLeft = new Point(pt.X - 1, pt.Y - 1);
                        if (!bolPixelsSeen[ptUpLeft.X, ptUpLeft.Y])
                        {
                            lstQ.Add(ptUpLeft);
                            bolPixelsSeen[ptUpLeft.X, ptUpLeft.Y] = true;
                        }
                    }

                    if (pt.X < bmp.Width - 1 && pt.Y > 1)
                    {
                        Point ptUpRight = new Point(pt.X + 1, pt.Y - 1);
                        if (!bolPixelsSeen[ptUpRight.X, ptUpRight.Y])
                        {
                            lstQ.Add(ptUpRight);
                            bolPixelsSeen[ptUpRight.X, ptUpRight.Y] = true;
                        }
                    }

                    if (pt.X < bmp.Width - 1 && pt.Y < bmp.Height - 1)
                    {
                        Point ptDownRight = new Point(pt.X + 1, pt.Y + 1);
                        if (!bolPixelsSeen[ptDownRight.X, ptDownRight.Y])
                        {
                            lstQ.Add(ptDownRight);
                            bolPixelsSeen[ptDownRight.X, ptDownRight.Y] = true;
                        }
                    }

                    if (pt.X > 0 && pt.Y < bmp.Height - 1)
                    {
                        Point ptDownLeft = new Point(pt.X - 1, pt.Y + 1);
                        if (!bolPixelsSeen[ptDownLeft.X, ptDownLeft.Y])
                        {
                            lstQ.Add(ptDownLeft);
                            bolPixelsSeen[ptDownLeft.X, ptDownLeft.Y] = true;
                        }
                    }
                }

            }
            return lstRetVal;
        }
    }
    public class classPointTree
    {
        classPointTreeElement cRoot = null;
        public void Insert(PointF ptf, ref object data)
        {
            if (cRoot == null)
            {
                cRoot = new classPointTreeElement(ptf, ref data);
                return;
            }
            classPointTreeElement cThis = cRoot;
            string strNew = getPointString(ptf);
            do
            {
                string strThis = getPointString(cThis.ptf);
                int intCompareResult = string.Compare(strNew, strThis);
                if (intCompareResult  > 0)
                {
                    if (cThis.right != null)
                        cThis = cThis.right;
                    else
                    {
                        cThis.right = new classPointTreeElement(ptf, ref data);
                        return;
                    }
                }
                else if (intCompareResult < 0)
                {
                    if (cThis.left != null)
                        cThis = cThis.left;
                    else
                    {
                        cThis.left = new classPointTreeElement(ptf, ref data);
                        return;
                    }
                }
                else
                {
                    cThis.lstData.Add(data);
                    return;
                }
            } while (true);
        }
        string getPointString(PointF ptf) { return ptf.X.ToString() + "." + ptf.Y.ToString(); }

        public List<object> Search(Point pt)
        {
            PointF ptf =new PointF( pt.X, pt.Y);
            return Search(ptf);
        }
        public List<object> Search(PointF ptf)
        {
            if (cRoot == null) return new List<object>();

            classPointTreeElement cThis = cRoot;
            string strSearch = getPointString(ptf);
            do
            {
                string strThis = getPointString(cThis.ptf);
                int intCompareResult = string.Compare(strSearch, strThis);
                if (intCompareResult > 0)
                {
                    if (cThis.right != null)
                        cThis = cThis.right;
                    else
                        return new List<object>();
                }
                else if (intCompareResult < 0)
                {
                    if (cThis.left != null)
                        cThis = cThis.left;
                    else
                        return new List<object>();
                }
                else
                    return cThis.lstData;

            } while (true);
        }

        public class classPointTreeElement
        {
            public PointF ptf;
            public classPointTreeElement left = null;
            public classPointTreeElement right = null;
            public classPointTreeElement YTreeRoot = null;
            public List<object>  lstData = new List<object>();

            public classPointTreeElement(PointF _ptf, ref object obj)
            {
                ptf = _ptf;
                lstData.Add(obj);
            }
        }
    }

    public partial class formGet_Double : Form
    {
        /*
           sample call :
           
           void main()
            {
                string strHeading = "Edit Size " + cLandmark_ToEdit.eType.ToString();
                if (frmGetDouble == null)
                    frmGetDouble = new Math3.formGet_Double(_eventhandler_Landmark_EditSize, strHeading);
                else
                    frmGetDouble.Text = strHeading;
                frmGetDouble.Value = cLandmark_ToEdit.dblDrawSizeFactor;
                frmGetDouble.TopMost = true;
                frmGetDouble.Location = new Point(e.X + 15, e.Y - 15 - frmGetDouble.Height);
                while (frmGetDouble.Right > picMap.Width)
                    frmGetDouble.Left -= 5;
                while (frmGetDouble.Top < 0)
                    frmGetDouble.Top += 5;
                frmGetDouble.Show();
            }
        
            public void _eventhandler_Landmark_EditSize(object sender, EventArgs e)
            {
                if (cLandmark_ToEdit != null)
                {
                    Math3.formGet_Double frmSender = (Math3.formGet_Double)sender;
                    cLandmark_ToEdit.dblDrawSizeFactor = frmSender.Value;
                }
            }
         */

        Label lblValue = new Label();
        TextBox txtInput = new TextBox();
        Button btnOk = new Button();
        Button btnCancel = new Button();
        Button btnRnd = new Button();

        classMinMaxDouble cMinMax = new classMinMaxDouble(0, 10);
        public double Min
        {
            get { return cMinMax.Min; }
            set
            {
                cMinMax.Min = value;
            }
        }

        public double Max
        {
            get { return cMinMax.Max; }
            set
            {
                cMinMax.Max = value;
            }
        }
     
        GroupBox grbMinMaxEditor = new GroupBox();
        TextBox txtMinMax_Editor_Min = new TextBox();
        TextBox txtMinMax_Editor_Max = new TextBox();
        Label lblMinMax_Editor_Min = new Label();
        Label lblMinMax_Editor_Max = new Label();
        Button btnEditor_Ok = new Button();

        EventHandler eventHandler_ValueEntered = null;
        public formGet_Double(EventHandler _eventHandler_ValueEntered,
                              string strHeading)
        {
            eventHandler_ValueEntered = _eventHandler_ValueEntered;
            Text = strHeading;
            TopMost = true;

            Controls.Add(btnOk);
            Controls.Add(btnCancel);
            Controls.Add(lblValue);
            Controls.Add(txtInput);
            Controls.Add(btnRnd);
            Controls.Add(grbMinMaxEditor);

            buildMinMaxEditor();

            btnOk.AutoSize
                = btnCancel.AutoSize
                = btnRnd.AutoSize
                = true;
            btnOk.AutoSizeMode
                = btnCancel.AutoSizeMode
                = btnRnd.AutoSizeMode
                = AutoSizeMode.GrowAndShrink;

            btnRnd.Tag = (object)cMinMax;

            btnOk.Text = "Ok";
            btnCancel.Text = "Cancel";
            btnRnd.Text
                = "rnd";

            lblValue.Text = "X:";

            lblValue.AutoSize = true;

            Size = new Size(250, 105);
            Size recFrame = new Size(20, 45);

            btnOk.Location = new Point(Width - btnOk.Width - recFrame.Width, Height - btnOk.Height - recFrame.Height);
            btnCancel.Location = new Point(btnOk.Left - btnCancel.Width - 5, btnOk.Top);

            txtInput.Width
                = Width - lblValue.Width - btnRnd.Width - recFrame.Width - 15; ;
            txtInput.Text = "0";

            lblValue.Location = new Point(5, 5);// recFrame.Height);
            txtInput.Location = new Point(lblValue.Right + 5, lblValue.Top);
            btnRnd.Location = new Point(txtInput.Right, txtInput.Top);

            WindowState = FormWindowState.Normal;
            StartPosition = FormStartPosition.CenterParent;

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;
            txtInput.KeyDown += TxtInput_KeyDown;
            VisibleChanged += FormGet_PointF_VisibleChanged;
            btnRnd.MouseDown += BtnRnd_MouseDown;
            Show();
        }

        private void BtnRnd_MouseDown(object sender, MouseEventArgs e)
        {
            Button btnSender = (Button)sender;
            classMinMaxDouble cMM = (classMinMaxDouble)btnSender.Tag;
            if (e.Button == MouseButtons.Left)
            {
                    double dblX = classRND.Get_Double(cMinMax);
                    txtInput.Text = dblX.ToString();
            }
            else if (e.Button == MouseButtons.Right)
            {
                grbMinMaxEditor.Tag = (object)cMM;
                txtMinMax_Editor_Min.Text = cMM.Min.ToString();
                txtMinMax_Editor_Max.Text = cMM.Max.ToString();
                grbMinMaxEditor.Text = "Edit MinMax Rnd - "
                                        + (cMM == cMinMax ? "X" : "Y");
                grbMinMaxEditor.Location = new Point(10, 10);
                grbMinMaxEditor.Show();
            }
        }

        void buildMinMaxEditor()
        {
            grbMinMaxEditor.Controls.Add(txtMinMax_Editor_Min);
            grbMinMaxEditor.Controls.Add(txtMinMax_Editor_Max);
            grbMinMaxEditor.Controls.Add(lblMinMax_Editor_Min);
            grbMinMaxEditor.Controls.Add(lblMinMax_Editor_Max);
            grbMinMaxEditor.Controls.Add(btnEditor_Ok);

            grbMinMaxEditor.Text = "Random MinMax Editor";
            lblMinMax_Editor_Min.AutoSize
                = lblMinMax_Editor_Max.AutoSize
                = btnEditor_Ok.AutoSize
                = true;
            btnEditor_Ok.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            lblMinMax_Editor_Min.Text = "min:";
            lblMinMax_Editor_Max.Text = "max:";
            btnEditor_Ok.Text = "ok";

            txtMinMax_Editor_Max.Size
                = txtMinMax_Editor_Min.Size
                = new Size(120, 25);

            grbMinMaxEditor.Location = new Point();
            grbMinMaxEditor.BringToFront();

            lblMinMax_Editor_Min.Location = new Point(5, 15);
            lblMinMax_Editor_Max.Location = new Point(lblMinMax_Editor_Min.Left, lblMinMax_Editor_Min.Bottom + 5);

            txtMinMax_Editor_Min.Location = new Point(lblMinMax_Editor_Max.Right, lblMinMax_Editor_Min.Top);
            txtMinMax_Editor_Max.Location = new Point(lblMinMax_Editor_Max.Right, lblMinMax_Editor_Max.Top);

            btnEditor_Ok.Location = new Point(txtMinMax_Editor_Max.Right + 5, txtMinMax_Editor_Max.Top);

            grbMinMaxEditor.Size = new Size(btnEditor_Ok.Right + 5, btnEditor_Ok.Bottom + 5);

            txtMinMax_Editor_Min.KeyDown += TxtInput_KeyDown;
            txtMinMax_Editor_Max.KeyDown += TxtInput_KeyDown;
            btnEditor_Ok.Click += BtnEditor_Ok_Click;
            grbMinMaxEditor.VisibleChanged += GrbMinMaxEditor_VisibleChanged;
            grbMinMaxEditor.Hide();
        }

        private void GrbMinMaxEditor_VisibleChanged(object sender, EventArgs e)
        {
            txtInput.Visible
                = btnRnd.Visible
                = btnOk.Visible
                = btnCancel.Visible
                = lblValue.Visible
                = !grbMinMaxEditor.Visible;
        }

        private void BtnEditor_Ok_Click(object sender, EventArgs e)
        {
            classMinMaxDouble cMM = (classMinMaxDouble)grbMinMaxEditor.Tag;
            try
            {
                double dbl1 = Convert.ToDouble(txtMinMax_Editor_Min.Text);
                double dbl2 = Convert.ToDouble(txtMinMax_Editor_Max.Text);
                cMM.Min = dbl1 < dbl2 ? dbl1 : dbl2;
                cMM.Max = dbl1 > dbl2 ? dbl1 : dbl2;
            }
            catch (Exception )
            {
            }
            grbMinMaxEditor.Hide();
        }

        private void FormGet_PointF_VisibleChanged(object sender, EventArgs e)
        {
            txtInput.Focus();
        }

        public double Value
        {
            get
            {
                double dblValue = 0;
                try
                {
                    dblValue = Convert.ToDouble(txtInput.Text);
                    return dblValue;
                }
                catch (Exception )
                {
                    return 0;
                }
            }
            set
            {
                txtInput.Text = value.ToString();
            }
        }
        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            string strAllowedChar = "0123456789";
            TextBox txtSender = (TextBox)sender;
            switch (e.KeyCode)
            {
                case Keys.Back:
                case Keys.Left:
                case Keys.Delete:
                case Keys.Insert:
                case Keys.Home:
                case Keys.End:
                case Keys.OemMinus:
                case Keys.Decimal:
                    break;

                default:
                    char chrPressed = (char)e.KeyCode;
                    if (!strAllowedChar.Contains(chrPressed))
                        e.SuppressKeyPress = true;
                    break;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            Hide();
            object objSender = (object)this;
            eventHandler_ValueEntered(objSender, new EventArgs());
        }
    }
    
    public partial class formGet_PointF : Form
    {
        /*
           sample call :     

            formGet_String(_eventHandler_Get_PointF, "Velocity :");

             public void _eventHandler_Get_PointF(object sender, EventArgs e)
             {
                 formGet_String frmSender = (formGet_String)sender;
                 PointF ptf = frmSender.ptf;
                  .. do work ..
             }
         */
        Label lblX = new Label();
        Label lblY = new Label();
        TextBox txtInput_X = new TextBox();
        TextBox txtInput_Y = new TextBox();
        Button btnOk = new Button();
        Button btnCancel = new Button();
        Button btnRnd_X = new Button();
        Button btnRnd_Y = new Button();

        classMinMaxDouble cMinMax_X = new classMinMaxDouble(0, 10);
        public double X_Min
        {
            get { return cMinMax_X.Min; }
            set
            {
                cMinMax_X.Min = value;
            }
        }

        public double X_Max
        {
            get { return cMinMax_X.Max; }
            set
            {
                cMinMax_X.Max = value;
            }
        }

        classMinMaxDouble cMinMax_Y = new classMinMaxDouble(0, 10);
        public double Y_Min
        {
            get { return cMinMax_Y.Min; }
            set
            {
                cMinMax_Y.Min = value;
            }
        }

        public double Y_Max
        {
            get { return cMinMax_Y.Max; }
            set
            {
                cMinMax_Y.Max = value;
            }
        }

        GroupBox grbMinMaxEditor = new GroupBox();
        TextBox txtMinMax_Editor_Min = new TextBox();
        TextBox txtMinMax_Editor_Max = new TextBox();
        Label lblMinMax_Editor_Min = new Label();
        Label lblMinMax_Editor_Max = new Label();
        Button btnEditor_Ok = new Button();

        EventHandler eventHandler_ValueEntered = null;
        public formGet_PointF(EventHandler _eventHandler_ValueEntered,
                              string strHeading)
        {
            eventHandler_ValueEntered = _eventHandler_ValueEntered;
            Text = strHeading;
            TopMost = true;

            Controls.Add(btnOk);
            Controls.Add(btnCancel);
            Controls.Add(lblX);
            Controls.Add(lblY);
            Controls.Add(txtInput_X);
            Controls.Add(txtInput_Y);
            Controls.Add(btnRnd_X);
            Controls.Add(btnRnd_Y);
            Controls.Add(grbMinMaxEditor);

            buildMinMaxEditor();

            btnOk.AutoSize
                = btnCancel.AutoSize
                = btnRnd_X.AutoSize
                = btnRnd_Y.AutoSize
                = true;
            btnOk.AutoSizeMode
                = btnCancel.AutoSizeMode
                = btnRnd_X.AutoSizeMode
                = btnRnd_Y.AutoSizeMode
                = AutoSizeMode.GrowAndShrink;

            btnRnd_X.Tag = (object)cMinMax_X;
            btnRnd_Y.Tag = (object)cMinMax_Y;

            btnOk.Text = "Ok";
            btnCancel.Text = "Cancel";
            btnRnd_X.Text
                = btnRnd_Y.Text
                = "rnd";

            lblX.Text = "X:";
            lblY.Text = "Y:";

            lblY.AutoSize
                = lblX.AutoSize = true;

            Size = new Size(250, 125);
            Size recFrame = new Size(20, 45);

            btnOk.Location = new Point(Width - btnOk.Width - recFrame.Width, Height - btnOk.Height - recFrame.Height);
            btnCancel.Location = new Point(btnOk.Left - btnCancel.Width - 5, btnOk.Top);

            txtInput_X.Width
                = txtInput_Y.Width
                = Width - lblX.Width - btnRnd_X.Width - recFrame.Width - 15; ;
            txtInput_X.Text = "0";
            txtInput_Y.Text = "0";

            lblX.Location = new Point(5, 5);// recFrame.Height);
            txtInput_X.Location = new Point(lblX.Right + 5, lblX.Top);
            btnRnd_X.Location = new Point(txtInput_X.Right, txtInput_X.Top);

            lblY.Location = new Point(lblX.Left, txtInput_X.Bottom + 5);
            txtInput_Y.Location = new Point(lblY.Right + 5, lblY.Top);
            btnRnd_Y.Location = new Point(txtInput_Y.Right, txtInput_Y.Top);

            WindowState = FormWindowState.Normal;
            StartPosition = FormStartPosition.CenterParent;

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;
            txtInput_X.KeyDown += TxtInput_KeyDown;
            txtInput_Y.KeyDown += TxtInput_KeyDown;
            VisibleChanged += FormGet_PointF_VisibleChanged;
            btnRnd_X.MouseDown += BtnRnd_MouseDown;
            btnRnd_Y.MouseDown += BtnRnd_MouseDown;
            Show();
        }

        private void BtnRnd_MouseDown(object sender, MouseEventArgs e)
        {
            Button btnSender = (Button)sender;
            classMinMaxDouble cMM = (classMinMaxDouble)btnSender.Tag;
            if (e.Button == MouseButtons.Left)
            {
                if (cMM == cMinMax_X)
                {
                    double dblX = classRND.Get_Double(cMinMax_X);
                    txtInput_X.Text = dblX.ToString();
                }
                else
                {
                    double dblY = classRND.Get_Double(cMinMax_Y);
                    txtInput_Y.Text = dblY.ToString();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                grbMinMaxEditor.Tag = (object)cMM;
                txtMinMax_Editor_Min.Text = cMM.Min.ToString();
                txtMinMax_Editor_Max.Text = cMM.Max.ToString();
                grbMinMaxEditor.Text = "Edit MinMax Rnd - "
                                        + (cMM == cMinMax_X ? "X" : "Y");
                grbMinMaxEditor.Location = new Point(10, 10);
                grbMinMaxEditor.Show();
            }
        }

        void buildMinMaxEditor()
        {
            grbMinMaxEditor.Controls.Add(txtMinMax_Editor_Min);
            grbMinMaxEditor.Controls.Add(txtMinMax_Editor_Max);
            grbMinMaxEditor.Controls.Add(lblMinMax_Editor_Min);
            grbMinMaxEditor.Controls.Add(lblMinMax_Editor_Max);
            grbMinMaxEditor.Controls.Add(btnEditor_Ok);

            grbMinMaxEditor.Text = "Random MinMax Editor";
            lblMinMax_Editor_Min.AutoSize
                = lblMinMax_Editor_Max.AutoSize
                = btnEditor_Ok.AutoSize
                = true;
            btnEditor_Ok.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            lblMinMax_Editor_Min.Text = "min:";
            lblMinMax_Editor_Max.Text = "max:";
            btnEditor_Ok.Text = "ok";

            txtMinMax_Editor_Max.Size
                = txtMinMax_Editor_Min.Size
                = new Size(120, 25);

            grbMinMaxEditor.Location = new Point();
            grbMinMaxEditor.BringToFront();

            lblMinMax_Editor_Min.Location = new Point(5, 15);
            lblMinMax_Editor_Max.Location = new Point(lblMinMax_Editor_Min.Left, lblMinMax_Editor_Min.Bottom + 5);

            txtMinMax_Editor_Min.Location = new Point(lblMinMax_Editor_Max.Right, lblMinMax_Editor_Min.Top);
            txtMinMax_Editor_Max.Location = new Point(lblMinMax_Editor_Max.Right, lblMinMax_Editor_Max.Top);

            btnEditor_Ok.Location = new Point(txtMinMax_Editor_Max.Right + 5, txtMinMax_Editor_Max.Top);

            grbMinMaxEditor.Size = new Size(btnEditor_Ok.Right + 5, btnEditor_Ok.Bottom + 5);

            txtMinMax_Editor_Min.KeyDown += TxtInput_KeyDown;
            txtMinMax_Editor_Max.KeyDown += TxtInput_KeyDown;
            btnEditor_Ok.Click += BtnEditor_Ok_Click;
            grbMinMaxEditor.VisibleChanged += GrbMinMaxEditor_VisibleChanged;
            grbMinMaxEditor.Hide();
        }

        private void GrbMinMaxEditor_VisibleChanged(object sender, EventArgs e)
        {
            txtInput_X.Visible
                = txtInput_Y.Visible
                = btnRnd_X.Visible
                = btnRnd_Y.Visible
                = btnOk.Visible
                = btnCancel.Visible
                = lblX.Visible
                = lblY.Visible
                = !grbMinMaxEditor.Visible;
        }

        private void BtnEditor_Ok_Click(object sender, EventArgs e)
        {
            classMinMaxDouble cMM = (classMinMaxDouble)grbMinMaxEditor.Tag;
            try
            {
                double dbl1 = Convert.ToDouble(txtMinMax_Editor_Min.Text);
                double dbl2 = Convert.ToDouble(txtMinMax_Editor_Max.Text);
                cMM.Min = dbl1 < dbl2 ? dbl1 : dbl2;
                cMM.Max = dbl1 > dbl2 ? dbl1 : dbl2;
            }
            catch (Exception )
            {
            }
            grbMinMaxEditor.Hide();
        }

        private void FormGet_PointF_VisibleChanged(object sender, EventArgs e)
        {
            txtInput_X.Focus();
        }

        public PointF ptf
        {
            get
            {
                double dbl_X = 0, dbl_Y = 0;
                try
                {
                    dbl_X = Convert.ToDouble(txtInput_X.Text);
                    dbl_Y = Convert.ToDouble(txtInput_Y.Text);
                    return new PointF((float)dbl_X, (float)dbl_Y);
                }
                catch (Exception )
                {
                    return new PointF();
                }
            }
            set
            {
                txtInput_X.Text = value.X.ToString();
                txtInput_Y.Text = value.Y.ToString();
            }
        }
        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            string strAllowedChar = "0123456789";
            TextBox txtSender = (TextBox)sender;
            switch (e.KeyCode)
            {
                case Keys.Back:
                case Keys.Left:
                case Keys.Delete:
                case Keys.Insert:
                case Keys.Home:
                case Keys.End:
                case Keys.OemMinus:
                case Keys.Decimal:
                    break;

                default:
                    char chrPressed = (char)e.KeyCode;
                    if (!strAllowedChar.Contains(chrPressed))
                        e.SuppressKeyPress = true;
                    break;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            Hide();
            object objSender = (object)this;
            eventHandler_ValueEntered(objSender, new EventArgs());
        }
    }  
    public partial class formGet_SizeF : Form
    {
        /*
           sample call :     

            formGet_String(_eventHandler_Get_SizeF, "Size :");

             public void _eventHandler_Get_SizeF(object sender, EventArgs e)
             {
                 formGet_String frmSender = (formGet_String)sender;
                 SizeF ptf = frmSender.ptf;
                  .. do work ..
             }
         */
        Label lblWidth = new Label();
        Label lblHeight = new Label();
        TextBox txtInput_Width = new TextBox();
        TextBox txtInput_Height = new TextBox();
        Button btnOk = new Button();
        Button btnCancel = new Button();
        Button btnRnd_Width = new Button();
        Button btnRnd_Height = new Button();

        classMinMaxDouble cMinMaWidth_Width = new classMinMaxDouble(1, 100);
        public double Width_Min
        {
            get { return cMinMaWidth_Width.Min; }
            set
            {
                cMinMaWidth_Width.Min = value;
            }
        }

        public double Width_Max
        {
            get { return cMinMaWidth_Width.Max; }
            set
            {
                cMinMaWidth_Width.Max = value;
            }
        }

        classMinMaxDouble cMinMaWidth_Height = new classMinMaxDouble(0, 10);
        public double Height_Min
        {
            get { return cMinMaWidth_Height.Min; }
            set
            {
                cMinMaWidth_Height.Min = value;
            }
        }

        public double Height_Max
        {
            get { return cMinMaWidth_Height.Max; }
            set
            {
                cMinMaWidth_Height.Max = value;
            }
        }

        GroupBox grbMinMaxEditor = new GroupBox();
        TextBox txtMinMaWidth_Editor_Min = new TextBox();
        TextBox txtMinMaWidth_Editor_Max = new TextBox();
        Label lblMinMaWidth_Editor_Min = new Label();
        Label lblMinMaWidth_Editor_Max = new Label();
        Button btnEditor_Ok = new Button();

        EventHandler eventHandler_ValueEntered = null;
        public formGet_SizeF(EventHandler _eventHandler_ValueEntered,
                              string strHeading)
        {
            eventHandler_ValueEntered = _eventHandler_ValueEntered;
            Text = strHeading;
            TopMost = true;

            Controls.Add(btnOk);
            Controls.Add(btnCancel);
            Controls.Add(lblWidth);
            Controls.Add(lblHeight);
            Controls.Add(txtInput_Width);
            Controls.Add(txtInput_Height);
            Controls.Add(btnRnd_Width);
            Controls.Add(btnRnd_Height);
            Controls.Add(grbMinMaxEditor);

            buildMinMaxEditor();

            btnOk.AutoSize
                = btnCancel.AutoSize
                = btnRnd_Width.AutoSize
                = btnRnd_Height.AutoSize
                = true;
            btnOk.AutoSizeMode
                = btnCancel.AutoSizeMode
                = btnRnd_Width.AutoSizeMode
                = btnRnd_Height.AutoSizeMode
                = AutoSizeMode.GrowAndShrink;

            btnRnd_Width.Tag = (object)cMinMaWidth_Width;
            btnRnd_Height.Tag = (object)cMinMaWidth_Height;

            btnOk.Text = "Ok";
            btnCancel.Text = "Cancel";
            btnRnd_Width.Text
                = btnRnd_Height.Text
                = "rnd";

            lblWidth.Text = "X:";
            lblHeight.Text = "Y:";

            lblHeight.AutoSize
                = lblWidth.AutoSize = true;

            Size = new Size(250, 125);
            Size recFrame = new Size(20, 45);

            btnOk.Location = new Point(Width - btnOk.Width - recFrame.Width, Height - btnOk.Height - recFrame.Height);
            btnCancel.Location = new Point(btnOk.Left - btnCancel.Width - 5, btnOk.Top);

            txtInput_Width.Width
                = txtInput_Height.Width
                = Width - lblWidth.Width - btnRnd_Width.Width - recFrame.Width - 15; ;
            txtInput_Width.Text = "0";
            txtInput_Height.Text = "0";

            lblWidth.Location = new Point(5, 5);// recFrame.Height);
            txtInput_Width.Location = new Point(lblWidth.Right + 5, lblWidth.Top);
            btnRnd_Width.Location = new Point(txtInput_Width.Right, txtInput_Width.Top);

            lblHeight.Location = new Point(lblWidth.Left, txtInput_Width.Bottom + 5);
            txtInput_Height.Location = new Point(lblHeight.Right + 5, lblHeight.Top);
            btnRnd_Height.Location = new Point(txtInput_Height.Right, txtInput_Height.Top);

            WindowState = FormWindowState.Normal;
            StartPosition = FormStartPosition.CenterParent;

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;
            txtInput_Width.KeyDown += TxtInput_KeyDown;
            txtInput_Height.KeyDown += TxtInput_KeyDown;
            VisibleChanged += FormGet_SizeF_VisibleChanged;
            btnRnd_Width.MouseDown += BtnRnd_MouseDown;
            btnRnd_Height.MouseDown += BtnRnd_MouseDown;
        }

        private void BtnRnd_MouseDown(object sender, MouseEventArgs e)
        {
            Button btnSender = (Button)sender;
            classMinMaxDouble cMM = (classMinMaxDouble)btnSender.Tag;
            if (e.Button == MouseButtons.Left)
            {
                if (cMM == cMinMaWidth_Width)
                {
                    double dblX = classRND.Get_Double(cMinMaWidth_Width);
                    txtInput_Width.Text = dblX.ToString();
                }
                else
                {
                    double dblY = classRND.Get_Double(cMinMaWidth_Height);
                    txtInput_Height.Text = dblY.ToString();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                grbMinMaxEditor.Tag = (object)cMM;
                txtMinMaWidth_Editor_Min.Text = cMM.Min.ToString();
                txtMinMaWidth_Editor_Max.Text = cMM.Max.ToString();
                grbMinMaxEditor.Text = "Edit MinMax Rnd - "
                                        + (cMM == cMinMaWidth_Width ? "X" : "Y");
                grbMinMaxEditor.Location = new Point(10, 10);
                grbMinMaxEditor.Show();
            }
        }

        void buildMinMaxEditor()
        {
            grbMinMaxEditor.Controls.Add(txtMinMaWidth_Editor_Min);
            grbMinMaxEditor.Controls.Add(txtMinMaWidth_Editor_Max);
            grbMinMaxEditor.Controls.Add(lblMinMaWidth_Editor_Min);
            grbMinMaxEditor.Controls.Add(lblMinMaWidth_Editor_Max);
            grbMinMaxEditor.Controls.Add(btnEditor_Ok);

            grbMinMaxEditor.Text = "Random MinMax Editor";
            lblMinMaWidth_Editor_Min.AutoSize
                = lblMinMaWidth_Editor_Max.AutoSize
                = btnEditor_Ok.AutoSize
                = true;
            btnEditor_Ok.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            lblMinMaWidth_Editor_Min.Text = "min:";
            lblMinMaWidth_Editor_Max.Text = "max:";
            btnEditor_Ok.Text = "ok";

            txtMinMaWidth_Editor_Max.Size
                = txtMinMaWidth_Editor_Min.Size
                = new Size(120, 25);

            grbMinMaxEditor.Location = new Point();
            grbMinMaxEditor.BringToFront();

            lblMinMaWidth_Editor_Min.Location = new Point(5, 15);
            lblMinMaWidth_Editor_Max.Location = new Point(lblMinMaWidth_Editor_Min.Left, lblMinMaWidth_Editor_Min.Bottom + 5);

            txtMinMaWidth_Editor_Min.Location = new Point(lblMinMaWidth_Editor_Max.Right, lblMinMaWidth_Editor_Min.Top);
            txtMinMaWidth_Editor_Max.Location = new Point(lblMinMaWidth_Editor_Max.Right, lblMinMaWidth_Editor_Max.Top);

            btnEditor_Ok.Location = new Point(txtMinMaWidth_Editor_Max.Right + 5, txtMinMaWidth_Editor_Max.Top);

            grbMinMaxEditor.Size = new Size(btnEditor_Ok.Right + 5, btnEditor_Ok.Bottom + 5);

            txtMinMaWidth_Editor_Min.KeyDown += TxtInput_KeyDown;
            txtMinMaWidth_Editor_Max.KeyDown += TxtInput_KeyDown;
            btnEditor_Ok.Click += BtnEditor_Ok_Click;
            grbMinMaxEditor.VisibleChanged += GrbMinMaxEditor_VisibleChanged;
            grbMinMaxEditor.Hide();
        }

        private void GrbMinMaxEditor_VisibleChanged(object sender, EventArgs e)
        {
            txtInput_Width.Visible
                = txtInput_Height.Visible
                = btnRnd_Width.Visible
                = btnRnd_Height.Visible
                = btnOk.Visible
                = btnCancel.Visible
                = lblWidth.Visible
                = lblHeight.Visible
                = !grbMinMaxEditor.Visible;
        }

        private void BtnEditor_Ok_Click(object sender, EventArgs e)
        {
            classMinMaxDouble cMM = (classMinMaxDouble)grbMinMaxEditor.Tag;
            try
            {
                double dbl1 = Convert.ToDouble(txtMinMaWidth_Editor_Min.Text);
                double dbl2 = Convert.ToDouble(txtMinMaWidth_Editor_Max.Text);
                cMM.Min = dbl1 < dbl2 ? dbl1 : dbl2;
                cMM.Max = dbl1 > dbl2 ? dbl1 : dbl2;
            }
            catch (Exception )
            {
            }
            grbMinMaxEditor.Hide();
        }

        private void FormGet_SizeF_VisibleChanged(object sender, EventArgs e)
        {
            txtInput_Width.Focus();
        }

        public SizeF sizeF
        {
            get
            {
                double dbl_Width = 0, dbl_Height = 0;
                try
                {
                    dbl_Width = Convert.ToDouble(txtInput_Width.Text);
                    dbl_Height = Convert.ToDouble(txtInput_Height.Text);
                    return new SizeF((float)dbl_Width, (float)dbl_Height);
                }
                catch (Exception )
                {
                    return new SizeF();
                }
            }
            set
            {
                txtInput_Width.Text = value.Width.ToString();
                txtInput_Height.Text = value.Height.ToString();
            }
        }
        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            string strAllowedChar = "0123456789";
            TextBox txtSender = (TextBox)sender;
            switch (e.KeyCode)
            {
                case Keys.Back:
                case Keys.Left:
                case Keys.Delete:
                case Keys.Insert:
                case Keys.Home:
                case Keys.End:
                case Keys.OemMinus:
                case Keys.Decimal:
                    break;

                default:
                    char chrPressed = (char)e.KeyCode;
                    if (!strAllowedChar.Contains(chrPressed))
                        e.SuppressKeyPress = true;
                    break;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            Hide();
            object objSender = (object)this;
            eventHandler_ValueEntered(objSender, new EventArgs());
        }
    }
    public partial class formGet_Size : Form
    {
        /*
           sample call :     

            formGet_String(_eventHandler_Get_Size, "Size :");

             public void _eventHandler_Get_Size(object sender, EventArgs e)
             {
                 formGet_String frmSender = (formGet_String)sender;
                 Size ptf = frmSender.ptf;
                  .. do work ..
             }
         */
        Label lblWidth = new Label();
        Label lblHeight = new Label();
        TextBox txtInput_Width = new TextBox();
        TextBox txtInput_Height = new TextBox();
        Button btnOk = new Button();
        Button btnCancel = new Button();
        Button btnRnd_Width = new Button();
        Button btnRnd_Height = new Button();

        classMinMaxDouble cMinMaWidth_Width = new classMinMaxDouble(1, 100);
        public double Width_Min
        {
            get { return cMinMaWidth_Width.Min; }
            set
            {
                cMinMaWidth_Width.Min = value;
            }
        }

        public double Width_Max
        {
            get { return cMinMaWidth_Width.Max; }
            set
            {
                cMinMaWidth_Width.Max = value;
            }
        }

        classMinMaxDouble cMinMaWidth_Height = new classMinMaxDouble(0, 10);
        public double Height_Min
        {
            get { return cMinMaWidth_Height.Min; }
            set
            {
                cMinMaWidth_Height.Min = value;
            }
        }

        public double Height_Max
        {
            get { return cMinMaWidth_Height.Max; }
            set
            {
                cMinMaWidth_Height.Max = value;
            }
        }

        GroupBox grbMinMaxEditor = new GroupBox();
        TextBox txtMinMaWidth_Editor_Min = new TextBox();
        TextBox txtMinMaWidth_Editor_Max = new TextBox();
        Label lblMinMaWidth_Editor_Min = new Label();
        Label lblMinMaWidth_Editor_Max = new Label();
        Button btnEditor_Ok = new Button();

        EventHandler eventHandler_ValueEntered = null;
        public formGet_Size(EventHandler _eventHandler_ValueEntered,
                              string strHeading)
        {
            eventHandler_ValueEntered = _eventHandler_ValueEntered;
            Text = strHeading;
            TopMost = true;

            Controls.Add(btnOk);
            Controls.Add(btnCancel);
            Controls.Add(lblWidth);
            Controls.Add(lblHeight);
            Controls.Add(txtInput_Width);
            Controls.Add(txtInput_Height);
            Controls.Add(btnRnd_Width);
            Controls.Add(btnRnd_Height);
            Controls.Add(grbMinMaxEditor);

            buildMinMaxEditor();

            btnOk.AutoSize
                = btnCancel.AutoSize
                = btnRnd_Width.AutoSize
                = btnRnd_Height.AutoSize
                = true;
            btnOk.AutoSizeMode
                = btnCancel.AutoSizeMode
                = btnRnd_Width.AutoSizeMode
                = btnRnd_Height.AutoSizeMode
                = AutoSizeMode.GrowAndShrink;


            btnRnd_Width.Tag = (object)cMinMaWidth_Width;
            btnRnd_Height.Tag = (object)cMinMaWidth_Height;

            btnOk.Text = "Ok";
            btnCancel.Text = "Cancel";
            btnRnd_Width.Text
                = btnRnd_Height.Text
                = "rnd";

            lblWidth.Text = "X:";
            lblHeight.Text = "Y:";

            lblHeight.AutoSize
                = lblWidth.AutoSize = true;

            Size = new Size(250, 125);
            Size recFrame = new Size(20, 45);

            btnOk.Location = new Point(Width - btnOk.Width - recFrame.Width, Height - btnOk.Height - recFrame.Height);
            btnCancel.Location = new Point(btnOk.Left - btnCancel.Width - 5, btnOk.Top);

            txtInput_Width.Width
                = txtInput_Height.Width
                = Width - lblWidth.Width - btnRnd_Width.Width - recFrame.Width - 15; ;
            txtInput_Width.Text = "0";
            txtInput_Height.Text = "0";

            lblWidth.Location = new Point(5, 5);// recFrame.Height);
            txtInput_Width.Location = new Point(lblWidth.Right + 5, lblWidth.Top);
            btnRnd_Width.Location = new Point(txtInput_Width.Right, txtInput_Width.Top);

            lblHeight.Location = new Point(lblWidth.Left, txtInput_Width.Bottom + 5);
            txtInput_Height.Location = new Point(lblHeight.Right + 5, lblHeight.Top);
            btnRnd_Height.Location = new Point(txtInput_Height.Right, txtInput_Height.Top);

            WindowState = FormWindowState.Normal;
            StartPosition = FormStartPosition.CenterParent;

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;
            txtInput_Width.KeyDown += TxtInput_KeyDown;
            txtInput_Height.KeyDown += TxtInput_KeyDown;
            txtInput_Height.MouseWheel += TxtInput_MouseWheel;
            txtInput_Width.MouseWheel += TxtInput_MouseWheel;
            txtInput_Height.TextChanged += TxtInput_TextChanged;
            txtInput_Width.TextChanged += TxtInput_TextChanged;

            VisibleChanged += FormGet_Size_VisibleChanged;
            btnRnd_Width.MouseDown += BtnRnd_MouseDown;
            btnRnd_Height.MouseDown += BtnRnd_MouseDown;
        }

        private void TxtInput_TextChanged(object sender, EventArgs e)
        {
            TextBox txtSender = (TextBox)sender;
            try
            {
                int intValue = Convert.ToInt32(txtSender.Text);
                if (intValue < (txtSender == txtInput_Height ? Height_Min : Width_Min))
                    txtSender.Text = (txtSender == txtInput_Height ? Height_Min : Width_Min).ToString();
                if (intValue > (txtSender == txtInput_Height ? Height_Max : Width_Max))
                    txtSender.Text = (txtSender == txtInput_Height ? Height_Max : Width_Max).ToString();
            }
            catch (Exception)
            {
            }
        }

        private void TxtInput_MouseWheel(object sender, MouseEventArgs e)
        {
            TextBox txtSender = (TextBox)sender;
            
            try
            {
                int intValue = Convert.ToInt32(txtSender.Text);
                intValue += (int)(Math.Abs(e.Delta) / e.Delta);
                txtSender.Text = intValue.ToString();
            }
            catch (Exception)
            {
                return;
            }
            

        }

        private void BtnRnd_MouseDown(object sender, MouseEventArgs e)
        {
            Button btnSender = (Button)sender;
            classMinMaxDouble cMM = (classMinMaxDouble)btnSender.Tag;
            if (e.Button == MouseButtons.Left)
            {
                if (cMM == cMinMaWidth_Width)
                {
                    double dblX = classRND.Get_Double(cMinMaWidth_Width);
                    txtInput_Width.Text = dblX.ToString();
                }
                else
                {
                    double dblY = classRND.Get_Double(cMinMaWidth_Height);
                    txtInput_Height.Text = dblY.ToString();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                grbMinMaxEditor.Tag = (object)cMM;
                txtMinMaWidth_Editor_Min.Text = cMM.Min.ToString();
                txtMinMaWidth_Editor_Max.Text = cMM.Max.ToString();
                grbMinMaxEditor.Text = "Edit MinMax Rnd - "
                                        + (cMM == cMinMaWidth_Width ? "X" : "Y");
                grbMinMaxEditor.Location = new Point(10, 10);
                grbMinMaxEditor.Show();
            }
        }

        void buildMinMaxEditor()
        {
            grbMinMaxEditor.Controls.Add(txtMinMaWidth_Editor_Min);
            grbMinMaxEditor.Controls.Add(txtMinMaWidth_Editor_Max);
            grbMinMaxEditor.Controls.Add(lblMinMaWidth_Editor_Min);
            grbMinMaxEditor.Controls.Add(lblMinMaWidth_Editor_Max);
            grbMinMaxEditor.Controls.Add(btnEditor_Ok);

            grbMinMaxEditor.Text = "Random MinMax Editor";
            lblMinMaWidth_Editor_Min.AutoSize
                = lblMinMaWidth_Editor_Max.AutoSize
                = btnEditor_Ok.AutoSize
                = true;
            btnEditor_Ok.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            lblMinMaWidth_Editor_Min.Text = "min:";
            lblMinMaWidth_Editor_Max.Text = "max:";
            btnEditor_Ok.Text = "ok";

            txtMinMaWidth_Editor_Max.Size
                = txtMinMaWidth_Editor_Min.Size
                = new Size(120, 25);

            grbMinMaxEditor.Location = new Point();
            grbMinMaxEditor.BringToFront();

            lblMinMaWidth_Editor_Min.Location = new Point(5, 15);
            lblMinMaWidth_Editor_Max.Location = new Point(lblMinMaWidth_Editor_Min.Left, lblMinMaWidth_Editor_Min.Bottom + 5);

            txtMinMaWidth_Editor_Min.Location = new Point(lblMinMaWidth_Editor_Max.Right, lblMinMaWidth_Editor_Min.Top);
            txtMinMaWidth_Editor_Max.Location = new Point(lblMinMaWidth_Editor_Max.Right, lblMinMaWidth_Editor_Max.Top);

            btnEditor_Ok.Location = new Point(txtMinMaWidth_Editor_Max.Right + 5, txtMinMaWidth_Editor_Max.Top);

            grbMinMaxEditor.Size = new Size(btnEditor_Ok.Right + 5, btnEditor_Ok.Bottom + 5);

            txtMinMaWidth_Editor_Min.KeyDown += TxtInput_KeyDown;
            txtMinMaWidth_Editor_Max.KeyDown += TxtInput_KeyDown;
            btnEditor_Ok.Click += BtnEditor_Ok_Click;
            grbMinMaxEditor.VisibleChanged += GrbMinMaxEditor_VisibleChanged;
            grbMinMaxEditor.Hide();
        }

        private void GrbMinMaxEditor_VisibleChanged(object sender, EventArgs e)
        {
            txtInput_Width.Visible
                = txtInput_Height.Visible
                = btnRnd_Width.Visible
                = btnRnd_Height.Visible
                = btnOk.Visible
                = btnCancel.Visible
                = lblWidth.Visible
                = lblHeight.Visible
                = !grbMinMaxEditor.Visible;
        }

        private void BtnEditor_Ok_Click(object sender, EventArgs e)
        {
            classMinMaxDouble cMM = (classMinMaxDouble)grbMinMaxEditor.Tag;
            try
            {
                double dbl1 = Convert.ToDouble(txtMinMaWidth_Editor_Min.Text);
                double dbl2 = Convert.ToDouble(txtMinMaWidth_Editor_Max.Text);
                cMM.Min = dbl1 < dbl2 ? dbl1 : dbl2;
                cMM.Max = dbl1 > dbl2 ? dbl1 : dbl2;
            }
            catch (Exception )
            {
            }
            grbMinMaxEditor.Hide();
        }

        private void FormGet_Size_VisibleChanged(object sender, EventArgs e)
        {
            txtInput_Width.Focus();
        }

        bool bolRnd = true;
        public bool RND
        { 
        get { return bolRnd; }
        set 
            {
                bolRnd = value;
                btnRnd_Height.Visible
                        = btnRnd_Width.Visible
                        = bolRnd;
            }
        }

        public Size size
        {
            get
            {
                double dbl_Width = 0, dbl_Height = 0;
                try
                {
                    dbl_Width = Convert.ToDouble(txtInput_Width.Text);
                    dbl_Height = Convert.ToDouble(txtInput_Height.Text);
                    return new Size((int)dbl_Width, (int)dbl_Height);
                }
                catch (Exception )
                {
                    return new Size();
                }
            }
            set
            {
                txtInput_Width.Text = value.Width.ToString();
                txtInput_Height.Text = value.Height.ToString();
            }
        }
        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            string strAllowedChar = "0123456789";
            TextBox txtSender = (TextBox)sender;
            switch (e.KeyCode)
            {
                case Keys.Back:
                case Keys.Left:
                case Keys.Delete:
                case Keys.Insert:
                case Keys.Home:
                case Keys.End:
                case Keys.OemMinus:
                case Keys.Decimal:
                    break;

                default:
                    char chrPressed = (char)e.KeyCode;
                    if (!strAllowedChar.Contains(chrPressed))
                        e.SuppressKeyPress = true;
                    break;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            Hide();
            object objSender = (object)this;
            eventHandler_ValueEntered(objSender, new EventArgs());
        }
    }
    public class classMinMax
    {
        public int Max = 100;
        public int Min = 0;
        public classMinMax(int intMin, int intMax)
        {
            Min = intMin;
            Max = intMax;
        }

        public classMinMax Factored(double dblFactor)
        {
            return new classMinMax((int)(Min * dblFactor), (int)(Max * dblFactor));
        }

        public classMinMax Copy()
        {
            classMinMax cRetVal = new classMinMax(Min, Max);
            return cRetVal;
        }
    }

    public class classMinMaxDouble
    {
        public double Max = 100;
        public double Min = 0;
        public classMinMaxDouble(double dblMin, double dblMax)
        {
            Min = dblMin;
            Max = dblMax;
        }
    }

    public class classPointFPair
    {
        public PointF ptf1;
        public PointF ptf2;

        public classPointFPair(ref PointF _ptf1, ref PointF _ptf2)
        {
            ptf1 = _ptf1;
            ptf2 = _ptf2;
        }

        public classPointFPair(PointF _ptf1, PointF _ptf2)
        {
            ptf1 = _ptf1;
            ptf2 = _ptf2;
        }
    }

    public class classPointF3D
    {
        public double X = 0;
        public double Y = 0;
        public double Z = 0;
        public classPointF3D() { }
        public classPointF3D(double _X, double _Y, double _Z)
        {
            X = _X;
            Y = _Y;
            Z = _Z;
        }
    }
    public class classRND
    {
        public static Random rnd = new Random();
        public const double twoPi = Math.PI * 2;

        public static double Get_Double()
        {
            return rnd.NextDouble();
        }

        public static double Get_Double(classMinMax cMM)
        {
            double dblRnd = rnd.NextDouble();
            double dblRange = cMM.Max - cMM.Min;
            double dblBase = cMM.Min;
            return dblBase + dblRnd * dblRange;
        }

        public static double Get_Double(classMinMaxDouble cMM)
        {
            double dblRnd = rnd.NextDouble();
            double dblRange = cMM.Max - cMM.Min;
            double dblBase = cMM.Min;
            return dblBase + dblRnd * dblRange;
        }

        public static PointF Get_Point(float fltRange)
        {
            //return new PointF(200, 0);

            return new PointF((float)(Get_Sign() * Get_Double() * fltRange), (float)(Get_Sign() * Get_Double() * fltRange));
        }
        public static PointF Get_PointF(classMinMax cMM_X, classMinMax cMM_Y)
        {
            return new PointF((float)(Get_Sign() * Get_Double() * Get_Int(cMM_X)), (float)(Get_Sign() * Get_Double() * Get_Int(cMM_Y)));
        }
        public static Point Get_Point(classMinMax cMM_X, classMinMax cMM_Y)
        {
            return new Point((int)(Get_Sign() * Get_Double() * Get_Int(cMM_X)), (int)(Get_Sign() * Get_Double() * Get_Int(cMM_Y)));
        }


        public static void Shuffle(ref List<int> lstInt)
        {
            for (int intCounter = 0; intCounter < lstInt.Count * 8; intCounter++)
            {
                int intA = 0, intB = 0;
                while (intA == intB)
                {
                    intA = Get_Int(0, lstInt.Count);
                    intB = Get_Int(0, lstInt.Count);
                }
                int intTemp = lstInt[intA];
                lstInt[intA] = lstInt[intB];
                lstInt[intB] = intTemp;
            }
        }

        public static void Shuffle(ref List<object> lstObj)
        {
            if (lstObj.Count > 3)
                for (int intCounter = 0; intCounter < lstObj.Count * 8; intCounter++)
                {
                    int intA = 0, intB = 0;
                    while (intA == intB)
                    {
                        intA = Get_Int(0, lstObj.Count);
                        intB = Get_Int(0, lstObj.Count);
                    }
                    object objTemp = lstObj[intA];
                    lstObj[intA] = lstObj[intB];
                    lstObj[intB] = objTemp;
                }
            else
            {
                List<object> lstTemp = new List<object>();

                for (int intCounter = lstObj.Count - 1; intCounter >= 0; intCounter--)
                    lstTemp.Add(lstObj[intCounter]);

                lstObj = lstTemp;
            }

        }


        public static int Get_Sign()
        {
            double dblMax = Math.Pow(2, 16);

            int intRnd = (int)(rnd.NextDouble() * dblMax) % 2;


            return intRnd == 0
                           ? -1
                           : 1;
        }

        public static int Get_Int(classMinMax cMM)
        {
            if (cMM == null)
            {
                System.Windows.Forms.MessageBox.Show("error: Get_In(classMinMax cMM) received a null parameter", "null parameter");
                return 0;
            }

            return Get_Int(cMM.Min, cMM.Max);
        }
        public static int Get_Int(int intMin, int intMax)
        {
            if (intMin > intMax) return 0;
            if (intMin == intMax) return intMax;
            int intRange = intMax - intMin;
            int intBase = (int)((rnd.NextDouble() * Math.Pow(2, 32)) % intRange);
            return intMin + intBase;
        }

        public static Color Get_Color()
        {
            int intR = (int)(rnd.NextDouble() * Math.Pow(2, 16) % 256);
            int intG = (int)(rnd.NextDouble() * Math.Pow(2, 16) % 256);
            int intB = (int)(rnd.NextDouble() * Math.Pow(2, 16) % 256);
            return Color.FromArgb(intR, intG, intB);
        }


        public static char Get_Alpha()
        {
            return (char)('A' + rnd.NextDouble() * 26);
        }
    }

    public class classRadialCoordinate
    {
        public classRadialCoordinate() { }
        public classRadialCoordinate(PointF pt1, PointF pt2) { init(pt2.X - pt1.X, pt2.Y - pt1.Y); }
        public classRadialCoordinate(PointF ptf) { init(ptf.X, ptf.Y); }
        public classRadialCoordinate(double radians, double magnitude)
        {
            _magnitude = magnitude;
            _radians = radians;
        }

        private void init(double x, double y)
        {
            _magnitude = Math.Sqrt(x * x + y * y);
            _radians = Math.Atan2(y, x);
        }

        public classRadialCoordinate Copy()
        {
            classRadialCoordinate cRetVal = new classRadialCoordinate(_radians, _magnitude);
            return cRetVal;
        }

        double _radians;
        public double Radians
        {
            get { return _radians; }
            set { _radians = value; }
        }
        public double Degrees
        {
            get 
            {
                double dblRetVal = (_radians * 360.0 / (Math.PI * 2.0) % 360);
                while (dblRetVal < 0)
                    dblRetVal += 360;
                while (dblRetVal > 360)
                    dblRetVal -= 360;
                return dblRetVal;
            }
            set { _radians = value / 360.0 * (2.0 * Math.PI); }
        }

        double _magnitude;
        public double Magnitude
        {
            get { return _magnitude; }
            set { _magnitude = value; }
        }



        public Point toPoint()
        {
            return new Point((int)(_magnitude * Math.Cos(_radians)), (int)(_magnitude * Math.Sin(_radians)));
        }

        public PointF toPointF()
        {
            return new PointF((float)(_magnitude * Math.Cos(_radians)), (float)(_magnitude * Math.Sin(_radians)));
        }


    }


    public class classCircle
    {
        public PointF ptfCenter;
        public double dblRadius;

        public classCircle() { }
        public classCircle(PointF center, double radius)
        {
            ptfCenter = center;
            dblRadius = radius;
        }
    }

    public class classLine
    {
        public classLine() { }
        Rectangle recAABB = new Rectangle();
        public classLine(PointF pt1, PointF pt2)
        {
            _pt1.X = pt1.X;
            _pt1.Y = pt1.Y;

            _pt2.X = pt2.X;
            _pt2.Y = pt2.Y;
            setMCRad();
        }

        float _M;
        public float M
        {
            get { return _M; }
        }

        float _B;
        public float B
        {
            get { return _B; }
        }

        PointF _pt1;
        public PointF pt1
        {
            get { return _pt1; }
            set
            {
                if (value.X != _pt1.X
                    ||
                    value.Y != _pt1.Y)
                {
                    _pt1.X = value.X;
                    _pt1.Y = value.Y;
                    setMCRad();
                    recAABB.Width 
                        = recAABB.Height 
                        =0;
                }
            }
        }



        PointF _pt2;
        public PointF pt2
        {
            get { return _pt2; }
            set
            {
                if (value.X != _pt2.X
                    ||
                    value.Y != _pt2.Y)
                {
                    _pt2.X = value.X;
                    _pt2.Y = value.Y;
                    setMCRad();
                    recAABB.Width
                        = recAABB.Height
                        = 0;
                }
            }
        }

        PointF ptTL = new PointF();
        PointF ptBR = new PointF();

        void setMCRad()
        {
            float dblDelta_X = _pt2.X - _pt1.X;
            float dbLDelta_Y = _pt2.Y - _pt1.Y;
            if (dblDelta_X == 0)
                _M = float.MaxValue;
            else
                _M = dbLDelta_Y / dblDelta_X;
            // Y=mX+C -> C=Y-mX
            _B = _pt1.Y - _M * _pt1.X;
            _cRad.Radians = Math.Atan2(dbLDelta_Y, dblDelta_X);
            _cRad.Magnitude = Math.Sqrt(dblDelta_X * dblDelta_X + dbLDelta_Y * dbLDelta_Y);

            ptTL.X = pt1.X < pt2.X ? pt1.X : pt2.X;
            ptBR.X = pt1.X > pt2.X ? pt1.X : pt2.X;

            ptTL.Y = pt1.Y < pt2.Y ? pt1.Y : pt2.Y;
            ptBR.Y = pt1.Y > pt2.Y ? pt1.Y : pt2.Y;
        }

        classRadialCoordinate _cRad = new classRadialCoordinate();
        public classRadialCoordinate cRad
        {
            get { return _cRad; }
        }

        public bool ContainsPointF(PointF pt)
        {
            return (pt.X >= ptTL.X
                    && pt.X <= ptBR.X
                    && pt.Y >= ptTL.Y
                    && pt.Y <= ptBR.Y);
        }
        void setRec()
        {
            Point ptLocation = new Point((int)((pt1.X < pt2.X) ? pt1.X : pt2.X),(int)((pt1.Y < pt2.Y) ? pt1.Y : pt2.Y));
            Size sz = new Size((int)Math.Abs(pt1.X - pt2.X), (int)Math.Abs(pt1.Y - pt2.Y));
            recAABB = new Rectangle(ptLocation, sz);
        }

        public bool IntersectsLine(ref classLine cLine, ref PointF ptIntersection)
        {
            if (M == cLine.M) // lines are parallel
                return false;
            if (recAABB.Height == 0)
                setRec();
            if (cLine.recAABB.Width == 0)
                cLine.setRec();

            if (classMath3.RectanglesIntersect(recAABB, cLine.recAABB))
            {
                // y=Mx+C
                // y1=M1x1+C1 & y2=M2x2+C2
                // at y1=y2 & x1=x2
                // y = m1X+c1 = m2X+c2
                // c1-c2 = X(m2-m1)
                // x = (c1-c2)/(m2-m1)

                if (Math.Abs(cLine.M) > Math.Pow(2, 32))
                    ptIntersection.X = cLine.pt1.X;
                else if (Math.Abs(M) > Math.Pow(2, 32))
                    ptIntersection.X = pt1.X;
                else
                    ptIntersection.X = ((B - cLine.B) / (cLine.M - M));
                // y = xM+C -> x = (y-C)/M
                ptIntersection.Y = (ptIntersection.X * M + B);
                int intTolerance = 2;
                if ((pt1.X < pt2.X ? pt1.X : pt2.X) <= ptIntersection.X + intTolerance)
                {
                    if ((pt1.X > pt2.X ? pt1.X : pt2.X) >= ptIntersection.X - intTolerance )
                    {
                        if ((pt1.Y < pt2.Y ? pt1.Y : pt2.Y) <= ptIntersection.Y + intTolerance )
                        {
                            if ((pt1.Y > pt2.Y ? pt1.Y : pt2.Y) >= ptIntersection.Y -intTolerance )
                            {
                                if ((cLine.pt1.X < cLine.pt2.X ? cLine.pt1.X : cLine.pt2.X) <= ptIntersection.X + intTolerance )
                                {
                                    if ((cLine.pt1.X > cLine.pt2.X ? cLine.pt1.X : cLine.pt2.X) >= ptIntersection.X -intTolerance )
                                    {
                                        if ((cLine.pt1.Y < cLine.pt2.Y ? cLine.pt1.Y : cLine.pt2.Y) <= ptIntersection.Y + intTolerance )
                                        {
                                            if ((cLine.pt1.Y > cLine.pt2.Y ? cLine.pt1.Y : cLine.pt2.Y) >= ptIntersection.Y- intTolerance )
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
    }

    public enum enuCorner { TL, TR, BR, BL, _num };
    public class classLineF
    {
        public classLineF() { }

        public classLineF(PointF ptf1, PointF ptf2)
        {
            _ptf1.X = ptf1.X;
            _ptf1.Y = ptf1.Y;

            _ptf2.X = ptf2.X;
            _ptf2.Y = ptf2.Y;
            setMCRad();
        }

        float _M;
        public float M
        {
            get { return _M; }
        }

        float _B;
        public float B
        {
            get { return _B; }
        }

        PointF _ptf1;
        public PointF ptf1
        {
            get { return _ptf1; }
            set
            {
                if (value.X != _ptf1.X
                    ||
                    value.Y != _ptf1.Y)
                {
                    _ptf1.X = value.X;
                    _ptf1.Y = value.Y;
                    setMCRad();
                }
            }
        }

        PointF _ptf2;
        public PointF ptf2
        {
            get { return _ptf2; }
            set
            {
                if (value.X != _ptf2.X
                    ||
                    value.Y != _ptf2.Y)
                {
                    _ptf2.X = value.X;
                    _ptf2.Y = value.Y;
                    setMCRad();
                }
            }
        }

        PointF ptfTL = new PointF();
        PointF ptfBR = new PointF();

        void setMCRad()
        {
            float dblDelta_X = _ptf2.X - _ptf1.X;
            float dbLDelta_Y = _ptf2.Y - _ptf1.Y;
            if (dblDelta_X == 0)
                _M = float.MaxValue;
            else
                _M = dbLDelta_Y / dblDelta_X;
            // Y=mX+C -> C=Y-mX
            _B = _ptf1.Y - _M * _ptf1.X;
            _cRad.Radians = Math.Atan2(dbLDelta_Y, dblDelta_X);
            _cRad.Magnitude = Math.Sqrt(dblDelta_X * dblDelta_X + dbLDelta_Y * dbLDelta_Y);

            ptfTL.X = ptf1.X < ptf2.X ? ptf1.X : ptf2.X;
            ptfBR.X = ptf1.X > ptf2.X ? ptf1.X : ptf2.X;

            ptfTL.Y = ptf1.Y < ptf2.Y ? ptf1.Y : ptf2.Y;
            ptfBR.Y = ptf1.Y > ptf2.Y ? ptf1.Y : ptf2.Y;
        }

        classRadialCoordinate _cRad = new classRadialCoordinate();
        public classRadialCoordinate cRad
        {
            get { return _cRad; }
        }

        public bool ContainsPointF(PointF ptf)
        {
            return (ptf.X >= ptfTL.X
                    && ptf.X <= ptfBR.X
                    && ptf.Y >= ptfTL.Y
                    && ptf.Y <= ptfBR.Y);
        }


        public bool IntersectsLine(ref classLineF cLine, ref PointF ptfIntersection)
        {
            if (M == cLine.M) // lines are parallel
                return false;

            // y=Mx+C
            // y1=M1x1+C1 & y2=M2x2+C2
            // at y1=y2 & x1=x2
            // y = m1X+c1 = m2X+c2
            // c1-c2 = X(m2-m1)
            // x = (c1-c2)/(m2-m1)

            if (Math.Abs(cLine.M) > Math.Pow(2, 32) || Math.Abs(M) > Math.Pow(2, 32))
            {
                ptfIntersection.X = Math.Abs(cLine.M) > Math.Abs(M)
                                                      ? (cLine.ptf1.X + cLine.ptf2.X) / 2
                                                      : (ptf1.X + ptf2.X) / 2;
            }
            else
                ptfIntersection.X = (B - cLine.B) / (cLine.M - M);


            // y = xM+C -> x = (y-C)/M
            if (Math.Abs(cLine.M) < Math.Pow(2, -32) || Math.Abs(M) < Math.Pow(2, -32))
            {
                ptfIntersection.Y = Math.Abs(cLine.M) < Math.Abs(M)
                                                       ? (cLine.ptf1.Y + cLine.ptf2.Y) / 2
                                                       : (ptf1.Y + ptf2.Y) / 2;
            }
            else
            {
                if (Math.Abs(M) > Math.Pow(2, 32))
                    ptfIntersection.Y = ptfIntersection.X * cLine.M + cLine.B;
                else
                    ptfIntersection.Y = ptfIntersection.X * M + B;
            }

            if ((ptf1.X < ptf2.X ? ptf1.X : ptf2.X) <= ptfIntersection.X)
            {
                if ((ptf1.X > ptf2.X ? ptf1.X : ptf2.X) >= ptfIntersection.X)
                {
                    if ((ptf1.Y < ptf2.Y ? ptf1.Y : ptf2.Y) <= ptfIntersection.Y)
                    {
                        if ((ptf1.Y > ptf2.Y ? ptf1.Y : ptf2.Y) >= ptfIntersection.Y)
                        {
                            if ((cLine.ptf1.X < cLine.ptf2.X ? cLine.ptf1.X : cLine.ptf2.X) <= ptfIntersection.X)
                            {
                                if ((cLine.ptf1.X > cLine.ptf2.X ? cLine.ptf1.X : cLine.ptf2.X) >= ptfIntersection.X)
                                {
                                    if ((cLine.ptf1.Y < cLine.ptf2.Y ? cLine.ptf1.Y : cLine.ptf2.Y) <= ptfIntersection.Y)
                                    {
                                        if ((cLine.ptf1.Y > cLine.ptf2.Y ? cLine.ptf1.Y : cLine.ptf2.Y) >= ptfIntersection.Y)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
    }

    public class classGraphics
    {
        public enum enuAlign { Left, Center, Right };
        public static void DrawString(string strText, string strFontFamilyName, Color clr, Rectangle rec, ref Graphics g)
        {
            DrawString(strText, strFontFamilyName, clr, rec, ref g, enuAlign.Left);
        }
        public static void DrawString(string strText, string strFontFamilyName, Color clr, Rectangle rec, ref Graphics g, enuAlign eAlign)
        {
            Font fnt;
            List<string> lstLines = new List<string>();
            SolidBrush br = new SolidBrush(clr);
            
            // calculate largest font that will fit inside rec
            {
                int intRecArea = (int)(rec.Width * rec.Height * .8);
                int intFontSize = 73;
                int intTextArea = 0;
                do
                {
                    intFontSize--;
                    fnt = new Font(strFontFamilyName, intFontSize);
                    Size szText = TextRenderer.MeasureText(strText, fnt);
                    intTextArea = szText.Width * szText.Height;
                } while (intTextArea > intRecArea && intFontSize >= 8);
            }
            // parse text into lines that will fit onto the screen
            {
                string strCopy = strText;
                char[] chrEndWord = " .,-:!?".ToArray<char>();
                string strLine = "";
                while (strCopy.Length > 0)
                {
                    int intEndWord = strCopy.IndexOfAny(chrEndWord, strLine.Length +1);
                    if (intEndWord > 0)
                    {
                        string strLine_Test = strCopy.Substring(0, intEndWord);
                        Size szLine = TextRenderer.MeasureText(strLine_Test, fnt);
                        if (szLine.Width > rec.Width)
                        {
                            if (strLine.Length == 0)
                                strLine = CutToWidth(strCopy, fnt, rec);
                            lstLines.Add(strLine);
                            strCopy = strCopy.Substring(strLine.Length).Trim();
                            strLine = "";
                        }
                        else
                            strLine = strLine_Test;
                    }
                    else
                    {
                        lstLines.Add(strCopy);
                        strCopy = "";
                    }
                }
            }
            // draw the lines 
            {
                int intTop = rec.Top;
                for (int intLineCounter = 0; intLineCounter < lstLines.Count; intLineCounter++)
                {
                    string strLine = lstLines[intLineCounter];
                    Size szLine = TextRenderer.MeasureText(strLine, fnt);
                    switch (eAlign)
                    {
                        case enuAlign.Left:
                            {
                                g.DrawString(strLine, fnt, br, new Point(rec.Left, intTop));
                            }
                            break;

                        case enuAlign.Center:
                            {
                                g.DrawString(strLine, fnt, br, new Point(rec.Left + (rec.Width - szLine.Width) / 2, intTop));
                            }
                            break;

                        case enuAlign.Right:
                            {
                                g.DrawString(strLine, fnt, br, new Point(rec.Right - szLine.Width, intTop));
                            }
                            break;
                    }
                    intTop += szLine.Height;
                } 
            }
        }


        static string CutToWidth(string strText, Font fnt, Rectangle rec)
        {
            int intCount = 0;
            do
            {
                intCount++;
                Size szTest = TextRenderer.MeasureText(strText.Substring(0, intCount), fnt);
                if (szTest.Width > rec.Width)
                {
                    intCount++;
                    break;
                }
            }
            while (intCount < strText.Length);

            if (intCount > 0 && intCount < strText.Length)
                return strText.Substring(0, intCount);

            return strText;
        }
    }


}