using System;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers;
using UnityEngine.Perception.Randomization.Samplers;
using UnityEngine.Perception.Randomization.Utilities;
using UnityEngine.Scripting.APIUpdating;
using System.Collections;
using System.Timers;

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

    int countIteration;

    int countFrame;
    int countTotalFrame = 2;

    public CategoricalParameter<GameObject> Tree;

    private GameObject currentInstanceTree;


    protected override void OnIterationStart()
    {   
        countFrame = 0;
            /*currentInstanceTree = GameObject.Instantiate(Tree.Sample());
            var d = getTreePosition();
            currentInstanceTree.transform.position = d;

            currentInstanceSheep = GameObject.Instantiate(Sau.Sample());
            var p = getRandomPosition();
            currentInstanceSheep.transform.position = p;*/
    }
    
    protected override void OnUpdate(){
        if(countFrame == 0){
            currentInstanceTree = GameObject.Instantiate(Tree.Sample());
            var d = getTreePosition();
            currentInstanceTree.transform.position = d;
        }

        if(countFrame == 3){
            GameObject.Destroy(currentInstanceTree);
            GameObject.Destroy(currentInstanceSheep);
        }

        if(countFrame == 2){

            currentInstanceSheep = GameObject.Instantiate(Sau.Sample());

            var taggedObjects = tagManager.Query<CameraTag>();

            foreach (var taggedObject in taggedObjects){

                var camera = taggedObject.GetComponent<Camera>();


                Vector3[] vertices;

                var mesh = currentInstanceSheep.GetComponentInChildren<MeshCollider>().sharedMesh;
                vertices = mesh.vertices;
                int countHit = 0;
                
                var p = getRandomPosition();
                Matrix4x4 localToWorld = currentInstanceSheep.transform.GetChild(1).localToWorldMatrix;
                localToWorld[0,3] = p.x;
                localToWorld[1,3] = p.y;
                localToWorld[2,3] = p.z;
                
                /*
                for(int i = 0; i<vertices.Length; ++i){
                    var point = localToWorld.MultiplyPoint3x4(vertices[i]);
                    if (Physics.Linecast(camera.transform.position, point, out RaycastHit hitInfo))
                    {
                        countHit = countHit + 1;
                    }
                }
                Debug.Log(countHit);
                double sum = (double) countHit / (double) vertices.Length;
                Debug.Log(sum);*/

                while(true){
                    countHit = 0;
                    p = getRandomPosition();
                    localToWorld = currentInstanceSheep.transform.GetChild(1).localToWorldMatrix;
                    localToWorld[0,3] = p.x;
                    localToWorld[1,3] = p.y;
                    localToWorld[2,3] = p.z;
                    for(int i = 0; i<vertices.Length; ++i){
                        var point = localToWorld.MultiplyPoint3x4(vertices[i]);
                        if (Physics.Linecast(camera.transform.position, point, out RaycastHit hitInfo))
                        {
                            countHit = countHit + 1;
                        }
                    }
                    double sum = (double) countHit / (double) vertices.Length;
                    if(((sum) > 0.5 && (sum) < 0.95)){
                        break;
                    }
                }

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
                
                System.IO.Directory.CreateDirectory("Generated/BoundingBoxes");

                string path = "./Generated/BoundingBoxes/rgb_"+countTotalFrame+".txt";

                using(var sw = new StreamWriter(path, true))
                {
                    sw.WriteLine("0 " + centerX + " " + centerY + " " + pixelWidth + " " + pixelHight);
                }
            }

            }
            
        countFrame += 1;
        countTotalFrame +=1;
    }

    protected override void OnIterationEnd()
    {
        //GameObject.Destroy(currentInstanceSheep);
        //GameObject.Destroy(currentInstanceTree);
    }

    private Vector3 getRandomPosition()
    {
        // Randomize position on surface
        var p = SurfaceBounds.Sample();
        RaycastHit hit;
        Physics.Raycast(new Vector3(p.x, 11, p.y), Vector3.down, out hit);
        return hit.point;
    }

    private Vector3 getTreePosition()
    {
        // Randomize position on surface
        var p = SurfaceBounds.Sample();
        RaycastHit hit;
        Physics.Raycast(new Vector3(8, 100, 0), Vector3.down, out hit);
        return hit.point;
    }
}
