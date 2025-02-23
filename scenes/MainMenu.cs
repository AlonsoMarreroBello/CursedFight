using Godot;
using System;

public partial class MainMenu : Node2D
{
	public override void _Ready()
	{
	}

	private void _on_exit_button_pressed()
	{
		GetTree().Quit();
	}
}
