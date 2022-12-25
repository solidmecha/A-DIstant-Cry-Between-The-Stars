using UnityEngine;
using System.Collections.Generic;

public class ShipControl : MonoBehaviour {
    public static ShipControl singleton;
    public List<ShipScript> Ships;
    public int[] ShipCounts;
    public ShipScript ActiveUnit;
    public bool inBattle;
    public int[] MaxShipCounts;
    public ZoneScript[] Zones;
    public int ZoneIndex;
    public UnityEngine.UI.Text[] ShipCountText;
    public UnityEngine.UI.Button PhaseButton;
    public bool GroundBattle;
    public bool RegimeInvade;
    public UnityEngine.UI.Text MsgText;

    private void Awake()
    {
        singleton = this;
    }

    // Use this for initialization
    void Start () {
        //SetAIShips();
        ShipCounts = new int[2];
	}

    public void SetAIShips()
    {
        for (int i = 0; i < 4; i++)
        {
            Zones[i].PlayerOwned = (i < 2 && !GameControl.singleton.RegimePlayer) || (i > 1 && GameControl.singleton.RegimePlayer); 
        }
        Zones[5].PlayerOwned = !GameControl.singleton.RegimePlayer;
        Zones[4].PlayerOwned = GameControl.singleton.RegimePlayer;
        for(int i=0;i<16;i++)
        {
            Ships[i].PlayerOwned = (Ships[i].ID == 0 && !GameControl.singleton.RegimePlayer) || (Ships[i].ID == 1 && GameControl.singleton.RegimePlayer);
            if (!Ships[i].PlayerOwned)
                GetComponent<ShipAI>().Ships.Add(Ships[i]);
        }
        if(!GameControl.singleton.RegimePlayer)
        {
            GetComponent<ShipAI>().AtkZone = 3;
            GetComponent<ShipAI>().DefZone = 2;
        }
        if (GameControl.singleton.RegimePlayer)
        {
            GetComponent<ShipAI>().AtkZone = 0;
            GetComponent<ShipAI>().DefZone = 1;
        }
    }

    public void BeginFleet(string n, bool groundBattle, bool regimeInvade)
    {
        MsgText.text = n + " Battle";
        Camera.main.transform.position = new Vector3(0, -25, -10);
        GroundBattle = groundBattle;
        RegimeInvade = regimeInvade;
        ShipCounts[0] = GameControl.singleton.SelectedPlace.ShipCount[0];
        ShipCounts[1] = GameControl.singleton.SelectedPlace.ShipCount[1];
        ShowShips();
        GameControl.singleton.TechText[0].transform.root.position = new Vector2(11.8f, -29);
        GameControl.singleton.ShowBattleTech();
    }

    public void FleetRound()
    {
        inBattle = true;
        for (int i = 0; i < 4; i++)
            Zones[i].Count = 0;
        foreach(ShipScript s in Ships)
        {
            if (s.visible)
                s.Roll();
        }
    }

    public void AdvancePhase()
    {
        if (FullZones())
        {
            inBattle = false;
            GetComponent<ShipAI>().MoveShips();
            PhaseButton.onClick.RemoveAllListeners();
            PhaseButton.onClick.AddListener(delegate { ResolveZones(); });
        }
    }

    public bool FullZones()
    {
        int a = 0;
        int b = 1;
        if(Zones[2].PlayerOwned)
        {
            a = 2;
            b = 3;
        }
        if(ShipCounts[GameControl.singleton.ShipIndex]>1)
        {
            return Zones[a].Count == MaxShipCounts[a] && Zones[b].Count == MaxShipCounts[b];
        }
        else
        {
            return Zones[a].Count == MaxShipCounts[a] || Zones[b].Count == MaxShipCounts[b];
        }
    }

    public void ResolveZones()
    {
        for (int i = 0; i < 2; i++)
        {
            if (Zones[i].Value > 0 && Zones[i + 2].Value > 0)
            {
                int diff = Zones[i].Value - Zones[i + 2].Value;
                if (diff > 0)
                {
                    Zones[i + 2].HandleLoss();
                    Zones[i].HandleWin(diff);
                }
                else if (diff < 0)
                {
                    Zones[i].HandleLoss();
                    Zones[i + 2].HandleWin(-1 * diff);
                }
                else
                {
                    Zones[i].HandleWin(diff);
                    Zones[i + 2].HandleWin(diff);
                }
            }
        }

       ShowShips();
    }

