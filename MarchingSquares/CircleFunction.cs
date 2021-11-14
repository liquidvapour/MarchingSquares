using System;

namespace MarchingSquares
{
    public class CircleFunction : IHaveADistanceFunction
    {
        private readonly Context _context;

        public CircleFunction(Context context)
        {
            _context = context;
        }

        public float CalculateDistance(float cx, float cy)
        {
            var l = _context.MouseX - cx;
            var h = _context.MouseY - cy;
            return 64.0f / (float)Math.Sqrt(l * l + h * h);
        }
    }
}