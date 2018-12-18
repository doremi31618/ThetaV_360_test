using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Instantiate512 cubes with spectrumdata.
/// </summary>
public class Instantiate512CubesWithSpectrumdata : MonoBehaviour {
    public GameObject _CubePrefabs;
    GameObject[] _sampleCubes = new GameObject[512];
    public float MaxScale;
	// Use this for initialization
	void Start () {
        for (int i = 0; i < 512;i++)
        {
            GameObject newSampleCubes = Instantiate(_CubePrefabs) as GameObject;
            newSampleCubes.transform.position = this.transform.position;
            newSampleCubes.transform.parent = this.transform;
            newSampleCubes.name = "newSampleCubes" + i;
            this.transform.eulerAngles = new Vector3(0, -0.703125f * i, 0);//360 / 521 = 0.703125
            newSampleCubes.transform.position = Vector3.forward * 100;
            _sampleCubes[i] = newSampleCubes;
        }
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < 512; i++)
        {
            if(_sampleCubes[i] != null)
            {
                _sampleCubes[i].transform.localScale = new Vector3(10, AudioReader._AudioSamples[i] * MaxScale, 10);

            }
        }

	}
}
