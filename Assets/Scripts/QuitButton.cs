using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button3 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    GameObject menuPointer;
    RectTransform menuPointerTf;
    Image menuPointerImage;

    GameObject director;
    StartMenuDirector directorSC;

    private void Start()
    {
        this.menuPointer = GameObject.Find("MenuPointer");
        this.menuPointerTf = this.menuPointer.GetComponent<RectTransform>();
        this.menuPointerImage = this.menuPointer.GetComponent<Image>();

        this.director = GameObject.Find("GameDirector");
        this.directorSC = this.director.GetComponent<StartMenuDirector>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.menuPointerTf.anchoredPosition = new Vector3(-715, -223, 0);
        this.menuPointerImage.color = new Color32(255, 255, 255, 255);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.menuPointerImage.color = new Color32(255, 255, 255, 0);
    }

    public void OnButtonClick()
    {
        if (!this.directorSC.isOptionOpened)
        {
            Debug.Log("게임 종료");
            UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
        }
    }
}
