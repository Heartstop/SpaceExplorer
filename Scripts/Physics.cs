using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Godot;

namespace SpaceExplorer.Scripts;

public static class Physics
{
    [Pure]
    public static Vector2 CalculateForce(RigidBody2D influenced, IEnumerable<CelestialBody> influencing)
    {
        return influencing
            .Select(other =>
            {
                var pull = influenced.Mass * other.CelestialMass / influenced.GlobalPosition.DistanceSquaredTo(other.GlobalPosition);
                return pull * influenced.GlobalPosition.DirectionTo(other.GlobalPosition);
            })
            .Aggregate((v1, v2) => v1 + v2);
    }
}