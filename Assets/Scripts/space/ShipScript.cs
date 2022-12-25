using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShipScript : MonoBehaviour {
    public UnityEngine.UI.Text[] Vals;
    public int Atk;
    public int Def;
    public int ID;
    public bool PlayerOwned;
    public ZoneScript Zone;
    public bool visible;
    public Vector3 startPos; 

    public void Roll()
    {
        RollAttack();
        RollDef();
        if (PlayerOwned)
            ShowVals();
        else
        {
            Vals[0].text = "?";
            Vals[1].text = "?";
        }
    }

    public void RollAttack()
    {
        Atk = 1 + GameControl.singleton.RNG.Next(20);
    }

    public void RollDef()
    {
        Def = 1 + GameControl.singleton.RNG.Next(20);
    }

    public void OnMouseDown()
    {
        if (PlayerOwned)
        {
            ShipControl.singleton.ZoneIndex = Zone.ID;
            ShipControl.singleton.ActiveUnit = this;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public void ShowVals()
    {
        Vals[0].text = Atk.ToString();
        Vals[1].text = Def.ToString();
    }

    public void ToggleView()
    {
        if (visible)
        {
            transform.position = new Vector3(-100, -100, -100);
            transform.SetParent(null);
        }
        else
            transform.position = startPos;
        visible = !visible;

    }

    public void MakeSave(int DC)
    {
        int r = GameControl.singleton.RNG.Next(20) + 1;
        if (r < DC)
            Def = 0;
    }

    // Use this for initialization
    void Start () {
        Vals = new UnityEngine.UI.Text[2];
        Vals[0] = transform.GetChild(2).GetChild(0).GetComponent<UnityEngine.UI.Text>();
        Vals[1] = transform.GetChild(2).GetChild(1).GetComponent<UnityEngine.UI.Text>();
        startPos = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
