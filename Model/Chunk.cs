using Godot;

namespace PixelTerrarium.Model
{
    public class Chunk
    {
        public Vector2 Position;
        public Vector2 ChunkSize;
        public Pixel[,] Pixels;
        public bool IsActive = false;
        public bool IsActiveNextFrame = false;
        public Chunk(Vector2 chunkSize, Vector2 position)
        {
            ChunkSize = chunkSize;
            Pixels = new Pixel[(int)ChunkSize.x, (int)ChunkSize.y];
            for (int i = 0; i < chunkSize.x; i++)
            {
                for (int j = 0; j < chunkSize.y; j++)
                {
                    Pixels[i, j] = new Pixel();
                }
            }
            Position = position;
        }

        public void AdvanceActivity()
        {
            IsActive = IsActiveNextFrame;
            IsActiveNextFrame = false;
        }

        public void Simulate(TerrariumService terrariumService, bool left)
        {
            if (left)
            {
                for (int x = 0; x < ChunkSize.x; x++)
                {
                    SubSimY(terrariumService, x);
                }
            }
            else
            {
                for (int x = (int)ChunkSize.x - 1; x >= 0; x--)
                {
                    SubSimY(terrariumService, x);
                }
            }
        }

        private void SubSimY(TerrariumService terrariumService, int x)
        {
            for (int y = 0; y < ChunkSize.y; y++)
            {
                int mapX = (int) (x + ChunkSize.x * Position.x), mapY = (int) (y + ChunkSize.y * Position.y);
                var p = terrariumService.GetPixel(mapX, mapY);
                if (p.Mat == null) continue;
                bool simulated = false;
                if (p.Mat.Types.Contains(Material.MaterialType.Dust))
                {
                    if (mapY == 0) continue;
                    bool moved = false;
                    int leftRight = (int) (GD.Randi() % 2) == 0 ? 1 : -1;
                    bool isLeftAllowed = leftRight == 1 ? mapX != 0 : mapX != terrariumService.MapSize.x - 1;
                    bool isRightAllowed = leftRight == 1 ? mapX != terrariumService.MapSize.x - 1 :  mapX != 0; 
                    if (terrariumService.SimulatedSetPixel(mapX, mapY - 1, p.Mat, (int)p.PaletteRef)
                        || isLeftAllowed && terrariumService.SimulatedSetPixel(mapX - leftRight, mapY - 1, p.Mat, (int)p.PaletteRef)
                        || isRightAllowed && terrariumService.SimulatedSetPixel(mapX + leftRight, mapY - 1, p.Mat, (int)p.PaletteRef))
                    {
                        terrariumService.ClearPixel(mapX, mapY);
                        IsActiveNextFrame = true;
                        simulated = true;
                    }
                }

                if (simulated)
                {
                    if (x == 0 && Position.x != 0)
                    {
                        terrariumService.ChunkMap[(int) Position.x - 1, (int) Position.y].IsActiveNextFrame = true;
                    }

                    if (x == (int) ChunkSize.x - 1 && (int) Position.x != (int) terrariumService.ChunkMapSize.x - 1)
                    {
                        terrariumService.ChunkMap[(int) Position.x + 1, (int) Position.y].IsActiveNextFrame = true;
                    }

                    if (y == 0 && Position.y != 0)
                    {
                        terrariumService.ChunkMap[(int) Position.x, (int) Position.y - 1].IsActiveNextFrame = true;
                    }

                    if (y == (int) ChunkSize.y - 1 && (int) Position.y != (int) terrariumService.ChunkMapSize.y - 1)
                    {
                        terrariumService.ChunkMap[(int) Position.x, (int) Position.y + 1].IsActiveNextFrame = true;
                    }
                }
            }
        }
    }
}