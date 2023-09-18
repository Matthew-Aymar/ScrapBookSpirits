using Godot;
using System;

public partial class TestMovement : RigidBody2D
{
	public TileMap map;

	public float speed;
	public Vector2 direction;
	public bool ascending;
	public bool descending;
	public int currentLayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		map = (TileMap)GetNode("../");

		currentLayer = 0;
		ascending = false;
		descending = false;
		direction = new Vector2();
		speed = 300;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 movement = new Vector2();
		Vector2 velocity = new Vector2();
		Vector2I coords = new Vector2I();
		TileData currentTile = new TileData();

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

		if(movement.X != 0 || movement.Y != 0)
		{
			velocity = movement * (float)delta * speed;
			coords = map.LocalToMap(this.Position + movement);
			currentTile = map.GetCellTileData(currentLayer + 1, coords);
			if(currentTile != null)
			{
				if(currentTile.ZIndex > this.ZIndex && !ascending)
				{
					ascending = true;
				}
				else if(currentTile.ZIndex < this.ZIndex && !descending)
				{
					descending = true;
				}
			}

			KinematicCollision2D colli = MoveAndCollide(velocity, true);
			if(colli != null)
			{
				Vector2 adjMove1 = new Vector2();
				Vector2 adjMove2 = new Vector2();

				if(movement.X != 0 && movement.Y != 0)
				{
					adjMove1.X = movement.X;
					adjMove2.Y = movement.Y;
				}
				else if(movement.X == 0)
				{
					adjMove1.X = -1;
					adjMove1.Y = movement.Y * 0.5f;

					adjMove2.X = 1;
					adjMove2.Y = movement.Y * 0.5f;
				}
				else if(movement.Y == 0)
				{
					adjMove1.X = movement.X;
					adjMove1.Y = -0.5f;

					adjMove2.X = movement.X;
					adjMove2.Y = 0.5f;
				}

				Vector2 tempVel = adjMove1 * (float)delta * speed;
				KinematicCollision2D tempColli = MoveAndCollide(tempVel, true);
				if(tempColli != null)
				{
					tempVel = adjMove2 * (float)delta * speed;
					tempColli = MoveAndCollide(tempVel, true);
					if(tempColli != null)
					{
						if(movement.X < 0)
							tempVel = new Vector2(0, -2) * (float)delta * speed;
						else if(movement.X > 0)
							tempVel = new Vector2(0, 2) * (float)delta * speed;
						else if(movement.Y < 0)
							tempVel = new Vector2(2, 0) * (float)delta * speed;
						else if(movement.Y > 0)
							tempVel = new Vector2(-2, 0) * (float)delta * speed;

						MoveAndCollide(tempVel);
					}
					else
					{
						MoveAndCollide(tempVel);
					}
				}
				else
				{
					MoveAndCollide(tempVel);
				}
			}
			else
			{
				MoveAndCollide(movement);
			}
		}

		if(movement.X != 0 && movement.Y != 0)
		{
			movement.Y *= 0.5f;
		}
	}
}
