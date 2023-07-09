using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Farmer : EnemyEntity
{
	public Farmer() : base(100) { }

	private Sprite2D Light;
	private List<Crow> Crows;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Play();
		this.Light = GetNode<Sprite2D>("Sprite2D");
		this.Crows = GetTree().GetNodesInGroup("crows").Select(x=>x as Crow).ToList();
	}



	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var ClosestCrow = this.Crows.OrderBy(x=>x.Position.DistanceTo(this.Position)).FirstOrDefault();

		var directionToClosestCrow = (this.Position - ClosestCrow.Position).Normalized();

		directionToClosestCrow = directionToClosestCrow.Rotated((float)-1.55);

		this.Light.Rotation = directionToClosestCrow.Angle();
	}

	protected override void onKilled()
	{
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Hide();
		this.Light.Hide();
		var rng = new RandomNumberGenerator();
		var sound = GetNode<AudioStreamPlayer2D>($"Sounds/dead{rng.RandiRange(1,3)}");
		sound.Play();
	}
	public void _on_area_entered(Crow crow)
	{
		this.TakeHit();
	}
}
