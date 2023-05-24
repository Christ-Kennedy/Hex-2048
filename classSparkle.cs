using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Drawing;
using Math3;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

namespace Hex_2048
{
    public enum enuSparkleColors { Green, LightBlue, Orange, Pink, Purple, Red, Yellow, _numSparkleColors };
    public enum enuSparkleShapes { Star, Caltrop, Spike, _numSparkleShapes };

    public class classFlare
    {
        const int conMaxAge = 10;
        static int intAge = 0;

        static List<Point> lstLocation = new List<Point>();
        static List<Point> lstVelocity = new List<Point>();
        static public Point ptStart
        {
            set
            {
                lstLocation.Clear();
                lstVelocity.Clear();
                int intNumFlares = 3;
                int intVelMin = 15;
                for (int intFlareCounter = 0; intFlareCounter < intNumFlares; intFlareCounter++)
                {
                    lstLocation.Add(value);
                    int intVelX = (intVelMin + classRND.Get_Int(0, intVelMin)) * classRND.Get_Sign();
                    int intVelY = (intVelMin + classRND.Get_Int(0, intVelMin)) * classRND.Get_Sign();
                    lstVelocity.Add(new Point(intVelX, intVelY));
                }
                intAge = 0;

            }
        }


        static public bool Animate()
        {
            intAge++;
            if (intAge >= conMaxAge) return false;

            double dblMaxSparkles = 30;
            double dblTailConeAngle = Math.PI / 12;
            int intNumSparkles = (int)((double)intAge / (double)conMaxAge * dblMaxSparkles);
            for (int intFlareCounter = 0; intFlareCounter < lstLocation.Count; intFlareCounter++)
            {
                Point ptVelocity = lstVelocity[intFlareCounter];
                int intX = (int)(lstLocation[intFlareCounter].X +  ptVelocity.X );
                int intY = (int)(lstLocation[intFlareCounter].Y + ptVelocity.Y );
                lstLocation[intFlareCounter] = new Point(intX, intY);
                classRadialCoordinate cRadFlare_Tail = new classRadialCoordinate(ptVelocity);
                cRadFlare_Tail.Radians += Math.PI;
                

                for (int intSparkleCounter = 0; intSparkleCounter < intNumSparkles; intSparkleCounter++)
                {
                    int intVelocity = classRND.Get_Int(10, 15);
                    Point ptSparkleVelocity = classRND.Get_Point(new classMinMax(-intVelocity, intVelocity),
                                                                 new classMinMax(-intVelocity, intVelocity));

                    new classSparkle(lstLocation[intFlareCounter], ptSparkleVelocity);
                }
            }
            return true;
        }

    }
    public class classStreamer
    {
        const int conMaxAge = 30;
        const double conDeltaRadians = Math.PI / 6;
        static int intAge = 0;
        static classRadialCoordinate cRad = new classRadialCoordinate(0, 1);
        static Point _ptStart = new Point();
        static public Point ptStart
        {
            get { return _ptStart; }
            set
            {
                _ptStart = value;
                intAge = 0;
                cRad = new classRadialCoordinate(0, 1);
            }
        }

        static public bool Animate()
        {
            intAge++;
            if (intAge >= conMaxAge) return false;

            // using equation of ellipse   x^2/rx^2 + y^2/ry^2 = 1
            //           for max sparkles at mid-age (maxAge/2)
            //              set rx = Mid-age
            //                  ry = max sparkle
            //                  shift equation to center about mid-age
            // solve for y 
            int intMaxSparkles = 40;
            int intMidAge = conMaxAge / 2;
            int intNumSparkles = (int)Math.Sqrt((1 - (Math.Pow(intMidAge - intAge, 2) / Math.Pow(intMidAge, 2))) * Math.Pow(intMaxSparkles, 2));
            double dblMaxRadius = (double)(formHex2048.recGame.Width / 2f);
            cRad.Magnitude = dblMaxRadius * ((double)intAge / (double)conMaxAge);
            cRad.Radians += conDeltaRadians;

            Point ptCenter = Math3.classMath3.AddTwoPoints(ptStart, cRad.toPoint());
            
            for (int intSparkleCounter = 0; intSparkleCounter < intNumSparkles; intSparkleCounter++)
            {
                int intVelocity = classRND.Get_Int(10, 15);
                Point ptVelocity = classRND.Get_Point(new classMinMax(-intVelocity, intVelocity),
                                                      new classMinMax(-intVelocity, intVelocity));
                new classSparkle(ptCenter, ptVelocity);
            }
            return true;
        }
    }
    public class classSparkle
    {
        static bool bolInitialized = false;
        static List<classSparkle> lstSparkles = new List<classSparkle>();

