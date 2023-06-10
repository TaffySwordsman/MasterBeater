using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class UIController : MonoBehaviour
{
    public HeartController heart;
    public TextMeshProUGUI reading; 
    public TextMeshProUGUI cash; 

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        reading.text = heart.systolic.ToString("0.") + " / " + heart.diastolic.ToString("0.");
        cash.text = "$" + heart.money.ToString("0.00");
    }
}
