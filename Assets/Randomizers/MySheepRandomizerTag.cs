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
using Random=UnityEngine.Random;
using MapMagic.Terrains;
using Den.Tools;

// Add this Component to any GameObject that you would like to be randomized. This class must have an identical name to
// the .cs file it is defined in.
public class MySheepRandomizerTag : RandomizerTag {}

[Serializable]
[AddRandomizerMenu("MySheepRandomizer")]
public class MySheepRandomizer : Randomizer
{   
    public Vector2Parameter SurfaceBoundsSheep;
    public Vector2Parameter SurfaceBoundsTree;

    public CategoricalParameter<GameObject> Sau;

    private GameObject currentInstanceSheep;

    int countIteration;

    int countFrame;
    int countTotalFrame = 2;

    public CategoricalParameter<GameObject> Tree;

    private GameObject currentInstanceTree;

    public CategoricalParameter<AnimationClip> animationClips;

    const string k_StateName = "Base Layer.";

    public CategoricalParameter<Material> materials = new CategoricalParameter<Material>();

    public CategoricalParameter<Material> materialsFur = new CategoricalParameter<Material>();

    int class_value;

    protected override void OnIterationStart()
    {   
        countFrame = 0;
        currentInstanceTree = GameObject.Instantiate(Tree.Sample());
        var d = getTreePosition();
        currentInstanceTree.transform.position = d;
    }
    
    protected override void OnUpdate(){
        if(countFrame == 2){
            currentInstanceSheep = GameObject.Instantiate(Sau.Sample());
            var animator = currentInstanceSheep.GetComponent<Animator>();
            animator.Play(k_StateName + animationClips.Sample().name, 0, Random.Range(0.0f, 1.0f));

        }

        if(countFrame == 4){
            GameObject.Destroy(currentInstanceTree);
            GameObject.Destroy(currentInstanceSheep);
        }

        if(countFrame == 3){

            var taggedObjects = tagManager.Query<CameraTag>();

            foreach (var taggedObject in taggedObjects){

                var camera = taggedObject.GetComponent<Camera>();

                Vector3[] vertices;
                SkinnedMeshRenderer skinnedMeshRenderer = currentInstanceSheep.GetComponent<SkinnedMeshRenderer>();

                var tempMaterials = skinnedMeshRenderer.materials;
                tempMaterials[0] = materials.Sample();
                var fur = materialsFur.Sample();
                tempMaterials[1] = fur;
                skinnedMeshRenderer.materials = tempMaterials;

                Debug.Log(fur.name);

                if(fur.name == "M_SheepFur Brown"){
                    class_value = 5;
                } else if(fur.name == "M_SheepFur Gray"){
                    class_value = 6;
                } else if(fur.name == "M_SheepFur Black"){
                    class_value = 4;
                } else if(fur.name == "M_SheepFur"){
                    class_value = 7;
                } else if (fur.name == "M_SheepFur_Legacy"){
                    class_value = 7;
                }

                Mesh skinnedMesh = new Mesh();
                skinnedMeshRenderer.BakeMesh(skinnedMesh);
                vertices = skinnedMesh.vertices;
                int countHit = 0;
                int loopcount = 0;
                Vector3 p;
                Matrix4x4 localToWorld;
                Vector3 rotation;

                Vector3 maxPosX;
                Vector3 maxPosZ;
                Vector3 minPosX;
                Vector3 minPosZ;
                Vector3 maxPosY;
                Vector3 minPosY;

                while(true){

                    maxPosX = Vector3.one * float.NegativeInfinity;
                    maxPosZ = Vector3.one * float.NegativeInfinity;

                    minPosX = Vector3.one * float.PositiveInfinity;
                    minPosZ = Vector3.one * float.PositiveInfinity;


                    countHit = 0;
                    p = getRandomPosition();
                    int randomNumber = Random.Range(0, 360);
                    rotation = new Vector3(0, randomNumber, 0);
                    localToWorld = currentInstanceSheep.transform.GetChild(1).localToWorldMatrix;
                    Matrix4x4 rotationmatrix = Matrix4x4.Rotate(Quaternion.Euler(new Vector3(0, 0, randomNumber)));
                    localToWorld[0,3] = p.x;
                    localToWorld[1,3] = p.y;
                    localToWorld[2,3] = p.z;
                    localToWorld = localToWorld * rotationmatrix;
                    for(int i = 0; i<vertices.Length; ++i){
                        var point = Quaternion.Euler(90f, 0f, 0f) * vertices[i];
                        point = localToWorld.MultiplyPoint3x4(point);
                        if (Physics.Linecast(camera.transform.position, point, out RaycastHit hitInfo))
                        {
                            countHit = countHit + 1;
                        }
                        if (point.x < minPosX.x) minPosX = point;
                        if (point.z < minPosZ.z) minPosZ = point;
                        if (point.x > maxPosX.x) maxPosX = point;
                        if (point.z > maxPosZ.z) maxPosZ = point;

                    }
                    double sum = (double) countHit / (double) vertices.Length;
                    if((((sum) > 0.8 && (sum) < 0.9))){
                        break;
                    }
                    if(loopcount == 100){
                        System.IO.Directory.CreateDirectory("Generated/BoundingBoxes/");
                        string pathUnvalid = "./Generated/BoundingBoxes/unvalid.txt";
                        using(var sw = new StreamWriter(pathUnvalid, true))
                        {
                            sw.WriteLine("rgb_"+countTotalFrame);
                        }
                        break;
                    }
                    loopcount += 1;
                }

                currentInstanceSheep.transform.position = p;
                currentInstanceSheep.transform.rotation = Quaternion.Euler(rotation);

                var renderer = currentInstanceSheep.GetComponent<Renderer>();
                var bounds = renderer.bounds;

                var imgHight = 640;
                var imgWidth = 640;

                minPosX = camera.WorldToScreenPoint(minPosX);
                maxPosX = camera.WorldToScreenPoint(maxPosX);
                minPosY = camera.WorldToScreenPoint(minPosZ);
                maxPosY = camera.WorldToScreenPoint(maxPosZ);

                var pixelWidth = (maxPosX.x - minPosX.x) / imgWidth;
                var pixelHight = (maxPosY.y - minPosY.y) / imgHight;
                

                var centerX = ((maxPosX.x + minPosX.x) / 2) / imgWidth;
                var centerY = 1 - ((maxPosY.y + minPosY.y) / 2) / imgHight;
                
                System.IO.Directory.CreateDirectory("Generated/BoundingBoxes");

                string path = "./Generated/BoundingBoxes/rgb_"+countTotalFrame+".txt";

                using(var sw = new StreamWriter(path, true))
                {
                    sw.WriteLine(class_value + " " + centerX + " " + centerY + " " + pixelWidth + " " + pixelHight);
                }
            }

            }
            
        countFrame += 1;
        countTotalFrame +=1;
    }

    protected override void OnIterationEnd()
    {

    }

    private Vector3 getRandomPosition()
    {
        // Randomize position on surface
        var p = SurfaceBoundsSheep.Sample();
        Vector3 point;
        point = new Vector3(p.x, 0, p.y);
        return point;
    }

    private Vector3 getTreePosition()
    {
        // Randomize position on surface
        var p = SurfaceBoundsTree.Sample();
        Vector3 point;
        point = new Vector3(p.x, 0, p.y);
        return point;
    }
}
