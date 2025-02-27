using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
    [Export] public int playerNumber = 1;
    [Export] private float pushForce = 300.0f;

    [Export] public float maxPushDistance = 200f;
    public const float Speed = 1000.0f;
    public const float JumpHeight = -2050.0f;

    private int health = 100;

    private int energy = 0;

    private int maxEnergy = 100;

    private const float JumpSpeedMultiplier = 8.5f;
    private const float FallSpeedMultiplier = 8.5f;
    private AnimatedSprite2D animatedSprite2D;

    private Area2D hitboxPunch;
    private Area2D hitboxKick;

    private Area2D hitBoxUltimateKick;


    private CollisionShape2D hitboxUpperBody;

    private CollisionShape2D hitboxLowerBody;

    private TextureProgressBar healthBarP1;

    private TextureProgressBar energyBarP1;
    private TextureProgressBar healthBarP2;

    private TextureProgressBar energyBarP2;

    private float originalUpperBodyHitboxPositionX;
    private float originalUpperBodyHitboxPositionY;

    private float originalLowerBodyHitboxPositionY;

    private float originalLowerBodyHitboxPositionX;
    private Dictionary<string, int> actionFrames;

    private string currentAnimation = "idle";
    private Player otherPlayer;
    private bool facingRight = true;

    private float displacementDistance = 0f;
    private bool displacement = false;

    private bool guardImpactQueued = false;

    private bool hasReceivedDamage = false;

    public override void _Ready()
    {
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animatedSprite2D.Play(currentAnimation);
        actionFrames = new Dictionary<string, int>
        {
            { "punch", 3 },
            { "kick", 3 },
            {"run", 18},
            {"teleport", 21},
            {"ultimate", 36},
        };
        hitboxPunch = GetNode<Area2D>("HitboxPunch");
        hitboxPunch.Monitoring = false;
        hitboxPunch.Visible = false;
        hitboxKick = GetNode<Area2D>("HitboxKick");
        hitboxKick.Monitoring = false;
        hitboxKick.Visible = false;
        hitBoxUltimateKick = GetNode<Area2D>("HitboxUltimateKick");
        hitBoxUltimateKick.Monitoring = false;
        hitBoxUltimateKick.Visible = false;
        hitboxUpperBody = GetNode<CollisionShape2D>("HitboxUpperBody");
        hitboxLowerBody = GetNode<CollisionShape2D>("HitboxLowerBody");
        originalUpperBodyHitboxPositionX = hitboxUpperBody.Position.X;
        originalUpperBodyHitboxPositionY = hitboxUpperBody.Position.Y;
        originalLowerBodyHitboxPositionY = hitboxLowerBody.Position.Y;
        originalLowerBodyHitboxPositionX = hitboxLowerBody.Position.X;
        animatedSprite2D.AnimationFinished += _on_animation_finished;
        animatedSprite2D.FrameChanged += _on_frame_changed;

        facingRight = playerNumber == 1;
        healthBarP1 = GetParent().GetNode<TextureProgressBar>("UI/Player1/LifeBar");
        energyBarP1 = GetParent().GetNode<TextureProgressBar>("UI/Player1/EnergyBar");
        healthBarP2 = GetParent().GetNode<TextureProgressBar>("UI/Player2/LifeBar");
        energyBarP2 = GetParent().GetNode<TextureProgressBar>("UI/Player2/EnergyBar");
        if (playerNumber == 1)
        {
            healthBarP1.Value = health;
            energyBarP1.Value = energy;
        }
        else
        {
            healthBarP2.Value = health;
            energyBarP2.Value = energy;
        }
    }

    public void _set_other_player(Player otherPlayer)
    {
        this.otherPlayer = otherPlayer;
        if (playerNumber == 1)
        {
            healthBarP2.Value = otherPlayer.health;
            energyBarP2.Value = otherPlayer.energy;
        }
        else
        {
            healthBarP1.Value = otherPlayer.health;
            energyBarP1.Value = otherPlayer.energy;
        }
        _update_facing_direction();
    }

    private void _on_animation_finished()
    {
        if (currentAnimation == "guard_impact")
        {
            _change_state("guard");
        } else if (currentAnimation == "take_attack")
        {
            _change_state("idle");
        }
        else if (currentAnimation != "jump" && currentAnimation != "fall" && currentAnimation != "crouch" && currentAnimation != "guard" && currentAnimation != "walk" && currentAnimation != "take_attack")
        {
            _change_state("idle");
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;
        _update_facing_direction();
        // Add the gravity.
        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
            if (velocity.Y > 0)
            {
                velocity.Y += GetGravity().Y * (FallSpeedMultiplier - 1) * (float)delta;
                _change_state("fall");

            }
            if (velocity.Y < 0)
            {
                velocity.Y += GetGravity().Y * (JumpSpeedMultiplier - 1) * (float)delta;
            }
        }

        if (IsOnFloor() && currentAnimation != "ultimate" && (playerNumber == 1 && Input.IsActionJustPressed("jump1") || playerNumber == 2 && Input.IsActionJustPressed("jump2")))
        {
            velocity.Y = JumpHeight;
            _change_state("jump");
        }
        else if (IsOnFloor() && currentAnimation != "ultimate" && currentAnimation != "kick" && currentAnimation != "take_attack" && (playerNumber == 1 && Input.IsActionJustPressed("punch1") || playerNumber == 2 && Input.IsActionJustPressed("punch2")))
        {
            _change_state("punch");
        }
        else if (IsOnFloor() && currentAnimation != "ultimate" && currentAnimation != "punch" && currentAnimation != "take_attack" && (playerNumber == 1 && Input.IsActionJustPressed("kick1") || playerNumber == 2 && Input.IsActionJustPressed("kick2")))
        {
            _change_state("kick");
        }
        else if (IsOnFloor() && currentAnimation != "ultimate" && (playerNumber == 1 && Input.IsActionJustPressed("crouch1") || playerNumber == 2 && Input.IsActionJustPressed("crouch2")))
        {
            _change_state("crouch");
        }
        else if (IsOnFloor() && currentAnimation != "ultimate" && (playerNumber == 1 && Input.IsActionJustReleased("crouch1") || playerNumber == 2 && Input.IsActionJustReleased("crouch2")))
        {
            if (currentAnimation == "guard")
            {
                _change_state("guard");
            }
            else
            {
                _change_state("idle");
            }
        }
        else if (IsOnFloor() && hasReceivedDamage && currentAnimation != "guard")
        {
            _change_state("take_attack");
            hasReceivedDamage = false;
        }
        else if (IsOnFloor() && guardImpactQueued && currentAnimation == "guard")
        {
            _change_state("guard_impact");
            guardImpactQueued = false;
        }
        else if (IsOnFloor() && currentAnimation != "ultimate" && currentAnimation != "guard_impact" && currentAnimation != "take_attack" && (playerNumber == 1 && Input.IsActionJustPressed("guard1") || playerNumber == 2 && Input.IsActionJustPressed("guard2")))

        {
            _change_state("guard");

        }
        else if (IsOnFloor() && currentAnimation != "ultimate" && (playerNumber == 1 && Input.IsActionJustReleased("guard1") || playerNumber == 2 && Input.IsActionJustReleased("guard2")))
        {
            _change_state("idle");
        }
        else if (IsOnFloor() && (playerNumber == 1 && Input.IsActionJustPressed("ultimate1") || playerNumber == 2 && Input.IsActionJustPressed("ultimate2")))
        {
            if (checkIfEnergyIsFull(playerNumber))
            {
                _change_state("ultimate");
                ClearEnergy(playerNumber);

            }

        }
        else if (IsOnFloor() && currentAnimation != "ultimate" && currentAnimation != "punch" && currentAnimation != "kick" && currentAnimation != "crouch" && currentAnimation != "guard" && currentAnimation != "guard_impact" && currentAnimation != "walk" && currentAnimation != "take_attack")
        {
            _change_state("idle");
        }



        Vector2 direction = Vector2.Zero;
        if (playerNumber == 1)
        {
            Vector2 playerOneDirection = Input.GetVector("left1", "right1", "jump1", "crouch1");
            direction = playerOneDirection;
        }
        if (playerNumber == 2)
        {
            Vector2 playerTwoDirection = Input.GetVector("left2", "right2", "jump2", "crouch2");
            direction = playerTwoDirection;
        }

        if (direction != Vector2.Zero && currentAnimation != "ultimate" && currentAnimation != "crouch" && currentAnimation != "guard")
        {
            velocity.X = direction.X * Speed;

            if (direction.X > 0)
            {
                facingRight = true;
                _change_state("walk");
            }
            else if (direction.X < 0)
            {
                facingRight = false;
                _change_state("walk");
            }
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            if (currentAnimation == "walk")
            {
                _change_state("idle");
            }


        }


        Velocity = velocity;
        MoveAndSlide();
    }

    public override void _Process(double delta)
    {

        _handle_hitbox_positions();

	}

    private void _update_facing_direction()
    {
        if (currentAnimation == "ultimate") return;

        if (otherPlayer != null && otherPlayer.currentAnimation != "ultimate")
        {

            facingRight = Position.X < otherPlayer.Position.X;
        }
        animatedSprite2D.FlipH = !facingRight;
    }


    private void _handle_hitbox_positions()
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
            float directionModifier = animatedSprite2D.FlipH ? 1 : -1;
            hitboxUpperBody.Position = new Vector2(directionModifier * 200, originalUpperBodyHitboxPositionY);
            hitboxLowerBody.Position = new Vector2(directionModifier * 100, originalLowerBodyHitboxPositionY);

            if (animatedSprite2D.Frame == actionFrames["kick"])
            {
                hitboxKick.Visible = true;
                hitboxKick.Monitoring = true;
            }
            else
            {
                hitboxUpperBody.Position = new Vector2(originalUpperBodyHitboxPositionX, originalUpperBodyHitboxPositionY);
                hitboxLowerBody.Position = new Vector2(originalLowerBodyHitboxPositionX, originalLowerBodyHitboxPositionY);
            }
        }
        else if (animatedSprite2D.Animation == "punch")
        {
            float directionModifier = animatedSprite2D.FlipH ? 1 : -1;

			hitboxUpperBody.Position = new Vector2(directionModifier * 50, originalUpperBodyHitboxPositionY);
			hitboxLowerBody.Position = new Vector2(directionModifier * 50, originalLowerBodyHitboxPositionY);


            if (animatedSprite2D.Frame == actionFrames["punch"])
            {
                hitboxPunch.Visible = true;
                hitboxPunch.Monitoring = true;
            }
            else
            {

                hitboxUpperBody.Position = new Vector2(originalUpperBodyHitboxPositionX, originalUpperBodyHitboxPositionY);
                hitboxLowerBody.Position = new Vector2(originalLowerBodyHitboxPositionX, originalLowerBodyHitboxPositionY);

            }
        }
        else if (animatedSprite2D.Animation == "ultimate")
        {
            if (Position.X > otherPlayer.Position.X)
            {
                hitBoxUltimateKick.Scale = new Vector2(1, 1);
            }
            else
            {
                hitBoxUltimateKick.Scale = new Vector2(-1, 1);
            }
            if (animatedSprite2D.Frame >= actionFrames["ultimate"])
            {
                hitBoxUltimateKick.Monitoring = true;
                hitBoxUltimateKick.Visible = true;

            }
        }
        else
        {
            hitboxUpperBody.Position = new Vector2(originalUpperBodyHitboxPositionX, originalUpperBodyHitboxPositionY);
            hitboxLowerBody.Position = new Vector2(originalLowerBodyHitboxPositionX, originalLowerBodyHitboxPositionY);
            hitboxPunch.Visible = false;
            hitboxPunch.Monitoring = false;
            hitboxKick.Visible = false;
            hitboxKick.Monitoring = false;
            hitBoxUltimateKick.Monitoring = false;
            hitBoxUltimateKick.Visible = false;

		}
	}

    private void _change_state(string newState)
    {
        if (currentAnimation == newState)
            return;
        if (newState == "ultimate")
        {
            animatedSprite2D.FlipH = !facingRight;
        }
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

    private void _on_frame_changed()
    {
        if (animatedSprite2D.Animation == "ultimate" && !displacement)
        {
            if (animatedSprite2D.Frame == actionFrames["run"])
            {

                displacementDistance = otherPlayer.Position.X - Position.X;
                Position += new Vector2(displacementDistance, 0);
                displacement = true;
            }

            if (animatedSprite2D.Frame == actionFrames["teleport"])
            {
                float offsetDistance = 350f;

                if (!otherPlayer.facingRight)
                {
                    displacementDistance = otherPlayer.Position.X + offsetDistance;
                }
                else
                {
                    displacementDistance = otherPlayer.Position.X - offsetDistance;
                }
                float minX = 0f;
                float maxX = 1920f;
                displacementDistance = Mathf.Clamp(displacementDistance, minX, maxX);

                Position = new Vector2(displacementDistance, Position.Y);
            }
        }
        displacement = false;
    }

    private void _on_hitbox_punch_body_entered(Node2D body)
    {
        if (body is Player targetPlayer && targetPlayer != this && currentAnimation == "punch")
        {
            if (targetPlayer.IsGuarding() && targetPlayer.energy >= 0)
            {
                targetPlayer.TargetQueueGuardImpact();
                targetPlayer.TakeEnergy(12);
                if (targetPlayer.energy == 0)
                {
                    ApplyDamage(targetPlayer, 5);
                    targetPlayer.ApplyPush(this, "ultimate");
                }
            }
            else
            {
                ApplyDamage(targetPlayer, 5);
                targetPlayer.TargetReceiveDamage();
                targetPlayer.ApplyPush(this, "punch");

            }
            ApplyEnergy(10);
        }

    }

    public void _on_hitbox_kick_body_entered(Node2D body)
    {
        if (body is Player targetPlayer && targetPlayer != this && currentAnimation == "kick")
        {
            if (targetPlayer.IsGuarding() && targetPlayer.energy >= 0)
            {
                targetPlayer.TargetQueueGuardImpact();
                targetPlayer.TakeEnergy(10);
                if (targetPlayer.energy == 0)
                {
                    ApplyDamage(targetPlayer, 5);
                    targetPlayer.ApplyPush(this, "ultimate");
                }
            }
            else
            {
                ApplyDamage(targetPlayer, 5);
                targetPlayer.TargetReceiveDamage();
                targetPlayer.ApplyPush(this, "kick");
                
            }
            ApplyEnergy(5);
        }
    }

    public void _on_hitbox_ultimate_kick_body_entered(Node2D body)
    {
        if (body is Player targetPlayer && targetPlayer != this && currentAnimation == "ultimate")
        {
            if (targetPlayer.IsGuarding() && targetPlayer.energy >= 0)
            {
                targetPlayer.TargetQueueGuardImpact();
                targetPlayer.TakeEnergy(50);
                if (targetPlayer.energy == 0)
                {
                    ApplyDamage(targetPlayer, 20);
                    targetPlayer.ApplyPush(this, "ultimate");
                }
            }
            else
            {
                ApplyDamage(targetPlayer, 30);
                targetPlayer.TargetReceiveDamage();
                targetPlayer.ApplyPush(this, "ultimate");
            }

        }

    }

    private bool checkIfEnergyIsFull(int playerNumber)
    {
        if (playerNumber == 1)
        {
            return energy >= maxEnergy;
        }
        else
        {
            return energy >= maxEnergy;
        }
    }

    private void ApplyDamage(Player target, int damage)
    {
        GD.Print($"Player {playerNumber} aplica {damage} daño.");
        target.TakeDamage(damage);
    }

    private void ApplyEnergy(int energy)
    {
        if (playerNumber == 1)
        {
            this.energy += energy;
            energyBarP1.Value = this.energy;
        }
        else
        {
            this.energy += energy;
            energyBarP2.Value = this.energy;
        }
    }

    public void ApplyPush(Node2D attacker, string attackType)
    {
        Vector2 direction = (Position - attacker.Position).Normalized();

        float appliedPushForce = pushForce;
        switch (attackType)
        {
            case "punch":
                appliedPushForce *= 0.5f;
                break;
            case "kick":
                appliedPushForce *= 0.75f;
                break;
            case "ultimate":
                appliedPushForce *= 7.5f;
                break;
        }

        Vector2 pushVector = direction * appliedPushForce;

        if (pushVector.Length() > maxPushDistance)
        {
            pushVector = pushVector.Normalized() * maxPushDistance;
        }


        Position += pushVector;

        ClampPositionToMapBounds();
    }

    private void ClampPositionToMapBounds()
    {
        float minX = 0f; 
        float maxX = 1920f; 

        Position = new Vector2(Mathf.Clamp(Position.X, minX, maxX), Position.Y);
    }

    private void TakeEnergy(int energyTaked)
    {

        if (energy <= 0)
        {
            energy = 0;
            return;
        }

        energy -= energyTaked;

        if (playerNumber == 1)
        {

            energyBarP1.Value = energy;
        }
        else
        {

            energyBarP2.Value = energy;
        }
    }

    public void TakeDamage(int damage)
    {
        if (health <= 0)
        {
            health = 0;
            return;
        }
        health -= damage;
        if (playerNumber == 1)
        {
            healthBarP1.Value = health;
        }
        else
        {
            healthBarP2.Value = health;
        }
    }

    private void ClearEnergy(int playerNumber)
    {
        if (playerNumber == 1)
        {
            energy = 0;
            energyBarP1.Value = energy;
        }
        else
        {
            energy = 0;
            energyBarP2.Value = energy;
        }
    }

    public bool IsGuarding()
    {
        return currentAnimation == "guard";
    }

    private void TargetQueueGuardImpact()
    {
        guardImpactQueued = true;
    }

    private void TargetReceiveDamage()
    {
        hasReceivedDamage = true;
    }
}
