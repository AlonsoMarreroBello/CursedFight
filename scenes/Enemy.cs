using Godot;
using System;
using System.Collections.Generic;
using System.Threading;

public partial class Enemy : CharacterBody2D
{
    [Export] public float Speed = 500.0f;
    //[Export] public float JumpHeight = -50.0f;
    [Export] private float attackRange = 170f;
    [Export] private float detectionRange = 800f;
    [Export] private float blockChance = 0.3f; 
    [Export] private float jumpChance = 0.1f;
    [Export] private float ultimateThreshold = 100f;
    [Export] private float retreatTime = 0.5f; 

	private TextureProgressBar healthBar, energyBar;

    private int health = 100;
    private int energy = 0;
    private int maxEnergy = 100;

    private AnimatedSprite2D animatedSprite2D;
    private Player targetPlayer;
    private bool facingRight = false;
    private string currentAnimation = "idle";

    private bool isRetreating = false;
    private float retreatTimer = 0f;
    private bool isAttacking = false;
    private float attackCooldown = 0.5f; 
    private float attackTimer = 0f;

    public override void _Ready()
    {
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animatedSprite2D.Play(currentAnimation);

		healthBar = GetParent().GetParent().GetNode<TextureProgressBar>("UI/Player2/LifeBar");
        energyBar = GetParent().GetParent().GetNode<TextureProgressBar>("UI/Player2/EnergyBar");

		healthBar.Value = health;

        targetPlayer = GetParent().GetNode<Player>("Player"); 
    }

    public override void _PhysicsProcess(double delta)
    {
		GD.Print(currentAnimation);
        if (targetPlayer == null) return;

        float distanceToPlayer = Position.DistanceTo(targetPlayer.Position);
        Vector2 velocity = Velocity;

        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }

        if (isAttacking) 
        {
            Velocity = Vector2.Zero;
            return;
        }

        if (isRetreating)
        {
            retreatTimer -= (float)delta;
            if (retreatTimer <= 0)
            {
                isRetreating = false;
            }
            else
            {
                MoveAwayFromPlayer(ref velocity);
            }
        }
        else
        {
            if (distanceToPlayer <= detectionRange)
            {
                if (energy >= maxEnergy)
                {
                    _change_state("ultimate");
                    energy = 0;
                }
                else if (distanceToPlayer > attackRange)
                {
                    MoveTowardsPlayer(ref velocity);
                }
                else
                {
                    if (attackTimer <= 0)
                    {
                        DecideCombatAction();
                    }
                }
            }
        }

        attackTimer -= (float)delta;

        Velocity = velocity;
        MoveAndSlide();
    }

    private void MoveTowardsPlayer(ref Vector2 velocity)
    {
        Vector2 direction = (targetPlayer.Position - Position).Normalized();
        velocity.X = direction.X * Speed;

        facingRight = direction.X > 0;
        animatedSprite2D.FlipH = !facingRight;

        _change_state("walk");

		Random rnd = new();
        if (rnd.Next(0,1000) < -1 && !isRetreating)
        {
            isRetreating = true;
            retreatTimer = retreatTime;
        }

        if (GD.Randf() < jumpChance && IsOnFloor())
        {
            //velocity.Y = JumpHeight;
            //_change_state("jump");
        }
    }

    private void MoveAwayFromPlayer(ref Vector2 velocity)
    {
        Vector2 direction = (Position - targetPlayer.Position).Normalized();
        velocity.X = direction.X * Speed;

        facingRight = direction.X > 0;
        animatedSprite2D.FlipH = !facingRight;

        _change_state("walk");
    }

    private void DecideCombatAction()
    {
        isAttacking = true; 
        attackTimer = attackCooldown; 

        
		if (GD.Randf() < 0.5f){
			GD.Print("Entro");
			_change_state("punch");
		}
		else{
			GD.Print("Entro 2");
			_change_state("kick");        
		}

        GetNode<AnimatedSprite2D>("./AnimatedSprite2D").AnimationFinished += ChangeToIdle;
		        
    }

	private void ChangeToIdle(){
		currentAnimation = "idle";
		_change_state("idle");
		isAttacking = false;
	}

    private void _change_state(string newState)
    {
        if (currentAnimation == newState) return;
        currentAnimation = newState;
        animatedSprite2D.Play(newState);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            (this.GetParent().GetParent() as CombatScreen).FinishCombat(0);
        }

		healthBar.Value = health;
    }

	private void _on_hitbox_punch_body_entered(Node2D body)
    {
		GD.Print("Ahora si");
		if (body is Player targetPlayer)
		{
			ApplyAttackToPlayer(targetPlayer, 5, 12);
		}
    }

    public void _on_hitbox_kick_body_entered(Node2D body)
    {
        if (body is Player targetPlayer)
		{
			ApplyAttackToPlayer(targetPlayer, 5, 10);
		}
    }

    public void _on_hitbox_ultimate_kick_body_entered(Node2D body)
    {
        if (body is Player targetPlayer)
		{
			ApplyAttackToPlayer(targetPlayer, 30, 50);
		}
    }

	private void ApplyAttackToPlayer(Player target, int damage, int energyGain)
	{
		GD.Print($"El enemigo golpea al jugador por {damage} de daño.");
		target.TakeDamage(damage);
		energy += energyGain;
		energyBar.Value = energy;
	}
}