    public void ShowShips()
    {
        foreach(ShipScript s in Ships)
        {
            if(s.visible && s.Def==0)
            {
                ShipCounts[s.ID]--;
            }
        }
        ShipCountText[0].text = ShipCounts[0].ToString();
        ShipCountText[1].text = ShipCounts[1].ToString();
        if (ShipCounts[0] == 0 || ShipCounts[1] == 0)
        {
            EndFight();
        }
        else
        {
            SetVisibleShips();
            foreach(ShipScript s in Ships)
            {
                if (s.visible && s.ID == 0)
                    Zones[5].PlaceShip(s);
                else if (s.visible)
                    Zones[4].PlaceShip(s);
                    
            }
            GetComponent<ShipAI>().CleanUp();
            PhaseButton.onClick.RemoveAllListeners();
            PhaseButton.onClick.AddListener(delegate { AdvancePhase(); });
            for (int i = 0; i < 4; i++)
                Zones[i].CalcValue();
            FleetRound();
        }

    }

    public void EndFight()
    {
        PhaseButton.onClick.RemoveAllListeners();
        PhaseButton.onClick.AddListener(delegate { LastStep(); });
    }

    public void LastStep()
    {
        foreach (ShipScript s in Ships)
        {
            s.Def = 1;
            if (!s.visible)
                s.ToggleView();
        }
        GetComponent<ShipAI>().Ships.Clear();
        SetAIShips();
        MaxShipCounts[0] = 4;
        MaxShipCounts[1] = 4;
        MaxShipCounts[2] = 4;
        MaxShipCounts[3] = 4;
        MaxShipCounts[4] = 8;
        MaxShipCounts[5] = 8;
        GameControl.singleton.SelectedPlace.ShipCount[0] = ShipCounts[0];
        GameControl.singleton.SelectedPlace.ShipCount[1] = ShipCounts[1];
        if (((RegimeInvade && ShipCounts[1] > 0) || (!RegimeInvade && ShipCounts[0] > 0)) && GroundBattle)
            BeginGround();
        else if (((RegimeInvade && ShipCounts[1] <= 0) || (!RegimeInvade && ShipCounts[0] <= 0)) && GroundBattle)
        {
            GameControl.singleton.TakeBackGround(RegimeInvade == GameControl.singleton.RegimePlayer);
            GameControl.singleton.ReturnToMap();
        }
        else
        {
            GameControl.singleton.ReturnToMap();
            PhaseButton.onClick.RemoveAllListeners();
            PhaseButton.onClick.AddListener(delegate { AdvancePhase(); });
        }
    }

   public void BeginGround()
    {
        GameControl.singleton.SelectedPlace.StartGroundBattle();
    }

    public void SetVisibleShips()
    {
        if((ShipCounts[0]<MaxShipCounts[0]*2 || ShipCounts[1] < MaxShipCounts[0]*2) && ShipCounts[0]>1 && ShipCounts[1]>1)
        {
            int t = 0;
            if (ShipCounts[0] < ShipCounts[1])
                t =2*(ShipCounts[0] / 2);
            else
                t =2*(ShipCounts[1] / 2);
            int vis1 = 0;
            int vis2 = 0;
            for(int i=0;i<8;  i++)
            {
                if(Ships[i].visible)
                {
                    vis1++;
                    if (vis1 > t)
                        Ships[i].ToggleView();
                }
                if(Ships[8+i].visible)
                {
                    vis2++;
                    if (vis2 > t)
                        Ships[8+i].ToggleView();
                }
            }
            for (int i = 0; i < 4; i++)
                MaxShipCounts[i] = t / 2;
        }
        else if((ShipCounts[0] == 1 || ShipCounts[1] == 1))
        {
            int t0 = Mathf.Clamp(ShipCounts[0],1, 2);
            int t1 = Mathf.Clamp(ShipCounts[1], 1, 2);
            int vis1 = 0;
            int vis2 = 0;
            for (int i = 0; i < 8; i++)
            {
                if (Ships[i].visible)
                {
                    vis1++;
                    if (vis1 > t0)
                        Ships[i].ToggleView();
                }
                if (Ships[8 + i].visible)
                {
                    vis2++;
                    if (vis2 > t1)
                        Ships[8+i].ToggleView();
                }
            }
            for (int i = 0; i < 4; i++)
                MaxShipCounts[i] = 1;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (inBattle && Input.GetMouseButton(0) && ActiveUnit != null)
            ActiveUnit.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        else if (inBattle && Input.GetMouseButtonUp(0) && ActiveUnit != null)
        {
            Zones[ZoneIndex].PlaceShip(ActiveUnit);
            ActiveUnit = null;
        }
	
	}
}
