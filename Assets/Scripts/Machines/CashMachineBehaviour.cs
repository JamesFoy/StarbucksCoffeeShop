using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;
using VRTK.Controllables;
using VRTK.Controllables.ArtificialBased;
using TMPro;

public class CashMachineBehaviour : MonoBehaviour
{
    public TMP_Text orderText;

    public GameEvent event1;
    public GameEvent event2;
    public GameEvent event3;
    public GameEvent eventIncorrect;

    public string orderName;

    #region OrderVaribles
    [BoxGroup("Possible Orders")]
    public List<Order> PossibleOrders;
    [BoxGroup("Current Order")]
    public Order currentOrder;
    [BoxGroup("Current Order")]
    public int orderAmount;

    [BoxGroup("Order Amounts")]
    public List<int> amounts;
    #endregion

    #region DrinkPusherSetups
    [BoxGroup("Drink Pusher Settings")]
    [SerializeField]
    bool showDrinkSettings;

    [BoxGroup("Drink Pusher Settings")]
    [ShowIf(nameof(showDrinkSettings))]
    public VRTK_ArtificialPusher FlatWhite, CaffeAmericano, Frappe, LatteMacchino, Cappuccino, Expresso, Macchito, IrishCoffee, LongBlack;

    [BoxGroup("Drink Pusher Settings")]
    [ShowIf(nameof(showDrinkSettings))]
    public UnityEvent FlatWhiteButtonActivated, CaffeAmericanoButtonActivated, FrappeButtonActivated, LatteMacchinoButtonActivated, CapuccinoButtonActivated, ExpressoButtonActivated, MacchitoButtonActivated, IrishCoffeeButtonActivated, LongBlackButtonActivated;
    #endregion

    #region AmountPusherSetups
    [BoxGroup("Amount Pusher Settings")]
    [SerializeField]
    bool showAmountSettings;

    [BoxGroup("Amount Pusher Settings")]
    [ShowIf(nameof(showAmountSettings))]
    public VRTK_ArtificialPusher x1, x2;

    [BoxGroup("Amount Pusher Settings")]
    [ShowIf(nameof(showAmountSettings))]
    public UnityEvent x1ButtonActivated, x2ButtonActivated;
    #endregion

    #region ConfirmPusherSetups
    [BoxGroup("Confirm Pusher Settings")]
    [SerializeField]
    bool showConfirmSettings;

    [BoxGroup("Confirm Pusher Settings")]
    [ShowIf(nameof(showConfirmSettings))]
    public VRTK_ArtificialPusher confirm, decline;

    [BoxGroup("Confirm Pusher Settings")]
    [ShowIf(nameof(showConfirmSettings))]
    public UnityEvent ConfirmButtonActivated, DeclineButtonActivated;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        currentOrder = PossibleOrders[UnityEngine.Random.Range(0, PossibleOrders.Count)];
        orderAmount = amounts[UnityEngine.Random.Range(0, amounts.Count)];

        orderName = currentOrder.orderName;
        Debug.Log("Current order is " + orderName + " with an amount of " + orderAmount);

        event1 = currentOrder.stepsEvent[0];
        event2 = currentOrder.stepsEvent[1];
        event3 = currentOrder.stepsEvent[2];
        eventIncorrect = currentOrder.stepsEvent[3];

        confirm.gameObject.GetComponent<GameEventListener>().Event = event3;
        decline.gameObject.GetComponent<GameEventListener>().Event = eventIncorrect;

