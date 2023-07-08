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
			mouseClickPos = mouseButton.Position;
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
		this.Position += targetPos;
		this.Rotation = targetPos.Angle();
	}

	private Vector2 GoToMouse()
	{
		return (mouseClickPos - this.Position)/Speed;
	}

	private Vector2 PerceivedCentreOfMass()
	{
		var allCrows = GetTree().GetNodesInGroup(groupName);
		
		Vector2 perceivedCOM = new Vector2();
		foreach(Crow crow in allCrows)
		{
			if (crow != this)
			{
				perceivedCOM += crow.Position;
			}
		}
		
		perceivedCOM /= allCrows.Count - 1;
		return (perceivedCOM - this.Position)/Speed;
	}

	private Vector2 KeepDistance()
	{
		var allCrows = GetTree().GetNodesInGroup(groupName);
		Vector2 collision = new Vector2();
		foreach(Crow crow in allCrows)
		{
			if (crow != this)
			{
				var distance = crow.Position.DistanceTo(this.Position);
				if (distance < Separation)
				{
					collision -= (crow.Position - this.Position);
				}
			}
		}

		return collision;
	}
}
