using UnityEngine;
using System.Collections.Generic;

public class ShipAI : MonoBehaviour {

    public List<ShipScript> Ships;
    public int AtkZone;
    public int DefZone;

    public void MoveShips()
    {
        if (Ships.Count > 1)
        {
            List<int> ia= new List<int>();
            List<int> idef = new List<int>();
            for(int i=0;i<Ships.Count;i++)
            {
                ia.Add(Ships[i].Atk);
                idef.Add(Ships[i].Def);
            }
            ia.Sort();
            idef.Sort();
            ia.RemoveRange(0, Ships.Count/2);
            idef.RemoveRange(0, Ships.Count/2);
            List<int> AtkIndex = new List<int>();
            List<int> DefIndex = new List<int>();
            List<int> NumsA = new List<int>();
            List<int> NumsD = new List<int>();
            for (int i = 0; i < Ships.Count; i++)
            {
                NumsA.Add(i);
                NumsD.Add(i);
                Ships[i].ShowVals();
                if (ia.Contains(Ships[i].Atk))
                    AtkIndex.Add(i);
                if (idef.Contains(Ships[i].Def))
                    DefIndex.Add(i);
            }
            int DefValLeft = 0;
            int AtkValLeft = 0;
            int DefMax = 0; ;
            int AtkMax = 0;

            for (int i = 0; i < Ships.Count/2; i++)
            {
                NumsA.Remove(DefIndex[i]);
                NumsD.Remove(AtkIndex[i]);
            }
            for (int i = 0; i < Ships.Count/2; i++)
            {
                DefMax += Ships[DefIndex[i]].Def;
                AtkMax += Ships[AtkIndex[i]].Atk;
                AtkValLeft += Ships[NumsA[i]].Atk;
                DefValLeft += Ships[NumsD[i]].Def;
            }
            if (AtkValLeft >=Ships.Count*11/2 || DefValLeft >= Ships.Count * 11 / 2)
            {
                if (AtkMax > Ships.Count*17/2)
                {
                    for (int i = 0; i < Ships.Count/2; i++)
                    {
                        ShipControl.singleton.Zones[DefZone].PlaceShip(Ships[NumsD[i]]);
                        ShipControl.singleton.Zones[AtkZone].PlaceShip(Ships[AtkIndex[i]]);
                    }
                }
                else if (DefMax > Ships.Count*17/2)
                {
                    for (int i = 0; i < Ships.Count/2; i++)
                    {
                        ShipControl.singleton.Zones[DefZone].PlaceShip(Ships[DefIndex[i]]);
                        ShipControl.singleton.Zones[AtkZone].PlaceShip(Ships[NumsA[i]]);
                    }
                }
                else if (AtkValLeft >= DefValLeft)
                {
                    for (int i = 0; i < Ships.Count/2; i++)
                    {
                        ShipControl.singleton.Zones[DefZone].PlaceShip(Ships[DefIndex[i]]);
                        ShipControl.singleton.Zones[AtkZone].PlaceShip(Ships[NumsA[i]]);
                    }
                }
                else
                {
                    for (int i = 0; i < Ships.Count/2; i++)
                    {
                        ShipControl.singleton.Zones[DefZone].PlaceShip(Ships[NumsD[i]]);
                        ShipControl.singleton.Zones[AtkZone].PlaceShip(Ships[AtkIndex[i]]);
                    }
                }
            }
            else
            {
                if (DefMax >= AtkMax)
                {
                    for (int i = 0; i < Ships.Count/2; i++)
                    {
                        ShipControl.singleton.Zones[DefZone].PlaceShip(Ships[DefIndex[i]]);
                        ShipControl.singleton.Zones[AtkZone].PlaceShip(Ships[NumsA[i]]);
                    }
                }
                else
                {
                    for (int i = 0; i < Ships.Count/2; i++)
                    {
                        ShipControl.singleton.Zones[DefZone].PlaceShip(Ships[NumsD[i]]);
                        ShipControl.singleton.Zones[AtkZone].PlaceShip(Ships[AtkIndex[i]]);
                    }
                }
            }
        }
        else
        {
            if(Ships[0].Atk>Ships[0].Def)
            {
                ShipControl.singleton.Zones[AtkZone].PlaceShip(Ships[0]);
            }
            else
                ShipControl.singleton.Zones[DefZone].PlaceShip(Ships[0]);
        }
    }

    public void CleanUp()
    {
        for(int i=Ships.Count-1;i>=0;i--)
        {
            if (!Ships[i].visible)
                Ships.RemoveAt(i);
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
