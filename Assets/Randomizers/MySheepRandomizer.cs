using System;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers;
using UnityEngine.Perception.Randomization.Samplers;
using UnityEngine.Perception.Randomization.Utilities;
using UnityEngine.Scripting.APIUpdating;

// Add this Component to any GameObject that you would like to be randomized. This class must have an identical name to
// the .cs file it is defined in.
public class MySheepRandomizerTag : RandomizerTag {}

[Serializable]
[AddRandomizerMenu("MySheepRandomizer")]
public class MySheepRandomizer : Randomizer
{   
    public Vector2Parameter SurfaceBounds;

    public CategoricalParameter<GameObject> Sau;

    private GameObject currentInstanceSheep;


    protected override void OnIterationStart()
    {   

        currentInstanceSheep = GameObject.Instantiate(Sau.Sample());
        var p = getRandomPosition();
        currentInstanceSheep.transform.position = p;
    }

    protected override void OnIterationEnd()
    {
        GameObject.Destroy(currentInstanceSheep);
    }

    private Vector3 getRandomPosition()
    {
        // Randomize position on surface
        var p = SurfaceBounds.Sample();
        RaycastHit hit;
        Physics.Raycast(new Vector3(p.x, 100, p.y), Vector3.down, out hit);
        return hit.point;
    }
}
