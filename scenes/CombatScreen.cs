using Godot;

public partial class CombatScreen : Node2D
{
	Timer _timer;
	Label _timer_label;
	Main main;

	public override void _Ready()
	{
		main = this.GetParent() as Main;
		if (main.isMultiplayer){
			this.AddChild(GD.Load<PackedScene>("res://scenes/MultiplayerNode.tscn").Instantiate());

			Player player1 = GetNode<Player>("./MultiplayerNode/Player");
			Player player2 = GetNode<Player>("./MultiplayerNode/Player2");

			player1._set_other_player(player2);
			player2._set_other_player(player1);

		} else{
			this.AddChild(GD.Load<PackedScene>("res://scenes/SinglePlayerNode.tscn").Instantiate());
		}
		GD.Print("Entro");
		AnimatedSprite2D _background_image = GetNode<AnimatedSprite2D>("BackgroundImage");
		AudioStreamPlayer _audio_player = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		AudioStream _new_audio = GD.Load<AudioStream>(CursedFight.Global.Music.MAIN_THEME);

		ResetAnimations(_background_image);

		switch (CursedFight.Global.Context.GetActualStage())
		{
			case "airplane":
				_change_texture("airplane", _background_image);
				_new_audio = GD.Load<AudioStream>(CursedFight.Global.Music.MAIN_THEME);
				Sprite2D stork = this.GetNode<Sprite2D>("./BackgroundImage/Stork");
				stork.Show();
				_background_image.GetNode<Parallax2D>("./Parallax2D").Show();
				_background_image.GetNode<Sprite2D>("./Parallax2D/Clouds3").Hide();
				AnimationPlayer storkAnimation = stork.GetNode<AnimationPlayer>("./Animation");
				storkAnimation.Play("stork");
				GD.Print("bruh");
				break;
			case "fight_club":
				_change_texture("fight_club", _background_image);
				_new_audio = GD.Load<AudioStream>(CursedFight.Global.Music.TECHNO_THEME);
				this.GetNode<CollisionShape2D>("./StageLimits/AirplaneCollision").SetDeferred("disabled","false");
				break;
			case "throne_room":
				_change_texture("throne_room", _background_image);
				_new_audio = GD.Load<AudioStream>(CursedFight.Global.Music.SURPRISE_IMPACT);
				this.GetNode<CollisionShape2D>("./StageLimits/AirplaneCollision").SetDeferred("disabled","false");
				break;
			case "la_gomera":
				_change_texture("la_gomera", _background_image);
				_new_audio = GD.Load<AudioStream>(CursedFight.Global.Music.MAIN_THEME);
				this.GetNode<CollisionShape2D>("./StageLimits/AirplaneCollision").SetDeferred("disabled","false");
				Sprite2D armasShip, fredOlsenShip, platform;
				armasShip = _background_image.GetNode<Sprite2D>("./ArmasShip");
				fredOlsenShip = _background_image.GetNode<Sprite2D>("./FredOlsenShip");
				platform = _background_image.GetNode<Sprite2D>("./GomeraPlatform");

				armasShip.Show();
				fredOlsenShip.Show();
				platform.Show();

				armasShip.GetNode<AnimationPlayer>("./Animation").Play("Armas");
				fredOlsenShip.GetNode<AnimationPlayer>("./Animation").Play("FredOlsen");

				_background_image.GetNode<Parallax2D>("./Parallax2D").Show();
				_background_image.GetNode<Sprite2D>("./Parallax2D/CloudsAirplane").Hide();
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

	private void ResetAnimations(AnimatedSprite2D background){
		foreach (Node2D item in background.GetChildren())
		{
			if(item.GetChildCount() >  0 && item.Name != "Parallax2D"){
				item.GetNode<AnimationPlayer>("./Animation").Stop();
			}

			item.Hide();
		}
	}

	public void FinishCombat(int id){
		//TODO add logic to show winner combat

		this.AddChild(GD.Load<PackedScene>("res://scenes/Winner.tscn").Instantiate());

		if (id == 1 && main.isMultiplayer){
			this.GetNode<Label>("./Winner/Label").Text = "Winner is player 2";
		}else if(id == 1 && !main.isMultiplayer){
			this.GetNode<Label>("./Winner/Label").Text = "Winner is AI";
		}else{
			this.GetNode<Label>("./Winner/Label").Text = "Winner is player 1";
		}

		this.GetNode<Button>("./Winner/MainMenu").ProcessMode = ProcessModeEnum.Always;
		GetTree().Paused = true;

		this.GetNode<Button>("./Winner/MainMenu").Pressed += () =>
		{
			GetTree().Paused = false;
			GetTree().ChangeSceneToFile("res://scenes/MainMenu.tscn");
		};
	}

}
