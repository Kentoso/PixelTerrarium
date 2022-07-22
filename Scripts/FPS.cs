using Godot;
using System;
using System.Diagnostics;

public class FPS : Label
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private int _frameCounter = 0;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public override void _Process(float delta)
    {
        if (_frameCounter % 10 == 0)
        {
            Text = Performance.GetMonitor(Performance.Monitor.TimeFps).ToString();
        }
        _frameCounter++;
    }
    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
