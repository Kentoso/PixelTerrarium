namespace PixelTerrarium.Model
{
    public class Pixel
    {
        public int? Type;
        public int? PaletteRef;

        public Pixel(int type, int paletteRef)
        {
            Type = type;
            PaletteRef = paletteRef;
        }

        public Pixel()
        {
            Type = null;
            PaletteRef = null;
        }
    }
}