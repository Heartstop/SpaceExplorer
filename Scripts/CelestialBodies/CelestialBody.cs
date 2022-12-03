using Godot;

namespace SpaceExplorer.Scripts.CelestialBodies;

public class CelestialBody : Node2D
{
	public float CelestialMass => Scale.LengthSquared() * 1000;
}
