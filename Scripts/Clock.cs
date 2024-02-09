using Godot;
using System;

public partial class Clock : AnimatedSprite2D
{
	public AnimatedSprite2D bigHand;
	public AnimatedSprite2D smallHand;

	public float startRot; //In degrees

	public Color rainbowColor;
	public float drawSpeed;
	public float drawTime;

	private float drawPercent;
	private bool canDraw;
	private int colorPhase;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		bigHand = (AnimatedSprite2D)this.GetNode("BigHand");
		smallHand = (AnimatedSprite2D)this.GetNode("SmallHand");

		startRot = 35;
		drawSpeed = 5;
		drawTime = 0;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsKeyPressed(Key.R))
			StartClock();

		drawPercent = drawTime / drawSpeed;
		float drawDegrees = (drawPercent * 360) + startRot;

		smallHand.RotationDegrees = drawDegrees;
		if(drawTime < drawSpeed)
			drawTime += (float)delta;
		else if(!canDraw)
		{
			rainbowColor = new Color(1,0.85f,0.85f,1);
			colorPhase = 0;
			canDraw = true;
			drawTime = drawSpeed;
		}
		else
			ColorShift((float)delta * 0.2f);
	}

	public void ColorShift(float d)
	{
		if(colorPhase == 0)
		{
			rainbowColor.R -= d;
			rainbowColor.G += d;
			if(rainbowColor.R <= 0.85f)
				colorPhase = 1;
		}
		else if(colorPhase == 1)
		{
			rainbowColor.G -= d;
			rainbowColor.B += d;
			if(rainbowColor.G <= 0.85f)
				colorPhase = 2;
		}
		else if(colorPhase == 2)
		{
			rainbowColor.B -= d;
			rainbowColor.R += d;
			if(rainbowColor.B <= 0.85f)
				colorPhase = 0;
		}

		this.SelfModulate = rainbowColor;
	}

	public void StartClock()
	{
		this.SelfModulate = new Color(1,1,1,1); //Reset to white
		rainbowColor = this.SelfModulate;

		drawTime = 0;
		canDraw = false;

		this.ProcessMode = Node.ProcessModeEnum.Pausable;
		this.Visible = true;
	}
}
