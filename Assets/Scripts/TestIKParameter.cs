using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestIKParameter : MonoBehaviour
{
    public GameObject P0;
    public GameObject P1;
    public GameObject P2;
    public GameObject P3;
    public GameObject P4;
    public GameObject target;


    private Vector2 d;

    private float angle;

    private float minAngle;
    private float maxAngle;

    private Vector3 targetLocal;
    private Vector3 p1Local;

    public float inputAngle;

    float offsetAngle_a = 0.0f;
    float offsetAngle_b = 0.0f;
    float offsetAngle_c = 0.0f;

    BowIK ik = new BowIK();

    private void Start()
    {
        Vector3 a = P2.transform.position - P1.transform.position;
        Vector3 b = P3.transform.position - P2.transform.position;
        Vector3 c = P4.transform.position - P3.transform.position;

        d = new Vector2(P4.transform.position.x, P4.transform.position.y) - new Vector2(P1.transform.position.x, P1.transform.position.y);

        offsetAngle_a = Vector3.SignedAngle(new Vector3(a.x, 0, a.z), a, P1.transform.forward);
        offsetAngle_b = Vector3.SignedAngle(a, b, P1.transform.forward);
        offsetAngle_c = Vector3.SignedAngle(b, c, P1.transform.forward);


        ik.AddLink(a.magnitude, offsetAngle_a, Color.cyan);
        ik.AddLink(b.magnitude, offsetAngle_b, Color.magenta);
        ik.AddLink(c.magnitude, offsetAngle_c, Color.green);


        ik.SetGlobalMinAngle(0f * Mathf.Deg2Rad);
        ik.SetGlobalMaxAngle(89.2f * Mathf.Deg2Rad);


    }

    private void Update()
    {

        float dist = Vector3.Distance(P1.transform.position, target.transform.position);
        if (ik.ComputeAngle(dist, 1.0f * Mathf.Deg2Rad, 10))
        {
            float beta = ik.GetAngleBetweenLinks();
            float alpha = ik.GetFirstAngle();

            targetLocal = P0.transform.worldToLocalMatrix * target.transform.position;
            p1Local = P0.transform.worldToLocalMatrix * P1.transform.position;

            Vector2 d1 = new Vector2(targetLocal.x, targetLocal.y) - new Vector2(p1Local.x, p1Local.y);
            Vector2 d2 = new Vector2(1, 0) - new Vector2(p1Local.x, p1Local.y);

            float gamma = Vector2.SignedAngle(d1, d2);



            P1.transform.localRotation = Quaternion.Euler(P1.transform.localRotation.x, P1.transform.localRotation.y, alpha - gamma);
            P2.transform.localRotation = Quaternion.Euler(P2.transform.localRotation.x, P2.transform.localRotation.y, -beta);
            P3.transform.localRotation = Quaternion.Euler(P3.transform.localRotation.x, P3.transform.localRotation.y, -beta);
            P4.transform.localRotation = Quaternion.Euler(P4.transform.localRotation.x, P4.transform.localRotation.y, (-P4.transform.parent.rotation.ToEuler().z * Mathf.Rad2Deg) - inputAngle);

        }
        else
        {
            Debug.Log("no solution: RESET TARGET TO LAST VALID LOCATION");
        }

        swingArm();

    }


    void swingArm()
    {
        float planarAngle = Vector3.SignedAngle(new Vector3(target.transform.position.x, 0.0f, target.transform.position.z) - P0.transform.position, P0.transform.parent.right, P0.transform.parent.up);
        P0.transform.localRotation = Quaternion.Euler(0, -planarAngle, 0);
    }
}