using Godot;
using System;

public partial class BackgroundImage : Node2D
{
	private const float CLOUD_SPEED = 30.0f;
	private const float STORK_SPEED = 30.0f;
	private const float STORK_SPAWN_TIME = 30.0f; // intervalo entre spawns

	private Sprite2D clouds1, clouds2, clouds3;
	private AnimatedSprite2D stork, fredOlsenShip, armasShip;
	private Random random = new Random();
	private Timer storkTimer;
	private string currentStage;

	public void SetStage(string stage)
	{
		currentStage = stage;
		SetupStage();
	}

	public override void _Ready()
	{
		storkTimer = new Timer();
		storkTimer.WaitTime = STORK_SPAWN_TIME
;
		storkTimer.OneShot = false;
		AddChild(storkTimer);
		storkTimer.Timeout += SpawnStork;
	}

	private void SetupStage()
	{
		// volver a estado inicial
		if (clouds1 != null) clouds1.Visible = false;
		if (clouds2 != null) clouds2.Visible = false;
		if (stork != null) stork.Visible = false;

		if (clouds3 != null) clouds2.Visible = false;
		if (fredOlsenShip != null) fredOlsenShip.Visible = false;
		if (armasShip != null) armasShip.Visible = false;

		// cambia según el escenario
		switch (currentStage)
		{
			case "airplane":
				clouds1 = GetNode<Sprite2D>("Clouds1");
				clouds2 = GetNode<Sprite2D>("Clouds2");
				stork = GetNode<AnimatedSprite2D>("Stork");

				clouds1.Visible = clouds2.Visible =stork.Visible = true;
				
				clouds1.Position = new Vector2(clouds1.Position.X + clouds1.Texture.GetWidth(), clouds1.Position.Y);
				clouds2.Position = new Vector2(clouds2.Position.X + clouds2.Texture.GetWidth(), clouds2.Position.Y);

				storkTimer.Start();
				break;

			case "la_gomera":
				clouds3 = GetNode<Sprite2D>("Clouds3");
				fredOlsenShip = GetNode<AnimatedSprite2D>("FredOlsenShip");
				armasShip = GetNode<AnimatedSprite2D>("ArmasShip");

				clouds3.Visible = fredOlsenShip.Visible = armasShip.Visible = true;
				
				clouds3.Position = new Vector2(clouds3.Position.X + clouds3.Texture.GetWidth(), clouds3.Position.Y);
				fredOlsenShip.Position = new Vector2();
				armasShip.Position = new Vector2();

				break;
		}
	}

	public override void _Process(double delta)
	{
		float deltaTime = (float)delta;

		if (clouds1 != null && clouds1.Visible)
		{
			clouds1.Position -= new Vector2(CLOUD_SPEED * deltaTime, 0);
			if (clouds1.Position.X <= -clouds1.Texture.GetWidth())
				clouds1.Position = new Vector2(GetViewportRect().Size.X, clouds1.Position.Y);
		}

		if (clouds2 != null && clouds2.Visible)
		{
			clouds2.Position -= new Vector2(CLOUD_SPEED * deltaTime, 0);
			if (clouds2.Position.X <= -clouds2.Texture.GetWidth())
				clouds2.Position = new Vector2(clouds1.Position.X + clouds1.Texture.GetWidth(), clouds2.Position.Y);
		}

		if (stork != null && stork.Visible)
		{
			stork.Position += new Vector2(STORK_SPEED * deltaTime, 0);
			if (stork.Position.X > GetViewportRect().Size.X)
				stork.Visible = false;
		}
	}

	private void SpawnStork()
	{
		if (stork != null && !stork.Visible)
		{
			stork.Position = new Vector2(-stork.SpriteFrames.GetFrameTexture("fly", 0).GetWidth(), random.Next(50, 200));
			stork.Visible = true;
			stork.Play("fly");
		}
	}
}
