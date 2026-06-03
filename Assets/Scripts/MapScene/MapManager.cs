using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    public Province[] provinces;

    public bool graphInitialized = false;

    public Sprite[] sprites = new Sprite[4];


    private void Awake()
    {
        Instance = this;
        RegisterProvinces();
        InitGraph();
        UpdateProvincesState();
        ChangeFlag();
    }

    private void RegisterProvinces()
    {
        provinces = GetComponentsInChildren<Province>().OrderBy(p => p.nodeID).ToArray(); //영토 배열에 영토 저장
        Debug.Log($"Province 초기화 완료: {provinces.Length}개");
    }

    public void OnProvinceClicked(Province province)
    {
        string str = "인접 영토: ";
        foreach(Province p in province.adjacentProvinces)
        {
            str += p.nodeKRName + " ";
        }

        Debug.Log(
            $"선택 지역: {province.nodeKRName} " +
            $"({province.nodeName}) / ID: {province.nodeID}"
        );
        Debug.Log(str);
        
    }

    public void ChangeFlag()
    {
        foreach(Province p in provinces)
        {
            SpriteRenderer flag = p.GetFlag();
            if (p.isOwned)
            {
                if (p.isDefendable) 
                    flag.sprite = sprites[3];
                else
                    flag.sprite = sprites[1];

            } 
            else
            {
                if (p.isAttackable)
                {
                    flag.sprite = sprites[2];
                }
                else
                {
                    flag.sprite = sprites[0];
                }
            }
                
        }
    }

    public void UpdateProvincesState()
    {
        foreach (Province province in provinces)
        {
            province.isAttackable = false;
            province.isDefendable = false;

            foreach (Province adjacent in province.adjacentProvinces)
            {
                if (province.isOwned && !adjacent.isOwned)
                {
                    province.isDefendable = true;
                    break;
                }

                if (!province.isOwned && adjacent.isOwned)
                {
                    province.isAttackable = true;
                    break;
                }
            }
        }
    }

    private void InitGraph()
    {
        //0번(Paris) 노드
        AddEdge(0, 1);
        AddEdge(0, 2);

        //1번(Melun) 노드
        AddEdge(1, 0);
        AddEdge(1, 2);
        AddEdge(1, 5);
        AddEdge(1, 8);
        AddEdge(1, 10);

        //2번(Versalles) 노드
        AddEdge(2, 0);
        AddEdge(2, 1);
        AddEdge(2, 3);
        AddEdge(2, 5);

        //3번(Evreux) 노드
        AddEdge(3, 2);
        AddEdge(3, 4);
        AddEdge(3, 5);

        //4번(Rouen) 노드
        AddEdge(4, 3);
        AddEdge(4, 5);

        //5번(Amiens) 노드
        AddEdge(5, 1);
        AddEdge(5, 2);
        AddEdge(5, 3);
        AddEdge(5, 4);
        AddEdge(5, 6);
        AddEdge(5, 8);

        //6번(Arras) 노드
        AddEdge(6, 5);
        AddEdge(6, 7);

        //7번(Lille) 노드
        AddEdge(7, 6);
        AddEdge(7, 8);
        AddEdge(7, 16);
        AddEdge(7, 17);

        //8번(Laon) 노드
        AddEdge(8, 1);
        AddEdge(8, 5);
        AddEdge(8, 7);
        AddEdge(8, 9);
        AddEdge(8, 10);
        AddEdge(8, 16);

        //9번(Ardennes) 노드
        AddEdge(9, 8);
        AddEdge(9, 10);
        AddEdge(9, 11);
        AddEdge(9, 16);
        AddEdge(9, 21);

        //10번(Marne) 노드
        AddEdge(10, 1);
        AddEdge(10, 8);
        AddEdge(10, 9);
        AddEdge(10, 11);

        //11번(Meuse) 노드
        AddEdge(11, 9);
        AddEdge(11, 10);
        AddEdge(11, 12);
        AddEdge(11, 13);
        AddEdge(11, 14);
        AddEdge(11, 21);

        //12번(Epinal) 노드
        AddEdge(12, 11);
        AddEdge(12, 13);
        AddEdge(12, 15);

        //13번(Nancy) 노드
        AddEdge(13, 11);
        AddEdge(13, 12);
        AddEdge(13, 14);
        AddEdge(13, 15);

        //14번(Metz) 노드
        AddEdge(14, 11);
        AddEdge(14, 13);
        AddEdge(14, 15);
        AddEdge(14, 21);
        AddEdge(14, 22);
        AddEdge(14, 28);

        //15번(Strasbourg) 노드
        AddEdge(15, 12);
        AddEdge(15, 13);
        AddEdge(15, 14);
        AddEdge(15, 23);
        AddEdge(15, 28);

        //16번(Bruxelles) 노드
        AddEdge(16, 7);
        AddEdge(16, 8);
        AddEdge(16, 9);
        AddEdge(16, 17);
        AddEdge(16, 18);
        AddEdge(16, 19);
        AddEdge(16, 20);
        AddEdge(16, 21);

        //17번(Brugge) 노드
        AddEdge(17, 7);
        AddEdge(17, 16);
        AddEdge(17, 18);

        //18번(Gent) 노드
        AddEdge(18, 16);
        AddEdge(18, 17);
        AddEdge(18, 19);

        //19번(Hasselt) 노드
        AddEdge(19, 16);
        AddEdge(19, 18);
        AddEdge(19, 20);
        AddEdge(19, 21);

        //20번(Liege) 노드
        AddEdge(20, 16);
        AddEdge(20, 19);
        AddEdge(20, 21);
        AddEdge(20, 22);
        AddEdge(20, 29);
        AddEdge(20, 34);

        //21번(Namur) 노드
        AddEdge(21, 9);
        AddEdge(21, 11);
        AddEdge(21, 14);
        AddEdge(21, 16);
        AddEdge(21, 20);
        AddEdge(21, 22);

        //22번(Luxembourg) 노드
        AddEdge(22, 14);
        AddEdge(22, 20);
        AddEdge(22, 21);
        AddEdge(22, 28);
        AddEdge(22, 29);

        //23번(Freilburg) 노드
        AddEdge(23, 15);
        AddEdge(23, 24);
        AddEdge(23, 26);
        AddEdge(23, 27);

        //24번(Tubingen) 노드
        AddEdge(24, 23);
        AddEdge(24, 25);
        AddEdge(24, 26);

        //25번(Ulm) 노드
        AddEdge(25, 24);
        AddEdge(25, 26);
        AddEdge(25, 27);

        //26번(Stuttgart) 노드
        AddEdge(26, 23);
        AddEdge(26, 24);
        AddEdge(26, 25);
        AddEdge(26, 27);

        //27번(Wiesbaden) 노드
        AddEdge(27, 23);
        AddEdge(27, 25);
        AddEdge(27, 26);
        AddEdge(27, 28);
        AddEdge(27, 30);

        //28번(Saarbrucken) 노드
        AddEdge(28, 14);
        AddEdge(28, 15);
        AddEdge(28, 22);
        AddEdge(28, 27);
        AddEdge(28, 29);
        AddEdge(28, 30);

        //29번(Mainz) 노드
        AddEdge(29, 20);
        AddEdge(29, 22);
        AddEdge(29, 28);
        AddEdge(29, 30);
        AddEdge(29, 34);
        AddEdge(29, 35);

        //30번(Kassel) 노드
        AddEdge(30, 27);
        AddEdge(30, 28);
        AddEdge(30, 29);
        AddEdge(30, 31);
        AddEdge(30, 35);

        //31번(Paderborn) 노드
        AddEdge(31, 30);
        AddEdge(31, 32);
        AddEdge(31, 35);

        //32번(Bielefeld) 노드
        AddEdge(32, 31);
        AddEdge(32, 33);
        AddEdge(32, 35);

        //33번(Munster) 노드
        AddEdge(33, 32);
        AddEdge(33, 34);
        AddEdge(33, 35);

        //34번(Koln) 노드
        AddEdge(34, 19);
        AddEdge(34, 20);
        AddEdge(34, 29);
        AddEdge(34, 33);
        AddEdge(34, 35);

        //35번(Dortmund) 노드
        AddEdge(35, 29);
        AddEdge(35, 30);
        AddEdge(35, 31);
        AddEdge(35, 32);
        AddEdge(35, 33);
        AddEdge(35, 34);
    }

    private void AddEdge(int a, int b)
    {
        provinces[a].adjacentProvinces = provinces[a].adjacentProvinces.Concat(new[] { provinces[b] }).ToArray();
    }

}
