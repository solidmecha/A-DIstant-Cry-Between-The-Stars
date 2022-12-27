using UnityEngine;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {
    public bool[] LivingLeads;
    public int ShipIndex;
    public int UnitIndex;

    public void TakeTurn()
    {
        List<int> PlayerFreeIndex = new List<int>();
        for (int p = 0; p < 24; p++)
        {
            if (GameControl.singleton.Places[p].UnitCount[GameControl.singleton.UnitIndex] + GameControl.singleton.Places[p].UnitCount[GameControl.singleton.UnitIndex + 1]
            + GameControl.singleton.Places[p].UnitCount[GameControl.singleton.UnitIndex + 2] == 0 && GameControl.singleton.Places[p].ShipCount[GameControl.singleton.ShipIndex] == 0)
                PlayerFreeIndex.Add(p);
        }
        if (PlayerFreeIndex.Count > 0)
        {
            int p = PlayerFreeIndex[GameControl.singleton.RNG.Next(PlayerFreeIndex.Count)];
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
            if (GameControl.singleton.RegimePlayer)
            {
                if (GameControl.singleton.RNG.Next(3) == 1)
                {
                    int c = GameControl.singleton.RNG.Next(1, 5);
                    for (int i = 0; i < c; i++)
                    {
                        GameControl.singleton.Resources[0][0] += GameControl.singleton.RNG.Next(5);
                        GameControl.singleton.Resources[0][1] += GameControl.singleton.RNG.Next(5);
                        GameControl.singleton.Resources[0][2] += GameControl.singleton.RNG.Next(5);
                        Liberate(GameControl.singleton.Places[PlayerFreeIndex[GameControl.singleton.RNG.Next(PlayerFreeIndex.Count)]]);
                        CheckWin();
                    }
                    PlaceUnits(GameControl.singleton.Places[PlayerFreeIndex[GameControl.singleton.RNG.Next(PlayerFreeIndex.Count)]]);
                    AttackMove();
                }
            }
            else
            {
                if (GameControl.singleton.RNG.Next(5) == 1)
                {
                    AttackLeads();
                    CheckWin();
                }
            }
        }
        GameControl.singleton.CurrentState = GameControl.GameState.PlayerTurn;
    }

    public void AttackLeads()
    {
        List<int> PlayerFreeIndex = new List<int> { };
        List<int> LeaderLoc = new List<int> { };
        for (int i = 0; i < 24; i++)
        {
            if (GameControl.singleton.Places[i].HasPlayerLead())
                LeaderLoc.Add(i);
            else if(GameControl.singleton.Places[i].ShipCount[ShipIndex]>0 && 
                GameControl.singleton.Places[i].UnitCount[UnitIndex]+ GameControl.singleton.Places[i].UnitCount[UnitIndex+1]+GameControl.singleton.Places[i].UnitCount[UnitIndex+2] > 0)
            {
                PlayerFreeIndex.Add(i);
            }
        }
        if (PlayerFreeIndex.Count > 0 && LeaderLoc.Count > 0)
        {
            GameControl.singleton.StartPlace = GameControl.singleton.Places[PlayerFreeIndex[GameControl.singleton.RNG.Next(PlayerFreeIndex.Count)]];
            GameControl.singleton.SelectedPlace = GameControl.singleton.Places[LeaderLoc[GameControl.singleton.RNG.Next(LeaderLoc.Count)]];
            Move();
        }
    }

    public void AttackMove()
    {
        List<int> PlayerFreeIndex = new List<int> { };
        List<int> PlayerLoc = new List<int> { };
        for (int i = 0; i < 24; i++)
        {
            if (GameControl.singleton.Places[i].ShipCount[GameControl.singleton.ShipIndex] > 0 &&
                GameControl.singleton.Places[i].UnitCount[GameControl.singleton.UnitIndex] + GameControl.singleton.Places[i].UnitCount[GameControl.singleton.UnitIndex + 1] + GameControl.singleton.Places[i].UnitCount[GameControl.singleton.UnitIndex + 2] > 0)
            {
                PlayerLoc.Add(i);
            }
            else if (GameControl.singleton.Places[i].ShipCount[ShipIndex] > 0 &&
                GameControl.singleton.Places[i].UnitCount[UnitIndex] + GameControl.singleton.Places[i].UnitCount[UnitIndex + 1] + GameControl.singleton.Places[i].UnitCount[UnitIndex + 2] > 0)
            {
                PlayerFreeIndex.Add(i);
            }
        }
        if (PlayerFreeIndex.Count > 0 && PlayerLoc.Count > 0)
        {
            GameControl.singleton.StartPlace = GameControl.singleton.Places[PlayerFreeIndex[GameControl.singleton.RNG.Next(PlayerFreeIndex.Count)]];
            GameControl.singleton.SelectedPlace = GameControl.singleton.Places[PlayerLoc[GameControl.singleton.RNG.Next(PlayerLoc.Count)]];
            Move();
        }
    }

    public void PlaceUnits(PlaceScript p)
    {
        if(GameControl.singleton.RegimePlayer)
        {
            p.UnitCount[0] = GameControl.singleton.RNG.Next(5);
            p.UnitCount[1]= GameControl.singleton.RNG.Next(5);
            p.ShipCount[0] = GameControl.singleton.RNG.Next(5);
        }
        else
        {
            p.UnitCount[3] = GameControl.singleton.RNG.Next(5);
            p.UnitCount[4] = GameControl.singleton.RNG.Next(5);
            p.ShipCount[1] = GameControl.singleton.RNG.Next(5);
        }
        //print(p.name+ " placed");
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
