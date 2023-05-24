using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using Math3;
using System.Runtime.CompilerServices;

namespace Hex_2048
{
    public class classChallenge
    {
        static int _intNumLevels = -1;
        static public int NumLevels
        {
            get { return _intNumLevels; }
            set { _intNumLevels = value; }
        }

        static int _intNumBranches = -1;
        static public int NumBranches
        {
            get { return _intNumBranches; }
            set { _intNumBranches = value; }
        }

        static int intMinutesToSolveChallenge = 1;
        static int MinutesToSolveChallenge
        {
            get { return intMinutesToSolveChallenge; }
            set { intMinutesToSolveChallenge = value;}
        }

        static int _intChallengesSolved = 0;
        public static int intChallengesSolved
        {
            get
            {
                return _intChallengesSolved;
            }
            set
            {
                _intChallengesSolved = value;
            }
        }
 

        static public void Challenge_Next()
        {
            lstLevels.Clear();
            lstTiles.Clear();
            int intNumBranches = 1;
            int intDegree = 0; 

            int intOddShift =  classRND.Get_Int(0, int.MaxValue) % 2;     // 0, 1
            int intRnd = 1 + classRND.Get_Int(0, int.MaxValue) % 3;         //  1, 2, 3 


            //if (true)
            //{
            //    intDegree = 2;
            //    lstLevels.Add(3);  // # 2, 4, 8,
            //    lstLevels.Add(5);
            //    lstLevels.Add(3);  // # 2, 4, 8,
            //    intNumBranches = 3;
            //    MinutesToSolveChallenge = 30;
            //}   else       
            if (intChallengesSolved < 2)
            {
                intDegree = 0;
                lstLevels.Add(intOddShift + intRnd);  // # 2, 4, 8, 16
                intNumBranches = 1;
                MinutesToSolveChallenge = 20;
            }
            else if (intChallengesSolved <15)
            {
                intDegree = 1;
                lstLevels.Add(intOddShift + intRnd);  // # 2, 4, 8, 16
                intNumBranches = 1;
                MinutesToSolveChallenge = 10;
            }
            else if (intChallengesSolved <50)
            {
                intDegree = 2;
                lstLevels.Add(1+ intOddShift);  // # 2, 4, 8, 16
                lstLevels.Add(3+ intOddShift);  // # 2, 4, 8, 16
                intNumBranches = 2;
                MinutesToSolveChallenge = 10;
            }            
            else if (intChallengesSolved <100)
            {
                intDegree = 3;
                lstLevels.Add(1+ intOddShift);  // # 2, 4, 8, 16
                lstLevels.Add(3 + intOddShift);  // # 2, 4, 8, 16
                intNumBranches = 2;
                MinutesToSolveChallenge = 10;
            }
            else if (intChallengesSolved < 500)
            {
                intDegree = 4;
                lstLevels.Add(1);  // # 4, 8, 16
                lstLevels.Add(3);  // # 2, 4, 8, 16 -> shifted down/up to math
                lstLevels.Add(5);  // #  4, 8, 16
                intNumBranches = 3;
                MinutesToSolveChallenge = 10;
            }            
            else if (intChallengesSolved < 1000)
            {
                intDegree = 5;
                lstLevels.Add(2);  // # 4, 8, 16
                lstLevels.Add(4);  // # 2, 4, 8, 16 -> shifted down/up to math
                lstLevels.Add(6);  // #  4, 8, 16
                intNumBranches = 4;
                MinutesToSolveChallenge = 10;
            }            
            else 
            {
                intDegree = 6;
                lstLevels.Add(intOddShift + intRnd);  // # 4, 8, 16
                lstLevels.Add(intOddShift +2+ intRnd);  // #  4, 8, 16
                intNumBranches = 4;
                MinutesToSolveChallenge = 10;
            }

            lstLevelsCopy.Clear();
            lstLevelsCopy.AddRange(lstLevels);

            intNumBranches_Build
                = NumBranches
                = intNumBranches;
            intNumLevels_Build
                = NumLevels
                = lstLevels.Count;
            intNumTries = 0;
            eState = enuChallengeState.building;
            dtChallengStart = DateTime.Now;

            ArrTiles = null;
            ptCenter = new Point(-1, -1);

            
            string strMsg = "----created- degree:" + intDegree.ToString();
            for (int intCounter = 0; intCounter < lstLevels.Count; intCounter++)
            {
                int intLevel = lstLevels[intCounter];
                int intNumeral = (int)Math.Pow(2, intLevel-1);
                strMsg += " " + intNumeral.ToString();
            }

            System.Diagnostics.Debug.Print(strMsg);
        }




