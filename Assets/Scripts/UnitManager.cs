using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance {  get; private set; }
     
    private List<Unit> unitList;
    private List<Unit> friendlyUnitList;
    private List<Unit> enemyList;


    private void Awake()
    {
        if(Instance != null )
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        unitList = new List<Unit>();
        friendlyUnitList = new List<Unit>();
        enemyList = new List<Unit>();
    }

    private void Start()
    {
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDead += Unit_OnyUnitDead;
    }

    private void Unit_OnyUnitDead(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;
        if(unit.IsEnemy())
        {
            enemyList.Remove(unit);
        } else
        {
            friendlyUnitList.Remove(unit);
        }

        unitList.Remove(unit);
    }

    private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;
        if (unit.IsEnemy())
        {
            enemyList.Add(unit);
        }
        else
        {
            friendlyUnitList.Add(unit);
        }

        unitList.Add(unit);
    }

    public List<Unit> GetUnitList()
    {
        return unitList;
    }

    public List<Unit> GetEnemyUnitList()
    {
        return enemyList; 
    }

    public List<Unit> GetFriendlyUnitList()
    {
        return friendlyUnitList;
    }
}
