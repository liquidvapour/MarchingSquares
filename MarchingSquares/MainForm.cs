using System;
using System.Collections.Immutable;
using System.Drawing;
using System.Windows.Forms;

namespace MarchingSquares
{
    
    public partial class MainForm : Form
    {
        private const int SquareSize = 8;
        private const int HalfSquareSize = SquareSize / 2;
        private const int BLIndex = 1;
        private const int BRIndex = 2;
        private const int TRIndex = 4;
        private const int TLIndex = 8;

        private Func<float, float, float> _f;

        private readonly Lines lines = new Lines();

        private readonly BubbleFunction _bubbleFunction;

        private readonly (Pen Pens, Brush Brush) _red = (new Pen(Color.Red, 3), Brushes.Red);
        private readonly (Pen Pens, Brush Brush) _indianRed = (new Pen(Color.IndianRed, 3), Brushes.IndianRed);
        private readonly (Pen Pens, Brush Brush) _paleVioletRed = (new Pen(Color.PaleVioletRed, 3), Brushes.PaleVioletRed);
        private readonly Bubbles _bubbles;

        public MainForm()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            InitializeComponent();
            _bubbles = new Bubbles(
                (64.0f, 0.0f, 0.0f),
                (32, 300, 300));
            _bubbleFunction = new BubbleFunction(_bubbles);

            SetDistanceFunction(_bubbleFunction);
        }

        private void SetDistanceFunction(IHaveADistanceFunction functionProvider)
        {
            _f = functionProvider.CalculateDistance;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Aquamarine, ClientRectangle);
            var lineCount = 0;
            for (var y = 0; y < ClientSize.Height; y+=SquareSize)
            {
                for (var x = 0; x < ClientSize.Width; x += SquareSize)
                {
                    var f = 1.0f;
                    var contourCase = GetContourCase(x, y, f, _f);
                    var r = GetCasePoints(contourCase, x, y, f);

                    while (!r.IsEmpty)
                    {
                        r = r.Dequeue(out var i);
                        lines.StartX[lineCount] = i.startX;
                        lines.EndX[lineCount] = i.endX;
                        lines.StartY[lineCount] = i.startY;
                        lines.EndY[lineCount] = i.endY;
                        lineCount += 1;
                    }
                }

            }
            lines.Length = lineCount;
            DrawLines(lines, _indianRed, e.Graphics);
            Invalidate();
        }

        private void DrawLines(
            Lines lines, 
            (Pen Pens, Brush Brush) color, 
            Graphics e)
        {
            for (var i = 0; i < lines.Length; i++)
            {
                e.DrawLine(color.Pens, lines.StartX[i], lines.StartY[i], lines.EndX[i], lines.EndY[i]);
            }
        }

        private float FindPoint(float xa, float xb, float ua, float ub, float f) => 
            xa + (f - ua) / (ub - ua) * (xb - xa);

        private ImmutableQueue<(float startX, float startY, float endX, float endY)> GetCasePoints((int caseId, CellInfo cellInfo) contourCase, float x, float y, float f)
        {
            var q = ImmutableQueue<(float startX, float startY, float endX, float endY)>.Empty;
            switch (contourCase.caseId)
            {
                case 0:
                    break;
                case 15:
                    //g.FillRectangle(brush, x, y, SquareSize, SquareSize);
                    break;
                case 1:
                case 14:
                    {
                        var startX = x;
                        var startY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.BL, f);
                        var endX = FindPoint(x, x + SquareSize, contourCase.cellInfo.BL, contourCase.cellInfo.BR, f);
                        var endY = y + SquareSize;
                        q = q.Enqueue((startX, startY, endX, endY));
                        break;
                    }
                case 2:
                case 13:
                    {
                        var startX = x + SquareSize;
                        var startY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TR, contourCase.cellInfo.BR, f);
                        var endX = FindPoint(x, x + SquareSize, contourCase.cellInfo.BL, contourCase.cellInfo.BR, f);
                        var endY = y + SquareSize;
                        q = q.Enqueue((startX, startY, endX, endY));
                        break;
                    }
                case 3:
                case 12:
                    {
                        var startX = x;
                        var startY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.BL, f);
                        var endX = x + SquareSize;
                        var endY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TR, contourCase.cellInfo.BR, f);
                        q = q.Enqueue((startX, startY, endX, endY));
                        break;
                    }
                case 4:
                case 11:
                    {
                        var startX = FindPoint(x, x + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.TR, f);
                        var startY = y;
                        var endX = x + SquareSize;
                        var endY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TR, contourCase.cellInfo.BR, f);
                        q = q.Enqueue((startX, startY, endX, endY));
                        break;
                    }
                case 5:
                    {
                        var startY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.BL, f);
                        var endX = FindPoint(x, x + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.TR, f);
                        var startX = x;
                        var endY = y;
                        q = q.Enqueue((startX, startY, endX, endY));
                        startX = FindPoint(x, x + SquareSize, contourCase.cellInfo.BL, contourCase.cellInfo.BR, f);
                        endY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TR, contourCase.cellInfo.BR, f);
                        startY = y + SquareSize;
                        endX = x + SquareSize;
                        q = q.Enqueue((startX, startY, endX, endY));
                        break;
                    }
                case 6:
                case 9:
                    {
                        var startX = FindPoint(x, x + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.TR, f);
                        var endX = FindPoint(x, x + SquareSize, contourCase.cellInfo.BL, contourCase.cellInfo.BR, f);
                        var startY = y;
                        var endY = y + SquareSize;
                        q = q.Enqueue((startX, startY, endX, endY));
                        break;
                    }
                case 7:
                case 8:
                    {
                        var startX = x;
                        var startY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.BL, f);
                        var endX = FindPoint(x, x + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.TR, f);
                        var endY = y;
                        q = q.Enqueue((startX, startY, endX, endY));
                        break;
                    }
                case 10:
                    {
                        var startX = x;
                        var startY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.BL, f);
                        var endX = FindPoint(x, x + SquareSize, contourCase.cellInfo.BL, contourCase.cellInfo.BR, f);
                        var endY = y + SquareSize;
                        q = q.Enqueue((startX, startY, endX, endY));
                        startX = FindPoint(x, x + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.TR, f);
                        startY = y;
                        endX = x + SquareSize;
                        endY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TR, contourCase.cellInfo.BR, f);
                        q = q.Enqueue((startX, startY, endX, endY));
                        break;
                    }
            }

            return q;
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
            _bubbles.X[0] = e.X;
            _bubbles.Y[0] = e.Y;
            //Invalidate();
        }
    }

    public struct Lines
    {
        internal int Length;

        public readonly float[] StartX = new float[1024];
        public readonly float[] StartY = new float[1024];
        public readonly float[] EndX = new float[1024];
        public readonly float[] EndY = new float[1024];

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
