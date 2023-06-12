using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RankDisplay : MonoBehaviour
{
    TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();
        
    }

    public void UpdateRank(string rank)
    {
        text.text = "<rainb><wave>" + rank + "</wave></rainb>";
    }
}
