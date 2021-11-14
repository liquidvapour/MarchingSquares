using System;

namespace MarchingSquares
{
    public class BubbleFunction : IHaveADistanceFunction
    {
        private readonly Context _context;
        private const float R = 64.0f;
        private const int OtherBlobX = 100;
        private const int OtherBlobY = 100;

        public int MouseX { get; set; }

        public int MouseY { get; set; }

        public BubbleFunction(Context context)
        {
            _context = context;
        }

        public float CalculateDistance(float cx, float cy) => 
            GetDistOverRadius(cx, cy, _context.MouseX, _context.MouseY, R) + 
            GetDistOverRadius(cx, cy, OtherBlobX, OtherBlobY, R);

        private static float GetDistOverRadius(float cx, float cy, int i, int j, float radius)
        {
            var al = i - cx;
            var ah = j - cy;
            return radius / (float) Math.Sqrt(al * al + ah * ah);
        }
    }
}