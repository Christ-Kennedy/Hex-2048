using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Drawing;

namespace Hex_2048
{
  public class Preferences
    {
        const string conPreferencesFilename = "Hex2048_Preferences.bin";
        static string Filename { get { return formHex2048.WorkingDirectory + conPreferencesFilename; } }

        public static BinaryFormatter formatter = new BinaryFormatter();
        static Semaphore semIO = new Semaphore(1, 1);
        public static formHex2048 frmHex = null;
        public static Point ptLocation_Load = new Point();
        public static bool Abort = false;
        static public bool Load()
        {
            if (!System.IO.File.Exists(Filename))
                return false;
            semIO.WaitOne();
            {
                using (FileStream fs = new FileStream(Filename, FileMode.Open))
                {
                    // form dimensions
                    int intLeft = (int)formatter.Deserialize(fs);
                    int intTop = (int)formatter.Deserialize(fs);
                    float fltDeltaDrawSize = (float)formatter.Deserialize(fs);
                    frmHex.bolIgnoreFormChanges = true;
                    {
                        ptLocation_Load = new Point(intLeft, intTop);
                        formHex2048.DeltaDrawSize = fltDeltaDrawSize;
                    }
                    frmHex.bolIgnoreFormChanges = false;

                    Board.Populate();
                    // game state
                    for (int intX = 0; intX < formHex2048.szGame.Width; intX++)
                        for (int intY = 0; intY < formHex2048.szGame.Height; intY++)
                            Board.Tiles[intX, intY].Value = (int)formatter.Deserialize(fs);

                    // score 
                    formHex2048.intScore = (int)formatter.Deserialize(fs);

                    // eFSM 
                    frmHex.eFSM = (enuHex2048_FSM)(int)formatter.Deserialize(fs);

                    // flareTriggers
                    formHex2048.lstFlareTrigger.Clear();
                    for (int intFTCounter = 0; intFTCounter < classTile.numColors; intFTCounter++)
                        formHex2048.lstFlareTrigger.Add((bool)formatter.Deserialize(fs));

                    // high score
                    formHex2048.intHighScore = (int)formatter.Deserialize(fs);

                    // timer delay minutes 
                    formHex2048.lngTimerDelay_Minutes = (long)formatter.Deserialize(fs);

                    classChallenge.intChallengesSolved = (int)formatter.Deserialize(fs);

                }
            }
            semIO.Release();
            return true;
        }

        static public void Save()
        {
            if (Abort) return;

            semIO.WaitOne();
            {
                if (System.IO.File.Exists(Filename))
                    System.IO.File.Delete(Filename);

                // create filestream
                using (FileStream fs = new FileStream(Filename, FileMode.Create))
                {
                    // form dimensions
                    formatter.Serialize(fs, (int)formHex2048.instance.Location.X);
                    formatter.Serialize(fs, (int)formHex2048.instance.Location.Y);
                    formatter.Serialize(fs, (float)formHex2048.DeltaDrawSize);

                    // game state
                    for (int intX =0; intX < formHex2048.szGame.Width; intX ++)
                        for (int intY =0; intY < formHex2048.szGame.Height; intY++)
                            formatter.Serialize(fs, (int)Board.Tiles[intX, intY].Value);

                    // score
                    formatter.Serialize(fs, (int)formHex2048.intScore);

                    // FSM
                    formatter.Serialize(fs, (int)frmHex.eFSM);

                    // flareTriggers
                    for (int intFTCounter =0; intFTCounter < formHex2048.lstFlareTrigger.Count; intFTCounter++)
                        formatter.Serialize(fs, (bool)formHex2048.lstFlareTrigger[intFTCounter]);

                    // high score
                    formatter.Serialize(fs, (int)formHex2048.intHighScore);

                    // timer delay minutes
                    formatter.Serialize(fs, (long)formHex2048.lngTimerDelay_Minutes);

                    // challenges
                    formatter.Serialize(fs, (int)classChallenge.intChallengesSolved);
                }
            }
            semIO.Release();
        }
    }
}
