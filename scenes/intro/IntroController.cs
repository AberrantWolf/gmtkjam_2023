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


  [Export]
  public PackedScene scene;
  CrowHiveMind mind = CrowHiveMind.Instance;
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    base._Ready();
    world = GetNode<World>(_world);
    tileManager = GetNode<Tiles>(_tileManager);

    tileManager.SetTiles(new Vector2I(24, 24), new Vector2I(40, 40), TileTypes.Empty);
    tileManager.SetTile(new Vector2I(32, 32), TileTypes.Forest);
    tileManager.SetTile(new Vector2I(32, 32) + new Vector2I(3, 0), TileTypes.Fields);
    tileManager.SetTile(new Vector2I(32, 32) + new Vector2I(3, 1), TileTypes.Fields);
    tileManager.SetTile(new Vector2I(32, 32) + new Vector2I(4, 0), TileTypes.Fields);
    tileManager.SetTile(new Vector2I(32, 32) + new Vector2I(4, 1), TileTypes.Farmhouse);

  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
  }

  public void AddCrow()
  {
    world.AddAdditonalCrow();
  }

  public void StartGame()
  {
    GetTree().ChangeSceneToPacked(scene);
  }
}
