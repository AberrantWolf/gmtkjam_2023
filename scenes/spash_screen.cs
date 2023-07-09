using Godot;
using System;

public partial class spash_screen : Control
{
  [Export]
  public PackedScene tutorial;

  [Export]
  public PackedScene game;

  [Export]
  public NodePath _fade;
  public FadeOut fade;

  public override void _Ready()
  {
    fade = GetNode<FadeOut>(_fade);
  }

  private void _on_tutorial_button_down()
  {
    fade.StartNextScene(tutorial);
  }


  private void _on_play_button_down()
  {
    fade.StartNextScene(game);
  }
}



