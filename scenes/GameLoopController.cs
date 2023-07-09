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
		time_alive += delta;
		GD.Print(world.crowCount);
		GD.Print(game_over);
	}
	
	
	public void GameOver()
	{
		var camera = world.GetNode<Camera2D>("MainCam");
		camera.Zoom = new Vector2I(4,4);
		camera.GetNode<Control>("GameOver").Show();
		camera.GetNode<Label>("GameOver/timer").Text = $"you survived {(int) time_alive} seconds";
		
		GetTree().Paused = true;
	}
	
	public void Restart()
	{
		world.QueueFree();

		var scene = ResourceLoader.Load<PackedScene>("res://Scenes/World.tscn").Instantiate();
		// Add the node as a child of the node the script is attached to.
		GetParent().AddChild(scene);
		world = (World) scene;
		var gameover = GetParent().GetNode<Control>("World/MainCam/GameOver");
		gameover.Hide();
		game_over = false;
		GetTree().Paused = false;
	}
	
	public double GetTimeAlive(){
		return time_alive;
	}
}



