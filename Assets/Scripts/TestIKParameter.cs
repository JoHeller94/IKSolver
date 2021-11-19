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

        float test = Vector2.Angle(P2.transform.position - P1.transform.position, Vector2.right);
        Debug.Log("Offset " + test);

        offsetAngle_a = Vector3.SignedAngle(new Vector3(a.x,0,a.z),a, P1.transform.forward);
        offsetAngle_b = Vector3.SignedAngle(a,b, P1.transform.forward);
        offsetAngle_c = Vector3.SignedAngle(b,c, P1.transform.forward);

        //float angleP1P4 = Vector2.Angle(new Vector2(P4.transform.position.x - P1.transform.position.x, P4.transform.position.y - P1.transform.position.y), Vector2.left);
        //Debug.Log(angleP1P4);

        //float p1p4 = Vector2.Distance(new Vector2(P1.transform.position.x, P1.transform.position.y), new Vector2(P4.transform.position.x, P4.transform.position.y));
        //float p1p2 = Vector2.Distance(new Vector2(P1.transform.position.x, P1.transform.position.y), new Vector2(P2.transform.position.x, P2.transform.position.y));
        //float Psi = Mathf.Sin(p1p2 / p1p4);
        //Debug.Log("Angle" + (angleP1P4 - sinPsi));

        Debug.Log("Angle A = " + offsetAngle_a);
        Debug.Log("Angle B = " + offsetAngle_b);
        Debug.Log("Angle C = " + offsetAngle_c);



        ik.AddLink(a.magnitude, offsetAngle_a, Color.cyan);
        ik.AddLink(b.magnitude, offsetAngle_b, Color.magenta);
        ik.AddLink(c.magnitude, offsetAngle_c, Color.green);


        ik.SetGlobalMinAngle(0f * Mathf.Deg2Rad);
        ik.SetGlobalMaxAngle(89.2f * Mathf.Deg2Rad);

        
    }

    private void Update()
    {
        

        //ik.BowIKConcatVec2(angle);
        //float targetDistance, float angle, float minAngle, float maxAngle, float threshold, int i
        float dist = Vector3.Distance(P1.transform.position, target.transform.position);
        if (ik.ComputeAngle(dist, 1.0f * Mathf.Deg2Rad, 10))
        {
            float beta = ik.GetAngleBetweenLinks();
            float alpha = ik.GetFirstAngle();
            //Debug.Log("beta = " + beta * Mathf.Rad2Deg);
            //Debug.Log("alpha = " + alpha);

            targetLocal = P0.transform.worldToLocalMatrix * target.transform.position;
            p1Local = GetPosition(P0.transform.worldToLocalMatrix * GetLocalMat(P1));
            

            //Vector2 d2 = new Vector2(target.transform.position.x, target.transform.position.y) - new Vector2(P1.transform.position.x, P1.transform.position.y);
            //Vector3 d3 = target.transform.position - P1.transform.position;

            //float gamma_2D = Vector2.SignedAngle(d2, new Vector2(-1.0f, 0.0f));
            //Vector3 rotAxis = Vector3.Cross(d3, new Vector3(d3.x, 0.0f, d3.z));
            //float gamma_3D = Vector3.SignedAngle(d3, new Vector3(d3.x, 0.0f, d3.z), rotAxis);

            //Debug.Log("Gamma2D: " + gamma_2D + " = Gamma3D: " + gamma_3D);

            Vector2 d1 = new Vector2(targetLocal.x, targetLocal.y) - new Vector2(p1Local.x, p1Local.y);
            Vector2 d2 = new Vector2(1,0) - new Vector2(p1Local.x, p1Local.y);

            float gamma = Vector2.SignedAngle(d1, d2);

            //Debug.Log("Gamma: " + gamma  + " Alpha: " + -alpha + " Combined: " + (-alpha-gamma));

            P1.transform.localRotation = Quaternion.Euler(P1.transform.localRotation.x, P1.transform.localRotation.y, (alpha - gamma));
            P2.transform.localRotation = Quaternion.Euler(P2.transform.localRotation.x, P2.transform.localRotation.y, (- beta * Mathf.Rad2Deg));
            P3.transform.localRotation = Quaternion.Euler(P3.transform.localRotation.x, P3.transform.localRotation.y, (- beta * Mathf.Rad2Deg));
            P4.transform.localRotation = Quaternion.Euler(P4.transform.localRotation.x, P4.transform.localRotation.y, AdjustNozzle(P3.transform.position,P4.transform.position, inputAngle));

        }
        else
        {
            Debug.Log("could not compute solution");
        }

        RotationOnY();

    }

    float AdjustNozzle(Vector3 nozzleTransform, Vector3 parentTransform, float additionalAngle=0)
    {
        Vector3 nozzMinusParent = nozzleTransform - parentTransform;
        float angleUpDown = Vector2.Angle(new Vector2(nozzMinusParent.x, nozzMinusParent.y), new Vector2(0, 1));
        float angeleSide = Vector2.Angle(new Vector2(nozzMinusParent.x, nozzMinusParent.y), new Vector2(1, 0));

        if (angeleSide > 90f)
        {
            return -angleUpDown + additionalAngle;
        }

        else
        {
            return angleUpDown + additionalAngle;
        }

    }

    void RotationOnY()
    {      
        float plainAngle = Vector3.SignedAngle(new Vector3(target.transform.position.x, 0 ,target.transform.position.z) - P0.transform.position, Vector3.right,Vector3.up);       
        P0.transform.localRotation = Quaternion.Euler(0, -plainAngle, 0);
    }

    Matrix4x4 GetLocalMat(GameObject go)
    {
        if (go.transform.parent == null)
        {
            Debug.LogError("NO PARENT");
            return new Matrix4x4();
        }

        return go.transform.parent.transform.localToWorldMatrix.inverse * go.transform.localToWorldMatrix;
    }


    public Vector3 GetPosition(Matrix4x4 matrix)
    {
        var x = matrix.m03;
        var y = matrix.m13;
        var z = matrix.m23;

        return new Vector3(x, y, z);
    }
}
 