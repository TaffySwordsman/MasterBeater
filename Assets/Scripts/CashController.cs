using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=System.Random;

public class CashController : MonoBehaviour
{    
    public GameObject Crypto;
    public float money = 0.0f;
    public float btcPrice = 0f;
    public float btc = 0.0f;
    public float btcTickRate = 0.5f;
    public float btcEventFactor = 2f;
    private bool btcOn = false;
    public float taxRate = 35f;
    public float employeeTick = 10f;
    private float lastTick = 0f;
    private float lastBTCTick = 0f;
    private float lastBTCEvent = 0f;
    private Random rng;
    private AudioSource audioData;

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
        rng = new Random();
        audioData = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Employees
        float tickReduction = HRGeneralists * HRGeneralistsAffect + 
                              Recruiter * RecruiterAffect +
                              Pinkertons * PinkertonsAffect;
        if (Time.time - lastTick > employeeTick - tickReduction) {
            lastTick = Time.time;
            EarnWithTax(MarketingSpecialists * MarketingSpecialistsAffect +
                        CustomerService * CustomerServiceAffect +
                        SalesReps * SalesRepsAffect);
        }

        // BTC
        if (!btcOn && btc > 0f) {
            btcOn = true;
            Crypto.SetActive(true);
        }
        if (Time.time - lastBTCTick > btcTickRate) {
            lastBTCTick = Time.time;
            UpdateBTCPrice();
        }
    }
    
    public float NetTaxRate() {
        float fraudRate = Accountants * AccountantsAffect -
                          IRSInvestigators * IRSInvestigatorsAffect -
                          Senators * SenatorsAffect;
        return 1f - taxRate - fraudRate;
    }
    
    public float EarnBTC(float mined) {
        btc += mined;
        return mined;
        // return EarnWithTax(mined);
    }

    public void SellBTC() {
        EarnWithTax(btcPrice * btc);
        btc = 0;
    }

    public void UpdateBTCPrice() {
        float x = (Time.time - 50f) / 30;
        float m = 5.6f;
        float f = Mathf.Sin(x)+Mathf.Cos(2f*x-2f)-Mathf.Cos(x/2f-2f)+Mathf.Sin(2f*x+4f)+Mathf.Cos(3f*x)-Mathf.Sin(x/2f+5f);
        float g = m*Mathf.Sin(x/30f) - 2f*m*Mathf.Sin((x-60f)/40f) - 1.5f*m*Mathf.Cos((2f*x-20f)/35f) + 1.5f*m*Mathf.Cos(x/27.3f);
        float p = Mathf.Sin(8f*x-2f)+Mathf.Sin(9f*x-5f);

        double rand = rng.NextDouble();
        if (rand > 0.995) {
            UpdateEventFactor(true);
        } else if (rand < .005) {
            UpdateEventFactor(false);
        } else if (Time.time - lastBTCEvent > 6f) {
            btcEventFactor = 1f;
        }
        btcPrice = btcEventFactor * (f + g + p + 35f) * 25000f / 45f;
    }

    private void UpdateEventFactor(bool good) {
        lastBTCEvent = Time.time;
        if (good) {
            btcEventFactor = 2 * (float) rng.NextDouble();
        } else {
            btcEventFactor = (float) rng.NextDouble() / 2 + .5f;
        }
    }
    
    public float EarnWithTax(float income) {
        float postTax = NetTaxRate() * income;
        money += income;
        return income;
    }

    public bool Spend(float expense) {
        if (money >= expense) {
            audioData.Play();
            money -= expense;
            return true;
        }
        return false;
    }

    public void AddPacer() { AutoPacer += 1; }
    public void AddInterface() { BrainInterface += 1; }
    public void AddModulator() { BeatModulator += 1; }
    public void AddLawyers() { MalpracticeDefenseLawyers += 1; }
    public void AddSurrogate() { PerfluorocarbonSurogate += 1; }
    public void AddLube() { IntravanousLubricant += 1; }
    public void AddValve() { EmergencyReleaseValve += 1; }
    public void AddTerms() { RevisedTermsOfService += 1; }
    
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
