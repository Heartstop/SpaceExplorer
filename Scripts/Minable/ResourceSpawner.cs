using System;
using System.Linq;
using Godot;

namespace SpaceExplorer.Scripts.Minable;

public class ResourceSpawner : Node2D
{
    [Export]
    public float SpawnRadius = 10;

    [Export]
    public int SpawnAmount;

    [Export]
    public int MaxClusters;

    [Export(PropertyHint.Enum)]
    public MinableType MinableType;

    public override void _Ready()
    {
        var random = new Random();
        var minableScene = GetMinable(MinableType);
        foreach (var cluster in Enumerable.Range(0, SpawnAmount)
                     .Select(_ => MaxClusters == 0 ? random.Next() : random.Next(MaxClusters))
                     .GroupBy(i => i)
                     .Select(g => new { Count = g.Count(), Angle = (float)(random.NextDouble() * Math.PI * 2) }))
        {
            for (var i = 0; i < cluster.Count; i++)
            {
                var minable = minableScene.Instance<RigidBody2D>();
                minable.Position = new Vector2(Mathf.Sin(cluster.Angle), Mathf.Cos(cluster.Angle)) * SpawnRadius;
                minable.Rotation = minable.Position.Angle() + Mathf.Pi * 0.5f;
                AddChild(minable);
            }
        }
    }

    private static PackedScene GetMinable(MinableType minableType)
    {
        return minableType switch
        {
            MinableType.Iron => ResourceLoader.Load<PackedScene>("res://Scenes/Minable/Iron.tscn"),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}