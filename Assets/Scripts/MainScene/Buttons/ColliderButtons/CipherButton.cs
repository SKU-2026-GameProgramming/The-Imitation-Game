using UnityEngine;

public class CipherButton : MonoBehaviour
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

    void Update()
    {

    }

    private void OnMouseDown()
    {
        if (!director.isUIOpened)
        {
            aud.PlayOneShot(clickSF);
            cipherUI.anchoredPosition = new Vector3(-470, 0, 0);
        }
    }
}
