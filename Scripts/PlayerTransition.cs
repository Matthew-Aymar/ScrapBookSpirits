using Godot;
using System;

public partial class PlayerTransition : AnimatedSprite2D
{
	private Vector2 startPosition;
	private float speed;
	public bool spawning;
	public int spawnFrame;
	
	public CanvasItem player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		startPosition = this.Position;
		speed = 2500;
		spawning = false;
		spawnFrame = 0;

		player = (CanvasItem)GetNode("../CombatPlayer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!spawning)
		{
			if(this.Position.Y < 200)
			this.Position = new Vector2(0, this.Position.Y + (float)(speed * delta));
			else
			{
				spawning = true;
				this.Play("spawn");
				this.Position = new Vector2(0, this.Position.Y - 128);
			}
		}
	}

	public void Reset()
	{
		this.Position = startPosition;
	}

	public void CheckFrame()
	{
		if(spawning)
		{
			spawnFrame++;
			if(spawnFrame == 4)
			{
				player.ProcessMode = Node.ProcessModeEnum.Pausable;
				player.Visible = true;

				Health hp = (Health)this.GetNode("../Combat_UI/Health");
				Clock cl = (Clock)this.GetNode("../Combat_UI/Clock");
				hp.StartText();
				cl.StartClock();
			}
		}
	}
}
