using System;
using System.Drawing;
using System.Drawing.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MetroFramework
{
    public enum MetroLabelSize
    {
        Small,
        Medium,
        Tall
    }

    public enum MetroLabelWeight
    {
        Light,
        Regular,
        Bold
    }

    public enum MetroLinkSize
    {
        Small,
        Medium,
        Tall
    }

    public enum MetroLinkWeight
    {
        Light,
        Regular,
        Bold
    }

    public enum MetroTextBoxSize
    {
        Small,
        Medium,
        Tall
    }

    public enum MetroTextBoxWeight
    {
        Light,
        Regular,
        Bold
    }

    public sealed class MetroFonts
    {
        private static Font GetSaveFont(string key, FontStyle style, float size)
        {
            return new Font(key, size, style, GraphicsUnit.Pixel);
        }

        public static Font DefaultLight(float size)
        {
            return GetSaveFont("Segoe UI Light", FontStyle.Regular, size);
        }

        public static Font Default(float size)
        {
            return GetSaveFont("Segoe UI", FontStyle.Regular, size);
        }

        public static Font DefaultBold(float size)
        {
            return GetSaveFont("Segoe UI", FontStyle.Bold, size);
        }

        public static Font Title
        {
            get { return DefaultLight(24f); }
        }

        public static Font Subtitle
        {
            get { return Default(14f); }
        }

        public static Font Button
        {
            get { return DefaultBold(11f); }
        }

        public static Font Tile
        {
            get { return Default(14f); }
        }

        public static Font TileCount
        {
            get { return Default(44f); }
        }

        public static Font Link(MetroLinkSize linkSize, MetroLinkWeight linkWeight)
        {
            if (linkSize == MetroLinkSize.Small)
            {
                if (linkWeight == MetroLinkWeight.Light)
                    return DefaultLight(12f);
                if (linkWeight == MetroLinkWeight.Regular)
                    return Default(12f);
                if (linkWeight == MetroLinkWeight.Bold)
                    return DefaultBold(12f);
            }
            else if (linkSize == MetroLinkSize.Medium)
            {
                if (linkWeight == MetroLinkWeight.Light)
                    return DefaultLight(14f);
                if (linkWeight == MetroLinkWeight.Regular)
                    return Default(14f);
                if (linkWeight == MetroLinkWeight.Bold)
                    return DefaultBold(14f);
            }
            else if (linkSize == MetroLinkSize.Tall)
            {
                if (linkWeight == MetroLinkWeight.Light)
                    return DefaultLight(18f);
                if (linkWeight == MetroLinkWeight.Regular)
                    return Default(18f);
                if (linkWeight == MetroLinkWeight.Bold)
                    return DefaultBold(18f);
            }

            return Default(12f);
        }

        public static Font Label(MetroLabelSize labelSize, MetroLabelWeight labelWeight)
        {
            if (labelSize == MetroLabelSize.Small)
            {
                if (labelWeight == MetroLabelWeight.Light)
                    return DefaultLight(12f);
                if (labelWeight == MetroLabelWeight.Regular)
                    return Default(12f);
                if (labelWeight == MetroLabelWeight.Bold)
                    return DefaultBold(12f);
            }
            else if (labelSize == MetroLabelSize.Medium)
            {
                if (labelWeight == MetroLabelWeight.Light)
                    return DefaultLight(14f);
                if (labelWeight == MetroLabelWeight.Regular)
                    return Default(14f);
                if (labelWeight == MetroLabelWeight.Bold)
                    return DefaultBold(14f);
            }
            else if (labelSize == MetroLabelSize.Tall)
            {
                if (labelWeight == MetroLabelWeight.Light)
                    return DefaultLight(18f);
                if (labelWeight == MetroLabelWeight.Regular)
                    return Default(18f);
                if (labelWeight == MetroLabelWeight.Bold)
                    return DefaultBold(18f);
            }

            return DefaultLight(14f);
        }

        public static Font TextBox(MetroTextBoxSize linkSize, MetroTextBoxWeight linkWeight)
        {
            if (linkSize == MetroTextBoxSize.Small)
            {
                if (linkWeight == MetroTextBoxWeight.Light)
                    return DefaultLight(12f);
                if (linkWeight == MetroTextBoxWeight.Regular)
                    return Default(12f);
                if (linkWeight == MetroTextBoxWeight.Bold)
                    return DefaultBold(12f);
            }
            else if (linkSize == MetroTextBoxSize.Medium)
            {
                if (linkWeight == MetroTextBoxWeight.Light)
                    return DefaultLight(14f);
                if (linkWeight == MetroTextBoxWeight.Regular)
                    return Default(14f);
                if (linkWeight == MetroTextBoxWeight.Bold)
                    return DefaultBold(14f);
            }
            else if (linkSize == MetroTextBoxSize.Tall)
            {
                if (linkWeight == MetroTextBoxWeight.Light)
                    return DefaultLight(18f);
                if (linkWeight == MetroTextBoxWeight.Regular)
                    return Default(18f);
                if (linkWeight == MetroTextBoxWeight.Bold)
                    return DefaultBold(18f);
            }

            return Default(12f);
        }
    }
}
