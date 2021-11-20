using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace MarchingSquares
{
    
    public partial class MainForm : Form
    {
        private const int SquareSize = 8;
        private const int BLIndex = 1;
        private const int BRIndex = 2;
        private const int TRIndex = 4;
        private const int TLIndex = 8;

        private Func<int, int, float> _f;

        private readonly BubbleFunction _bubbleFunction;

        private readonly (Pen Pens, Brush Brush) _red = (new Pen(Color.Red, 3), Brushes.Red);
        private readonly (Pen Pens, Brush Brush) _indianRed = (new Pen(Color.IndianRed, 3), Brushes.IndianRed);
        private readonly (Pen Pens, Brush Brush) _paleVioletRed = (new Pen(Color.PaleVioletRed, 3), Brushes.PaleVioletRed);
        private readonly Bubbles _bubbles;

        private readonly Stopwatch _thinkTime = new();
        private readonly Stopwatch _drawTime = new();
        private readonly Stopwatch _wallTime = Stopwatch.StartNew();
        private int frames = 0;
        private float _lastFrameTime;

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

            _f = _bubbleFunction.CalculateDistance;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            var thisFrameTime = GetTimeElapsedMicroseconds(_wallTime);
            if (_lastFrameTime > 0.0f)
            {
                var dt = thisFrameTime - _lastFrameTime;
                Draw(dt, e.Graphics);
            }
            _lastFrameTime = thisFrameTime;

            Invalidate();
        }

        private void Draw(float dt, Graphics gc)
        {
            gc.FillRectangle(Brushes.Aquamarine, ClientRectangle);
            _thinkTime.Start();
            var lines = new Lines();
            CalculateLines(dt, lines);
            _thinkTime.Stop();
            gc.DrawString(
                GetTimeElapsedMicroseconds(_thinkTime).ToString("N", CultureInfo.InvariantCulture),
                Font,
                Brushes.Black,
                0,
                0);
            _thinkTime.Reset();
            DrawLines(lines, _indianRed, gc);
            gc.DrawString(lines.Length.ToString(), Font, Brushes.Black, 0, 40);
            DoFps(gc, dt);
        }

        private void DoFps(Graphics gc, float dt)
        {
            var fps = (100000.0f / dt);
            Console.WriteLine(dt);
            gc.DrawString(fps.ToString("N"), Font, Brushes.Black, 0, 60);
            gc.DrawString(dt.ToString("N"), Font, Brushes.Black, 0 , 80);
        }

        private static float totalTime;
        private void CalculateLines(float dt, Lines lines)
        {
            totalTime += dt;
            var lineCount = 0;
            for (var y = 0; y < ClientSize.Height; y += SquareSize)
            {
                for (var x = 0; x < ClientSize.Width; x += SquareSize)
                {
                    const float threshold = 1.0f;
                    var contourCase = GetContourCase(x, y, threshold, _f);
                    lineCount += GetCasePoints(contourCase, x, y, threshold, lines, lineCount);

                }
            }

            lines.Length = lineCount;
        }

        private static float GetTimeElapsedMicroseconds(Stopwatch stopwatch) => 
            stopwatch.ElapsedTicks / (float)Stopwatch.Frequency / 0.000001f;

        private void DrawLines(
            Lines lines, 
            (Pen Pens, Brush Brush) color, 
            Graphics e)
        {
            _drawTime.Start();
            for (var i = 0; i < lines.Length; i++)
            {
                e.DrawLine(color.Pens, lines.StartX[i], lines.StartY[i], lines.EndX[i], lines.EndY[i]);
            }
            _drawTime.Stop();
            e.DrawString(
                GetTimeElapsedMicroseconds(_drawTime).ToString("N", CultureInfo.InvariantCulture), 
                Font, 
                Brushes.Black, 
                0, 20);
            _drawTime.Reset();
        }

        private float FindPoint(float xa, float xb, float ua, float ub, float f) => 
            xa + (f - ua) / (ub - ua) * (xb - xa);

        private int GetCasePoints((int caseId, CellInfo cellInfo) contourCase, float x, float y, float f, Lines lines,
            int lineCount)
        {
            
            switch (contourCase.caseId)
            {
                case 0:
                case 15:
                    return default;
                case 1:
                case 14:
                    {
                        var startX = x;
                        var startY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.BL, f);
                        var endX = FindPoint(x, x + SquareSize, contourCase.cellInfo.BL, contourCase.cellInfo.BR, f);
                        var endY = y + SquareSize;
                        lines.StartX[lineCount] = startX;
                        lines.StartY[lineCount] = startY;
                        lines.EndX[lineCount] = endX;
                        lines.EndY[lineCount] = endY;
                        return 1;
                    }
                case 2:
                case 13:
                    {
                        var startX = x + SquareSize;
                        var startY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TR, contourCase.cellInfo.BR, f);
                        var endX = FindPoint(x, x + SquareSize, contourCase.cellInfo.BL, contourCase.cellInfo.BR, f);
                        var endY = y + SquareSize;
                        lines.StartX[lineCount] = startX;
                        lines.StartY[lineCount] = startY;
                        lines.EndX[lineCount] = endX;
                        lines.EndY[lineCount] = endY;
                        return 1;
                    }
                case 3:
                case 12:
                    {
                        var startX = x;
                        var startY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.BL, f);
                        var endX = x + SquareSize;
                        var endY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TR, contourCase.cellInfo.BR, f);
                        lines.StartX[lineCount] = startX;
                        lines.StartY[lineCount] = startY;
                        lines.EndX[lineCount] = endX;
                        lines.EndY[lineCount] = endY;
                        return 1;
                    }
                case 4:
                case 11:
                    {
                        var startX = FindPoint(x, x + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.TR, f);
                        var startY = y;
                        var endX = x + SquareSize;
                        var endY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TR, contourCase.cellInfo.BR, f);
                        lines.StartX[lineCount] = startX;
                        lines.StartY[lineCount] = startY;
                        lines.EndX[lineCount] = endX;
                        lines.EndY[lineCount] = endY;
                        return 1;
                    }
                case 5:
                    {
                        var startY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.BL, f);
                        var endX = FindPoint(x, x + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.TR, f);
                        var startX = x;
                        var endY = y;
                        lines.StartX[lineCount] = startX;
                        lines.StartY[lineCount] = startY;
                        lines.EndX[lineCount] = endX;
                        lines.EndY[lineCount] = endY;
                        lineCount++;
                        startX = FindPoint(x, x + SquareSize, contourCase.cellInfo.BL, contourCase.cellInfo.BR, f);
                        endY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TR, contourCase.cellInfo.BR, f);
                        startY = y + SquareSize;
                        endX = x + SquareSize;
                        lines.StartX[lineCount] = startX;
                        lines.StartY[lineCount] = startY;
                        lines.EndX[lineCount] = endX;
                        lines.EndY[lineCount] = endY;
                        return 2;
                    }
                case 6:
                case 9:
                    {
                        var startX = FindPoint(x, x + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.TR, f);
                        var endX = FindPoint(x, x + SquareSize, contourCase.cellInfo.BL, contourCase.cellInfo.BR, f);
                        var startY = y;
                        var endY = y + SquareSize;
                        lines.StartX[lineCount] = startX;
                        lines.StartY[lineCount] = startY;
                        lines.EndX[lineCount] = endX;
                        lines.EndY[lineCount] = endY;
                        return 1;
                    }
                case 7:
                case 8:
                    {
                        var startX = x;
                        var startY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.BL, f);
                        var endX = FindPoint(x, x + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.TR, f);
                        var endY = y;
                        lines.StartX[lineCount] = startX;
                        lines.StartY[lineCount] = startY;
                        lines.EndX[lineCount] = endX;
                        lines.EndY[lineCount] = endY;
                        return 1;
                    }
                case 10:
                    {
                        var startX = x;
                        var startY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.BL, f);
                        var endX = FindPoint(x, x + SquareSize, contourCase.cellInfo.BL, contourCase.cellInfo.BR, f);
                        var endY = y + SquareSize;
                        lines.StartX[lineCount] = startX;
                        lines.StartY[lineCount] = startY;
                        lines.EndX[lineCount] = endX;
                        lines.EndY[lineCount] = endY;
                        lineCount++;
                        startX = FindPoint(x, x + SquareSize, contourCase.cellInfo.TL, contourCase.cellInfo.TR, f);
                        startY = y;
                        endX = x + SquareSize;
                        endY = FindPoint(y, y + SquareSize, contourCase.cellInfo.TR, contourCase.cellInfo.BR, f);
                        lines.StartX[lineCount] = startX;
                        lines.StartY[lineCount] = startY;
                        lines.EndX[lineCount] = endX;
                        lines.EndY[lineCount] = endY;
                        return 2;
                    }
                default:
                    throw new InvalidOperationException($"Case {contourCase.caseId} not supported");
            }
        }

        private static (bool inside, float dist) InsideThreshold(float dist, float threshold)
        {
            return (dist >= threshold, dist);
        }

        private static (bool inside, float dist) TopLeftIn(int ix, int iy, float threshold, Func<int, int, float> func) => 
            InsideThreshold(func(ix, iy), threshold);

        private static (bool inside, float dist) TopRightIn(int ix, int iy, float threshold, Func<int, int, float> func) => 
            InsideThreshold(func(ix + SquareSize, iy), threshold);

        private static (bool inside, float dist) BottomLeftIn(int ix, int iy, float threshold, Func<int, int, float> func) => 
            InsideThreshold(func(ix , iy + SquareSize), threshold);

        private static (bool inside, float dist) BottomRightIn(int ix, int iy, float threshold, Func<int, int, float> func) => 
            InsideThreshold(func(ix + SquareSize, iy + SquareSize), threshold);

        private static (int caseId, CellInfo cellInfo) GetContourCase(int ix, int iy, float threshold, Func<int, int, float> func)
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
            return (caseId, new(bottomLeft.dist, bottomRight.dist, topRight.dist, topLeft.dist));
        }


        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            _bubbles.X[0] = e.X;
            _bubbles.Y[0] = e.Y;
            //Invalidate();
        }
    }

    public ref struct Lines
    {
        private const int DefaultLength = 1024;
        public int Length { get; set; }

        public readonly float[] StartX;
        public readonly float[] StartY;
        public readonly float[] EndX;
        public readonly float[] EndY;

        public Lines() : this(DefaultLength)
        {
            
        }
        public Lines(int length)
        {
            StartX = new float[DefaultLength];
            StartY = new float[DefaultLength];
            EndX = new float[DefaultLength];
            EndY = new float[DefaultLength];
            Length = length;
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