        public enum enuChallengeState { idle, building, ready };
        static enuChallengeState _eState = enuChallengeState.idle;
        static Semaphore semState = new Semaphore(1, 1);
        public static enuChallengeState eState
        {
            get { return _eState; }
            set
            {
                if (_eState != value)
                {
                    _eState = value;
                    switch(eState)
                    {
                        case enuChallengeState.idle:
                            {

                            }
                            break;

                        case enuChallengeState.building:
                            {
                            }
                            break;

                        case enuChallengeState.ready:
                            {

                            }
                            break;
                    }
                }
            }
        }
   

        public static Size szGame {  get { return formHex2048.szGame; } }

        public static classTile[,] ArrTiles = null;
        static List<classTile> lstTiles = new List<classTile>();
        static Point ptCenter = new Point(-1, -1);

        //List<Point> EmptyTiles()
        //{
        //    List<Point> lstRetVal = new List<Point>();
        //    for (int intX = 0; intX < szGame.Width; intX++)
        //        for (int intY = 0; intY < szGame.Height; intY++)
        //            if (Board.Tiles[intX, intY].Value == 0)
        //                lstRetVal.Add(new Point(intX, intY));
        //    return lstRetVal;
        //}


        //static List<classTile_New> lstTiles = new List<classTile_New>();
        static int intNumTries = 0;
        static int intNumTries_Max = 64;
        public static void tmr_Tick(object sender, EventArgs e)
        {
            switch(eState)
            {
                case enuChallengeState.idle:
                    {
                        Challenge_Next();
                    }
                    break;

                case enuChallengeState.building:
                    {
                        Challenge_Build();
                    }
                    break;

                case enuChallengeState.ready:
                    {
                        Challenge_TestComplete();
                    }
                    break;
            }            
        }

        static void Challenge_Build()
        {
            intNumTries++;
            if (intNumTries >intNumTries_Max)
            {
                ;
            }

            if (ArrTiles == null)
            { // init tiles
                ArrTiles = new classTile[szGame.Width, szGame.Height];
                return;
            }
            else if (ptCenter.X <0)
            { // pick a center 
                ptCenter.X = classRND.Get_Int(0, szGame.Width);
                ptCenter.Y = classRND.Get_Int(0, szGame.Height);
                return;
            }
            else
            {
                if (lstLevels.Count > 0)
                {
                    int intLevel = lstLevels[0];
                    lstLevels.RemoveAt(0);
                    Challenge_Build_AddLeg(intLevel);
                }
                else if (NumBranches > 0)
                {
                    int intIndexRnd = classRND.Get_Int(0, int.MaxValue) % lstLevelsCopy.Count;
                    int intLevel = lstLevelsCopy[intIndexRnd];
                    NumBranches -= 1;
                    Challenge_Build_AddLeg(intLevel);
                }
                else
                    eState = enuChallengeState.ready;
            }
        }

        static List<Point> Challenge_Build_getTilesSameValue(Point pt, int intValue)
        {
            List<Point> lstRetVal = new List<Point>();
            
            for (int intX = 0; intX < szGame.Width; intX++)
            {
                for (int intY = 0; intY < szGame.Height; intY++)
                {
                    Point ptNeighbour = new Point(intX, intY);
                    classTile cTileExistant = ArrTiles[ptNeighbour.X, ptNeighbour.Y];
                    if (cTileExistant != null && cTileExistant.Value == intValue)
                    {
                        if (!lstRetVal.Contains(ptNeighbour))
                        lstRetVal.Add(ptNeighbour);
                        // reject it and its neighbours
                        for (int intDirCounter = 0; intDirCounter < 6; intDirCounter++)
                        {
                            Point ptAdj = formHex2048.move(ptNeighbour, intDirCounter);
                            if (!lstRetVal.Contains(ptAdj))
                            if (ptAdj.X >=0 && ptAdj.X < szGame.Width && ptAdj.Y >=0 && ptAdj.Y < szGame.Height)
                            {
                                    lstRetVal.Add(ptAdj);
                            }
                        }
                    }
                }
            }

            return lstRetVal;
        }

        static void Challenge_Build_AddLeg(int intLevel)
        {
            Point ptStart = ptCenter;
            int intError = 0;
            int intErrorMax = 32;
            int intPointsToFind = 3;
            List<Point> lstRejects = Challenge_Build_getTilesSameValue(ptCenter, intLevel);


            Point ptNew0 = new Point();
            do
            {
                intError++;
                ptNew0 = getEmptyNeighbour(ptStart, lstRejects);
                if (ptNew0.X != ptCenter.X || ptNew0.Y != ptCenter.Y)
                    if (!lstRejects.Contains(ptNew0))
                    {
                        classTile cTileNew = new classTile(ptNew0);
                        ArrTiles[ptNew0.X, ptNew0.Y] = cTileNew;
                        ArrTiles[ptNew0.X, ptNew0.Y].Value = intLevel;

                        lstTiles.Add(cTileNew);

                        ptStart = ptNew0;
                        intPointsToFind--;
                    }

            }
            while (intError < intErrorMax && intPointsToFind > 0);

            if (intError >= intErrorMax)
                eState = enuChallengeState.idle;
            else
                NumBranches -= 1;
        }

