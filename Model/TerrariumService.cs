using System;
using Godot;

namespace PixelTerrarium.Model
{
    public class TerrariumService
    {
        public Pixel[,] PixelMap;
        public Vector2Int MapSize;
        public Palette Palette;

        public TerrariumService(Vector2Int mapSize, int paletteSize)
        {
            PixelMap = new Pixel[mapSize.x, mapSize.y];
            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    PixelMap[i, j] = new Pixel();
                }
            }
            MapSize = mapSize;
            Palette = new Palette(paletteSize);
        }

        public void SetPixel(int x, int y, int type, int paletteRef)
        {
            try
            {
                if (x >= MapSize.x || y >= MapSize.y) return;
                var p = new Pixel(type, paletteRef);
                PixelMap[x, y] = p;
            }
            catch (Exception e)
            {
                GD.PrintErr($"[TERRARIUM]: {x}, {y} out of bounds");
            }
            
        }

        public Pixel GetPixel(int x, int y)
        {
            return PixelMap[x, y];
        }

        public void ClearPixel(int x, int y)
        {
            PixelMap[x, y] = new Pixel();
        }
        
        public void Clear()
        {
            for (int i = 0; i < MapSize.x; i++)
            {
                for (int j = 0; j < MapSize.y; j++)
                {
                    ClearPixel(i, j);
                }
            }
        }
        public void DrawFunction(Vector2 range, Func<double, double> func, bool onlyInts = true)
        {
            if (onlyInts)
            {
                for (int i = (int)range.x; i < (int)range.y; i++)
                {
                    SetPixel(i, (int)func(i), 0, 0);
                }
            }
        }

        public void DrawFunction(Vector2 range, Func<double, double> func, int type, int paletteRef,bool onlyInts = true)
        {
            if (onlyInts)
            {
                for (int i = (int) range.x; i < (int) range.y; i++)
                {
                    SetPixel(i, (int) func(i), type, paletteRef);
                }
            }
        }
        public void GenerateWorld(int groundHeight)
        {
            for (int i = groundHeight; i >= 0; i--)
            {
                var i1 = i;
                DrawFunction(new Vector2(0, MapSize.x), x => Math.Sin(x/ 32) * 10 + Math.Sin(x / 128) * 20 + i1, 0, i % 8 );
            }
        }

        public void Simulate()
        {
            
        }
    }
}