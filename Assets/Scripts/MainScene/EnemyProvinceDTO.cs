using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class EnemyProvinceDTO
{
    //지역 고유 정보
    public int nodeID; //지역 인덱스
    public string nodeName; //지역 이름(스크립트용)
    public int[] adjacentNodeID;

    //점령전 시 정보
    public string grade; //점령 시 가치
    public bool isOwnedByEnemy = false; //점령 여부(뺏기면 False)
    public bool isAttackableByEnemy = false; //공격 가능 여부
    public bool isDefendableByEnemy = false; //방어 가능 여부
    public int power; //점령(또는 수비)할 때 투입할 전력의 양

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class EnemyProvinceDTOContainer
{
    public EnemyProvinceDTO[] provinces;
}
