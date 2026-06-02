using UnityEngine;

public class StartMenuDirector : MonoBehaviour
{
    AudioSource bgm;

    public bool isOptionOpened = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.bgm = GetComponent<AudioSource>();
        bgm.Play();
    }

    // Update is called once per frame
    void Update()
    {
           
    }
}
