using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFlowField : MonoBehaviour {
    FastNoise _fastNoise;
    public Vector3Int _gridSize;
    public float _increment;
    public Vector3 _offset, _offsetSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void CalculateFlowFieldDirection()
    {
        
    }
    private void OnDrawGizmos()
    {
        _fastNoise = new FastNoise();

        float xOff = 0;
        for (int x = 0; x < _gridSize.x;x++)
        {
            float yOff = 0;
            for (int y = 0;y < _gridSize.y;y++)
            {
                float zOff = 0;
                for (int z = 0; z < _gridSize.z; z++)
                {
                    float noise = _fastNoise.GetSimplex(xOff + _offset.x, yOff + _offset.y, zOff + _offset.z) + 1;//return value 0 - 2
                    Vector3 noiseDirection = new Vector3(Mathf.Cos(noise * Mathf.PI), 
                                                         Mathf.Sin(noise * Mathf.PI), 
                                                         Mathf.Cos(noise * Mathf.PI));
                    Gizmos.color = new Color(noiseDirection.normalized.x, 
                                             noiseDirection.normalized.y, 
                                             1 - noiseDirection.normalized.z,1f);
                    Vector3 pos = new Vector3(x, y, z) + transform.position;
                    Vector3 endPos = pos + Vector3.Normalize(noiseDirection);
                    Gizmos.DrawLine(pos, endPos);
                    Gizmos.DrawSphere(endPos, 0.1f);
                    zOff += _increment;
                }
                yOff += _increment;
            }
            xOff += _increment;
        }
    }
}
