using UnityEngine;

public class ProvinceClickHandler : MonoBehaviour
{
    public LayerMask provinceLayer;

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

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
            Debug.Log("영역 감지 실패");
            return;
        }

        Province province =
            hit.collider.GetComponent<Province>();

        if (province == null)
            return;

        if (MapManager.Instance == null)
        {
            Debug.LogError("MapManager.Instance가 null입니다. MapManager가 씬에 없거나 Awake에서 Instance 설정이 안 됐습니다.");
            return;
        }

        MapManager.Instance.OnProvinceClicked(province);
    }
}
