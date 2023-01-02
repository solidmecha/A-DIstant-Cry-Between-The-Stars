using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BattleControl : MonoBehaviour {

    public static BattleControl singleton;
    public List<UnitScript> Units;
    public UnitScript activeUnit;
    public int[] UnitCounts;
    public Text[] CountText;
    public Text MsgText;
    public Button button;
    public bool phaseOne;


    private void Awake()
    {
        singleton = this;
    }


    // Use this for initialization
    void Start () {
    //SetUpBattle(new int[6] {10,1,0,1,0,0});

	}

    public void SetPlayerOwned()
    {
        foreach (UnitScript u in Units)
        {
            u.Def = 1;
            u.PlayerOwned = (u.ID > 2 && GameControl.singleton.RegimePlayer) || (u.ID < 3 && !GameControl.singleton.RegimePlayer);
            u.visible = true;
            u.transform.position = u.startPos;
            u.GetComponent<LineRenderer>().SetPosition(0, u.startPos);
            u.GetComponent<LineRenderer>().SetPosition(1, u.startPos);
            u.Target = null;
        }
    }


    public void SetUpBattle(string n)
    {
        SetPlayerOwned();
        MsgText.text = n + " Battle";
        Camera.main.transform.position = new Vector3(0, 21, -10);
        for (int i = 0; i < 6; i++)
            UnitCounts[i] = GameControl.singleton.SelectedPlace.UnitCount[i];
        ShowUnitCounts();
        foreach (UnitScript u in Units)
        {
            if (!u.PlayerOwned)
                BattleAI.singleton.AIunits.Add(u);
            else
                BattleAI.singleton.AvailableTargets.Add(u);
        }
        GameControl.singleton.TechText[0].transform.root.position = new Vector2(10f, 17.75f);
        GameControl.singleton.ShowBattleTech();
        BattleRound();

    }

    void BattleRound()
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(delegate { ShowRollsPhase(); });
        foreach (UnitScript u in Units)
        {
            if(u.visible)
                u.Roll();
        }
        BattleAI.singleton.SetTargets();
        phaseOne = true;
    }

    public void ShowRollsPhase()
    {
        phaseOne = false;
        foreach (UnitScript u in Units)
        {
            if (!u.PlayerOwned)
            {
                u.ShowTarget();
                u.ShowVals();
            }
        }
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(delegate { ResolveTargets(); });

    }

   public void ResolveTargets()
    {
        foreach (UnitScript u in Units)
        {
            if(u.visible)
                u.HitTarget();
        }
        CleanUp();
    }

    void CleanUp()
    {
       foreach(UnitScript u in Units)
        {
            if (u.visible && u.Def <= 0)
                UnitCounts[u.ID]--;
        }
        ShowUnitCounts();
        foreach (UnitScript u in Units)
        {
            u.Target = null;
            u.ShowTarget();
        }
        if (UnitCounts[0] == 0 && UnitCounts[1] == 0 && UnitCounts[2] == 0)
            EndBattle(true);
        else if (UnitCounts[3] == 0 && UnitCounts[4] == 0 && UnitCounts[5] == 0)
            EndBattle(false);
        else
        {
            BattleAI.singleton.CleanUp();
            BattleRound();
        }
    }

    void ShowUnitCounts()
    {
        for(int i=0;i<6;i++)
        {
            CountText[i].text = UnitCounts[i].ToString();
            if((i==0 || i==3) && UnitCounts[i]<5)
            {
                int c = 0;
                foreach(UnitScript u in Units)
                {
                    if(u.ID==i && u.visible)
                    {
                        c++;
                        if (c > UnitCounts[i])
                            u.ToggleView();
                    }
                }
            }
            else if((i == 1 || i == 4) && UnitCounts[i] < 4)
            {
                int c = 0;
                foreach (UnitScript u in Units)
                {
                    if (u.ID == i && u.visible)
                    {
                        c++;
                        if (c > UnitCounts[i])
                            u.ToggleView();
                    }
                }
            }
            else if((i == 2 || i == 5) && UnitCounts[i] < 1)
            {
                int c = 0;
                foreach (UnitScript u in Units)
                {
                    if (u.ID == i && u.visible)
                    {
                        c++;
                        if (c > UnitCounts[i])
                            u.ToggleView();
                    }
                }

            }
        }


    }

    public void EndBattle(bool RegimeWon)
    {
        
        if (RegimeWon)
            MsgText.text = "Regime Win!";
        else
            MsgText.text = "Revos Win!";
        BattleAI.singleton.End();
        bool b = GameControl.singleton.SelectedPlace.UnitCount[2]==1;
        for (int i = 0; i < 6; i++)
        {
            GameControl.singleton.SelectedPlace.UnitCount[i] = UnitCounts[i];
        }
        if (b && GameControl.singleton.SelectedPlace.UnitCount[2] ==0)
        {
            int a = GameControl.singleton.SelectedPlace.LeaderID;
            if (a > 0)
                a -= 5;
            GameControl.singleton.PC.LivingLeads[a] = false;
            GameControl.singleton.SelectedPlace.LeaderID = -1;
        }
        GameControl.singleton.PC.CheckWin();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(delegate { GameControl.singleton.ReturnToMap(); });
    }



    


	
	// Update is called once per frame
	void Update () {
	
	}
}
