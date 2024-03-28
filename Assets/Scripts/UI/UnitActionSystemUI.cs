using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;
    [SerializeField] private TextMeshProUGUI actionPointText;
    
    private List<ActionButtonUI> actionButtonUIList;

    private void Awake()
    {
        actionButtonUIList = new List<ActionButtonUI>();
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChange += UnitActionSystem_OnSelectedUnitChange;
        UnitActionSystem.Instance.OnSelectedActionChange += UnitActionSystem_OnSelectedActionChange;
        UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;
        TurnSystem.Instance.OnTurnsChanged += TurnSystem_OnNextChanged;
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;


        CreateUnityActionButton();
        UpdateSeletedVisual();
    }


    private void UpdateSeletedVisual()
    {
        foreach (ActionButtonUI button in actionButtonUIList)
        {
            button.UpdateSelectedVisual();
        }    
    }

    private void CreateUnityActionButton()
    {
        Unit unit = UnitActionSystem.Instance.GetSelectedUnit();

        foreach (Transform actionButonTransform in actionButtonContainerTransform)
        {
            Destroy(actionButonTransform.gameObject);
        }

        actionButtonUIList.Clear();

        foreach (BaseAction baseAction in unit.GetBaseActionArray())
        {
             ActionButtonUI actionButtonUI = Instantiate(actionButtonPrefab, actionButtonContainerTransform)
                                                        .GetComponent<ActionButtonUI>();
            actionButtonUI.SetBaseAction(baseAction);

            actionButtonUIList.Add(actionButtonUI);
        }
    }

    private void UnitActionSystem_OnSelectedActionChange(object sender, EventArgs e)
    {
        UpdateSeletedVisual();
    }
    private void UnitActionSystem_OnSelectedUnitChange(object sender, EventArgs e)
    {
        CreateUnityActionButton();
        UpdateSeletedVisual();
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnActionStarted(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void TurnSystem_OnNextChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }
    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void UpdateActionPoints()
    {
        Unit selectedUnt = UnitActionSystem.Instance.GetSelectedUnit();
        actionPointText.text = "Action Point: " + selectedUnt.GetActionPoints();
    }
}
