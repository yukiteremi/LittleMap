using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BigMapPanel : MonoBehaviour
{
    public Image Map;
    public Image TargetImage;
    public Button btn;
    public Action Del;
    // Start is called before the first frame update
    void Start()
    {
        btn.onClick.AddListener(()=> {
            Del();
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnSceneMapClick();
        }
    }

    void OnSceneMapClick()
    {
        Vector3 localPos = Map.transform.InverseTransformPoint(Input.mousePosition);
        Vector3 mapPos = MapPosToScenePos(localPos);
        bool flag= WorldSingel.ins.player.AutoMove(mapPos);
        if (flag)
        {
            TargetImage.transform.position = Input.mousePosition;
        }
    }

    Vector3 MapPosToScenePos(Vector3 mapPos)
    {
        //mapPos = ApplyMapOffset(mapPos, 1);
        float posScale = (float)Map.GetComponent<RectTransform>().rect.width / 200;
        float curXPos = (mapPos.x + Map.GetComponent<RectTransform>().rect.width * 0.5f) / posScale;
        float curYPos = (mapPos.y + Map.GetComponent<RectTransform>().rect.height * 0.5f) / posScale;
        return new Vector3(curXPos, 101, curYPos);
    }


}
