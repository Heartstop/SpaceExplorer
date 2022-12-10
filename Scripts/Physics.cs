using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Godot;

namespace SpaceExplorer.Scripts;

public static class Physics
{
	[Pure]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 CalculateGravity(ImmutableArray<Area2D> gravityFields, Vector2 globalPosition)
	{
		var gravityForce = Vector2.Zero;
		foreach (var gravityField in gravityFields)
		{
			var directionVector = gravityField.GlobalPosition - globalPosition;
			var scaledDistance = directionVector.Length() * gravityField.GravityDistanceScale;
			gravityForce += directionVector.Normalized() * (gravityField.Gravity / (scaledDistance * scaledDistance));
		}

		return gravityForce;
	}
}
