using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCOrdering : MonoBehaviour
{
    public Order currentOrder;
    CashMachineBehaviour cashMachine;

    [SerializeField]
    GameObject triggers;

    [SerializeField]
    GameEventListener correct, incorrect;

    public TMP_Text dialogueTextBox;

    private void Awake()
    {
        cashMachine = CashMachineBehaviour.Instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentOrder = cashMachine.currentOrder;
        dialogueTextBox.text = currentOrder.orderDescription;

        correct.Event = currentOrder.stepsEvent[0];
        incorrect.Event = currentOrder.stepsEvent[3];

        triggers.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

        if (currentOrder.stepsEvent[0])
        {
            correct.Event = currentOrder.stepsEvent[0];
        }
        else if (currentOrder.stepsEvent[1])
        {
            correct.Event = currentOrder.stepsEvent[1];
        }
        else if (currentOrder.stepsEvent[2])
        {
            correct.Event = currentOrder.stepsEvent[2];
        }
    }

    public void CorrectStep()
    {
        dialogueTextBox.text = currentOrder.orderCorrect;
    }

    public void IncorrectStep()
    {
        dialogueTextBox.text = currentOrder.orderIncorrect;
    }


}
