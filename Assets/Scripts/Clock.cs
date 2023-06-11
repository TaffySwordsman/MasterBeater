using System;
using UnityEngine;
using TMPro;

public class Clock : MonoBehaviour
{
    TextMeshProUGUI text;
    
    void Start()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        text.text = System.DateTime.Now.ToString("HH:mm");
    }
}
