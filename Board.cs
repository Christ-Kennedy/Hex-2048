using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Math3;

namespace Hex_2048
{
    public class Board
    {
        public static classTile[,] Tiles = null;
        public static void Populate()
        {
            // init array
            Tiles = new classTile[formHex2048.szGame.Width, formHex2048.szGame.Height];

            // create tiles
            for (int intX = 0; intX < formHex2048.szGame.Width; intX++)
                for (int intY = 0; intY < formHex2048.szGame.Height; intY++)
                    Tiles[intX, intY] = new classTile(new Point(intX, intY));

            // create pointers between neaghbouring tiles
            for (int intX = 0; intX < formHex2048.szGame.Width; intX++)
                for (int intY = 0; intY < formHex2048.szGame.Height; intY++)
                {
                    classTile cTile = Tiles[intX, intY];
                    for (int intDirCounter = 0; intDirCounter < 6; intDirCounter++)
                        cTile.lstNeaghbours.Add(Tile_FromId(formHex2048.move(cTile.ID, intDirCounter)));
                }
        }

        static uint untUpdateCounter = 0;
        public static uint UpdateCounter { get { return untUpdateCounter; } }

        public static void Update() { untUpdateCounter++; }

        public static classTile Tile_FromId(Point ptID)
        {
            if (formHex2048.TileInBounds(ptID))
                return Tiles[ptID.X, ptID.Y];
            else
                return null;
        }

    }

    public class classTile
    {

        #region Static 
        #region Static_Functions
        public static string TileText_Get(int intTileValue)
        {
            if (intTileValue <= 0) return "";
            int intValue = (int)Math.Pow(2, intTileValue - 1);
            return intValue.ToString();
        }
        #endregion

        #region static_Variables
        public const int numColors = 14;
        public static Color[] clrArray =
           {
            Color.Gray,             // -1
            Color.Blue,             // 0
            Color.Red,              // 1
            Color.Green,            // 2
            Color.Magenta,          // 3
            Color.LightGreen,       // 4
            Color.Yellow,           // 5
            Color.Pink,             // 6
            Color.Purple,           // 7
            Color.Orchid,           // 8
            Color.LightBlue,        // 9
            Color.DarkRed,          // 10
            Color.Orange,           // 11
            Color.DarkBlue,         // 12
            Color.DarkOrange        // 13
        };
        static Font fnt = new Font("ms sans-serif", 14, FontStyle.Bold);
        #endregion
        #endregion

        #region Properties
        public classTile(Point ptID) { this.ptID = ptID; }
        Point ptID = new Point();
        public Point ID { get { return ptID; } }

        int intValue = 0;
        public int Value
        {
            get { return intValue; }
            set 
            {
                intValue = value;
            }
        }

        public PointF ptfDrawCenter
        {
            get
            {
                //PointF ptfTemp = ptCenter;
                if (cRad_Shift.Magnitude != 0)
                    return classMath3.AddTwoPointFs(ptCenter, cRad_Shift.toPointF());
                else
                    return new PointF(ptCenter.X, ptCenter.Y);
            }
        }

        public PointF ptfTL { get { return classMath3.SubTwoPointFs(ptfDrawCenter, new PointF(formHex2048.szTile.Width / 2, formHex2048.szTile.Height / 2)); } }
        #endregion

        #region variables
        public List<classTile> lstNeaghbours = new List<classTile>();
        public classRadialCoordinate cRad_Shift = new classRadialCoordinate();
        public double dblShift_DeltaMag = 0;
        public double dblShiftMag_Max = 0;
        Point _ptCenter = new Point();
        public Point ptCenter
        {
            get { return _ptCenter; }
            set 
            {
                _ptCenter = value; 
            }
        }
        List<Point> lstPushSources = new List<Point>();
        uint untUpdate = uint.MaxValue;
        #endregion

        #region functions
        public void Animate(ref Graphics g)
        {
            if (untUpdate != Board.UpdateCounter)
                Center_update();
            Shift();
            /*
           g.FillEllipse(Brushes.Gray, new RectangleF(new PointF(ptfTL.X + 1, ptfTL.Y + 1), new SizeF(formHex2048.szTile.Width - 2, formHex2048.szTile.Height - 2)));
           /*/
            SolidBrush sbr = new SolidBrush(clrArray[intValue]);
            g.FillEllipse(sbr, new RectangleF(new PointF(ptfTL.X + 1, ptfTL.Y + 1), new SizeF(formHex2048.szTile.Width - 2, formHex2048.szTile.Height - 2)));

            if (classChallenge.ArrTiles != null)
            {
                classTile cTileChallenge = classChallenge.ArrTiles[ptID.X, ptID.Y];
                if (cTileChallenge != null)
                {
                    Color clrPen = (cTileChallenge.Value == Value)
                                                     ? (classChallenge.Highlight ? Color.Gold : clrArray[cTileChallenge.Value])
                                                     :(classChallenge.Highlight ?  clrArray[cTileChallenge.Value] : Color.White);
                    //clrPen = Color.White;
                    Pen pChallenge = new Pen(clrPen, 3);
                    g.DrawEllipse(pChallenge, new RectangleF(new PointF(ptfTL.X + 1, ptfTL.Y + 1), new SizeF(formHex2048.szTile.Width - 2, formHex2048.szTile.Height - 2)));
                }
            }
            if (intValue > 0)
            {
                string strText = TileText_Get(intValue);
                Size szText = TextRenderer.MeasureText(strText, fnt);
                g.DrawString(strText, 
                             fnt, 
                             Brushes.Black, 
                             new Point((int)(ptfDrawCenter.X - szText.Width / 2), 
                                       (int)(ptfDrawCenter.Y - szText.Height / 2)));
            }
            // */
        }