        static Point getEmptyNeighbour(Point ptStart)
        {
            return getEmptyNeighbour(ptStart, new List<Point>());
        }
        static Point getEmptyNeighbour(Point ptStart, List<Point> lstPt_Reject)
        {
            List<int> lstDir = new List<int>();
            for (int i = 0; i < 6; i++)
                lstDir.Add(i);

            do
            {
                int intRnd = classRND.Get_Int(0, lstDir.Count);
                int intDir = lstDir[intRnd];
                lstDir.RemoveAt(intRnd);
                Point ptNew = formHex2048.move(ptStart, intDir);
                if (ptNew.X >= 0 && ptNew.X < szGame.Width)
                {
                    if (ptNew.Y >= 0 && ptNew.Y <= szGame.Height)
                    {
                        if (!(ptNew.X == ptCenter.X && ptNew.Y == ptCenter.Y))
                        {
                            if (ptNew.X >= 0 && ptNew.X < szGame.Width && ptNew.Y >= 0 && ptNew.Y < szGame.Height)
                            {
                                if (ArrTiles[ptNew.X, ptNew.Y] == null)
                                {
                                    for (int intDirCounter = 0; intDirCounter < 6; intDirCounter++)
                                    {
                                        Point ptNeighbour = formHex2048.move(ptNew, intDirCounter);

                                        if (ptNeighbour.X >= 0 && ptNeighbour.X < szGame.Width && ptNeighbour.Y >= 0 && ptNeighbour.Y < szGame.Height)
                                        {
                                            if (!lstPt_Reject.Contains(ptNeighbour))
                                            {
                                                return ptNew;

                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            } while (lstDir.Count > 0);

            return ptStart;
        }

        static int intAnimationCounter = 0;
        public static bool Highlight
        {
            get { return intAnimationCounter >3; }
        }
        static public void Animate()
        { intAnimationCounter = (intAnimationCounter + 1) % 10; }

        static TimeSpan _tsChallengeTimeRemaining;
        public static TimeSpan tsChallengeTimeRemaining
        {
            get { return _tsChallengeTimeRemaining; }
            set 
            { 
                _tsChallengeTimeRemaining = value;
                System.Diagnostics.Debug.Print(_tsChallengeTimeRemaining.Minutes.ToString("00") + ":" + _tsChallengeTimeRemaining.Seconds.ToString("00"));
            }
        }

        static void Challenge_TestComplete()
        {
            DateTime dtNow = DateTime.Now;
            TimeSpan tsElapsed = dtNow.Subtract(dtChallengStart);
            TimeSpan tsNeededToElapse = new TimeSpan(0, 10, 0);

            tsChallengeTimeRemaining = tsNeededToElapse - tsElapsed;
            if (tsChallengeTimeRemaining.TotalMilliseconds <0)
            {
                int intASubScore = -2000;
                Point ptTileSubScore = formHex2048.ptTileClicked;
                new classAddScore(intASubScore, formHex2048.TileCenter(ptTileSubScore));

                eState = enuChallengeState.idle;
                return;
            }


            for (int intRowCounter = 0; intRowCounter < szGame.Height; intRowCounter++)
            {
                for (int intColumnCounter = 0; intColumnCounter < szGame.Width; intColumnCounter++)
                {
                    classTile cTile_Challenge = ArrTiles[intColumnCounter, intRowCounter];
                    if (cTile_Challenge != null)
                    {
                        classTile cTile_Board = Board.Tiles[intColumnCounter, intRowCounter];
                        if (cTile_Board == null)
                            return;
                        if (cTile_Board.Value != cTile_Challenge.Value)
                            return;
                    }
                }
            }

            // challenge solved 
            int intAddScore = ScoreValue();
            Point ptTile = formHex2048.ptTileClicked;
            new classAddScore(intAddScore,formHex2048.TileCenter(ptTile));

            intChallengesSolved++;
            eState = enuChallengeState.idle;

        }

        static int ScoreValue()
        {
            int intRetVal = (intNumBranches_Build * intNumLevels_Build ) * 1000;
            return intRetVal;
        }
        static List<int> lstLevels = new List<int>();
        static List<int> lstLevelsCopy = new List<int>();
  
        static int intNumBranches_Build=0, intNumLevels_Build = 0;
        static DateTime dtChallengStart = DateTime.Now;
        static bool bolFirstChallenge = true;
  
    }
}
