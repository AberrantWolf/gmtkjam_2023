using Godot;
using System;

public partial class GameLoopController : Node
{
  [Export]
  private NodePath _world;


  [Export]
  public NodePath _fade;
  public FadeOut fade;

  private World world;
  private bool game_over = false;
  private double time_alive = 0.0;

  public override void _Ready()
  {
    world = GetNode<World>(_world);
    fade = GetNode<FadeOut>(_fade);
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
    if (!game_over && world.crowCount <= 0)
    {
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
    camera.Zoom = new Vector2I(4, 4);
    camera.GetNode<CanvasLayer>("GameOver").Show();
    camera.GetNode<Label>("GameOver/timer").Text = $"you survived {(int)time_alive} seconds";

    GetTree().Paused = true;
  }

  public void Restart()
  {
    // GetTree().Paused = false;
    // GetTree().ReloadCurrentScene();

    GD.Print("wowee");
    // fade.StartReset();
    fade.StartNextScene();
  }

  public double GetTimeAlive()
  {
    return time_alive;
  }
}
