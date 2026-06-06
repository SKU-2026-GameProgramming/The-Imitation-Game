using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider slider;

    GameObject director;
    AudioSource bgm;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (slider != null)
        {
            slider.onValueChanged.AddListener(OnSliderEvent);
        }

        this.director = GameObject.Find("GameDirector");
        this.bgm = this.director.GetComponent<AudioSource>();
    }


    // Update is called once per frame
   public void OnSliderEvent(float value)
    {
        this.bgm.volume = value;
    }
}
