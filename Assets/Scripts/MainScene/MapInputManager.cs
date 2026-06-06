using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProvinceClickHandler : MonoBehaviour
{
    public LayerMask provinceLayer;
    public AudioClip clickSF;
    public Province province;

    RectTransform provinceUI;
    TextMeshProUGUI krName;
    TextMeshProUGUI engName;
    TextMeshProUGUI powText;
    TMP_InputField powInputField;
    Button allocateButton;

    private void Start()
    {
        provinceUI = GameObject.Find("ProvinceUI").GetComponent<RectTransform>();
        krName = GameObject.Find("KRNameText").GetComponent<TextMeshProUGUI>();
        engName = GameObject.Find("NameText").GetComponent<TextMeshProUGUI>();
        powText = GameObject.Find("PowerText").GetComponent<TextMeshProUGUI>() ;
        powInputField = GameObject.Find("PowerInput").GetComponent<TMP_InputField>();
        allocateButton = GameObject.Find("AllocateButton").GetComponent<Button>();
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        AudioSource audio = GetComponent<AudioSource>();
        audio.PlayOneShot(clickSF);

        Vector2 mouseWorldPosition =
            Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit =
            Physics2D.Raycast(
                mouseWorldPosition,
                Vector2.zero,
                0f,
                provinceLayer
            );

        if (hit.collider == null)
        {
            provinceUI.anchoredPosition = new Vector3(0, 750, 0);
            return;
        }

        province = hit.collider.GetComponent<Province>();

        if (province == null)
        {
            provinceUI.anchoredPosition = new Vector3(0, 750, 0);
            return;
        }


        if (MapManager.Instance == null)
        {
            Debug.LogError("MapManager.Instance가 null입니다. MapManager가 씬에 없거나 Awake에서 Instance 설정이 안 됐습니다.");
            return;
        }

        MapManager.Instance.OnProvinceClicked(province);

        //말풍선 UI 관리
        provinceUI.anchoredPosition = new Vector3(Input.mousePosition.x - 750, Input.mousePosition.y - 425, 0);

        krName.text = province.nodeKRName;
        engName.text = province.nodeName;

        if (province.allocatable)
        {
            powInputField.gameObject.SetActive(true);
            allocateButton.gameObject.SetActive(true);
            powInputField.text = province.power.ToString();

            if (province.isAttackable)
                powText.text = "공격 병력: ";
            else
                powText.text = "방어 병력: ";
        }
        else
        {
            
            powText.text = "";
            powInputField.gameObject.SetActive(false);
            allocateButton.gameObject.SetActive(false);
        }
            
    }
}
