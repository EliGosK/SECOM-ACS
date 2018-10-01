using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSI.EPPlus
{
    public static class ExcelPackageExtensions
    {
        public static void SetBorder(this Border b, ExcelBorderPosition borderPositions, ExcelBorderStyle borderStyle, System.Drawing.Color color)
        {
            if ((borderPositions & ExcelBorderPosition.Top) > 0)
            {
                b.Top.Style = borderStyle;
                if (borderStyle != ExcelBorderStyle.None)
                {
                    b.Top.Color.SetColor(color);
                }
            }

            if ((borderPositions & ExcelBorderPosition.Right) > 0)
            {
                b.Right.Style = borderStyle;
                if (borderStyle != ExcelBorderStyle.None)
                {
                    b.Right.Color.SetColor(color);
                }
            }

            if ((borderPositions & ExcelBorderPosition.Bottom) > 0)
            {
                b.Bottom.Style = borderStyle;
                if (borderStyle != ExcelBorderStyle.None)
                {
                    b.Bottom.Color.SetColor(color);
                }
            }

            if ((borderPositions & ExcelBorderPosition.Left) > 0)
            {
                b.Left.Style = borderStyle;
                if (borderStyle != ExcelBorderStyle.None)
                {
                    b.Left.Color.SetColor(color);
                }
            }
        }
    }
}
