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

	[Export]
	public PackedScene DestructionFx;

	private Vector2 lastDirection { get; set; } = Vector2.Right;

	private IEnumerable<Crow> allCrows = null;
	private Vector2 randomMultiplier { get; set; } = Vector2.Right;
	private int AverageWeighting = 500;
	private int CurrentFramesOfAddedWeight = 0;
	private int LocalCount = 0;
	private double monsterFrequency = 0.0;

	public void Die()
	{
		CrowHiveMind.Instance.AllCrows.Remove(this);

		var destFx = DestructionFx.Instantiate();
		GetParent().AddChild(destFx);

		var featherFx = destFx as GpuParticles2D;
		featherFx.Emitting = true;

		destFx.GetNode<GpuParticles2D>("Shockwave").Emitting = true;

		featherFx.Position = Position;

		QueueFree();
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
		this.monsterFrequency = rng.RandfRange((float)0.2, (float)1.1);
		this.AverageWeighting = rng.RandiRange(30, 100);
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

		if (CrowHiveMind.Instance.ShouldMonster)
		{
			toMouse *= 100;
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

		this.Position += lastDirection * ((float)delta * 1000);
		this.Rotation = lastDirection.Angle();


		Helpers.Debounce(Monster, this.monsterFrequency, delta, this.Name);
	}

	private void Monster()
	{
		try
		{
			var tiles = GetParent().GetNode<Tiles>("TileMap");
			var x = (int)this.Position.X / 128;
			var y = (int)this.Position.Y / 128;

			var tilesArray = tiles.TilesArray.Tiles;

			var xlen = tilesArray.GetLength(0);
			var ylen = tilesArray.GetLength(1);

			if (xlen > x && x > 0 && ylen > y && y > 0)
			{
				var tile = tiles.TilesArray.Tiles[x, y];

				if (tile.CanBeAttacked)
				{
					tile.Attack(1);
					var sound = GetNode<AudioStreamPlayer2D>("Monch");
					var rng = new RandomNumberGenerator();
					sound.PitchScale = 1 + rng.RandfRange((float)-0.5, (float)0.5);
					sound.Play();
				}
			}
		}
		catch { }
	}

	private Vector2 listAverage(IEnumerable<Vector2> items)
	{
		return new Vector2(items.Select(x => x.X).Average(), items.Select(x => x.Y).Average());
	}

}