        public classSparkle(Point ptLocation, Point ptVelocity)
        {
            lstSparkles.Add(this);

            if (!bolInitialized)
                InitFile();
            eColor = (enuSparkleColors)classRND.Get_Int(0, (int)enuSparkleColors._numSparkleColors);
            eShape = (enuSparkleShapes)classRND.Get_Int(0, (int)enuSparkleShapes._numSparkleShapes);
            
            dblDeltaRad = classRND.Get_Double() * Math.PI / 4.0;
            dblRad = classRND.Get_Double() * Math.PI * 2.0;

            this.Location = ptLocation;
            this.Velocity = ptVelocity;

            Age = conStartAge;
        }

        #region settings
        enuSparkleColors _eColor = enuSparkleColors.Green;
        public enuSparkleColors eColor
        {
            get { return _eColor; }
            set { _eColor = value; }
        }

        enuSparkleShapes _eShape = enuSparkleShapes.Caltrop;
        public enuSparkleShapes eShape
        {
            get { return _eShape; }
            set { _eShape = value; }
        }

        int intAge = 10;
        public int Age
        {
            get { return intAge; }
            set { intAge = value; }
        }

        double dblRad = 0;
        public double Rad
        {
            get { return dblRad; }
            set { dblRad = value; }
        }

        double dblDeltaRad = 0;
        public double DeltaRad
        {
            get { return dblDeltaRad; }
            set { dblDeltaRad = value; }
        }

        Point ptLocation = new Point();
        public Point Location
        {
            get { return ptLocation; }
            set { ptLocation = value; }
        }

        Point ptVelocity = new Point();
        public Point Velocity
        {
            get { return ptVelocity; }
            set { ptVelocity = value; }
        }
        #endregion 

        static public void Animate(ref Graphics g)
        {
            for (int intSparkleCounter = lstSparkles.Count - 1; intSparkleCounter >= 0; intSparkleCounter--)
            {
                classSparkle cSparkle = lstSparkles[intSparkleCounter];
                cSparkle.Age--;
                if (cSparkle.Age <= 0)
                {
                    lstSparkles.Remove(cSparkle);
                }
                else
                {
                    cSparkle.ptLocation.X += cSparkle.Velocity.X ;
                    cSparkle.ptLocation.Y += cSparkle.Velocity.Y ;
                    cSparkle.Rad += cSparkle.DeltaRad;
                    Bitmap bmpSparkle = Image(cSparkle.eShape, cSparkle.eColor, cSparkle.Rad);

                    double dblSizeFactor = (double)cSparkle.Age / (double)conStartAge;
                    Size szDraw = new Size((int)(bmpSparkle.Width * dblSizeFactor), (int)(bmpSparkle.Height * dblSizeFactor));
                    Point ptDraw = new Point(cSparkle.Location.X - szDraw.Width / 2, cSparkle.Location.Y - szDraw.Height / 2);

                    Rectangle recDestination = new Rectangle(ptDraw, szDraw);
                    Rectangle recSource = new Rectangle(0, 0, bmpSparkle.Width, bmpSparkle.Height);

                    g.DrawImage(bmpSparkle, recDestination, recSource, GraphicsUnit.Pixel);
                }
            }
        }

        #region StaticFunctionsAndVariables


