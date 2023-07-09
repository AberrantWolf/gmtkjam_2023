using Godot;
using System;

public partial class spash_screen : Control
{
	[Export]
  	public PackedScene tutorial;
	
	[Export]
  	public PackedScene game;
	
	

	
	private void StartGame()
	{
		GetTree().ChangeSceneToPacked(game);
	}
	
	private void StartTutorial()
	{
		GetTree().ChangeSceneToPacked(tutorial);
	}
	
	private void _on_tutorial_button_down()
	{
		StartTutorial();
	}


	private void _on_play_button_down()
	{
		StartGame();
	}
}



