using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPanel : MonoBehaviour
{
    public Slider Sound;
   // public Slider Speed;
    public GameObject setPanel;
    public void SetSound()
    {
        GameManager.Instance.GetComponent<AudioSource>().volume = Sound.value;
    }

    public void CloseTheSetPanel()
    {
        setPanel.SetActive(false);
    }
}
