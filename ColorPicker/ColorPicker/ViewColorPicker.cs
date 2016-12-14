using System;
using System.Collections.Generic;
using System.Linq;
using NControl.Abstractions;
using NGraphics;
using Xamarin.Forms;
using Color = NGraphics.Color;
using Point = NGraphics.Point;
using Size = NGraphics.Size;

namespace ColorPicker
{
    internal class ViewColorPicker : NControlView
    {
        public readonly List<Color> Colors;
        private double _middleX, _middleY, _sectorRad, _radius, _innerD;

        public ViewColorPicker(List<Color> colors, int selColor = 0)
        {
            _selColor = selColor;
            Colors = colors;

            var rl = new RelativeLayout();
            Content = rl;
        }

        #region SelectedColor

        private int _selColor;

        public int SelectedColor
        {
            get { return _selColor; }
            set
            {
                _selColor = value;
                ColorChanged?.Invoke(_selColor);
                Invalidate();
            }
        }

        #endregion

        #region TouchesEnded

        public override bool TouchesCancelled(IEnumerable<Point> points)
        {
            if (savedColor != -1 && (savedColor != SelectedColor))
                ColorChanged?.Invoke(savedColor);
            return true;
        }

        public override bool TouchesEnded(IEnumerable<Point> points)
        {
            if (savedColor != -1 && (savedColor != SelectedColor))
                ColorChanged?.Invoke(savedColor);
            return true;
        }

        #endregion

        #region TouchesBegan

        private int savedColor = -1;
        public override bool TouchesBegan(IEnumerable<Point> points)
        {
            var pRaw = points.First();
            var p = new Point(pRaw.X * App.ScreenDensity, pRaw.Y * App.ScreenDensity);
            var dist = DistanceBetweenTwoPoints(new Point(_middleX, _middleY), p);

            if ((dist <= Math.Min(_middleX, _middleY)) && (dist >= _innerD / 2d))
            {
                savedColor = SelectedColor;
                SetNearestSector(p);
                return true;
            }

            return false;
        }

        #endregion

        #region TouchesMoved

        public override bool TouchesMoved(IEnumerable<Point> points)
        {
            var pRaw = points.FirstOrDefault();
            var p = new Point(pRaw.X * App.ScreenDensity, pRaw.Y * App.ScreenDensity);
            var dist = DistanceBetweenTwoPoints(new Point(_middleX, _middleY), p);

            if ((dist <= _middleX) && (dist >= _innerD / 2d))
            {
                SetNearestSector(p);
            }
            return base.TouchesMoved(points);
        }

        #endregion

        #region SetNearestSector

        private void SetNearestSector(Point p)
        {
            if (sectorPoints == null)
            {
                sectorPoints = new List<Point>();

                var px0 = _middleX + (float)(_radius * Math.Cos(-_sectorRad));
                var py0 = _middleY + (float)(_radius * Math.Sin(-_sectorRad));
                for (var i = 0; i < Colors.Count; i++)
                {
                    var angle = _sectorRad * i;
                    var px = _middleX + (float)(_radius * Math.Cos(angle));
                    var py = _middleY + (float)(_radius * Math.Sin(angle));
                    var pM = new Point((px0 + px) / 2d, (py0 + py) / 2d);
                    sectorPoints.Add(pM);

                    px0 = px;
                    py0 = py;
                }
            }

            var minDistance = double.NaN;
            var colorNum = -1;

            for (var i = 0; i < Colors.Count; i++)
            {
                var dist = DistanceBetweenTwoPoints(p, sectorPoints[i]);
                if (double.IsNaN(minDistance) || dist < minDistance)
                {
                    minDistance = dist;
                    colorNum = i;
                }
            }

            if (colorNum != _selColor)
            {
                SelectedColor = colorNum;
            }
        }

        #endregion

        #region DistanceBetweenTwoPoints

