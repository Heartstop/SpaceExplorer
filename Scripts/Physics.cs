using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;
using SpaceExplorer.Scripts.CelestialBodies;

namespace SpaceExplorer.Scripts;

public static class Physics
{
    [Pure]
    public static Vector2 CalculateForce(RigidBody2D influenced, IEnumerable<CelestialBody> influencing)
    {
        return influencing
            .Select(other => CalculateForce(influenced, other))
            .Aggregate((v1, v2) => v1 + v2);
    }
    
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 CalculateForce(RigidBody2D influenced, CelestialBody influencing)
    {
        var pull = influenced.Mass * influencing.CelestialMass / influenced.GlobalPosition.DistanceSquaredTo(influencing.GlobalPosition);
        return pull * influenced.GlobalPosition.DirectionTo(influencing.GlobalPosition);
    }
}