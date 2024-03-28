using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private float timer;
    private State state;
    private enum State
    {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy
    }

    private void Awake()
    {
        state = State.WaitingForEnemyTurn;
    }
    private void Start()
    {
        TurnSystem.Instance.OnTurnsChanged += TurnsSystem_OnTurnsChanged;
    }

    private void TurnsSystem_OnTurnsChanged(object sender, System.EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            Debug.Log("entre");
            state = State.TakingTurn;
            timer = 2f;
        }
    }

    void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn()) return;

        switch (state)
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer < 0f)
                {
                    state = State.Busy;
                    if(TryTakeEnemyAIAction(SetStateTakingTurn))
                    {
                        state = State.Busy;
                    } else
                    {
                        TurnSystem.Instance.NextTurn();
                    }
                }
                break;
            case State.Busy:
                break;
        }
    }

    private void SetStateTakingTurn()
    {
        timer = 0.5f;
        state = State.TakingTurn;
    }

    private bool TryTakeEnemyAIAction(Action onEnemyActionComplete)
    {
        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
        {
            if (TryTakeEnemyAIAction(enemyUnit, onEnemyActionComplete))
            {
                return true;
            }
        }

        return false;
    }

    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyActionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestAction = null;

        foreach (BaseAction action in enemyUnit.GetBaseActionArray())
        {
            if(!enemyUnit.CanSpendActionPointToTakeAction(action)) 
            {
                continue;
            }

            if (bestEnemyAIAction == null)
            {
                bestEnemyAIAction = action.GetBestEnemyAIAction();
                bestAction = action;
            } else
            {
                EnemyAIAction testEnemyAction = action.GetBestEnemyAIAction();
                if(testEnemyAction != null && testEnemyAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = testEnemyAction;
                    bestAction = action;
                }
            }
        }

        if(bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestAction))
        {
            bestAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyActionComplete);
            return true;
        }
        return false;
    }


}
