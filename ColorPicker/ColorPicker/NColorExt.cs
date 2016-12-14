using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ColorPicker
{
    public static class NColorExt
    {
        public static NGraphics.Color ToNColor(this Color color)
        {
            return new NGraphics.Color(color.R, color.G, color.B, color.A);
        }
        public static Color ToColor(this NGraphics.Color color)
        {
            return new Color(color.R / 255d, color.G / 255d, color.B / 255d, color.A / 255d);
        }
    }
}
