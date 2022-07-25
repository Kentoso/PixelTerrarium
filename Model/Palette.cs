using Godot;

namespace PixelTerrarium.Model
{
    public class Palette
    {
        private Color[] _colors;
        private int _size;
        public int Length;

        public Palette(int size)
        {
            _colors = new Color[size];
            _size = size;
        }

        public Palette(Palette another)
        {
            _size = another._size;
            _colors = new Color[_size];
            for (int i = 0; i < _size; i++)
            {
                _colors[i] = another._colors[i];
            }

            Length = another.Length;
        }

        public void AppendColor(Color color)
        {
            _colors[Length] = color;
            Length++;
        }
        public void SetColor(int id, Color color)
        {
            if (id >= _size)
            {
                GD.PushError($"[PALETTE]: {id} is out of bounds");
                return;
            }
            _colors[id] = color;
        }

        public Color GetColor(int id)
        {
            if (id >= _size)
            {
                GD.PushError($"[PALETTE]: {id} is out of bounds");
                return Colors.White;
            }
            return _colors[id];
        }

        public void CreateLinearGradient(int start, int end, Color startColor, Color endColor, bool invert = false)
        {
            if (invert)
            {
                for (int i = start; i < end; i++)
                {
                    var currColor = endColor + (i / (float) (end - start)) * (startColor - endColor);
                    SetColor(i, currColor);
                }
            }
            else
            {
                for (int i = start; i < end; i++)
                {
                    var currColor = startColor + (i / (float)(end - start)) * (endColor - startColor);
                    SetColor(i, currColor);
                }
            }
        }
        
        public Palette AnimateRange(int start, int end, bool invert = false)
        {
            var newPal = new Palette(this);
            if (invert)
            {
                newPal.SetColor(end - 1, GetColor(start));
                for (int i = end - 1; i > start; i--)
                {
                    newPal.SetColor(i - 1, GetColor(i));
                }
            }
            else
            {
                newPal.SetColor(start, GetColor(end - 1));
                for (int i = start; i < end - 1; i++)
                {
                    newPal.SetColor(i + 1, GetColor(i));
                }
            }
            return newPal;
        }
    }
}