using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matrix : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Update(){
        
        int count = 0;
        Camera camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        var mesh = GetComponentInChildren<MeshCollider>().sharedMesh;
        var vertices = mesh.vertices;
        RaycastHit hit;


        Matrix4x4 localToWorld = transform.GetChild(1).localToWorldMatrix;
        
        for(int i = 0; i<vertices.Length; ++i){
            var point = localToWorld.MultiplyPoint3x4(vertices[i]);
            Debug.DrawLine(camera.transform.position, point);
        }
    }
}
