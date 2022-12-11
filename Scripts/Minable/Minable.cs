using System.Collections.Generic;
using Godot;

namespace SpaceExplorer.Scripts.Minable;

[Tool]
public class Minable : RigidBody2D
{
	[Export]
	public int SpriteIndex = 0;

	[Export]
	public float TimeToMine = 5;

	[Export]
	public MinableType MinableType = MinableType.Iron;

	private float TimeLeftToMine;

	[Signal]
	delegate void Mined();
	
	public override void _Ready()
	{
		base._Ready();
		var animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animationPlayer.Play("index");
		animationPlayer.Advance(SpriteIndex);
		animationPlayer.PlaybackSpeed = 0;
		TimeLeftToMine = TimeToMine;
		Connect(nameof(Mined), this, nameof(OnMined));
	}
	
	public void OnMined(float miningProgress)
	{
		TimeLeftToMine -= miningProgress;
		if (TimeLeftToMine <= 0)
		{
			ResourceInventory.AddResources(new Dictionary<MinableType, int> { { MinableType, 1 }});
			QueueFree();
		}
	}
}
