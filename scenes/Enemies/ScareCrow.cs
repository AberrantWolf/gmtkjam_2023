using Godot;
using System;

public partial class ScareCrow : EnemyEntity
{
	public ScareCrow(): base(10) {

	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	protected override void onKilled()
	{
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Animation = "death";
		var rng = new RandomNumberGenerator();
		var sound = GetNode<AudioStreamPlayer2D>($"Sounds/dead{rng.RandiRange(1,3)}");
		sound.Play();
	}
	public void _on_area_entered(Crow crow)
	{
		this.TakeHit();
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

	
}


