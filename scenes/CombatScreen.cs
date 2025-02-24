using Godot;
using System;

public partial class CombatScreen : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AnimatedSprite2D _background_image = GetNode<AnimatedSprite2D>("BackgroundImage");
		switch (Global._actual_stage)
		{
			case "airplane":
				_change_texture("airplane", _background_image);
				break;
			case "fight_club":
				_change_texture("fight_club", _background_image);
				break;
			case "throne_room":
				_change_texture("throne_room", _background_image);
				break;

			default:
				break;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _change_texture(string _animation_name, AnimatedSprite2D sprite)
	{

		sprite.Play(_animation_name);
	}
}
