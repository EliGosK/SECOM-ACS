using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSI.EPPlus
{
    [Flags]
    public enum ExcelBorderPosition
    {
        Top = 1,
        Bottom = 2,
        Left = 4,
        Right = 8,
        All = Top | Bottom | Left | Right
    }

    [Flags]
    public enum CopyCellOptions
    {
        Value = 1,
        FontStyle = 2,
        Fill = 4,
        All = Value | FontStyle | Fill
    }
}
