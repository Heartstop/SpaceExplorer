using System.Linq;
using Godot;
using SpaceExplorer.Scripts.Minable;

public class TractorBeam : Area2D
{
    public float PullSpeed => 1f;
    public override void _PhysicsProcess(float delta)
    {
        foreach (var minable in GetOverlappingBodies().OfType<Minable>())
        {
            minable.LinearVelocity += delta * PullSpeed * minable.GlobalPosition.DirectionTo(GlobalPosition);
        }
    }
}
