using Godot;
using System;

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
    private bool isGameOver = false;

    private AnimatedSprite2D animatedSprite2D;
    private Player targetPlayer;
    private bool facingRight = false;
    private string currentAnimation = "idle";

    private bool isRetreating = false;
    private float retreatTimer = 0f;
    private bool isAttacking = false;
    private float attackCooldown = 0.5f; 
    private float attackTimer = 0f;
    private bool appliedDamage = false;

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
        if (isGameOver)
            return;
            
        GD.Print(currentAnimation);
        if (targetPlayer == null)
            return;

        float distanceToPlayer = Position.DistanceTo(targetPlayer.Position);
        Vector2 velocity = Velocity;

        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }

        if (isAttacking) 
        {
            attackTimer -= (float)delta;
            if (attackTimer <= 0)
            {
                isAttacking = false;
                attackTimer = 0f;
                appliedDamage = false;
            }
            Velocity = Vector2.Zero;
            MoveAndSlide();
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
                    energyBar.Value = energy;
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
        if (rnd.Next(0, 1000) < -1 && !isRetreating)
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
        appliedDamage = false;
        
        if (GD.Randf() < 0.5f)
        {
            GD.Print("Entro");
            _change_state("punch");
        }
        else
        {
            GD.Print("Entro 2");
            _change_state("kick");        
        }

        animatedSprite2D.AnimationFinished -= ChangeToIdle;
        animatedSprite2D.AnimationFinished += ChangeToIdle;
    }

    private void ChangeToIdle()
    {
        animatedSprite2D.AnimationFinished -= ChangeToIdle;
        _change_state("idle");
        isAttacking = false;
        appliedDamage = false;
    }

    private void _change_state(string newState)
    {
        if (currentAnimation == newState)
            return;
        currentAnimation = newState;
        animatedSprite2D.Play(newState);
    }

    public void TakeDamage(int damage)
    {
        if (isGameOver)
            return;
            
        health -= damage;
        health = Math.Max(0, health); 
        
        healthBar.Value = health;
        
        if (health <= 0)
        {
            GD.Print("Enemy health reached zero, ending combat");
            FinishCombat(0);
        }
    }
    
    private void FinishCombat(int winner)
    {
        if (isGameOver)
            return;
        
        isGameOver = true;
        var combatScreen = GetParent().GetParent() as CombatScreen;
        if (combatScreen != null)
        {
            combatScreen.FinishCombat(winner);
        }
        else
        {
            GD.Print("CombatScreen is null, can't finish combat");
        }
    }

    public int GetHealth()
    {
        return health;
    }

    private void _on_hitbox_punch_body_entered(Node2D body)
    {
        if (appliedDamage || isGameOver)
            return;
            
        GD.Print("Ahora si");
        if (body is Player player)
        {
            ApplyAttackToPlayer(player, 5, 12);
            appliedDamage = true;
        }
    }

    public void _on_hitbox_kick_body_entered(Node2D body)
    {
        if (appliedDamage || isGameOver)
            return;
            
        if (body is Player player)
        {
            ApplyAttackToPlayer(player, 5, 10);
            appliedDamage = true;
        }
    }

    public void _on_hitbox_ultimate_kick_body_entered(Node2D body)
    {
        if (appliedDamage || isGameOver)
            return;
            
        if (body is Player player)
        {
            ApplyAttackToPlayer(player, 30, 50);
            appliedDamage = true;
        }
    }

    private void ApplyAttackToPlayer(Player target, int damage, int energyGain)
    {
        if (isGameOver)
            return;
            
        GD.Print($"El enemigo golpea al jugador por {damage} de daño. Vida restante: {target.GetHealth() - damage}");
        target.TakeDamage(damage);
        energy += energyGain;
        energy = Math.Min(energy, maxEnergy); 
        energyBar.Value = energy;
        
        if (target.GetHealth() <= 0)
        {
            GD.Print("El jugador ha muerto, terminando combate");
            FinishCombat(1); 
        }
    }
}