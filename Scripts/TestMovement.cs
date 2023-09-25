using Godot;
using System;

public partial class TestMovement : CharacterBody2D
{
	public TileMap map;

	public float speed;
	public float accel;
	public float decel;
	public bool moving;

	public float movementMag;

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
		accel = 40;
		speed = 5;
		decel = 10;

		coords = new Vector2I();
	}

	public override void _PhysicsProcess(double delta)
	{
		movement = new Vector2();
		testPosition = this.Position;
		vel = Velocity;
		movementMag = Mathf.Sqrt(Velocity.X * Velocity.X + Velocity.Y * Velocity.Y);

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
			movementMag = Mathf.Sqrt(Velocity.X * Velocity.X + Velocity.Y * Velocity.Y);

			if(movement.X != lastMovement.X || movement.Y != lastMovement.Y)
			{
				Velocity = Vector2.Zero;
				vel = new Vector2(movement.X * movementMag, movement.Y * movementMag);

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
		if((colli != null) && (movement.X == 0 || movement.Y == 0))
		{
			surfaceVec = colli.GetNormal();
			colPos = colli.GetPosition();
			surfaceVec = new Vector2(surfaceVec.Y, -1 * surfaceVec.X).Normalized();
			debugLine.SetPointPosition(0, colPos);
			debugLine.SetPointPosition(1, colPos + (surfaceVec * 25));

			colli = MoveAndCollide(surfaceVec * movementMag, true);
			if(colli != null)
			{
				//Probably on corner, this surface will lead to getting stuck inside
				//still gets stuck but best I can do without making it ridiculous atm, wiggle it to the side of movement till it gets unstuck
				this.Position += movement.Rotated(Mathf.DegToRad(90)) * (float)delta * 2;
			}
			
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
