using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
    public const float Speed = 1000.0f;
    public const float JumpHeight = -2050.0f;
    public const float JumpSpeedMultiplier = 8.5f;
    public const float FallSpeedMultiplier = 8.5f;
    private AnimatedSprite2D animatedSprite2D;

    private Area2D hitboxPunch;
    private Area2D hitboxKick;
	private CollisionShape2D hitboxUpperBody;

    private CollisionShape2D hitboxLowerBody;

    private float originalUpperBodyHitboxPositionX;
	private float originalUpperBodyHitboxPositionY;

	private float originalLowerBodyHitboxPositionY;

	private float originalLowerBodyHitboxPositionX;
    private Dictionary<string, int> impactFrames;

    private string currentAnimation = "idle";

    public override void _Ready()
    {
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animatedSprite2D.Play(currentAnimation);
        impactFrames = new Dictionary<string, int>
        {
            { "punch", 3 },
            { "kick", 3 },
        };
        hitboxPunch = GetNode<Area2D>("HitboxPunch");
        hitboxPunch.Monitoring = false;
        hitboxPunch.Visible = false;
        hitboxKick = GetNode<Area2D>("HitboxKick");
        hitboxKick.Monitoring = false;
        hitboxKick.Visible = false;
        hitboxUpperBody = GetNode<CollisionShape2D>("HitboxUpperBody");
		hitboxLowerBody = GetNode<CollisionShape2D>("HitboxLowerBody");
        originalUpperBodyHitboxPositionX = hitboxUpperBody.Position.X;
		originalUpperBodyHitboxPositionY = hitboxUpperBody.Position.Y;
		originalLowerBodyHitboxPositionY = hitboxLowerBody.Position.Y;
		originalLowerBodyHitboxPositionX = hitboxLowerBody.Position.X;
		animatedSprite2D.AnimationFinished += _on_animation_finished;
    }

	private void _on_animation_finished(){
		if (currentAnimation != "jump" && currentAnimation!= "fall")
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
            if (velocity.Y > 0)
            {
                velocity.Y += GetGravity().Y * (FallSpeedMultiplier - 1) * (float)delta;
                _change_state("fall"); 
                GD.Print("Caída: " + currentAnimation + " IsOnFloor: " + IsOnFloor() + " Velocity.Y: " + velocity.Y);
            }
            if (velocity.Y < 0)
            {
                velocity.Y += GetGravity().Y * (JumpSpeedMultiplier - 1) * (float)delta;
            }
        }

        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
        {
            velocity.Y = JumpHeight;
            _change_state("jump"); 
            GD.Print("Salto: " + currentAnimation);
        }
        else if (Input.IsActionJustPressed("punch"))
        {
            _change_state("punch");
        }
        else if (Input.IsActionJustPressed("kick"))
        {
            _change_state("kick");
        }

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

        Velocity = velocity;
        MoveAndSlide();
        if (IsOnFloor())
        {
            if (currentAnimation == "fall" || currentAnimation == "jump")
            {
                if (Mathf.Abs(velocity.X) > 0.1f)
                {
                    _change_state("idle"); 
                }
                else
                {
                    _change_state("idle");
                }
                GD.Print("Toca suelo: " + currentAnimation);
            }
        }
    }

    public override void _Process(double delta)
    {
        if (animatedSprite2D.FlipH)
        {
            hitboxPunch.Scale = new Vector2(-1, 1);
            hitboxKick.Scale = new Vector2(-1, 1);
        }
        else
        {
            hitboxPunch.Scale = new Vector2(1, 1);
            hitboxKick.Scale = new Vector2(1, 1);
        }

        if (animatedSprite2D.Animation == "kick")
        {
            if (animatedSprite2D.FlipH)
            {
                hitboxUpperBody.Position = new Vector2(200, originalUpperBodyHitboxPositionY);
				hitboxLowerBody.Position = new Vector2(100, originalLowerBodyHitboxPositionY);
            }
            else
            {
                hitboxUpperBody.Position = new Vector2(-200, originalUpperBodyHitboxPositionY);
				hitboxLowerBody.Position = new Vector2(-100, originalLowerBodyHitboxPositionY);
            }
            if (animatedSprite2D.Frame == impactFrames["kick"])
            {
                hitboxKick.Visible = true;
                hitboxKick.Monitoring = true;
            }
            else
            {
                hitboxKick.Visible = false;
                hitboxKick.Monitoring = false;
                hitboxUpperBody.Position = new Vector2(originalUpperBodyHitboxPositionX,originalUpperBodyHitboxPositionY );
				hitboxLowerBody.Position = new Vector2(originalLowerBodyHitboxPositionX,originalLowerBodyHitboxPositionY );
            }
        }

        if (animatedSprite2D.Animation == "punch")
        {
            if (animatedSprite2D.FlipH)
            {
                hitboxUpperBody.Position = new Vector2(-50, originalUpperBodyHitboxPositionY);
				hitboxLowerBody.Position = new Vector2(-50, originalLowerBodyHitboxPositionY);
            }
            else
            {
                hitboxUpperBody.Position = new Vector2(50, originalUpperBodyHitboxPositionY);
				hitboxLowerBody.Position = new Vector2(50, originalLowerBodyHitboxPositionY);
            }
            if (animatedSprite2D.Frame == impactFrames["punch"])
            {
                hitboxPunch.Visible = true;
                hitboxPunch.Monitoring = true;
            }
            else
            {
                hitboxPunch.Visible = false;
                hitboxPunch.Monitoring = false;
                hitboxUpperBody.Position = new Vector2(originalUpperBodyHitboxPositionX,originalUpperBodyHitboxPositionY );
				hitboxLowerBody.Position = new Vector2(originalLowerBodyHitboxPositionX,originalLowerBodyHitboxPositionY );
                
            }
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
        GD.Print($"Attempting to play: {animationName}, Current animation: {animatedSprite2D.Animation}");
        if (animatedSprite2D.Animation != animationName)
        {
            animatedSprite2D.Play(animationName);
        }
    }
}