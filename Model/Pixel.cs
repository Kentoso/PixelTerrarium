namespace PixelTerrarium.Model
{
    public class Pixel
    {
        public int? PaletteRef;
        public Material Mat;
        public Pixel(Material mat, int paletteRef)
        {
            Mat = mat;
            PaletteRef = paletteRef;
        }
        
        public Pixel()
        {
            Mat = null;
            PaletteRef = null;
        }

        public Pixel(Pixel other)
        {
            Mat = other.Mat;
            PaletteRef = other.PaletteRef;
        }

        public void Set(Material mat)
        {
            Mat = mat;
        }
        public void Set(Material mat, int pref)
        {
            Mat = mat;
            PaletteRef = pref;
        }
        
        public void CopyPixel(Pixel other)
        {
            Mat = other.Mat;
            PaletteRef = other.PaletteRef;
        }

        public void Clear()
        {
            Mat = null;
            PaletteRef = null;
        }
    }
}