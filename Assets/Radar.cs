/********************************************************************
	created:	2014/02/27
	created:	27:2:2014   9:45
	filename: 	Radar.cs
	file base:	Radar
	author:		王迪
	
	purpose:    雷达小地图
*********************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Radar : MonoBehaviour
{
    private float m_mapTexWidth = 1470;      // 地图预制体宽度
    private float m_mapTexHeight = 1470;     // 地图预制体长度 之后会将地图图片扩大至此大小
    //private float m_mapRealWidth = 42;      // 图片宽度对应的逻辑宽度

    private float MapScreenHalfWidth = 100;  // 显示区域宽度的一半
    private float MapScreenHalfHeight = 100; // 显示区域高度的一半

    public float UPDATE_DELAY = 0.5f;       // 更新延迟

    public GameObject   MapClip;        //地图
    public GameObject   ObjArrow;       // 主角箭头
	public GameObject   FriendPoint;	// 友好npc的预制体 
	public GameObject	NeutralPoint;   // 中立npc的预制体 
    public GameObject	EnemyPoint;     // 敌对npc的预制体 
    public GameObject   OtherPoint;     // 其他npc的预制体 
    public Text LabelPos;               // 位置信息Label
	public GameObject   TexTarget;      // 寻路目标位置提示图片  如有必要再添加

    public Text LabelSceneName;         // 当前场景名
    public Text LabelChannel;           // 当前频道
    public GameObject    PanelMapClip;
    private Vector3 arrowPos    = Vector3.zero;     
    private Vector3 arrowRot    = Vector3.zero;
    private Vector3 mapPos      = Vector3.zero;  

	private  List<Image> TexListFriend     = new List<Image>();
	private  List<Image> TexListNeutral    = new List<Image>();
	private  List<Image> TexListEnemy      = new List<Image>();
	private  List<Image> TexListOther      = new List<Image>();

    private float m_scale = 1.0f;     // 当前地图与实际地形比例
    private bool m_bLoadMap = false;
    
    void Start()
    {
        ObjArrow.SetActive(false);
        m_bLoadMap = false;
       

        LabelSceneName.color = Color.green;
        LabelSceneName.text = "地图1";

        //m_mapTexWidth = curScene.SceneMapWidth;
        //m_mapTexHeight = curScene.SceneMapHeight;
        //if (curScene.SceneMapLogicWidth == 0)
        //{
        //    // LogModule.ErrorLog("load scene with is 0 :" + curScene.SceneMapTexture);
        //    return;
        //}
        m_scale = m_mapTexWidth / (2* MapScreenHalfWidth);
        //m_scale = 7.5f;
        Sprite curTexture = Resources.Load<Sprite>("MinMap_jiangxia");
        if (null == curTexture)
        {
            //LogModule.ErrorLog("load scene map fail :" + curScene.SceneMapTexture);
            return;
        }
        else
        {
            MapScreenHalfHeight = PanelMapClip.GetComponent<RectTransform>().rect.width * 0.5f;
            MapScreenHalfWidth = PanelMapClip.GetComponent<RectTransform>().rect.height * 0.5f;
            MapClip.GetComponent<Image>().sprite = curTexture;
            MapClip.GetComponent<RectTransform>().sizeDelta = new Vector2((int)m_mapTexWidth, (int)m_mapTexHeight);
            MapClip.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
        }
        ObjArrow.SetActive(true);
       
        m_bLoadMap = true;

        InvokeRepeating("UpdateMap", 0, UPDATE_DELAY);
    }
    
    void UpdateMap()
    {
        if (!m_bLoadMap)
        {
            return;
        }

        Obj_MainPlayer curPlayer = WorldSingel.ins.player;
        if (null == curPlayer)
        {
            return;
        }

        

        arrowPos = GetMapPos(curPlayer.Go.transform.position);
        ObjArrow.transform.localPosition = arrowPos;

        arrowRot.z = -curPlayer.Go.transform.localRotation.eulerAngles.y;
        ObjArrow.transform.rotation = Quaternion.Euler(arrowRot);

        mapPos.x = Mathf.Min(-MapScreenHalfWidth, Mathf.Max(-arrowPos.x, MapScreenHalfWidth - m_mapTexWidth));
        mapPos.y = Mathf.Min(-MapScreenHalfHeight, Mathf.Max(-arrowPos.y, MapScreenHalfHeight - m_mapTexHeight));
        MapClip.transform.localPosition = mapPos;

		if(null != LabelPos)
		{
			LabelPos.text =((int)curPlayer.Go.transform.position.x).ToString() + ", " + ((int)curPlayer.Go.transform.position.z).ToString();
		}


        int curFriendCount = 0;
        int curNeutralCount = 0;
        int curEnemyCount = 0;
        int curOtherCount = 0;
        foreach (Obj curObj in WorldSingel.ins.CharatorPool.Values)
        {
            //只显示如下三种类型
            if (curObj is Obj_Character)
            {
                Obj_Character curChar = curObj as Obj_Character;
                if (null == curChar)
                {
                    continue;
                }

                float xPosDiff = curChar.Go.transform.localPosition.x - curPlayer.Go.transform.localPosition.x;
                float yPosDiff = curChar.Go.transform.localPosition.z - curPlayer.Go.transform.localPosition.z;

                if (Mathf.Abs(xPosDiff) * m_scale > MapScreenHalfWidth || Mathf.Abs(yPosDiff) * m_scale > MapScreenHalfHeight)
                {
                    continue;
                }
               
                AddDotToList(TexListFriend, curFriendCount, FriendPoint, curObj, Color.green);
                setTexColor(curChar, TexListFriend, curFriendCount);
                curFriendCount++;
              
            }

          

        }

		DeActiveList(curFriendCount, TexListFriend, arrowPos);
		DeActiveList(curNeutralCount, TexListNeutral,arrowPos);
		DeActiveList(curEnemyCount, TexListEnemy,arrowPos);
		DeActiveList(curOtherCount, TexListOther,arrowPos);
        
    }

    private void setTexColor(Obj_Character curChar, List<Image> texList, int index)
    {
        if (curChar.Die)
        {
            if (texList[index].enabled)
            {
                texList[index].color = new Color(1, 1, 1, 0.005f);
                texList[index].enabled = false;
            }
        }
    }
    // 将小点加入缓存列表
    void AddDotToList(List<Image> curList,  int curIndex, GameObject instanceObj,  Obj curShowObj, Color color)
    {
        if (curIndex >= curList.Count)
        {
			GameObject newObj = CreateRadarPoint(instanceObj, curShowObj.Go.gameObject.transform.localPosition);
			if (null == newObj)
				return;

			Image sprite = newObj.GetComponent<Image>();
//			GameObject newObj = CreateTexture(color, curShowObj.transform.localPosition);
			if (null != sprite)
				curList.Add(sprite);
        }
        else
        {
			//            curList[curIndex].SetActive(true);
			Obj_Character curChar = curShowObj as Obj_Character;
			if(!curChar.Die)
			{
				curList[curIndex].enabled = true;
				curList[curIndex].color = Color.white;
				curList[curIndex].gameObject.transform.localPosition = GetMapPos(curShowObj.Go.gameObject.transform.localPosition);
			}else{
				curList[curIndex].enabled = false;
			}

        }
    }

    // 逻辑位置转换地图位置
    Vector3 GetMapPos(Vector3 curPos)
    {
		return GetMapPos(curPos.x, curPos.z);
	}

    // 逻辑位置转换地图位置
	Vector3 GetMapPos(float xPos, float zPos)
	{
			Vector3 tempPos = Vector3.zero;
			tempPos.x = xPos * m_scale;
			tempPos.y = zPos * m_scale;
			return tempPos;
	}

	// Create a Radar Point
	GameObject CreateRadarPoint(GameObject obj, Vector3 targetPos)
	{
		if (null == obj)
			return null;

		GameObject newObj = (GameObject)GameObject.Instantiate(obj);
		if (null == newObj)
			return null;

		newObj.transform.parent = MapClip.transform;
		newObj.transform.localScale = Vector3.one;
		newObj.transform.localPosition = GetMapPos(targetPos);
		newObj.layer = MapClip.layer;
		newObj.SetActive(true);

		return newObj;
	}

    // 将不用的小点隐藏
    void DeActiveList(int useCount, List<Image> curList, Vector3 centerPos)
    {
        Vector3 finalPos = centerPos;
        for (int i = useCount; i < curList.Count; i++)
        {
            if (curList[i].color != new Color(1, 1, 1, 0.005f))
            {
                finalPos.x = centerPos.x - MapScreenHalfWidth / 2 + Random.Range(0, MapScreenHalfWidth);
                finalPos.y = centerPos.y - MapScreenHalfHeight / 2 + Random.Range(0, MapScreenHalfHeight);
                curList[i].color = new Color(1, 1, 1, 0.005f);
                curList[i].transform.localPosition = finalPos;
            }
        }
    }

}