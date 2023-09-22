using Godot;
using System;

public partial class TestMovement : CharacterBody2D
{
	public TileMap map;

	public float speed;
	public float accel;
	public float decel;
	public bool moving;

	Vector2 testPosition = new Vector2();
	Vector2 movement = new Vector2();
	Vector2 lastMovement = new Vector2();
	Vector2 vel = new Vector2();
	Vector2I coords = new Vector2I();
	TileData currentTile = new TileData();

	Vector2 colPos = new Vector2();
	Vector2 surfaceVec = new Vector2();
	public Line2D debugLine;

	RandomNumberGenerator ran = new RandomNumberGenerator();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		map = (TileMap)GetNode("../");
		debugLine = (Line2D)GetNode("../../Line2D");
		accel = 25;
		speed = 4;
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
			moving = true;
			vel += movement * accel * (float)delta;

			if(movement.X != lastMovement.X || movement.Y != lastMovement.Y)
			{
				float mag = Mathf.Sqrt(Velocity.X * Velocity.X + Velocity.Y * Velocity.Y);
				Velocity = Vector2.Zero;
				vel = new Vector2(movement.X * mag, movement.Y * mag);

				lastMovement = movement;
			}

			vel.X = Mathf.Clamp(vel.X, -speed, speed);
			if(movement.X != 0 && movement.Y != 0)
				vel.Y = Mathf.Clamp(vel.Y, -speed * 0.5f, speed * 0.5f);
			else
				vel.Y = Mathf.Clamp(vel.Y, -speed, speed);
		}
		else //Start Decellerating
		{
			moving = false;
			vel += new Vector2(Velocity.X * -1, Velocity.Y * -1) * decel * (float)delta;
			if(vel.Y > -1 && vel.Y < 1)
				vel.Y = 0;
			if(vel.X > -1 && vel.X < 1)
				vel.X = 0;
		}

		Velocity = vel;
		KinematicCollision2D colli = MoveAndCollide(Velocity, true);
		if(colli != null && (movement.X == 0 || movement.Y == 0))
		{
			surfaceVec = colli.GetNormal();
			colPos = colli.GetPosition();
			surfaceVec = new Vector2(surfaceVec.Y, -1 * surfaceVec.X).Normalized();
			debugLine.SetPointPosition(0, colPos);
			debugLine.SetPointPosition(1, colPos + (surfaceVec * 100));
			
			colli = MoveAndCollide(surfaceVec, true);
			if(colli != null)
			{
				//Probably on corner, this surface will lead to getting stuck inside
				//Find and pick an adjacent movement that will free us
				GD.Print("Will get stuck");
			}

			/*
			if(Mathf.Abs(0.895f - Mathf.Abs(surfaceVec.X)) > 0.001f || Mathf.Abs(0.448f - Mathf.Abs(surfaceVec.Y)) > 0.001f)
			{
				
			}
			*/
			float dist1 = movement.DistanceTo(surfaceVec);
			float dist2 = movement.DistanceTo(surfaceVec * -1);

			if(dist1 > dist2)
				movement = surfaceVec * -1;
			else
				movement = surfaceVec;

			float mag = Mathf.Sqrt(Velocity.X * Velocity.X + Velocity.Y * Velocity.Y);
			vel = new Vector2(movement.X * mag, movement.Y * mag);
			
			//This allows for acceleration while on walls
			if(moving)
				vel += movement * accel * (float)delta;

			lastMovement = movement;
			Velocity = vel;
		}
		
		MoveAndCollide(Velocity);
	}
}
