using Godot;
using System;
using System.Linq;

public partial class World : Node2D
{
	[Export]
	private string groupName = "crows";
	
	[Export]
	private int crowCount = 3;

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

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Right)
		{
			this.AddAdditonalCrow();
		}
		else if (@event is InputEventKey eventKey && eventKey.Keycode == Key.Delete)
		{
			this.UnceremoniouslyMonsterAHelplessCrow();
		}
	}

	public void AddAdditonalCrow()
	{
		var random = new Random();
		var screenSize = GetViewportRect().Size;
		var rng = new RandomNumberGenerator();
		this.crowCount++;
		var crow = crowScene.Instantiate() as Crow;
		crow.Name = $"Crow{this.crowCount}-{rng.RandiRange(0,1000)}";
		crow.GlobalPosition = new Vector2(rng.RandfRange(0, screenSize.X), rng.RandfRange(0, screenSize.Y));
		var direction = (random.NextDouble() * (Math.PI * 2)) - Math.PI;
		crow.GlobalRotation = (float)direction;
		crow.AddToGroup(groupName);
		AddChild(crow);
	}

	public void UnceremoniouslyMonsterAHelplessCrow()
	{
		var crows = GetTree().GetNodesInGroup(groupName).Select(x=>x as Crow);
		var helplessCrowToBeMurdered = crows.FirstOrDefault();
		RemoveChild(helplessCrowToBeMurdered);
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
