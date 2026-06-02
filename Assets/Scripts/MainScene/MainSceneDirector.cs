using TMPro;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    AudioSource bgm;

    GameObject optionPaper;
    RectTransform optionPaperTf;

    float time = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 60;

        this.bgm = GetComponent<AudioSource>();
        this.bgm.Play();
    }

    // Update is called once per frame
    void Update()
    {
        this.time += Time.deltaTime;
    }
}
