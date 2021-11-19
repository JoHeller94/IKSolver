using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugRays : MonoBehaviour
{
    public GameObject P1;
    public GameObject P2;
    public GameObject P3;
    public GameObject P4;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(P1.transform.position, P2.transform.position - P1.transform.position, Color.red, 0.5f);
        Debug.DrawRay(P2.transform.position, P3.transform.position - P2.transform.position, Color.red, 0.5f);
        Debug.DrawRay(P3.transform.position, P4.transform.position - P3.transform.position, Color.red, 0.5f);
        Debug.DrawRay(P1.transform.position, P4.transform.position - P1.transform.position, Color.green, 0.5f);
      
    }
}
