using Godot;
using System;

public partial class MainMenu : Node2D
{
	public override void _Ready()
	{
		AudioStreamPlayer _audio_player = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		AudioStream _new_texture = GD.Load<AudioStream>("ras://assets/Music/the-god-beyond-the-man-kaledon-main.mp3");
		_audio_player.Play();
	}

	private void _on_exit_button_pressed()
	{
		GetTree().Quit();
	}

	private void _on_singleplayer_button_pressed()
	{
		PackedScene _new_scene = GD.Load<PackedScene>("res://scenes/StageSelection.tscn");
		if (_new_scene == null)
		{
			GD.Print("Error: No se pudo cargar la escena en la ruta: res://scenes/StageSelection");
			return;
		}

		GetTree().ChangeSceneToPacked(_new_scene);

	}

	private void _on_multiplayer_button_pressed()
	{
		PackedScene _new_scene = GD.Load<PackedScene>("res://scenes/StageSelection.tscn");
		if (_new_scene == null)
		{
			GD.Print("Error: No se pudo cargar la escena en la ruta: res://scenes/StageSelection");
			return;
		}

		GetTree().ChangeSceneToPacked(_new_scene);

	}


}
