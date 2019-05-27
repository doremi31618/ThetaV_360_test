using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebCamTest : MonoBehaviour
{
   // public int numberOfCamera = 0;
    // Use this for initialization
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        Debug.Log("Number of web cams connencted: " + devices.Length);
        Renderer renderer = this.GetComponent<Renderer>();
        WebCamTexture myCam = new WebCamTexture();
        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log(devices[i].name);
            if(devices[i].name == "ManyCam Virtual Webcam")
            {
                string camName = devices[i].name;
                Debug.Log("The WebCam name is" + camName);
                myCam.deviceName = camName;
                renderer.material.mainTexture = myCam;
                myCam.Play();
                break;
            }
        }
        /*
        string camName = devices[numberOfCamera].name;
        Debug.Log("The WebCam name is" + camName);
        myCam.deviceName = camName;
        renderer.material.mainTexture = myCam;
        myCam.Play();
        */
    }
}
