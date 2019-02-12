using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeatherStation;
using Kvant;

public class SingletonWeatherStationData : MonoBehaviour
{
    #region attribute


    [Header("環境風參數")]

    [Range(0, 360)]
    public int WindDirction = 0;
    [Range(0, 50)]
    public float AverageWindSpeed = 0;
    [Range(0, 50)]
    public float MaxWindSpeed = 0;

    private float currentWindSpeed = 0;

    public Beaufort_scale windLevel = Beaufort_scale.calm;

    /// <summary>
    /// 風力等級 (Beaufort scale) : 0 ~ 12
    /// </summary>
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
        /// 小樹枝被吹折，步行不能前進。
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


    [Header("氣象站參數")]

    [Tooltip("如果使用氣象站資料作為輸入，將不採用上面資料")]
    public bool isUseWeatherStaion = true;

    [Tooltip("多久讀取一次氣象站資料")]
    public float readSpeed = 15.0f;

    [Header("遊戲物件參數")]
    public GameObject Balloon;
    public Grass grass;
    public UniStormSystem unistorm;
    public WindZone treeWindZone;
    /*
    public float OneHourRainFall;

    public float OneDayRainFall;

    public float Temperature;

    public int Humidity;

    public float BarometricPressure;
    */

    public JObject jObject;

    private static SingletonWeatherStationData singleton;
    public SingletonWeatherStationData Instance
    {
        get
        {
            if (singleton == null)
                singleton = this;
            return singleton;
        }
    }



    #endregion
    #region Unity funtion

    private void Awake()
    {
        
    }

    // Use this for initialization
    void Start()
    {
        //Initialize stage
        jObject = null;
        if(grass == null)
            grass = GameObject.Find("GrassShader").GetComponent<Grass>();

        if(unistorm == null)
            unistorm = GameObject.FindWithTag("UniStorm").GetComponent<UniStormSystem>();
        unistorm.isDontChangeWindZonInUnistorm = true;
        unistorm.UniStormWindZone = treeWindZone;
        if(Balloon == null)
        {
            Balloon = new GameObject();
            Balloon.AddComponent<FloatingObjects>();
        }



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

    private void Update()
    {

    }

    #endregion
    #region IEnumerator
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
                WindDataTransfer(jObject.Feeds[jObject.Feeds.Count - 1].field2, 
                                 jObject.Feeds[jObject.Feeds.Count - 1].field3, 
                                 jObject.Feeds[jObject.Feeds.Count - 1].field1);
                GrassAttributeByWind(WindDirction);

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
            //float degree = Mathf.PerlinNoise(Time.time, 0) * 360;
            WindDataTransfer(AverageWindSpeed, MaxWindSpeed, WindDirction);
            GrassAttributeByWind(WindDirction);


            yield return new WaitForSeconds(1f);
        }
    }
