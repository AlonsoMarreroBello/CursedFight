using Godot;

public partial class CombatScreen : Node2D
{
	Timer _timer;
	Label _timer_label;
	public override void _Ready()
	{
		AnimatedSprite2D _background_image = GetNode<AnimatedSprite2D>("BackgroundImage");
		AudioStreamPlayer _audio_player = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		AudioStream _new_audio = GD.Load<AudioStream>(CursedFight.Global.Music.MAIN_THEME); ;
		switch (CursedFight.Global.Context.GetActualStage())
		{
			case "airplane":
				_change_texture("airplane", _background_image);
				_new_audio = GD.Load<AudioStream>(CursedFight.Global.Music.MAIN_THEME);
				break;
			case "fight_club":
				_change_texture("fight_club", _background_image);
				_new_audio = GD.Load<AudioStream>(CursedFight.Global.Music.TECHNO_THEME);
				break;
			case "throne_room":
				_change_texture("throne_room", _background_image);
				_new_audio = GD.Load<AudioStream>(CursedFight.Global.Music.SURPRISE_IMPACT);
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
