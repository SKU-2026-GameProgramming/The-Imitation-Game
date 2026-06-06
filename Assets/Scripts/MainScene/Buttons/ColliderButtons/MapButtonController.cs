using UnityEngine;
using UnityEngine.SceneManagement;

public class MapButtonController : MonoBehaviour
{
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
        if (!director.isUIOpened)
        {
            map.transform.position = new Vector3(-0.03f, -0.4f, 0);
            director.isUIOpened = true;
        }
    }
}
