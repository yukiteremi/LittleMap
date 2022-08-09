using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject BigMapPanel,LittleMapPanel;
    // Start is called before the first frame update
    void Start()
    {
        WorldSingel.ins.player = new Obj_MainPlayer(player);
        BigMapPanel.GetComponent<BigMapPanel>().Del = HideBigMap;
        LittleMapPanel.GetComponent<Button>().onClick.AddListener(ShowBigMap);
    }

    public void ShowBigMap()
    {
        BigMapPanel.SetActive(true);
        LittleMapPanel.SetActive(false);
    }
    public void HideBigMap()
    {
        LittleMapPanel.SetActive(true);
        BigMapPanel.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        WorldSingel.ins.player.Update();
    }
}
