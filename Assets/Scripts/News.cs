using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class News : MonoBehaviour
{
    TextMeshProUGUI text;
    public float speed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
        text = gameObject.GetComponent<TextMeshProUGUI>();
    }

    public void RunHeadline(string headline) {
        text.text = headline;
        Vector3 pos = gameObject.transform.localPosition;
        pos.x = 1000;
        gameObject.transform.localPosition = pos;
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 pos = gameObject.transform.localPosition;
        pos.x -= speed * Time.deltaTime;
        gameObject.transform.localPosition = pos;
    }
}
