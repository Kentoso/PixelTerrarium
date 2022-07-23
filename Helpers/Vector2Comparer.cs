using System.Collections.Generic;
using Godot;

namespace PixelTerrarium.Helpers
{
    public class Vector2Comparer : IComparer<Vector2>
    {
        public int Compare(Vector2 x, Vector2 y)
        {
            var xComparison = x.x.CompareTo(y.x);
            if (xComparison != 0) return xComparison;
            return x.y.CompareTo(y.y);
        }
    }
}