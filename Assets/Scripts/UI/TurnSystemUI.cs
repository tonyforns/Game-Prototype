using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour
{

    [SerializeField] private Button nextTurnButton;
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private Transform enemyTurnVisualTransform;


    private void Start()
    {
        TurnSystem.Instance.OnTurnsChanged += TurnSystem_OnNextChanged;
        nextTurnButton.onClick.AddListener(TurnSystem.Instance.NextTurn);

        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEnemyTurnButtonVisibility();
    }

    private void TurnSystem_OnNextChanged(object sender, System.EventArgs e)
    {
        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEnemyTurnButtonVisibility();
    }

    private void UpdateTurnText()
    {
        turnText.text = "Turn: " + TurnSystem.Instance.GetTurn();
    }

    private void UpdateEnemyTurnVisual()
    {
        enemyTurnVisualTransform.gameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn());
    }

    private void UpdateEnemyTurnButtonVisibility()
    {
        nextTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
    }

}
