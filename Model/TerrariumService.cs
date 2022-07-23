using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Godot;
using PixelTerrarium.Helpers;

namespace PixelTerrarium.Model
{
    public class TerrariumService : Godot.Object
    {
        public Chunk[,] ChunkMap;
        public Vector2Int MapSize;
        public Palette Palette;
        public Vector2 ChunkSize;
        public Vector2 ChunkMapSize;
        public Image RenderImage;
        private Stopwatch _stopwatch;
        private bool _left = true;
        public List<Material> RegisteredMaterials;
        public Material CurrentMaterial;
        private Random _random;
        private HashSet<Vector2> _changedPositions;

        public TerrariumService()
        {
        }

        public TerrariumService(Vector2Int mapSize, int paletteSize, Vector2 chunkSize)
        {
            _changedPositions = new HashSet<Vector2>();
            RegisteredMaterials = new List<Material>();
            _random = new Random();
            _stopwatch = new Stopwatch();
            ChunkMapSize = new Vector2(mapSize.x / chunkSize.x, mapSize.y / chunkSize.y);
            ChunkMap = new Chunk[(int) (mapSize.x / chunkSize.x), (int) (mapSize.y / chunkSize.y)];
            for (int i = 0; i < mapSize.x / chunkSize.x; i++)
            {
                for (int j = 0; j < mapSize.y / chunkSize.y; j++)
                {
                    ChunkMap[i, j] = new Chunk(chunkSize, new Vector2(i, j));
                }
            }

            ChunkSize = chunkSize;
            MapSize = mapSize;
            Palette = new Palette(paletteSize);
            RenderImage = new Image();
            RenderImage.Create((int) (MapSize.x), (int) (MapSize.y), false, Image.Format.Rgba8);
            RenderImage.Lock();
            RenderImage.SetPixel(0, 0, Colors.Transparent);
            RenderImage.Unlock();
        }

        public void RegisterMaterial(List<Material.MaterialType> types, List<Color> colors)
        {
            var mat = new Material(types, colors);
            
            var startLen = Palette.Length;
            foreach (var clr in colors)    
            {
                Palette.SetColor(Palette.Length, clr);
                Palette.Length++;
            }

            mat.ColorInPaletteRange = new Vector2(startLen, Palette.Length - 1);
            RegisteredMaterials.Add(mat);
        }
        
        public bool SimulatedSetPixel(int x, int y, Material material, int paletteRef)
        {
            if (GetPixel(x, y).Mat == null)
            {
                SetPixel(x, y, material, paletteRef);
                return true;
            }

            return false;
        }


        public bool SimulatedSetPixelSquare(int x, int y, int size, Material mat, int paletteRef)
        {
            bool pixelSet = false;
            for (int i = -size / 2; i <= size / 2; i++)
            {
                for (int j = -size / 2; j <= size / 2; j++)
                {
                    if (x + i < 0 || x + i >= MapSize.x || y + j < 0 || y + j >= MapSize.y) continue;
                    if (GetPixel(x + i, y + j).Mat == null)
                    {
                        if (SimulatedSetPixel(x + i, y + j, mat, paletteRef)) pixelSet = true;
                    }
                }
            }

            return pixelSet;
        }

        public bool SimulatedSetPixelSquare(int x, int y, int size, Material mat)
        {
            bool pixelSet = false;
            for (int i = -size / 2; i <= size / 2; i++)
            {
                for (int j = -size / 2; j <= size / 2; j++)
                {
                    if (x + i < 0 || x + i >= MapSize.x || y + j < 0 || y + j >= MapSize.y) continue;
                    if (GetPixel(x + i, y + j).Mat == null)
                    {
                        var paletteRef = _random.Next((int) mat.ColorInPaletteRange.x,
                            (int) mat.ColorInPaletteRange.y + 1);
                        if (SimulatedSetPixel(x + i, y + j, mat, paletteRef)) pixelSet = true;
                    }
                }
            }

            return pixelSet;
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

            ChunkMap[chunkX, chunkY].Pixels[pixelX, pixelY].Clear();
            RenderImage.Lock();
            RenderImage.SetPixel(x, MapSize.y - y - 1, Colors.Transparent);
            RenderImage.Unlock();
        }

        public void SetPixel(int x, int y, Material mat, int paletteRef)
        {
            try
            {
                if (x >= MapSize.x || y >= MapSize.y) return;
                var chunkX = x / (int) ChunkSize.x;
                var chunkY = y / (int) ChunkSize.y;
                var pixelX = x % (int) ChunkSize.x;
                var pixelY = y % (int) ChunkSize.y;
                ChunkMap[chunkX, chunkY].Pixels[pixelX, pixelY].Set(mat, paletteRef);
                ChunkMap[chunkX, chunkY].IsActive = true;
                RenderImage.Lock();
                RenderImage.SetPixel(x, MapSize.y - y - 1, Palette.GetColor(paletteRef));
                RenderImage.Unlock();
                //GD.Print($"CHUNKX: {chunkX}, CHUNKY: {chunkY}, PIXELX: {pixelX}, PIXELY: {pixelY}");
            }
            catch (Exception e)
            {
                GD.PushError($"[TERRARIUM]: {x}, {y} out of bounds");
            }
        }

        public bool Simulate()
        {
            bool wasActive = false;
            _changedPositions.Clear();
            for (int i = 0; i < ChunkMapSize.x; i++)
            {
                for (int j = 0; j < ChunkMapSize.y; j++)
                {
                    var currChunk = ChunkMap[i, j];
                    if (currChunk.IsActive)
                    {
                        wasActive = true;
                        currChunk.Simulate(this, _left, ref _changedPositions);
                    }

                    currChunk.AdvanceActivity();
                }
            }

            _left = !_left;
            return wasActive;
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
    }
}