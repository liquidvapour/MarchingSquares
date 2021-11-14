using System;
using System.Drawing;
using System.Windows.Forms;

namespace MarchingSquares
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            InitializeComponent();

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //e.Graphics.DrawArc(Pens.Black, rect, 0.0f, 360.0f);
            for (int y = 0; y < ClientSize.Height; y+=squareSize)
            {
                for (int x = 0; x < ClientSize.Width; x += squareSize)
                {
                    var contourCase = GetContourCase(x, y);
                    DrawCase(contourCase, x, y, e.Graphics);
                }
                
            }
        }

        private Pen pen = Pens.Black;

        private void DrawCase(int contourCase, int x, int y, Graphics g)
        {

            switch (contourCase)
            {
                case 0:
                    break;
                case 1:
                    g.DrawLine(pen, x, y + halfSquareSize, x + halfSquareSize, y + squareSize);
                    break;
                case 2:
                    g.DrawLine(pen, x + squareSize, y + halfSquareSize, x + halfSquareSize, y + squareSize);
                    break;
                case 3:
                    g.DrawLine(pen, x, y + halfSquareSize, x + squareSize, y + halfSquareSize);
                    break;
                case 4:
                    g.DrawLine(pen, x + halfSquareSize, y, x + squareSize, y + halfSquareSize);
                    break;
                case 5:
                    g.DrawLine(pen, x, y + halfSquareSize, x + halfSquareSize, y);
                    g.DrawLine(pen, x + halfSquareSize, y + squareSize, x + squareSize, y + halfSquareSize);
                    break;
                case 6:
                    g.DrawLine(pen, x + halfSquareSize, y, x + halfSquareSize, y + squareSize);
                    break;
                case 7:
                    g.DrawLine(pen, x, y + halfSquareSize, x + halfSquareSize, y);
                    break;
                case 8:
                    g.DrawLine(pen, x, y + halfSquareSize, x + halfSquareSize, y);
                    break;
                case 9:
                    g.DrawLine(pen, x + halfSquareSize, y, x + halfSquareSize, y + squareSize);
                    break;
                case 10:
                    g.DrawLine(pen, x, y + halfSquareSize, x + halfSquareSize, y + squareSize);
                    g.DrawLine(pen, x + halfSquareSize, y, x + squareSize, y + halfSquareSize);
                    break;
                case 11:
                    g.DrawLine(pen, x + halfSquareSize, y, x + squareSize, y + halfSquareSize);
                    break;
                case 12:
                    break;
                case 13:
                    g.DrawLine(pen, x + squareSize, y + halfSquareSize, x + halfSquareSize, y + squareSize);
                    break;
                case 14:
                    g.DrawLine(pen, x, y + halfSquareSize, x + halfSquareSize, y + squareSize);
                    break;
                case 15:
                    break;
            }
        }

        private float CircleFunc(float cx, float cy)
        {
            var l = x - cx;
            var h = y - cy;
            var dist = (float)Math.Sqrt(l * l + h * h);
            return dist - r;
        }

        private bool TopLeftIn(int ix, int iy)
        {
            var dist = CircleFunc(ix, iy);

            return dist <= 0;

        }

        private bool TopRightIn(int ix, int iy)
        {
            var dist = CircleFunc(ix + squareSize, iy);

            return dist <= 0;

        }

        private bool BottomLeftIn(int ix, int iy)
        {
            var dist = CircleFunc(ix , iy + squareSize);

            return dist <= 0;

        }

        private bool BottomRightIn(int ix, int iy)
        {
            var dist = CircleFunc(ix + squareSize, iy + squareSize);

            return dist <= 0;

        }

        private int GetContourCase(int ix, int iy)
        {
            var bl = BottomLeftIn(ix, iy)? 1: 0;
            var br = BottomRightIn(ix, iy)? 2: 0;
            var tr = TopRightIn(ix, iy)? 4: 0;
            var tl = TopLeftIn(ix, iy)? 8: 0;

            return bl + br + tr + tl;
        }

        private int x = 0;
        private int y = 0;
        private float r = 20.0f;
        private const int width = 40;
        private const int halfWidth = width / 2;
        private Rectangle rect = new Rectangle(0, 0, width, width);
        private const int squareSize = 10;
        private const int halfSquareSize = squareSize / 2;

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            x = e.X;
            y = e.Y;
            rect.X = e.X - halfWidth;
            rect.Y = e.Y - halfWidth;
            Invalidate();
        }
    }
}
