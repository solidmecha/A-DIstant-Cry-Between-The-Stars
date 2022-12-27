using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
public class GameControl : MonoBehaviour {

    public static GameControl singleton;
    public System.Random RNG;
    public bool RegimePlayer;
    public int[][] Resources; //p1 fuel, metal, credits, p2 fmc
    public GameObject PlayerActionPanel;
    public Sprite[] UnitSprites; //0revos, 1mech, 2agents, 3squadron, 4conscript,5 tank,6 elite,7 super
    public Sprite[] ResourceSprites; //FMC
    public Sprite[] BuildingSprites; //Fact, Yard
    public Sprite[] Sigils;
    public enum GameState { PlayerTurn, Menu, Move, FX};
    public GameState CurrentState;
    public Text MsgText;
    public PlaceScript[] Places;
    public PlaceScript SelectedPlace;
    public PlaceScript StartPlace;
    bool[] allowedActions = new bool[8] { true, true, true, true, true, true, true, true };
    public GameObject MoveSlider;
    public int ShipIndex;
    public int UnitIndex;
    public int[] ShipCost;
    public int[] MechCost;
    public int[] InfantryCost;
    public Text[] ResourceText;
    public TechScript TechRef;
    public int[] UnitMax;
    public List<int> PlayerTech;
    public GameObject TechMsgBoard;
    public enum ViewState { Unit, Res, Building};
    public ViewState CurrentView;
    public Text[] TechText;
    public int TechIndex;
    public int IntelCost;
    public int SetBacks;
    public int[] LiberationCost;
    public PlayerControl PC;
    public Button CancelMove;

    private void Awake()
    {
        singleton = this;
        RNG = new System.Random();
        Resources = new int[2][];
        Resources[0] = new int[3];
        Resources[1] = new int[3];
    }

    public void StartGame()
    {
        if (RegimePlayer)
        {
            ShipIndex = 1;
            UnitIndex = 3;
            PC.ShipIndex = 0;
            PC.UnitIndex = 0;
            MoveSlider.transform.GetChild(0).GetChild(4).GetChild(2).GetChild(0).GetComponent<Image>().sprite = UnitSprites[7];
            MoveSlider.transform.GetChild(0).GetChild(5).GetChild(2).GetChild(0).GetComponent<Image>().sprite = UnitSprites[4];
            MoveSlider.transform.GetChild(0).GetChild(6).GetChild(2).GetChild(0).GetComponent<Image>().sprite = UnitSprites[5];
            MoveSlider.transform.GetChild(0).GetChild(7).GetChild(2).GetChild(0).GetComponent<Image>().sprite = UnitSprites[6];
        }

        CurrentState = GameState.PlayerTurn;
        ShipControl.singleton.SetAIShips();
        BattleControl.singleton.SetPlayerOwned();
        StartPlaces();
        UpdateMapUnits();
        Camera.main.transform.position = new Vector3(0, 0, -10);
        Resources[ShipIndex][0] = 3;
        Resources[ShipIndex][1] = 4;
        Resources[ShipIndex][2] = 10;
        UpdateResourceText();
        /*
         *         PC.TakeTurn();
        PC.TakeTurn();
        PC.TakeTurn();
        PC.TakeTurn();
        SelectedPlace = Places[0];
        Places[0].ShipCount[0] = 3;
        Places[0].ShipCount[1] = 9;
        ShipControl.singleton.BeginFleet("Test Battle", false, false);
        */
    }

