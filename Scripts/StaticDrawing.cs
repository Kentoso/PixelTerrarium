using Godot;
using System;
using PixelTerrarium.Model;

public class StaticDrawing : Node2D
{
    private TerrariumService _terrariumService;

    private int _tileSize;
    
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
       
    }

    public override void _Draw()
    {
        if (_terrariumService == null) return;

        var windowSize = OS.WindowSize;
        var mapSize = (Vector2) _terrariumService.MapSize;
        var toCenter = (windowSize - mapSize * _tileSize) / 2;
        for (int i = 0; i < _terrariumService.ChunkMapSize.x; i++)
        {
            for (int j = 0; j < _terrariumService.ChunkMapSize.y; j++)
            {
                var chunk = _terrariumService.ChunkMap[i, j];
                DrawRect(new Rect2(i * _terrariumService.ChunkSize.x * _tileSize + toCenter.x, j *
                        _terrariumService.ChunkSize.y * _tileSize + toCenter.y,
                        _terrariumService.ChunkSize * _tileSize),
                    Colors.Yellow, false);
            }
        }
    }

    public void _on_Terrarium_DrawStatic(TerrariumService terrariumService, int tileSize)
    {
        if (_terrariumService == null)
        {
            _terrariumService = terrariumService;
            _tileSize = tileSize;
        }

        Update();
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
