using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public HeartController heart;
    private Label reading; 

    // Start is called before the first frame update
    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        reading = root.Q<Label>("Reading");
    }

    // Update is called once per frame
    void Update()
    {
        reading.text = heart.systolic.ToString("0.") + " / " + heart.diastolic.ToString("0.");
    }
}
