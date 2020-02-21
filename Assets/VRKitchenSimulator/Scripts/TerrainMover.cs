using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMover : MonoBehaviour
{
    Vector3 vel;
    float speed = 30;
    Vector3 originalPosition;
    void Start()
    {
        originalPosition = transform.position;
        vel = new Vector3(0, 0, -speed);
    }
    void Update()
    {
        transform.position += (vel * Time.deltaTime);
        if (Mathf.Abs(transform.position.z) >= 450)
        {
            transform.position = originalPosition;
            transform.localEulerAngles = new Vector3(0, -transform.localEulerAngles.y, 0);
        }
    }
}