    public void StartPlaces()
    {
        List<int> n = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        foreach (PlaceScript p in Places)
        {
            if (p.hasTech)
            {
                int r = RNG.Next(n.Count);
                p.TechID = n[r];
                n.RemoveAt(r);
            }
            p.setUnitSprites();
            p.HideUnits();
            p.UnitCount = new int[6];
            p.ShipCount = new int[2];
        }
        List<int> nums=new List<int> { };
        for (int i = 0; i < 24; i++)
            nums.Add(i);
        List<int> Ls = new List<int> { 0, 6, 7};
        for(int i=0;i<6;i++)
        {
            int r = RNG.Next(nums.Count);
            if (i < 3)
            {
                Places[nums[r]].UnitCount[0] = 20;
                Places[nums[r]].UnitCount[1] = 10;
                Places[nums[r]].UnitCount[2] = 1;
                int x = RNG.Next(Ls.Count);
                Places[nums[r]].LeaderID = Ls[x];
                Ls.RemoveAt(x);
                Places[nums[r]].ShipCount[0] = 6;
                Places[nums[r]].transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = GameControl.singleton.UnitSprites[2+ Places[nums[r]].LeaderID];
                print(Places[nums[r]].name);
            }
            else
            {
                Places[nums[r]].UnitCount[3] = 20;
                Places[nums[r]].UnitCount[4] = 10;
                Places[nums[r]].UnitCount[5] = 1;
                Places[nums[r]].ShipCount[1] = 6;
                print(Places[nums[r]].name);
            }
            nums.RemoveAt(r);
        }
    }

    public void UpdateResourceText()
    {
        for (int i = 0; i < 3; i++)
            ResourceText[i].text = Resources[ShipIndex][i].ToString();
    }

    public void GatherResource()
    {
        if (SelectedPlace.ResourceID < 3)
        {
            Resources[ShipIndex][SelectedPlace.ResourceID] +=RNG.Next(2, 8);
            SelectedPlace.ResourceID += RNG.Next(5, 10);
            UpdateResourceText();
            SetActionsvisible(false);
            CurrentState = GameState.PlayerTurn;
            if(PC.LivingLeads[1])
                PC.CheckWin();
        }
    }

