using UnityEngine;
using System.Collections;

public class PlaceScript : MonoBehaviour {

    public bool loyalRegime;
    public bool hasTech;
    public int TechID;
    public int ResourceID;
    public int BuildingID;
    public int[] UnitCount;
    public int[] ShipCount;
    public int TechCost;
    public GameObject CurrentBoard;
    public int LeaderID;

    public void setUnitSprites()
    {
        if(GameControl.singleton.RegimePlayer)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = GameControl.singleton.UnitSprites[4];
            transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = GameControl.singleton.UnitSprites[6];
            transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = GameControl.singleton.UnitSprites[5];
            transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = GameControl.singleton.UnitSprites[7];
        }
        else
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = GameControl.singleton.UnitSprites[0];
            transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = GameControl.singleton.UnitSprites[2];
            transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = GameControl.singleton.UnitSprites[1];
            transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = GameControl.singleton.UnitSprites[3];
        }
    }

    public void ShowCurrentView()
    {
        switch(GameControl.singleton.CurrentView)
        {
            case GameControl.ViewState.Unit:
                ShowUnits();
                break;
            case GameControl.ViewState.Building:
                ShowBuilding();
                break;
            case GameControl.ViewState.Res:
                ShowResource();
                break;
        }
    }

    public void ShowUnits()
    {
        int index = 0;
        if (GameControl.singleton.RegimePlayer)
        {
            index += 3;
            transform.GetChild(4).GetComponent<SpriteRenderer>().enabled = ShipCount[1] > 0;
        }
        else
            transform.GetChild(4).GetComponent<SpriteRenderer>().enabled = ShipCount[0] > 0;
        transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = UnitCount[index]>0;
        transform.GetChild(2).GetComponent<SpriteRenderer>().enabled = UnitCount[index + 2] > 0;
        transform.GetChild(3).GetComponent<SpriteRenderer>().enabled = UnitCount[index+1] > 0;
        
    }

    public void Tick()
    {
        if (ResourceID > 2)
        {
            ResourceID--;
            if (ResourceID == 2)
                SetResource();
            ShowCurrentView();
        }
        if(BuildingID>1)
        {
            BuildingID--;
            if (BuildingID == 1)
                SetBuilding();
            ShowCurrentView();
        }
    }

    public void HideUnits()
    {
        transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = false;
        transform.GetChild(2).GetComponent<SpriteRenderer>().enabled = false;
        transform.GetChild(3).GetComponent<SpriteRenderer>().enabled = false;
        transform.GetChild(4).GetComponent<SpriteRenderer>().enabled = false;
    }

    public void HideResourceBuilding()
    {
        transform.GetChild(5).GetComponent<SpriteRenderer>().enabled = false;
    }

    public void ShowResource()
    {
        transform.GetChild(5).GetComponent<SpriteRenderer>().enabled = true;
        transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = GameControl.singleton.ResourceSprites[ResourceID];
    }

    public void ShowBuilding()
    {
        transform.GetChild(5).GetComponent<SpriteRenderer>().enabled = true;
        transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = GameControl.singleton.BuildingSprites[BuildingID];
    }

    public void HideTech()
    {
        if (CurrentBoard != null)
            Destroy(CurrentBoard);
    }

    public void SetSigil(bool Regime)
    {
        loyalRegime = Regime;
        if (Regime)
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = GameControl.singleton.Sigils[1];
        else
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = GameControl.singleton.Sigils[0];
    }

    public bool HasPlayerLead()
    {
        return (GameControl.singleton.RegimePlayer && UnitCount[5] == 1) || (!GameControl.singleton.RegimePlayer && UnitCount[2] == 1);
    }

    public void SetBuilding()
    {
        BuildingID = GameControl.singleton.RNG.Next(2);
    }

    public void SetResource()
    {
        ResourceID = GameControl.singleton.RNG.Next(3);
    }

    private void OnMouseDown()
    {
        if (GameControl.singleton.CurrentState==GameControl.GameState.PlayerTurn)
        {
            GameControl.singleton.SelectedPlace = this;
            GameControl.singleton.PlayerActionPanel.transform.position = transform.GetChild(6).position;
            GameControl.singleton.CurrentState= GameControl.GameState.Menu;
        }
        else if(GameControl.singleton.CurrentState == GameControl.GameState.Move && ShipCount[GameControl.singleton.ShipIndex]>0)
        {
            GameControl.singleton.CurrentState = GameControl.GameState.FX;
            GameControl.singleton.StartPlace = this;
            GameControl.singleton.MoveSlider.transform.position = Vector3.zero;
            GameControl.singleton.MoveSlider.GetComponentInChildren<UnitSelectHelper>().SetMax(new int[4]{ShipCount[GameControl.singleton.ShipIndex], UnitCount[GameControl.singleton.UnitIndex]
                , UnitCount[GameControl.singleton.UnitIndex+1], UnitCount[GameControl.singleton.UnitIndex+2] });
        }
    }

    public void SpaceBattleCheck(bool Player)
    {
        if (Player)
        {
            if (ShipCount[0] > 0 && ShipCount[1] > 0)
            {
                ShipControl.singleton.BeginFleet(name, (UnitCount[0] + UnitCount[1] + UnitCount[2] > 0 && UnitCount[3] + UnitCount[4] + UnitCount[5] > 0), GameControl.singleton.UnitIndex == 3);
            }
            else if(UnitCount[0] + UnitCount[1] + UnitCount[2] > 0 && UnitCount[3] + UnitCount[4] + UnitCount[5] > 0)
            {
                BattleControl.singleton.SetUpBattle(GameControl.singleton.SelectedPlace.name);
            }
        }
        else
        {
            if (ShipCount[0] > 0 && ShipCount[1] > 0)
            {
                ShipControl.singleton.BeginFleet(name, (UnitCount[0] + UnitCount[1] + UnitCount[2] > 0 && UnitCount[3] + UnitCount[4] + UnitCount[5] > 0), GameControl.singleton.UnitIndex != 3);
            }
            else if (UnitCount[0] + UnitCount[1] + UnitCount[2] > 0 && UnitCount[3] + UnitCount[4] + UnitCount[5] > 0)
            {
                BattleControl.singleton.SetUpBattle(GameControl.singleton.SelectedPlace.name);
            }
        }
    }

    public void StartGroundBattle()
    {
        if (UnitCount[0] + UnitCount[1] + UnitCount[2] > 0 && UnitCount[3] + UnitCount[4] + UnitCount[5] > 0)
        {
            BattleControl.singleton.SetUpBattle(name);
        }
    }

    private void OnMouseEnter()
    {
        if (GameControl.singleton.CurrentState == GameControl.GameState.PlayerTurn)
        {
            if (GameControl.singleton.RegimePlayer)
            {
                if (ShipCount[1] > 0 || UnitCount[3]+ UnitCount[4]+ UnitCount[5]>0)
                GameControl.singleton.MsgText.text = ShipCount[1].ToString() + " Ships, " + UnitCount[3].ToString() + " Conscripts, "
                    + UnitCount[4].ToString() + " Tanks, " + UnitCount[5].ToString() + " Elites";
                else
                    GameControl.singleton.MsgText.text = "?? Ships, "+ "?? Militia, "
                   +"?? Mechs, " + "?? Leaders";
            }
            else
            {
                if (ShipCount[0] > 0 || UnitCount[0] + UnitCount[1] + UnitCount[2] > 0)
                {
                    string s = " Leaders";
                    if (UnitCount[2] == 1)
                    {
                        if (LeaderID == 0)
                            s = " General";
                        else if (LeaderID == 6)
                            s = " Merchant";
                        else if (LeaderID == 7)
                            s = " Diplomat";
                    }
                    GameControl.singleton.MsgText.text = ShipCount[0].ToString() + " Ships, " + UnitCount[0].ToString() + " Militia, "
                   + UnitCount[1].ToString() + " Mechs, " + UnitCount[2].ToString() + s;
                }
                else
                {
                    GameControl.singleton.MsgText.text = "?? Ships, " + "?? Conscripts, "
                   + "?? Tanks, " + "?? Elites";
                }
            }
        }
    }

    // Use this for initialization
    void Start () {
        SetBuilding();
        SetResource();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
