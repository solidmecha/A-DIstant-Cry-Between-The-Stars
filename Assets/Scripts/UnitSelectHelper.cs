using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitSelectHelper : MonoBehaviour {

    
    public void UpdateNumbers()
    {
        for(int i=0;i<4;i++)
        {
            transform.GetChild(i).GetComponent<Text>().text = transform.GetChild(i + 4).GetComponent<Slider>().value.ToString() + "/" + transform.GetChild(i + 4).GetComponent<Slider>().maxValue.ToString();
        }
    }

    public void SetMax(int[] Max)
    {
        for (int i = 0; i < 4; i++)
        {
            transform.GetChild(i + 4).GetComponent<Slider>().maxValue=Max[i];
        }
        UpdateNumbers();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
