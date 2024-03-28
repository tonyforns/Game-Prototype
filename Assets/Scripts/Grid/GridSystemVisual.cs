using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }

    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;
    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }
    public enum GridVisualType
    {
        White,
        Blue,
        Red,
        RedSoft,
        Yellow
    }

    private GridSystemVisualSingle[,] gridSystemVisualSinglesArray;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        gridSystemVisualSinglesArray = new GridSystemVisualSingle[LevelGrid.Instance.GetWidht(), LevelGrid.Instance.GetHeigt()];
        for (int x = 0; x < LevelGrid.Instance.GetWidht(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeigt(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform gridSystemVisualSingle = Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
                gridSystemVisualSinglesArray[x,z] = gridSystemVisualSingle.GetComponent<GridSystemVisualSingle>();
            }
        }
        HideAllGridPisition();

        UnitActionSystem.Instance.OnSelectedActionChange += UnitActionSystem_OnSelectedActionChange;
        LevelGrid.Instance.OnAnyUnitMoveGridPosition += LevelGrid_OnAnyUnitMoveGridPosition;

        UpdateGridVisual();
    }

    private void LevelGrid_OnAnyUnitMoveGridPosition(object sender, System.EventArgs e)
    {
        UpdateGridVisual();
    }

    private void UnitActionSystem_OnSelectedActionChange(object sender, System.EventArgs e)
    {
        UpdateGridVisual();
    }

    public void HideAllGridPisition()
    {
        foreach (GridSystemVisualSingle gridSystemVisualSingle in gridSystemVisualSinglesArray)
        {
            gridSystemVisualSingle.Hide();
        }
    }

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionsList = new List<GridPosition>();
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x,z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) continue;

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > range) continue;

                gridPositionsList.Add(testGridPosition);

            }
        }

        ShowGridPositionList(gridPositionsList, gridVisualType);
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType) 
    {
        Material material = GetGridVisualTypeMaterial(gridVisualType);
        foreach (GridPosition gridPosition in gridPositionList)
        {
            gridSystemVisualSinglesArray[gridPosition.x, gridPosition.z].Show(material);
        }
    }

    private void UpdateGridVisual()
    {
        HideAllGridPisition();

        BaseAction baseAction = UnitActionSystem.Instance.GetSelectedAction();
        Unit unit = UnitActionSystem.Instance.GetSelectedUnit();

        GridVisualType gridVisualType;
            
        switch (baseAction)
        {
            default:
            case MoveAction moveAction:
                gridVisualType = GridVisualType.White;
                break;
            case ShootAction shootAction:
                gridVisualType = GridVisualType.Red;

                ShowGridPositionRange(unit.GetGridPosition(), shootAction.GetMaxShootDistance(), GridVisualType.RedSoft);
                break;
        }
        if (!baseAction) return;
        ShowGridPositionList(baseAction.GetValidActionGridPositionList(), gridVisualType);
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        return gridVisualTypeMaterialList.First(gridVisualMateria => gridVisualMateria.gridVisualType == gridVisualType).material;
    }
}
