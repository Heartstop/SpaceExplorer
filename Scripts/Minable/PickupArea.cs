using Godot;
using SpaceExplorer.Scripts.Minable;

public class PickupArea : Area2D
{
	public override void _Ready()
	{
		Connect("body_entered", this, nameof(OnBodyEntered));
	}

	public void OnBodyEntered(PhysicsBody2D body)
	{
		if (body is Minable minable)
		{
			minable.GetParent().RemoveChild(minable);
		}
	}
}
