using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonWeatherStationData : MonoBehaviour {
    
    private static SingletonWeatherStationData singleton ;
    public SingletonWeatherStationData Instance
    {
        get{
            if (singleton == null)
                singleton = this;
            return singleton;
        }
    }
    [Range(0, 360)] 
    public int WindDirction = 0;
    [Range(0, 50)]
    public float AverageWindSpeed = 0;
    [Range(0, 50)]
    public float MaxWindSpeed = 0;

    public float OneHourRainFall;

    public float OneDayRainFall;

    public float Temperature;

    public int Humidity;

    public float BarometricPressure;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
