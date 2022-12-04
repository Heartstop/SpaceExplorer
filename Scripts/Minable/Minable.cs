using Godot;

namespace SpaceExplorer.Scripts.Minable;

[Tool]
public class Minable : RigidBody2D
{
    [Export] public int SpriteIndex = 0;

    public override void _Ready()
    {
        base._Ready();
        GetNode<AnimatedSprite>("AnimatedSprite").Frame = SpriteIndex;
    }

}
