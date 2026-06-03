using UnityEngine;

public class Province : MonoBehaviour
{
    //지역 고유 정보
    public int nodeID; //지역 인덱스
    public string nodeName; //지역 이름(스크립트용)
    public string nodeKRName; //지역 한국 이름(UI용)
    
    //점령전 시 정보
    public int importance; //점령 시 가치
    public bool isOwned; //점령 여부(뺏기면 False)
    public int power; //점령(또는 수비)할 때 투입할 전력의 양
}
