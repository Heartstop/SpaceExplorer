using Godot;
using SpaceExplorer.Scripts.Minable;
using System;
using System.Runtime.InteropServices;

public class ResourceLabel : Label
{

    [Export] MinableType MinableType;
    string _label = null!;
    public override void _Ready()
    {
        _label = Text;
    }

    public override void _Process(float delta)
    {
        Text = $"{_label} {ResourceInventory.ResourceCounts[MinableType]}";
    }
}
