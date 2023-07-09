using Godot;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class CrowHiveMind : Node
{
  public Vector2 CenterOfMass { get; set; } = new Vector2();
  public Vector2 FocusPoint { get; set; } = new Vector2();
  public Vector2 MouseLocation { get; set; } = new Vector2();
  public List<Crow> AllCrows { get; } = new List<Crow>();

  public static CrowHiveMind Instance;

  [Export]
  private Camera2D _mainCam;

  public int CrowCount
  {
	get
	{
	  return AllCrows.Count();
	}
  }

  public override void _EnterTree()
  {
	base._EnterTree();

	Instance = this;
  }

  public override void _Ready()
  {
	ProcessPriority = 0; // Less than Crow.cs, processes first, which is important!!
  }

  // this is where energy goes
  public override void _Input(InputEvent @event)
  {
	base._Input(@event);

	if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left)
	{
	  FocusPoint = _mainCam.GetGlobalMousePosition();
	}
  }

  public override void _Process(double delta)
  {
	base._Process(delta);
	if (CrowCount < 1) return;
	CenterOfMass = listAverage(AllCrows.Select(crow => crow.Position));
	MouseLocation = _mainCam.GetGlobalMousePosition();
  }

  private Vector2 listAverage(IEnumerable<Vector2> items)
  {
	if (CrowCount < 1) return Vector2.Zero;
	return new Vector2(items.Select(pos => pos.X).Average(), items.Select(pos => pos.Y).Average());
  }
}
