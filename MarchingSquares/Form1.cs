using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            e.Graphics.DrawArc(Pens.Black, r, 0.0f, 360.0f);
        }

        private int x = 0;
        private int y = 0;
        private const int width = 40;
        private const int halfWidth = width / 2;
        private Rectangle r = new Rectangle(0, 0, width, width);
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            r.X = e.X - halfWidth;
            r.Y = e.Y - halfWidth;
            Invalidate();
        }
    }
}
