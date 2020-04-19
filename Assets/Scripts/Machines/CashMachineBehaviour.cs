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
    #region Singleton Setup
    private static CashMachineBehaviour instance = null;

    public static CashMachineBehaviour Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        GenerateOrder();
    }
    #endregion

    #region Enums
    public enum CoffeeChoice { FlatWhite, CaffeAmericano, Frappe, LatteMacchiato, Cappuccino, Expresso, Macchito, IrishCoffee, LongBlack };
    public CoffeeChoice choice;

    public enum AmountChoice { x1, x2 };
    public AmountChoice amountChoice;
    #endregion

    #region Canvas Setup
    [BoxGroup("UI Stuff")]
    public TMP_Text orderPaymentText, orderPriceText;
    int finalCost;
    float price;
    int amountChosen;
    string coffeeName;
    #endregion

    #region Order Debug Info
    [BoxGroup("Order Info")]
    [SerializeField]
    bool showOrderInfo;

    [BoxGroup("Order Info")]
    [ShowIf(nameof(showOrderInfo))]
    public TMP_Text orderText;

    [BoxGroup("Order Info")]
    [ShowIf(nameof(showOrderInfo))]
    public GameEvent event1, event2, event3, eventIncorrect;

    [BoxGroup("Order Info")]
    [ShowIf(nameof(showOrderInfo))]
    public string orderName;
    #endregion

    #region OrderVaribles
    [BoxGroup("Possible Orders")]
    public List<Order> PossibleOrders;
    [BoxGroup("Current Order")]
    public Order currentOrder;
    [BoxGroup("Current Order")]
    public int orderAmount;
    [BoxGroup("CurrentOrder")]
    public bool paymentReady;

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

    #region Start/Update
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        #region Enums setting amount variables
        if (amountChoice == AmountChoice.x1)
        {
            amountChosen = 1;
        }
        else if (amountChoice == AmountChoice.x2)
        {
            amountChosen = 2;
        }
        #endregion

        #region Enums setting coffee string variables
        if (choice == CoffeeChoice.FlatWhite)
        {
            coffeeName = "Flat White";
            price = 2.50f;
        }
        else if (choice == CoffeeChoice.CaffeAmericano)
        {
            coffeeName = "Caffe Americano";
            price = 2.45f;
        }
        else if (choice == CoffeeChoice.Frappe)
        {
            coffeeName = "Frappe";
        }
        else if (choice == CoffeeChoice.LatteMacchiato)
        {
            coffeeName = "Latte Macchiato";
            price = 2.85f;
        }
        else if (choice == CoffeeChoice.Cappuccino)
        {
            coffeeName = "Cappuccino";
            price = 2.85f;
        }
        else if (choice == CoffeeChoice.Expresso)
        {
            coffeeName = "Expresso";
            price = 1.90f;
        }
        else if (choice == CoffeeChoice.Macchito)
        {
            coffeeName = "Macchito";
            price = 2.50f;
        }
        else if (choice == CoffeeChoice.IrishCoffee)
        {
            coffeeName = "Irish Coffe";
            price = 7.80f;
        }
        else if (choice == CoffeeChoice.LongBlack)
        {
            coffeeName = "Long Black";
            price = 2.73f;
        }
        #endregion

        orderPaymentText.text = " You chose x" + amountChosen + " : " + coffeeName;
        orderPriceText.text = "Payment Required = Total: £" + finalCost;
    }
    #endregion

    #region HelperFunctions
    public void CostPrice(int amount)
    {
        finalCost = Mathf.RoundToInt(amount * price);
        Debug.Log(price * amount);
    }

    public void GenerateOrder()
    {
        paymentReady = false;

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
    #endregion

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

        confirm.MaxLimitReached += ButtonConfirmOnMaxLimitReached;
        decline.MaxLimitReached += ButtonDeclineOnMaxLimitReached;
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

        confirm.MaxLimitReached -= ButtonConfirmOnMaxLimitReached;
        decline.MaxLimitReached -= ButtonDeclineOnMaxLimitReached;
    }
    #endregion

    #region UnityEventTriggers
    private void ButtonFlatWhiteOnMaxLimitReached(object sender, ControllableEventArgs e)
    {
        FlatWhiteButtonActivated.Invoke();
        choice = CoffeeChoice.FlatWhite;

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
        choice = CoffeeChoice.CaffeAmericano;

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
        choice = CoffeeChoice.Frappe;

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
        choice = CoffeeChoice.LatteMacchiato;

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
        choice = CoffeeChoice.Cappuccino;

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
        choice = CoffeeChoice.Expresso;

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
        choice = CoffeeChoice.Macchito;

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
        choice = CoffeeChoice.IrishCoffee;

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
        choice = CoffeeChoice.LongBlack;

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
        amountChoice = AmountChoice.x1;
        CostPrice(amountChosen);

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
        amountChoice = AmountChoice.x2;
        CostPrice(amountChosen);

        if (orderAmount == 2)
        {
            event2.Raise();
        }
        else
        {
            eventIncorrect.Raise();
        }
    }

    private void ButtonConfirmOnMaxLimitReached(object sender, ControllableEventArgs e)
    {
        ConfirmButtonActivated.Invoke();
        event3.Raise();
        paymentReady = true;
    }

    private void ButtonDeclineOnMaxLimitReached(object sender, ControllableEventArgs e)
    {
        DeclineButtonActivated.Invoke();
        event3.Raise();
        paymentReady = false;
    }
    #endregion
}