        public void Radiate(double dblFactor)
        {
            lstPushSources.Add(ID);
            double dblMagnitude_Max = (double)formHex2048.szTile.Width * .35 * dblFactor;
            double dblDeltaMagnitude = dblMagnitude_Max * .2;
            for (int intDirCounter = 0; intDirCounter < 6; intDirCounter++)
            {
                classTile cTileNeaghbour = lstNeaghbours[intDirCounter];
                if (cTileNeaghbour != null)
                    cTileNeaghbour.Impulse(ID, intDirCounter, dblDeltaMagnitude, dblMagnitude_Max);
            }
        }

        public void Impulse(Point ptSource, int intImpulseDir)
        {
            double dblMagnitude_Max = (double)formHex2048.szTile.Width * .35 * (double)intValue / (double)numColors;
            double dblDeltaMagnitude = dblMagnitude_Max * .2;
            Impulse(ptSource, intImpulseDir, dblDeltaMagnitude, dblMagnitude_Max);
        }

        public void Impulse (Point ptSource, int intImpuseDir, double dblDelta_Magnitude, double dblMagnitude_Max)
        {
            if (lstPushSources.Contains(ptSource))
                return;
            lstPushSources.Add(ptSource);
            classRadialCoordinate cRadNew = new classRadialCoordinate(RadiansFromDir(intImpuseDir), dblDelta_Magnitude);
            Point ptNew = cRadNew.toPoint();
            Point ptCurrent = cRad_Shift.toPoint();
            Point ptSum = classMath3.AddTwoPoints(ptNew, ptCurrent);
            cRad_Shift = new classRadialCoordinate(ptSum);
            
            this.dblShiftMag_Max += dblMagnitude_Max;
            this.dblShift_DeltaMag = dblShiftMag_Max * .2;
        }


        double RadiansFromDir(int intDir){return -Math.PI / 2 + ((double)intDir * Math.PI / 3.0);}
        int DirFromRadians(double dblRadians)
        {
            double dblRadShifted = classMath3.cleanAngle(dblRadians + Math.PI/2.0 + Math.PI/6.0);
            return (int)(dblRadShifted / (Math.PI / 3.0));
        }

        void Shift()
        {
            cRad_Shift.Magnitude += dblShift_DeltaMag;
            if (cRad_Shift.Magnitude < 0)
            {
                cRad_Shift = new classRadialCoordinate();
                dblShiftMag_Max
                    = dblShift_DeltaMag
                    = 0;
                lstPushSources.Clear();
            }
            else if (cRad_Shift.Magnitude > dblShiftMag_Max)
            {
                cRad_Shift.Magnitude = dblShiftMag_Max;
                int intDir = DirFromRadians(cRad_Shift.Radians);
                HitNeaghbours(intDir, dblShift_DeltaMag , dblShiftMag_Max );
                dblShift_DeltaMag *= -1;
            }
        }

        void HitNeaghbours(int intImpuseDir, double dblDelta_Magnitude, double dblMagnitude_Max)
        {
            if (dblMagnitude_Max < 0.1) 
                return;

            List<classTile> lstHitList = new List<classTile>();
            for (int intDirCounter = intImpuseDir - 1; intDirCounter <= intImpuseDir + 1; intDirCounter++)
            {
                int intDir = (intDirCounter + 6) % 6;
                classTile cNeaghbour = lstNeaghbours[intDir];
                lstHitList.Add(cNeaghbour);
            }

            while (lstHitList.Count >0)
            {
                int intRNDIndex = classRND.Get_Int(0, lstHitList.Count);
                classTile cNeaghbour = lstHitList[intRNDIndex];
                if (cNeaghbour != null)
                    cNeaghbour.Impulse(ID, lstNeaghbours.IndexOf(cNeaghbour), dblDelta_Magnitude*.33, dblMagnitude_Max*.33);
                lstHitList.Remove(cNeaghbour);
            }
        }

        void Center_update()
        {
            ptCenter = formHex2048.TileCenter(ID);
            untUpdate = Board.UpdateCounter;
        }
        #endregion 
    }
}
