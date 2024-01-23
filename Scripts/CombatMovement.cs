using Godot;
using System;

public partial class CombatMovement : CharacterBody2D
{
	AnimatedSprite2D anim;
	char inputCode;
	string currentAction;

	bool left = false;

	public Vector2 moveDir;
	public KinematicCollision2D col;

	//Movement Vars
	public float speed;
	public float accel;
	public float decel;

	//Jump Vars
	public float force;
	public float grav;
	public bool grounded;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		anim = (AnimatedSprite2D)GetNode("./AnimatedSprite2D");
		anim.Play("Idle");
		currentAction = "Idle";

		speed = 13;
		accel = 35;
		decel = 40;

		force = 0;
		grav = 10;
		grounded = true;

		moveDir = new Vector2(0,0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Inputs are sorted in a way that specifies priority in case multiple inputs are clicked at the same time.
		inputCode = 'I'; 		//Idle

		if(Input.IsActionPressed("left"))
		{
			inputCode = 'L';	//Left Move
		}
		if(Input.IsActionPressed("right"))
		{
			inputCode = 'R';	//Right Move
		}

		if(Input.IsActionJustPressed("attack"))
		{
			inputCode = 'A';	//Attack
		}

		if(Input.IsActionJustPressed("special"))
		{
			inputCode = 'S';	//Special
		}

		if(Input.IsActionJustPressed("up") || Input.IsActionJustPressed("jump"))
		{
			inputCode = 'J';	//Jump
		}

		InputResolve();
	}

	public override void _PhysicsProcess(double delta)
	{
		if(currentAction != "Idle")
			Move(delta);
		
		if(currentAction == "Jump")
			Jump(delta);

		Velocity = moveDir;
		col = MoveAndCollide(Velocity);
	}

	private void InputResolve()
	{
		if(inputCode == 'I')
		{
			if(IsActionDone())
			{
				if(!anim.Animation.Equals("Idle"))
					anim.Play("Idle");

				currentAction = "Idle";
			}
		}
		else if(inputCode == 'L')
		{
			if(IsActionDone())
			{
				if(!left)
				{
					left = true;
					anim.FlipH = true;
				}

				if(!anim.Animation.Equals("Move"))
					anim.Play("Move");

				currentAction = "Movement";
			}
		}
		else if(inputCode == 'R')
		{
			if(IsActionDone())
			{
				if(left)
				{
					left = false;
					anim.FlipH = false;
				}

				if(!anim.Animation.Equals("Move"))
					anim.Play("Move");

				currentAction = "Movement";
			}
		}
		else if(inputCode == 'A')
		{
			if(IsActionDone())
			{
				currentAction = "Attack";
			}
		}
		else if(inputCode == 'S')
		{
			if(IsActionDone())
			{
				currentAction = "Special";
			}
		}
		else if(inputCode == 'J')
		{
			if(IsActionDone())
			{
				currentAction = "Jump";
			}
		}
	}

	private bool IsActionDone()
	{
		if(currentAction == "Idle")
			return true;
		else if(currentAction == "Movement")
		{
			if(inputCode == 'I')
			{
				if(Mathf.Abs(moveDir.X) <= 0.1f)
				{
					moveDir.X = 0;
					return true;
				}
			}
			else
				return true;
		}
		else if(currentAction == "jump")
		{
			if(grounded)
				return true;
		}

		return false;
	}

	private void Move(double delta)
	{
		bool lastLeft = false;
		if(moveDir.X < 0)
			lastLeft = true;

		moveDir.X = Mathf.Abs(moveDir.X);
		if(inputCode == 'L' || inputCode == 'R')
		{
			moveDir.X += accel * (float)delta;
			moveDir.X = Mathf.Clamp(moveDir.X, -speed, speed);
		}
		else if(moveDir.X > 0)
		{
			moveDir.X -= decel * (float)delta;
			if(moveDir.X < 0)
				moveDir.X = 0;
		}
		
		if(currentAction == "Jump")
		{
			if((left && inputCode == 'L') || (!left && inputCode == 'R'))
				moveDir *= 1.25f;
			else if((left && inputCode != 'L') || (!left && inputCode != 'R'))
				moveDir *= 0.75f;
		}
		else
		{
			if(lastLeft != left)
				moveDir.X *= 0.5f;	
		}

		if(left)
			moveDir.X *= -1;
	}

	private void Jump(double delta)
	{
		if(grounded)
		{
			force = 40;
			grav = 75;
			grounded = false;
		}
		else
		{
			force -= grav * (float)delta;
			grav += grav * (float)delta;
		}

		moveDir.Y = -1 * force;

		if(col != null && force < 0)
		{
			grounded = true;
			moveDir.Y = 0;
			currentAction = "Movement";
		}
	}
}
