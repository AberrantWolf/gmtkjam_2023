using Godot;
using System;

public partial class MainCam : Camera2D
{
	[Export]
	public float ZoomSpeed = 1.1f;
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
		if(@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
		{
			if(mouseButton.ButtonIndex == MouseButton.WheelDown)
			{
				this.Zoom /= ZoomSpeed;
			}
			else if(mouseButton.ButtonIndex == MouseButton.WheelUp)
			{
				this.Zoom *= ZoomSpeed;
			}
		}
    }
}
