using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    RectTransform msgBox;
    RectTransform confirmBox;
    MapManager mm;

    AudioSource aud;
    public AudioClip clickSF;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        msgBox = GameObject.Find("ProvinceUI").GetComponent<RectTransform>();
        confirmBox = GameObject.Find("BattleConfirmUI").GetComponent<RectTransform>();
        mm = GameObject.Find("Provinces").GetComponent<MapManager>();
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
        confirmBox.anchoredPosition = new Vector3(732, 766, 0);
        mm.ConfirmBattle();
    }
}
