using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CipherButtonManager : MonoBehaviour
{
    RectTransform cipherUI;
    MainSceneDirector director;

    AudioSource aud;
    public AudioClip clickSF;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cipherUI = GameObject.Find("CipherUI").GetComponent<RectTransform>();
        director = GameObject.Find("GameDirector").GetComponent<MainSceneDirector>();
        aud = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    public void OnQuitButtonClick()
    {
        
        aud.PlayOneShot(clickSF);
        cipherUI.anchoredPosition = new Vector3(-842, 1100, 0);
        director.isUIOpened = false; 
    }
}