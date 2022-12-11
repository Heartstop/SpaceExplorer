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
                var minable = minableScene.Instance<Minable>();
                minable.SpriteIndex = random.Next(0, 4);
                minable.Position = new Vector2(Mathf.Sin(cluster.Angle), Mathf.Cos(cluster.Angle)) * SpawnRadius;
                minable.Rotation = minable.Position.Angle() + Mathf.Pi * 0.5f;
                AddChild(minable);
            }
        }
    }

    private static PackedScene GetMinable(MinableType minableType)
    {
        var scenePath = minableType switch
        {
            MinableType.Iron => "res://Scenes/Minable/Iron.tscn",
            MinableType.Aluminum => "res://Scenes/Minable/Aluminum.tscn",
            MinableType.Copper => "res://Scenes/Minable/Copper.tscn",
            MinableType.Glycol => "res://Scenes/Minable/Glycol.tscn",
            MinableType.Titanium => "res://Scenes/Minable/Titanium.tscn",
            MinableType.Uranium => "res://Scenes/Minable/Uranium.tscn",
            MinableType.Lead => "res://Scenes/Minable/Lead.tscn",
            MinableType.Tungsten => "res://Scenes/Minable/Tungsten.tscn",
            _ => throw new ArgumentOutOfRangeException()
        };

        return ResourceLoader.Load<PackedScene>(scenePath);
    }
}