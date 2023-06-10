using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public HeartController heart;
    private Label reading; 
    private Label cash; 

    // Start is called before the first frame update
    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        reading = root.Q<Label>("Reading");
        cash = root.Q<Label>("Cash");
    }

    // Update is called once per frame
    void Update()
    {
        reading.text = heart.systolic.ToString("0.") + " / " + heart.diastolic.ToString("0.");
        cash.text = "$" + heart.money.ToString("0.00");
    }
}
