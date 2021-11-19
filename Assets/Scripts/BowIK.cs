using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowIK : MonoBehaviour
{
    public static Vector2 Rotate2D(Vector2 v, float angle)
    {
        return new Vector2(v.x * Mathf.Cos(angle) - v.y * Mathf.Sin(angle), v.x * Mathf.Sin(angle) + v.y * Mathf.Cos(angle));
    }

    public struct Link
    {
        public float length;
        public float angleOffset;
        public Color c; //delete
    }
    
    public List<Link> links = new List<Link>();
      
    public void AddLink(float len, float offset_angle, Color c) //delete Color
    {
        Link tmp;
        tmp.length = len;
        tmp.angleOffset = offset_angle * Mathf.Deg2Rad;
        tmp.c = c; //delete

        links.Add(tmp);
    }

    private float firstAngle;
    private float angleBetweenLinks = 0f;
    private float globalMinAngle = 0.0f;
    private float globalMaxAngle = Mathf.PI;
    


    private Vector2 ConcatVec2(float angle)
    {
        Vector2 startToEnd = new Vector2(0, 0);
        Vector2 lastStartToEnd = new Vector2(0, 0);//delete
        float sumAngles = 0f;

        foreach(Link l in links)
        {
           
            Vector2 tmp = new Vector2(l.length, 0.0f);
            sumAngles -= l.angleOffset;
            startToEnd += Rotate2D(tmp, -sumAngles);
            Debug.DrawLine(lastStartToEnd, startToEnd, l.c, 0.2f); //delete
            sumAngles += angle;
            
            lastStartToEnd = startToEnd; //delete
        }
        Debug.DrawLine(new Vector2(0f,0f), startToEnd, Color.yellow, 0.2f); //delete
        return startToEnd;
    }

    public bool ComputeAngle(float distance, float threshold, int iterations)
    {
        return  BinarySearchforAngle(distance, globalMinAngle, globalMaxAngle, threshold, iterations, angleBetweenLinks);
    } 

    public void SetGlobalMinAngle(float angle)
    {
        globalMinAngle = angle;
    }    
    public void SetGlobalMaxAngle(float angle)
    {
        globalMaxAngle = angle;
    }

    public float GetFirstAngle()
    {    
        return firstAngle - links[0].angleOffset * Mathf.Rad2Deg;
    }

    public float GetAngleBetweenLinks()
    {      
        return angleBetweenLinks * Mathf.Rad2Deg;
    }

    private  bool BinarySearchforAngle(float targetDistance, float minAngle, float maxAngle, float threshold, int i, float angle)
    {
        if (i == 0)
        {
            return false;
        }

        Vector2 startToEnd = ConcatVec2(angle);
        float error = startToEnd.magnitude - targetDistance;

        if (error > threshold * -0.5f && error < threshold * 0.5f)
        {
            angleBetweenLinks = angle;
            firstAngle = Vector2.Angle(startToEnd.normalized, Rotate2D(new Vector2(1f, 0f), links[0].angleOffset));
            return true;
        }

        else if (error > threshold * 0.5f) //Angles too small
        {
           minAngle = angle;
           angle +=  (maxAngle -angle) * 0.5f;
          
           return BinarySearchforAngle(targetDistance, minAngle, maxAngle, threshold, --i, angle);
        }

        else if (error < threshold * -0.5f) //Angles too large
        {
            maxAngle = angle​;
            angle -= (angle - minAngle) * 0.5f;
           
            return BinarySearchforAngle(targetDistance, minAngle, maxAngle, threshold, --i, angle);
        }

        else return false;
       
    }
}
