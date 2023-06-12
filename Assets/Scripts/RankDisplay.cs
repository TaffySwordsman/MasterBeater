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
    public GameObject star1, star2, star3, upgradeItem;
    bool complete = false;
    bool upgraded = false;

    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();
    }

    public void Update()
    {
        if(star1.activeSelf && star2.activeSelf && star3.activeSelf && !upgraded)
        {
            complete = true;
            upgradeItem.SetActive(true);
        }
    }

    public void UpdateRank(string rank, string name)
    {
        if(upgraded)
            text.text = "<rainb><wave>Master Beater</wave></rainb>";
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
        if(upgraded)
            text.text = "<rainb><wave>Master Beater</wave></rainb>";
        else
        {
            if(name == "Civilian")
            text.text = heart1;
        
            if(name == "Patient")
                text.text = heart2;
            
            if(name == "Soldier")
                text.text = heart3;
        }
    }

    public void UpgradeBought() { upgraded = true; text.text = "<rainb><wave>Master Beater</wave></rainb>"; }
}
