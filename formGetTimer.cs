using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Drawing;

namespace Hex_2048
{
    public class formGetTimer : Form
    {
        NumericUpDown nudHours = new NumericUpDown();
        NumericUpDown nudMinutes = new NumericUpDown();
        Button btnOk = new Button();
        Button btnCancel = new Button();
        Label lblColon = new Label();
        Label lblHeading = new Label();
        public formGetTimer ()
        {
            BackColor = Color.Black;
            ForeColor = Color.White;
            Controls.Add(lblHeading);
            lblHeading.AutoSize = true;
            lblHeading.Text = "Timer";

            Controls.Add(lblColon);
            lblColon.AutoSize = true;
            lblColon.Text = ":";

            Controls.Add(nudHours);
            nudHours.Minimum = 0;
            nudHours.Maximum = 99;
            nudHours.Value = 0; 
            nudHours.MouseWheel += new MouseEventHandler(this.ScrollHandlerFunction);

            Controls.Add(nudMinutes);
            nudMinutes.Minimum = 0;
            nudMinutes.Maximum = 59;
            nudMinutes.Value = 10;
            nudMinutes.MouseWheel += new MouseEventHandler(this.ScrollHandlerFunction);

            Controls.Add(btnOk);
            btnOk.Text = "Ok";
            btnOk.AutoSize = true;
            btnOk.Click += BtnOk_Click;
                
            Controls.Add(btnCancel);
            btnCancel.Text = "Cancel";
            btnCancel.AutoSize = true;
            btnCancel.Click += BtnCancel_Click;

            FormBorderStyle = FormBorderStyle.None;
            TopMost = true;

            Activated += FormGetTimer_Activated;
        }
  
        private void ScrollHandlerFunction(object sender, MouseEventArgs e)
        {
            NumericUpDown nudSender = (NumericUpDown)sender;
            HandledMouseEventArgs handledArgs = e as HandledMouseEventArgs;
            handledArgs.Handled = true;
            int intNewValue = (int)(nudSender.Value + (handledArgs.Delta > 0 ? 1 : -1));

            if (intNewValue < nudSender.Minimum)
                nudSender.Value = nudSender.Minimum;
            else if (intNewValue > nudSender.Maximum)
                nudSender.Value = nudSender.Maximum;
            else
                nudSender.Value = intNewValue;

        }

        bool bolActivated = false;
        private void FormGetTimer_Activated(object sender, EventArgs e)
        {
            if (bolActivated) return;
            bolActivated = true;
            nudHours.Width
                = nudMinutes.Width = 45;

            btnOk.Size = TextRenderer.MeasureText(btnOk.Text, btnOk.Font);
            btnCancel.Size = TextRenderer.MeasureText(btnCancel.Text, btnCancel.Font);

            lblHeading.Location = new System.Drawing.Point(5, 5);
            nudHours.Location = new System.Drawing.Point(lblHeading.Left, lblHeading.Bottom);
            lblColon.Location = new System.Drawing.Point(nudHours.Right, nudHours.Top);
            nudMinutes.Location = new System.Drawing.Point(lblColon.Right, nudHours.Top);
            
            btnOk.Location = new System.Drawing.Point(nudMinutes.Right - btnOk.Width, nudMinutes.Bottom);
            btnCancel.Location = new System.Drawing.Point(btnOk.Left - btnCancel.Width, btnOk.Top);
            
            Width = btnOk.Right;
            Height = btnOk.Bottom;

            Location = new Point(formHex2048.instance.Left + (int)(formHex2048.instance.Width - Width) / 2,
                                 formHex2048.instance.Top + (int)formHex2048.recFrame.Top + (int)(formHex2048.recFrame.Height - Height) / 2);
            
        }

        public long lngDelayMinutes = 0;
        private void BtnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            lngDelayMinutes = (long)(60 * nudHours.Value
                              +  nudMinutes.Value);
            if (lngDelayMinutes < 1)
                lngDelayMinutes = 1;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
