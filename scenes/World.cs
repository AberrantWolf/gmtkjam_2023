using Godot;
using System;

public partial class World : Node2D
{
	[Export]
	private string groupName = "crows";
	
	[Export]
	private int crowCount = 10;

	[Export]
	private PackedScene crowScene = ResourceLoader.Load<PackedScene>("res://scenes/Crow.tscn");

	public override void _EnterTree()
	{
		base._EnterTree();
		AddCrows();
	}

	public override void _Ready()
	{
		base._Ready();
	}

	private void AddCrows()
	{
		var random = new Random();
		var screenSize = GetViewportRect().Size;
		var rng = new RandomNumberGenerator();
		for (int i = 0; i < crowCount; i++)
		{
			var crow = crowScene.Instantiate() as Crow;
			crow.Name = $"Crow{i}";
			crow.GlobalPosition = new Vector2(rng.RandfRange(0, screenSize.X), rng.RandfRange(0, screenSize.Y));
			var direction = (random.NextDouble() * (Math.PI * 2)) - Math.PI;
			crow.GlobalRotation = (float)direction;
			crow.AddToGroup(groupName);
			AddChild(crow);
		}
	}
}
