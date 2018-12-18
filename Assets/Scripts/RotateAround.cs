using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour {
    public float speed = 1;
    public GameObject focusObject;
    public float radius = 10;

    private void FixedUpdate()
    {
        transform.RotateAround(focusObject.transform.position,new Vector3(0,1,0),Time.deltaTime * speed);
        //transform.LookAt(focusObject.transform);
        transform.Translate(0, 0, (transform.position - focusObject.transform.position).magnitude - radius);
    }
}
