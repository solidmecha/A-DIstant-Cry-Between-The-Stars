using UnityEngine;
using System.Collections;

public class ZoneScript : MonoBehaviour {

    public bool PlayerOwned;
    public int ID;
    public int Count;
    public int index;
    public int Value;
    public bool AtkZone;
    public UnityEngine.UI.Text ValText;

    private void OnMouseEnter()
    {
        if (PlayerOwned && Input.GetMouseButton(0) && ShipControl.singleton.ActiveUnit != null && Count<ShipControl.singleton.MaxShipCounts[ID]
            &&ShipControl.singleton.ActiveUnit.Zone.ID!=ID)
        {
            ShipControl.singleton.ZoneIndex = ID;
        }
    }

    public void PlaceShip(ShipScript S)
    {
        S.Zone.Count--;
        S.transform.position = transform.GetChild(index).position;
        S.transform.SetParent(transform.GetChild(index));
        S.Zone.FindIndex();
        S.Zone.CalcValue();
        S.Zone = this;
        S.GetComponent<BoxCollider2D>().enabled = true;
        Count++;
        FindIndex();
        CalcValue();
    }

    void FindIndex()
    {
        index = -1;
        for(int i=0;i<transform.childCount;i++)
        {
            if (transform.GetChild(i).childCount == 0)
                index = i;
        }
    }

    public void HandleLoss()
    {
        for(int i=0;i<4;i++)
        {
            if(transform.GetChild(i).childCount==1)
                transform.GetChild(i).GetChild(0).GetComponent<ShipScript>().Def = 0;
        }
    }

    public void HandleWin(int diff)
    {
        for (int i = 0; i < 4; i++)
        {
            if (transform.GetChild(i).childCount == 1)
                transform.GetChild(i).GetChild(0).GetComponent<ShipScript>().MakeSave(10-diff);
        }
    }

    public void CalcValue()
    {
        if (ID < 4)
        {
            int v = 0;
            foreach (ShipScript s in GetComponentsInChildren<ShipScript>())
            {
                if (AtkZone)
                    v += s.Atk;
                else
                    v += s.Def;
            }
            Value = v;
            ValText.text = v.ToString();
        }
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