    public void showTech()
    {
        CurrentState = GameState.FX;
        SetActionsvisible(false);
           SelectedPlace.HideResourceBuilding();
            SelectedPlace.HideUnits();
            SelectedPlace.CurrentBoard = Instantiate(TechMsgBoard, SelectedPlace.transform.position, Quaternion.identity) as GameObject;
            SelectedPlace.CurrentBoard.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = TechRef.Description(SelectedPlace.TechID);
        SelectedPlace.CurrentBoard.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = "BUY FOR $" + SelectedPlace.TechCost.ToString();
        SelectedPlace.CurrentBoard.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { BuyTech(); });
        SelectedPlace.CurrentBoard.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { CancelTechBuy(); });
        SelectedPlace.CurrentBoard.transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { CancelTechBuy(); });
    }

    public void ShowBattleTech()
    {
        if (PlayerTech.Count>0)
        {
            TechText[0].text = TechRef.Description(PlayerTech[TechIndex]);
            TechText[1].text = (TechIndex+1).ToString() + "/" + PlayerTech.Count.ToString();
        }
        else
        {
            TechText[0].text = "Buy tech at planets. Yes of course even that one.";
            TechText[1].text = "0/0";
        }
    }

    public void NextTech()
    {
        if (PlayerTech.Count > 1)
        {
            TechIndex++;
            if (TechIndex == PlayerTech.Count)
                TechIndex = 0;
            ShowBattleTech();
        }
    }

    public void PrevTech()
    {
        if (PlayerTech.Count > 1)
        {
            TechIndex--;
            if (TechIndex == -1)
                TechIndex = PlayerTech.Count-1;
        }
        else
        {
            TechIndex = 0;
        }
        ShowBattleTech();
    }

    public void UseTech()
    {
        if((SelectedPlace.ShipCount[0]>0 && SelectedPlace.ShipCount[1]>0 && ShipControl.singleton.inBattle)
            || (SelectedPlace.UnitCount[0]+ SelectedPlace.UnitCount[0] + SelectedPlace.UnitCount[0]>0 &&
            SelectedPlace.UnitCount[0] + SelectedPlace.UnitCount[0] + SelectedPlace.UnitCount[0]>0 &&
            BattleControl.singleton.phaseOne))
        {
            TechRef.Use(PlayerTech[TechIndex]);
            PlayerTech.RemoveAt(TechIndex);
            PrevTech();
        }
    }

    public void BuyTech()
    {
        if(Resources[ShipIndex][2]>=SelectedPlace.TechCost)
        {
            Resources[ShipIndex][2] -= SelectedPlace.TechCost;
            SelectedPlace.TechCost++;
            UpdateResourceText();
            PlayerTech.Add(SelectedPlace.TechID);
        }
    }

    public void CancelTechBuy()
    {
        SelectedPlace.ShowCurrentView();
        Destroy(SelectedPlace.CurrentBoard);
        CurrentState = GameState.PlayerTurn;
    }


    public void GainInfantry()
    {
        Resources[ShipIndex][1] -= InfantryCost[0];
        Resources[ShipIndex][2] -= InfantryCost[1];
        UpdateResourceText();
        int I = RNG.Next(2, 6);
        SelectedPlace.UnitCount[UnitIndex] += I;
        MsgText.text = "Gained " + I.ToString() + " units";
        if(CurrentView==ViewState.Unit)
            UpdateMapUnits();
    }

    public void BuildStuff()
    {
        if(SelectedPlace.BuildingID==0 && Resources[ShipIndex][1]>=MechCost[0] && Resources[ShipIndex][2] >= MechCost[1])
        {
            int I = RNG.Next(1, 5);
            Resources[ShipIndex][1] -= MechCost[0];
            Resources[ShipIndex][2] -= MechCost[1];
            SelectedPlace.UnitCount[UnitIndex + 1]+=I;
            if(!RegimePlayer)
                MsgText.text = "Gained " + I.ToString() + " Mechs";
            else
                MsgText.text = "Gained " + I.ToString() + " Tanks";
            SelectedPlace.BuildingID = 3;
        }
        else if(SelectedPlace.BuildingID==1 && Resources[ShipIndex][1] >= ShipCost[0] && Resources[ShipIndex][2] >= ShipCost[1])
        {
            int I = RNG.Next(1, 5);
            Resources[ShipIndex][1] -= ShipCost[0];
            Resources[ShipIndex][2] -= ShipCost[1];
            SelectedPlace.ShipCount[ShipIndex] += I;
            MsgText.text = "Gained " + I.ToString() + " Ships";
            SelectedPlace.BuildingID = 3;
        }
    }

    public void Intel()
    {
        Resources[ShipIndex][2] -= IntelCost;
        UpdateResourceText();
        if (!RegimePlayer)
        {
                MsgText.text = SelectedPlace.ShipCount[1].ToString() + " Ships, " + SelectedPlace.UnitCount[3].ToString() + " Conscripts, "
                    + SelectedPlace.UnitCount[4].ToString() + " Tanks, " + SelectedPlace.UnitCount[5].ToString() + " Elites";
        }
        else
        {
            string s = " Leaders";
            if (SelectedPlace.UnitCount[2] == 1)
            {
                if (SelectedPlace.LeaderID == 0)
                    s = " General";
                else if (SelectedPlace.LeaderID == 6)
                    s = " Merchant";
                else if (SelectedPlace.LeaderID == 7)
                    s = " Diplomat";
            }
            MsgText.text = SelectedPlace.ShipCount[0].ToString() + " Ships, " + SelectedPlace.UnitCount[0].ToString() + " Militia, "
               + SelectedPlace.UnitCount[1].ToString() + " Mechs, " + SelectedPlace.UnitCount[2].ToString() + " Leaders";
        }
    }

    public void Liberate()
    {
        for (int i = 0; i < 3; i++)
            Resources[ShipIndex][i] -= LiberationCost[i];
        UpdateResourceText();
        SetActionsvisible(false);
        CurrentState = GameState.PlayerTurn;
        SelectedPlace.SetSigil(RegimePlayer);
        if(!RegimePlayer)
            PC.CheckWin();
    }

    public void CloseMenu()
    {
        SetActionsvisible(false);
        CurrentState = GameState.PlayerTurn;
    }

    public void UpdateMapUnits()
    {
        CurrentView = ViewState.Unit;
        foreach (PlaceScript p in Places)
            p.ShowUnits();
    }

    public void HideAllUnits()
    {
        foreach (PlaceScript p in Places)
            p.HideUnits();
    }

    public void ShowBuildings()
    {
        CurrentView = ViewState.Building;
        foreach (PlaceScript p in Places)
            p.ShowBuilding();
    }

    public void ShowResources()
    {
        CurrentView = ViewState.Res;
        foreach (PlaceScript p in Places)
            p.ShowResource();
    }

    public void HideResources()
    {
        foreach (PlaceScript p in Places)
            p.HideResourceBuilding();
    }




    public void StartGameRegime()
    {
        RegimePlayer = true;
        StartGame();
    }

    public void SetActionsvisible(bool vis)
    {
        if (!vis)
        {
            PlayerActionPanel.transform.position = new Vector3(100, 100, -100);
            foreach (Button b in PlayerActionPanel.GetComponentsInChildren<Button>())
                b.interactable = false;
        }
        else
            PlayerActionPanel.transform.position = SelectedPlace.transform.GetChild(6).position;
    }

    public void ReturnToMap()
    {
        Camera.main.transform.position = new Vector3(0, 0, -10);
        UpdateMapUnits();
    }

    public void cancelMoveMode()
    {
        CurrentState = GameState.PlayerTurn;
        MsgText.text = "";
        CancelMove.transform.position = new Vector3(100, 100, 100);
    }

    public void MoveMode()
    {
        CurrentState = GameState.Move;
        SetActionsvisible(false);
        MsgText.text = "Select place to move from";
        CancelMove.transform.position = new Vector3(6.7f, -7.8f, 0);
    }
    public void MakeMove()
    {
        if (MoveSlider.transform.GetChild(0).GetChild(4).GetComponent<Slider>().value == 0)
        {
            MsgText.text = "Must Move at least 1 Ship";
        }
        else
        {
            if (SelectedPlace.HasPlayerLead() && MoveSlider.transform.GetChild(0).GetChild(5 + 2).GetComponent<Slider>().value == 1)
            {
                if (RegimePlayer)
                    MsgText.text = "Cannot stack Elites";
                else
                    MsgText.text = "Cannot stack Leaders";
            }
            else
            {
                Resources[ShipIndex][0] -= 1;
                UpdateResourceText();
                StartPlace.ShipCount[ShipIndex] -= (int)MoveSlider.transform.GetChild(0).GetChild(4).GetComponent<Slider>().value;
                SelectedPlace.ShipCount[ShipIndex] += (int)MoveSlider.transform.GetChild(0).GetChild(4).GetComponent<Slider>().value;
                for (int i = 0; i < 3; i++)
                {
                    StartPlace.UnitCount[UnitIndex + i] -= (int)MoveSlider.transform.GetChild(0).GetChild(5 + i).GetComponent<Slider>().value;
                    SelectedPlace.UnitCount[UnitIndex + i] += (int)MoveSlider.transform.GetChild(0).GetChild(5 + i).GetComponent<Slider>().value;
                }
                if (!RegimePlayer && SelectedPlace.HasPlayerLead() && !StartPlace.HasPlayerLead() && MoveSlider.transform.GetChild(0).GetChild(5 + 2).GetComponent<Slider>().value==1)
                {
                    SelectedPlace.LeaderID = StartPlace.LeaderID;
                    StartPlace.LeaderID = -1;
                    SelectedPlace.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = singleton.UnitSprites[2 + SelectedPlace.LeaderID];
                }

                SelectedPlace.SpaceBattleCheck(true);
                MoveSlider.transform.position = new Vector3(100, 100, -100);
                CurrentState = GameState.PlayerTurn;
                UpdateMapUnits();
                MsgText.text = "";
            }
        }
    }


    public void TakeBackGround(bool Player)
    {
        if (Player)
        {
            for (int i = UnitIndex; i < UnitIndex + 3; i++)
            {
                StartPlace.UnitCount[i] = SelectedPlace.UnitCount[i];
                SelectedPlace.UnitCount[i] = 0;
            }
            if (!RegimePlayer && SelectedPlace.UnitCount[2] == 1)
            {
                StartPlace.LeaderID = SelectedPlace.LeaderID;
                SelectedPlace.LeaderID = -1;
            }
        }
        else
        {
            for (int i = PC.UnitIndex; i < PC.UnitIndex + 3; i++)
            {
                StartPlace.UnitCount[i] = SelectedPlace.UnitCount[i];
                SelectedPlace.UnitCount[i] = 0;
            }
            if (RegimePlayer && SelectedPlace.UnitCount[2] == 1)
            {
                StartPlace.LeaderID = SelectedPlace.LeaderID;
                SelectedPlace.LeaderID = -1;
            }
        }
    }



    // Use this for initialization
    void Start () {

	}

    public void NextTurn()
    {
        if (CurrentState == GameState.PlayerTurn)
        {
            SetActionsvisible(false);
            CurrentState = GameState.FX;
            foreach (PlaceScript p in Places)
                p.Tick();
            PC.TakeTurn();
            Resources[ShipIndex][0]++;
            Resources[ShipIndex][1]++;
            Resources[ShipIndex][2]++;
            UpdateResourceText();
        }
    }

    public void SetMenuButtons()
    {
        allowedActions[0] = Resources[ShipIndex][0] > 0;
        allowedActions[1] = SelectedPlace.loyalRegime==RegimePlayer && SelectedPlace.HasPlayerLead()
            && Resources[ShipIndex][1]>=InfantryCost[0] && Resources[ShipIndex][2] >= InfantryCost[1];
        allowedActions[2] = SelectedPlace.loyalRegime == RegimePlayer && SelectedPlace.HasPlayerLead() && SelectedPlace.BuildingID<2;
        if (allowedActions[2] && SelectedPlace.BuildingID == 0)
            allowedActions[2] = Resources[ShipIndex][1] >= MechCost[0] && Resources[ShipIndex][2] >= MechCost[1];
        else if(allowedActions[2] && SelectedPlace.BuildingID == 1)
            allowedActions[2] = Resources[ShipIndex][1] >= ShipCost[0] && Resources[ShipIndex][2] >= ShipCost[1];
        allowedActions[3]= Resources[ShipIndex][2] > IntelCost;
        allowedActions[4] = SelectedPlace.loyalRegime != RegimePlayer && SelectedPlace.HasPlayerLead() && Resources[ShipIndex][0] >= LiberationCost[0] && Resources[ShipIndex][1] >= LiberationCost[1] && Resources[ShipIndex][2] >= LiberationCost[2];
        allowedActions[5] = SelectedPlace.UnitCount[UnitIndex] > 0 || SelectedPlace.UnitCount[UnitIndex + 1] > 0 || SelectedPlace.UnitCount[UnitIndex + 2] > 0;
        if (SelectedPlace.TechID >= 0)
        {
            if (RegimePlayer)
                allowedActions[6] = SelectedPlace.UnitCount[5] == 1;
            else
                allowedActions[6] = SelectedPlace.UnitCount[2] == 1;
        }
        else
            allowedActions[6] = false;

        for (int i = 0; i < 8; i++)
            PlayerActionPanel.transform.GetChild(0).GetChild(0).GetChild(i).GetComponent<Button>().interactable = allowedActions[i];
    }

    
	
	// Update is called once per frame
	void Update () {
        if(CurrentState==GameState.Menu)
        {
            if(Input.GetMouseButtonUp(0))
            {
                SetMenuButtons();
            }
        }
	}
}
