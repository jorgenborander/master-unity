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

    int count = 1;


    protected override void OnIterationStart()
    {   
        count += 1;
        var taggedObjects = tagManager.Query<CameraTag>();

        foreach (var taggedObject in taggedObjects){
            var camera = taggedObject.GetComponent<Camera>();

            currentInstanceSheep = GameObject.Instantiate(Sau.Sample());
            var p = getRandomPosition();
            currentInstanceSheep.transform.position = p;

            var renderer = currentInstanceSheep.GetComponent<Renderer>();
            var bounds = renderer.bounds;

            var imgHight = 1080;
            var imgWidth = 1920;

            var yxMin = (bounds.center) - (bounds.extents);
            var yxMax = (bounds.center) + (bounds.extents);

            var centerBounds = camera.WorldToScreenPoint(bounds.center);

            Vector3 minPos = camera.WorldToScreenPoint(yxMin);
            Vector3 maxPos = camera.WorldToScreenPoint(yxMax);

            var centerX = centerBounds.x / imgWidth;
            var centerY = 1 - (centerBounds.y / imgHight);

            var pixelWidth = (maxPos.x - minPos.x) / imgWidth;
            var pixelHight = (maxPos.y - minPos.y) / imgHight;

            string path = "/Users/jorgen/Documents/Skole/NTNU/Unity/rgb_"+count+".txt";

            using(var sw = new StreamWriter(path, true))
            {
                sw.WriteLine("0 " + centerX + " " + centerY + " " + pixelWidth + " " + pixelHight);
            }


            //Debug.Log("0 " + centerX + " " + centerY + " " + pixelWidth + " " + pixelHight);


        }
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
