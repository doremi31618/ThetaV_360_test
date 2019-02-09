using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeatherStation;
using Kvant;

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
    [Header("風力等級 (Beaufort scale) : 0 ~ 12")]
    Beaufort_scale windLevel = Beaufort_scale.calm;
    public enum Beaufort_scale
    {
        /// <summary>
        /// level 0
        /// 煙直上
        /// </summary>
        calm = 0,

        /// <summary>
        /// level 1 
        /// wind speed :0.3-1.5
        /// 僅煙能表示風向，但不能轉動風標。
        /// </summary>
        light_air,

        /// <summary>
        /// level 2 
        /// wind speed : 1.6-3.3
        /// 人面感覺有風，樹葉搖動，普通之風標轉動。
        /// </summary>
        light_breeze,

        /// <summary>
        /// level 3
        /// wind speed : 3.4-5.4
        /// 樹葉及小枝搖動不息，旌旗飄展。
        /// </summary>
        gentle_breeze,

        /// <summary>
        /// level 4 
        /// wind speed : 5.5-7.9
        /// 塵土及碎紙被風吹揚，樹之分枝搖動。
        /// </summary>
        moderate_breeze,

        /// <summary>
        /// level 5
        /// wind speed : 8.0-10.7
        /// 有葉之小樹開始搖擺。
        /// </summary>
        fresh_breeze,

        /// <summary>
        /// level 6
        /// wind speed : 10.8-13.8
        /// 樹之木枝搖動，電線發出呼呼嘯聲，張傘困難。
        /// </summary>
        strong_breeze,

        /// <summary>
        /// level 7
        /// wind speed : 13.9-17.1
        /// 全樹搖動，逆風行走感困難。
        /// </summary>
        near_gale,

        /// <summary>
        /// level 8
        /// wind speed : 17.2-20.7
        // 小樹枝被吹折，步行不能前進。
        /// </summary>
        gale,

        /// <summary>
        /// level 9
        /// wind speed : 20.8-24.4
        /// 建築物有損壞，煙囪被吹倒。
        /// </summary>
        strong_gale,

        /// <summary>
        /// level 10
        /// wind speed : 24.5-28.4
        /// 樹被風拔起，建築物有相當破壞。
        /// </summary>
        storm,

        /// <summary>
        /// level 11
        /// wind speed : 28.5-32.6
        /// 極少見，如出現必有重大災害。
        /// </summary>
        violent_storm,

        /// <summary>
        /// level 12
        /// wind speed : 32.7-36.9
        /// </summary>
        hurricane

    }

    [Range(0, 360)] 
    public int WindDirction = 0;
    [Range(0, 50)]
    public float AverageWindSpeed = 0;
    [Range(0, 50)]
    public float MaxWindSpeed = 0;

    [Tooltip("如果使用氣象站資料作為輸入，將不採用上面資料")]
    public bool isUseWeatherStaion = true;

    public float readSpeed = 15.0f;
    /*
    public float OneHourRainFall;

    public float OneDayRainFall;

    public float Temperature;

    public int Humidity;

    public float BarometricPressure;
    */

    public JObject jObject;
    public GameObject GrassInstance;

    Grass grass;
    UniStormSystem unistorm;

    // Use this for initialization
    void Start()
    {
        jObject = null;
        grass = GrassInstance.GetComponent<Grass>();
        unistorm = GameObject.FindWithTag("UniStorm").GetComponent<UniStormSystem>();

        //grass.rotationNoiseAxis = new Vector3(1, 0, 0);
        //grass.rotationNoiseSpeed = 1;

        //LoadData.ReadFileFromInternet(jObject);
        //StartCoroutine(_ReadFileFromInternet(jObject));


        if (isUseWeatherStaion)
        {
            StartCoroutine(LoadData.ReadFileFromInternet());
            StartCoroutine(DataManager());
        }
        else
        {
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
            yield return new WaitForSeconds(readSpeed);
        }
        Debug.Log(" jObject.Feeds.Count " + (jObject.Feeds.Count));
        while (true)
        {
            jObject = LoadData.GameData;

            if (jObject.Feeds.Count >= 1)
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

            yield return new WaitForSeconds(readSpeed);
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
            grass.rotationNoiseSpeed = Random.Range(1, 5);
            yield return new WaitForSeconds(15f);
        }
    }

}
