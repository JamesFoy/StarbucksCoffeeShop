using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashMachineAnimations : MonoBehaviour
{
    Animator anim;

    public GameObject quantityCanvas, paymentCanvas;

    public enum ButtonCanvas { Menu, Quantity, FinalPayment };
    public ButtonCanvas currentButtonCanvas;
    private bool InMyState;

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
            CheckAnim();
        }
        else if (currentButtonCanvas == ButtonCanvas.Quantity)
        {
            anim.SetTrigger("CloseQuantity");
            CheckAnim();
        }
        else if (currentButtonCanvas == ButtonCanvas.FinalPayment)
        {
            anim.SetTrigger("ClosePayment");
            CheckAnim();
        }
    }

    IEnumerator CanvasChange()
    {
        yield return new WaitForSeconds(1.1f);
        gameObject.SetActive(false);
    }

    void CheckAnim()
    {
        StartCoroutine(CanvasChange());
    }

    private void OnDisable()
    {
        if (gameObject.name == "Menu")
        {
            quantityCanvas.SetActive(true);
        }
        else if (gameObject.name == "Quantity")
        {
            paymentCanvas.SetActive(true);
        }
        else if (gameObject.name == "FinalPayment")
        {
            return;
        }
    }
}
