using Godot;
using System;
using System.Linq;

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
  [Export]
  public float LimLeft = -4096f;
  [Export]
  public float LimTop = -4096f;
  [Export]
  public float LimRight = 4096f;
  [Export]
  public float LimBot = 4096f;

  private Vector2 zoomTarget;

  public override void _Ready()
  {
    base._Ready();
    zoomTarget = Zoom;
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
    var rect = GetViewport().GetVisibleRect();
    var mousePos = GetViewport().GetMousePosition();

    var topLeft = rect.Position;
    var botRight = rect.End;

    Vector2 pan = Vector2.Zero;

    // if we're in the pan threshold
    pan.X = Mathf.Min((mousePos.X - (botRight.X - PanThreshold)) / PanThreshold, 1);
    // if we're not in the threshold, try the other side
    if (pan.X < 0)
    {
      pan.X = Mathf.Max((mousePos.X - (topLeft.X + PanThreshold)) / PanThreshold, -1);
      if (pan.X > 0) pan.X = 0;
    }

    // if we're in the pan threshold
    pan.Y = Mathf.Min((mousePos.Y - (botRight.Y - PanThreshold)) / PanThreshold, 1);
    // if we're not in the threshold, try the other side
    if (pan.Y < 0)
    {
      pan.Y = Mathf.Max((mousePos.Y - (topLeft.Y + PanThreshold)) / PanThreshold, -1);
      if (pan.Y > 0) pan.Y = 0;
    }

    // GD.Print(Position + " " + Position + pan * PanSpeed * (float)delta);
    pan = Position + pan * PanSpeed * 1 / Zoom * (float)delta;
    Position = pan.Clamp(new Vector2(LimLeft, LimTop), new Vector2(LimRight, LimBot));
    GD.Print(Position);
    Zoom = Zoom.Lerp(zoomTarget, 0.2f);
  }

  static MouseButton[] scroll = new MouseButton[] { MouseButton.WheelDown, MouseButton.WheelUp };

  public override void _Input(InputEvent @event)
  {
    if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
    {
      if (mouseButton.ButtonIndex == MouseButton.WheelDown)
      {
        zoomTarget /= ZoomSpeed;
      }
      else if (mouseButton.ButtonIndex == MouseButton.WheelUp)
      {
        zoomTarget *= ZoomSpeed;
      }
    }
    zoomTarget = zoomTarget.Clamp(MinZoom * Vector2.One, MaxZoom * Vector2.One);
  }
}
