using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfRotate : MonoBehaviour {
    public float speed;
	
    private void FixedUpdate()
    {
        transform.Rotate(0,1 * speed * Time.deltaTime,0); 

    }
}
