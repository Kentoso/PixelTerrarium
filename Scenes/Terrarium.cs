using Godot;
using System;
using PixelTerrarium.Model;

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
    private float period1 = 0.1f;
    private Camera2D _camera;
    private float period2 = 0.1f;
    private float _zoomDelta = 0.25f;

    [Signal]
    public delegate void DrawStatic(TerrariumService terrariumService, int tileSize);
    public override void _Ready()
    {
        _translateDelta = _defaultTranslateDelta;
        _camera = GetParent().GetNode<Camera2D>("./Camera");
        _translate = new Vector2(0, 0);
        GetTree().Root.Connect("size_changed", this, "OnResize");
        _terrariumService = new TerrariumService(new Vector2Int(256, 256), 129, new Vector2(8, 8));
        _terrariumService.SetPixel(0, 0, 0, 1);
        _terrariumService.Palette.SetColor(0, Colors.Gold);
        _terrariumService.Palette.SetColor(1, Colors.Red);
        EmitSignal(nameof(DrawStatic), _terrariumService, _tileSize);
    }

    public override void _Process(float delta)
    {
        if (Input.IsMouseButtonPressed((int) ButtonList.Left))
        {
            var mousePos = GetLocalMousePosition();
            var mapSize = (Vector2) _terrariumService.MapSize;
            var windowSize = OS.WindowSize;
            var toCenter = (windowSize - mapSize * _tileSize) / 2;
            if (new Rect2(toCenter, mapSize * _tileSize).HasPoint(mousePos))
            {
                var newMousePos = mousePos - toCenter - _translate - new Vector2(0, _tileSize * mapSize.y);
                newMousePos *= new Vector2(1, -1);
                newMousePos /= _tileSize;
                _terrariumService.SetPixel((int) newMousePos.x, (int) newMousePos.y, 0, 0);
                Update();
            }
        }
        else
        {
            GD.PushWarning("b");
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        _frameCounter++;
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
            
        }
    }

    public override void _Draw()
    {
        var windowSize = OS.WindowSize;
        var mapSize = (Vector2)_terrariumService.MapSize;
        var toCenter = (windowSize - mapSize * _tileSize) / 2;
        DrawRect(new Rect2(toCenter, mapSize * _tileSize), Colors.Black, false);
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                var p = _terrariumService.GetPixel(x, y);
                if (p.Type == null) continue;
                var xPos = (x) * _tileSize + toCenter.x;
                var yPos = windowSize.y - (y  + 1) * _tileSize - toCenter.y;
                //GD.Print($"POS: {xPos}, {yPos}");
                if (p.Type != null && p.PaletteRef != null)
                {
                    DrawRect(new Rect2(xPos, yPos, _tileSize, _tileSize), _terrariumService.Palette.GetColor((int)p.PaletteRef));
                }
            }
        }
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
