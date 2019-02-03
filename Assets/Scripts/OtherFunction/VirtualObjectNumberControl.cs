//using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class listLayerObject
{
    public string name;
    public MeshRenderer[] objects;
}

public class VirtualObjectNumberControl : MonoBehaviour {

    [Tooltip("如果打勾會自動選擇子物件layer作為控制依據")]
    public bool isUseDevalutLayer = true;

    [Tooltip("在isUseDevalutLayer為False時打開")]
    public string[] CustomTagsMask;

    //[Tooltip("最上層會先消失")]
    //public bool isUpSideDown = true;

    public  string[] Tags;
    //public Transform[] testuse;

    [Tooltip("虛擬物體數量百分比")]
    [Range(0,100)]public int PercentageOfVisibleObjects = 100;

    public List<listLayerObject> m_LayerObject = new List<listLayerObject>();

    private int divideNumber;

    // Use this for initialization
    void Start () {
        
        if (isUseDevalutLayer && transform.childCount > 0)
        {
            Tags = new string[transform.childCount];
            for (int i = 0; i < Tags.Length; i++)
            {
                Tags[i] = transform.GetChild(i).gameObject.tag;
                //Debug.Log(Tags[i]);
            }
        }
        else if (!isUseDevalutLayer && CustomTagsMask.Length > 0)
        {
            Tags = CustomTagsMask;
        }
        else
        {
            Debug.LogError("there are not childs in this gameObject,please add some childs and custom layers in below");
        }

        int numberOfTags = Tags.Length;
        divideNumber = (int) Mathf.Floor(100 / ((numberOfTags != 0) ? numberOfTags : 1));
        //Debug.Log(divideNumber);
        //Debug.Log(Tags.Length);
        for (int i = 0; i < Tags.Length; i++)
        {
            if(isUseDevalutLayer)
            {
                
                listLayerObject newLayerObjects = new listLayerObject();
                newLayerObjects.name = Tags[i];
                newLayerObjects.objects = transform.GetChild(i).GetComponentsInChildren<MeshRenderer>();

                //Debug.Log("i : " + i + " objects.Length : " + newLayerObjects.objects.Length);
                m_LayerObject.Add(newLayerObjects);

                //testuse = newLayerObjects.objects;
            }
            else{
                GameObject newFather = new GameObject();
                newFather.transform.parent = null;
                newFather.name = Tags[i];
                newFather.tag = "Untagged";

                GameObject[] tagObjects = GameObject.FindGameObjectsWithTag(Tags[i]);
                foreach(GameObject gobj in tagObjects)
                {
                    gobj.transform.parent = newFather.transform;
                }

                listLayerObject newLayerObjects = new listLayerObject();
                newLayerObjects.name = Tags[i];
                newLayerObjects.objects = newFather.transform.GetComponentsInChildren<MeshRenderer>();
                m_LayerObject.Add(newLayerObjects);
            }
        }

	}
	
	// Update is called once per frame
	void Update () {
        NumberControlVisible(PercentageOfVisibleObjects);
	}

    void NumberControlVisible(int percentage)
    {
        
        int districtNumber = (percentage/divideNumber) == Tags.Length ? 0 :(Tags.Length-1)- percentage / divideNumber;
        float percentageInDistrict = (percentage / divideNumber) == Tags.Length ? 0: 1-(float)percentage % divideNumber/(float)divideNumber ;
        //Debug.Log("districtNumber : "+districtNumber + "percentageInDistrict : " + percentageInDistrict);
        ManageObjectAcitve( districtNumber, percentageInDistrict);

    }
    int lastDistrict;
    float lasrPercentage;
    void ManageObjectAcitve( int district, float districtPercentage)
    {
        if ((district == lastDistrict && districtPercentage * 100 == lasrPercentage * 100) )
            return;

        
        int endPoint = (int)Mathf.Lerp(0.001f, m_LayerObject[district].objects.Length - 1, districtPercentage);
        //Debug.Log("endPoint : " + endPoint);

        /*
         * 
         * 
         if ((district == lastDistrict && (int)districtPercentage * 100 == (int)lasrPercentage * 100) || (int)districtPercentage * 100 == 0)
            return;
            */

        bool enable = false;
        for (int i = 0; i < Tags.Length; i++)
        {
            if (i == district)
            {
                for (int n = 0; n <= m_LayerObject[i].objects.Length - 1; n++)
                {
                    if (n == endPoint)
                    {
                        if (n == m_LayerObject[i].objects.Length - 1 && i == Tags.Length - 1)
                            enable = false;
                        else enable = true;
                    }
                    if (m_LayerObject[i].objects[n].gameObject.activeSelf != enable)
                        m_LayerObject[i].objects[n].gameObject.SetActive(enable);
                }

            }
            else
            {

                foreach (MeshRenderer m_object in m_LayerObject[i].objects)
                {
                    if (m_object.gameObject.activeSelf != enable)
                        m_object.gameObject.SetActive(enable);
                }
            }

        }
        lastDistrict = district;
        lasrPercentage = districtPercentage;
    }
}
