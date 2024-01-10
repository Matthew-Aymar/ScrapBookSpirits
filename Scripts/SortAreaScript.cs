using Godot;
using System;

public partial class SortAreaScript : Area2D
{
	Sprite2D sprite;
	int originalZ;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite = (Sprite2D)this.GetNode("../");
		originalZ = sprite.ZIndex;
		
	}

	private void _on_body_entered(PhysicsBody2D body)
	{
		if(body.Name.Equals("Player"))
		{
			sprite.ZIndex = body.ZIndex + 1;
		}
	}

	private void _on_body_exited(PhysicsBody2D body)
	{
		if(body.Name.Equals("Player"))
		{
			sprite.ZIndex = originalZ;
		}
	}
}
