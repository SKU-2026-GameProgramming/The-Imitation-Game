using UnityEngine;

public class Province : MonoBehaviour
{
    //지역 고유 정보
    public int nodeID; //지역 인덱스
    public string nodeName; //지역 이름(스크립트용)
    public string nodeKRName; //지역 한국 이름(UI용)
    public Province[] adjacentProvinces = new Province[0]; //인접 지역들을 담는 배열

    //점령전 시 정보
    public int importance; //점령 시 가치
    public bool isOwned; //점령 여부(뺏기면 False)
    public bool isAttackable = false; //공격 가능 여부
    public bool isDefendable = false; //방어 가능 여부
    public int power; //점령(또는 수비)할 때 투입할 전력의 양

    public SpriteRenderer GetFlag()
    {
        SpriteRenderer flag = GetComponentInChildren<SpriteRenderer>();
        return flag;
    }
}