        public static void Burst(Point ptSource, int intNum, int intVelocity)
        {
            classRadialCoordinate cRadVelocity = new classRadialCoordinate(0, intVelocity);
            double dblDeltaRad = 2.0 * Math.PI / (double)(intNum);
            for (int intCounter =0; intCounter < intNum; intCounter++)
            {
                cRadVelocity.Radians = dblDeltaRad * intCounter;
                new classSparkle(ptSource, cRadVelocity.toPoint());
            }

        }

        static public Bitmap Image(enuSparkleShapes eShape, enuSparkleColors eColor, double dblRad) { return Image(eShape, eColor, IndexFromRad(dblRad)); }
        static public Bitmap Image(enuSparkleShapes eShape, enuSparkleColors eColor, int intIndex)
        {
            int intBaseIndex = intIndex % NumRotationPerQuarterTurn;
            Bitmap bmpRetVal = Image_Get(eShape, eColor, intBaseIndex);

            if (intIndex < NumRotationPerQuarterTurn)
            {
                // do nothing
            }
            else if (intIndex < 2 * NumRotationPerQuarterTurn)
            {
                bmpRetVal.RotateFlip(RotateFlipType.Rotate270FlipNone);
            }
            else if (intIndex < 3 * NumRotationPerQuarterTurn)
            {
                bmpRetVal.RotateFlip(RotateFlipType.Rotate180FlipNone);
            }
            else
            {
                bmpRetVal.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
            return bmpRetVal;
        }

        const int conStartAge = 15;
        static long lngFileIndexAddress = -1;
        public const int NumRotationPerQuarterTurn = 8;
        public static BinaryFormatter formatter = new BinaryFormatter();
        static FileStream fs = null;
        static string _strFilename = "SparkleImages.bin";
        static string Filename
        {
            get { return formHex2048.WorkingDirectory + _strFilename; }
        }
        static bool FileExists { get { return System.IO.File.Exists(Filename); } }

        static int IndexFromRad(double dblRad)
        {
            double dblDeltaRad = (Math.PI / 2.0) / (double)NumRotationPerQuarterTurn;
            dblRad = Math3.classMath3.cleanAngle(dblRad);

            return (int)(dblRad / dblDeltaRad);
        }

        public static void InitFile()
        {
            SizeLongInteger_Measure();
            if (!FileExists)
                File_Create();
            else
                File_Open();
            bolInitialized = true;
        }

        static long SizeLongInteger = 0;
        static void SizeLongInteger_Measure()
        {
            string strTempFilename = "DoNotTryThisAtHome.bin";
            FileStream fsTemp = new FileStream(strTempFilename, FileMode.Create);
            fsTemp.Position = 0;

            formatter.Serialize(fsTemp, (long)1);
            SizeLongInteger = fsTemp.Position;

            fsTemp.Close();

            System.IO.File.Delete(strTempFilename);
        }

        static void File_Create()
        {
            fs = new FileStream(Filename, FileMode.Create);
            fs.Position = 0;

            Color[] clrArray = { Color.Green, Color.LightBlue, Color.Orange, Color.Pink, Color.Purple, Color.Red, Color.Yellow };
            List<Color> lstColor = clrArray.ToList<Color>();

            // generate sparkle images and save to file
            List<long> lstAddresses = new List<long>();
            for (int intColorCounter = 0; intColorCounter < (int)enuSparkleColors._numSparkleColors; intColorCounter++)
            {
                enuSparkleColors eColor = (enuSparkleColors)intColorCounter;
                for (int intShapeCounter = 0; intShapeCounter < (int)enuSparkleShapes._numSparkleShapes; intShapeCounter++)
                {
                    enuSparkleShapes eShape = (enuSparkleShapes)intShapeCounter;
                    Bitmap bmpShapeColor = DrawSparkleShape(eShape, clrArray[intColorCounter]);
                    double dblDeltaRad = (Math.PI / 2.0) / (double)NumRotationPerQuarterTurn;
                    for (double dblRad = 0; dblRad < Math.PI / 2; dblRad += dblDeltaRad)
                    {
                        Bitmap bmpRotImg = classRotateImage.rotateImage(bmpShapeColor, dblRad);
                        bmpRotImg.MakeTransparent(Color.White);
                        lstAddresses.Add(fs.Position);
                        formatter.Serialize(fs, (Bitmap)bmpRotImg);
                    }
                }
            }
            lngFileIndexAddress = fs.Position;

            for (int intAddrCounter = 0; intAddrCounter < lstAddresses.Count; intAddrCounter++)
                formatter.Serialize(fs, (long)lstAddresses[intAddrCounter]);

            formatter.Serialize(fs, (long)lngFileIndexAddress);
        }
        static Color clrTransparent = Color.Black;
        static Bitmap DrawSparkleShape(enuSparkleShapes eShape, Color clr)
        {
            switch (eShape)
            {
                case enuSparkleShapes.Spike:
                case enuSparkleShapes.Star:
                case enuSparkleShapes.Caltrop:
                    {
                        int intNumPoints = 5;
                        int intRadius_Outer = 8;
                        int intRadius_Inner = 4;

                        switch (eShape)
                        {
                            case enuSparkleShapes.Spike:
                                intNumPoints = 2;
                                intRadius_Outer = 12;
                                intRadius_Inner = 2;
                                break;

                            case enuSparkleShapes.Star:
                                intNumPoints =5;
                                intRadius_Outer = 8;
                                intRadius_Inner = 4;
                                break;

                            case enuSparkleShapes.Caltrop:
                                intNumPoints = 4;
                                intRadius_Outer = 8;
                                intRadius_Inner = 4;
                                break;
                        }

                        double dblDeltaRad = Math.PI * 2.0 / (double)(2 * intNumPoints);
                        List<Point> lstPoints = new List<Point>();
                        Point ptCenter = new Point(intRadius_Outer, intRadius_Outer);
                        for (int intPointCounter = 0; intPointCounter < intNumPoints * 2; intPointCounter++)
                        {
                            int intRadius = intPointCounter % 2 == 0
                                                                 ? intRadius_Outer
                                                                 : intRadius_Inner;
                            double dblRad = (double)intPointCounter * dblDeltaRad;
                            Point pt = new Point(ptCenter.X + (int)(intRadius * Math.Cos(dblRad)),
                                                 ptCenter.Y + (int)(intRadius * Math.Sin(dblRad)));
                            lstPoints.Add(pt);
                        }
                        Bitmap bmpRetVal = new Bitmap(2 * intRadius_Outer, 2 * intRadius_Outer);

                        using (Graphics g = Graphics.FromImage(bmpRetVal))
                        {
                            g.FillRectangle(new SolidBrush(clrTransparent), new RectangleF(0, 0, bmpRetVal.Width, bmpRetVal.Height));
                            g.FillPolygon(new SolidBrush(clr), lstPoints.ToArray());
                        }
                        bmpRetVal.MakeTransparent(clrTransparent);

                        return bmpRetVal;
                    }

                default:
                    MessageBox.Show("shape unidentified -- could not create sparkle image");
                    return null;
            }


        }

        static void File_Open()
        {
            try
            {
                fs = new FileStream(Filename, FileMode.Open);
                fs.Position = fs.Length - SizeLongInteger;
                lngFileIndexAddress = (long)formatter.Deserialize(fs);
            }
            catch (Exception e)
            {
                formHex2048.instance.tmrFlashDelay.Enabled = false;
                Preferences.Abort = true;
                formHex2048.instance.Quit();
            }
            
        }

        static long ImageAddress(enuSparkleShapes eShape, enuSparkleColors eColor, int intRotation)
        {
            long lngIndexAddress = lngFileIndexAddress
                                    + (((int)eColor * ((int)enuSparkleShapes._numSparkleShapes * NumRotationPerQuarterTurn))
                                        + ((int)eShape * NumRotationPerQuarterTurn)
                                        + intRotation) * SizeLongInteger;
            fs.Position = lngIndexAddress;
            return (long)formatter.Deserialize(fs);
        }

        static Bitmap Image_Get(enuSparkleShapes eShape, enuSparkleColors eColor, int intRotation)
        {
            fs.Position = ImageAddress(eShape, eColor, intRotation);
            return (Bitmap)formatter.Deserialize(fs);
        }
        #endregion
    }

}