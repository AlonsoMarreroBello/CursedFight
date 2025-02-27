using Godot;
using System;

public partial class Parallax2d : Parallax2D
{
	Sprite2D gomeraClouds;
	Sprite2D airplaneClouds;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gomeraClouds = this.GetNode<Sprite2D>("./Clouds3");
		airplaneClouds = this.GetNode<Sprite2D>("./CloudsAirplane");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		Vector2 velocity = new(10, 0);

		gomeraClouds.GlobalPosition = gomeraClouds.GlobalPosition + (velocity * (float)delta);
		airplaneClouds.GlobalPosition = gomeraClouds.GlobalPosition + (velocity * (float)delta);
	}
}
