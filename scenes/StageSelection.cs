using Godot;
using System;

public partial class StageSelection : Node2D
{

	public override void _Ready()
	{
	}

	private void _on_button_mouse_entered(string buttonName)
	{
		Sprite2D _background_image = GetNode<Sprite2D>("BackgroundImage");
		switch (buttonName)
		{
			case "airplane":
				_change_texture(CursedFight.Global.Assets.AIRPLANE_BACKGROUND, _background_image);
				break;
			case "fight_club":
				_change_texture(CursedFight.Global.Assets.FIGHT_CLUB_BACKGROUND, _background_image);
				break;
			case "throne_room":
				_change_texture(CursedFight.Global.Assets.THRONE_ROOM_BACKGROUND, _background_image);
				break;
			case "la_gomera":
				_change_texture(CursedFight.Global.Assets.LA_GOMERA_BACKGROUND, _background_image);
				break;
			default:
				break;
		}
	}

	private void _on_button_pressed(string buttonName)
	{
		CursedFight.Global.Context.SetActualStage(buttonName);

		PackedScene _new_scene = GD.Load<PackedScene>(CursedFight.Global.Scenes.COMBAT_SCREEN);
		if (_new_scene == null)
		{
			GD.Print("Error: No se pudo cargar la escena en la ruta: " + CursedFight.Global.Scenes.COMBAT_SCREEN);
			return;
		}

		GetTree().ChangeSceneToPacked(_new_scene);

	}

	private void _on_return_button_pressed()
	{
		PackedScene _new_scene = GD.Load<PackedScene>(CursedFight.Global.Scenes.MAIN_MENU);
		if (_new_scene == null)
		{
			GD.Print("Error: No se pudo cargar la escena en la ruta: " + CursedFight.Global.Scenes.MAIN_MENU);
			return;
		}

		GetTree().ChangeSceneToPacked(_new_scene);
	}

	private void _change_texture(string textureRoute, Sprite2D sprite)
	{

		Texture2D _new_texture = GD.Load<Texture2D>(textureRoute);

		if (_new_texture == null)
		{
			GD.Print("Error: No se pudo cargar la textura en la ruta: ", textureRoute);
			return;
		}

		sprite.Texture = _new_texture;
	}
}
