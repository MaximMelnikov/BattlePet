using System;
using UnityEngine;

public class Classification  {
    [Flags]
    public enum Type
    {
        Physical = 0x0,        
        Spiritual = 0x1,
        Death = 0x2,
        Fire = 0x3,
        Frost = 0x4,
        Electric = 0x5,
        Acid = 0x6,
        All = Physical | Spiritual | Death | Fire | Frost | Electric | Acid
    }
    
    public enum Quality
    {
        Normal,
        Rare,
        Epic,
        Legendary
    }

    public static Color QualityColor(Quality quality)
    {
        Color color = Color.white;
        switch (quality)
        {
            case Quality.Normal:
                return color = hexToColor("1eff00");
            case Quality.Rare:
                return color = hexToColor("0070dd");
            case Quality.Epic:
                return color = hexToColor("a335ee");
            case Quality.Legendary:
                return color = hexToColor("ff8000");
            default:
                return Color.white;
        }
    }

    public static string colorToHex(Color32 color)
    {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return hex;
    }

    public static Color hexToColor(string hex)
    {
        hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
        hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
        byte a = 255;//assume fully visible unless specified in hex
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        //Only use alpha if the string has enough characters
        if (hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        }
        return new Color32(r, g, b, a);
    }
}