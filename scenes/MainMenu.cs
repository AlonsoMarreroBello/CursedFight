using Godot;
using System;

public partial class MainMenu : Node2D
{
	public override void _Ready()
	{
		AudioStreamPlayer _audio_player = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		AudioStream _new_audio = GD.Load<AudioStream>(CursedFight.Global.Music.MAIN_THEME);
		_audio_player.Stream = _new_audio;
		_audio_player.Play();
	}

	private void _on_exit_button_pressed()
	{
		GetTree().Quit();
	}

	private void _on_singleplayer_button_pressed()
	{
		PackedScene _new_scene = GD.Load<PackedScene>(CursedFight.Global.Scenes.STAGE_SELECTION);
		if (_new_scene == null)
		{
			GD.Print("Error: No se pudo cargar la escena en la ruta: " + CursedFight.Global.Scenes.STAGE_SELECTION);
			return;
		}

		GetTree().ChangeSceneToPacked(_new_scene);

	}

	private void _on_multiplayer_button_pressed()
	{
		PackedScene _new_scene = GD.Load<PackedScene>(CursedFight.Global.Scenes.STAGE_SELECTION);
		if (_new_scene == null)
		{
			GD.Print("Error: No se pudo cargar la escena en la ruta: " + CursedFight.Global.Scenes.STAGE_SELECTION);
			return;
		}

		GetTree().ChangeSceneToPacked(_new_scene);

	}


}
