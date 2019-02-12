using UnityEngine;
[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class FloatingObjects : MonoBehaviour
{
    
    [Range(5,6)]public float floatingSpeed = 5.5f;

    //Range(0,10)]public int windLevel = 0;

    [Tooltip("以氣象站為中心的方向")]
    [Range(0,360)]public float windDirection = 90;

    public bool isUseWeatherStationData = true;
    [Tooltip("It will auto search the object with (WeatherStaion tag)")]
    public SingletonWeatherStationData m_WeatherData;


    private float radian = 0;//弧度
    private float perRadian = 0.2f;//每次变化的弧度
    private float radius = 0.05f;//半径
    private Vector3 oldPos;//开始时候的坐标
    public bool disabledRot = true;

    public bool floatingUp = true;

    private void Awake()
    {
        if(m_WeatherData == null && isUseWeatherStationData)
        {
            m_WeatherData = GameObject.FindWithTag("WeatherStation").GetComponent<SingletonWeatherStationData>();
        }

    }

    void Start()
    {
        oldPos = transform.position;//将最初的位置保存到oldPos
    }

    void Update()
    {
        if(isUseWeatherStationData)
        {
            GetWeatherStationData();
        }

        if(floatingUp)
        {
            Vector3 movingTowardDir = new Vector3(Mathf.Sin(Mathf.Deg2Rad * windDirection), 0, Mathf.Cos(Mathf.Deg2Rad * windDirection));
            movingTowardDir = movingTowardDir.normalized * floatingSpeed * Time.deltaTime;
            movingTowardDir.y = floatingSpeed * Time.deltaTime;
            this.transform.Translate(movingTowardDir);
        }
        else
        {
            radian += perRadian;//弧度每次加0.03
            float dy = Mathf.Cos(radian) * radius;//dy定义的是针对y轴的变量，也可以使用sin，找到一个合适的值就可以
            transform.position = oldPos + new Vector3(0, dy, 0);
        }


        if (disabledRot)
            transform.localEulerAngles += new Vector3(0, 1f, 0);
    }

    void GetWeatherStationData()
    {
        floatingSpeed = m_WeatherData.AverageWindSpeed;
        windDirection = m_WeatherData.WindDirction;
    }
}