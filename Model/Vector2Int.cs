using Godot;

namespace PixelTerrarium.Model
{
    public class Vector2Int
    {
        private Vector2 _vector2;

        public Vector2Int(int x, int y)
        {
            _vector2 = new Vector2(x, y);
        }
        public int x
        {
            get => (int)_vector2.x;
            set => _vector2.x = value;
        }

        public int y
        {
            get => (int) _vector2.y;
            set => _vector2.y = value;
        }

        public static implicit operator Vector2(Vector2Int v) => v._vector2;
    }
}