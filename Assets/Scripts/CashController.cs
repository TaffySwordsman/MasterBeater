using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashController : MonoBehaviour
{    
    public float money = 0.0f;
    public float taxRate = 35f;
    public float employeeTick = 10f;
    private float lastTick = 0f;

    [Header("Upgrades")]
    public int AutoPacer = 0;
    public int BrainInterface = 0;
    public int BeatModulator = 0;
    public int MalpracticeDefenseLawyers = 0;
    public int PerfluorocarbonSurogate = 0;
    public int IntravanousLubricant = 0;
    public int EmergencyReleaseValve = 0;
    public int RevisedTermsOfService = 0;

    [Header("Employees")]
    public int MarketingSpecialists = 0;
    public int CustomerService = 0;
    public int SalesReps = 0;
    public int Accountants = 0;
    public int IRSInvestigators = 0;
    public int Senators = 0;
    public int Recruiter = 0;
    public int HRGeneralists = 0;
    public int Pinkertons = 0;

    [Header("Employee Effects")]
    public float MarketingSpecialistsAffect = 1f;
    public float CustomerServiceAffect = 5f;
    public float SalesRepsAffect = 20f;
    public float AccountantsAffect = 0.5f;
    public float IRSInvestigatorsAffect = 3f;
    public float SenatorsAffect = 10f;
    public float RecruiterAffect = 0.025f;
    public float HRGeneralistsAffect = 0.25f;
    public float PinkertonsAffect = 2.5f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float tickReduction = HRGeneralists * HRGeneralistsAffect + 
                              Recruiter * RecruiterAffect +
                              Pinkertons * PinkertonsAffect;
        if (Time.time - lastTick > employeeTick - tickReduction) {
            lastTick = Time.time;
            EarnWithTax(MarketingSpecialists * MarketingSpecialistsAffect +
                        CustomerService * CustomerServiceAffect +
                        SalesReps * SalesRepsAffect);
            Debug.Log("Tick");
        }
    }
    
    public float NetTaxRate() {
        float fraudRate = Accountants * AccountantsAffect -
                          IRSInvestigators * IRSInvestigatorsAffect -
                          Senators * SenatorsAffect;
        return 1f - taxRate - fraudRate;
    }
    
    public float EarnBTC(float mined) {
        return EarnWithTax(mined);
    }
    
    public float EarnWithTax(float income) {
        float postTax = NetTaxRate() * income;
        money += income;
        return income;
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
