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

		if(@event is InputEventKey key && key.Pressed)
		{
			if(key.Keycode == Key.Up)
			{
				this.Position += new Vector2(0, -10);
			}
			if(key.Keycode == Key.Down)
			{
				this.Position += new Vector2(0, 10);
			}
			if(key.Keycode == Key.Left)
			{
				this.Position += new Vector2(-10, 0);
			}
			if(key.Keycode == Key.Right)
			{
				this.Position += new Vector2(10, 0);
			}
		}
	}
}
