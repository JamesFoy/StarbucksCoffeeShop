using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class NPCOrdering : MonoBehaviour
{
    public Order currentOrder;
    CashMachineBehaviour cashMachine;

    Animator anim;

    public enum PaymentTypes { Cash, Card };
    public PaymentTypes paymentType;

    [SerializeField]
    GameObject triggers;

    [SerializeField]
    NPCGameEventListener correct, incorrect, reset;

    public TMP_Text dialogueTextBox;

    private void Awake()
    {
        cashMachine = CashMachineBehaviour.Instance;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        GetOrder();
        paymentType = UnityEngine.Random.value < .5 ? PaymentTypes.Cash : PaymentTypes.Card;
    }

    private void Update()
    {
        if (paymentType == PaymentTypes.Cash && cashMachine.paymentReady)
        {
            anim.SetTrigger("CashPayment");
        }
        else if (paymentType == PaymentTypes.Card && cashMachine.paymentReady)
        {
            anim.SetTrigger("CardPayment");
        }
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
        GetOrder();
    }
}
