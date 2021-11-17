using System;

namespace MarchingSquares
{
    public class BubbleFunction : IHaveADistanceFunction
    {
        private readonly Bubbles _bubbles;

        public BubbleFunction(Bubbles bubbles)
        {
            _bubbles = bubbles;
        }

        public float CalculateDistance(float cx, float cy)
        {
            var dist = 0.0f;
            for (var i = 0; i < _bubbles.Length; i++)
            {
                dist += GetDistOverRadius(cx, cy, _bubbles.X[i], _bubbles.Y[i], _bubbles.Radius[i]);
            }

            return dist;
        }

        private static float GetDistOverRadius(float cx, float cy, float i, float j, float radius)
        {
            var al = i - cx;
            var ah = j - cy;
            return radius / (float) Math.Sqrt(al * al + ah * ah);
        }
    }

    public readonly struct Bubbles
    {
        public float[] X { get; }
        public float[] Y { get; }
        public float[] Radius { get; }

        public int Length => Radius.Length;

        public Bubbles(params (float radius, float x, float y)[] bubbles)
        {
            Radius = new float[bubbles.Length];
            X = new float[bubbles.Length];
            Y = new float[bubbles.Length];

            for (int i = 0; i < bubbles.Length; i++)
            {
                Radius[i] = bubbles[i].radius;
                X[i] = bubbles[i].x;
                Y[i] = bubbles[i].y;
            }
        }
    }
}