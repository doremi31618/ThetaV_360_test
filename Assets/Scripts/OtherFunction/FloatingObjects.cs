using UnityEngine;
public class FloatingObjects : MonoBehaviour
{

    private float radian = 0;//弧度
    private float perRadian = 0.2f;//每次变化的弧度
    private float radius = 0.05f;//半径
    private Vector3 oldPos;//开始时候的坐标
    public bool disabledRot;

    void Start()
    {
        oldPos = transform.position;//将最初的位置保存到oldPos
    }

    void Update()
    {
        radian += perRadian;//弧度每次加0.03
        float dy = Mathf.Cos(radian) * radius;//dy定义的是针对y轴的变量，也可以使用sin，找到一个合适的值就可以
        transform.position = oldPos + new Vector3(0, dy, 0);
        if (disabledRot == false)
            transform.localEulerAngles += new Vector3(0, 1f, 0);
    }

}