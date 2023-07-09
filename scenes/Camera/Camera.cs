using Godot;
using System;

public partial class Camera : Camera2D
{
	[Export]
	public float ZoomSpeed = 1.1f;

	[Export]
	public float MaxZoom = 10f;

	[Export]
	public float MinZoom = 0.1f;

	[Export]
	public float PanThreshold = 200f;

	[Export]
	public float PanSpeed = 1f;

	public Vector2 Velocity = Vector2.Zero;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var rec = GetViewport().GetVisibleRect();
		var v = GetLocalMousePosition() + rec.Size/2;

		if(rec.Size.X - v.X <= PanThreshold || v.X <= PanThreshold || rec.Size.Y - v.Y <= PanThreshold || v.Y <= PanThreshold)
		{
			this.Position = Position.Lerp(GetGlobalMousePosition(), (float)(PanSpeed * delta));
		}
	}
}
