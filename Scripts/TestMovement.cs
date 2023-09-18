using Godot;
using System;

public partial class TestMovement : CharacterBody2D
{
	public TileMap map;

	public float speed;
	public float accel;
	public float decel;
	public Vector2 direction;

	Vector2 testPosition = new Vector2();
	Vector2 movement = new Vector2();
	Vector2 lastMovement = new Vector2();
	Vector2 vel = new Vector2();
	Vector2I coords = new Vector2I();
	TileData currentTile = new TileData();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		map = (TileMap)GetNode("../");

		direction = new Vector2();
		accel = 25;
		speed = 6;
		decel = 7;

		coords = new Vector2I();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		movement = new Vector2();
		testPosition = this.Position;
		vel = Velocity;

		if(Input.IsActionPressed("left"))
		{
			movement.X -= 1;
		}
		if(Input.IsActionPressed("right"))
		{
			movement.X += 1;
		}
		if(Input.IsActionPressed("up"))
		{
			movement.Y -= 1;
		}
		if(Input.IsActionPressed("down"))
		{
			movement.Y += 1;
		}
		movement = movement.Normalized();

		//There is any movement...
		if(movement.X != 0 || movement.Y != 0)
		{
			vel += movement * accel * (float)delta;

			testPosition += vel;
			coords = map.LocalToMap(testPosition);
			currentTile = map.GetCellTileData(1, coords);
			if(currentTile != null)
			{
				vel = Vector2.Zero;
			}

			if(movement.X != lastMovement.X || movement.Y != lastMovement.Y)
				{
					float mag = Mathf.Sqrt(Velocity.X * Velocity.X + Velocity.Y * Velocity.Y);
					Velocity = Vector2.Zero;
					vel = new Vector2(movement.X * mag, movement.Y * mag);

					lastMovement = movement;
				}

			vel.X = Mathf.Clamp(vel.X, -speed, speed);
			if(movement.X != 0 && movement.Y != 0)
			{
				vel.Y = Mathf.Clamp(vel.Y, -speed * 0.5f, speed * 0.5f);
			}
			else
				vel.Y = Mathf.Clamp(vel.Y, -speed, speed);
		}
		else //Start Decellerating
		{
			vel += new Vector2(Velocity.X * -1, Velocity.Y * -1) * decel * (float)delta;
			if(vel.Y > -1 && vel.Y < 1)
				vel.Y = 0;
			if(vel.X > -1 && vel.X < 1)
				vel.X = 0;
		}

		Velocity = vel;
		MoveAndCollide(Velocity);
	}
}
