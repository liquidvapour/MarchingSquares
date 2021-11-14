using System;
using System.Drawing;
using System.Windows.Forms;

namespace MarchingSquares
{
    public partial class Form1 : Form
    {
        private const float R = 63.0f;
        private const int SquareSize = 8;
        private const int HalfSquareSize = SquareSize / 2;
        private const int BLIndex = 1;
        private const int BRIndex = 2;
        private const int TRIndex = 4;
        private const int TLIndex = 8;

        private readonly Func<float, float, float> _f;
        
        private int _mouseX;
        private int _mouseY;


        public Form1()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            InitializeComponent();
            _f = BubbleFunc;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            //e.Graphics.DrawArc(Pens.Black, rect, 0.0f, 360.0f);
            for (var y = 0; y < ClientSize.Height; y+=SquareSize)
            {
                for (var x = 0; x < ClientSize.Width; x += SquareSize)
                {
                    DrawCase(GetContourCase(x, y, 0.75f), x, y, e.Graphics, (Pens.IndianRed, Brushes.IndianRed));
                    DrawCase(GetContourCase(x, y, 1), x, y, e.Graphics, (Pens.Red, Brushes.Red));
                    DrawCase(GetContourCase(x, y, 1.5f), x, y, e.Graphics, (Pens.DarkRed, Brushes.DarkRed));
                }

            }
        }


        private void DrawCase(int contourCase, int x, int y, Graphics g, (Pen Pen, Brush Brush) color)
        {
            
            switch (contourCase)
            {
                case 0:
                    break;
                case 15:
                    g.FillRectangle(color.Brush, x, y, SquareSize, SquareSize);
                    break;
                case 1:
                case 14:
                    g.DrawLine(color.Pen, x, y + HalfSquareSize, x + HalfSquareSize, y + SquareSize);
                    break;
                case 2:
                case 13:
                    g.DrawLine(color.Pen, x + SquareSize, y + HalfSquareSize, x + HalfSquareSize, y + SquareSize);
                    break;
                case 3:
                case 12:
                    g.DrawLine(color.Pen, x, y + HalfSquareSize, x + SquareSize, y + HalfSquareSize);
                    break;
                case 4:
                case 11:
                    g.DrawLine(color.Pen, x + HalfSquareSize, y, x + SquareSize, y + HalfSquareSize);
                    break;
                case 5:
                    g.DrawLine(color.Pen, x, y + HalfSquareSize, x + HalfSquareSize, y);
                    g.DrawLine(color.Pen, x + HalfSquareSize, y + SquareSize, x + SquareSize, y + HalfSquareSize);
                    break;
                case 6:
                case 9:
                    g.DrawLine(color.Pen, x + HalfSquareSize, y, x + HalfSquareSize, y + SquareSize);
                    break;
                case 7:
                case 8:
                    g.DrawLine(color.Pen, x, y + HalfSquareSize, x + HalfSquareSize, y);
                    break;
                case 10:
                    g.DrawLine(color.Pen, x, y + HalfSquareSize, x + HalfSquareSize, y + SquareSize);
                    g.DrawLine(color.Pen, x + HalfSquareSize, y, x + SquareSize, y + HalfSquareSize);
                    break;
            }
        }

        private float CircleFunc(float cx, float cy)
        {
            var l = _mouseX - cx;
            var h = _mouseY - cy;
            return R / (float)Math.Sqrt(l * l + h * h);
        }


        private const int OtherBlobX = 100;
        private const int OtherBlobY = 100;

        private float BubbleFunc(float cx, float cy) => GetDistOverRadius(cx, cy, _mouseX, _mouseY, R) + GetDistOverRadius(cx, cy, OtherBlobX, OtherBlobY, R);

        private static float GetDistOverRadius(float cx, float cy, int i, int j, float radius)
        {
            var al = i - cx;
            var ah = j - cy;
            return radius / (float) Math.Sqrt(al * al + ah * ah);
        }


        private static bool InsideThreshold(float dist, float threshold) => dist >= threshold;

        private bool TopLeftIn(int ix, int iy, float threshold) => InsideThreshold(_f(ix, iy), threshold);

        private bool TopRightIn(int ix, int iy, float threshold) => InsideThreshold(_f(ix + SquareSize, iy), threshold);

        private bool BottomLeftIn(int ix, int iy, float threshold) => InsideThreshold(_f(ix , iy + SquareSize), threshold);

        private bool BottomRightIn(int ix, int iy, float threshold) => InsideThreshold(_f(ix + SquareSize, iy + SquareSize), threshold);

        private int GetContourCase(int ix, int iy, float threshold)
        {
            var bl = BottomLeftIn(ix, iy, threshold)? BLIndex: 0;
            var br = BottomRightIn(ix, iy, threshold)? BRIndex: 0;
            var tr = TopRightIn(ix, iy, threshold)? TRIndex: 0;
            var tl = TopLeftIn(ix, iy, threshold)? TLIndex: 0;

            return bl + br + tr + tl;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            _mouseX = e.X;
            _mouseY = e.Y;
            Invalidate();
        }
    }
}
