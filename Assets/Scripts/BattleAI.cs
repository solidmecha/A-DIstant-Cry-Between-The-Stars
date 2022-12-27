using UnityEngine;
using System.Collections.Generic;

public class BattleAI : MonoBehaviour {
    public static BattleAI singleton;
    public List<UnitScript> AIunits;
    public List<UnitScript> AvailableTargets;
    public List<int> AssignedDamage;

    private void Awake()
    {
        singleton = this;
    }
    // Use this for initialization
    void Start () {
	
	}

    public void End()
    {
        AIunits.Clear();
        AvailableTargets.Clear();
        AssignedDamage.Clear();
    }

    public void CleanUp()
    {
        AssignedDamage.Clear();
        foreach (UnitScript u in AIunits)
        {
            u.ShowTarget();
        }
        
    }

    public void SetTargets()
    {
        foreach (UnitScript u in AvailableTargets)
            AssignedDamage.Add(0);
        foreach (UnitScript u in AIunits)
        {
            if(u.visible)
                LockOnTarget(u);
        }
    }

    public void LockOnTarget(UnitScript u)
    {
        for (int i = 0; i < AvailableTargets.Count; i++)
        {
            if (AvailableTargets[i].visible && AssignedDamage[i] < TargetDmg(AvailableTargets[i].ID) && AvailableTargets[i].ID != GameControl.singleton.UnitIndex+2)
            {
                u.Target = AvailableTargets[i];
                AssignedDamage[i] += u.Atk;
                break;
            }
        }
        if(u.Target==null)
        {
            u.Target = AvailableTargets[GameControl.singleton.RNG.Next(AvailableTargets.Count)];
        }
    }

    public int TargetDmg(int id)
    {
        switch(id)
        {
            case 1:
                return GameControl.singleton.RNG.Next(6, 9);
            case 2:
                return 20;
            case 3:
                return 6;
            case 4:
                return 9;
            case 5:
                return 24;
            default: return GameControl.singleton.RNG.Next(4,7);
        }
        
    }


	
	// Update is called once per frame
	void Update () {
	
	}
}
