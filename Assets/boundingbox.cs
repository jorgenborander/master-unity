using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

public class boundingbox : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        var camera = Camera.main;

        SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        Mesh skinnedMesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(skinnedMesh);
        var vertices = skinnedMesh.vertices;
        Debug.Log(vertices);

        var localToWorld = transform.GetChild(1).localToWorldMatrix;

        Vector3 maxPosX = Vector3.one * float.NegativeInfinity;
        Vector3 maxPosZ = Vector3.one * float.NegativeInfinity;

        Vector3 minPosX = Vector3.one * float.PositiveInfinity;
        Vector3 minPosZ = Vector3.one * float.PositiveInfinity;
        for(int i = 0; i<vertices.Length; i+=2){
            var point = Quaternion.Euler(90f, 0f, 0f) * vertices[i];
            point = localToWorld.MultiplyPoint3x4(point);

            if (Physics.Linecast(camera.transform.position, point, out RaycastHit hitInfo))
            {
                Debug.DrawLine(camera.transform.position, hitInfo.point);
            } else {
                Debug.DrawLine(camera.transform.position, point);
            }
        }

        //var centerBounds = GetComponent<Camera>().WorldToScreenPoint(bounds.center);

        //Vector3 minPos = GetComponent<Camera>().WorldToScreenPoint(yxMin);
        //Vector3 maxPos = GetComponent<Camera>().WorldToScreenPoint(yxMax);
        
    }
}
