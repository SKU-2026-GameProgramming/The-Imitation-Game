using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Button1 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    GameObject menuPointer;
    RectTransform menuPointerTf;
    Image menuPointerImage;

    GameObject director;
    StartMenuDirector directorSC;

    private void Start()
    {
        menuPointer = GameObject.Find("MenuPointer");
        menuPointerTf = menuPointer.GetComponent<RectTransform>();
        menuPointerImage = menuPointer.GetComponent<Image>();

        this.director = GameObject.Find("GameDirector");
        this.directorSC = this.director.GetComponent<StartMenuDirector>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        menuPointerTf.anchoredPosition = new Vector3(-715, 56, 0);
        menuPointerImage.color = new Color32(255, 255, 255, 255);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        menuPointerImage.color = new Color32(255, 255, 255, 0);
    }

    public void OnButtonClick()
    {
        if (!this.directorSC.isOptionOpened)
        {
            Debug.Log("게임 시작");
            SceneManager.LoadScene("IntroScene");
        }
    }
}
