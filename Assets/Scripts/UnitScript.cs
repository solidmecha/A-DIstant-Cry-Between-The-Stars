using UnityEngine;
using System.Collections;

public class UnitScript : MonoBehaviour {
    public UnityEngine.UI.Text[] Vals;
    public int Atk;
    public int Def;
    public int ID; //Revos, RevoMech, RevCom, RegConscript,RegArmor RegLead
    public bool PlayerOwned;
    public UnitScript Target;
    public bool visible;
    public Vector3 startPos;

    private void OnMouseDown()
    {
        if (PlayerOwned && BattleControl.singleton.phaseOne)
            BattleControl.singleton.activeUnit = this;
    }

    private void OnMouseOver()
    {
        if(Input.GetMouseButton(0) && !PlayerOwned && BattleControl.singleton.activeUnit!=null && BattleControl.singleton.phaseOne)
        {
            if (ID == 5)
            {
                if(BattleControl.singleton.UnitCounts[2]==0 && BattleControl.singleton.UnitCounts[3] == 0)
                {
                    BattleControl.singleton.activeUnit.Target = this;
                    BattleControl.singleton.activeUnit.ShowTarget();
                }
                else
                {
                    BattleControl.singleton.MsgText.text = "Eliminate All Other Units";
                }
            }
            else if(ID==2)
            {
                if (BattleControl.singleton.UnitCounts[0] == 0 && BattleControl.singleton.UnitCounts[1] == 0)
                {
                    BattleControl.singleton.activeUnit.Target = this;
                    BattleControl.singleton.activeUnit.ShowTarget();
                }
                else
                {
                    BattleControl.singleton.MsgText.text = "Eliminate All Other Units";
                }
            }
            else
            {
                BattleControl.singleton.activeUnit.Target = this;
                BattleControl.singleton.activeUnit.ShowTarget();
            }
        }
    }

    public void HitTarget()
    {
        if (Target != null)
        {
            Target.Def -= Atk;
            Target.ShowVals();
        }
    }

    public void Roll()
    {
        switch(ID)
        {
            case 0:
                Atk = 1+GameControl.singleton.RNG.Next(6);
                int a= 1 + GameControl.singleton.RNG.Next(6);
                if (a > Atk)
                    Atk = a;
                Def = 1 + GameControl.singleton.RNG.Next(6);
                int d = 1 + GameControl.singleton.RNG.Next(6);
                if (d > Def)
                    Def = d;
                break;
            case 1:
                Atk = 3 + GameControl.singleton.RNG.Next(8);
                Def = 3 + GameControl.singleton.RNG.Next(8);
                break;
            case 2:
                Atk = 1 + GameControl.singleton.RNG.Next(20);
                a = 1 + GameControl.singleton.RNG.Next(20);
                if (a > Atk)
                    Atk = a;
                Def = 1 + GameControl.singleton.RNG.Next(20);
                d = 1 + GameControl.singleton.RNG.Next(20);
                if (d > Def)
                    Def = d;
                break;
            case 3:
                Atk = 1 + GameControl.singleton.RNG.Next(8);
                Def = 1 + GameControl.singleton.RNG.Next(8);
                break;
            case 4:
                Atk = 1 + GameControl.singleton.RNG.Next(12);
                Def = 1 + GameControl.singleton.RNG.Next(12);
                break;
            case 5:
                Atk = 1 + GameControl.singleton.RNG.Next(24);
                Def = 1 + GameControl.singleton.RNG.Next(24);
                break;
        }
        if (PlayerOwned)
            ShowVals();
        else
        {
            Vals[0].text = "?";
            Vals[1].text = "?";
        }

    }

    public void ShowVals()
    {
        Vals[0].text = Atk.ToString();
        Vals[1].text = Def.ToString();
    }

    public void ShowTarget()
    {
        if(Target != null)
            GetComponent<LineRenderer>().SetPosition(1, Target.transform.position);
        else
            GetComponent<LineRenderer>().SetPosition(1, transform.position);
    }

    public void ToggleView()
    {
        if (visible)
        {
            transform.position = new Vector3(-0, -0, -100);
            GetComponent<LineRenderer>().SetPosition(0, transform.position);
            GetComponent<LineRenderer>().SetPosition(1, transform.position);
        }
        else
        {
            transform.position = startPos;
            GetComponent<LineRenderer>().SetPosition(0, transform.position);
            GetComponent<LineRenderer>().SetPosition(1, transform.position);
        }
        visible = !visible;
        
    }
    
	// Use this for initialization
	void Start () {
        Vals = new UnityEngine.UI.Text[2];
        Vals[0] = transform.GetChild(2).GetChild(0).GetComponent<UnityEngine.UI.Text>();
        Vals[1] = transform.GetChild(2).GetChild(1).GetComponent<UnityEngine.UI.Text>();
        startPos = transform.position;
        GetComponent<LineRenderer>().SetPosition(0, transform.position);
        GetComponent<LineRenderer>().SetPosition(1, transform.position);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
