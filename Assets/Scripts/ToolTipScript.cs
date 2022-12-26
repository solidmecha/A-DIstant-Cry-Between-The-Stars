using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ToolTipScript : MonoBehaviour, IPointerEnterHandler
{
    public string Tip;


    public void OnPointerEnter(PointerEventData eventData)
    {
        GameControl.singleton.MsgText.text = Tip;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        GameControl.singleton.MsgText.text = "";
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
