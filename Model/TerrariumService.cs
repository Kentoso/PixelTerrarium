using System;
using Godot;

namespace PixelTerrarium.Model
{
    public class TerrariumService : Godot.Object
    {
        //public Pixel[,] PixelMap;
        public Chunk[,] ChunkMap;
        public Vector2Int MapSize;
        public Palette Palette;
        public Vector2 ChunkSize;
        public Vector2 ChunkMapSize;

        public TerrariumService()
        {
            
        }
        public TerrariumService(Vector2Int mapSize, int paletteSize, Vector2 chunkSize)
        {
            // PixelMap = new Pixel[mapSize.x, mapSize.y];
            // for (int i = 0; i < mapSize.x; i++)
            // {
            //     for (int j = 0; j < mapSize.y; j++)
            //     {
            //         PixelMap[i, j] = new Pixel();
            //     }
            // }
            ChunkMapSize = new Vector2(mapSize.x / chunkSize.x, mapSize.y / chunkSize.y);
            ChunkMap = new Chunk[(int)(mapSize.x / chunkSize.x), (int)(mapSize.y / chunkSize.y)];
            for (int i = 0; i < mapSize.x / chunkSize.x; i++)
            {
                for (int j = 0; j < mapSize.y / chunkSize.y; j++)
                {
                    ChunkMap[i, j] = new Chunk(chunkSize);
                }
            }

            ChunkSize = chunkSize;
            MapSize = mapSize;
            Palette = new Palette(paletteSize);
        }

        public void SetPixel(int x, int y, int type, int paletteRef)
        {
            try
            {
                if (x >= MapSize.x || y >= MapSize.y) return;
                var p = new Pixel(type, paletteRef);
                var chunkX = x / (int) ChunkSize.x;
                var chunkY = y / (int) ChunkSize.y;
                var pixelX = x % (int) ChunkSize.x;
                var pixelY = y % (int) ChunkSize.y;
                ChunkMap[chunkX, chunkY].Pixels[pixelX, pixelY] = p;
                GD.Print($"CHUNKX: {chunkX}, CHUNKY: {chunkY}, PIXELX: {pixelX}, PIXELY: {pixelY}");
            }
            catch (Exception e)
            {
                GD.PrintErr($"[TERRARIUM]: {x}, {y} out of bounds");
            }
            
        }

        public Pixel GetPixel(int x, int y)
        {
            var chunkX = x / (int) ChunkSize.x;
            var chunkY = y / (int) ChunkSize.y;
            var pixelX = x % (int) ChunkSize.x;
            var pixelY = y % (int) ChunkSize.y;
            return ChunkMap[chunkX, chunkY].Pixels[pixelX, pixelY];
        }

        public void ClearPixel(int x, int y)
        {
            var chunkX = x / (int) ChunkSize.x;
            var chunkY = y / (int) ChunkSize.y;
            var pixelX = x % (int) ChunkSize.x;
            var pixelY = y % (int) ChunkSize.y;
            ChunkMap[chunkX, chunkY].Pixels[pixelX, pixelX] = new Pixel();
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