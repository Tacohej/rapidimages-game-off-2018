using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIMoveSound : MonoBehaviour, IMoveHandler
{
    SoundManager soundManager;
    void Start()
    {
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }
    //When the focus moves to another selectable object, Invoke this Method.
    public void OnMove(AxisEventData eventData)
    {
        soundManager.PlayClipFile("Sound/MenuSounds/MenuNavigate");
    }
}
