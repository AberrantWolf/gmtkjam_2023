using Godot;
using System;

public partial class FadeOut : Control
{
  [Export]
  private NodePath _animationPlayer;
  private AnimationPlayer ap;


  [Export]
  private PackedScene scene;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    ap = GetNode<AnimationPlayer>(_animationPlayer);
  }

  public void StartNextScene(PackedScene scene = null)
  {
    if (scene != null) this.scene = scene;
    ap.Play("next-scene");
  }

  public void StartReset()
  {
    ap.Play("reset");
  }

  public void OnAnimationEndNextScene()
  {
    GetTree().ChangeSceneToPacked(scene);
  }

  public void OnAnimationEndReload()
  {
    GetTree().Paused = false;
    GetTree().ReloadCurrentScene();
  }
}