        public static double DistanceBetweenTwoPoints(Point point1, Point point2)
        {
            var dx = point1.X - point2.X;
            var dy = point1.Y - point2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        #endregion

        #region Draw

        private List<Point> sectorPoints;
        private List<List<PathOp>> paths;

        public override void Draw(ICanvas canvas, Rect rect)
        {
            #region variables

            _middleX = rect.Width / 2d;
            _middleY = rect.Height / 2d;
            _sectorRad = (float)(2 * Math.PI / Colors.Count);
            _radius = Math.Min(rect.Width, rect.Height) * 40d / 100d;
            _innerD = Math.Min(rect.Width, rect.Height) * 20d / 100d;

            #endregion

            #region back circle

            var outerD = _radius * 2;
            canvas.DrawEllipse(
                (rect.Width - outerD) / 2d - 15,
                (rect.Height - outerD) / 2d - 15,
                outerD + 30,
                outerD + 30,
                null,
                new SolidBrush(new Color(76d / 255d, 84d / 255d, 95d / 255d)));

            canvas.DrawEllipse(
                (rect.Width - outerD) / 2d - 5,
                (rect.Height - outerD) / 2d - 5,
                outerD + 10,
                outerD + 10,
                null,
                new SolidBrush(new Color(54d / 255d, 61d / 255d, 71d / 255d)));

            #endregion

            #region sectors
            
            var px0 = _middleX + (float)(_radius * Math.Cos(-_sectorRad));
            var py0 = _middleY + (float)(_radius * Math.Sin(-_sectorRad));
            paths = new List<List<PathOp>>();

            for (var i = 0; i < Colors.Count; i++)
            {
                var angle = _sectorRad * i;
                var px = _middleX + (float)(_radius * Math.Cos(angle));
                var py = _middleY + (float)(_radius * Math.Sin(angle));

                var path = new List<PathOp>();
                if (Xamarin.Forms.Device.OS == TargetPlatform.Android)
                {
                    path = new List<PathOp>
                    {
                        new MoveTo(_middleX, _middleY),
                        new LineTo(px0, py0),
                        new ArcTo(new Size(_radius), false, true, new Point(px, py)),
                        new LineTo(_middleX, _middleY)
                    };
                }
                else
                {
                    path = new List<PathOp>
                    {
                        new MoveTo(_middleX, _middleY),
                        new LineTo(px, py),
                        new ArcTo(new Size(_radius), true, true, new Point(px0, py0)),
                        new LineTo(_middleX, _middleY)
                    };
                }
                paths.Add(path);
                px0 = px;
                py0 = py;
            }

            for (var i = 0; i < Colors.Count; i++)
            {
                canvas.DrawPath(paths[i],
                    new Pen(Colors[i]),
                    new SolidBrush(Colors[i]));
            }

            #endregion

            #region inner circle

            var rdelta = 1.05d;
            canvas.DrawEllipse(
                _middleX - _radius * rdelta,
                _middleY - _radius * rdelta,
                _radius * 2 * rdelta,
                _radius * 2 * rdelta,
                null,
                new SolidBrush(new Color(0d, 0d, 0d, 52 / 256d)));

            var middleColor = Color.FromWhite(1);

            canvas.DrawEllipse(
                _middleX - _innerD / 2d,
                _middleY - _innerD / 2d,
                _innerD,
                _innerD,
                null,
                new SolidBrush(middleColor));

            #endregion

            #region draw selected color

            if (SelectedColor != -1)
            {
                var angle = _sectorRad * (SelectedColor - 1);

                var px1 = _middleX + (float)(_radius * rdelta * Math.Cos(angle));
                var py1 = _middleY + (float)(_radius * rdelta * Math.Sin(angle));
                angle = _sectorRad * SelectedColor;
                var px2 = _middleX + (float)(_radius * rdelta * Math.Cos(angle));
                var py2 = _middleY + (float)(_radius * rdelta * Math.Sin(angle));


                var path = new List<PathOp>();
                if (Xamarin.Forms.Device.OS == TargetPlatform.Android)
                    path = new List<PathOp>
                    {
                        new MoveTo(_middleX, _middleY),
                        new LineTo(px1, py1),
                        new ArcTo(new Size(_radius), false, true, new Point(px2, py2)),
                        new LineTo(_middleX, _middleY)
                    };
                else
                    path = new List<PathOp>
                    {
                        new MoveTo(_middleX, _middleY),
                        new LineTo(px2, py2),
                        new ArcTo(new Size(_radius), true, true, new Point(px1, py1)),
                        new LineTo(_middleX, _middleY)
                    };

                canvas.DrawPath(path,
                    new Pen(middleColor, 2),
                    new SolidBrush(Colors[SelectedColor].WithBrightness(0.8d)));

                canvas.DrawEllipse(
                    _middleX - _innerD / 2d + 2d,
                    _middleY - _innerD / 2d + 2d,
                    _innerD - 4d,
                    _innerD - 4d,
                    null,
                    new SolidBrush(Colors[SelectedColor].WithBrightness(0.8d)));
            }

            #endregion
        }

        #endregion
        
        public delegate void OnColorChanged(int i);
        public event OnColorChanged ColorChanged;
    }
}
