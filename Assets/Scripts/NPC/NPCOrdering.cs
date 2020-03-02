using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class NPCOrdering : MonoBehaviour
{
    public Order currentOrder;
    CashMachineBehaviour cashMachine;

    [SerializeField]
    GameObject triggers;

    [SerializeField]
    NPCGameEventListener correct, incorrect, reset;

    public TMP_Text dialogueTextBox;

    private void Awake()
    {
        cashMachine = CashMachineBehaviour.Instance;
    }

    private void Update()
    {
        GetOrder();
    }

    private void GetOrder()
    {
        currentOrder = cashMachine.currentOrder;

        currentOrder = cashMachine.currentOrder;

        dialogueTextBox.text = currentOrder.orderDescription + " x" + cashMachine.orderAmount;

        correct.Event = currentOrder.stepsEvent[0];
        incorrect.Event = currentOrder.stepsEvent[3];
        reset.Event = currentOrder.stepsEvent[2];

        triggers.SetActive(true);
    }

    public void CorrectStep()
    {
        dialogueTextBox.text = currentOrder.orderCorrect;
    }

    public void IncorrectStep()
    {
        dialogueTextBox.text = currentOrder.orderIncorrect;
        correct.Event = currentOrder.stepsEvent[1];
    }

    public void Reset()
    {
        dialogueTextBox.text = currentOrder.orderDescription;
        correct.Event = currentOrder.stepsEvent[0];
        incorrect.Event = currentOrder.stepsEvent[3];
    }
}
