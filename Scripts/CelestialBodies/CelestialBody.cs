using Godot;

namespace SpaceExplorer.Scripts;

public class CelestialBody : Node2D
{
    public float CelestialMass => Scale.LengthSquared() * 10;
}