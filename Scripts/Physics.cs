using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;

namespace SpaceExplorer.Scripts;

public static class Physics
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 CalculateGravity(IEnumerable<Area2D> gravityFields, Vector2 globalPosition) =>
        gravityFields.Select(area =>
        {
            var directionVector = area.GlobalPosition - globalPosition;
            var scaledDistance = directionVector.Length() * area.GravityDistanceScale;
            var gravityForce = directionVector.Normalized() * (area.Gravity / (scaledDistance * scaledDistance));
            return gravityForce;
        }).Aggregate(Vector2.Zero, (vec1, vec2) => vec1 + vec2);
}