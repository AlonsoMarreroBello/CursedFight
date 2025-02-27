using Godot;
using System;

public partial class MainMenu : Node2D
{
	private Main main;

	public override void _Ready()
	{
		main = this.GetParent() as Main;
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

		main.isSinglePlayer = true;
		GD.Print("SINGLE PLAYER MODE ON");
		main.AddChild(_new_scene.Instantiate<StageSelection>());
		main.RemoveChild(this);

		// GetTree().ChangeSceneToPacked(_new_scene); // esto elimina el nodo main e impide al resto de nodos acceder de vuelta
	}

	private void _on_multiplayer_button_pressed()
	{
		PackedScene _new_scene = GD.Load<PackedScene>(CursedFight.Global.Scenes.STAGE_SELECTION);
		if (_new_scene == null)
		{
			GD.Print("Error: No se pudo cargar la escena en la ruta: " + CursedFight.Global.Scenes.STAGE_SELECTION);
			return;
		}

		main.isMultiplayer = true;
		GD.Print("MULTIPLAYER MODE ON");
		main.AddChild(_new_scene.Instantiate<StageSelection>());
		main.RemoveChild(this);

		// GetTree().ChangeSceneToPacked(_new_scene);
	}
}
