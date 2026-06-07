using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    RectTransform msgBox;
    RectTransform confirmBox;
    RectTransform cipherUI;
    GameObject map;
    MapManager mm; 
    MainSceneDirector director;

    AudioSource aud;
    public AudioClip clickSF;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        msgBox = GameObject.Find("ProvinceUI").GetComponent<RectTransform>();
        confirmBox = GameObject.Find("BattleConfirmUI").GetComponent<RectTransform>();
        map = GameObject.Find("Map");
        cipherUI = GameObject.Find("CipherUI").GetComponent<RectTransform>();
        mm = GameObject.Find("Provinces").GetComponent<MapManager>();
        director = GameObject.Find("GameDirector").GetComponent<MainSceneDirector>();
        aud = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    public void OnAllocateButtonClick()
    {
        Debug.Log("투입 버튼 클릭");
        aud.PlayOneShot(clickSF);
        if (mm.AllocatePlayerPower())
            msgBox.anchoredPosition = new Vector3(0, 760, 0);
    }

    public void OnMsgQuitButtonClick()
    {
        aud.PlayOneShot(clickSF);
        msgBox.anchoredPosition = new Vector3(0, 760, 0);
    }

    public void OnNoConfirmButtonClick()
    {
        aud.PlayOneShot(clickSF);
        confirmBox.anchoredPosition = new Vector3(732, 766, 0);
    }

    public void OnConfirmBattleClick()
    {
        aud.PlayOneShot(clickSF);

        cipherUI.anchoredPosition = new Vector3(-1955, 1055, 0);
        director.isUIOpened = false;
        map.transform.position = new Vector3(0, 10, 0);
        confirmBox.anchoredPosition = new Vector3(732, 766, 0);
        mm.ConfirmBattle();
    }
}
