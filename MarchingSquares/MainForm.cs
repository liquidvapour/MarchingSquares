using System;
using System.Drawing;
using System.Windows.Forms;

namespace MarchingSquares
{
    public partial class MainForm : Form
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


        public MainForm()
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

        private readonly (Pen Pens, Brush Brush) _red = (new Pen(Color.Red, 3), Brushes.Red);
        private readonly (Pen Pens, Brush Brush) _indianRed = (new Pen(Color.IndianRed, 3), Brushes.IndianRed);
        private readonly (Pen Pens, Brush Brush) _paleVioletRed = (new Pen(Color.PaleVioletRed, 3), Brushes.PaleVioletRed);

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Aquamarine, ClientRectangle);
            for (var y = 0; y < ClientSize.Height; y+=SquareSize)
            {
                for (var x = 0; x < ClientSize.Width; x += SquareSize)
                {
                    DrawCase(GetContourCase(x, y, 0.75f, _f), x, y, e.Graphics, _red, 0.75f);
                    DrawCase(GetContourCase(x, y, 1, _f), x, y, e.Graphics, _indianRed, 1);
                    DrawCase(GetContourCase(x, y, 1.5f, _f), x, y, e.Graphics, _paleVioletRed, 1.5f);
                }

            }
        }

        private float FindPoint(float xa, float xb, float ua, float ub, float f) => xa + (f - ua) / (ub - ua) * (xb - xa);

        private void DrawCase((int caseId, CellInfo cellInfo) contourCase, int x, int y, Graphics g, (Pen Pen, Brush Brush) color, float f)
        {
            var (pen, brush) = color;
            switch (contourCase.caseId)
            {
                case 0:
                    break;
                case 15:
                    //g.FillRectangle(brush, x, y, SquareSize, SquareSize);
                    break;
                case 1:
                case 14:
                {   var startY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.BL, f);
                    var endX = FindPoint(x, x + SquareSize, contourCase.cellInfo.BL, contourCase.cellInfo.BR, f);
                    g.DrawLine(pen, x, startY, endX, y + SquareSize);
                    break;
                }
                case 2:
                case 13:
                {
                    var startY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TR, contourCase.cellInfo.BR, f);
                    var endX = FindPoint(x, x + SquareSize, contourCase.cellInfo.BL, contourCase.cellInfo.BR, f);
                    g.DrawLine(pen, x + SquareSize, startY, endX, y + SquareSize);
                    break;
                }
                case 3:
                case 12:
                {
                    var startY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.BL, f);
                    var endY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TR, contourCase.cellInfo.BR, f);
                    g.DrawLine(pen, x, startY, x + SquareSize, endY);
                    break;
                }
                case 4:
                case 11:
                {
                    var startX = FindPoint(x, x + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.TR, f);
                    var endY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TR, contourCase.cellInfo.BR, f);
                    g.DrawLine(pen, startX, y, x + SquareSize, endY);
                    break;
                }
                case 5:
                {
                    var startY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.BL, f);
                    var endX = FindPoint(x, x + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.TR, f);  
                    g.DrawLine(pen, x, startY, endX, y);
                    var startX = FindPoint(x, x + SquareSize, contourCase.cellInfo.BL, contourCase.cellInfo.BR, f);
                    var endY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TR, contourCase.cellInfo.BR, f);
                    g.DrawLine(pen, startX, y + SquareSize, x + SquareSize, endY);
                    break;
                }   
                case 6:
                case 9:
                {
                    var startX = FindPoint(x, x + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.TR, f);
                    var endX = FindPoint(x, x + SquareSize, contourCase.cellInfo.BL, contourCase.cellInfo.BR, f);
                    g.DrawLine(pen, startX, y, endX, y + SquareSize);
                    break;
                }
                case 7:
                case 8:
                {
                    var startY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.BL, f);
                    var endX = FindPoint(x, x + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.TR, f);
                    g.DrawLine(pen, x, startY, endX, y);
                    break;
                }  
                case 10:
                {
                    var startY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.BL, f);
                    var endX = FindPoint(x, x + SquareSize, contourCase.cellInfo.BL, contourCase.cellInfo.BR, f);
                    g.DrawLine(pen, x, startY, endX, y + SquareSize);
                    var startX = FindPoint(x, x + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.TR, f);
                    var endY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TR, contourCase.cellInfo.BR, f);
                    g.DrawLine(pen, startX, y, x + SquareSize, endY);
                    break;
                }
            }
        }

        private static (bool inside, float dist) InsideThreshold(float dist, float threshold)
        {
            return (dist >= threshold, dist);
        }

        private static (bool inside, float dist) TopLeftIn(int ix, int iy, float threshold, Func<float, float, float> func) => 
            InsideThreshold(func(ix, iy), threshold);

        private static (bool inside, float dist) TopRightIn(int ix, int iy, float threshold, Func<float, float, float> func) => 
            InsideThreshold(func(ix + SquareSize, iy), threshold);

        private static (bool inside, float dist) BottomLeftIn(int ix, int iy, float threshold, Func<float, float, float> func) => 
            InsideThreshold(func(ix , iy + SquareSize), threshold);

        private static (bool inside, float dist) BottomRightIn(int ix, int iy, float threshold, Func<float, float, float> func) => 
            InsideThreshold(func(ix + SquareSize, iy + SquareSize), threshold);

        private static (int caseId, CellInfo cellInfo) GetContourCase(int ix, int iy, float threshold, Func<float, float, float> func)
        {
            var bottomLeft = BottomLeftIn(ix, iy, threshold, func);
            var blNum = bottomLeft.inside? BLIndex: 0;
            var bottomRight = BottomRightIn(ix, iy, threshold, func);
            var brNum = bottomRight.inside ? BRIndex: 0;
            var topRight = TopRightIn(ix, iy, threshold, func);
            var trNum = topRight.inside ? TRIndex: 0;
            var topLeft = TopLeftIn(ix, iy, threshold, func);
            var tlNum = topLeft.inside ? TLIndex: 0;

            var caseId = blNum + brNum + trNum + tlNum;
            return (caseId, new CellInfo(bottomLeft.dist, bottomRight.dist, topRight.dist, topLeft.dist));
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            _context.MouseX = e.X;
            _context.MouseY = e.Y;
            Invalidate();
        }
    }

    internal readonly struct CellInfo
    {
        public CellInfo(float bl, float br, float tr, float tl)
        {
            BL = bl;
            BR = br;
            TR = tr;
            TL = tl;
        }
        public float BL { get; }
        public float BR { get; }
        public float TR { get; }
        public float TL { get; }
    }
}
