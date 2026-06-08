using TMPro;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CipherButtonManager : MonoBehaviour
{
    RectTransform cipherUI;
    RectTransform map;
    RectTransform guide;
    RectTransform shifter;

    MainSceneDirector director;
    MapManager mm;

    public TextMeshProUGUI[] cipherChars = new TextMeshProUGUI[4];
    

    AudioSource aud;
    public AudioClip clickSF;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cipherUI = GameObject.Find("CipherUI").GetComponent<RectTransform>();
        map = GameObject.Find("CipherUIMap").GetComponent<RectTransform>();
        guide = GameObject.Find("Guide").GetComponent <RectTransform>();
        shifter = GameObject.Find("ShifterUI").GetComponent<RectTransform>();

        director = GameObject.Find("GameDirector").GetComponent<MainSceneDirector>();
        mm = GameObject.Find("Provinces").GetComponent<MapManager>();

        aud = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    public void OnQuitButtonClick()
    {
        
        aud.PlayOneShot(clickSF);
        cipherUI.anchoredPosition = new Vector3(-842, 1100, 0);
    }

    public void OnMapButtonClick()
    {
        aud.PlayOneShot(clickSF);
        map.anchoredPosition = Vector2.zero;
    }

    public void OnMapQuitButtonClick()
    {
        aud.PlayOneShot(clickSF);
        map.anchoredPosition = new Vector3(0, 1511, 0);
    }

    public void OnGuideButtonClick()
    {
        aud.PlayOneShot(clickSF);
        guide.anchoredPosition = Vector2.zero;
    }

    public void OnQuitGuideButtonClick()
    {
        aud.PlayOneShot(clickSF);
        guide.anchoredPosition = new Vector3(0, 1475, 0);
    }

    public void OnCipherClick()
    { 
        aud.PlayOneShot(clickSF);

        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;

        if (clickedButton != null && mm.day >= 2 )
        {
            string cipher = clickedButton.GetComponentInChildren<TextMeshProUGUI>().text;
            for (int i = 0; i < 4; i++)
                cipherChars[i].text = cipher.Substring(i, 1);
            shifter.anchoredPosition = new Vector3(485, 0, 0);
            director.isShifterOpened = true;
        }
    }
}