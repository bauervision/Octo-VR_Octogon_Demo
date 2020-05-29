using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carousel : MonoBehaviour
{
    public float rotationSpeed = 20f;

    private void OnMouseDrag()
    {
        float rotY = Input.GetAxis("Mouse Y") * rotationSpeed * Mathf.Deg2Rad;
        transform.Rotate(-Vector3.up, rotY);

    }
}
