using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private const int ACTION_POINTS_MAX = 2;

    public static EventHandler OnAnyActionPointsChanged;
    public static EventHandler OnAnyUnitSpawned;
    public static EventHandler OnAnyUnitDead;

    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private BaseAction[] baseActionArray;

    [SerializeField] private int actionPoint = ACTION_POINTS_MAX;

    [SerializeField] private bool isEnemy;

    private GridPosition gridPosition;

    public void Awake()
    {
        baseActionArray = GetComponents<BaseAction>();
        healthSystem = GetComponent<HealthSystem>();
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        TurnSystem.Instance.OnTurnsChanged += TurnSystem_OnNextChanged;

        healthSystem.OnDead += HealthSystem_OnDead;

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

   

    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;
            LevelGrid.Instance.UnitMoveGridPosition(this, oldGridPosition, newGridPosition);
        }
    }
   

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public override string ToString()
    {
        return gameObject.name;
    }

    public BaseAction[] GetBaseActionArray()
    {
        return baseActionArray;
    }

    public bool CanSpendActionPointToTakeAction(BaseAction baseAction)
    {
        return baseAction.GetActionPointCost() <= actionPoint;
    }

    private void SpendActionPoints(int amout)
    {
        actionPoint -= amout;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    } 

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if(CanSpendActionPointToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointCost());
            return true;
        }
        return false;
    }

    public int GetActionPoints()
    {
        return actionPoint;
    }

    private void TurnSystem_OnNextChanged(object sender, System.EventArgs e)
    {
        if(IsInPlayerAndTurnPlayer() || !IsInPlayerAndTurnPlayer())
        {
            actionPoint = ACTION_POINTS_MAX;
            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public bool IsInPlayerAndTurnPlayer()
    {
        return !IsEnemy() && TurnSystem.Instance.IsPlayerTurn();
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }
    internal void Damage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);

        Destroy(gameObject);
    }

    public float GetHealthNormalized()
    {
        return healthSystem.GetHealthNormilized();
    }

    public T GetAction<T>() where T : BaseAction
    {
        foreach(BaseAction baseAction in baseActionArray)
        {
            if (baseAction is T)
            {
                return (T)baseAction;
            }
        }
        return null;
    }
}
