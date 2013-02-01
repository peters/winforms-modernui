using System.Drawing;
using System.Collections.Generic;

namespace MetroFramework
{
    public sealed class MetroPens
    {
        private static Dictionary<string, Pen> metroPens;
        private static Pen GetSavePen(string key, Color color)
        {
            if (metroPens == null)
                metroPens = new Dictionary<string, Pen>();

            if (!metroPens.ContainsKey(key))
                metroPens.Add(key, new Pen(color, 1f));

            return metroPens[key].Clone() as Pen;
        }

        public static Pen Black
        {
            get
            {
                return GetSavePen("Black", MetroColors.Black);
            }
        }

        public static Pen White
        {
            get
            {
                return GetSavePen("White", MetroColors.White);
            }
        }

        public static Pen Silver
        {
            get
            {
                return GetSavePen("Silver", MetroColors.Silver);
            }
        }

        public static Pen Blue
        {
            get
            {
                return GetSavePen("Blue", MetroColors.Blue);
            }
        }

        public static Pen Green
        {
            get
            {
                return GetSavePen("Green", MetroColors.Green);
            }
        }

        public static Pen Lime
        {
            get
            {
                return GetSavePen("Lime", MetroColors.Lime);
            }
        }

        public static Pen Teal
        {
            get
            {
                return GetSavePen("Teal", MetroColors.Teal);
            }
        }

        public static Pen Orange
        {
            get
            {
                return GetSavePen("Orange", MetroColors.Orange);
            }
        }

        public static Pen Brown
        {
            get
            {
                return GetSavePen("Brown", MetroColors.Brown);
            }
        }

        public static Pen Pink
        {
            get
            {
                return GetSavePen("Pink", MetroColors.Pink);
            }
        }

        public static Pen Magenta
        {
            get
            {
                return GetSavePen("Magenta", MetroColors.Magenta);
            }
        }

        public static Pen Purple
        {
            get
            {
                return GetSavePen("Purple", MetroColors.Purple);
            }
        }

        public static Pen Red
        {
            get
            {
                return GetSavePen("Red", MetroColors.Red);
            }
        }

        public static Pen Yellow
        {
            get
            {
                return GetSavePen("Yellow", MetroColors.Yellow);
            }
        }
    }
}
