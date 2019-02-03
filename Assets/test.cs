using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeatherStation;
using Kvant;
public class test : MonoBehaviour {
    public JObject jObject ;
    public GameObject GrassInstance;

    Grass grass;
    public bool isUseWeatherStaion = true;
	// Use this for initialization
	void Start () {
        jObject = null;
        grass = GrassInstance.GetComponent<Grass>();
        //grass.rotationNoiseAxis = new Vector3(1, 0, 0);
        //grass.rotationNoiseSpeed = 1;

        //LoadData.ReadFileFromInternet(jObject);
        //StartCoroutine(_ReadFileFromInternet(jObject));


        if(isUseWeatherStaion)
        {
            StartCoroutine(LoadData.ReadFileFromInternet());
            StartCoroutine(DataManager());
        }   
        else{
            StartCoroutine(DataManagerWithNoWeatherStation());
        }

	}

    IEnumerator DataManager()
    {
        jObject = LoadData.GameData;
        Debug.Log(jObject);
        if (jObject == null)
        {
            Debug.Log("jObject == null");
            yield return new WaitForSeconds(15f);
        }
        Debug.Log(" jObject.Feeds.Count " + (jObject.Feeds.Count));
        while (true)
        {
            jObject = LoadData.GameData;

            if(jObject.Feeds.Count >= 1 )
            {
                //grass.rotationNoiseAxis = new Vector3(
                    //Mathf.Cos(jObject.Feeds[jObject.Feeds.Count - 1].field1 * 3.141f/360),
                    //0,
                    //Mathf.Sin(jObject.Feeds[jObject.Feeds.Count - 1].field1 * 3.141f/360));
                grass.rotationNoiseAxis = new Vector3(
                    Mathf.Sin(jObject.Feeds[jObject.Feeds.Count - 1].field1 * Mathf.Deg2Rad),
                              0,
                    Mathf.Cos(jObject.Feeds[jObject.Feeds.Count - 1].field1 * Mathf.Deg2Rad)
                );
                
                grass.rotationNoiseSpeed = 1;
                Debug.Log("Axis : " + jObject.Feeds[jObject.Feeds.Count - 1].field1);
                Debug.Log("Rotation Noise Axis : " + grass.rotationNoiseAxis);

            }

            yield return new WaitForSeconds(15f);
        }
    }
    IEnumerator DataManagerWithNoWeatherStation()
    {
        while (true)
        {
            grass.rotationNoiseAxis = new Vector3(
                Random.Range(0, 5),
                0,
                Random.Range(0, 5));
            grass.rotationNoiseSpeed = Random.Range(1,5);
            yield return new WaitForSeconds(15f);
        }
    }
}
