using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{
    [SerializeField] private Unit unit;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChange += UnitActionSystem_OnSelectedUnitChange;
        UpdadateVisual();
    }

    private void UnitActionSystem_OnSelectedUnitChange(object sender, EventArgs e)
    {
        UpdadateVisual();
    }

    private void UpdadateVisual()
    {
        meshRenderer.enabled = unit == UnitActionSystem.Instance.GetSelectedUnit();
    }

    private void OnDestroy()
    {
        UnitActionSystem.Instance.OnSelectedUnitChange -= UnitActionSystem_OnSelectedUnitChange;
    }
}
