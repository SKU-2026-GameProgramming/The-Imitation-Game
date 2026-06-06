using UnityEngine;

public class MapQuitButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    GameObject map;
    MainSceneDirector director;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        map = GameObject.Find("Map");
        director = GameObject.Find("GameDirector").GetComponent<MainSceneDirector>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {
        map.transform.position = new Vector3(-0.03f, 10.0f, 0);
        director.isUIOpened = false;
    }
}
