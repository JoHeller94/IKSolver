using SplineMesh;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject TestCube;

    public Vector3 pos;
    public GameObject rope;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pos = TestCube.transform.position;
        GetNodePosition();
    }

    void GetNodePosition()
    {
        rope.GetComponent<Spline>().nodes[2].Position = pos;
    }
}
