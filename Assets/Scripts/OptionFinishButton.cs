using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionFinishButton : MonoBehaviour
{
    AudioSource clickSF;

    GameObject optionPaper;
    RectTransform optionPaperTf;

    GameObject director;
    StartMenuDirector directorSC;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.clickSF = GetComponent<AudioSource>();

        this.optionPaper = GameObject.Find("OptionPaper");
        this.optionPaperTf = this.optionPaper.GetComponent<RectTransform>();

        this.director = GameObject.Find("GameDirector");
        this.directorSC = this.director.GetComponent<StartMenuDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonClick() //클릭 시 옵션 창 제거
    { 
        this.directorSC.isOptionOpened = false;
        this.optionPaperTf.anchoredPosition = new Vector3(0, 1000, 0);
        this.clickSF.Play();
        Debug.Log("설정");
        
    }
}
