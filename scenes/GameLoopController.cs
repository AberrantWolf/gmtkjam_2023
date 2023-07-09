using Godot;
using System;

public partial class GameLoopController : Node
{
	[Export]
	private NodePath _world;

	private World world;
	private bool game_over = false;
	private double time_alive = 0.0;
	
	public override void _Ready()
	{
		world = GetNode<World>(_world);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!game_over && world.crowCount <= 0){
			game_over = true;
			GameOver();
		}
//		if(!game_over && world.energy <= 0){
//			game_over = true;
//			GameOver();
//		}
		time_alive += delta;
	}
	
	
	public void GameOver()
	{
		var camera = world.GetNode<Camera2D>("MainCam");
		world.GetNode<Label>("MainCam/UI/crowcount").Text = "0 crows";
		camera.Zoom = new Vector2I(4,4);
		camera.GetNode<CanvasLayer>("GameOver").Show();
		camera.GetNode<Label>("GameOver/timer").Text = $"you survived {(int) time_alive} seconds";
		
		GetTree().Paused = true;
	}
	
	public void Restart()
	{
//		world.QueueFree();
//		time_alive = 0.0;
//		var scene = ResourceLoader.Load<PackedScene>("res://Scenes/World.tscn").Instantiate();
//		GetParent().AddChild(scene);
//		world = (World) scene;
//		var gameover = GetParent().GetNode<CanvasLayer>("World/MainCam/GameOver");
//		gameover.Hide();
//		game_over = false;
		GetTree().Paused = false;
		GetTree().ReloadCurrentScene();
	}
	
	public double GetTimeAlive(){
		return time_alive;
	}
}



