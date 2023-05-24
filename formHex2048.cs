using Math3;
using Microsoft.Win32.SafeHandles;
using PerPixelForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hex_2048
{
    public partial class formHex2048 : PerPixelForm.PerPixelAlphaForm
    {
        #region FSM
        enuHex2048_FSM _eFSM = enuHex2048_FSM.Game_Over;
        public enuHex2048_FSM eFSM
        {
            get { return _eFSM; }
            set
            {
                switch (_eFSM)
                {
                    case enuHex2048_FSM.Form_Resizing:
                    case enuHex2048_FSM.Form_Moving:
                        {
                            bolIgnoreFormChanges = false;
                            Preferences.Save();
                        }
                        break;

                    case enuHex2048_FSM.Tiles_RemoveFinal:
                        {
                            Board.Tiles[ptFinalTileRemoved.X, ptFinalTileRemoved.Y].Value = 0;
                        }
                        break;
                }

                _eFSM = value;

                switch (_eFSM)
                {
                    case enuHex2048_FSM.idle:
                        {
                            intTilesGathered_ThisScore = 0;
                            if (bolQuitWhenIdle)
                                Quit();
                        }
                        break;

                    case enuHex2048_FSM.Tiles_Adding_Init:
                        {
                            bolNewTiles = true;
                            AddRndTiles();
                        }
                        break;

                    case enuHex2048_FSM.Tiles_Adding_Animate:
                        {

                        }
                        break;

                    case enuHex2048_FSM.Tiles_GatherLike:
                        {
                            Tiles_GatherLike();
                        }
                        break;

                    case enuHex2048_FSM.Tiles_GatherLike_Flash:
                        {
                            intCounter_FlashTiles = 12;
                        }
                        break;

                    case enuHex2048_FSM.Tiles_RemoveLike:
                        {
                            Tiles_RemoveLike();
                        }
                        break;

                    case enuHex2048_FSM.Game_New:
                        {
                            Game_New();
                        }
                        break;

                    case enuHex2048_FSM.Game_Over:
                        {
                            DrawGame_Over();
                        }
                        break;

                    case enuHex2048_FSM.Tiles_RemoveFinal:
                        {

                        }
                        break;

                    case enuHex2048_FSM.Tile_Moving:
                        {
                            MoveSelectedTile();
                        }
                        break;

                    case enuHex2048_FSM.Form_Moving:
                    case enuHex2048_FSM.Form_Resizing:
                        {
                            //bolIgnoreFormChanges = true;
                        }
                        break;
                }
            }
        }

        #endregion

        #region Variables
        public static formHex2048 instance = null;

        Semaphore semFormChanges = new Semaphore(1, 1);
        static Random rnd = new Random();
        static float fltSize_Default = 14;
        ContextMenu cMnu = new ContextMenu();
        static Font fnt = new Font("ms sans-serif", 14, FontStyle.Bold);

        public static string WorkingDirectory = "";
        System.Windows.Forms.Timer tmr = new System.Windows.Forms.Timer();

        public static Size szGame = new Size(7, 9);
        public static Size szTile = new Size();
        List<classTile_New> lstTilesNew = new List<classTile_New>();

        List<Point> lstPtMove = new List<Point>();
        public static List<List<Point>> lstPtMoveDir = new List<List<Point>>();

        List<Point> lstTilesGathered = new List<Point>();
        Bitmap bmpFrameSource = Properties.Resources.Frame;
        Bitmap[] bmpGame_Over = new Bitmap[2];

        bool bolHighlight = false;
        bool bolMouseOverGrab = false;
        bool bolMouseOverResize = false;
        bool bolTilesGathered = false;
        bool bolActivated = false;
        bool bolQuitWhenIdle = false;
        public bool bolIgnoreFormChanges = true;
        public static List<bool> lstFlareTrigger = new List<bool>();

        public static int intHighScore = 1000;

        int intCounter_AddTile = 0;
        int intCounter_FlashTiles = 0;
        int intCounter_GameOver = 0;
        int intCounter_NoPath = 0;
        int intTimerCounter = 0;
        const int intTimerCounterMax = 64;

        static public long lngTimerDelay_Minutes = -1;
        DateTime dt_Start = DateTime.Now;
        TimeSpan elapsedSpan;
        #endregion

        #region dimensions
        static Point ptDrawGameGap = new Point();
        static Point ptFinalTileRemoved = new Point();
        static Point ptGrab = new Point();

        public static PointF ptNewTilesStart = new PointF();
        static RectangleF recResize = new RectangleF();
        static RectangleF recGrab = new RectangleF();
        static RectangleF recChallengeTimer = new RectangleF();
        static RectangleF recTitle = new RectangleF();
        public static RectangleF recGame = new RectangleF();
        public static RectangleF recScore = new RectangleF();
        public  static PointF ptScoreBoardCenter = new PointF();
        public static RectangleF recFrame = new RectangleF();
        public static SizeF szForm = new SizeF();

        static PointF ptNewTilesStart_default = new PointF(258, 113);
        static RectangleF recResize_default = new RectangleF(3,768,520-3,774-768);
        static RectangleF recGrab_default = new RectangleF(207, 12, 312 - 207, 31 - 12);
        static RectangleF recChallengeTimer_default = new RectangleF(226, 99, 283-236, 119-99);
        static RectangleF recTitle_default = new RectangleF(160, 40, 359 - 160, 78 - 40);
        public static RectangleF recGame_default = new RectangleF(69, 164, 453 - 69, 708 - 164);
        static RectangleF recScore_default = new RectangleF(27, 21, 225 - 27, 58 - 21);
        static PointF ptScoreBoardCenter_default = new PointF (259, 747);
        public static RectangleF recFrame_default = new RectangleF(0,0,524, 779);
        public static SizeF szForm_default = new SizeF();

        static void DeltaDrawSizeAdjust_RecResize(ref RectangleF rec, RectangleF recDefault, float fltFactor)
        {
            rec.X = recDefault.X * fltFactor ;
            rec.Y = recDefault.Y * fltFactor ;
            rec.Width = recDefault.Width * fltFactor;
            rec.Height = recDefault.Height * fltFactor;
        }

        static void RecSizes_Set()
        {
            szForm.Width = szForm_default.Width * fltDeltaDrawSize;
            szForm.Height = szForm_default.Height * fltDeltaDrawSize;

            DeltaDrawSizeAdjust_RecResize(ref recGrab, recGrab_default, fltDeltaDrawSize);
            DeltaDrawSizeAdjust_RecResize(ref recChallengeTimer, recChallengeTimer_default, fltDeltaDrawSize);
            DeltaDrawSizeAdjust_RecResize(ref recTitle, recTitle_default, fltDeltaDrawSize);
            DeltaDrawSizeAdjust_RecResize(ref recGame, recGame_default, fltDeltaDrawSize);
            DeltaDrawSizeAdjust_RecResize(ref recScore, recScore_default, fltDeltaDrawSize);
            DeltaDrawSizeAdjust_RecResize(ref recResize, recResize_default, fltDeltaDrawSize);
            DeltaDrawSizeAdjust_RecResize(ref recFrame, recFrame_default, fltDeltaDrawSize);

            ptScoreBoardCenter.X = ptScoreBoardCenter_default.X * fltDeltaDrawSize ;
            ptScoreBoardCenter.Y = ptScoreBoardCenter_default.Y * fltDeltaDrawSize ;

            ptNewTilesStart.X = ptNewTilesStart_default.X * fltDeltaDrawSize;
            ptNewTilesStart.Y = ptNewTilesStart_default.Y * fltDeltaDrawSize;
        }


        static float fltDeltaDrawSize = 1f;
        public static float DeltaDrawSize
        {
            get { return fltDeltaDrawSize; }
            set
            {
                fltDeltaDrawSize = value;
                float fltFontSize = fltSize_Default * fltDeltaDrawSize;
                fnt = new Font(fnt.FontFamily, fltFontSize, fnt.Style);

                RecSizes_Set();

                TileSize_Set();
                Board.Update();
            }
        }


        void szForm_default_Initialize()
        {
            int intWidthSides_Extra = (int)recFrame_default.Width / 2;

            szForm_default.Width = 2 * intWidthSides_Extra + recFrame_default.Width;
            szForm_default.Height = recFrame_default.Height + 1000;

            recFrame_default = new RectangleF(intWidthSides_Extra, 0, recFrame_default.Width, recFrame_default.Height);
            recGame_default = new RectangleF(intWidthSides_Extra + 69, recGame_default.Top, recGame_default.Width, recGame_default.Height);
            recGrab_default = new RectangleF(207 + intWidthSides_Extra, recGrab_default.Top, recGrab_default.Width, recGrab_default.Height);
            recChallengeTimer_default = new RectangleF(235 + intWidthSides_Extra, recChallengeTimer_default.Top, recChallengeTimer_default.Width, recChallengeTimer_default.Height);
            ptNewTilesStart_default.X = intWidthSides_Extra + 418;
            recResize_default = new RectangleF(3 + intWidthSides_Extra, recResize_default.Top, recResize_default.Width, recResize_default.Height);
            recTitle_default = new RectangleF(160 + intWidthSides_Extra, recTitle_default.Top, recTitle_default.Width, recTitle_default.Height);
            ptScoreBoardCenter_default.X 
                = ptNewTilesStart_default.X 
                = 259 + intWidthSides_Extra;
        }

        #endregion

        #region Properties
        Bitmap _bmpNoPath = null;
        public Bitmap bmpNoPath
        {
            get { return _bmpNoPath; }
            set
            {
                _bmpNoPath = value;
                intCounter_NoPath = 12;
            }
        }

        Bitmap _bmpForm = null;
        public Bitmap bmpForm
        {
            get 
            {
                return _bmpForm;
            }
            set 
            {
                _bmpForm = value;
                DrawPerPixelForm(_bmpForm);
            }
        }


        void DrawPerPixelForm(Bitmap bmp)
        {
            if (IsDisposed) return;
            SetBitmap(bmp);
        }


        bool _bolNewTiles = false;
        bool bolNewTiles
        {
            get { return _bolNewTiles; }
            set
            {
                if (_bolNewTiles != value)
                {
                    _bolNewTiles = value;
                }
            }
        }

        static int _intScore = 0;
        static public int intScore
        {
            get { return _intScore; }
            set
            {
                _intScore = value;
                if (_intScore > intHighScore)
                    intHighScore = _intScore;
            }
        }

        Point _ptTileHighlight = new Point(-1, -1);
        Point ptTileHighLight
        {
            get { return _ptTileHighlight; }
            set
            {
                if (_ptTileHighlight.X != value.X || _ptTileHighlight.Y != value.Y)
                {
                    _ptTileHighlight = value;
                }
            }
        }

        classTile cTileMoving = null;
        public classTile TileMoving
        {
            get { return cTileMoving; }
            set
            {
                cTileMoving = value;
            }
        }

        classTile _cTileSelected = null;
        public classTile TileSelected
        {
            get { return _cTileSelected; }
            set
            {
                _cTileSelected = value;
            }
        }

        Point _ptTileSelected = new Point(-1, -1);
        Point ptTileSelected
        {
            get { return _ptTileSelected; }
            set
            {
                _ptTileSelected = value;
                if (TileInBounds(_ptTileSelected))
                    TileSelected = Board.Tiles[ptTileSelected.X, ptTileSelected.Y];
            }
        }

        #endregion
        public System.Windows.Forms.Timer tmrFlashDelay = new System.Windows.Forms.Timer();
        
        public formHex2048()
        {
            instance
                = Preferences.frmHex
                = this;
            InitializeComponent();

            Top = Screen.PrimaryScreen.Bounds.Height + 100;

            new formFlash();

            tmrFlashDelay.Interval = 2000;
            tmrFlashDelay.Tick += TmrFlashDelay_Tick;
            tmrFlashDelay.Enabled = true;

            LocationChanged += FormHex2048_LocationChanged;
            SizeChanged += FormHex2048_SizeChanged;
            Activated += FormHex2048_Activated;
        }



        #region Menus
        private void CMnu_Popup(object sender, EventArgs e)
        {
            cMnu.MenuItems.Clear();

            cMnu.MenuItems.Add(new MenuItem("New Game", mnuGame_New_Click));
            cMnu.MenuItems.Add(new MenuItem("Timer", mnuTimer_click));
            cMnu.MenuItems.Add(new MenuItem("Quit", mnuQuit_Click));
       //     cMnu.MenuItems.Add(new MenuItem("create challenge", mnuCreateChallenge_Click));
        }
        void mnuCreateChallenge_Click(object sender, EventArgs e)
        {
            classChallenge.intChallengesSolved += 1;
            System.Diagnostics.Debug.Print("ChallengesSolved:" + classChallenge.intChallengesSolved.ToString());
            classChallenge.Challenge_Next();
        }

        void mnuTimer_click(object sender, EventArgs e)
        {
            formGetTimer frmGetTimer = new formGetTimer();
            if (frmGetTimer.ShowDialog() == DialogResult.OK)
            {
                dt_Start = DateTime.Now;
                lngTimerDelay_Minutes = frmGetTimer.lngDelayMinutes;
            }
        }

        void mnuGame_New_Click(object sender, EventArgs e)
        {   
            
            if (eFSM != enuHex2048_FSM.Game_Over )
            {
                if (MessageBox.Show("are you sure you want to quit this game?", "new game?", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;
            }


            Game_New();
        }

        void mnuQuit_Click(object sender, EventArgs e)
        {
            if (eFSM != enuHex2048_FSM.idle)
            {
                bolQuitWhenIdle = true;
                return;
            }

            Quit();
        }
        public void Quit()
        {
            tmr.Enabled = false;
            Preferences.Save();
            Dispose();
        }
        #endregion

        #region Helper_Functions
        static int DirReverse(int dir) { return (dir + 3) % 6; }

        public static int RND_GetInt(int intMax) { return RND_GetInt(0, intMax); }
        public static int RND_GetInt(int intMin, int intMax)
        {
            int intRange = intMax - intMin;

            int intRND = (int)((double)intRange * 1024 * rnd.NextDouble()) % intRange;
            return intMin + intRND;
        }

        public static Point move(Point pt, int dir)
        {
            if (lstPtMoveDir.Count == 0)
            {
                List<Point> lstEven = new List<Point>();
                {
                    lstEven.Add(new Point(0, -1));
                    lstEven.Add(new Point(1, -1));
                    lstEven.Add(new Point(1, 0));
                    lstEven.Add(new Point(0, 1));
                    lstEven.Add(new Point(-1, 0));
                    lstEven.Add(new Point(-1, -1));
                }
                lstPtMoveDir.Add(lstEven);

                List<Point> lstOdd = new List<Point>();
                {
                    lstOdd.Add(new Point(0, -1));
                    lstOdd.Add(new Point(1, 0));
                    lstOdd.Add(new Point(1, 1));
                    lstOdd.Add(new Point(0, 1));
                    lstOdd.Add(new Point(-1, 1));
                    lstOdd.Add(new Point(-1, 0));
                }
                lstPtMoveDir.Add(lstOdd);
            }

            return new Point(pt.X + lstPtMoveDir[pt.X % 2][dir].X, pt.Y + lstPtMoveDir[pt.X % 2][dir].Y);
        }

        public static int DirFromMove(Point ptStart, Point ptEnd)
        {
            for (int intDir = 0; intDir < 6; intDir ++)
            {
                Point ptMove = move(ptStart, intDir);

                if (ptMove.X == ptEnd.X && ptMove.Y == ptEnd.Y)
                    return intDir;
            }
            MessageBox.Show("fuck!!!");
            return -1;
        }

        #endregion

        #region Events

        private void Tmr_Tick(object sender, EventArgs e)
        {
            if (IsDisposed)
            {
                tmr.Enabled = false;
                return;
            }
            intTimerCounter = (intTimerCounter + 1) % intTimerCounterMax;

            switch (eFSM)
            {
                case enuHex2048_FSM.Tiles_GatherLike:
                    {

                    }
                    break;

                case enuHex2048_FSM.Form_Moving:
                    {
                        Left = (int)(MousePosition.X - ptGrab.X);
                        Top = (int)(MousePosition.Y - ptGrab.Y);
                    }
                    break;

                case enuHex2048_FSM.Form_Resizing:
                    {
                        DeltaDrawSize = (float)(MousePosition.Y - Top + 31 * DeltaDrawSize) / recFrame_default.Height;
                    }
                    break;

                case enuHex2048_FSM.Game_Over:
                    {
                        if (bolQuitWhenIdle)
                            Quit();
                        if (intTimerCounter % 3 == 0)
                            intCounter_GameOver = (intCounter_GameOver + 1) % 2;
                        DrawPerPixelForm(bmpGame_Over[intCounter_GameOver]);
                        if (!IsDisposed)
                            Show();
                        return;
                    }

                case enuHex2048_FSM.Tiles_GatherLike_Flash:
                    {
                        classFlare.Animate();
                    }
                    break;

                case enuHex2048_FSM.Tile_Moving:
                    {    // set next step to value of moving piece
                        Board.Tiles[lstPtMove[0].X, lstPtMove[0].Y].Value = Board.Tiles[ptTileSelected.X, ptTileSelected.Y].Value;

                        // set old value to zero
                        Board.Tiles[ptTileSelected.X, ptTileSelected.Y].Value = 0;

                        int intDirMove = DirFromMove(ptTileSelected, lstPtMove[0]);

                        // point to new tile
                        ptTileSelected = lstPtMove[0];

                        // remove sstep from list
                        lstPtMove.RemoveAt(0);

                        Board.Tiles[ptTileSelected.X, ptTileSelected.Y].Impulse(ptTileSelected, intDirMove);

                        if (lstPtMove.Count == 0)
                            eFSM = enuHex2048_FSM.Tiles_GatherLike;
                    }
                    break;

                case enuHex2048_FSM.Tiles_Adding_Animate:
                    {
                        if (intTimerCounter % 1 == 0)
                        {
                            if (intCounter_AddTile > 0)
                                AddTiles_Animate();
                            else
                                eFSM = enuHex2048_FSM.Tiles_GatherLike;
                        }
                    }
                    break;

                case enuHex2048_FSM.Tiles_RemoveFinal:
                    {
                        if (!classStreamer.Animate())
                            eFSM = enuHex2048_FSM.Tiles_GatherLike;
                    }
                    break;

                default:
                    {
                        if (intTimerCounter % 3 == 0)
                            bolHighlight = !bolHighlight;
                    }
                    break;
            }

            DateTime currentDate = DateTime.Now;

            long elapsedTicks = currentDate.Ticks - dt_Start.Ticks;

            elapsedSpan = new TimeSpan(elapsedTicks);

            if (lngTimerDelay_Minutes >= 0 && elapsedSpan.TotalMinutes > lngTimerDelay_Minutes)
            {
                if (eFSM == enuHex2048_FSM.idle)
                    Quit();
                else
                    bolQuitWhenIdle = true;
            }

            DrawGame();

            classChallenge.tmr_Tick(sender,e);

            if (!IsDisposed)
                Show();

        }

        
        private void TmrFlashDelay_Tick(object sender, EventArgs e)
        {
            tmrFlashDelay.Enabled = false;
            formFlash.instance.Hide();
            formFlash.instance.Refresh();
            formFlash.instance.Dispose();
            bolIgnoreFormChanges = true;
            Location = Preferences.ptLocation_Load;
            Refresh();
            Show();
            Refresh();

            bolIgnoreFormChanges = false;
            tmr.Enabled = true;
        }
        private void FormHex2048_Activated(object sender, EventArgs e)
        {
            if (bolActivated) return;
            bolActivated = true;

            recFrame_default = new RectangleF(new Point(0, 0), Properties.Resources.Frame.Size);
            WorkingDirectory = System.IO.Directory.GetCurrentDirectory() + "\\";

            classSparkle.InitFile();
            szForm_default_Initialize();

            Board.Populate();

            DeltaDrawSize = (float)Screen.PrimaryScreen.WorkingArea.Height / recFrame_default.Height;

            bmpFrameSource.MakeTransparent(bmpFrameSource.GetPixel(0, 0));

            ContextMenu = cMnu;
            cMnu.Popup += CMnu_Popup;
            //cMnu_Build();

            tmr.Interval = 100;
            tmr.Tick += Tmr_Tick;

            MouseMove += formHex2048_MouseMove;
            MouseDown += FormHex2048_MouseDown;
            MouseUp += FormHex2048_MouseUp;
            MouseClick += formHex2048_MouseClick;
            bolIgnoreFormChanges = true;
            {
                TileSize_Set();
                if (!Preferences.Load())
                    eFSM = enuHex2048_FSM.Game_New;
                else
                {
                    DrawGame();
                    eFSM = enuHex2048_FSM.idle;
                }
                if (!IsDisposed)
                Show();
            }
            bolIgnoreFormChanges = false;
        }

  

        private void FormHex2048_LocationChanged(object sender, EventArgs e)
        {
            if (bolIgnoreFormChanges) return;
            semFormChanges.WaitOne();
            Preferences.Save();
            semFormChanges.Release();
        }

        private void FormHex2048_SizeChanged(object sender, EventArgs e)
        {
            if (bolIgnoreFormChanges) return;
            semFormChanges.WaitOne();
            TileSize_Set();
            DrawGame();
            if (!bolIgnoreFormChanges)
                Preferences.Save();
            semFormChanges.Release();
        }
        public static Point ptTileClicked = new Point();
        public void formHex2048_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (eFSM == enuHex2048_FSM.idle)
                {
                    Point ptTile
                        = ptTileClicked
                        = Tile_FromPoint(new Point(e.X, e.Y));
                    if (TileInBounds(ptTile))
                    {
                        if (Board.Tiles[ptTile.X, ptTile.Y].Value != 0)
                        {
                            ptTileSelected = ptTile;
                            classTile cTile = Board.Tiles[ptTile.X, ptTile.Y];

                            TileSelected.Radiate((double)cTile.Value / (double)classTile.numColors);
                        }
                        else
                        {
                            if (TileInBounds(ptTileSelected))
                                if (Board.Tiles[ptTileSelected.X, ptTileSelected.Y].Value != 0)
                                {
                                    TileMoving = Board.Tiles[ptTileSelected.X, ptTile.Y];
                                    eFSM = enuHex2048_FSM.Tile_Moving;
                                }
                        }
                    }
                }
            }
        }

     public  void formHex2048_MouseMove(object sender, MouseEventArgs e)
        {
            Point ptMouse = new Point(e.X, e.Y);
            bolMouseOverGrab = PointOverGrab(ptMouse);
            bolMouseOverResize = PointOverResize(ptMouse);
            ptTileHighLight = Tile_FromPoint(ptMouse);
        }

        public void FormHex2048_MouseUp(object sender, MouseEventArgs e)
        {
            if (eFSM == enuHex2048_FSM.Form_Moving)
                eFSM = enuHex2048_FSM.idle;
            else if (eFSM == enuHex2048_FSM.Form_Resizing)
                eFSM = enuHex2048_FSM.idle;
            else
                formHex2048_MouseClick(sender, e);
        }


        bool PointOverResize(PointF pt) 
        {
            return Math3.classMath3.PointIsInsideARectangle(pt, recResize); 
        }
        bool PointOverGrab(PointF pt) { return Math3.classMath3.PointIsInsideARectangle(pt, recGrab); }

        public void FormHex2048_MouseDown(object sender, MouseEventArgs e)
        {
            PointF ptMouse = new Point(e.X, e.Y);
            if (MouseButtons == MouseButtons.Left && bolMouseOverGrab)
            {
                eFSM = enuHex2048_FSM.Form_Moving;
                ptGrab = new Point((int)ptMouse.X, (int)ptMouse.Y);
                return;
            }

            if (MouseButtons == MouseButtons.Left && bolMouseOverResize)
            {
                eFSM = enuHex2048_FSM.Form_Resizing;
                return;
            }

        }

        #endregion

        #region MoveSelectedTiles
        List<Point> MoveSelectedTile_BFS()
        {
            Point ptDestination = ptTileHighLight;
            Point ptStart = ptTileSelected;

            int[,] intSeen = new int[szGame.Width, szGame.Height];
            // init intSeen
            for (int intX = 0; intX < szGame.Width; intX++)
                for (int intY = 0; intY < szGame.Height; intY++)
                    intSeen[intX, intY] = -1;

            List<Point> lstQ = new List<Point>();
            lstQ.Add(ptStart);
            intSeen[ptStart.X, ptStart.Y] = 0;
            int intStepsTaken = 0;
            while (lstQ.Count > 0)
            {
                Point ptTile = lstQ[0];
                lstQ.RemoveAt(0);
                intStepsTaken = intSeen[ptTile.X, ptTile.Y];

                for (int intDirCounter = 0; intDirCounter < 6; intDirCounter++)
                {
                    Point ptNeaghbour = move(ptTile, intDirCounter);

                    if (TileInBounds(ptNeaghbour))
                    {
                        if (intSeen[ptNeaghbour.X, ptNeaghbour.Y] < 0)
                        {  // BFS has not seen neaghbour
                            if (Board.Tiles[ptNeaghbour.X, ptNeaghbour.Y].Value == 0)
                            { // neaghbour is not occupied by a colored tile
                                intSeen[ptNeaghbour.X, ptNeaghbour.Y] = intStepsTaken + 1;

                                if (ptNeaghbour.X == ptDestination.X && ptNeaghbour.Y == ptDestination.Y)
                                {  // we have found a path to the destination
                                    lstQ.Clear();
                                    goto validPath;
                                }
                                else
                                {
                                    lstQ.Add(ptNeaghbour);
                                }
                            }

                        }
                    }
                }
            }

            return new List<Point>();

        validPath:
            intStepsTaken = intSeen[ptDestination.X, ptDestination.Y];

            List<Point> lstSteps = new List<Point>();
            lstSteps.Add(ptDestination);
            Point ptCurrent = ptDestination;
            while (!(ptCurrent.X == ptStart.X && ptCurrent.Y == ptStart.Y))
            {
                for (int intDirCounter = 0; intDirCounter < 6; intDirCounter++)
                {
                    Point ptNeaghbour = move(ptCurrent, intDirCounter);
                    if (TileInBounds(ptNeaghbour))
                    {
                        if (intSeen[ptNeaghbour.X, ptNeaghbour.Y] == intStepsTaken - 1)
                        {
                            lstSteps.Add(ptNeaghbour);
                            ptCurrent = ptNeaghbour;
                            intStepsTaken--;
                            break;
                        }
                    }
                }
            }

            lstSteps.Reverse();
            lstSteps.RemoveAt(0);

            return lstSteps;
        }
        void MoveSelectedTile()
        {
            lstPtMove = MoveSelectedTile_BFS();
            if (lstPtMove.Count <= 0)
            {
                bmpNoPath_Draw();
                eFSM = enuHex2048_FSM.idle;
            }
        }

        void bmpNoPath_Draw()
        {
            /*
            Point ptDestination =TileCenter( ptTileHighLight);
            Point ptStart = TileCenter( ptTileSelected);
            /*/
            Point ptRecGameTL= new Point((int)recGame.Left, (int)recGame.Top);
            Point ptStart = classMath3.SubTwoPoints(TileCenter( ptTileSelected), ptRecGameTL);
            Point ptDestination = classMath3.SubTwoPoints(TileCenter(ptTileHighLight), ptRecGameTL);
            //*/
            Bitmap bmpTemp = new Bitmap((int)recGame.Width, (int)recGame.Height);
            Color clrTransparent = Color.DarkRed;
            using (Graphics g = Graphics.FromImage(bmpTemp))
            {
                g.FillRectangle(new SolidBrush(clrTransparent), new RectangleF(0, 0, bmpTemp.Width, bmpTemp.Height));
                Pen pLine = new Pen(Color.LightGray, 10);
                g.DrawLine(pLine, ptStart, ptDestination);

                classRadialCoordinate cRad = new classRadialCoordinate(ptStart, ptDestination);
                cRad.Magnitude = szTile.Width * 1.2;
                cRad.Radians += (2.0 * Math.PI)/3.0;
                Point ptArrow1 = (classMath3.AddTwoPoints(cRad.toPoint(), ptDestination));
                g.DrawLine(pLine, ptDestination, ptArrow1);

                cRad.Radians += (2.0 * Math.PI)/3.0;
                Point ptArrow2 = (classMath3.AddTwoPoints(cRad.toPoint(), ptDestination));
                g.DrawLine(pLine, ptDestination, ptArrow2);

                string strNoPath = "No Path";
                Font fntNoPath = new Font(fnt.FontFamily.Name, fnt.Size * 3);
                Size szText = TextRenderer.MeasureText(strNoPath, fntNoPath);
                Point ptTextCenter = new Point (bmpTemp.Width/2, bmpTemp.Height/2);
                Point ptTextTL = new Point(ptTextCenter.X - szText.Width / 2,
                                           ptTextCenter.Y - szText.Height / 2);
                if (ptTextTL.X < 0)
                    ptTextTL.X = 0;
                if (ptTextTL.X + szText.Width > bmpTemp.Width)
                    ptTextTL.X = bmpTemp.Width - szText.Width;

                if (ptTextTL.Y < 0)
                    ptTextTL.Y = 0;
                if (ptTextTL.Y + szText.Height > bmpTemp.Height)
                    ptTextTL.Y = bmpTemp.Height - szText.Height;

                int intShift = 2;
                g.DrawString(strNoPath, fntNoPath, Brushes.Black, new Point(ptTextTL.X - intShift, ptTextTL.Y - intShift));
                g.DrawString(strNoPath, fntNoPath, Brushes.Red, ptTextTL);
                g.DrawString(strNoPath, fntNoPath, Brushes.Yellow, new Point(ptTextTL.X  + intShift, ptTextTL.Y + intShift));

            }

            bmpTemp.MakeTransparent(clrTransparent);
            bmpNoPath = bmpTemp;
        }

        #endregion

        #region AddNewTiles
        void AddTiles_Animate()
        {
            for (int intTileCounter = 0; intTileCounter < lstTilesNew.Count; intTileCounter++)
                lstTilesNew[intTileCounter].Move();
            intCounter_AddTile--;
            if (intCounter_AddTile <= 0)
            {
                for (int intTileCounter = 0; intTileCounter < lstTilesNew.Count; intTileCounter++)
                {
                    classTile_New cTile = lstTilesNew[intTileCounter];
                    Board.Tiles[cTile.pt.X, cTile.pt.Y].Value = cTile.Value;
                }
            }
        }

        void AddRndTiles()
        {
            int intNumNewTiles =  6 + RND_GetInt(2);
            List<Point> lstEmptyTiles = EmptyTiles();

            if (intNumNewTiles > lstEmptyTiles.Count)
                intNumNewTiles = lstEmptyTiles.Count;
            lstTilesNew.Clear();

            do
            {
                int intNew = RND_GetInt(lstEmptyTiles.Count);
                classTile_New cTile = new classTile_New(lstEmptyTiles[intNew]);
                lstEmptyTiles.RemoveAt(intNew);
                lstTilesNew.Add(cTile);
                intNumNewTiles--;
            } while (intNumNewTiles > 0);
            intCounter_AddTile = classTile_New.intNumSteps;
            eFSM = enuHex2048_FSM.Tiles_Adding_Animate;
        }

        #endregion

        #region Game
        void Game_New()
        {
            Board.Populate();
            bool[] arrBool = new bool[classTile.numColors];
            lstFlareTrigger.Clear();
            lstFlareTrigger.AddRange(arrBool.ToList<bool>());
            classChallenge.intChallengesSolved = 0;
            classChallenge.eState = classChallenge.enuChallengeState.idle;
            DrawGame();
            intScore = 0;
            eFSM = enuHex2048_FSM.Tiles_Adding_Init;
        }
        void DrawGame_Over()
        {
            DrawGame();
            Bitmap bmpGameMap = new Bitmap(bmpForm);

            Color[] clrGame_Over = { Color.Black, Color.Red };

            string strGame_Over = "Game Over";
            Font fntGame_Over = new Font("Ms sans-serif", 32);
            Size szGame_Over = TextRenderer.MeasureText(strGame_Over, fntGame_Over);

            // determine the region of the game bitmap which draws the game map

            double dblARGame_Over = (double)szGame_Over.Height / (double)szGame_Over.Width;
            Size szDraw = new Size((int)recGame.Width, (int)(recGame.Width * dblARGame_Over));
            Rectangle recDraw = new Rectangle(new Point((int)(recGame.Left + (recGame.Width - szDraw.Width) / 2),
                                                        (int)((recGame.Height - szDraw.Height) / 2)), szDraw);

            for (int intDrawGame_Over = 0; intDrawGame_Over <= bmpGame_Over.GetUpperBound(0); intDrawGame_Over++)
            {
                // draw text on separate bitmap  in the difference colors specified by clrGame_Over array
                Bitmap bmpText = new Bitmap(szGame_Over.Width, szGame_Over.Height);
                Color clrTransparent = Color.Purple;
                using (Graphics g = Graphics.FromImage(bmpText))
                {
                    g.FillRectangle(new SolidBrush(clrTransparent), new RectangleF(0, 0, bmpText.Width, bmpText.Height));
                    g.DrawString(strGame_Over, fntGame_Over, new SolidBrush(clrGame_Over[intDrawGame_Over]), new Point());
                }
                bmpText.MakeTransparent(clrTransparent);

                // create a new bitmap copy of the game screen
                Bitmap bmpTemp = new Bitmap(bmpGameMap);

                // draw text over game screen
                using (Graphics g = Graphics.FromImage(bmpTemp))
                    g.DrawImage(bmpText, recDraw, new Rectangle(0, 0, bmpText.Width, bmpText.Height), GraphicsUnit.Pixel);

                bmpGame_Over[intDrawGame_Over] = bmpTemp;
            }
        }
        #endregion 

        #region Tiles
        static void TileSize_Set()
        {
            int intTileSize = (int)(recGame.Width/ (szGame.Width));
            szTile = new Size(intTileSize, intTileSize);

            ptDrawGameGap = new Point((int)(recGame.Width - szGame.Width * intTileSize) / 2,
                                      (int)(recGame.Height - szGame.Height * intTileSize) / 2);
        }

        public static bool TileInBounds(Point pt)
        {
            return pt.X >= 0 && pt.X < szGame.Width && pt.Y >= 0 && pt.Y < szGame.Height;
        }
        List<Point> EmptyTiles()
        {
            List<Point> lstRetVal = new List<Point>();
            for (int intX = 0; intX < szGame.Width; intX++)
                for (int intY = 0; intY < szGame.Height; intY++)
                    if (Board.Tiles[intX, intY].Value == 0)
                        lstRetVal.Add(new Point(intX, intY));
            return lstRetVal;
        }

        public static Point TileTL(Point ptTile)
        {
            int intX = (int)recGame.Left + ptDrawGameGap.X
                            + ptTile.X * szTile.Width;
            int intY = (int)recGame.Top + ptDrawGameGap.Y
                            + ptTile.Y * szTile.Width
                            + (ptTile.X % 2 == 1 ? szTile.Width / 2 : 0);

            return new Point(intX, intY);
        }

        public static Point TileCenter(Point ptTile)
        {
            Point ptTL = TileTL(ptTile);

            return new Point(ptTL.X + szTile.Width / 2, ptTL.Y + szTile.Height / 2);
        }

        public Point Tile_FromPoint(Point pt)
        {
            int intX = (pt.X + szTile.Width - (int)recGame.Left - ptDrawGameGap.X ) / szTile.Width - 1;
            int intY = ((pt.Y + szTile.Height - (int)recGame.Top - ptDrawGameGap.Y ) - (intX % 2 == 1 ? szTile.Width / 2 : 0)) / szTile.Width - 1;

            if (intX >= szGame.Width)
                intX = -1;
            if (intY >= szGame.Height)
                intY = -1;

            return new Point(intX, intY);
        }

     
        #endregion

        #region Draw

        void Highlight(ref Graphics g, Point ptTile, bool bolHighlight)
        {
            if (!TileInBounds(ptTile)) return;

            classTile cTile = Board.Tiles[ptTile.X, ptTile.Y];

            switch (eFSM)
            {
                case enuHex2048_FSM.Tiles_RemoveFinal:
                    {

                    }
                    break;

                case enuHex2048_FSM.Tiles_GatherLike_Flash:
                    {
                        if (!bolHighlight)
                        {
                            g.FillEllipse(new SolidBrush(classTile.clrArray[0]), new RectangleF(cTile.ptfTL, szTile));
                        }
                    }
                    break;

                default:
                    {
                        Color clr = classTile.clrArray[0];

                        if (bolHighlight)
                        {
                            if ((ptTile.X == ptTileSelected.X && ptTile.Y == ptTileSelected.Y) || eFSM == enuHex2048_FSM.Tiles_GatherLike_Flash)
                                clr = Color.Black;
                            else
                                clr = Color.DarkGray;
                        }
                        else
                            clr = classTile.clrArray[0];

                        g.DrawEllipse(new Pen(clr, 2), new RectangleF(cTile.ptfTL, szTile));
                    }
                    break;
            }
        }


        void DrawGame()
        {
            Bitmap bmpTemp = new Bitmap((int)szForm.Width, (int)szForm.Height);

            Color clrTransparent = Color.Azure;
            Graphics g = Graphics.FromImage(bmpTemp);
            {
                g.FillRectangle(new SolidBrush(clrTransparent), new RectangleF(new Point(), new Size(bmpTemp.Width, bmpTemp.Height)));
                DrawGameBoard(ref g);
                //bmpTemp.Save(@"c:\debug\HexBoard.png");
                
                switch (eFSM)
                {
                    case enuHex2048_FSM.Tiles_GatherLike_Flash:
                        {
                            if (intTimerCounter % 3 == 0)
                                bolHighlight = !bolHighlight;
                            for (int intTileCounter = 0; intTileCounter < lstTilesGathered.Count; intTileCounter++)
                                Highlight(ref g, lstTilesGathered[intTileCounter], bolHighlight);
                            intCounter_FlashTiles--;
                            if (intCounter_FlashTiles <= 0)
                                eFSM = enuHex2048_FSM.Tiles_RemoveLike;
                        }
                        break;
                }

                Highlight(ref g, ptTileHighLight, bolHighlight);
                Highlight(ref g, ptTileSelected, bolHighlight);

                Rectangle recSource = new Rectangle(0, 0, bmpFrameSource.Width, bmpFrameSource.Height);
                RectangleF recDestination = recFrame;
                g.DrawImage(bmpFrameSource, recDestination, recSource, GraphicsUnit.Pixel);

                if (bolMouseOverGrab)
                    g.FillRectangle(Brushes.Gray, recGrab);
                else if (bolMouseOverResize)
                    g.FillRectangle(Brushes.DarkGray, recResize);

                if (intCounter_AddTile > 0)
                    DrawAddNewTiles(ref g);
                DrawScore(ref g);
                DrawHighScore(ref g);
                DrawNoPath(ref g);
                DrawTimer(ref g);
                DrawChallengeTimer(ref g);
                //bmpTemp.Save(@"c:\debug\bmpTemp.png");
                classFlare.Animate();
                classSparkle.Animate(ref g);
                classAddScore.Animate(ref g);
            }
            g.Dispose();

            bmpTemp.MakeTransparent(clrTransparent);
            bmpForm = bmpTemp;
        }

        void DrawTimer(ref Graphics g)
        {
            if (lngTimerDelay_Minutes >= 0)
            {
                double dblTemp = 0;
                double dblTotalSeconds_Remaining
                    = dblTemp
                    = (double)lngTimerDelay_Minutes * 60 - elapsedSpan.TotalSeconds;
                double dblHours = Math.Floor(dblTotalSeconds_Remaining / 3600.0);
                dblTotalSeconds_Remaining -= 3600 * dblHours;
                double dblMinutes = Math.Floor(dblTotalSeconds_Remaining / 60.0);
                double dblSeconds = dblTotalSeconds_Remaining - dblMinutes * 60;

                string strTime = dblHours.ToString("00.") + ":" + dblMinutes.ToString("00") + ":" + dblSeconds.ToString("00");

                Size szTime = TextRenderer.MeasureText(strTime, fnt);
                Bitmap bmpTime = new Bitmap(szTime.Width, szTime.Height);
                RectangleF recSource = new RectangleF(0, 0, bmpTime.Width, bmpTime.Height);
                RectangleF recDes;
                using (Graphics gTime = Graphics.FromImage(bmpTime))
                {
                    gTime.FillRectangle(Brushes.Black, recSource);
                    gTime.DrawString(strTime,
                                     fnt,
                                     dblTemp > 60
                                             ? Brushes.Green
                                             : dblTemp > 10
                                                       ? Brushes.Yellow
                                                       : Brushes.Red,
                                     new Point(0, 0));
                }

                double dblAR_Rec = (double)recGrab.Height / (double)recGrab.Width;
                double dblAR_Time = (double)bmpTime.Height / (double)bmpTime.Width;

                if (dblAR_Time > dblAR_Rec)
                {
                    // height is limiting
                    SizeF szDest = new SizeF((float)(recGrab.Height / dblAR_Time), recGrab.Height);
                    recDes = new RectangleF(recGrab.Left + (recGrab.Width - szDest.Width) / 2,
                                            recGrab.Top,
                                            szDest.Width,
                                            szDest.Height);
                }
                else
                {
                    // width is limiting
                    SizeF szDest = new SizeF(recGrab.Width, (float)(recGrab.Width * dblAR_Time));
                    recDes = new RectangleF(recGrab.Left,
                                            recGrab.Top + (recGrab.Height - szDest.Height) / 2,
                                            szDest.Width,
                                            szDest.Height);
                }
                //bmpTime.Save(@"c:\debug\bmpTime.bmp");
                g.DrawImage(bmpTime, recDes, recSource, GraphicsUnit.Pixel);
            }
                
        }

        void DrawNoPath(ref Graphics g)
        {
            intCounter_NoPath--;
            if (intCounter_NoPath < 0) return;

            Rectangle recSource = new Rectangle(0, 0, bmpNoPath.Width, bmpNoPath.Height);
            g.DrawImage(bmpNoPath, recGame, recSource, GraphicsUnit.Pixel);

        }


        void DrawHighScore(ref Graphics g)
        {
            string strHighScore = intTimerCounter > intTimerCounterMax / 2
                                              ? intHighScore.ToString()
                                              : "High Score";
            Size szScore = TextRenderer.MeasureText(strHighScore, fnt);

            double dblAR_Board = (double)recTitle.Height / (double)recTitle.Width;
            double dblAR_Text = (double)szScore.Height / (double)szScore.Width;
            Size szDraw = new Size();
            if (dblAR_Text > dblAR_Board)
            { // height is limit
                szDraw.Width = (int)(recTitle.Height / dblAR_Text);
                szDraw.Height = (int)recTitle.Height;
            }
            else
            { // width is limit
                szDraw.Height = (int)(recTitle.Width * dblAR_Text);
                szDraw.Width = (int)recTitle.Width;
            }

            Bitmap bmpText = new Bitmap(szScore.Width, szScore.Height);
            SolidBrush sbrBackground = new SolidBrush(intScore == intHighScore
                                                                ? Color.Yellow
                                                                : Color.Black);
            using (Graphics gText = Graphics.FromImage(bmpText))
            {
                gText.FillRectangle(sbrBackground,
                                    new RectangleF(0, 0, bmpText.Width, bmpText.Height));
                gText.DrawString(strHighScore,
                                 fnt,
                                 new SolidBrush(intScore == intHighScore
                                                          ? Color.Black
                                                          : Color.Yellow),
                                 new Point());
            }

            Point ptDraw = new Point((int)(recTitle.Left + (recTitle.Width - szDraw.Width) / 2),
                                     (int)(recTitle.Top + (recTitle.Height - szDraw.Height) / 2));

            Rectangle recDest = new Rectangle(ptDraw, szDraw);
            Rectangle recSource = new Rectangle(new Point(), bmpText.Size);
            g.FillRectangle(sbrBackground, recTitle);
            g.DrawImage(bmpText, recDest, recSource, GraphicsUnit.Pixel);

        }
        void DrawChallengeTimer(ref Graphics g)
        {
            string strChallengeTimer = classChallenge.tsChallengeTimeRemaining.Minutes.ToString("00")  + ":"+ classChallenge.tsChallengeTimeRemaining.Seconds.ToString("00");
            Size szChallengeTimer = TextRenderer.MeasureText(strChallengeTimer, fnt);

            double dblAR_Board = (double)recChallengeTimer.Height / (double)recChallengeTimer.Width;
            double dblAR_Text = (double)szChallengeTimer.Height / (double)szChallengeTimer.Width;
            Size szDraw = new Size();
            if (dblAR_Text > dblAR_Board)
            { // height is limit
                szDraw.Width = (int)(recChallengeTimer.Height / dblAR_Text);
                szDraw.Height = (int)recChallengeTimer.Height;
            }
            else
            { // width is limit
                szDraw.Height = (int)(recChallengeTimer.Width * dblAR_Text);
                szDraw.Width = (int)recChallengeTimer.Width;
            }

            Bitmap bmpText = new Bitmap(szChallengeTimer.Width, szChallengeTimer.Height);
            Color clrTransparent = Color.Purple;
            SolidBrush sbrBackground = new SolidBrush(clrTransparent);
            SolidBrush sbrForeground = null;
            if (classChallenge.tsChallengeTimeRemaining.Minutes > 7 )
            {
                sbrForeground = new SolidBrush(Color.Green);
            }
            else if (classChallenge.tsChallengeTimeRemaining.Minutes > 4)
            {
                sbrForeground = new SolidBrush(Color.Blue);
            }
            else if (classChallenge.tsChallengeTimeRemaining.Minutes >1)
            {
                sbrForeground = new SolidBrush(Color.Black);
            }
            else
            {
                sbrForeground = new SolidBrush(Color.Red);
            }


            using (Graphics gText = Graphics.FromImage(bmpText))
            {
                gText.FillRectangle(sbrBackground,
                                    new RectangleF(0, 0, bmpText.Width, bmpText.Height));
                gText.DrawString(strChallengeTimer,
                                 fnt,
                                 sbrForeground,
                                 new Point());
            }
            bmpText.MakeTransparent(clrTransparent);
            //bmpText.Save(@"c:\debug\bmpText.png");

            Point ptDraw = new Point((int)(recChallengeTimer.Left + (recChallengeTimer.Width - szDraw.Width) / 2),
                                     (int)(recChallengeTimer.Top + (recChallengeTimer.Height - szDraw.Height) / 2));

            Rectangle recDest = new Rectangle(ptDraw, szDraw);
            Rectangle recSource = new Rectangle(new Point(), bmpText.Size);
            //g.FillRectangle(sbrBackground, recChallengeTimer);
            g.DrawImage(bmpText, recDest, recSource, GraphicsUnit.Pixel);

        }
        void DrawScore(ref Graphics g)
        {
            Bitmap bmpScoreBoard = new Bitmap(Properties.Resources.scoreboard);

            string strScore = "Score:" + intScore.ToString();
            Size szScore = TextRenderer.MeasureText(strScore, fnt);

            double dblAR_Board = (double)recScore_default.Height / (double)recScore_default.Width;
            double dblAR_Text = (double)szScore.Height / (double)szScore.Width;
            Size szDraw = new Size();
            if (dblAR_Text > dblAR_Board)
            { // height is limit
                szDraw.Width = (int)(recScore_default.Height / dblAR_Text);
                szDraw.Height = (int)recScore_default.Height;
            }
            else
            { // width is limit
                szDraw.Height = (int)(recScore_default.Width * dblAR_Text);
                szDraw.Width = (int)recScore_default.Width;
            }

            Bitmap bmpText = new Bitmap(szScore.Width, szScore.Height);
            Color clrSample = bmpScoreBoard.GetPixel(bmpScoreBoard.Width / 2, bmpScoreBoard.Height / 2);
            using (Graphics gText = Graphics.FromImage(bmpText))
            {
                gText.FillRectangle(new SolidBrush(clrSample), new RectangleF(0, 0, bmpText.Width, bmpText.Height));
                gText.DrawString(strScore, fnt, Brushes.Black, new Point());
            }

            Point ptDraw = new Point((int)(recScore_default.Left + (recScore_default.Width - szDraw.Width) / 2),
                                     (int)(recScore_default.Top + (recScore_default.Height - szDraw.Height) / 2));

            Rectangle recDest = new Rectangle(ptDraw, szDraw);
            Rectangle recSource = new Rectangle(new Point(), bmpText.Size);
            using (Graphics gScoreBoard = Graphics.FromImage(bmpScoreBoard))
            {
                gScoreBoard.DrawImage(bmpText, recDest, recSource, GraphicsUnit.Pixel);
            }
            bmpScoreBoard.MakeTransparent(bmpScoreBoard.GetPixel(0, 0));
            RectangleF recScoreBoard = new RectangleF(ptScoreBoardCenter.X - (int)(bmpScoreBoard.Width * .5 * DeltaDrawSize),
                                                      ptScoreBoardCenter.Y - (int)(bmpScoreBoard.Height * .5 * DeltaDrawSize),
                                                      (float)(bmpScoreBoard.Width * DeltaDrawSize),
                                                      (float)(bmpScoreBoard.Height * DeltaDrawSize));
            g.DrawImage(bmpScoreBoard, recScoreBoard, new Rectangle(0, 0, bmpScoreBoard.Width, bmpScoreBoard.Height), GraphicsUnit.Pixel);
        }

        void DrawAddNewTiles(ref Graphics g)
        {
            Size szDraw = new Size(szTile.Width - 2, szTile.Height - 2);
            for (int intTileCounter = 0; intTileCounter < lstTilesNew.Count; intTileCounter++)
            {
                classTile_New cTile = lstTilesNew[intTileCounter];
                Point ptTL = new Point((int)(cTile.ptGraphics.X + 1), 
                                       (int)(cTile.ptGraphics.Y + 1));
                g.FillEllipse(new SolidBrush(classTile.clrArray[cTile.Value]), new RectangleF(ptTL, szDraw));

                int intTile = cTile.Value;
                string strText =classTile.TileText_Get(intTile);
                Size szText = TextRenderer.MeasureText(strText, fnt);
                Point ptCenter = new Point(ptTL.X + szDraw.Width / 2, ptTL.Y + szDraw.Height / 2);
                g.DrawString(strText, fnt, Brushes.Black, new Point(ptCenter.X - szText.Width / 2, ptCenter.Y - szText.Height / 2));
            }
        }

        void DrawGameBoard(ref Graphics g)
        {
            classChallenge.Animate();
            g.FillRectangle(Brushes.Black, recGame);
            Font fntDebug = new Font("ms sans-serif", 8);
            for (int intX = 0; intX < szGame.Width; intX++)
                for (int intY = 0; intY < szGame.Height; intY++)
                    Board.Tiles[intX, intY].Animate(ref g);
        }
        #endregion

        #region Tiles_GatherLike

        void Tiles_GatherLike()
        {
            for (int intX = 0; intX < szGame.Width; intX++)
            {
                for (int intY = 0; intY < szGame.Height; intY++)
                {
                    lstTilesGathered = Tiles_GatherLike_BFS(new Point(intX, intY));
                    if (lstTilesGathered.Count >= 4)
                    {
                        eFSM = enuHex2048_FSM.Tiles_GatherLike_Flash;
                        bolTilesGathered = true;
                        return;
                    }
                }
            }

            if (bolNewTiles || bolTilesGathered)
            {
                eFSM = (EmptyTiles().Count == 0) ? enuHex2048_FSM.Game_Over : enuHex2048_FSM.idle;
                bolNewTiles = false;
                bolTilesGathered = false;
            }
            else
            {
                eFSM = enuHex2048_FSM.Tiles_Adding_Init;
            }
        }

        List<Point> Tiles_GatherLike_BFS(Point ptSeed)
        {
            if (!TileInBounds(ptSeed)) return new List<Point>();
            int[,] intSeen = new int[szGame.Width, szGame.Height];
            int intSeedValue = Board.Tiles[ptSeed.X, ptSeed.Y].Value;
            if (intSeedValue <= 0) return new List<Point>();

            List<Point> lstQ = new List<Point>();
            lstQ.Add(ptSeed);

            List<Point> lstRetVal = new List<Point>();

            while (lstQ.Count > 0)
            {
                Point ptTest = lstQ[0];
                intSeen[ptTest.X, ptTest.Y] = 1;
                lstQ.RemoveAt(0);
                int intTileValue = Board.Tiles[ptTest.X, ptTest.Y].Value;
                if (intTileValue == intSeedValue)
                {
                    if (!lstRetVal.Contains(ptTest))
                    {
                        lstRetVal.Add(ptTest);
                        for (int intDir = 0; intDir < 6; intDir++)
                        {
                            Point ptNeaghbour = move(ptTest, intDir);
                            if (TileInBounds(ptNeaghbour))
                                if (intSeen[ptNeaghbour.X, ptNeaghbour.Y] == 0)
                                    if (!lstQ.Contains(ptNeaghbour) && !lstRetVal.Contains(ptNeaghbour))
                                        lstQ.Add(ptNeaghbour);
                        }
                    }
                }
            }
            return lstRetVal;
        }
        int intTilesGathered_ThisScore = 0;
        void Tiles_RemoveLike()
        {
            Point ptKeep = new Point(-1, -1);
            // use lstNewTiles & move tile to set location of joined tiles
            if (lstTilesGathered.Contains(ptTileSelected))
                ptKeep = ptTileSelected;
            else
            {
                for (int intNewTilesCounter = 0; intNewTilesCounter < lstTilesNew.Count; intNewTilesCounter++)
                {
                    Point ptTileNew = lstTilesNew[intNewTilesCounter].pt;
                    if (lstTilesGathered.Contains(ptTileNew))
                    {
                        ptKeep = ptTileNew;
                        break;
                    }
                }
            }

            int intNewValue = Board.Tiles[ptKeep.X, ptKeep.Y].Value + 2;
            intTilesGathered_ThisScore += lstTilesGathered.Count - 3;

            int intAddScore = (int)Math.Pow(2, intNewValue - 1)*intTilesGathered_ThisScore;
            new classAddScore(intAddScore, TileCenter(ptKeep));

            while (lstTilesGathered.Count > 0)
            {
                Point ptRemove = lstTilesGathered[0];
                Board.Tiles[ptRemove.X, ptRemove.Y].Value = 0;
                lstTilesGathered.RemoveAt(0);
            }

            Board.Tiles[ptKeep.X, ptKeep.Y].Value = intNewValue;
            Board.Tiles[ptKeep.X, ptKeep.Y].Radiate((double)intNewValue / (double)classTile.numColors);
            if (intNewValue > 11)
            {
                ptFinalTileRemoved = ptKeep;
                classStreamer.ptStart = TileCenter(ptKeep);
                eFSM = enuHex2048_FSM.Tiles_RemoveFinal;
            }
            else if (!lstFlareTrigger[Board.Tiles[ptKeep.X, ptKeep.Y].Value])
            {
                lstFlareTrigger[Board.Tiles[ptKeep.X, ptKeep.Y].Value] = true;
                classFlare.ptStart = TileCenter(ptKeep);
                eFSM = enuHex2048_FSM.Tiles_GatherLike;
            }
            else
                eFSM = enuHex2048_FSM.Tiles_GatherLike;

            Sparkles_Add(ptKeep, intNewValue * 7);
            return;
        }

        void Sparkles_Add(Point pt, int intNumSparkles)
        {
            Point ptCenter = TileCenter(pt);
            for (int intSparkleCounter = 0; intSparkleCounter < intNumSparkles; intSparkleCounter++)
            {
                int intVelocity = classRND.Get_Int(10, 15);
                Point ptVelocity = classRND.Get_Point(new classMinMax(-intVelocity, intVelocity),
                                                      new classMinMax(-intVelocity, intVelocity));
                new classSparkle(ptCenter, ptVelocity);
            }
        }
        #endregion 
    }

    public class classTile_New
    {
        public PointF ptGraphics = new PointF();
        public Point pt = new Point();
        public int Value = 0;

        public static int intNumSteps = 10;
        Point ptMove = new Point();

        static bool bolRightOrb = false;
        public classTile_New(Point pt)
        {
            bolRightOrb = !bolRightOrb;
            this.pt = pt;

            ptGraphics = new Point((int)(formHex2048.recFrame.Left + formHex2048.recFrame.Width / 2 ),
                                   (int)(formHex2048.recFrame.Top ));

            ptGraphics = formHex2048.ptNewTilesStart;

            ptGraphics.X -= formHex2048.szTile.Width / 2;
            ptGraphics.Y -= formHex2048.szTile.Height / 2;
            Point ptEnd = formHex2048.TileCenter(pt);

            Value = formHex2048.RND_GetInt(4) + 1;

            ptMove.X = (int)(ptEnd.X - ptGraphics.X) / intNumSteps;
            ptMove.Y = (int)(ptEnd.Y - ptGraphics.Y) / intNumSteps;
            
        }

        public void Move()
        {
            ptGraphics.X += (ptMove.X );
            ptGraphics.Y += (ptMove.Y );
        }
    }

    public class formFlash : PerPixelForm.PerPixelAlphaForm
    {
        public static formFlash instance = null;
        public formFlash()
        {
            instance = this;

            Bitmap bmpFlash = (Bitmap)Properties.Resources.Hex_2048_flash;
            bmpFlash.MakeTransparent(bmpFlash.GetPixel(0, 0));
            SetBitmap(bmpFlash);
            Left = (Screen.PrimaryScreen.WorkingArea.Width - bmpFlash.Width) / 2;
            Top = (Screen.PrimaryScreen.WorkingArea.Height - bmpFlash.Height) / 2;
            Show();
        }
    }

}