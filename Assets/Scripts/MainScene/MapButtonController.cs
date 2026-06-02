using UnityEngine;
using UnityEngine.SceneManagement;

public class MapButtonController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {
        Debug.Log("버튼 작동");
        SceneManager.LoadScene("MapScene", LoadSceneMode.Additive);
    }
}
