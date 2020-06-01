using UnityEngine;
using System;

public class IconMaster : MonoBehaviour
{
    public GameObject centralPoint;

    private float farSide = 8.8f;
    //private float closeSide =  0.1633289f;

    private void Update()
    {
        float dist = Vector3.Distance(centralPoint.transform.position, transform.position);
        float scalePercentage = ((farSide - dist) / farSide);
        Debug.Log("distance to farSide = " + (farSide - dist) + " scale % is " + (scalePercentage));
        if (dist > 0.5)
        {
            float newValue = System.Convert.ToSingle(transform.localScale.x - (scalePercentage * 0.0001));
            transform.localScale = new Vector3(newValue, newValue, newValue);
        }
    }

}