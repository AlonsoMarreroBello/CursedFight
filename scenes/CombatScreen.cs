using Godot;
using System;

public partial class CombatScreen : Node2D
{
	// Called when the node enters the scene tree for the first time.

	Timer _timer;
	Label _timer_label;
	public override void _Ready()
	{
		AnimatedSprite2D _background_image = GetNode<AnimatedSprite2D>("BackgroundImage");
		AudioStreamPlayer _audio_player = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		AudioStream _new_audio = GD.Load<AudioStream>("res://assets/Music/the-god-beyond-the-man-kaledon-main.mp3"); ;
		switch (Global._actual_stage)
		{
			case "airplane":
				_change_texture("airplane", _background_image);
				_new_audio = GD.Load<AudioStream>("res://assets/Music/the-god-beyond-the-man-kaledon-main.mp3");
				break;
			case "fight_club":
				_change_texture("fight_club", _background_image);
				_new_audio = GD.Load<AudioStream>("res://assets/Music/pounding-industrial-techno-instrumental.mp3");
				break;
			case "throne_room":
				_change_texture("throne_room", _background_image);
				_new_audio = GD.Load<AudioStream>("res://assets/Music/surprise-impact.mp3");
				break;

			default:
				break;
		}

		_audio_player.Stream = _new_audio;
		_audio_player.Play();

		_timer = GetNode<Timer>("Timer");
		_timer_label = GetNode<Label>("UI/Timer/TimerLabel");

		_timer.Start();

	}

	public override void _Process(double delta)
	{
		_timer_label.Text = ((int)_timer.TimeLeft).ToString();
	}


	private void _change_texture(string _animation_name, AnimatedSprite2D sprite)
	{
		sprite.Play(_animation_name);
	}
}
