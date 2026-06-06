using UnityEngine;
using TMPro;

public class Province : MonoBehaviour
{
    //지역 고유 정보
    public int nodeID; //지역 인덱스
    public string nodeName; //지역 이름(스크립트용)
    public string nodeKRName; //지역 한국 이름(UI용)
    public Province[] adjacentProvinces = new Province[0]; //인접 지역들을 담는 배열

    //점령전 시 정보
    public int importance; //점령 시 가치
    public bool isOwned = false; //점령 여부(뺏기면 False)
    public bool isAttackable = false; //공격 가능 여부
    public bool isDefendable = false; //방어 가능 여부
    public int power = 0; //점령(또는 수비)할 때 투입할 전력의 양
    public int enemyPower = 0; //적의 투입시킨 전력의 양
    public bool isSelected = false;
    TextMeshPro powerText;

    //전력 배분 가능 여부 프로퍼티
    public bool allocatable
    {
        get
        {
            return isAttackable || isDefendable;
        }
    }

    public SpriteRenderer GetFlag()
    {
        SpriteRenderer flag = GetComponentInChildren<SpriteRenderer>();
        return flag;
    }

    private void Start()
    {
        powerText = GetComponentInChildren<TextMeshPro>();
    }

    private void Update()
    {
        if (allocatable)
            powerText.text = power.ToString(); 
        else
            powerText.text = "";
    }
}
