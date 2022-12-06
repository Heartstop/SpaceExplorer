using Godot;

namespace SpaceExplorer.Scripts.Minable;

[Tool]
public class Minable : RigidBody2D
{
	[Export]
	public int SpriteIndex = 0;

	[Export]
	public float TimeToMine = 5;

	private float TimeLeftToMine;

	[Signal]
	delegate void Mined();
	
	public override void _Ready()
	{
		base._Ready();
		GetNode<AnimatedSprite>("AnimatedSprite").Frame = SpriteIndex;
		TimeLeftToMine = TimeToMine;
		Connect("Mined", this, nameof(OnMined));
	}
	
	public void OnMined(float miningProgress)
	{
		TimeLeftToMine -= miningProgress;
		if (TimeLeftToMine <= 0)
		{
			QueueFree();
		}
	}
}
