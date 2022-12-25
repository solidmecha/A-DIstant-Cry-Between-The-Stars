using UnityEngine;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {
    public bool[] LivingLeads;
    public int ShipIndex;
    public int UnitIndex;

    public void TakeTurn()
    {
        int p = GameControl.singleton.RNG.Next(24);
        if(GameControl.singleton.Places[p].UnitCount[GameControl.singleton.UnitIndex]+ GameControl.singleton.Places[p].UnitCount[GameControl.singleton.UnitIndex+1]
            + GameControl.singleton.Places[p].UnitCount[GameControl.singleton.UnitIndex+2]>0 || GameControl.singleton.Places[p].ShipCount[GameControl.singleton.ShipIndex]>0)
            p= GameControl.singleton.RNG.Next(24);
        if (GameControl.singleton.Places[p].UnitCount[GameControl.singleton.UnitIndex] + GameControl.singleton.Places[p].UnitCount[GameControl.singleton.UnitIndex + 1]
            + GameControl.singleton.Places[p].UnitCount[GameControl.singleton.UnitIndex + 2] > 0 || GameControl.singleton.Places[p].ShipCount[GameControl.singleton.ShipIndex] > 0)
            p = GameControl.singleton.RNG.Next(24);
        if (GameControl.singleton.Places[p].UnitCount[GameControl.singleton.UnitIndex] + GameControl.singleton.Places[p].UnitCount[GameControl.singleton.UnitIndex + 1]
            + GameControl.singleton.Places[p].UnitCount[GameControl.singleton.UnitIndex + 2] > 0 || GameControl.singleton.Places[p].ShipCount[GameControl.singleton.ShipIndex] > 0)
            p = GameControl.singleton.RNG.Next(24);
        if (GameControl.singleton.Places[p].UnitCount[GameControl.singleton.UnitIndex] + GameControl.singleton.Places[p].UnitCount[GameControl.singleton.UnitIndex + 1]
            + GameControl.singleton.Places[p].UnitCount[GameControl.singleton.UnitIndex + 2] > 0 || GameControl.singleton.Places[p].ShipCount[GameControl.singleton.ShipIndex] > 0)
            p = GameControl.singleton.RNG.Next(24);
        if (GameControl.singleton.Places[p].UnitCount[GameControl.singleton.UnitIndex] + GameControl.singleton.Places[p].UnitCount[GameControl.singleton.UnitIndex + 1]
            + GameControl.singleton.Places[p].UnitCount[GameControl.singleton.UnitIndex + 2] > 0 || GameControl.singleton.Places[p].ShipCount[GameControl.singleton.ShipIndex] > 0)
            p = GameControl.singleton.RNG.Next(24);
        if (GameControl.singleton.Places[p].UnitCount[GameControl.singleton.UnitIndex] + GameControl.singleton.Places[p].UnitCount[GameControl.singleton.UnitIndex + 1]
            + GameControl.singleton.Places[p].UnitCount[GameControl.singleton.UnitIndex + 2] > 0 || GameControl.singleton.Places[p].ShipCount[GameControl.singleton.ShipIndex] > 0)
            { }
        else
        {
            switch (GameControl.singleton.RNG.Next(4))
            {
            case 0:
                if (GameControl.singleton.Places[p].ResourceID < 3)
                    Gather(GameControl.singleton.Places[p]);
                break;
            case 1:
                if (GameControl.singleton.Places[p].BuildingID < 2)
                    Build(GameControl.singleton.Places[p]);
                break;
            case 2:
                PlaceUnits(GameControl.singleton.Places[p]);
                break;
            case 3:
                Liberate(GameControl.singleton.Places[p]);
                break;
            }
        }
        GameControl.singleton.CurrentState = GameControl.GameState.PlayerTurn;
    }

    public void PlaceUnits(PlaceScript p)
    {
        if(GameControl.singleton.RegimePlayer)
        {
            p.UnitCount[0] = GameControl.singleton.RNG.Next(5);
            p.UnitCount[1]= GameControl.singleton.RNG.Next(4);
            p.ShipCount[0] = GameControl.singleton.RNG.Next(13);
        }
        else
        {
            p.UnitCount[3] = GameControl.singleton.RNG.Next(5);
            p.UnitCount[4] = GameControl.singleton.RNG.Next(4);
            p.ShipCount[1] = GameControl.singleton.RNG.Next(13);
        }
        print(p.name+ " placed");
    }

    public void Gather(PlaceScript p)
    {
        p.ResourceID += GameControl.singleton.RNG.Next(5, 10);
    }

    public void Build(PlaceScript p)
    {
        p.BuildingID += 2;
    }

    public void Liberate(PlaceScript p)
    {
        p.SetSigil(!GameControl.singleton.RegimePlayer);
    }

    public void Move()
    {
        GameControl.singleton.SelectedPlace.ShipCount[ShipIndex] += GameControl.singleton.StartPlace.ShipCount[ShipIndex];
        GameControl.singleton.StartPlace.ShipCount[ShipIndex] = 0;
        for (int i = 0; i < 3; i++)
        {
            GameControl.singleton.SelectedPlace.UnitCount[UnitIndex + i] += GameControl.singleton.StartPlace.UnitCount[UnitIndex + i];
            GameControl.singleton.StartPlace.UnitCount[UnitIndex + i] = 0;
        }
        if (GameControl.singleton.RegimePlayer && GameControl.singleton.SelectedPlace.UnitCount[2]==1)
        {
            GameControl.singleton.SelectedPlace.LeaderID = GameControl.singleton.StartPlace.LeaderID;
            GameControl.singleton.StartPlace.LeaderID = -1;
        }
        GameControl.singleton.SelectedPlace.SpaceBattleCheck(false);
    }

    public void CheckWin()
    {
        if (!(LivingLeads[0] || LivingLeads[1] || LivingLeads[2]))
        {
            GameControl.singleton.MsgText.text = "The Regime has won.";
            GameControl.singleton.CurrentState = GameControl.GameState.FX;
        }
        else
        {
            int loyalCount = 0;
            foreach (PlaceScript p in GameControl.singleton.Places)
            {
                if (!p.loyalRegime)
                    loyalCount++;
            }
            if (loyalCount > 12)
            {
                if (LivingLeads[0])
                {
                    bool hasElite = false;
                    foreach(PlaceScript p in GameControl.singleton.Places)
                    {
                        if(p.UnitCount[5]==1)
                        {
                            hasElite = true;
                            break;
                        }
                    }
                    if(!hasElite)
                    {
                        Camera.main.transform.position = new Vector3(0, 0, -10);
                        GameControl.singleton.MsgText.text = "Revolutionary Military Victory";
                        GameControl.singleton.CurrentState = GameControl.GameState.FX;
                    }

                }
                if (LivingLeads[1])
                {
                    if (GameControl.singleton.Resources[0][0] > 25 && GameControl.singleton.Resources[0][1] > 25 && GameControl.singleton.Resources[0][2] > 25)
                    {
                        Camera.main.transform.position = new Vector3(0, 0, -10);
                        GameControl.singleton.MsgText.text = "Revolutionary Economic Victory";
                        GameControl.singleton.CurrentState = GameControl.GameState.FX;
                    }
                }
                if (LivingLeads[2])
                {
                    if (loyalCount == 24)
                    {
                        Camera.main.transform.position = new Vector3(0, 0, -10);
                        GameControl.singleton.MsgText.text = "Revolutionary Diplomatic Victory";
                        GameControl.singleton.CurrentState = GameControl.GameState.FX;
                    }

                }
            }
        }
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

    }
}
