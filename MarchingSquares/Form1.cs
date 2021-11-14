﻿using System;
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
            f = BubbleFunc;
        }

        private Func<float, float, float> f;

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
                case 15:
                    break;
                case 1:
                case 14:
                    g.DrawLine(pen, x, y + halfSquareSize, x + halfSquareSize, y + squareSize);
                    break;
                case 2:
                case 13:
                    g.DrawLine(pen, x + squareSize, y + halfSquareSize, x + halfSquareSize, y + squareSize);
                    break;
                case 3:
                case 12:
                    g.DrawLine(pen, x, y + halfSquareSize, x + squareSize, y + halfSquareSize);
                    break;
                case 4:
                case 11:
                    g.DrawLine(pen, x + halfSquareSize, y, x + squareSize, y + halfSquareSize);
                    break;
                case 5:
                    g.DrawLine(pen, x, y + halfSquareSize, x + halfSquareSize, y);
                    g.DrawLine(pen, x + halfSquareSize, y + squareSize, x + squareSize, y + halfSquareSize);
                    break;
                case 6:
                case 9:
                    g.DrawLine(pen, x + halfSquareSize, y, x + halfSquareSize, y + squareSize);
                    break;
                case 7:
                case 8:
                    g.DrawLine(pen, x, y + halfSquareSize, x + halfSquareSize, y);
                    break;
                case 10:
                    g.DrawLine(pen, x, y + halfSquareSize, x + halfSquareSize, y + squareSize);
                    g.DrawLine(pen, x + halfSquareSize, y, x + squareSize, y + halfSquareSize);
                    break;
            }
        }

        private float CircleFunc(float cx, float cy)
        {
            var l = x - cx;
            var h = y - cy;
            var dist = r / (float)Math.Sqrt(l * l + h * h);
            return dist;
        }


        private int ax = 100;
        private int ay = 100;

        private float BubbleFunc(float cx, float cy) => GetDistOverRadius(cx, cy, x, y, r) + GetDistOverRadius(cx, cy, ax, ay, r);

        private static float GetDistOverRadius(float cx, float cy, int i, int j, float radius)
        {
            var al = i - cx;
            var ah = j - cy;
            return radius / (float) Math.Sqrt(al * al + ah * ah);
        }


        private bool TopLeftIn(int ix, int iy)
        {
            var dist = f(ix, iy);

            return dist <= 1;

        }

        private bool TopRightIn(int ix, int iy)
        {
            var dist = f(ix + squareSize, iy);

            return dist <= 1;

        }

        private bool BottomLeftIn(int ix, int iy)
        {
            var dist = f(ix , iy + squareSize);

            return dist <= 1;

        }

        private bool BottomRightIn(int ix, int iy)
        {
            var dist = f(ix + squareSize, iy + squareSize);

            return dist <= 1;

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
        private float r = 63.0f;
        private const int squareSize = 8;
        private const int halfSquareSize = squareSize / 2;

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            x = e.X;
            y = e.Y;
            Invalidate();
        }
    }
}
