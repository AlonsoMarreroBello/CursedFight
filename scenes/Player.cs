using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	private AnimatedSprite2D animatedSprite2D;

	private Area2D hitboxPunch;

	private Dictionary<string, int> impactFrames;

	private string currentAnimation = "idle";


	public override void _Ready()
	{
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Play(currentAnimation);
		hitboxPunch = GetNode<Area2D>("HitboxPunch");
		hitboxPunch.Monitoring = false;	
		hitboxPunch.Visible = false;
		impactFrames = new Dictionary<string, int>
        {
            { "punch", 3 },
			{ "kick", 3 },
        };
		animatedSprite2D.AnimationFinished += OnAnimationFinished;
	}
 	private void OnAnimationFinished()
    {
        if (currentAnimation != "idle")
        {
            _change_state("idle");
        }
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Inputs
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}
		if (Input.IsActionJustPressed("punch")) {
			_change_state("punch");
		}

		if (velocity.X == 0 && velocity.Y == 0 && IsOnFloor() && currentAnimation != "punch" && currentAnimation != "kick")
		{
			_change_state("idle");
		}
		
		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
			animatedSprite2D.FlipH = direction.X < 0;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		if (velocity.X == 0 && currentAnimation != "punch")
        {
            _change_state("idle");
        }

		Velocity = velocity;
		MoveAndSlide();
	}


	

	public override void _Process(double delta){

		if (animatedSprite2D.FlipH)
        {
			hitboxPunch.Scale = new Vector2(-1, 1);
        }
        else
        {
			hitboxPunch.Scale = new Vector2(1, 1);

        }

		if (animatedSprite2D.Animation == "punch")
        {
            if (impactFrames.ContainsKey("punch") && animatedSprite2D.Frame == impactFrames["punch"])
            {
                hitboxPunch.Visible = true;
                hitboxPunch.Monitoring = true;
            }
            else
            {
                hitboxPunch.Visible = false;
                hitboxPunch.Monitoring = false;
            }
        }
		else{
			hitboxPunch.Visible = false;
            hitboxPunch.Monitoring = false;
		}

	}
	

	private void _change_state(string newState)
    {
        if (currentAnimation == newState)
            return;

        currentAnimation = newState;
        _play_animation(newState);
    }


	private void _play_animation(string animationName)
    {
        if (animatedSprite2D.Animation != animationName)
        {
            animatedSprite2D.Play(animationName);
        }
    }

}
