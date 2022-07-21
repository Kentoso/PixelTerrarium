using Godot;

namespace PixelTerrarium.Model
{
    public class Chunk
    {
        public Vector2 Size;
        public Pixel[,] Pixels;
        public bool IsActive = false;

        public Chunk(Vector2 size)
        {
            Size = size;
            Pixels = new Pixel[(int)Size.x, (int)Size.y];
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    Pixels[i, j] = new Pixel();
                }
            }
        }
    }
}