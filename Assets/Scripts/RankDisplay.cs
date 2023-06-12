using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RankDisplay : MonoBehaviour
{
    TextMeshProUGUI text;
    string heart1 = "Beat Novice";
    string heart2 = "Beat Patient";
    string heart3 = "Beat Recruit";
    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();
        
    }

    public void UpdateRank(string rank, string name)
    {
        if(rank == "Master Beater")
            text.text = "<rainb><wave>" + rank + "</wave></rainb>";
        else
            text.text = rank;

        if(name == "Civilian")
            heart1 = rank;
        
        if(name == "Patient")
            heart2 = rank;
        
        if(name == "Soldier")
            heart3 = rank;
    }

    public void SetCurrentRank(string name)
    {
        if(name == "Civilian")
            text.text = heart1;
        
        if(name == "Patient")
            text.text = heart2;
        
        if(name == "Soldier")
            text.text = heart3;
    }
}
