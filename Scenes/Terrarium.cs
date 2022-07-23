using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using PixelTerrarium.Model;
using Material = PixelTerrarium.Model.Material;

public class Terrarium : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private TerrariumService _terrariumService;
    private Vector2 _translate;
    private float _defaultTranslateDelta = 2f;
    private float _translateDelta;
    private const int _tileSize = 10;
    private int _baseTileSize = 10;
    private uint _frameCounter = 0;
    private Camera2D _camera;
    private float _zoomDelta = 0.25f;
    private int _brushSize = 1;
    private ImageTexture _renderTexture;
    private Label _simsPerSec;
    
    private bool _stopSimulation = false;
    [Signal]
    public delegate void DrawStatic(TerrariumService terrariumService, int tileSize);

    public override void _EnterTree()
    {
        _renderTexture = new ImageTexture();
        _translateDelta = _defaultTranslateDelta;
        _translate = new Vector2(0, 0);
        _terrariumService = new TerrariumService(new Vector2Int(250, 250), 129, new Vector2(5, 5));
        _terrariumService.RegisterMaterial(new List<Material.MaterialType>() {PixelTerrarium.Model.Material.MaterialType.Dust},
            new List<Color>()
            {
                new Color("f6d7b0"), new Color("eccca2"), new Color("e7c496"),
                new Color("#f2d2a9")
            } );
        _terrariumService.CurrentMaterial = _terrariumService.RegisteredMaterials[0];
    }

    public override void _Ready()
    {
        _camera = GetParent().GetNode<Camera2D>("./Camera");
        _simsPerSec = GetParent().GetNode<Label>("./CanvasLayer/Control/SimsPerSec");
        GetTree().Root.Connect("size_changed", this, "OnResize");
        EmitSignal(nameof(DrawStatic), _terrariumService, _tileSize);
    }

    public override void _Process(float delta)
    {
        
        if (Input.IsMouseButtonPressed((int) ButtonList.Left))
        {
            var mousePos = GetGlobalMousePosition();
            var mapSize = (Vector2) _terrariumService.MapSize;
            var windowSize = OS.WindowSize;
            var toCenter = (windowSize - mapSize * _tileSize) / 2;
            if (new Rect2(toCenter, mapSize * _tileSize).HasPoint(mousePos - _translate))
            {
                var newMousePos = mousePos - toCenter - _translate - new Vector2(0, _tileSize * mapSize.y);
                newMousePos *= new Vector2(1, -1);
                newMousePos /= _tileSize;
                if (_terrariumService.SimulatedSetPixelSquare((int) newMousePos.x, (int) newMousePos.y, _brushSize, _terrariumService.CurrentMaterial));
            }
        }
        
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_frameCounter % 1 == 0)
        {

            if (_terrariumService.Simulate())
            {
                Update();
                _simsPerSec.Text = "Without texturemap and multi thread (?):" + Performance.GetMonitor(Performance.Monitor.Render2dDrawCallsInFrame).ToString();
            }
        }
        bool needToUpdate = false;
        _translateDelta = Input.IsActionPressed("speed_translation")
            ? 2 * _defaultTranslateDelta
            : _defaultTranslateDelta;
        
        // if (_frameCounter % 8 == 0)
        // {
        //     var palette = _terrariumService.Palette.AnimateRange(0, 8, true);
        //     _terrariumService.Palette = palette;
        //     needToUpdate = true;
        // }
        _frameCounter++;

        
        
        if (Input.IsActionPressed("translate_up"))
        {
            _translate.y += _translateDelta;
            Position = _translate;
        }

        if (Input.IsActionPressed("translate_left"))
        {   
            _translate.x += _translateDelta;
            Position = _translate;
        }

        if (Input.IsActionPressed("translate_down"))
        {   
            _translate.y -= _translateDelta;
            Position = _translate;
        }
        if (Input.IsActionPressed("translate_right"))
        {
            _translate.x -= _translateDelta;
            Position = _translate;
        }

        if (needToUpdate)
        {
            Update();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.IsPressed())
            {
                if (mouseEvent.ButtonIndex == (int)ButtonList.WheelUp)
                {
                    if (_camera.Zoom.x > 2 * _zoomDelta)
                    {
                        _camera.Zoom -= new Vector2(_zoomDelta, _zoomDelta);
                    }
                }
                else if (mouseEvent.ButtonIndex == (int)ButtonList.WheelDown)
                {
                    _camera.Zoom += new Vector2(_zoomDelta, _zoomDelta);
                }
                else if (mouseEvent.ButtonIndex == (int) ButtonList.Left)
                {
                    // var mousePos = GetCanvasTransform().AffineInverse() * mouseEvent.Position;
                    // GD.PushWarning($"{mousePos}");
                    //
                    // var mapSize = (Vector2) _terrariumService.MapSize;
                    // var windowSize = OS.WindowSize;
                    // var toCenter = (windowSize - mapSize * _tileSize) / 2;
                    // if (new Rect2(toCenter, mapSize * _tileSize).HasPoint(mousePos))
                    // {
                    //     var newMousePos = mousePos - toCenter - _translate - new Vector2(0, _tileSize * mapSize.y);
                    //     newMousePos *= new Vector2(1, -1);
                    //     newMousePos /= _tileSize;
                    //     _terrariumService.SetPixel((int)newMousePos.x, (int)newMousePos.y, 0, 0);
                    //     GD.Print($"MousePos: {mousePos}, NewMousePos: {newMousePos}");
                    //     Update();
                    // }
                }
            }
        }
        else if (@event is InputEventKey keyEvent)
        {
            if (keyEvent.IsPressed())
            {
                if (keyEvent.Scancode == (int)KeyList.Minus)
                {
                    if (_brushSize > 1) _brushSize--;
                }

                if (keyEvent.Scancode == (int) KeyList.Equal || keyEvent.Scancode == (int) KeyList.Plus)
                {
                    _brushSize++;
                }

                if (keyEvent.Scancode == (int)KeyList.Space)
                {
                    _stopSimulation = !_stopSimulation;
                }
            }
        }
    }

    public override void _Draw()
    {
        var windowSize = OS.WindowSize;
        var mapSize = (Vector2)_terrariumService.MapSize;
        var toCenter = (windowSize - mapSize * _tileSize) / 2;
        //DrawCircle(GetGlobalMousePosition(), 5f, Colors.Red);
        //DrawRect(new Rect2(toCenter, mapSize * _tileSize), Colors.Red, false);
        // for (int i = 0; i < _terrariumService.ChunkMapSize.x; i++)
        // {
        //     for (int j = 0; j < _terrariumService.ChunkMapSize.y; j++)
        //     {
        //         var chunk = _terrariumService.ChunkMap[i, j];
        //         if (chunk.IsActive)
        //         {
        //             DrawRect(new Rect2(i * _terrariumService.ChunkSize.x * _tileSize + toCenter.x, 
        //                     windowSize.y - (j + 1) * _terrariumService.ChunkSize.y * _tileSize - toCenter.y,
        //                     _terrariumService.ChunkSize * _tileSize),
        //                 Colors.Red, false, 5f);
        //         }
        //     }
        // }
        
        // for (int x = 0; x < mapSize.x; x++)
        // {
        //     for (int y = 0; y < mapSize.y; y++)
        //     {
        //         var p = _terrariumService.GetPixel(x, y);
        //         if (p.Type == null) continue;
        //         var xPos = (x) * _tileSize + toCenter.x;
        //         var yPos = windowSize.y - (y  + 1) * _tileSize - toCenter.y;
        //         //GD.Print($"POS: {xPos}, {yPos}");
        //         if (p.Type != null && p.PaletteRef != null)
        //         {
        //             DrawRect(new Rect2(xPos, yPos, _tileSize, _tileSize), _terrariumService.Palette.GetColor((int)p.PaletteRef));
        //         }
        //     }
        // }
        
        _renderTexture.CreateFromImage(_terrariumService.RenderImage);
        _renderTexture.Flags = 1;
        DrawTextureRect(_renderTexture, new Rect2(toCenter, mapSize * _tileSize), false);
    }

    private void OnResize()
    {
        EmitSignal(nameof(DrawStatic), _terrariumService, _tileSize);
        Update();
    }

    public override void _ExitTree()
    {
        _terrariumService.Free();
    }

}
