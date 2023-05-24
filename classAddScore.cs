using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hex_2048
{
    public class classAddScore
    {
        static List<classAddScore> lstAddScore = new List<classAddScore>();

        Point ptLocation = new Point();
        Point ptVelocity = new Point();
        Bitmap bmpText = null;
        int intAddScore = 0;
        int intStep = 0;
        static Font fnt = new Font("ms sans-serif", 76);
        static Color clrTransparent = Color.Azure;
        static SolidBrush brTransparent = new SolidBrush(clrTransparent);

        const int NumSteps = 30;
        public classAddScore(int intScore, Point pt)
        {
            intAddScore = intScore;
            ptLocation.X =(int)( pt.X );
            ptLocation.Y = (int)(pt.Y );

            ptVelocity.X = ((int)formHex2048.ptScoreBoardCenter.X  - ptLocation.X) / NumSteps;
            ptVelocity.Y = ((int)formHex2048.ptScoreBoardCenter.Y - ptLocation.Y) / NumSteps;
            intStep = 0;

            string strText = "+" + intScore.ToString();

            Size szText = TextRenderer.MeasureText(strText, fnt);
            int intShift = 2;
            szText.Width += 2*intShift;
            szText.Height += 2*intShift;

            bmpText = new Bitmap(szText.Width, szText.Height);

            using (Graphics g = Graphics.FromImage(bmpText))
            {
                g.FillRectangle(brTransparent, new RectangleF(0, 0, bmpText.Width, bmpText.Height));
                g.DrawString(strText, fnt, Brushes.Black, new Point(0, 0));
                g.DrawString(strText, fnt, Brushes.Yellow, new Point(intShift, intShift));
            }
            bmpText.MakeTransparent(clrTransparent);
            lstAddScore.Add(this);
        }

        public static void Animate(ref Graphics g)
        {
            for (int intScoreCounter = lstAddScore.Count-1; intScoreCounter >= 0 ; intScoreCounter--)
            {
                classAddScore cScore = lstAddScore[intScoreCounter];
                cScore.ptLocation.X += (int)(cScore.ptVelocity.X );
                cScore.ptLocation.Y += (int)(cScore.ptVelocity.Y );

                Rectangle recSource = new Rectangle(0, 0, cScore.bmpText.Width, cScore.bmpText.Height);
                double dblMid = NumSteps / 2;
                double dblMaxSize = 1.5;
                
                double dblSize = (double)((dblMid - Math.Abs(dblMid - cScore.intStep))/dblMid) *  dblMaxSize;

                Rectangle recDestination = new Rectangle((int)(cScore.ptLocation.X - (int)(dblSize * cScore.bmpText.Width / 2)),
                                                         (int)(cScore.ptLocation.Y - (int)(dblSize * cScore.bmpText.Height / 2)),
                                                         (int)(cScore.bmpText.Width * dblSize),
                                                         (int)(cScore.bmpText.Height * dblSize));

                g.DrawImage(cScore.bmpText, recDestination, recSource, GraphicsUnit.Pixel);
                cScore.intStep++;
                if (cScore.intStep >= NumSteps)
                {
                    lstAddScore.Remove(cScore);
                    formHex2048.intScore = formHex2048.intScore + cScore.intAddScore ;
                }
            }
        }
        

    }
}
