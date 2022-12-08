using System.Linq;
using Godot;

namespace SpaceExplorer.Scripts.Actor;

public class TrajectoryPoint : Area2D
{
    public TrajectoryPoint? NextPoint { get; set; }
    public Vector2 PointVelocity;
    public float TimeStep = 1;

    public override void _Ready()
    {
        var shape = new CollisionShape2D
        {
            Shape = new CircleShape2D
            {
                Radius = 1
            },
        };

        CollisionMask = 1;
        CollisionLayer = 0;
        AddChild(shape);
    }

    public override void _PhysicsProcess(float delta)
    {
        if(NextPoint is null)
            return;

        var overlappingAreas = GetOverlappingAreas();
        var force = overlappingAreas.Count == 0
            ? Vector2.Zero
            : overlappingAreas
            .Cast<Area2D>()
            .Where(area => area.GravityPoint)
            .Select(area =>
            {
                var directionVector = area.GlobalPosition - GlobalPosition;
                var scaledDistance = directionVector.Length() * area.GravityDistanceScale;
                var gravityForce = directionVector.Normalized() * (area.Gravity / (scaledDistance * scaledDistance));
                return gravityForce;
            })
            .Aggregate((vec1, vec2) => vec1 + vec2);

        NextPoint.PointVelocity = TimeStep * force + PointVelocity;
        NextPoint.GlobalPosition = GlobalPosition + PointVelocity * TimeStep;
    }
}