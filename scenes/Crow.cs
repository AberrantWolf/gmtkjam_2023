using Godot;
using System.Linq;

public partial class Crow : Area2D
{
	[Export]
	private string groupName = "crows";

	[Export]
	public int Speed = 100;

	[Export]
	public int Separation = 100;

	private Vector2 mouseClickPos { get; set; }

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left)
		{
			mouseClickPos = GetGlobalMousePosition();
		}
	}

	public override void _Ready()
	{
		base._Ready();
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		//animatedSprite2D.Play();
	}

	public override void _Process(double delta)
	{
		var mousePos = GoToMouse();
		var com = PerceivedCentreOfMass();
		var distance = KeepDistance();
		
		var targetPos = com+distance+mousePos;
		this.GlobalPosition += targetPos;
		this.Rotation = targetPos.Angle();
	}

	private Vector2 GoToMouse()
	{
		return (mouseClickPos - this.GlobalPosition)/Speed;
	}

	private Vector2 PerceivedCentreOfMass()
	{
		var allCrows = GetTree().GetNodesInGroup(groupName);
		
		Vector2 perceivedCOM = new Vector2();
		foreach(Crow crow in allCrows)
		{
			if (crow != this)
			{
				perceivedCOM += crow.GlobalPosition;
			}
		}
		
		perceivedCOM /= allCrows.Count - 1;
		return (perceivedCOM - this.GlobalPosition)/Speed;
	}

	private Vector2 KeepDistance()
	{
		var allCrows = GetTree().GetNodesInGroup(groupName);
		Vector2 collision = new Vector2();
		foreach(Crow crow in allCrows)
		{
			if (crow != this)
			{
				var distance = crow.GlobalPosition.DistanceTo(this.GlobalPosition);
				if (distance < Separation)
				{
					collision -= (crow.GlobalPosition - this.GlobalPosition);
				}
			}
		}

		return collision;
	}
}
