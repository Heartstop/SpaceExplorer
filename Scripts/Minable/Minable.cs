using System.Collections.Generic;
using Godot;

namespace SpaceExplorer.Scripts.Minable;

[Tool]
public class Minable : RigidBody2D
{
	[Export]
	public int SpriteIndex = 0;

	[Export]
	public float TimeToMine = 2.5;

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
		animationPlayer.PlaybackSpeed = 1;
		animationPlayer.Advance(SpriteIndex);
		animationPlayer.PlaybackSpeed = 0;
		TimeLeftToMine = TimeToMine;
		Connect(nameof(Mined), this, nameof(OnMined));
	}
	
	public void OnMined(MiningLaser miner, float miningProgress)
	{
		TimeLeftToMine -= miningProgress;
		if (TimeLeftToMine <= 0)
		{
			miner.EmitSignal(nameof(MiningLaser.Collected));
			ResourceInventory.AddResources(new Dictionary<MinableType, int> { { MinableType, 1 }});
			QueueFree();
		}
	}
}
