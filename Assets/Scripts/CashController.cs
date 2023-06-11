using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashController : MonoBehaviour
{    
    public float money = 0.0f;

    [Header("Upgrades")]
    public int AutoPacer = 0;
    public int BrainInterface = 0;
    public int BeatModulator = 0;
    public int MalpracticeDefenseLawyers = 0;
    public int PerfluorocarbonSurogate = 0;
    public int IntravanousLubricant = 0;
    public int EmergencyReleaseValve = 0;
    public int RevisedTermsOfService = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private static CashController _current;

    // OnEnable and Awake can happen simultaneously, causing errors
    // This is a hotfix to make calling current Find this object if it is null
    // Was a major issue in Level 1
    // TODO: Actual fix, this was a hotfix by Brandon
    public static CashController current
    {
        get
        {
            if (_current == null) {
                _current = FindObjectOfType<CashController>();
            }
            return _current;
        }
        set => _current = value;
    }
}
