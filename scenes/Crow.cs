using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Crow : Area2D
{
	[Export]
	private string groupName = "crows";

	[Export]
	public int Speed = 100;

	[Export]
	public int Separation = 100;

	private Vector2? mouseClickPos { get; set; } = null;
	private Vector2 lastDirection {get; set; } = Vector2.Right;

	private IEnumerable<Crow> allCrows = null;
	private Vector2 randomMultiplier {get; set; } = Vector2.Right;
	private int AverageWeighting = 500;
	private int FramesOfAddedWeight = 0;
	private int CurrentFramesOfAddedWeight = 0;
	private int LocalCount = 0;

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left)
		{
			mouseClickPos = mouseButton.Position;
			CurrentFramesOfAddedWeight = this.FramesOfAddedWeight;
		}
	}

	public override void _Ready()
	{
		base._Ready();
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		//animatedSprite2D.Play();
		var allCrows = GetTree().GetNodesInGroup(groupName).Select(x=>x as Crow);
		updateWeightings(allCrows);
	}

	public void updateWeightings(IEnumerable<Crow> Crows) {
		this.allCrows = Crows;
		var rng = new RandomNumberGenerator();
		this.AverageWeighting = rng.RandiRange(30,100);
		this.FramesOfAddedWeight = rng.RandiRange(50,250);
		this.randomMultiplier = new Vector2(rng.Randf(), rng.Randf());
		this.LocalCount = this.allCrows.Count()/10;
		if(this.LocalCount == 0) {
			this.LocalCount = this.allCrows.Count();
		}
	}

	public override void _Process(double delta)
	{
		var mouseLocation = GetViewport().GetMousePosition();


		//Aim towards center

		var allPositions = this.allCrows.Where(x=>x == this);
		var centerOfMass = listAverage(allPositions.Select(x=>x.Position));

		//avoid flying close
		
		var closeAverage = allPositions.Where(x=>x.Position.DistanceTo(this.Position) < this.LocalCount).ToList();
		var localAverage = listAverage(closeAverage.Select(x=>x.Position));

		//Head towards mouse location, avoiding com, equal goal

		var toMouse = (mouseLocation - this.Position).Normalized() * 2;
		
		if(CurrentFramesOfAddedWeight > 0) {
			CurrentFramesOfAddedWeight--;
			toMouse = (this.mouseClickPos.Value - this.Position).Normalized() * 10;
		}

		var toCOM = (centerOfMass - this.Position).Normalized();
		var toLocalCOM = (this.Position - localAverage).Normalized();

		var targetDirection = (
			randomMultiplier
			+ toMouse 
			+ toCOM 
			+ (toLocalCOM * 1000)
		);

		this.lastDirection = ((lastDirection * this.AverageWeighting) + targetDirection).Normalized();

		this.Position += lastDirection * ((float)delta * 500);
		this.Rotation = lastDirection.Angle();
	}

	private Vector2 listAverage(IEnumerable<Vector2> items) {
		return new Vector2(items.Select(x=>x.X).Average(), items.Select(x=>x.Y).Average());
	}

}

