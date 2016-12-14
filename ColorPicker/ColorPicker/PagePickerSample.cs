using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Xamarin.Forms;

namespace ColorPicker
{
    public class PagePickerSample : ContentPage
    {
        public PagePickerSample()
        {
            var colors = new List<NGraphics.Color>
            {
                Color.FromRgb(225, 137, 64).ToNColor(),
                Color.FromRgb(188, 77, 51).ToNColor(),
                Color.FromRgb(30, 140, 202).ToNColor(),
                Color.FromRgb(96, 148, 63).ToNColor(),
                Color.FromRgb(221, 174, 42).ToNColor(),
                Color.FromRgb(114, 167, 218).ToNColor(),
                Color.FromRgb(161, 191, 101).ToNColor(),
                Color.FromRgb(129, 101, 162).ToNColor(),
                Color.FromRgb(70, 161, 185).ToNColor()
            };

            var rl = new RelativeLayout();
            var bvColor = new BoxView();
            var cp = new ViewColorPicker(colors);

            cp.ColorChanged += i => bvColor.Color = colors[i].ToColor();

            rl.Children.Add(cp,
                Constraint.RelativeToParent(p => 0),
                Constraint.RelativeToParent(p => 0),
                Constraint.RelativeToParent(p => p.Height > p.Width ? p.Width : p.Height),
                Constraint.RelativeToParent(p => p.Height > p.Width ? p.Width : p.Height));

            rl.Children.Add(bvColor,
                Constraint.RelativeToParent(p => p.Height > p.Width ? p.Width / 2 - 50 : (p.Width + p.Height) / 2 - 50),
                Constraint.RelativeToParent(p => p.Height > p.Width ? (p.Width + p.Height) / 2 - 50 : p.Height / 2 - 50),
                Constraint.RelativeToParent(p => 100),
                Constraint.RelativeToParent(p => 100));


            Content = rl;
        }
    }
}
