using System;
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

	[Export]
	public int TileSize = 256;

	[Export]
	public int NumRows = 4;

	[Export]
	public int NumCols = 4;

	private int _MySpriteIndex;

	[Export]
	public Sprite2D CrowSprite;

	[Export]
	public GpuParticles2D[] StartParticles;

	private Vector2 lastDirection { get; set; } = Vector2.Right;

	private IEnumerable<Crow> allCrows = null;
	private Vector2 randomMultiplier { get; set; } = Vector2.Right;
	private int AverageWeighting = 500;
	private int FramesOfAddedWeight = 0;
	private int CurrentFramesOfAddedWeight = 0;
	private int LocalCount = 0;

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left)
		{
			CurrentFramesOfAddedWeight = this.FramesOfAddedWeight;
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		CrowHiveMind.Instance.AllCrows.Remove(this);
	}

	float scarfChance = 0.05f;

	public override void _Ready()
	{
		base._Ready();
		// var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		//animatedSprite2D.Play();
		CrowHiveMind.Instance.AllCrows.Add(this);
		updateWeightings(CrowHiveMind.Instance.AllCrows);

		uint idx = 0;
		uint spriteCount = (uint)(NumRows * NumCols);
		if (GD.Randf() > scarfChance)
		{
			idx = GD.Randi() % spriteCount + 1;
		}
		_MySpriteIndex = (int)idx;
		var row = _MySpriteIndex / NumCols;
		var col = _MySpriteIndex % NumRows;

		CrowSprite.RegionRect = new Rect2(col * TileSize, row * TileSize, TileSize, TileSize);

		foreach (var emitter in StartParticles)
		{
			emitter.Emitting = true;
		}
	}

	public void updateWeightings(IEnumerable<Crow> Crows)
	{
		this.allCrows = Crows;
		var rng = new RandomNumberGenerator();
		this.MonsterBase = rng.RandfRange((float)0.2, (float)1.1);
		this.AverageWeighting = rng.RandiRange(30, 100);
		this.FramesOfAddedWeight = rng.RandiRange(50, 250);
		this.randomMultiplier = new Vector2(rng.Randf(), rng.Randf());
		this.LocalCount = this.allCrows.Count() / 10;
		if (this.LocalCount == 0)
		{
			this.LocalCount = this.allCrows.Count();
		}
	}


	public override void _Process(double delta)
	{
		var mouseLocation = CrowHiveMind.Instance.MouseLocation;

		//Aim towards center

		var allCrows = CrowHiveMind.Instance.AllCrows;
		var centerOfMass = CrowHiveMind.Instance.CenterOfMass;

		//avoid flying close

		var closeCrows = allCrows.Where(crow => crow.Position.DistanceTo(this.Position) < this.LocalCount).ToList();
		var localAverage = listAverage(closeCrows.Select(x => x.Position));

		//Head towards mouse location, avoiding com, equal goal

		var toMouse = (mouseLocation - this.Position).Normalized() * 5;

		if (CurrentFramesOfAddedWeight > 0)
		{
			CurrentFramesOfAddedWeight--;
			toMouse = (CrowHiveMind.Instance.FocusPoint - this.Position).Normalized() * 100;
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

		this.Position += lastDirection * ((float)delta * 300);
		this.Rotation = lastDirection.Angle();

		MonsterTimer -= delta;
		if (MonsterTimer < 0)
		{
			MonsterTimer = MonsterBase;
			Monster();
		}
	}

	private double MonsterBase = 10;
	private double MonsterTimer = 10;

	private void Monster()
	{
		try
		{
			var tiles = GetParent().GetNode<Tiles>("TileMap");
			var x = (int)this.Position.X / 16;
			var y = (int)this.Position.Y / 16;

			var tile = tiles.TilesArray.Tiles[x, y];

			if (tile.CanBeAttacked)
			{
				tile.Attack(1);
				//GetParent<World>().AddAdditonalCrow();
				// Console.WriteLine("Field Monstered");
			}

		}
		catch { }
	}

	private Vector2 listAverage(IEnumerable<Vector2> items)
	{
		return new Vector2(items.Select(x => x.X).Average(), items.Select(x => x.Y).Average());
	}

}

