using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashMachineAnimations : MonoBehaviour
{
    Animator anim;

    public enum ButtonCanvas { Menu, Quantity, FinalPayment };
    public ButtonCanvas currentButtonCanvas;

    void OnEnable()
    {
        anim = GetComponent<Animator>();

        if (currentButtonCanvas == ButtonCanvas.Menu)
        {
            anim.SetTrigger("OpenMenu");
        }
        else if (currentButtonCanvas == ButtonCanvas.Quantity)
        {
            anim.SetTrigger("OpenQuantity");
        }
        else if (currentButtonCanvas == ButtonCanvas.FinalPayment)
        {
            anim.SetTrigger("OpenPayment");
        }
    }

    public void PlayResetAnimation()
    {
        if (currentButtonCanvas == ButtonCanvas.Menu)
        {
            anim.SetTrigger("CloseMenu");
        }
        else if (currentButtonCanvas == ButtonCanvas.Quantity)
        {
            anim.SetTrigger("CloseQuantity");
        }
        else if (currentButtonCanvas == ButtonCanvas.FinalPayment)
        {
            anim.SetTrigger("ClosePayment");
        }
    }
}
