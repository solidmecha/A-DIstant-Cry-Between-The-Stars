using UnityEngine;
using System.Collections;

public class TechScript : MonoBehaviour {

    public void Use(int ID)
    {
        switch(ID)
        {
            case 0:
                if(GameControl.singleton.SelectedPlace.ShipCount[0]>0 && GameControl.singleton.SelectedPlace.ShipCount[1] > 0)
                {
                    foreach (ShipScript s in ShipControl.singleton.Ships)
                        s.ShowVals();
                }
                else
                {
                    foreach (UnitScript u in BattleControl.singleton.Units)
                        u.ShowVals();
                }
                break;
            case 1:
                if (GameControl.singleton.SelectedPlace.ShipCount[0] > 0 && GameControl.singleton.SelectedPlace.ShipCount[1] > 0)
                {
                    foreach (ShipScript s in ShipControl.singleton.Ships)
                    {
                        if (s.PlayerOwned)
                        {
                            s.Atk += 2;
                            s.ShowVals();
                        }
                    }
                    foreach (ZoneScript z in ShipControl.singleton.Zones)
                        z.CalcValue();
                }
                else
                {
                    foreach (UnitScript u in BattleControl.singleton.Units)
                    {
                        if (u.PlayerOwned)
                        {
                            u.Atk += 2;
                            u.ShowVals();
                        }
                    }
                }
                break;
            case 2:
                if (GameControl.singleton.SelectedPlace.ShipCount[0] > 0 && GameControl.singleton.SelectedPlace.ShipCount[1] > 0)
                {
                    foreach (ShipScript s in ShipControl.singleton.Ships)
                    {
                        if (s.PlayerOwned)
                        {
                            s.Def += 2;
                            s.ShowVals();
                        }
                    }
                    foreach (ZoneScript z in ShipControl.singleton.Zones)
                        z.CalcValue();
                }
                else
                {
                    foreach (UnitScript u in BattleControl.singleton.Units)
                    {
                        if (u.PlayerOwned)
                        {
                            u.Def += 2;
                            u.ShowVals();
                        }
                    }
                }
                break;
            case 3:
                if (GameControl.singleton.SelectedPlace.ShipCount[0] > 0 && GameControl.singleton.SelectedPlace.ShipCount[1] > 0)
                {
                    foreach (ShipScript s in ShipControl.singleton.Ships)
                    {
                        if (s.PlayerOwned)
                        {
                            s.RollAttack();
                            s.ShowVals();
                        }
                    }
                    foreach (ZoneScript z in ShipControl.singleton.Zones)
                        z.CalcValue();
                }
                else
                {
                    foreach (UnitScript u in BattleControl.singleton.Units)
                    {
                        if (u.PlayerOwned)
                        {
                            int d = u.Def;
                            u.Roll();
                            u.Def = d;
                        }
                    }
                }
                break;
            case 4:
                if (GameControl.singleton.SelectedPlace.ShipCount[0] > 0 && GameControl.singleton.SelectedPlace.ShipCount[1] > 0)
                {
                    foreach (ShipScript s in ShipControl.singleton.Ships)
                    {
                        if (s.PlayerOwned)
                        {
                            s.RollDef();
                            s.ShowVals();
                        }
                    }
                    foreach (ZoneScript z in ShipControl.singleton.Zones)
                        z.CalcValue();
                }
                else
                {
                    foreach (UnitScript u in BattleControl.singleton.Units)
                    {
                        if (u.PlayerOwned)
                        {
                            int a = u.Atk;
                            u.Roll();
                            u.Atk = a;
                        }
                    }
                }
                break;
            case 5:
                if (GameControl.singleton.SelectedPlace.ShipCount[0] > 0 && GameControl.singleton.SelectedPlace.ShipCount[1] > 0)
                {
                    foreach (ShipScript s in ShipControl.singleton.Ships)
                    {

                        s.Atk = 21-s.Atk;
                        s.Def = 21-s.Def;
                        if (!s.Vals[0].Equals("?"))
                            s.ShowVals();
                    }
                    foreach (ZoneScript z in ShipControl.singleton.Zones)
                        z.CalcValue();
                }
                else
                {
                    foreach (UnitScript u in BattleControl.singleton.Units)
                    {
                        u.Atk = GameControl.singleton.UnitMax[u.ID] - u.Atk;
                            u.Def =GameControl.singleton.UnitMax[u.ID]-u.Def;
                        if (!u.Vals[0].Equals("?"))
                            u.ShowVals();
                    }
                }
                break;
            case 6:
                if (GameControl.singleton.SelectedPlace.ShipCount[0] > 0 && GameControl.singleton.SelectedPlace.ShipCount[1] > 0)
                {
                    if (ShipControl.singleton.ShipCounts[0] > ShipControl.singleton.ShipCounts[1] && GameControl.singleton.ShipIndex == 1)
                    {
                        ShipControl.singleton.ShipCounts[0]--;
                        ShipControl.singleton.ShipCountText[0].text = ShipControl.singleton.ShipCounts[0].ToString();
                    }
                    else if(ShipControl.singleton.ShipCounts[0] < ShipControl.singleton.ShipCounts[1] && GameControl.singleton.ShipIndex == 0)
                    {
                        ShipControl.singleton.ShipCounts[1]--;
                        ShipControl.singleton.ShipCountText[1].text = ShipControl.singleton.ShipCounts[1].ToString();
                    }
                }
                else
                {
                    if (BattleControl.singleton.UnitCounts[1] > BattleControl.singleton.UnitCounts[4] && GameControl.singleton.UnitIndex == 3)
                    {
                        BattleControl.singleton.UnitCounts[1]--;
                        BattleControl.singleton.CountText[1].text= BattleControl.singleton.UnitCounts[1].ToString();
                    }
                    else if (BattleControl.singleton.UnitCounts[1] < BattleControl.singleton.UnitCounts[4] && GameControl.singleton.UnitIndex == 0)
                    {
                        BattleControl.singleton.UnitCounts[4]--;
                        BattleControl.singleton.CountText[4].text = BattleControl.singleton.UnitCounts[4].ToString();
                    }
                }
                break;
            case 7:
                if (GameControl.singleton.SelectedPlace.ShipCount[0] > 0 && GameControl.singleton.SelectedPlace.ShipCount[1] > 0)
                {
                    ShipControl.singleton.ShipCounts[GameControl.singleton.ShipIndex]++;
                    ShipControl.singleton.ShipCountText[GameControl.singleton.ShipIndex].text = ShipControl.singleton.ShipCounts[GameControl.singleton.ShipIndex].ToString();
                }
                else
                {
                    int r = GameControl.singleton.RNG.Next(2);
                    BattleControl.singleton.UnitCounts[GameControl.singleton.UnitIndex+r]++;
                    BattleControl.singleton.CountText[GameControl.singleton.UnitIndex+r].text = BattleControl.singleton.UnitCounts[GameControl.singleton.UnitIndex+r].ToString();
                }
                break;
            case 8:
                if (GameControl.singleton.SelectedPlace.ShipCount[0] > 0 && GameControl.singleton.SelectedPlace.ShipCount[1] > 0)
                {
                    foreach (ShipScript s in ShipControl.singleton.Ships)
                    {
                        if (s.PlayerOwned)
                        {
                            s.RollAttack();
                            s.RollDef();
                            s.ShowVals();
                        }
                    }
                    foreach (ZoneScript z in ShipControl.singleton.Zones)
                        z.CalcValue();
                }
                else
                {
                    foreach (UnitScript u in BattleControl.singleton.Units)
                    {
                        if (u.PlayerOwned)
                        {
                            u.Roll();
                        }
                    }
                }
                break;

        }
    }

    public string Description(int ID)
    {
        switch(ID)
        {
            case 0: return "Reveal Enemy Roll";
            case 1:
                return "+2 to Attacks";
            case 2:
                return  "+2 to Defenses";
            case 3: return "Reroll Attacks";
            case 4: return "Reroll Defenses";
            case 5:
                return "Invert all rolls, the lower the better";
            case 6:
                return "Destroy constructed enemy unit at end of round if behind on that unit";
            case 7:
                return "Gain a unit for next round";
            case 8:
                return "Reroll Attacks and Defenses";
            default:
                return "";
        }
    }


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
