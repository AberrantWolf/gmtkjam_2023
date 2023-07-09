using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Farmer : EnemyEntity
{
	public Farmer() : base(100) { }

	private Sprite2D Light;
	private bool ouch_playing = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Play();
		this.Light = GetNode<Sprite2D>("Sprite2D");
	}



	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var ClosestCrow = CrowHiveMind.Instance.AllCrows.OrderBy(x => x.Position.DistanceTo(this.Position)).FirstOrDefault();
		var directionToClosestCrow = (this.Position - ClosestCrow.Position).Normalized();
		directionToClosestCrow = directionToClosestCrow.Rotated((float)-1.55);
		this.Light.Rotation = directionToClosestCrow.Angle();

		Helpers.Debounce(() => Shoot(ClosestCrow), 5, delta, this.Name);
		if (ShotFired)
		{
			Helpers.Debounce(() =>
			{
				ShotFired = false;
				//GetNode<Area2D>("Bullet").Hide();
			}, 2, delta, this.Name + "reload");
			var bullet = GetNode<Area2D>("Bullet");
			bullet.Position = new Vector2(bullet.Position.X - ShotVector.X, bullet.Position.Y - ShotVector.Y);
		}
	}

	private Vector2 ShotVector;
	private bool ShotFired = false;
	public void Shoot(Crow closestCrow)
	{
		var vectorToClosestCrow = (this.Position - closestCrow.Position);
		if (vectorToClosestCrow.Length() < 500)
		{
			var bullet = GetNode<Area2D>("Bullet");
			bullet.Position = this.GlobalPosition * (float)0.5;
			bullet.Show();
			GetNode<AudioStreamPlayer>("Sounds/shotgun").Play();
			ShotFired = true;
		}
		this.ShotVector = vectorToClosestCrow.Normalized();
	}

	protected override void onKilled()
	{
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Animation = "death";
		animatedSprite2D.Pause();
		this.Light.Hide();
		var rng = new RandomNumberGenerator();
		var sound = GetNode<AudioStreamPlayer>($"Sounds/dead{rng.RandiRange(1, 3)}");
		sound.Play();
	}
	public void _on_area_entered(Crow crow)
	{
		this.TakeHit();

		if (!ouch_playing)
		{
			var rng = new RandomNumberGenerator();
			var sound = GetNode<AudioStreamPlayer>($"Sounds/ouch{rng.RandiRange(1, 3)}");

			sound.PitchScale = 1 + rng.RandfRange((float)-0.3, (float)0.1);
			sound.Play();
			ouch_playing = true;
		}
	}

	private void _on_dead_1_finished()
	{
		this.QueueFree();
	}


	private void _on_dead_2_finished()
	{
		this.QueueFree();
	}


	private void _on_dead_3_finished()
	{
		this.QueueFree();
	}
	private void _on_ouch_finished()
	{
		ouch_playing = false;
	}
}






