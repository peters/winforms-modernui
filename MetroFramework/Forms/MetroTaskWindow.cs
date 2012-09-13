using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MetroFramework.Animation;
using MetroFramework.Components;
using MetroFramework.Controls;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;
using MetroFramework.Native;

namespace MetroFramework.Forms
{
    public sealed class MetroTaskWindow : MetroForm
    {
        private static MetroTaskWindow singletonWindow;
        public static void ShowTaskWindow(IWin32Window parent, string title, Control userControl, int secToClose)
        {
            if (singletonWindow != null)
            {
                singletonWindow.Close();
                singletonWindow.Dispose();
                singletonWindow = null;
            }

            singletonWindow = new MetroTaskWindow(secToClose, userControl);
            singletonWindow.Text = title;

            if (parent != null && parent is IMetroForm)
            {
                singletonWindow.Theme = ((IMetroForm)parent).Theme;
                singletonWindow.Style = ((IMetroForm)parent).Style;
                singletonWindow.StyleManager = ((IMetroForm)parent).StyleManager.Clone() as MetroStyleManager;

                if (singletonWindow.StyleManager != null)
                    singletonWindow.StyleManager.OwnerForm = singletonWindow;
            }

            singletonWindow.Show(parent);
        }

        public static void ShowTaskWindow(IWin32Window parent, string text, Control userControl)
        {
            ShowTaskWindow(parent, text, userControl, 0);
        }

        public static void ShowTaskWindow(string text, Control userControl, int secToClose)
        {
            ShowTaskWindow(null, text, userControl, secToClose);
        }

        public static void ShowTaskWindow(string text, Control userControl)
        {
            ShowTaskWindow(null, text, userControl);
        }

        public static void CancelAutoClose()
        {
            if (singletonWindow != null)
                singletonWindow.CancelTimer = true;
        }

        private bool cancelTimer = false;
        public bool CancelTimer
        {
            get { return cancelTimer; }
            set { cancelTimer = value; }
        }

        private readonly int closeTime = 0;
        private int elapsedTime = 0;
        private int progressWidth = 0;
        private DelayedCall timer;

        private readonly MetroPanel controlContainer;

        public MetroTaskWindow()
        {
            controlContainer = new MetroPanel();
            Controls.Add(controlContainer);
        }

        public MetroTaskWindow(int duration, Control userControl)
            : this()
        {
            controlContainer.Controls.Add(userControl);
            userControl.Dock = DockStyle.Fill;
            closeTime = duration * 500;

            if (closeTime > 0)
                timer = DelayedCall.Start(UpdateProgress, 5);
        }


        private bool isInitialized = false;
        protected override void OnActivated(EventArgs e)
        {
            if (!isInitialized)
            {
                controlContainer.Theme = Theme;
                controlContainer.Style = Style;
                controlContainer.StyleManager = StyleManager;

                MaximizeBox = false;
                MinimizeBox = false;
                Movable = false;

                TopMost = true;
                FormBorderStyle = FormBorderStyle.FixedDialog;

                Size = new Size(400, 200);

                Taskbar myTaskbar = new Taskbar();
                switch (myTaskbar.Position)
                {
                    case TaskbarPosition.Left:
                        Location = new Point(myTaskbar.Bounds.Width + 5, myTaskbar.Bounds.Height - Height - 5);
                        break;
                    case TaskbarPosition.Top:
                        Location = new Point(myTaskbar.Bounds.Width - Width - 5, myTaskbar.Bounds.Height + 5);
                        break;
                    case TaskbarPosition.Right:
                        Location = new Point(myTaskbar.Bounds.X - Width - 5, myTaskbar.Bounds.Height - Height - 5);
                        break;
                    case TaskbarPosition.Bottom:
                        Location = new Point(myTaskbar.Bounds.Width - Width - 5, myTaskbar.Bounds.Y - Height - 5);
                        break;
                    case TaskbarPosition.Unknown:
                    default:
                        Location = new Point(Screen.PrimaryScreen.Bounds.Width - Width - 5, Screen.PrimaryScreen.Bounds.Height - Height - 5);
                        break;
                }

                controlContainer.Location = new Point(0, 60);
                controlContainer.Size = new Size(Width - 40, Height - 80);
                controlContainer.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;

                StyleManager.UpdateOwnerForm();

                isInitialized = true;

                MoveAnimation myMoveAnim = new MoveAnimation();
                myMoveAnim.Start(controlContainer, new Point(20, 60), TransitionType.EaseInOutCubic, 15);
            }

            base.OnActivated(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (SolidBrush b = new SolidBrush(MetroPaint.BackColor.Form(Theme)))
            {
                e.Graphics.FillRectangle(b, new Rectangle(Width - progressWidth, 0, progressWidth, 5));
            }
        }

        private void UpdateProgress()
        {
            if (elapsedTime == closeTime)
            {
                timer.Dispose();
                timer = null;
                Close();
                return;
            }

            elapsedTime += 5;

            if (cancelTimer)
                elapsedTime = 0;

            double perc = (double)elapsedTime / ((double)closeTime / 100);
            progressWidth = (int)((double)Width * (perc / 100));
            Invalidate(new Rectangle(0,0,Width,5));

            if (!cancelTimer)
                timer.Reset();
        }
    }
}
