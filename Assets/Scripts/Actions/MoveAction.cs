using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveAction : BaseAction
{

    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private int maxMoveDistance;
    private void Awake()
    {
        unit = GetComponent<Unit>();
        targetPosition = transform.position;
    }


    private void Update()
    {
        float stopingDistance = .1f;
        if (transform.position == targetPosition) return;
        if (Vector3.Distance(targetPosition, transform.position) > stopingDistance)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            float moveSpeed = 4;
            transform.position += moveDirection * Time.deltaTime * moveSpeed;

            float rotateSpeed = 10;
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
        }
        else
        {
            OnStopMoving?.Invoke(this, EventArgs.Empty);
            transform.position = targetPosition;
            ActionComplete();
        }
    }

    public bool IsWalking()
    {
        return targetPosition != transform.position;
    }
    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);

        OnStartMoving?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList() 
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();
        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsettGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsettGridPosition;
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) continue;

                if (unitGridPosition == testGridPosition) continue;

                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) continue;

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Move";
    }

    public override EnemyAIAction GetBestEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10
        };
    }
}
