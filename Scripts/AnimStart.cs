using Godot;
using System;

public partial class AnimStart : AnimatedSprite2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void StartNextAnimation(string animName)
	{
		this.Play(animName);
	}
}
