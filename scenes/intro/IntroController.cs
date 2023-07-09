using Godot;
using System;

public partial class IntroController : Node
{
  [Export]
  public NodePath _world;
  public World world;

  [Export]
  public NodePath _tileManager;
  public Tiles tileManager;

  CrowHiveMind mind = CrowHiveMind.Instance;
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    world = GetNode<World>(_world);
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
  }

  public void AddCrow()
  {
    world.AddAdditonalCrow();
  }
}
