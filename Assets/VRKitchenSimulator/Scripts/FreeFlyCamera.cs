using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeFlyCamera : MonoBehaviour
{
    [Range(0.5f, 4f)] [SerializeField] private float movementSpeed = 0.1f;
    private void Start()
    {
        Cursor.visible = false;
    }
    void Update()
    {
        movementSpeed = Mathf.Max(movementSpeed += Input.GetAxis("Mouse ScrollWheel"), 0.0f);
        transform.position += (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical")) * movementSpeed;
        transform.eulerAngles += new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
    }
}