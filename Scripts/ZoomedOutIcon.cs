using System;
using Godot;

public class ZoomedOutIcon : Sprite
{
	[Export]
	public Boolean AlwaysHidden { get; set; } = true;
	public override void _Ready()
	{
	}
}
