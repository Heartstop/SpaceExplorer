using Godot;
using SpaceExplorer.Scripts.Minable;

public class PickupArea : Area2D
{
	AudioStreamPlayer2D _grabAudio = null!;
	public override void _Ready()
	{
		_grabAudio = GetNode<AudioStreamPlayer2D>("GrabAudio");
		Connect("body_entered", this, nameof(OnBodyEntered));
	}

	public void OnBodyEntered(PhysicsBody2D body)
	{
		if (body is Minable minable)
		{
			minable.GetParent().RemoveChild(minable);
			_grabAudio.Play();
		}
	}
}
