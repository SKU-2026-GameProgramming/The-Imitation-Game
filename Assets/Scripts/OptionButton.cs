using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button2 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    AudioSource audio;
    public AudioClip clickSF, pageSF;

    GameObject menuPointer;
    RectTransform menuPointerTf;
    Image menuPointerImage;

    GameObject optionPaper;
    RectTransform optionPaperTf;

    GameObject director;
    StartMenuDirector directorSC;


    private void Start()
    {
        this.audio = GetComponent<AudioSource>();

        this.menuPointer = GameObject.Find("MenuPointer");
        this.menuPointerTf = this.menuPointer.GetComponent<RectTransform>();
        this.menuPointerImage = this.menuPointer.GetComponent<Image>();

        this.optionPaper = GameObject.Find("OptionPaper");
        this.optionPaperTf = this.optionPaper.GetComponent<RectTransform>();

        this.director = GameObject.Find("GameDirector");
        this.directorSC = this.director.GetComponent<StartMenuDirector>();
    }

    public void OnPointerEnter(PointerEventData eventData) //커서에 버튼 갖다대면 메뉴 포인터 삼각형 출현
    {
        menuPointerTf.anchoredPosition = new Vector3(-715, -82, 0);
        menuPointerImage.color = new Color32(255, 255, 255, 255);
    }

    public void OnPointerExit(PointerEventData eventData) //커서가 버튼 탈출 시 메뉴 포인터 삼각형 투명화
    {
        menuPointerImage.color = new Color32(255, 255, 255, 0);
    }

    public void OnButtonClick() //클릭 시 옵션 창 소환
    {
        if (!this.directorSC.isOptionOpened)
        {
            this.directorSC.isOptionOpened = true;
            StartCoroutine(SlideDown());
            this.audio.PlayOneShot(this.pageSF);
            Debug.Log("설정");
        }
    }

    IEnumerator SlideDown() //창 소환 함수
    {
        Vector3 startPos = this.optionPaperTf.anchoredPosition;
        Vector3 endPos = new Vector3(0, 0, 0);
        float elapsedTime = 0f;

        while (elapsedTime < 0.2f)
        {
            elapsedTime += Time.deltaTime;

            float progress = elapsedTime / 0.2f;

            this.optionPaperTf.anchoredPosition = Vector3.Lerp(startPos, endPos, progress);

            yield return null; 
        }

        this.optionPaperTf.anchoredPosition = endPos;
    }
}
