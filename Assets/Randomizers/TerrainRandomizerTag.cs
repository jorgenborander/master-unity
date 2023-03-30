using System;
using UnityEngine;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers;
using UnityEngine.Perception.Randomization.Samplers;
using System.Threading;
using MapMagic.Terrains;
using Den.Tools;

// Add this Component to any GameObject that you would like to be randomized. This class must have an identical name to
// the .cs file it is defined in.
public class TerrainRandomizerTag : RandomizerTag {}

[Serializable]
[AddRandomizerMenu("Perception/Terrain Randomizer")]
public class TerrainRandomizer : Randomizer
{

    int count = 0;

    protected override void OnIterationStart()
    {
        count += 1;
        var taggedObjects = tagManager.Query<TerrainRandomizerTag>();
        foreach (var taggedObject in taggedObjects)
        {
            var mapMagic = taggedObject.GetComponent<MapMagic.Core.MapMagicObject>();

            var graph = mapMagic.graph;

            graph.random = new Noise(count, 32768);

            mapMagic.Refresh();
        }
        base.OnIterationStart();
    }
}
