using System;
using System.Drawing;
using System.Windows.Forms;

namespace MarchingSquares
{
    public partial class Form1 : Form
    {
        private const int SquareSize = 16;
        private const int HalfSquareSize = SquareSize / 2;
        private const int BLIndex = 1;
        private const int BRIndex = 2;
        private const int TRIndex = 4;
        private const int TLIndex = 8;

        private Func<float, float, float> _f;

        private readonly BubbleFunction _bubbleFunction;
        private readonly Context _context;


        public Form1()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            InitializeComponent();
            _context = new Context();
            _bubbleFunction = new BubbleFunction(_context);

            SetDistanceFunction(_bubbleFunction);
        }

        private void SetDistanceFunction(IHaveADistanceFunction functionProvider)
        {
            _f = functionProvider.CalculateDistance;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            for (var y = 0; y < ClientSize.Height; y+=SquareSize)
            {
                for (var x = 0; x < ClientSize.Width; x += SquareSize)
                {
                    DrawCase(GetContourCase(x, y, 0.75f, _f).caseId, x, y, e.Graphics, (Pens.IndianRed, Brushes.IndianRed));
                    DrawCase(GetContourCase(x, y, 1, _f).caseId, x, y, e.Graphics, (Pens.Red, Brushes.Red));
                    DrawCase(GetContourCase(x, y, 1.5f, _f).caseId, x, y, e.Graphics, (Pens.DarkRed, Brushes.DarkRed));
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

        private static (bool inside, float dist) InsideThreshold(float dist, float threshold)
        {
            return (dist >= threshold, dist);
        }

        private static (bool inside, float u) TopLeftIn(int ix, int iy, float threshold, Func<float, float, float> func) => 
            InsideThreshold(func(ix, iy), threshold);

        private static (bool inside, float dist) TopRightIn(int ix, int iy, float threshold, Func<float, float, float> func) => 
            InsideThreshold(func(ix + SquareSize, iy), threshold);

        private static (bool inside, float dist) BottomLeftIn(int ix, int iy, float threshold, Func<float, float, float> func) => 
            InsideThreshold(func(ix , iy + SquareSize), threshold);

        private static (bool inside, float dist) BottomRightIn(int ix, int iy, float threshold, Func<float, float, float> func) => 
            InsideThreshold(func(ix + SquareSize, iy + SquareSize), threshold);

        private static (int caseId, Line[] lines) GetContourCase(int ix, int iy, float threshold, Func<float, float, float> func)
        {
            var bl = BottomLeftIn(ix, iy, threshold, func).inside? BLIndex: 0;
            var br = BottomRightIn(ix, iy, threshold, func).inside ? BRIndex: 0;
            var tr = TopRightIn(ix, iy, threshold, func).inside ? TRIndex: 0;
            var tl = TopLeftIn(ix, iy, threshold, func).inside ? TLIndex: 0;

            return (bl + br + tr + tl, new Line[0]);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            _context.MouseX = e.X;
            _context.MouseY = e.Y;
            Invalidate();
        }
    }

    internal readonly struct Line
    {
        public Line(float ax, float ay, float bx, float by)
        {
            AX = ax;
            AY = ay;
            BX = bx;
            BY = by;
        }
        public float AX { get; }
        public float AY { get; }
        public float BX { get; }
        public float BY { get; }
    }
}