        FlatWhite.gameObject.GetComponent<GameEventListener>().Event = eventIncorrect;
        CaffeAmericano.gameObject.GetComponent<GameEventListener>().Event = eventIncorrect;
        Frappe.gameObject.GetComponent<GameEventListener>().Event = eventIncorrect;
        LatteMacchino.gameObject.GetComponent<GameEventListener>().Event = eventIncorrect;
        Cappuccino.gameObject.GetComponent<GameEventListener>().Event = eventIncorrect;
        Expresso.gameObject.GetComponent<GameEventListener>().Event = eventIncorrect;
        Macchito.gameObject.GetComponent<GameEventListener>().Event = eventIncorrect;
        IrishCoffee.gameObject.GetComponent<GameEventListener>().Event = eventIncorrect;
        LongBlack.gameObject.GetComponent<GameEventListener>().Event = eventIncorrect;

        AmountSwitch();
        ButtonSwitch();
    }

    #region SwitchStatements
    void AmountSwitch()
    {
        switch (orderAmount)
        {
            case 1:
                x1.gameObject.GetComponent<GameEventListener>().Event = event2;
                x2.gameObject.GetComponent<GameEventListener>().Event = eventIncorrect;
                break;
            case 2:
                x2.gameObject.GetComponent<GameEventListener>().Event = event2;
                x1.gameObject.GetComponent<GameEventListener>().Event = eventIncorrect;
                break;
        }
    }

    void ButtonSwitch()
    {
        switch (orderName)
        {
            case "FlatWhite":
                FlatWhite.gameObject.GetComponent<GameEventListener>().Event = event1;
                break;
            case "CaffeAmericano":
                CaffeAmericano.gameObject.GetComponent<GameEventListener>().Event = event1;
                break;
            case "Frappe":
                Frappe.gameObject.GetComponent<GameEventListener>().Event = event1;
                break;
            case "LatteMacchiato":
                LatteMacchino.gameObject.GetComponent<GameEventListener>().Event = event1;
                break;
            case "Cappuccino":
                Cappuccino.gameObject.GetComponent<GameEventListener>().Event = event1;
                break;
            case "Expresso":
                Expresso.gameObject.GetComponent<GameEventListener>().Event = event1;
                break;
            case "Macchito":
                Macchito.gameObject.GetComponent<GameEventListener>().Event = event1;
                break;
            case "IrishCoffee":
                IrishCoffee.gameObject.GetComponent<GameEventListener>().Event = event1;
                break;
            case "LongBlack":
                LongBlack.gameObject.GetComponent<GameEventListener>().Event = event1;
                break;
        }
    }
    #endregion

    #region OnEnable/Disable
    void OnEnable()
    {
        FlatWhite.MaxLimitReached += ButtonFlatWhiteOnMaxLimitReached;
        CaffeAmericano.MaxLimitReached += ButtonCaffeeAmericanoOnMaxLimitReached;
        Frappe.MaxLimitReached += ButtonFrappeOnMaxLimitReached;
        LatteMacchino.MaxLimitReached += ButtonLatteMacchinoOnMaxLimitReached;
        Cappuccino.MaxLimitReached += ButtonCappuccinoOnMaxLimitReached;
        Expresso.MaxLimitReached += ButtonExpressoOnMaxLimitReached;
        Macchito.MaxLimitReached += ButtonMacchitoOnMaxLimitReached;
        IrishCoffee.MaxLimitReached += ButtonIrishCoffeeOnMaxLimitReached;
        LongBlack.MaxLimitReached += ButtonLongBlackOnMaxLimitReached;

        x1.MaxLimitReached += Buttonx1OnMaxLimitReached;
        x2.MaxLimitReached += Buttonx2OnMaxLimitReached;
    }

    void OnDisable()
    {
        FlatWhite.MaxLimitReached -= ButtonFlatWhiteOnMaxLimitReached;
        CaffeAmericano.MaxLimitReached -= ButtonCaffeeAmericanoOnMaxLimitReached;
        Frappe.MaxLimitReached -= ButtonFrappeOnMaxLimitReached;
        LatteMacchino.MaxLimitReached -= ButtonLatteMacchinoOnMaxLimitReached;
        Cappuccino.MaxLimitReached -= ButtonCappuccinoOnMaxLimitReached;
        Expresso.MaxLimitReached -= ButtonExpressoOnMaxLimitReached;
        Macchito.MaxLimitReached -= ButtonMacchitoOnMaxLimitReached;
        IrishCoffee.MaxLimitReached -= ButtonIrishCoffeeOnMaxLimitReached;
        LongBlack.MaxLimitReached -= ButtonLongBlackOnMaxLimitReached;

        x1.MaxLimitReached -= Buttonx1OnMaxLimitReached;
        x2.MaxLimitReached -= Buttonx2OnMaxLimitReached;
    }
    #endregion

    #region UnityEventTriggers
    private void ButtonFlatWhiteOnMaxLimitReached(object sender, ControllableEventArgs e)
    {
        FlatWhiteButtonActivated.Invoke();
        if (orderName == "FlatWhite")
        {
            event1.Raise();
        }
        else
        {
            eventIncorrect.Raise();
        }
    }

    private void ButtonCaffeeAmericanoOnMaxLimitReached(object sender, ControllableEventArgs e)
    {
        CaffeAmericanoButtonActivated.Invoke();
        if (orderName == "CaffeAmericano")
        {
            event1.Raise();
        }
        else
        {
            eventIncorrect.Raise();
        }
    }

    private void ButtonFrappeOnMaxLimitReached(object sender, ControllableEventArgs e)
    {
        FrappeButtonActivated.Invoke();
        if (orderName == "Frappe")
        {
            event1.Raise();
        }
        else
        {
            eventIncorrect.Raise();
        }
    }

    private void ButtonLatteMacchinoOnMaxLimitReached(object sender, ControllableEventArgs e)
    {
        LatteMacchinoButtonActivated.Invoke();
        if (orderName == "LatteMacchino")
        {
            event1.Raise();
        }
        else
        {
            eventIncorrect.Raise();
        }
    }

    private void ButtonCappuccinoOnMaxLimitReached(object sender, ControllableEventArgs e)
    {
        CapuccinoButtonActivated.Invoke();
        if (orderName == "Capuccino")
        {
            event1.Raise();
        }
        else
        {
            eventIncorrect.Raise();
        }
    }

    private void ButtonExpressoOnMaxLimitReached(object sender, ControllableEventArgs e)
    {
        ExpressoButtonActivated.Invoke();
        if (orderName == "Expresso")
        {
            event1.Raise();
        }
        else
        {
            eventIncorrect.Raise();
        }
    }

    private void ButtonMacchitoOnMaxLimitReached(object sender, ControllableEventArgs e)
    {
        MacchitoButtonActivated.Invoke();
        if (orderName == "Macchito")
        {
            event1.Raise();
        }
        else
        {
            eventIncorrect.Raise();
        }
    }

    private void ButtonIrishCoffeeOnMaxLimitReached(object sender, ControllableEventArgs e)
    {
        IrishCoffeeButtonActivated.Invoke();
        if (orderName == "IrishCoffee")
        {
            event1.Raise();
        }
        else
        {
            eventIncorrect.Raise();
        }
    }

    private void ButtonLongBlackOnMaxLimitReached(object sender, ControllableEventArgs e)
    {
        LongBlackButtonActivated.Invoke();
        if (orderName == "LongBlack")
        {
            event1.Raise();
        }
        else
        {
            eventIncorrect.Raise();
        }
    }

    private void Buttonx1OnMaxLimitReached(object sender, ControllableEventArgs e)
    {
        x1ButtonActivated.Invoke();
        if (orderAmount == 1)
        {
            event2.Raise();
        }
        else
        {
            eventIncorrect.Raise();
        }
    }

    private void Buttonx2OnMaxLimitReached(object sender, ControllableEventArgs e)
    {
        x2ButtonActivated.Invoke();
        if (orderAmount == 2)
        {
            event2.Raise();
        }
        else
        {
            eventIncorrect.Raise();
        }
    }
    #endregion
}