using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroFramework
{
    public sealed class MetroBrushes
    {
        private static Dictionary<string, SolidBrush> metroBrushes;
        private static SolidBrush GetSaveBrush(string key, Color color)
        {
            if (metroBrushes == null)
                metroBrushes = new Dictionary<string, SolidBrush>();

            if (!metroBrushes.ContainsKey(key))
                metroBrushes.Add(key, new SolidBrush(color));

            return metroBrushes[key].Clone() as SolidBrush;
        }

        public static SolidBrush Black
        {
            get
            {
                return GetSaveBrush("Black", MetroColors.Black);
            }
        }

        public static SolidBrush White
        {
            get
            {
                return GetSaveBrush("White", MetroColors.White);
            }
        }

        public static SolidBrush Silver
        {
            get
            {
                return GetSaveBrush("Silver", MetroColors.Silver);
            }
        }

        public static SolidBrush Blue
        {
            get
            {
                return GetSaveBrush("Blue", MetroColors.Blue);
            }
        }

        public static SolidBrush Green
        {
            get
            {
                return GetSaveBrush("Green", MetroColors.Green);
            }
        }

        public static SolidBrush Lime
        {
            get
            {
                return GetSaveBrush("Lime", MetroColors.Lime);
            }
        }

        public static SolidBrush Teal
        {
            get
            {
                return GetSaveBrush("Teal", MetroColors.Teal);
            }
        }

        public static SolidBrush Orange
        {
            get
            {
                return GetSaveBrush("Orange", MetroColors.Orange);
            }
        }

        public static SolidBrush Brown
        {
            get
            {
                return GetSaveBrush("Brown", MetroColors.Brown);
            }
        }

        public static SolidBrush Pink
        {
            get
            {
                return GetSaveBrush("Pink", MetroColors.Pink);
            }
        }

        public static SolidBrush Magenta
        {
            get
            {
                return GetSaveBrush("Magenta", MetroColors.Magenta);
            }
        }

        public static SolidBrush Purple
        {
            get
            {
                return GetSaveBrush("Purple", MetroColors.Purple);
            }
        }

        public static SolidBrush Red
        {
            get
            {
                return GetSaveBrush("Red", MetroColors.Red);
            }
        }

        public static SolidBrush Yellow
        {
            get
            {
                return GetSaveBrush("Yellow", MetroColors.Yellow);
            }
        }
    }
}
