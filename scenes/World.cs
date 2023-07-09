using Godot;
using System;
using System.Linq;

public partial class World : Node2D
{
  [Export]
  private string groupName = "crows";

  [Export]
  public int crowCount = 0;

  [Export]
  private PackedScene crowScene = ResourceLoader.Load<PackedScene>("res://scenes/Crow.tscn");


  [Export]
  public bool IsTutorial { get; set; } = false;

  private double time_expired = 0.0;
  public double energy = 10;
  private int min_energy = 20;
  private int crow_cost = 20;
  private double energy_usage = 0.3;

  public override void _EnterTree()
  {
	base._EnterTree();
	AddAdditonalCrow();
	AddAdditonalCrow();
	AddAdditonalCrow();
  }



  public override void _Process(double delta)
  {
	if (IsTutorial) return;
	time_expired += delta;

	GetNode<Label>("MainCam/UI/time").Text = $"{(int)time_expired} seconds";
	GetNode<Label>("MainCam/UI/crowcount").Text = $"{crowCount} crows";
	GetNode<Label>("MainCam/UI/energy").Text = $"{(int)energy} energy";

	if (this.energy >= (min_energy + crow_cost))
	{
	  this.energy -= this.crow_cost;
	  this.AddAdditonalCrow();
	}

	this.energy -= crowCount * energy_usage * delta;
  }

  public override void _Ready()
  {
	base._Ready();
  }

  public override void _Input(InputEvent @event)
  {
	if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Right)
	{
	  this.AddAdditonalCrow();
	}
	else if (@event is InputEventKey eventKey && eventKey.Keycode == Key.Delete && eventKey.Pressed)
	{
	  this.UnceremoniouslyMonsterAHelplessCrow();
	}
  }

  public void AddAdditonalCrow()
  {
	var random = new Random();
	var screenSize = GetViewportRect().Size;
	var rng = new RandomNumberGenerator();
	this.crowCount++;
	var crow = crowScene.Instantiate() as Crow;
	crow.Name = $"Crow{this.crowCount}-{rng.RandiRange(0, 1000)}";
	crow.GlobalPosition = new Vector2(rng.RandfRange(0, screenSize.X), rng.RandfRange(0, screenSize.Y));
	var direction = (random.NextDouble() * (Math.PI * 2)) - Math.PI;
	crow.GlobalRotation = (float)direction;
	crow.AddToGroup(groupName);
	crow.SetParent(this);
	AddChild(crow);

	var crows = GetTree().GetNodesInGroup(groupName).Select(x => x as Crow);
	crows.All(x =>
	{
	  x.updateWeightings(crows);
	  return true;
	});
  }

  public void UnceremoniouslyMonsterAHelplessCrow()
  {
	var crows = CrowHiveMind.Instance.AllCrows;
	var helplessCrowToBeMurdered = crows.FirstOrDefault();
	crowCount--;
	helplessCrowToBeMurdered.Die();
	RemoveChild(helplessCrowToBeMurdered);
  }

  public void AddEnergy()
  {
	//		GD.Print("monch");
	this.energy += 3;
  }

  private void _on_button_button_down()
  {
//	GetNode<AudioStreamPlayer>("caw").Play();
	GetParent().GetNode<GameLoopController>("GameLoopController").Restart();
  }
  private void _on_death_timer_timeout()
  {
	if (energy <= 0)
	{
	  this.energy = 0;
	  UnceremoniouslyMonsterAHelplessCrow();
	}
  }
}




