using Godot;
using System;

public partial class Health : AnimatedSprite2D
{
	public int healthState;
	public int lastState;
	public float currentHealth;
	public float maxHealth;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		currentHealth = 100;
		maxHealth = 100;
		
		lastState = (int)((currentHealth / maxHealth) * 5);
		healthState = -1;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(healthState != lastState)
		{
			if(healthState == -1)
			{
				healthState = lastState;
			}

			switch (healthState)
			{
				case 0: 
					Play("bleak");
					break;
				case 1:
					Play("hurting");
					break;
				case 2:
					Play("okay");
					break;
				case 3:
					Play("healthy");
					break;
				case 4:
					Play("lively");
					break;
				case 5:
					Play("lively");
					break;
			}
		}
		healthState = (int)((currentHealth / maxHealth) * 5);
	}

	public void StartText()
	{
		this.ProcessMode = Node.ProcessModeEnum.Pausable;
		this.Visible = true;
	}
}
