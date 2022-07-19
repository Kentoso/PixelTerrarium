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
    private int _tileSize = 50;
    private int _baseTileSize = 10;
    private uint _frameCounter = 0;

    private float period1 = 0.1f;

    private float period2 = 0.1f;


    public override void _Ready()
    {
        _translateDelta = _defaultTranslateDelta;
        // GD.Print(OS.WindowSize);
        _translate = new Vector2(0, 0);
        GetTree().Root.Connect("size_changed", this, "OnResize");
        _terrariumService = new TerrariumService(new Vector2Int(256, 256), 129);
        _terrariumService.Palette.SetColor(0, Colors.Gold);
        _terrariumService.Palette.SetColor(1, Colors.Blue);
        _terrariumService.Palette.CreateLinearGradient(0, 9, Colors.Blue, Colors.Gold);
        RandomNumberGenerator r = new RandomNumberGenerator();  
        //
        // int clrn = 0;
        // for (int i = 0; i < _terrariumService.MapSize.x; i++)
        // {
        //     for (int j = 0; j < _terrariumService.MapSize.y; j++)
        //     {
        //         _terrariumService.SetPixel(i, j, 0, clrn++ % 64);
        //
        //     }
        // }
        //_terrariumService.DrawFunction(new Vector2(0, 256), x => Math.Sin(x / 32) * 5 + Math.Cos(x / 16) * 15 + 128);
        //_terrariumService.DrawFunction(new Vector2(0, 256), x => Math.Sin(x / period1) * 5 + Math.Cos(x / period2) * 15 + 128);
        _terrariumService.GenerateWorld(128);
    }
    // Called when the node enters the scene tree for the first time.

    public override void _PhysicsProcess(float delta)
    {
        _frameCounter++;
        bool needToUpdate = false;
        _translateDelta = Input.IsActionPressed("speed_translation")
            ? 2 * _defaultTranslateDelta
            : _defaultTranslateDelta;
        if (_frameCounter % 8 == 0)
        {
            var palette = _terrariumService.Palette.AnimateRange(0, 8, true);
            _terrariumService.Palette = palette;
            needToUpdate = true;
        }
        
        
        if (Input.IsActionPressed("translate_up"))
        {
            _translate.y += _translateDelta;
            Position = _translate;
            //needToUpdate = true;
        }

        if (Input.IsActionPressed("translate_left"))
        {   
            _translate.x += _translateDelta;
            Position = _translate;
            //needToUpdate = true;
        }

        if (Input.IsActionPressed("translate_down"))
        {   
            _translate.y -= _translateDelta;
            Position = _translate;
            //needToUpdate = true;
        }
        if (Input.IsActionPressed("translate_right"))
        {
            _translate.x -= _translateDelta;
            Position = _translate;
            //needToUpdate = true;
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
                    _tileSize++;
                    Update();
                }
                else if (mouseEvent.ButtonIndex == (int)ButtonList.WheelDown)
                {
                    if (_tileSize > 0)
                    {
                        _tileSize--;
                    }
                    Update();
                }
            }
        }
        else if (@event is InputEventKey keyEvent)
        {
            if (keyEvent.IsActionPressed("speed_translation"))
            {
                
            }
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

                //var xPos = (x) * _tileSize + toCenter.x + _translate.x;
                var xPos = (x) * _tileSize + toCenter.x;
                //var yPos = windowSize.y - (y  + 1) * _tileSize - toCenter.y + _translate.y;
                var yPos = windowSize.y - (y  + 1) * _tileSize - toCenter.y;
                if (p.Type != null && p.PaletteRef != null)
                {
                    DrawRect(new Rect2(xPos, yPos, _tileSize, _tileSize), _terrariumService.Palette.GetColor((int)p.PaletteRef));
                }
            }
        }
    }

    private void OnResize()
    {
        Update();
        //Position = OS.WindowSize
    }

    // public void _on_Period1_value_changed(float val)
    // {
    //     period1 = val;
    //     _terrariumService.Clear();
    //     _terrariumService.DrawFunction(new Vector2(0, 256), x => Math.Asin(Math.Sin(x / period1)) * 20 +
    //                                                              Math.Sin(
    //                                                                  x / period1) * 5 + Math.Cos(x / period2) * 15 +
    //                                                              128);       
    //     Update();
    // }    
    // public void _on_Period2_value_changed(float val)
    // {
    //     period2 = val;
    //     _terrariumService.Clear();
    //     _terrariumService.DrawFunction(new Vector2(0, 256), x => Math.Asin(Math.Sin(x / period1)) * 20 +
    //                                                              Math.Sin(
    //                                                                  x / period1) * 5 + Math.Cos(x / period2) * 15 +
    //                                                              128);        
    //     Update();
    // }
    //
    // public void _on_Clear_button_down()
    // {
    //     _terrariumService.Clear();
    //     Update();
    // }
    //
    // public void _on_Draw_button_down()
    // {
    //     _terrariumService.DrawFunction(new Vector2(0, 256), x => Math.Sin(x / 32) * 5 + Math.Cos(x / 16) * 15 + 128);
    //     Update();
    // }
    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
