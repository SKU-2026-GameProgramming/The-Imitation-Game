using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    public Province[] provinces;
    public bool[,] adjacencyMatrix;

    public bool graphInitialized = false;

    private void Awake()
    {
        Instance = this;
        RegisterProvinces();
        InitGraph();
    }

    private void RegisterProvinces()
    {
        provinces = GetComponentsInChildren<Province>().OrderBy(p => p.nodeID).ToArray(); //영토 배열에 영토 저장
        adjacencyMatrix = new bool[provinces.Length, provinces.Length]; //인접 행렬 초기화
        Debug.Log($"Province 초기화 완료: {provinces.Length}개");
    }

    public void OnProvinceClicked(Province province)
    {
        Debug.Log(
            $"선택 지역: {province.nodeKRName} " +
            $"({province.nodeName}) / ID: {province.nodeID}"
        );
    }

    private void InitGraph()
    {
        //0번(Paris) 노드
        AddOneWayEdge(0, 1);
        AddOneWayEdge(0, 2);

        //1번(Melun) 노드
        AddOneWayEdge(1, 0);
        AddOneWayEdge(1, 2);
        AddOneWayEdge(1, 5);
        AddOneWayEdge(1, 8);
        AddOneWayEdge(1, 10);

        //2번(Versalles) 노드
        AddOneWayEdge(2, 0);
        AddOneWayEdge(2, 1);
        AddOneWayEdge(2, 3);
        AddOneWayEdge(2, 5);

        //3번(Evreux) 노드
        AddOneWayEdge(3, 2);
        AddOneWayEdge(3, 4);
        AddOneWayEdge(3, 5);

        //4번(Rouen) 노드
        AddOneWayEdge(4, 5);
        AddOneWayEdge(3, 5);

        //5번(Amiens) 노드
        AddOneWayEdge(5, 1);
        AddOneWayEdge(5, 2);
        AddOneWayEdge(5, 3);
        AddOneWayEdge(5, 4);
        AddOneWayEdge(5, 6);
        AddOneWayEdge(5, 8);

        //6번(Arras) 노드
        AddOneWayEdge(6, 5);
        AddOneWayEdge(6, 7);

        //7번(Lille) 노드
        AddOneWayEdge(7, 6);
        AddOneWayEdge(7, 8);
        AddOneWayEdge(7, 16);
        AddOneWayEdge(7, 17);

        //8번(Laon) 노드
        AddOneWayEdge(8, 1);
        AddOneWayEdge(8, 5);
        AddOneWayEdge(8, 7);
        AddOneWayEdge(8, 9);
        AddOneWayEdge(8, 10);
        AddOneWayEdge(8, 16);

        //9번(Ardennes) 노드
        AddOneWayEdge(9, 8);
        AddOneWayEdge(9, 10);
        AddOneWayEdge(9, 11);
        AddOneWayEdge(9, 16);
        AddOneWayEdge(9, 21);

        //10번(Marne) 노드
        AddOneWayEdge(10, 1);
        AddOneWayEdge(10, 8);
        AddOneWayEdge(10, 9);
        AddOneWayEdge(10, 11);

        //11번(Meuse) 노드
        AddOneWayEdge(11, 9);
        AddOneWayEdge(11, 10);
        AddOneWayEdge(11, 12);
        AddOneWayEdge(11, 13);
        AddOneWayEdge(11, 14);
        AddOneWayEdge(11, 21);

        //12번(Epinal) 노드
        AddOneWayEdge(12, 11);
        AddOneWayEdge(12, 13);
        AddOneWayEdge(12, 15);

        //13번(Nancy) 노드
        AddOneWayEdge(13, 11);
        AddOneWayEdge(13, 12);
        AddOneWayEdge(13, 14);
        AddOneWayEdge(13, 15);

        //14번(Metz) 노드
        AddOneWayEdge(14, 11);
        AddOneWayEdge(14, 13);
        AddOneWayEdge(14, 15);
        AddOneWayEdge(14, 21);
        AddOneWayEdge(14, 22);
        AddOneWayEdge(14, 28);

        //15번(Strasbourg) 노드
        AddOneWayEdge(15, 12);
        AddOneWayEdge(15, 13);
        AddOneWayEdge(15, 14);
        AddOneWayEdge(15, 23);
        AddOneWayEdge(15, 28);

        //16번(Bruxelles) 노드
        AddOneWayEdge(16, 7);
        AddOneWayEdge(16, 8);
        AddOneWayEdge(16, 9);
        AddOneWayEdge(16, 17);
        AddOneWayEdge(16, 18);
        AddOneWayEdge(16, 19);
        AddOneWayEdge(16, 20);
        AddOneWayEdge(16, 21);

        //17번(Brugge) 노드
        AddOneWayEdge(17, 7);
        AddOneWayEdge(17, 16);
        AddOneWayEdge(17, 18);

        //18번(Gent) 노드
        AddOneWayEdge(18, 16);
        AddOneWayEdge(18, 17);
        AddOneWayEdge(18, 19);

        //19번(Hasselt) 노드
        AddOneWayEdge(19, 16);
        AddOneWayEdge(19, 18);
        AddOneWayEdge(19, 20);
        AddOneWayEdge(19, 21);

        //20번(Liege) 노드
        AddOneWayEdge(20, 16);
        AddOneWayEdge(20, 19);
        AddOneWayEdge(20, 21);
        AddOneWayEdge(20, 22);
        AddOneWayEdge(20, 29);
        AddOneWayEdge(20, 34);

        //21번(Namur) 노드
        AddOneWayEdge(21, 9);
        AddOneWayEdge(21, 11);
        AddOneWayEdge(21, 14);
        AddOneWayEdge(21, 16);
        AddOneWayEdge(21, 20);
        AddOneWayEdge(21, 22);

        //22번(Luxembourg) 노드
        AddOneWayEdge(22, 14);
        AddOneWayEdge(22, 20);
        AddOneWayEdge(22, 21);
        AddOneWayEdge(22, 28);
        AddOneWayEdge(22, 29);

        //23번(Freilburg) 노드
        AddOneWayEdge(23, 15);
        AddOneWayEdge(23, 24);
        AddOneWayEdge(23, 26);
        AddOneWayEdge(23, 27);

        //24번(Tubingen) 노드
        AddOneWayEdge(24, 23);
        AddOneWayEdge(24, 25);
        AddOneWayEdge(24, 26);

        //25번(Ulm) 노드
        AddOneWayEdge(25, 24);
        AddOneWayEdge(25, 26);
        AddOneWayEdge(25, 27);

        //26번(Stuttgart) 노드
        AddOneWayEdge(26, 23);
        AddOneWayEdge(26, 24);
        AddOneWayEdge(26, 25);
        AddOneWayEdge(26, 27);

        //27번(Wiesbaden) 노드
        AddOneWayEdge(27, 23);
        AddOneWayEdge(27, 25);
        AddOneWayEdge(27, 26);
        AddOneWayEdge(27, 28);
        AddOneWayEdge(27, 30);

        //28번(Saarbrucken) 노드
        AddOneWayEdge(28, 14);
        AddOneWayEdge(28, 15);
        AddOneWayEdge(28, 22);
        AddOneWayEdge(28, 27);
        AddOneWayEdge(28, 29);
        AddOneWayEdge(28, 30);

        //29번(Mainz) 노드
        AddOneWayEdge(29, 20);
        AddOneWayEdge(29, 22);
        AddOneWayEdge(29, 28);
        AddOneWayEdge(29, 30);
        AddOneWayEdge(29, 34);
        AddOneWayEdge(29, 35);

        //30번(Kassel) 노드
        AddOneWayEdge(30, 27);
        AddOneWayEdge(30, 28);
        AddOneWayEdge(30, 29);
        AddOneWayEdge(30, 31);
        AddOneWayEdge(30, 35);

        //31번(Paderborn) 노드
        AddOneWayEdge(31, 30);
        AddOneWayEdge(31, 32);
        AddOneWayEdge(31, 35);

        //32번(Bielefeld) 노드
        AddOneWayEdge(32, 31);
        AddOneWayEdge(32, 33);
        AddOneWayEdge(32, 35);

        //33번(Munster) 노드
        AddOneWayEdge(33, 32);
        AddOneWayEdge(33, 34);
        AddOneWayEdge(33, 35);

        //34번(Koln) 노드
        AddOneWayEdge(34, 19);
        AddOneWayEdge(34, 20);
        AddOneWayEdge(34, 29);
        AddOneWayEdge(34, 33);
        AddOneWayEdge(34, 35);

        //35번(Dortmund) 노드
        AddOneWayEdge(35, 29);
        AddOneWayEdge(35, 30);
        AddOneWayEdge(35, 31);
        AddOneWayEdge(35, 32);
        AddOneWayEdge(35, 33);
        AddOneWayEdge(35, 34);
    }

    private void AddOneWayEdge(int a, int b)
    {
        adjacencyMatrix[a, b] = true;
    }

    private void AddTwoWayEdge(int a, int b)
    {
        adjacencyMatrix[a, b] = true;
        adjacencyMatrix[b, a] = true;
    }
}
