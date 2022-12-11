using System.Collections.Generic;
using SpaceExplorer.Scripts.Minable;

public static class ResourceInventory {
    public static Dictionary<MinableType, int> ResourceCounts = new Dictionary<MinableType, int> {
        { MinableType.Iron, 0},
        { MinableType.Aluminum, 0},
        { MinableType.Copper, 0},
        { MinableType.Glycol, 0},
        { MinableType.Titanium, 0},
        { MinableType.Uranium, 0},
        { MinableType.Tungsten, 0},
        { MinableType.Lead, 0},
    };

    public static void SubtractResources(Dictionary<MinableType, int> x)
    {
        foreach(var resource in x)
            ResourceCounts[resource.Key] -= resource.Value; 
    }

    public static void AddResources(Dictionary<MinableType, int> x)
    {
        foreach(var resource in x)
            ResourceCounts[resource.Key] += resource.Value;
    }
    
}