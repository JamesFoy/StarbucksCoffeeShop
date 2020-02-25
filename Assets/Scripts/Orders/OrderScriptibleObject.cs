using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Order", order = 1)]
public class Order : ScriptableObject
{
    [BoxGroup("Order Name")]
    public string orderName;

    [BoxGroup("Order Talking")]
    public string orderDescription;
    [BoxGroup("Order Talking")]
    public string orderCorrect;
    [BoxGroup("Order Talking")]
    public string orderIncorrect;

    [BoxGroup("Event Steps")]
    public List<GameEvent> stepsEvent;
}
