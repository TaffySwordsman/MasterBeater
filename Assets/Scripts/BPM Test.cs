using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BPMTest : MonoBehaviour
{
    TextMeshProUGUI text;
    // Start is called before the first frame update
    void Awake()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();

        // add listeners
        EventDispatch.current.OnNormalize += (f) => UpdateBPM(f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void UpdateBPM(float newBPM)
    {
        Debug.Log("Run");
        text.text = "BPM: " + newBPM;
    }
}
