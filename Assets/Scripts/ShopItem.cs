using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public TextMeshProUGUI costText;
    public Button btn;
    public float cost;
    private CashController cash;

    void Start()
    {
        cash = CashController.current;
        costText.text = "Cost: $" + cost;
    }

    // Update is called once per frame
    void Update()
    {
        if(cash.money < cost)
            btn.interactable = false;
        else
            btn.interactable = true;

    }

    public void BuyItem()
    {
        cash.Spend(cost);

    }
}
