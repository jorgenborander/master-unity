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
        var p = new Vector3(2, 11, 0);
        var rotate = new Vector3(0, 0, 270);

        Matrix4x4 rotationmatrix = Matrix4x4.Rotate(Quaternion.Euler(rotate));

        Matrix4x4 localToWorld = transform.GetChild(1).localToWorldMatrix;

        localToWorld[0,3] = p.x;
        localToWorld[1,3] = p.y;
        localToWorld[2,3] = p.z;

        localToWorld = localToWorld * rotationmatrix;

        for(int i = 0; i<vertices.Length; ++i){
            var point = localToWorld.MultiplyPoint3x4(vertices[i]);
            Debug.DrawLine(camera.transform.position, point);
        }

        transform.position = p;
        transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
    }
}
