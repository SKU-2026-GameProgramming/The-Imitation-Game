using TMPro;
using UnityEngine;

public class ProvinceClickHandler : MonoBehaviour
{
    public LayerMask provinceLayer;
    public AudioClip clickSF;

    GameObject provinceUI;
    RectTransform provinceUIRT;

    GameObject krName;
    TextMeshProUGUI krNameText;
    GameObject engName;
    TextMeshProUGUI engNameText;
    GameObject pow;
    TextMeshProUGUI powText;

    private void Start()
    {
        provinceUI = GameObject.Find("ProvinceUI");
        provinceUIRT = provinceUI.GetComponent<RectTransform>();

        krName = GameObject.Find("KRNameText");
        krNameText = krName.GetComponent<TextMeshProUGUI>();
        engName = GameObject.Find("NameText");
        engNameText = engName.GetComponent<TextMeshProUGUI>();
        pow = GameObject.Find("PowerText");
        powText = pow.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
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
            provinceUIRT.anchoredPosition = new Vector3(0, 750, 0);
            return;
        }

        Province province = hit.collider.GetComponent<Province>();

        if (province == null)
        {
            provinceUIRT.anchoredPosition = new Vector3(0, 750, 0);
            return;
        }


        if (MapManager.Instance == null)
        {
            Debug.LogError("MapManager.Instance가 null입니다. MapManager가 씬에 없거나 Awake에서 Instance 설정이 안 됐습니다.");
            return;
        }

        MapManager.Instance.OnProvinceClicked(province);

        provinceUIRT.anchoredPosition = new Vector3(Input.mousePosition.x - 750, Input.mousePosition.y - 425, 0);

        krNameText.text = province.nodeKRName;
        engNameText.text = province.nodeName;
        powText.text = "투입 전력: ";
    }
}
