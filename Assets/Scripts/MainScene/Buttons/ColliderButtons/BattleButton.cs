using UnityEngine;

public class BattleButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    RectTransform confirm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        confirm = GameObject.Find("BattleConfirmUI").GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {
        confirm.anchoredPosition = Vector2.zero;
    }
}
