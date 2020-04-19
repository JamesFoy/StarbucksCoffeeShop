using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using VRTK;
using UnityEngine.AI;
using VRTK.Controllables;
using VRTK.Examples;

public class NPCOrdering : MonoBehaviour
{
    public GameObject changeDropZone;

    public GameObject npcCanvas;

    public Order currentOrder;
    CashMachineBehaviour cashMachine;

    bool hasPaid;

    GameManager gameManager;
    Transform orderPoint;
    Transform spawnPoint;

    NavMeshAgent agent;

    Animator anim;

    public enum PaymentTypes { Cash, Card };
    public PaymentTypes paymentType;

    [SerializeField]
    GameObject triggers;

    [SerializeField]
    NPCGameEventListener correct, incorrect, reset;

    public TMP_Text dialogueTextBox;

    private void Start()
    {
        cashMachine = GameObject.FindGameObjectWithTag("CashRegister").GetComponent<CashMachineBehaviour>();
        npcCanvas.SetActive(false);
        gameManager = GameManager.Instance;
        orderPoint = gameManager.transform.Find("OrderPoint");
        spawnPoint = gameManager.transform.Find("SpawnPoint");
        anim = GetComponent<Animator>();
        paymentType = UnityEngine.Random.value < .5 ? PaymentTypes.Cash : PaymentTypes.Card;
        changeDropZone.SetActive(false);
        agent = GetComponent<NavMeshAgent>();

        GetOrder();
    }

    private void Update()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance != 0)
            {
                agent.speed = 1.4f;
                anim.SetBool("Walk", true);
                NpcCanvasActive(false);
            }
            else if (!agent.pathPending && !agent.hasPath)
            {
                agent.speed = 0;
                anim.SetBool("Walk", false);
                transform.rotation = Quaternion.Lerp(transform.rotation, orderPoint.rotation, 2 * Time.deltaTime);
                NpcCanvasActive(true);
            }
        }

        if (paymentType == PaymentTypes.Cash && cashMachine.paymentReady)
        {
            NpcCanvasActive(false);
            anim.SetTrigger("CashPayment");
            changeDropZone.SetActive(true);
        }
        else if (paymentType == PaymentTypes.Card && cashMachine.paymentReady && !hasPaid)
        {
            NpcCanvasActive(false);
            hasPaid = true;
            anim.SetTrigger("CardPayment");
            StartCoroutine(WaitTillAnimEnd(3.7f));
        }
    }

    void NpcCanvasActive(bool isActive)
    {
        npcCanvas.SetActive(isActive);
    }

    public void NpcLeave()
    {
        agent.SetDestination(spawnPoint.position);
        StartCoroutine(NpcLeavingCafe());
    }

    IEnumerator NpcLeavingCafe()
    {
        yield return new WaitForSeconds(3.5f);
        gameManager.SpawnNewNpc();
        cashMachine.GenerateOrder();
        Destroy(this.gameObject);
    }

    IEnumerator WaitTillAnimEnd(float animLength)
    {
        yield return new WaitForSeconds(animLength);
        NpcLeave();
        yield return null;
    }

    public void GetOrder()
    {
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
        cashMachine.GenerateOrder();
        GetOrder();
    }
}
