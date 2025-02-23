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
				_change_texture("res://assets/BackGrounds/Airplain/Airplane.png", _background_image);
				break;
			case "fight_club":
				_change_texture("res://assets/BackGrounds/FightClub/FightClub01.png", _background_image);
				break;
			case "throne_room":
				_change_texture("res://assets/BackGrounds/ThroneRoom/ThroneRoom.png", _background_image);
				break;

			default:
				break;
		}
	}

	private void _on_button_pressed(string buttonName)
	{
		switch (buttonName)
		{
			case "airplane":
				GD.Print("airplane");
				break;
			case "fight_club":
				GD.Print("fight_club");
				break;
			case "throne_room":
				GD.Print("throne_room");
				break;

			default:
				break;
		}
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