#endregion
    void GrassAttributeByWind(float _WindDirection)
    {
        grass.rotationNoiseAxis = new Vector3(
            Mathf.Sin(_WindDirection * Mathf.Deg2Rad),0,
            Mathf.Cos(_WindDirection * Mathf.Deg2Rad));
        unistorm.UniStormWindZone.transform.eulerAngles = new Vector3(0,_WindDirection,0);
        grass.rotationNoiseSpeed = WindSpeedToRotationNoiseSpeed(windLevel);
        AdjustRandomPitchAndNoiseToRandomPitch(windLevel);

    }


    void AdjustRandomPitchAndNoiseToRandomPitch(Beaufort_scale _windLevel)
    {
        float _randomPitchAngle = 0;
        float _noisePitchAngle = 0;

        if ((int)_windLevel <= 4)
        {
            _randomPitchAngle = Mathf.SmoothStep(0, 6f, ((float)_windLevel) / 4f);
            _noisePitchAngle = Mathf.SmoothStep(0.5f, 4.6f, ((float)_windLevel) / 4f);

        }
        else if (4 < (int)_windLevel && (int)_windLevel <= 6)
        {
            _randomPitchAngle = Mathf.SmoothStep(6f, 20f, ((float)_windLevel - 4) / 2f);
            _noisePitchAngle = Mathf.SmoothStep(4.6f, 30f, ((float)_windLevel- 4) / 2f);
        }
        else if (6 < (int)_windLevel && (int)_windLevel <= 9)
        {
            _randomPitchAngle = Mathf.SmoothStep(20, 66.2f, ((float)_windLevel - 6) / 3f);
            _noisePitchAngle = Mathf.SmoothStep(30, 45, ((float)_windLevel - 6) / 3f);
        }
        else if (9 < (int)_windLevel)
        {
            _randomPitchAngle = Mathf.SmoothStep(66.2f, 90, ((float)_windLevel - 9) / 3f);
            _noisePitchAngle =Mathf.SmoothStep(45,60, ((float)_windLevel- 9) / 3f);

        }
        unistorm.UniStormWindZone.windMain = (int)_windLevel / 2;
        grass.randomPitchAngle = _randomPitchAngle;
        grass.noisePitchAngle = _noisePitchAngle;
    }
    float WindSpeedToRotationNoiseSpeed(Beaufort_scale _windLevel)
    {
        float rotationNoiseSpeed = 0;
        if((int)_windLevel <= 4)
        {
            rotationNoiseSpeed = Mathf.SmoothStep(1, 2, ((float)_windLevel)/4f);
        }
        else if(4<(int)_windLevel && (int) _windLevel <= 9 )
        {
            rotationNoiseSpeed = Mathf.SmoothStep(2, 3, ((float)_windLevel - 4)/5f);
        }
        else if(9 < (int)_windLevel)
        {
            rotationNoiseSpeed = Mathf.SmoothStep(3, 5, ((float)_windLevel - 9)/3f);
        }
        return rotationNoiseSpeed;
    }

    void WindDataTransfer(float _AverageWindSpeed,float _MaxWindSpeed ,float _WindDirection)
    {
        AverageWindSpeed = _AverageWindSpeed;
        MaxWindSpeed = _MaxWindSpeed;
        WindDirction = (int)Mathf.Floor(_WindDirection);
        currentWindSpeed = Mathf.PerlinNoise(Time.deltaTime, 0) * (MaxWindSpeed - AverageWindSpeed) + AverageWindSpeed;
        windLevelIdentifier(AverageWindSpeed);
    }

    void windLevelIdentifier(float _windSpeed)
    {
        AverageWindSpeed = _windSpeed;
        if(AverageWindSpeed <= 0.3f)
        {
            windLevel = Beaufort_scale.calm;
        }
        else if(0.3f < AverageWindSpeed && AverageWindSpeed<= 1.5f)
        {
            windLevel = Beaufort_scale.light_air;
        }
        else if(1.5f < AverageWindSpeed && AverageWindSpeed <= 3.3f)
        {
            windLevel = Beaufort_scale.light_breeze;
        }
        else if(3.3f < AverageWindSpeed && AverageWindSpeed <= 5.4f)
        {
            windLevel = Beaufort_scale.gentle_breeze;
        }
        else if(5.4f < AverageWindSpeed && AverageWindSpeed <= 7.9f)
        {
            windLevel = Beaufort_scale.moderate_breeze;
        }
        else if(7.9f < AverageWindSpeed && AverageWindSpeed <=  10.7f)
        {
            windLevel = Beaufort_scale.fresh_breeze;
        }
        else if(10.7f < AverageWindSpeed && AverageWindSpeed <= 13.8f)
        {
            windLevel = Beaufort_scale.strong_breeze;
        }
        else if(13.8f < AverageWindSpeed && AverageWindSpeed <= 17.1f)
        {
            windLevel = Beaufort_scale.near_gale;
        }
        else if(17.1f < AverageWindSpeed && AverageWindSpeed <= 20.7f)
        {
            windLevel = Beaufort_scale.gale;
        }
        else if(20.7f < AverageWindSpeed && AverageWindSpeed <= 24.4f)
        {
            windLevel = Beaufort_scale.strong_gale;
        }
        else if(24.4f < AverageWindSpeed && AverageWindSpeed <= 28.4f)
        {
            windLevel = Beaufort_scale.storm;
        }
        else if(28.4f < AverageWindSpeed && AverageWindSpeed <= 32.6f)
        {
            windLevel = Beaufort_scale.violent_storm;
        }
        else
        {
            windLevel = Beaufort_scale.hurricane;
        }
    }


}
